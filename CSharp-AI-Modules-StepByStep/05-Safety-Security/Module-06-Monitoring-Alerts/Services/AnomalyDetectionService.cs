using Module_06_Monitoring_Alerts.Models;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service for detecting anomalies in metrics and events
/// </summary>
public class AnomalyDetectionService
{
    private readonly MLContext _mlContext;
    private readonly Dictionary<string, BaselineMetrics> _baselines = new();
    private readonly Dictionary<string, List<double>> _historicalData = new();
    private const int MinSamplesForBaseline = 20;
    private const double ZScoreThreshold = 3.0;
    private const double IqrMultiplier = 1.5;

    public AnomalyDetectionService()
    {
        _mlContext = new MLContext(seed: 0);
    }

    /// <summary>
    /// Detect anomalies using statistical methods
    /// </summary>
    public async Task<AnomalyDetectionResult> DetectAnomalyAsync(string metricName, double value)
    {
        await Task.CompletedTask;

        // Add to historical data
        if (!_historicalData.ContainsKey(metricName))
            _historicalData[metricName] = new List<double>();

        _historicalData[metricName].Add(value);

        // Keep only recent data (last 1000 points)
        if (_historicalData[metricName].Count > 1000)
            _historicalData[metricName].RemoveAt(0);

        // Update baseline if enough data
        if (_historicalData[metricName].Count >= MinSamplesForBaseline)
            _baselines[metricName] = CalculateBaseline(metricName);

        // Check if baseline exists
        if (!_baselines.ContainsKey(metricName))
        {
            return new AnomalyDetectionResult
            {
                IsAnomaly = false,
                AnomalyScore = 0,
                Confidence = 0,
                MetricName = metricName,
                ObservedValue = value,
                ExpectedValue = value,
                Deviation = 0,
                Type = AnomalyType.Outlier,
                Explanation = "Insufficient data for baseline"
            };
        }

        var baseline = _baselines[metricName];

        // Z-Score method
        double zScore = Math.Abs(value - baseline.Mean) / baseline.StandardDeviation;
        bool isAnomalyZScore = zScore > ZScoreThreshold;

        // IQR method
        var q1 = CalculatePercentile(_historicalData[metricName], 25);
        var q3 = CalculatePercentile(_historicalData[metricName], 75);
        var iqr = q3 - q1;
        var lowerBound = q1 - (IqrMultiplier * iqr);
        var upperBound = q3 + (IqrMultiplier * iqr);
        bool isAnomalyIqr = value < lowerBound || value > upperBound;

        // Combine methods
        bool isAnomaly = isAnomalyZScore || isAnomalyIqr;
        double anomalyScore = Math.Min(zScore / ZScoreThreshold * 100, 100);
        double confidence = isAnomaly ? Math.Min(anomalyScore / 100.0, 1.0) : 0;

        // Determine anomaly type
        var anomalyType = DetermineAnomalyType(value, baseline.Mean, _historicalData[metricName]);

        return new AnomalyDetectionResult
        {
            IsAnomaly = isAnomaly,
            AnomalyScore = anomalyScore,
            Confidence = confidence,
            MetricName = metricName,
            ObservedValue = value,
            ExpectedValue = baseline.Mean,
            Deviation = value - baseline.Mean,
            Type = anomalyType,
            Explanation = GenerateExplanation(isAnomaly, zScore, anomalyType)
        };
    }

    /// <summary>
    /// Detect spike anomalies using ML.NET
    /// </summary>
    public async Task<List<AnomalyDetectionResult>> DetectSpikesAsync(string metricName, List<double> values, int confidence = 95, int pValueHistoryLength = 30)
    {
        await Task.CompletedTask;

        if (values.Count < 10)
            return new List<AnomalyDetectionResult>();

        // Prepare data
        var dataPoints = values.Select((v, i) => new MetricDataPoint { Value = v }).ToList();
        var dataView = _mlContext.Data.LoadFromEnumerable(dataPoints);

        // Configure spike detection
        var pipeline = _mlContext.Transforms.DetectIidSpike(
            outputColumnName: nameof(SpikePrediction.Prediction),
            inputColumnName: nameof(MetricDataPoint.Value),
            confidence: confidence,
            pvalueHistoryLength: pValueHistoryLength);

        // Transform data
        var transformedData = pipeline.Fit(dataView).Transform(dataView);
        var predictions = _mlContext.Data.CreateEnumerable<SpikePrediction>(transformedData, reuseRowObject: false).ToList();

        // Convert to results
        var results = new List<AnomalyDetectionResult>();
        for (int i = 0; i < predictions.Count; i++)
        {
            if (predictions[i].Prediction[0] == 1) // Spike detected
            {
                results.Add(new AnomalyDetectionResult
                {
                    IsAnomaly = true,
                    AnomalyScore = predictions[i].Prediction[1] * 100, // Score
                    Confidence = predictions[i].Prediction[2], // P-Value
                    MetricName = metricName,
                    ObservedValue = values[i],
                    ExpectedValue = i > 0 ? values[i - 1] : values[i],
                    Deviation = i > 0 ? values[i] - values[i - 1] : 0,
                    Type = AnomalyType.Spike,
                    Explanation = $"ML.NET detected spike at position {i}"
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Detect change points using ML.NET
    /// </summary>
    public async Task<List<AnomalyDetectionResult>> DetectChangePointsAsync(string metricName, List<double> values, int confidence = 95, int changeHistoryLength = 20)
    {
        await Task.CompletedTask;

        if (values.Count < 10)
            return new List<AnomalyDetectionResult>();

        // Prepare data
        var dataPoints = values.Select((v, i) => new MetricDataPoint { Value = v }).ToList();
        var dataView = _mlContext.Data.LoadFromEnumerable(dataPoints);

        // Configure change point detection
        var pipeline = _mlContext.Transforms.DetectIidChangePoint(
            outputColumnName: nameof(ChangePointPrediction.Prediction),
            inputColumnName: nameof(MetricDataPoint.Value),
            confidence: confidence,
            changeHistoryLength: changeHistoryLength);

        // Transform data
        var transformedData = pipeline.Fit(dataView).Transform(dataView);
        var predictions = _mlContext.Data.CreateEnumerable<ChangePointPrediction>(transformedData, reuseRowObject: false).ToList();

        // Convert to results
        var results = new List<AnomalyDetectionResult>();
        for (int i = 0; i < predictions.Count; i++)
        {
            if (predictions[i].Prediction[0] == 1) // Change point detected
            {
                results.Add(new AnomalyDetectionResult
                {
                    IsAnomaly = true,
                    AnomalyScore = predictions[i].Prediction[1] * 100, // Score
                    Confidence = predictions[i].Prediction[2], // P-Value
                    MetricName = metricName,
                    ObservedValue = values[i],
                    ExpectedValue = i > 0 ? values[i - 1] : values[i],
                    Deviation = i > 0 ? values[i] - values[i - 1] : 0,
                    Type = AnomalyType.Pattern,
                    Explanation = $"ML.NET detected change point at position {i} (martingale: {predictions[i].Prediction[3]:F2})"
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Get baseline metrics for a metric name
    /// </summary>
    public async Task<BaselineMetrics?> GetBaselineAsync(string metricName)
    {
        await Task.CompletedTask;
        return _baselines.TryGetValue(metricName, out var baseline) ? baseline : null;
    }

    /// <summary>
    /// Update baseline for a metric
    /// </summary>
    public async Task UpdateBaselineAsync(string metricName)
    {
        await Task.CompletedTask;

        if (_historicalData.ContainsKey(metricName) && _historicalData[metricName].Count >= MinSamplesForBaseline)
        {
            _baselines[metricName] = CalculateBaseline(metricName);
        }
    }

    /// <summary>
    /// Clear historical data for a metric
    /// </summary>
    public async Task ClearHistoricalDataAsync(string metricName)
    {
        await Task.CompletedTask;

        _historicalData.Remove(metricName);
        _baselines.Remove(metricName);
    }

    /// <summary>
    /// Get all tracked metrics
    /// </summary>
    public async Task<List<string>> GetTrackedMetricsAsync()
    {
        await Task.CompletedTask;
        return _historicalData.Keys.ToList();
    }

    // Helper methods

    private BaselineMetrics CalculateBaseline(string metricName)
    {
        var data = _historicalData[metricName];
        var sortedData = data.OrderBy(x => x).ToList();

        return new BaselineMetrics
        {
            MetricName = metricName,
            Mean = data.Average(),
            Median = CalculatePercentile(sortedData, 50),
            StandardDeviation = CalculateStandardDeviation(data),
            Min = sortedData.First(),
            Max = sortedData.Last(),
            SampleSize = data.Count,
            CalculatedAt = DateTime.UtcNow
        };
    }

    private double CalculateStandardDeviation(List<double> values)
    {
        if (values.Count < 2)
            return 0;

        double mean = values.Average();
        double sumOfSquares = values.Sum(v => Math.Pow(v - mean, 2));
        return Math.Sqrt(sumOfSquares / (values.Count - 1));
    }

    private double CalculatePercentile(List<double> sortedValues, int percentile)
    {
        if (sortedValues.Count == 0)
            return 0;

        int index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
        index = Math.Max(0, Math.Min(index, sortedValues.Count - 1));
        return sortedValues[index];
    }

    private AnomalyType DetermineAnomalyType(double value, double mean, List<double> history)
    {
        // Check for spike (sudden increase)
        if (value > mean * 2 && history.Count > 1 && value > history[^2] * 1.5)
            return AnomalyType.Spike;

        // Check for drop (sudden decrease)
        if (value < mean * 0.5 && history.Count > 1 && value < history[^2] * 0.5)
            return AnomalyType.Drop;

        // Check for trend
        if (history.Count >= 5)
        {
            var recentTrend = history.TakeLast(5).ToList();
            bool increasing = true, decreasing = true;

            for (int i = 1; i < recentTrend.Count; i++)
            {
                if (recentTrend[i] <= recentTrend[i - 1]) increasing = false;
                if (recentTrend[i] >= recentTrend[i - 1]) decreasing = false;
            }

            if (increasing) return AnomalyType.Trend;
            if (decreasing) return AnomalyType.Trend;
        }

        return AnomalyType.Outlier;
    }

    private string GenerateExplanation(bool isAnomaly, double zScore, AnomalyType type)
    {
        if (!isAnomaly)
            return "Value is within normal range";

        return type switch
        {
            AnomalyType.Spike => $"Sudden spike detected (z-score: {zScore:F2})",
            AnomalyType.Drop => $"Sudden drop detected (z-score: {zScore:F2})",
            AnomalyType.Trend => $"Abnormal trend detected (z-score: {zScore:F2})",
            AnomalyType.Outlier => $"Outlier detected (z-score: {zScore:F2})",
            _ => $"Anomaly detected (z-score: {zScore:F2})"
        };
    }

    // ML.NET model classes
    private class MetricDataPoint
    {
        public float Value { get; set; }
    }

    private class SpikePrediction
    {
        [Microsoft.ML.Data.VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }

    private class ChangePointPrediction
    {
        [Microsoft.ML.Data.VectorType(4)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }
}
