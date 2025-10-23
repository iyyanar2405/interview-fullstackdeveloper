using AI.DataQuality.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace AI.DataQuality.Services;

public interface IAnomalyDetectionService
{
    Task<AnomalyDetectionResult> DetectAnomaliesAsync(AnomalyDetectionRequest request);
    Task<List<Anomaly>> DetectOutliersAsync(List<double> values, string columnName, double threshold = 3.0);
    Task<List<Anomaly>> DetectMissingPatternsAsync(List<Dictionary<string, object>> data);
    Task<List<Anomaly>> DetectDuplicatesAsync(List<Dictionary<string, object>> data);
}

public class AnomalyDetectionService : IAnomalyDetectionService
{
    private readonly ILogger<AnomalyDetectionService> _logger;
    private readonly MLContext _mlContext;

    public AnomalyDetectionService(ILogger<AnomalyDetectionService> logger)
    {
        _logger = logger;
        _mlContext = new MLContext(seed: 0);
    }

    public async Task<AnomalyDetectionResult> DetectAnomaliesAsync(AnomalyDetectionRequest request)
    {
        var result = new AnomalyDetectionResult
        {
            DatasetName = request.DatasetName,
            TotalRecords = request.Data.Count
        };

        try
        {
            // Detect outliers in numeric columns
            foreach (var columnName in request.NumericColumns)
            {
                var values = request.Data
                    .Select(row => row.ContainsKey(columnName) ? Convert.ToDouble(row[columnName]) : 0)
                    .ToList();

                var outliers = await DetectOutliersAsync(values, columnName);
                result.Anomalies.AddRange(outliers);
            }

            // Detect missing patterns
            var missingAnomalies = await DetectMissingPatternsAsync(request.Data);
            result.Anomalies.AddRange(missingAnomalies);

            // Detect duplicates
            var duplicates = await DetectDuplicatesAsync(request.Data);
            result.Anomalies.AddRange(duplicates);

            result.AnomaliesDetected = result.Anomalies.Count;
            result.AnomalyPercentage = request.Data.Count > 0 
                ? (double)result.AnomaliesDetected / request.Data.Count * 100 
                : 0;

            _logger.LogInformation($"Detected {result.AnomaliesDetected} anomalies in '{request.DatasetName}'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error detecting anomalies in '{request.DatasetName}'");
        }

        return result;
    }

    public async Task<List<Anomaly>> DetectOutliersAsync(
        List<double> values,
        string columnName,
        double threshold = 3.0)
    {
        var anomalies = new List<Anomaly>();

        try
        {
            if (values.Count < 3)
            {
                return anomalies;
            }

            // Calculate mean and standard deviation
            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
            var stdDev = Math.Sqrt(variance);

            // Detect outliers using Z-score method
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                var zScore = stdDev > 0 ? Math.Abs((value - mean) / stdDev) : 0;

                if (zScore > threshold)
                {
                    anomalies.Add(new Anomaly
                    {
                        Type = AnomalyType.Outlier,
                        RowNumber = i + 1,
                        ColumnName = columnName,
                        Value = value,
                        ExpectedValue = mean,
                        AnomalyScore = zScore,
                        Description = $"Value {value:F2} is {zScore:F2} standard deviations from mean {mean:F2}"
                    });
                }
            }

            _logger.LogDebug($"Detected {anomalies.Count} outliers in column '{columnName}'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error detecting outliers in column '{columnName}'");
        }

        return await Task.FromResult(anomalies);
    }

    public async Task<List<Anomaly>> DetectMissingPatternsAsync(List<Dictionary<string, object>> data)
    {
        var anomalies = new List<Anomaly>();

        try
        {
            if (data.Count == 0)
            {
                return anomalies;
            }

            var columns = data.First().Keys.ToList();

            foreach (var column in columns)
            {
                // Track missing value patterns
                var missingIndices = new List<int>();

                for (int i = 0; i < data.Count; i++)
                {
                    if (!data[i].ContainsKey(column) || data[i][column] == null)
                    {
                        missingIndices.Add(i);
                    }
                }

                // Check for patterns in missing data
                if (missingIndices.Count > 0)
                {
                    var missingPercentage = (double)missingIndices.Count / data.Count * 100;

                    if (missingPercentage > 10) // More than 10% missing
                    {
                        anomalies.Add(new Anomaly
                        {
                            Type = AnomalyType.Missing,
                            ColumnName = column,
                            AnomalyScore = missingPercentage,
                            Description = $"Column has {missingPercentage:F2}% missing values ({missingIndices.Count} out of {data.Count})"
                        });
                    }

                    // Check for consecutive missing values
                    var consecutiveCount = 0;
                    var maxConsecutive = 0;

                    for (int i = 1; i < missingIndices.Count; i++)
                    {
                        if (missingIndices[i] == missingIndices[i - 1] + 1)
                        {
                            consecutiveCount++;
                            maxConsecutive = Math.Max(maxConsecutive, consecutiveCount);
                        }
                        else
                        {
                            consecutiveCount = 0;
                        }
                    }

                    if (maxConsecutive > 5)
                    {
                        anomalies.Add(new Anomaly
                        {
                            Type = AnomalyType.Pattern,
                            ColumnName = column,
                            AnomalyScore = maxConsecutive,
                            Description = $"Found {maxConsecutive} consecutive missing values"
                        });
                    }
                }
            }

            _logger.LogDebug($"Detected {anomalies.Count} missing pattern anomalies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting missing patterns");
        }

        return await Task.FromResult(anomalies);
    }

    public async Task<List<Anomaly>> DetectDuplicatesAsync(List<Dictionary<string, object>> data)
    {
        var anomalies = new List<Anomaly>();

        try
        {
            if (data.Count == 0)
            {
                return anomalies;
            }

            // Create hash for each row
            var rowHashes = new Dictionary<string, List<int>>();

            for (int i = 0; i < data.Count; i++)
            {
                var row = data[i];
                var hash = string.Join("|", row.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}:{kvp.Value}"));

                if (!rowHashes.ContainsKey(hash))
                {
                    rowHashes[hash] = new List<int>();
                }
                rowHashes[hash].Add(i);
            }

            // Find duplicates
            foreach (var entry in rowHashes.Where(kvp => kvp.Value.Count > 1))
            {
                anomalies.Add(new Anomaly
                {
                    Type = AnomalyType.Duplicate,
                    RowNumber = entry.Value.First() + 1,
                    AnomalyScore = entry.Value.Count,
                    Description = $"Duplicate row found {entry.Value.Count} times at rows: {string.Join(", ", entry.Value.Select(idx => idx + 1))}"
                });
            }

            _logger.LogDebug($"Detected {anomalies.Count} duplicate anomalies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting duplicates");
        }

        return await Task.FromResult(anomalies);
    }

    // ML.NET-based time series anomaly detection
    public class TimeSeriesData
    {
        public float Value { get; set; }
    }

    public class TimeSeriesPrediction
    {
        [VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }

    public async Task<List<Anomaly>> DetectTimeSeriesAnomaliesAsync(
        List<double> timeSeries,
        string columnName,
        int windowSize = 100)
    {
        var anomalies = new List<Anomaly>();

        try
        {
            // Convert to IDataView
            var data = timeSeries.Select(v => new TimeSeriesData { Value = (float)v }).ToList();
            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            // Create anomaly detection pipeline
            var pipeline = _mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(TimeSeriesPrediction.Prediction),
                inputColumnName: nameof(TimeSeriesData.Value),
                confidence: 95,
                pvalueHistoryLength: windowSize / 2);

            // Train model
            var model = pipeline.Fit(dataView);
            var transformedData = model.Transform(dataView);

            // Get predictions
            var predictions = _mlContext.Data.CreateEnumerable<TimeSeriesPrediction>(transformedData, reuseRowObject: false).ToList();

            for (int i = 0; i < predictions.Count; i++)
            {
                var prediction = predictions[i].Prediction;
                if (prediction[0] == 1) // Anomaly detected
                {
                    anomalies.Add(new Anomaly
                    {
                        Type = AnomalyType.Statistical,
                        RowNumber = i + 1,
                        ColumnName = columnName,
                        Value = timeSeries[i],
                        AnomalyScore = prediction[2], // P-value
                        Description = $"Time series anomaly detected using ML.NET"
                    });
                }
            }

            _logger.LogInformation($"Detected {anomalies.Count} time series anomalies in '{columnName}'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error detecting time series anomalies in '{columnName}'");
        }

        return await Task.FromResult(anomalies);
    }
}
