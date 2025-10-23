using System.Linq;
using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class TrafficStrategyService
{
    private readonly SimulationSettings _settings;

    public TrafficStrategyService(IOptions<CloudDeploymentOptions> options)
    {
        _settings = options.Value.Simulation;
    }

    public TrafficSwitchPlan BuildPlan(CloudBlueprint blueprint)
    {
        var strategy = blueprint.TrafficStrategies.FirstOrDefault() ?? new TrafficStrategy
        {
            Name = "blue-green",
            Pattern = "blue-green",
            Steps =
            {
                "Deploy green",
                "Smoke test",
                "Swap"
            }
        };

        var changes = new List<TrafficChangeStep>();
        var splits = _settings.TrafficSplits;
        for (var i = 0; i < splits.Count; i++)
        {
            changes.Add(new TrafficChangeStep
            {
                Split = splits[i],
                Criteria = i == 0 ? "Post-swap validation" : "Monitor error budget",
                Duration = i == splits.Count - 1 ? "15m" : "10m"
            });
        }

        return new TrafficSwitchPlan
        {
            Strategy = strategy.Pattern,
            InitialSplit = splits.FirstOrDefault() ?? "100-0",
            Changes = changes
        };
    }
}
