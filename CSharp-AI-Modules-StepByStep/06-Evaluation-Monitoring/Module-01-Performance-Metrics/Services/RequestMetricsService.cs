using System.Collections.Concurrent;
using Module_01_Performance_Metrics.Models;

namespace Module_01_Performance_Metrics.Services;

public sealed class RequestMetricsService
{
    private readonly ConcurrentQueue<RequestMetric> _metrics = new();
    private readonly int _maxEntries;
    private int _activeRequests;

    public RequestMetricsService(int maxEntries = 5000)
    {
        _maxEntries = Math.Max(1000, maxEntries);
    }

    public void IncrementActiveRequests() => Interlocked.Increment(ref _activeRequests);

    public void DecrementActiveRequests() => Interlocked.Decrement(ref _activeRequests);

    public int GetActiveRequests() => Volatile.Read(ref _activeRequests);

    public void RecordRequestMetric(RequestMetric metric)
    {
        _metrics.Enqueue(metric);
        while (_metrics.Count > _maxEntries && _metrics.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyCollection<RequestMetric> GetRecent(int count)
    {
        return _metrics.Reverse().Take(count).ToArray();
    }

    public ThroughputSnapshot GetThroughput(TimeSpan window)
    {
        var cutoff = DateTimeOffset.UtcNow - window;
        var windowMetrics = _metrics.Where(m => m.StartedAt >= cutoff).ToList();
        var seconds = Math.Max(window.TotalSeconds, 1);

        return new ThroughputSnapshot
        {
            Window = window,
            TotalRequests = windowMetrics.Count,
            RequestsPerSecond = windowMetrics.Count / seconds,
            RequestsPerMinute = windowMetrics.Count / (seconds / 60),
            RequestsPerHour = windowMetrics.Count / (seconds / 3600)
        };
    }

    public RequestMetricSummary GetSummary(TimeSpan window)
    {
        var cutoff = DateTimeOffset.UtcNow - window;
        var windowMetrics = _metrics.Where(m => m.StartedAt >= cutoff).ToList();
        if (windowMetrics.Count == 0)
        {
            return new RequestMetricSummary
            {
                Window = window,
                TotalRequests = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                AverageLatencyMs = 0,
                P50LatencyMs = 0,
                P90LatencyMs = 0,
                P95LatencyMs = 0,
                P99LatencyMs = 0,
                MaxLatencyMs = 0,
                MinLatencyMs = 0,
                AverageRequestSizeKb = 0,
                AverageResponseSizeKb = 0,
                StatusCodeGroups = new Dictionary<string, int>()
            };
        }

        var latencies = windowMetrics.Select(m => m.Duration.TotalMilliseconds).OrderBy(v => v).ToArray();
        var requestSizes = windowMetrics.Select(m => (double)m.RequestBytes / 1024).ToArray();
        var responseSizes = windowMetrics.Where(m => m.ResponseBytes.HasValue).Select(m => (double)m.ResponseBytes.Value / 1024).ToArray();
        var successCount = windowMetrics.Count(m => m.IsSuccess);
        var statusGroups = windowMetrics
            .GroupBy(m => (m.StatusCode / 100) * 100)
            .ToDictionary(g => $"{g.Key}s", g => g.Count());

        return new RequestMetricSummary
        {
            Window = window,
            TotalRequests = windowMetrics.Count,
            SuccessCount = successCount,
            ErrorCount = windowMetrics.Count - successCount,
            AverageLatencyMs = latencies.Average(),
            P50LatencyMs = Percentile(latencies, 0.50),
            P90LatencyMs = Percentile(latencies, 0.90),
            P95LatencyMs = Percentile(latencies, 0.95),
            P99LatencyMs = Percentile(latencies, 0.99),
            MaxLatencyMs = latencies.Max(),
            MinLatencyMs = latencies.Min(),
            AverageRequestSizeKb = requestSizes.Length == 0 ? 0 : requestSizes.Average(),
            AverageResponseSizeKb = responseSizes.Length == 0 ? null : responseSizes.Average(),
            StatusCodeGroups = statusGroups,
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }

    private static double Percentile(IReadOnlyList<double> sortedValues, double percentile)
    {
        if (sortedValues.Count == 0)
        {
            return 0;
        }

        var index = percentile * (sortedValues.Count - 1);
        var lowerIndex = (int)Math.Floor(index);
        var upperIndex = (int)Math.Ceiling(index);

        if (lowerIndex == upperIndex)
        {
            return sortedValues[lowerIndex];
        }

        var lowerValue = sortedValues[lowerIndex];
        var upperValue = sortedValues[upperIndex];
        var fraction = index - lowerIndex;
        return lowerValue + (upperValue - lowerValue) * fraction;
    }
}
