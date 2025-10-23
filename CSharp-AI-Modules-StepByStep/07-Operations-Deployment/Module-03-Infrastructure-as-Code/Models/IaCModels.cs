using System.Collections.Concurrent;

namespace Module_03_Infrastructure_as_Code.Models;

public sealed class InfrastructureOptions
{
    public List<IaCTemplate> Templates { get; set; } = new();
    public List<ComplianceRule> ComplianceRules { get; set; } = new();
    public SimulationOptions Simulation { get; set; } = new();
}

public sealed class SimulationOptions
{
    public int WorkerIntervalSeconds { get; set; } = 5;
    public double DriftLikelihood { get; set; } = 0.2;
    public string DefaultRequestedBy { get; set; } = "automation@contoso";
    public string DefaultProvider { get; set; } = "terraform";
    public string DefaultRegion { get; set; } = "eastus";
    public List<string> Environments { get; set; } = new() { "dev", "staging", "production" };
}

public sealed class IaCTemplate
{
    public string Name { get; set; } = "base";
    public string Provider { get; set; } = "terraform";
    public string Description { get; set; } = string.Empty;
    public List<IaCResourceTemplate> Resources { get; set; } = new();
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Tags { get; set; } = new();
}

public sealed class IaCResourceTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SecurityControls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class InfrastructureRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public string Environment { get; set; } = "dev";
    public string Region { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public List<IaCResource> Resources { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class IaCResource
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SecurityControls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class InfrastructurePlanRequest
{
    public string TemplateName { get; set; } = "base";
    public InfrastructureRequest Request { get; set; } = new();
}

public sealed class InfrastructurePlan
{
    public string ProjectName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public IReadOnlyCollection<IaCResource> Resources { get; set; } = Array.Empty<IaCResource>();
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> RecommendedControls { get; set; } = new();
    public string Terraform { get; set; } = string.Empty;
    public string ArmTemplate { get; set; } = string.Empty;
    public string Bicep { get; set; } = string.Empty;
    public List<ComplianceResult> Compliance { get; set; } = new();
}

public sealed class ComplianceRule
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "medium";
    public List<string> AppliesTo { get; set; } = new();
    public Dictionary<string, string> Requirements { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ComplianceResult
{
    public string RuleId { get; set; } = string.Empty;
    public string Status { get; set; } = "pass";
    public string Severity { get; set; } = "medium";
    public string Message { get; set; } = string.Empty;
}

public sealed class DriftFinding
{
    public string ResourceName { get; set; } = string.Empty;
    public string Property { get; set; } = string.Empty;
    public string DesiredValue { get; set; } = string.Empty;
    public string ActualValue { get; set; } = string.Empty;
    public string Impact { get; set; } = "low";
}

public sealed class ProvisioningQueueItem
{
    public Guid RunId { get; init; } = Guid.NewGuid();
    public InfrastructurePlan Plan { get; init; } = new();
    public InfrastructureRequest Request { get; init; } = new();
    public string RequestedBy { get; init; } = string.Empty;
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class ProvisioningRunRecord
{
    private readonly ConcurrentQueue<string> _log = new();

    public Guid RunId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public string RequestedBy { get; init; } = string.Empty;
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string Status { get; set; } = "Queued";
    public string? FailureReason { get; set; }
    public InfrastructurePlan? Plan { get; set; }
    public List<DriftFinding> DriftAnalysis { get; set; } = new();
    public List<ComplianceResult> Compliance { get; set; } = new();

    public IReadOnlyCollection<string> ExecutionLog => _log.ToArray();

    public void AppendLog(string message)
    {
        _log.Enqueue(message);
    }
}

public sealed class QueueProvisioningRequest
{
    public string TemplateName { get; set; } = "base";
    public InfrastructureRequest Request { get; set; } = new();
    public string RequestedBy { get; set; } = string.Empty;
}

public sealed class PlanPreview
{
    public InfrastructurePlan Plan { get; set; } = new();
    public IReadOnlyCollection<DriftFinding> PotentialDrift { get; set; } = Array.Empty<DriftFinding>();
}
