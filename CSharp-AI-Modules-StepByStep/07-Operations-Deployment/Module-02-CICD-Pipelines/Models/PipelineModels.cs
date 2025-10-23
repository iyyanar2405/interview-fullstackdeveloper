namespace Module_02_CICD_Pipelines.Models;

public enum PipelineProvider
{
    GitHubActions,
    AzureDevOps,
    GitLabCi,
    Jenkins,
    ArgoWorkflows
}

public sealed class PipelineJobTemplate
{
    public string Name { get; set; } = string.Empty;
    public string RunsOn { get; set; } = "ubuntu-latest";
    public List<string> Steps { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
}

public sealed class PipelineTemplate
{
    public string Name { get; set; } = string.Empty;
    public PipelineProvider Provider { get; set; } = PipelineProvider.GitHubActions;
    public string Description { get; set; } = string.Empty;
    public string Trigger { get; set; } = "push";
    public Dictionary<string, string> Variables { get; set; } = new();
    public List<PipelineJobTemplate> Jobs { get; set; } = new();
    public List<string> QualityChecks { get; set; } = new();
    public List<string> Notifications { get; set; } = new();
}

public sealed class PipelineRequest
{
    public string ProjectName { get; set; } = "ai-service";
    public PipelineProvider Provider { get; set; } = PipelineProvider.GitHubActions;
    public string DefaultBranch { get; set; } = "main";
    public bool IncludeTests { get; set; } = true;
    public bool IncludeSecurityScanning { get; set; } = true;
    public bool IncludeInfrastructure { get; set; } = false;
    public bool IncludeDeployment { get; set; } = true;
    public bool UseMatrix { get; set; } = true;
    public List<string> Environments { get; set; } = new() { "dev", "qa", "prod" };
    public Dictionary<string, string> RuntimeVersions { get; set; } = new()
    {
        ["dotnet"] = "8.0",
        ["node"] = "20"
    };
}

public sealed class PipelinePlan
{
    public string PipelineYaml { get; set; } = string.Empty;
    public List<string> RecommendedChecks { get; set; } = new();
    public Dictionary<string, string> EnvironmentMatrix { get; set; } = new();
    public List<string> DeploymentStages { get; set; } = new();
    public string Provider { get; set; } = string.Empty;
}

public sealed class PipelineRunRecord
{
    public Guid RunId { get; set; } = Guid.NewGuid();
    public string ProjectName { get; set; } = string.Empty;
    public PipelineProvider Provider { get; set; } = PipelineProvider.GitHubActions;
    public string TemplateName { get; set; } = string.Empty;
    public string Status { get; set; } = "Queued";
    public DateTimeOffset QueuedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
        = null;
    public PipelinePlan? Plan { get; set; }
        = null;
    public List<PipelineStageLog> StageLogs { get; set; } = new();
    public string TriggeredBy { get; set; } = "system";
    public string? FailureReason { get; set; }
        = null;
}

public sealed class PipelineStageLog
{
    public string Stage { get; set; } = string.Empty;
    public List<string> Logs { get; set; } = new();
    public string Status { get; set; } = "Pending";
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}

public sealed class PipelineOptions
{
    public List<PipelineTemplate> Templates { get; set; } = new();
    public SimulationOptions Simulation { get; set; } = new();
}

public sealed class SimulationOptions
{
    public int Concurrency { get; set; } = 2;
    public int ProcessingDelaySeconds { get; set; } = 6;
    public double FailureRate { get; set; } = 0.1;
    public List<string> SampleActors { get; set; } = new();
}

public sealed class PipelineQueueItem
{
    public PipelineRequest Request { get; set; } = new();
    public string TemplateName { get; set; } = string.Empty;
    public string TriggeredBy { get; set; } = "api";
}

public sealed class PipelinePlanRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public PipelineRequest Request { get; set; } = new();
}

public sealed class QueuePipelineRequest : PipelinePlanRequest
{
    public string TriggeredBy { get; set; } = "api";
}
