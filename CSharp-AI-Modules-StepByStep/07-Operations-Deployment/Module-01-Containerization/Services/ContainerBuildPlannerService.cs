using Module_01_Containerization.Models;
using Microsoft.Extensions.Options;

namespace Module_01_Containerization.Services;

public sealed class ContainerBuildPlannerService
{
    private readonly DockerfileTemplateService _templates;
    private readonly DockerfileGeneratorService _dockerfile;
    private readonly DockerComposeService _compose;
    private readonly ImageOptimizationService _optimization;
    private readonly ContainerizationOptions _options;

    public ContainerBuildPlannerService(
        DockerfileTemplateService templates,
        DockerfileGeneratorService dockerfile,
        DockerComposeService compose,
        ImageOptimizationService optimization,
        IOptions<ContainerizationOptions> options)
    {
        _templates = templates;
        _dockerfile = dockerfile;
        _compose = compose;
        _optimization = optimization;
        _options = options.Value;
    }

    public ContainerBuildPlan CreatePlan(string templateName, ContainerBuildRequest request, string? composeProfile = null)
    {
        var template = _templates.Get(templateName) ?? throw new InvalidOperationException($"Template '{templateName}' not found.");
        var dockerfile = _dockerfile.GenerateDockerfile(template, request);
        var optimization = _optimization.Analyze(template, request, dockerfile);

        string? composeYaml = null;
        if (!string.IsNullOrWhiteSpace(composeProfile))
        {
            var profile = ResolveProfile(composeProfile);
            composeYaml = _compose.GenerateCompose(profile, request);
        }

        return new ContainerBuildPlan
        {
            Dockerfile = dockerfile,
            ComposeYaml = composeYaml,
            OptimizationReport = optimization,
            Guidance = BuildGuidance(template, request)
        };
    }

    private ComposeProfile ResolveProfile(string name)
    {
        var profile = _options.ComposeProfiles.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        if (profile is null)
        {
            throw new InvalidOperationException($"Compose profile '{name}' not found.");
        }

        return profile;
    }

    private static Dictionary<string, string> BuildGuidance(DockerfileTemplate template, ContainerBuildRequest request)
    {
        var guidance = new Dictionary<string, string>
        {
            ["base-image"] = template.BaseImage,
            ["language"] = request.Language,
            ["multi-stage"] = template.MultiStage ? "enabled" : "disabled"
        };

        if (request.IncludeOpenTelemetry)
        {
            guidance["opentelemetry"] = "Include exporter packages and OTEL_RESOURCE_ATTRIBUTES in runtime layer.";
        }

        if (request.EnableHttps)
        {
            guidance["https"] = "Bind certificates via secrets or docker compose volumes for TLS.";
        }

        return guidance;
    }
}
