using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class InfrastructurePlanService
{
    private readonly InfrastructureTemplateService _templates;
    private readonly TerraformManifestService _terraform;
    private readonly ArmTemplateService _arm;
    private readonly BicepTemplateService _bicep;
    private readonly ComplianceInsightsService _compliance;
    private readonly DriftDetectionService _drift;
    private readonly InfrastructureOptions _options;

    public InfrastructurePlanService(
        InfrastructureTemplateService templates,
        TerraformManifestService terraform,
        ArmTemplateService arm,
        BicepTemplateService bicep,
        ComplianceInsightsService compliance,
        DriftDetectionService drift,
        IOptions<InfrastructureOptions> options)
    {
        _templates = templates;
        _terraform = terraform;
        _arm = arm;
        _bicep = bicep;
        _compliance = compliance;
        _drift = drift;
        _options = options.Value;
    }

    public InfrastructurePlan CreatePlan(string templateName, InfrastructureRequest request)
    {
        request ??= new InfrastructureRequest();
        var template = _templates.Get(templateName);
        var provider = string.IsNullOrWhiteSpace(request.Provider)
            ? template?.Provider ?? _options.Simulation.DefaultProvider
            : request.Provider;

        var region = string.IsNullOrWhiteSpace(request.Region)
            ? _options.Simulation.DefaultRegion
            : request.Region;

        var resources = MergeResources(template, request);
        var variables = MergeVariables(template, request, region);
        var compliance = _compliance.Evaluate(resources);

        var plan = new InfrastructurePlan
        {
            ProjectName = request.ProjectName,
            Environment = request.Environment,
            Provider = provider,
            Region = region,
            Resources = resources.ToArray(),
            Variables = variables,
            Metadata = BuildMetadata(templateName, request),
            RecommendedControls = DeriveControls(resources),
            Compliance = compliance.ToList()
        };

        plan.Terraform = _terraform.Build(plan);
        plan.ArmTemplate = _arm.Build(plan);
        plan.Bicep = _bicep.Build(plan);

        return plan;
    }

    public PlanPreview BuildPreview(string templateName, InfrastructureRequest request)
    {
        var plan = CreatePlan(templateName, request);
        var drift = _drift.Estimate(plan);
        return new PlanPreview
        {
            Plan = plan,
            PotentialDrift = drift
        };
    }

    private static List<IaCResource> MergeResources(IaCTemplate? template, InfrastructureRequest request)
    {
        var resources = new Dictionary<string, IaCResource>(StringComparer.OrdinalIgnoreCase);

        if (template is not null)
        {
            foreach (var resource in template.Resources)
            {
                resources[resource.Name] = new IaCResource
                {
                    Name = resource.Name,
                    Type = resource.Type,
                    Properties = new Dictionary<string, string>(resource.Properties, StringComparer.OrdinalIgnoreCase),
                    SecurityControls = new Dictionary<string, string>(resource.SecurityControls, StringComparer.OrdinalIgnoreCase)
                };
            }
        }

        foreach (var resource in request.Resources)
        {
            if (resources.TryGetValue(resource.Name, out var existing))
            {
                foreach (var property in resource.Properties)
                {
                    existing.Properties[property.Key] = property.Value;
                }

                foreach (var control in resource.SecurityControls)
                {
                    existing.SecurityControls[control.Key] = control.Value;
                }
            }
            else
            {
                var type = string.IsNullOrWhiteSpace(resource.Type)
                    ? "custom_resource"
                    : resource.Type;

                resources[resource.Name] = new IaCResource
                {
                    Name = resource.Name,
                    Type = type,
                    Properties = new Dictionary<string, string>(resource.Properties, StringComparer.OrdinalIgnoreCase),
                    SecurityControls = new Dictionary<string, string>(resource.SecurityControls, StringComparer.OrdinalIgnoreCase)
                };
            }
        }

        return resources.Values.ToList();
    }

    private Dictionary<string, string> MergeVariables(IaCTemplate? template, InfrastructureRequest request, string region)
    {
        var variables = template is not null
            ? new Dictionary<string, string>(template.Variables, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var parameter in request.Parameters)
        {
            variables[parameter.Key] = parameter.Value;
        }

        if (!variables.ContainsKey("location"))
        {
            variables["location"] = region;
        }

        if (!variables.ContainsKey("environment"))
        {
            variables["environment"] = request.Environment;
        }

        return variables;
    }

    private static Dictionary<string, string> BuildMetadata(string templateName, InfrastructureRequest request)
    {
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["template"] = templateName,
            ["generatedOn"] = DateTimeOffset.UtcNow.ToString("u"),
        };

        if (!request.Parameters.TryGetValue("requestedBy", out var requestedBy) || string.IsNullOrWhiteSpace(requestedBy))
        {
            requestedBy = "system";
        }

        metadata["requestedBy"] = requestedBy;

        if (!string.IsNullOrWhiteSpace(request.ProjectName))
        {
            metadata["project"] = request.ProjectName;
        }

        return metadata;
    }

    private static List<string> DeriveControls(IEnumerable<IaCResource> resources)
    {
        var controls = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var resource in resources)
        {
            foreach (var control in resource.SecurityControls)
            {
                controls.Add($"{resource.Name}:{control.Key}={control.Value}");
            }
        }

        return controls.ToList();
    }
}
