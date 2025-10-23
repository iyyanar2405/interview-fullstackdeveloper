using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class IntegrationComposerService
{
    private readonly IntegrationDefaults _defaults;

    public IntegrationComposerService(IOptions<CloudDeploymentOptions> options)
    {
        _defaults = options.Value.Integrations;
    }

    public IntegrationSummary Compose(CloudBlueprint blueprint, DeploymentPlanRequest request)
    {
        var summary = new IntegrationSummary
        {
            Observability = _defaults.ObservabilityStack,
            Secrets = _defaults.SecretsProvider,
            Registry = _defaults.Registry,
            ArtifactStore = _defaults.ArtifactStore,
            Notifications = new List<string>
            {
                "teams://cloud-release",
                "pagerduty://ops"
            }
        };

        if (blueprint.Provider.Equals("aws", StringComparison.OrdinalIgnoreCase))
        {
            summary.Observability = "cloudwatch";
            summary.Secrets = "aws-secrets-manager";
        }

        if (request.Overrides.TryGetValue("observability", out var overrideObservability))
        {
            summary.Observability = overrideObservability;
        }

        if (request.Overrides.TryGetValue("secrets", out var overrideSecrets))
        {
            summary.Secrets = overrideSecrets;
        }

        return summary;
    }
}
