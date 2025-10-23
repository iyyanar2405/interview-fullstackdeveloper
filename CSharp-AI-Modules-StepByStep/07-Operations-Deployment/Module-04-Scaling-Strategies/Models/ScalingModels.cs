using System.Collections.Concurrent;

namespace Module_04_Scaling_Strategies.Models;

public sealed class ScalingOptions
{
    public List<ScalingProfile> Profiles { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
    public ForecastDefaults Forecast { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int WorkerIntervalSeconds { get; set; } = 5;
    public int MaxParallelSimulations { get; set; } = 2;
    public List<LoadScenario> Scenarios { get; set; } = new();
    public List<string> Environments { get; set; } = new() { "dev", "staging", "production" };
}

public sealed class ForecastDefaults
{
    public double CpuGrowthPercentage { get; set; } = 20;
    public double RequestGrowthPercentage { get; set; } = 35;
    public int ForecastHorizonMinutes { get; set; } = 60;
}

public sealed class ScalingProfile
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WorkloadType { get; set; } = "web";
    public string ResourceType { get; set; } = "AzureAppService";
    public int MinInstances { get; set; } = 1;
    public int MaxInstances { get; set; } = 10;
    public int ScaleOutStep { get; set; } = 1;
    public int ScaleInStep { get; set; } = 1;
    public MetricThreshold Cpu { get; set; } = new();
    public MetricThreshold Memory { get; set; } = new();
    public MetricThreshold Requests { get; set; } = new();
    public double CostPerInstance { get; set; } = 0;
    public List<string> Regions { get; set; } = new();
    public Dictionary<string, string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class MetricThreshold
{
    public double ScaleOut { get; set; } = 70;
    public double ScaleIn { get; set; } = 30;
}

public sealed class LoadScenario
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = "steady";
    public double PeakRequestsPerSecond { get; set; } = 200;
    public double BaseRequestsPerSecond { get; set; } = 100;
    public double DurationMinutes { get; set; } = 30;
    public double ErrorBudgetPercent { get; set; } = 1;
}

public sealed class WorkloadSnapshot
{
    public double CpuPercentage { get; set; }
    public double MemoryPercentage { get; set; }
    public double RequestsPerSecond { get; set; }
    public double AverageLatencyMs { get; set; }
    public int CurrentInstances { get; set; }
    public double ErrorRatePercent { get; set; }
    public string Environment { get; set; } = "production";
}

public sealed class ScalingPlanRequest
{
    public string ProfileName { get; set; } = string.Empty;
    public WorkloadSnapshot Snapshot { get; set; } = new();
    public LoadScenario? Scenario { get; set; }
    public int ForecastHorizonMinutes { get; set; }
}

public sealed class ScalingPlan
{
    public string ProfileName { get; set; } = string.Empty;
    public WorkloadSnapshot Snapshot { get; set; } = new();
    public IReadOnlyCollection<ScalingAction> RecommendedActions { get; set; } = Array.Empty<ScalingAction>();
    public CapacityForecast CapacityForecast { get; set; } = new();
    public CostProjection CostProjection { get; set; } = new();
    public ScalingPolicyDocument Policy { get; set; } = new();
    public IReadOnlyCollection<string> Observations { get; set; } = Array.Empty<string>();
}

public sealed class ScalingAction
{
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int TargetInstances { get; set; }
    public double EstimatedLatencyMs { get; set; }
    public double EstimatedUtilization { get; set; }
}

public sealed class CapacityForecast
{
    public int ForecastHorizonMinutes { get; set; }
    public int RequiredInstances { get; set; }
    public double PeakCpuPercentage { get; set; }
    public double PeakRequestsPerSecond { get; set; }
    public IReadOnlyCollection<ForecastPoint> Timeline { get; set; } = Array.Empty<ForecastPoint>();
}

public sealed class ForecastPoint
{
    public DateTimeOffset Timestamp { get; set; }
    public double CpuPercentage { get; set; }
    public double RequestsPerSecond { get; set; }
    public int SuggestedInstances { get; set; }
}

public sealed class CostProjection
{
    public double HourlyCostCurrent { get; set; }
    public double HourlyCostForecast { get; set; }
    public double MonthlyCostCurrent { get; set; }
    public double MonthlyCostForecast { get; set; }
    public double SavingsOpportunity { get; set; }
}

public sealed class ScalingPolicyDocument
{
    public string Provider { get; set; } = "AzureMonitorAutoscale";
    public Dictionary<string, string> Settings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string SampleYaml { get; set; } = string.Empty;
    public string SampleJson { get; set; } = string.Empty;
}

public sealed class ScalingSimulationRequest
{
    public string ProfileName { get; set; } = string.Empty;
    public LoadScenario? Scenario { get; set; }
    public WorkloadSnapshot Snapshot { get; set; } = new();
    public int DurationMinutes { get; set; } = 30;
    public string RequestedBy { get; set; } = string.Empty;
}

public sealed class ScalingQueueItem
{
    public Guid RunId { get; init; } = Guid.NewGuid();
    public ScalingSimulationRequest Request { get; init; } = new();
    public ScalingPlan Plan { get; init; } = new();
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class ScalingRunRecord
{
    private readonly ConcurrentQueue<string> _log = new();

    public Guid RunId { get; init; }
    public string ProfileName { get; init; } = string.Empty;
    public string Scenario { get; init; } = string.Empty;
    public string RequestedBy { get; init; } = string.Empty;
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string Status { get; set; } = "Queued";
    public string? FailureReason { get; set; }
    public ScalingPlan Plan { get; set; } = new();
    public IReadOnlyCollection<ScalingSimulationTimelineEntry> Timeline { get; set; } = Array.Empty<ScalingSimulationTimelineEntry>();

    public IReadOnlyCollection<string> Log => _log.ToArray();

    public void AppendLog(string message)
    {
        _log.Enqueue(message);
    }
}

public sealed class ScalingSimulationTimelineEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public double CpuPercentage { get; set; }
    public double RequestsPerSecond { get; set; }
    public int ActualInstances { get; set; }
    public int TargetInstances { get; set; }
    public string Note { get; set; } = string.Empty;
}
