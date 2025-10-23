using System.Linq;
using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class DeploymentPlanService
{
    private readonly CloudBlueprintService _blueprints;
    private readonly ResourceManifestService _manifests;
    private readonly ReleaseRunbookService _runbook;
    private readonly GuardrailEvaluationService _guardrails;
    private readonly IntegrationComposerService _integrations;
    private readonly TrafficStrategyService _traffic;
    private readonly SimulationSettings _simulation;
    private readonly ILogger<DeploymentPlanService> _logger;

    public DeploymentPlanService(
        CloudBlueprintService blueprints,
        ResourceManifestService manifests,
        ReleaseRunbookService runbook,
        GuardrailEvaluationService guardrails,
        IntegrationComposerService integrations,
        TrafficStrategyService traffic,
        IOptions<CloudDeploymentOptions> options,
        ILogger<DeploymentPlanService> logger)
    {
        _blueprints = blueprints;
        _manifests = manifests;
        _runbook = runbook;
        _guardrails = guardrails;
        _integrations = integrations;
        _traffic = traffic;
        _simulation = options.Value.Simulation;
        _logger = logger;
    }

    public DeploymentPlan BuildPlan(DeploymentPlanRequest request)
    {
        request ??= new DeploymentPlanRequest();
        request.Window ??= new ReleaseWindow();
        request.Overrides ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var blueprint = ResolveBlueprint(request.BlueprintName);
        var manifests = _manifests.BuildManifests(blueprint, request);
        var steps = _runbook.BuildSteps(blueprint, request);
        var integration = _integrations.Compose(blueprint, request);
        var traffic = _traffic.BuildPlan(blueprint);

        var plan = new DeploymentPlan
        {
            BlueprintName = blueprint.Name,
            Provider = blueprint.Provider,
            Environment = blueprint.Environment,
            Window = request.Window,
            Steps = steps,
            Resources = manifests,
            Integrations = integration,
            Traffic = traffic,
            Observations = _runbook.BuildObservations(blueprint, request)
        };

        var guardrails = _guardrails.Evaluate(plan);
        plan.Guardrails = guardrails.ToList();

        _logger.LogInformation("Deployment plan built for {Blueprint} targeting {Provider}/{Environment}", blueprint.Name, blueprint.Provider, blueprint.Environment);
        return plan;
    }

    public DeploymentPlan BuildPreview(PreviewRequest request)
    {
        var planRequest = new DeploymentPlanRequest
        {
            BlueprintName = request.BlueprintName,
            Overrides = request.Overrides
        };

        return BuildPlan(planRequest);
    }

    private CloudBlueprint ResolveBlueprint(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var explicitBlueprint = _blueprints.Get(name);
            if (explicitBlueprint is not null)
            {
                return explicitBlueprint;
            }
        }

        var all = _blueprints.GetAll();
        if (all.Count == 0)
        {
            throw new InvalidOperationException("No blueprints available");
        }

        var providerPreference = _simulation.Providers.FirstOrDefault();
        return all.FirstOrDefault(bp => bp.Provider.Equals(providerPreference, StringComparison.OrdinalIgnoreCase))
               ?? all.First();
    }
}
