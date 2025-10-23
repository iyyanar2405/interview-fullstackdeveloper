using Microsoft.Extensions.Options;
using Module_01_Performance_Metrics.Models;

namespace Module_01_Performance_Metrics.Services;

public sealed class SLAOptions
{
    public List<SLATarget> Targets { get; set; } = new();
}

public sealed class SLAMonitoringService
{
    private readonly RequestMetricsService _requestMetricsService;
    private readonly SLAOptions _options;

    public SLAMonitoringService(RequestMetricsService requestMetricsService, IOptions<SLAOptions> options)
    {
        _requestMetricsService = requestMetricsService;
        _options = options.Value;
    }

    public SLAReport Evaluate(TimeSpan? windowOverride = null)
    {
        if (_options.Targets.Count == 0)
        {
            return new SLAReport
            {
                Window = windowOverride ?? TimeSpan.FromMinutes(60),
                Violations = Array.Empty<SLAViolation>(),
                ComplianceRate = 1.0
            };
        }

        var violations = new List<SLAViolation>();
        foreach (var target in _options.Targets)
        {
            var window = windowOverride ?? TimeSpan.FromMinutes(target.WindowMinutes);
            var summary = _requestMetricsService.GetSummary(window);
            var throughput = _requestMetricsService.GetThroughput(window);
            var observed = GetMetricValue(target.Metric, summary, throughput);
            if (!IsCompliant(target, observed))
            {
                violations.Add(new SLAViolation
                {
                    Name = target.Name,
                    Metric = target.Metric,
                    ObservedValue = observed,
                    Threshold = target.Threshold,
                    Comparison = target.Comparison,
                    Window = window
                });
            }
        }

        var complianceRate = _options.Targets.Count == 0
            ? 1.0
            : (_options.Targets.Count - violations.Count) / (double)_options.Targets.Count;

        return new SLAReport
        {
            Window = windowOverride ?? TimeSpan.FromMinutes(_options.Targets.Max(t => t.WindowMinutes)),
            Violations = violations,
            ComplianceRate = complianceRate,
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }

    private static double GetMetricValue(string metric, RequestMetricSummary summary, ThroughputSnapshot throughput)
    {
        return metric switch
        {
            "LatencyP50" => summary.P50LatencyMs,
            "LatencyP90" => summary.P90LatencyMs,
            "LatencyP95" => summary.P95LatencyMs,
            "LatencyP99" => summary.P99LatencyMs,
            "SuccessRate" => summary.SuccessRate * 100,
            "ErrorRate" => summary.ErrorRate * 100,
            "ThroughputPerSecond" => throughput.RequestsPerSecond,
            "ThroughputPerMinute" => throughput.RequestsPerMinute,
            "AverageLatency" => summary.AverageLatencyMs,
            _ => 0
        };
    }

    private static bool IsCompliant(SLATarget target, double observed)
    {
        return target.Comparison switch
        {
            SLAComparison.LessThanOrEqual => observed <= target.Threshold,
            SLAComparison.GreaterThanOrEqual => observed >= target.Threshold,
            _ => true
        };
    }
}
