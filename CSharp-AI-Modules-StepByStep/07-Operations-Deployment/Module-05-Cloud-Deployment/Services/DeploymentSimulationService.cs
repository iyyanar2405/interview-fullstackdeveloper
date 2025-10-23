using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class DeploymentSimulationService
{
    private readonly SimulationSettings _settings;
    private readonly ILogger<DeploymentSimulationService> _logger;

    public DeploymentSimulationService(IOptions<CloudDeploymentOptions> options, ILogger<DeploymentSimulationService> logger)
    {
        _settings = options.Value.Simulation;
        _logger = logger;
    }

    public IReadOnlyCollection<DeploymentTimelineEntry> Simulate(DeploymentPlan plan)
    {
        var random = new Random(CreateSeed(plan));
        var timeline = new List<DeploymentTimelineEntry>();
        var status = "Pending";

        foreach (var step in plan.Steps)
        {
            var entry = new DeploymentTimelineEntry
            {
                Timestamp = DateTimeOffset.UtcNow.AddMinutes(timeline.Count * 5),
                Step = step.Name,
                Status = status,
                Detail = $"Executing action '{step.Action}'"
            };

            if (random.NextDouble() < _settings.FailureRate)
            {
                entry.Status = "Warning";
                entry.Detail += " (requires manual validation)";
            }
            else
            {
                entry.Status = "Completed";
            }

            timeline.Add(entry);
        }

        _logger.LogInformation("Simulation complete for {Blueprint} with {Count} steps", plan.BlueprintName, timeline.Count);
        return timeline;
    }

    private static int CreateSeed(DeploymentPlan plan)
    {
        unchecked
        {
            var seed = 17;
            seed = seed * 23 + plan.BlueprintName.GetHashCode(StringComparison.OrdinalIgnoreCase);
            seed = seed * 23 + plan.Environment.GetHashCode(StringComparison.OrdinalIgnoreCase);
            seed = seed * 23 + plan.Provider.GetHashCode(StringComparison.OrdinalIgnoreCase);
            return seed;
        }
    }
}
