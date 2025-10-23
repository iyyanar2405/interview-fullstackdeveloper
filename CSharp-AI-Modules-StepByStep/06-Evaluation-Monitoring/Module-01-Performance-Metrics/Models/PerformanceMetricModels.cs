using System.Collections.Concurrent;

namespace Module_01_Performance_Metrics.Models;

public sealed class RequestMetric
{
    public string RequestId { get; init; } = Guid.NewGuid().ToString("N");
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public bool IsSuccess { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public TimeSpan Duration { get; init; }
    public long RequestBytes { get; init; }
    public long? ResponseBytes { get; init; }
    public string? UserAgent { get; init; }
    public string? TraceId { get; init; }
    public double? TokensUsed { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}

public sealed class RequestMetricSummary
{
    public int TotalRequests { get; init; }
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public double SuccessRate => TotalRequests == 0 ? 0 : SuccessCount / (double)TotalRequests;
    public double ErrorRate => TotalRequests == 0 ? 0 : ErrorCount / (double)TotalRequests;
    public double AverageLatencyMs { get; init; }
    public double P50LatencyMs { get; init; }
    public double P90LatencyMs { get; init; }
    public double P95LatencyMs { get; init; }
    public double P99LatencyMs { get; init; }
    public double MaxLatencyMs { get; init; }
    public double MinLatencyMs { get; init; }
    public double AverageRequestSizeKb { get; init; }
    public double? AverageResponseSizeKb { get; init; }
    public IReadOnlyDictionary<string, int> StatusCodeGroups { get; init; } = new Dictionary<string, int>();
    public TimeSpan Window { get; init; }
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class ThroughputSnapshot
{
    public TimeSpan Window { get; init; }
    public int TotalRequests { get; init; }
    public double RequestsPerSecond { get; init; }
    public double RequestsPerMinute { get; init; }
    public double RequestsPerHour { get; init; }
}

public sealed class ResourceSnapshot
{
    public DateTimeOffset Timestamp { get; init; }
    public double CpuUsagePercentage { get; init; }
    public double MemoryUsageMb { get; init; }
    public double ManagedMemoryMb { get; init; }
    public int ActiveThreads { get; init; }
    public int ActiveRequests { get; init; }
    public int HandleCount { get; init; }
}

public sealed class ResourceSummary
{
    public TimeSpan Window { get; init; }
    public double AverageCpuUsage { get; init; }
    public double PeakCpuUsage { get; init; }
    public double AverageMemoryUsageMb { get; init; }
    public double PeakMemoryUsageMb { get; init; }
    public double AverageManagedMemoryMb { get; init; }
    public int PeakActiveRequests { get; init; }
    public double AverageActiveRequests { get; init; }
    public double AverageThreads { get; init; }
    public double PeakThreads { get; init; }
}

public sealed class CostSettings
{
    public double BaseCostPerRequest { get; set; } = 0.001;
    public double CostPerSecond { get; set; } = 0.0005;
    public double CostPerMegabyte { get; set; } = 0.0002;
    public double TokenCostPerThousand { get; set; } = 0.002;
    public double DefaultTokenUsage { get; set; } = 750;
}

public sealed class CostRecord
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public string Path { get; init; } = string.Empty;
    public double Cost { get; init; }
    public double DurationMs { get; init; }
    public double? TokensUsed { get; init; }
    public int StatusCode { get; init; }
}

public sealed class CostSummary
{
    public TimeSpan Window { get; init; }
    public double TotalCost { get; init; }
    public double AverageCostPerRequest { get; init; }
    public double AverageTokensPerRequest { get; init; }
    public double CostPerMinute { get; init; }
    public int RequestCount { get; init; }
}

public enum SLAComparison
{
    LessThanOrEqual,
    GreaterThanOrEqual
}

public sealed class SLATarget
{
    public string Name { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public SLAComparison Comparison { get; set; } = SLAComparison.LessThanOrEqual;
    public double Threshold { get; set; }
    public int WindowMinutes { get; set; } = 60;
}

public sealed class SLAViolation
{
    public string Name { get; init; } = string.Empty;
    public string Metric { get; init; } = string.Empty;
    public double ObservedValue { get; init; }
    public double Threshold { get; init; }
    public SLAComparison Comparison { get; init; }
    public TimeSpan Window { get; init; }
}

public sealed class SLAReport
{
    public TimeSpan Window { get; init; }
    public IReadOnlyList<SLAViolation> Violations { get; init; } = Array.Empty<SLAViolation>();
    public double ComplianceRate { get; init; }
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class PerformanceDashboardSnapshot
{
    public RequestMetricSummary RequestSummary { get; init; } = new();
    public ThroughputSnapshot Throughput { get; init; } = new();
    public ResourceSummary Resource { get; init; } = new();
    public CostSummary Cost { get; init; } = new();
    public SLAReport Sla { get; init; } = new();
}

public sealed class ExportEnvelope
{
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
    public string Format { get; init; } = "json";
    public PerformanceDashboardSnapshot Snapshot { get; init; } = new();
}

public sealed class ResourceSamplingOptions
{
    public int IntervalSeconds { get; set; } = 5;
    public int HistorySize { get; set; } = 288;
}
