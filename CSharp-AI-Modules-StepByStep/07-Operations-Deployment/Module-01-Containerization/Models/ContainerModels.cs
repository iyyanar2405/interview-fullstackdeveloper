namespace Module_01_Containerization.Models;

public enum DeploymentTarget
{
    Docker,
    Kubernetes,
    AzureContainerApps,
    AzureWebAppForContainers,
    AwsEcs
}

public sealed class DockerfileTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BaseImage { get; set; } = string.Empty;
    public bool MultiStage { get; set; } = true;
    public List<string> BuildStageSteps { get; set; } = new();
    public List<string> RuntimeStageSteps { get; set; } = new();
    public List<string> ExposedPorts { get; set; } = new();
    public List<string> Labels { get; set; } = new();
    public Dictionary<string, string> Arguments { get; set; } = new();
    public List<string> OptimizationTips { get; set; } = new();
}

public sealed class ContainerBuildRequest
{
    public string ProjectName { get; set; } = "ai-service";
    public string Language { get; set; } = "dotnet";
    public string RuntimeVersion { get; set; } = "8.0";
    public DeploymentTarget Target { get; set; } = DeploymentTarget.Docker;
    public bool IncludeOpenTelemetry { get; set; } = true;
    public bool EnableHealthCheck { get; set; } = true;
    public bool EnableHttps { get; set; } = true;
    public bool UseSlimImage { get; set; } = true;
    public bool PublishTrimmed { get; set; } = false;
    public List<int> Ports { get; set; } = new() { 8080 };
    public Dictionary<string, string> Environment { get; set; } = new();
    public List<string> BuildArguments { get; set; } = new();
    public List<string> AdditionalDependencies { get; set; } = new();
}

public sealed class ContainerBuildPlan
{
    public string Dockerfile { get; set; } = string.Empty;
    public string? ComposeYaml { get; set; }
        = null;
    public ImageOptimizationReport OptimizationReport { get; set; } = new();
    public Dictionary<string, string> Guidance { get; set; } = new();
}

public sealed class ImageOptimizationReport
{
    public string BaseImage { get; set; } = string.Empty;
    public double EstimatedImageSizeMb { get; set; }
        = 0;
    public List<string> Recommendations { get; set; } = new();
    public List<LayerBreakdown> Layers { get; set; } = new();
}

public sealed class LayerBreakdown
{
    public string Instruction { get; set; } = string.Empty;
    public double EstimatedSizeMb { get; set; }
        = 0;
    public string Category { get; set; } = string.Empty;
}

public sealed class ComposeProfile
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ComposeServiceConfig> Services { get; set; } = new();
}

public sealed class ComposeServiceConfig
{
    public string ServiceName { get; set; } = string.Empty;
    public string? Image { get; set; }
        = null;
    public string? BuildContext { get; set; }
        = null;
    public Dictionary<string, string> Environment { get; set; } = new();
    public List<int> Ports { get; set; } = new();
    public List<string> DependsOn { get; set; } = new();
    public Dictionary<string, string> Volumes { get; set; } = new();
    public Dictionary<string, string> Labels { get; set; } = new();
    public List<string> Command { get; set; } = new();
}

public sealed class ContainerizationOptions
{
    public List<DockerfileTemplate> Templates { get; set; } = new();
    public List<ComposeProfile> ComposeProfiles { get; set; } = new();
    public SimulationOptions Simulation { get; set; } = new();
}

public sealed class SimulationOptions
{
    public int Concurrency { get; set; } = 2;
    public int ProcessingDelaySeconds { get; set; } = 5;
    public List<string> SampleRegistries { get; set; } = new();
}

public sealed class ContainerBuildRecord
{
    public Guid BuildId { get; set; } = Guid.NewGuid();
    public string ProjectName { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
        = null;
    public string RequestedBy { get; set; } = "system";
    public ContainerBuildRequest Request { get; set; } = new();
    public ContainerBuildPlan? Plan { get; set; }
        = null;
    public string Status { get; set; } = "Queued";
    public string Registry { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
        = null;
}

public sealed class BuildQueueItem
{
    public ContainerBuildRequest Request { get; set; } = new();
    public string RequestedBy { get; set; } = "system";
    public string TemplateName { get; set; } = "dotnet-api";
    public string? ComposeProfile { get; set; }
        = null;
}

public sealed class ContainerBuildPlanRequest
{
    public string TemplateName { get; set; } = "dotnet-api";
    public string? ComposeProfile { get; set; }
        = null;
    public ContainerBuildRequest Request { get; set; } = new();
}

public sealed class QueueBuildRequest : ContainerBuildPlanRequest
{
    public string RequestedBy { get; set; } = "api";
}
