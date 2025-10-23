using AI.DataQuality.Models;
using MathNet.Numerics.Statistics;

namespace AI.DataQuality.Services;

public interface IDataProfilingService
{
    Task<DataProfile> ProfileDatasetAsync(List<Dictionary<string, object>> data, string datasetName);
    Task<ColumnProfile> ProfileColumnAsync(List<object?> columnData, string columnName);
    Task<DataQualityMetrics> CalculateQualityMetricsAsync(List<Dictionary<string, object>> data, string datasetName);
}

public class DataProfilingService : IDataProfilingService
{
    private readonly ILogger<DataProfilingService> _logger;

    public DataProfilingService(ILogger<DataProfilingService> logger)
    {
        _logger = logger;
    }

    public async Task<DataProfile> ProfileDatasetAsync(List<Dictionary<string, object>> data, string datasetName)
    {
        var profile = new DataProfile
        {
            DatasetName = datasetName,
            ProfileType = ProfileType.Dataset,
            TotalRecords = data.Count,
            TotalColumns = data.FirstOrDefault()?.Count ?? 0
        };

        try
        {
            if (data.Count == 0)
            {
                _logger.LogWarning("Cannot profile empty dataset");
                return profile;
            }

            var columns = data.First().Keys.ToList();

            foreach (var columnName in columns)
            {
                var columnData = data.Select(row => row.ContainsKey(columnName) ? row[columnName] : null).ToList();
                var columnProfile = await ProfileColumnAsync(columnData, columnName);
                profile.Columns.Add(columnProfile);
            }

            // Calculate dataset-level statistics
            profile.Statistics["TotalSize"] = data.Count;
            profile.Statistics["AverageNullPercentage"] = profile.Columns.Average(c => c.NullPercentage);
            profile.Statistics["ColumnsWithNulls"] = profile.Columns.Count(c => c.NullCount > 0);
            profile.Statistics["TotalDistinctValues"] = profile.Columns.Sum(c => c.DistinctCount);

            _logger.LogInformation($"Profiled dataset '{datasetName}': {profile.TotalRecords} records, {profile.TotalColumns} columns");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error profiling dataset '{datasetName}'");
        }

        return profile;
    }

    public async Task<ColumnProfile> ProfileColumnAsync(List<object?> columnData, string columnName)
    {
        var profile = new ColumnProfile
        {
            ColumnName = columnName,
            TotalValues = columnData.Count
        };

        try
        {
            // Count nulls
            profile.NullCount = columnData.Count(v => v == null);
            profile.NullPercentage = profile.TotalValues > 0 
                ? (double)profile.NullCount / profile.TotalValues * 100 
                : 0;

            var nonNullValues = columnData.Where(v => v != null).ToList();

            if (nonNullValues.Count == 0)
            {
                profile.DataType = DataType.Unknown;
                return profile;
            }

            // Infer data type
            profile.DataType = InferDataType(nonNullValues.First()!);

            // Calculate distinct count
            var distinctValues = nonNullValues.Distinct().ToList();
            profile.DistinctCount = distinctValues.Count;
            profile.UniquenessRatio = (double)profile.DistinctCount / profile.TotalValues;

            // Find most common value
            var valueGroups = nonNullValues.GroupBy(v => v?.ToString() ?? "").OrderByDescending(g => g.Count()).ToList();
            if (valueGroups.Any())
            {
                profile.MostCommonValue = valueGroups.First().Key;
                profile.MostCommonValueCount = valueGroups.First().Count();
            }

            // Sample values
            profile.SampleValues = nonNullValues.Take(10).Select(v => v?.ToString() ?? "").ToList();

            // Value distribution (for categorical data)
            if (profile.DistinctCount <= 50)
            {
                profile.ValueDistribution = nonNullValues
                    .GroupBy(v => v?.ToString() ?? "")
                    .ToDictionary(g => g.Key, g => g.Count());
            }

            // Numeric statistics
            if (profile.DataType == DataType.Integer || profile.DataType == DataType.Decimal)
            {
                var numericValues = nonNullValues
                    .Select(v => Convert.ToDouble(v))
                    .Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
                    .ToList();

                if (numericValues.Any())
                {
                    profile.MinValue = numericValues.Min();
                    profile.MaxValue = numericValues.Max();
                    profile.Mean = numericValues.Average();
                    profile.Median = numericValues.Median();
                    profile.StandardDeviation = numericValues.StandardDeviation();
                }
            }

            // String statistics
            if (profile.DataType == DataType.String)
            {
                var stringValues = nonNullValues.Select(v => v?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (stringValues.Any())
                {
                    profile.MinValue = stringValues.Min(s => s.Length);
                    profile.MaxValue = stringValues.Max(s => s.Length);
                    profile.Mean = stringValues.Average(s => s.Length);
                }
            }

            // DateTime statistics
            if (profile.DataType == DataType.DateTime)
            {
                var dateValues = nonNullValues.OfType<DateTime>().ToList();
                if (dateValues.Any())
                {
                    profile.MinValue = dateValues.Min();
                    profile.MaxValue = dateValues.Max();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error profiling column '{columnName}'");
        }

        return await Task.FromResult(profile);
    }

    public async Task<DataQualityMetrics> CalculateQualityMetricsAsync(
        List<Dictionary<string, object>> data,
        string datasetName)
    {
        var metrics = new DataQualityMetrics
        {
            DatasetName = datasetName,
            TotalRecords = data.Count
        };

        try
        {
            if (data.Count == 0)
            {
                return metrics;
            }

            // Calculate completeness
            var totalCells = data.Count * data.First().Keys.Count;
            var nullCells = 0;

            foreach (var row in data)
            {
                nullCells += row.Values.Count(v => v == null);
            }

            metrics.CompletenessPercentage = totalCells > 0 
                ? (double)(totalCells - nullCells) / totalCells * 100 
                : 0;

            // Calculate uniqueness (based on duplicate rows)
            var uniqueRows = data.Select(row => string.Join("|", row.Values.Select(v => v?.ToString() ?? ""))).Distinct().Count();
            metrics.UniquenessPercentage = data.Count > 0 
                ? (double)uniqueRows / data.Count * 100 
                : 0;

            // Simple validity check (non-null values)
            metrics.ValidRecords = data.Count(row => row.Values.All(v => v != null));
            metrics.InvalidRecords = data.Count - metrics.ValidRecords;
            metrics.AccuracyPercentage = data.Count > 0 
                ? (double)metrics.ValidRecords / data.Count * 100 
                : 0;

            // Consistency (assume consistent if no nulls and proper types)
            metrics.ConsistencyPercentage = metrics.CompletenessPercentage;

            // Calculate dimension scores
            metrics.DimensionScores[DataQualityDimension.Completeness] = metrics.CompletenessPercentage;
            metrics.DimensionScores[DataQualityDimension.Accuracy] = metrics.AccuracyPercentage;
            metrics.DimensionScores[DataQualityDimension.Consistency] = metrics.ConsistencyPercentage;
            metrics.DimensionScores[DataQualityDimension.Uniqueness] = metrics.UniquenessPercentage;
            metrics.DimensionScores[DataQualityDimension.Validity] = metrics.AccuracyPercentage;

            // Calculate overall score
            metrics.OverallScore = metrics.DimensionScores.Values.Average();

            // Identify issues
            if (metrics.CompletenessPercentage < 100)
            {
                metrics.Issues.Add($"Completeness: {100 - metrics.CompletenessPercentage:F2}% of data is missing");
            }

            if (metrics.UniquenessPercentage < 100)
            {
                metrics.Issues.Add($"Uniqueness: {100 - metrics.UniquenessPercentage:F2}% of records are duplicates");
            }

            if (metrics.AccuracyPercentage < 100)
            {
                metrics.Issues.Add($"Accuracy: {metrics.InvalidRecords} records have invalid data");
            }

            _logger.LogInformation($"Quality metrics for '{datasetName}': Overall Score = {metrics.OverallScore:F2}%");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calculating quality metrics for '{datasetName}'");
        }

        return await Task.FromResult(metrics);
    }

    private DataType InferDataType(object value)
    {
        return value switch
        {
            int or long => DataType.Integer,
            float or double or decimal => DataType.Decimal,
            bool => DataType.Boolean,
            DateTime or DateTimeOffset => DataType.DateTime,
            string => DataType.String,
            _ => DataType.Unknown
        };
    }
}
