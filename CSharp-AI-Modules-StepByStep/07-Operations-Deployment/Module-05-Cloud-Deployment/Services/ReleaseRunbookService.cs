using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class ReleaseRunbookService
{
    public IReadOnlyCollection<DeploymentStep> BuildSteps(CloudBlueprint blueprint, DeploymentPlanRequest request)
    {
        var owner = string.IsNullOrWhiteSpace(request.RequestedBy) ? "release-engineer" : request.RequestedBy;
        var steps = new List<DeploymentStep>
        {
            new()
            {
                Name = "Pre-deployment checks",
                Action = "validate",
                Owner = owner,
                Checks =
                {
                    "Verify infrastructure state",
                    "Confirm observability endpoints",
                    "Validate secrets availability"
                }
            },
            new()
            {
                Name = "Deploy infrastructure",
                Action = "apply",
                Owner = "infra-team",
                Checks =
                {
                    "ARM/Bicep deployment succeeded",
                    "Container registry authenticated"
                }
            },
            new()
            {
                Name = "Deploy application",
                Action = "release",
                Owner = "app-team",
                Checks =
                {
                    "Image pull successful",
                    "Config maps applied",
                    "Smoke tests pass"
                }
            }
        };

        if (blueprint.DeploymentModel.Equals("canary", StringComparison.OrdinalIgnoreCase))
        {
            steps.Add(new DeploymentStep
            {
                Name = "Canary rollout",
                Action = "traffic-shift",
                Owner = "sre-team",
                Checks =
                {
                    "Monitor error budget",
                    "Compare baseline metrics"
                }
            });
        }
        else
        {
            steps.Add(new DeploymentStep
            {
                Name = "Blue/Green swap",
                Action = "slot-swap",
                Owner = "sre-team",
                Checks =
                {
                    "Slot warm",
                    "Post-swap validation"
                }
            });
        }

        steps.Add(new DeploymentStep
        {
            Name = "Post-deployment validation",
            Action = "observe",
            Owner = "operations",
            Checks =
            {
                "Monitor dashboards",
                "Validate alerting",
                "Update status report"
            }
        });

        return steps;
    }

    public IReadOnlyCollection<string> BuildObservations(CloudBlueprint blueprint, DeploymentPlanRequest request)
    {
        var notes = new List<string>
        {
            $"Deployment model: {blueprint.DeploymentModel}",
            $"Target provider: {blueprint.Provider}",
            $"Requested window start: {request.Window.RequestedStart:u}"
        };

        if (blueprint.Components.Count > 3)
        {
            notes.Add("Multiple components detected; consider phased rollout");
        }

        if (!blueprint.Tags.ContainsKey("compliance"))
        {
            notes.Add("No compliance tag set; ensure regulatory checks are handled separately");
        }

        if (request.Overrides.Count > 0)
        {
            notes.Add($"Overrides supplied: {string.Join(", ", request.Overrides.Keys)}");
        }

        return notes;
    }
}
