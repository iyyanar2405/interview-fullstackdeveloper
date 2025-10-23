using Module_06_Monitoring_Alerts.Models;
using Prometheus;
using System.Collections.Concurrent;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service for tracking metrics and exposing Prometheus-compatible counters
/// </summary>
public class MetricsService
{
    private readonly ConcurrentDictionary<string, Counter> _counters = new();
    private readonly ConcurrentDictionary<string, Gauge> _gauges = new();
    private readonly ConcurrentDictionary<string, Histogram> _histograms = new();
    private readonly ConcurrentDictionary<string, List<MetricData>> _metricHistory = new();
    private const int MaxHistoryEntries = 5000;

    /// <summary>
    /// Increment a counter metric
    /// </summary>
    public Task IncrementCounterAsync(string name, double increment = 1, params KeyValuePair<string, string>[] labels)
    {
        var counter = _counters.GetOrAdd(name, key => Metrics.CreateCounter(key, key, labels.Select(l => l.Key).ToArray()));
        counter.WithLabels(labels.Select(l => l.Value).ToArray()).Inc(increment);

        TrackMetric(name, MetricType.Counter, counter.WithLabels(labels.Select(l => l.Value).ToArray()).Value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Set a gauge metric
    /// </summary>
    public Task SetGaugeAsync(string name, double value, params KeyValuePair<string, string>[] labels)
    {
        var gauge = _gauges.GetOrAdd(name, key => Metrics.CreateGauge(key, key, labels.Select(l => l.Key).ToArray()));
        gauge.WithLabels(labels.Select(l => l.Value).ToArray()).Set(value);

        TrackMetric(name, MetricType.Gauge, value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Observe value for histogram metric
    /// </summary>
    public Task ObserveHistogramAsync(string name, double value, params KeyValuePair<string, string>[] labels)
    {
        var histogram = _histograms.GetOrAdd(name, key => Metrics.CreateHistogram(key, key, new double[] { 0.1, 0.5, 1, 2, 5, 10 }, labels.Select(l => l.Key).ToArray()));
        histogram.WithLabels(labels.Select(l => l.Value).ToArray()).Observe(value);

        TrackMetric(name, MetricType.Histogram, value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Get recent metrics for dashboard
    /// </summary>
    public Task<DashboardData> GetDashboardDataAsync()
    {
        var dashboard = new DashboardData
        {
            DashboardId = "default",
            Name = "Security Monitoring Dashboard",
            SystemHealth = new SystemHealthStatus
            {
                OverallStatus = HealthStatus.Healthy,
                ComponentStatus = new Dictionary<string, HealthStatus>
                {
                    ["API"] = HealthStatus.Healthy,
                    ["Database"] = HealthStatus.Healthy,
                    ["Messaging"] = HealthStatus.Healthy
                },
                LastChecked = DateTime.UtcNow
            },
            SecurityMetrics = new SecurityMetrics
            {
                TotalEvents = GetMetricTotal("security_events_total"),
                SecurityViolations = GetMetricTotal("security_violations_total"),
                FailedAuthentications = GetMetricTotal("authentication_failures_total"),
                BlockedIps = GetMetricTotal("ips_blocked_total"),
                AnomaliesDetected = GetMetricTotal("anomalies_detected_total"),
                EventsByType = GetEventsByType()
            },
            PerformanceMetrics = new PerformanceMetrics
            {
                AverageResponseTime = GetAverageMetric("response_time_seconds"),
                TotalRequests = GetMetricTotal("http_requests_total"),
                FailedRequests = GetMetricTotal("http_requests_failed_total"),
                ErrorRate = CalculateErrorRate(),
                ThroughputPerSecond = GetMetricRate("http_requests_total")
            }
        };

        return Task.FromResult(dashboard);
    }

    /// <summary>
    /// Query metric history
    /// </summary>
    public Task<List<MetricData>> QueryMetricsAsync(string metricName, DateTime? start = null, DateTime? end = null)
    {
        if (!_metricHistory.TryGetValue(metricName, out var history))
            return Task.FromResult(new List<MetricData>());

        var query = history.AsEnumerable();

        if (start.HasValue)
            query = query.Where(m => m.Timestamp >= start.Value);

        if (end.HasValue)
            query = query.Where(m => m.Timestamp <= end.Value);

        return Task.FromResult(query.OrderByDescending(m => m.Timestamp).ToList());
    }

    /// <summary>
    /// Export metrics to JSON
    /// </summary>
    public async Task<string> ExportMetricsAsync(string metricName, DateTime? start = null, DateTime? end = null)
    {
        var data = await QueryMetricsAsync(metricName, start, end);
        return System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Register a custom metric entry
    /// </summary>
    public Task RegisterCustomMetricAsync(MetricData data)
    {
        TrackMetric(data.Name, data.Type, data.Value, data.Tags);
        return Task.CompletedTask;
    }

    private void TrackMetric(string name, MetricType type, double value, Dictionary<string, string>? tags = null)
    {
        var entry = new MetricData
        {
            MetricId = Guid.NewGuid().ToString(),
            Name = name,
            Type = type,
            Value = value,
            Timestamp = DateTime.UtcNow,
            Tags = tags ?? new Dictionary<string, string>()
        };

        _metricHistory.AddOrUpdate(
            name,
            new List<MetricData> { entry },
            (_, list) =>
            {
                list.Add(entry);
                if (list.Count > MaxHistoryEntries)
                {
                    list.RemoveAt(0);
                }
                return list;
            });
    }

    private int GetMetricTotal(string metricName)
    {
        if (!_metricHistory.TryGetValue(metricName, out var history))
            return 0;

        return (int)history.Sum(m => m.Value);
    }

    private double GetAverageMetric(string metricName)
    {
        if (!_metricHistory.TryGetValue(metricName, out var history) || history.Count == 0)
            return 0;

        return history.Average(m => m.Value);
    }

    private double GetMetricRate(string metricName)
    {
        if (!_metricHistory.TryGetValue(metricName, out var history) || history.Count < 2)
            return 0;

        var recent = history.TakeLast(10).ToList();
        var duration = (recent.Last().Timestamp - recent.First().Timestamp).TotalSeconds;
        if (duration <= 0)
            return 0;

        var delta = recent.Last().Value - recent.First().Value;
        return delta / duration;
    }

    private double CalculateErrorRate()
    {
        var total = GetMetricTotal("http_requests_total");
        var failed = GetMetricTotal("http_requests_failed_total");

        if (total == 0)
            return 0;

        return failed / (double)total * 100;
    }

    private Dictionary<string, int> GetEventsByType()
    {
        var result = new Dictionary<string, int>();
        foreach (var entry in _metricHistory.Where(kvp => kvp.Key.StartsWith("security_events_type_")))
        {
            var type = entry.Key.Replace("security_events_type_", string.Empty);
            result[type] = (int)entry.Value.Sum(v => v.Value);
        }
        return result;
    }
}
