using System.Linq;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingSimulationService
{
    public IReadOnlyCollection<ScalingSimulationTimelineEntry> Simulate(ScalingPlan plan, ScalingSimulationRequest request)
    {
        var duration = Math.Max(15, request.DurationMinutes);
        var interval = 5;
        var steps = Math.Max(1, duration / interval);
        var timeline = new List<ScalingSimulationTimelineEntry>();
        var currentInstances = Math.Max(plan.Snapshot.CurrentInstances, plan.CapacityForecast.RequiredInstances);
        var currentCpu = plan.Snapshot.CpuPercentage;
        var currentRps = plan.Snapshot.RequestsPerSecond;

        for (var i = 0; i <= steps; i++)
        {
            var progress = i / (double)steps;
            var target = DetermineTargetInstances(plan, currentInstances, progress, i == steps);
            var cpu = AdjustMetric(currentCpu, progress, target, currentInstances);
            var rps = AdjustMetric(currentRps, progress, target, currentInstances);
            var timestamp = DateTimeOffset.UtcNow.AddMinutes(i * interval);

            timeline.Add(new ScalingSimulationTimelineEntry
            {
                Timestamp = timestamp,
                CpuPercentage = Math.Round(cpu, 2),
                RequestsPerSecond = Math.Round(rps, 2),
                ActualInstances = currentInstances,
                TargetInstances = target,
                Note = BuildNote(target, currentInstances)
            });

            currentInstances = target;
            currentCpu = cpu;
            currentRps = rps;
        }

        return timeline;
    }

    private static int DetermineTargetInstances(ScalingPlan plan, int currentInstances, double progress, bool isFinalStep)
    {
        var holdAction = plan.RecommendedActions.FirstOrDefault(action => action.Action == "hold");
        var scaleAction = plan.RecommendedActions.FirstOrDefault(action => action.Action is "scale-out" or "scale-in");
        var target = currentInstances;

        if (scaleAction is not null)
        {
            target = scaleAction.TargetInstances;
        }
        else if (holdAction is not null)
        {
            target = holdAction.TargetInstances;
        }

        if (!isFinalStep)
        {
            var diff = target - currentInstances;
            target = diff == 0
                ? currentInstances
                : currentInstances + Math.Sign(diff) * Math.Max(1, Math.Abs(diff) / 2);
        }

        return target;
    }

    private static double AdjustMetric(double current, double progress, int targetInstances, int currentInstances)
    {
        var changeFactor = targetInstances == 0 || currentInstances == 0
            ? 1
            : (double)currentInstances / targetInstances;

        var smoothed = current * changeFactor;
        return current + (smoothed - current) * Math.Min(1, progress * 1.5);
    }

    private static string BuildNote(int target, int current)
    {
        if (target > current)
        {
            return $"Scaling out to {target} instances";
        }

        if (target < current)
        {
            return $"Scaling in to {target} instances";
        }

        return "Capacity steady";
    }
}
