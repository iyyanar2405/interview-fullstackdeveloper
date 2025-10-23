using System.Collections.Concurrent;

namespace Module_05_Cloud_Deployment.Models;

public sealed class CloudDeploymentOptions
{
    public List<CloudBlueprint> Blueprints { get; set; } = new();
    public List<ReleaseGuardrail> Guardrails { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
    public IntegrationDefaults Integrations { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int WorkerIntervalSeconds { get; set; } = 5;
    public List<string> Regions { get; set; } = new() { "eastus" };
    public List<string> Providers { get; set; } = new() { "azure", "aws" };
    public double FailureRate { get; set; } = 0.15;
    public List<string> TrafficSplits { get; set; } = new() { "100-0", "80-20", "50-50" };
}

public sealed class IntegrationDefaults
{
    public string ObservabilityStack { get; set; } = "azure-monitor";
    public string SecretsProvider { get; set; } = "key-vault";
    public string Registry { get; set; } = "registry.azurecr.io/ai";
    public string ArtifactStore { get; set; } = "storageAccount";
}

public sealed class CloudBlueprint
{
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Environment { get; set; } = "production";
    public string DeploymentModel { get; set; } = "blue-green";
    public List<DeploymentComponent> Components { get; set; } = new();
    public List<TrafficStrategy> TrafficStrategies { get; set; } = new();
    public Dictionary<string, string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class DeploymentComponent
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public IDictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public ObservabilityConfig Observability { get; set; } = new();
}

public sealed class ObservabilityConfig
{
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogs { get; set; } = true;
    public bool EnableTracing { get; set; } = true;
    public string Exporter { get; set; } = "otlp";
}

public sealed class TrafficStrategy
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = "blue-green";
    public string Description { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();
}

public sealed class DeploymentPlanRequest
{
    public string BlueprintName { get; set; } = string.Empty;
    public ReleaseWindow Window { get; set; } = new();
    public Dictionary<string, string> Overrides { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string RequestedBy { get; set; } = string.Empty;
}

public sealed class ReleaseWindow
{
    public DateTimeOffset RequestedStart { get; set; } = DateTimeOffset.UtcNow;
    public int DurationMinutes { get; set; } = 60;
    public string Timezone { get; set; } = "UTC";
}

public sealed class DeploymentPlan
{
    public string BlueprintName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public ReleaseWindow Window { get; set; } = new();
    public IReadOnlyCollection<DeploymentStep> Steps { get; set; } = Array.Empty<DeploymentStep>();
    public List<ResourceManifest> Resources { get; set; } = new();
    public List<ReleaseGuardrail> Guardrails { get; set; } = new();
    public IntegrationSummary Integrations { get; set; } = new();
    public TrafficSwitchPlan Traffic { get; set; } = new();
    public IReadOnlyCollection<string> Observations { get; set; } = Array.Empty<string>();
}

public sealed class DeploymentStep
{
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public List<string> Checks { get; set; } = new();
}

public sealed class ResourceManifest
{
    public string ComponentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ProvisioningScript { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string HealthProbe { get; set; } = string.Empty;
    public Dictionary<string, string> Secrets { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ReleaseGuardrail
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "medium";
    public string AppliesTo { get; set; } = string.Empty;
    public string Remediation { get; set; } = string.Empty;
}

public sealed class IntegrationSummary
{
    public string Observability { get; set; } = string.Empty;
    public string Secrets { get; set; } = string.Empty;
    public string Registry { get; set; } = string.Empty;
    public string ArtifactStore { get; set; } = string.Empty;
    public List<string> Notifications { get; set; } = new();
}

public sealed class TrafficSwitchPlan
{
    public string Strategy { get; set; } = "blue-green";
    public string InitialSplit { get; set; } = "100-0";
    public List<TrafficChangeStep> Changes { get; set; } = new();
}

public sealed class TrafficChangeStep
{
    public string Split { get; set; } = string.Empty;
    public string Criteria { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
}

public sealed class DeploymentQueueItem
{
    public Guid RunId { get; init; } = Guid.NewGuid();
    public DeploymentPlan Plan { get; init; } = new();
    public DeploymentPlanRequest Request { get; init; } = new();
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class DeploymentRunRecord
{
    private readonly ConcurrentQueue<string> _log = new();

    public Guid RunId { get; init; }
    public string BlueprintName { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public string RequestedBy { get; init; } = string.Empty;
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string Status { get; set; } = "Queued";
    public string? FailureReason { get; set; }
    public DeploymentPlan Plan { get; set; } = new();
    public IReadOnlyCollection<DeploymentTimelineEntry> Timeline { get; set; } = Array.Empty<DeploymentTimelineEntry>();

    public IReadOnlyCollection<string> Log => _log.ToArray();

    public void AppendLog(string message)
    {
        _log.Enqueue(message);
    }
}

public sealed class DeploymentTimelineEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public string Step { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string Detail { get; set; } = string.Empty;
}

public sealed class DeploymentSimulationRequest
{
    public string BlueprintName { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public ReleaseWindow Window { get; set; } = new();
    public Dictionary<string, string> Overrides { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PreviewRequest
{
    public string BlueprintName { get; set; } = string.Empty;
    public Dictionary<string, string> Overrides { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
