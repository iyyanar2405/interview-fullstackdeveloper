using System.Linq;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingRecommendationService
{
    private readonly ScalingProfileService _profiles;
    private readonly CapacityForecastService _forecast;
    private readonly CostProjectionService _costs;
    private readonly ScalingPolicyBuilderService _policies;
    private readonly ILogger<ScalingRecommendationService> _logger;

    public ScalingRecommendationService(
        ScalingProfileService profiles,
        CapacityForecastService forecast,
        CostProjectionService costs,
        ScalingPolicyBuilderService policies,
        ILogger<ScalingRecommendationService> logger)
    {
        _profiles = profiles;
        _forecast = forecast;
        _costs = costs;
        _policies = policies;
        _logger = logger;
    }

    public ScalingPlan BuildPlan(ScalingPlanRequest request)
    {
        request ??= new ScalingPlanRequest();
        request.Snapshot ??= new WorkloadSnapshot();

        var profile = ResolveProfile(request.ProfileName);
        var forecast = _forecast.BuildForecast(profile, request.Snapshot, request.Scenario, request.ForecastHorizonMinutes);
        var costs = _costs.ProjectCosts(profile, request.Snapshot, forecast);
        var actions = DetermineActions(profile, request.Snapshot, forecast);
        var policy = _policies.BuildPolicy(profile, forecast);
        var observations = BuildObservations(profile, request.Snapshot, forecast, costs);

        _logger.LogInformation("Scaling plan built for profile {Profile} with {ActionCount} actions", profile.Name, actions.Count);

        return new ScalingPlan
        {
            ProfileName = profile.Name,
            Snapshot = request.Snapshot,
            RecommendedActions = actions,
            CapacityForecast = forecast,
            CostProjection = costs,
            Policy = policy,
            Observations = observations
        };
    }

    private ScalingProfile ResolveProfile(string name)
    {
        var profile = string.IsNullOrWhiteSpace(name) ? null : _profiles.Get(name);
        return profile ?? _profiles.GetAll().First();
    }

    private static List<ScalingAction> DetermineActions(ScalingProfile profile, WorkloadSnapshot snapshot, CapacityForecast forecast)
    {
        var actions = new List<ScalingAction>();
        var target = Math.Clamp(forecast.RequiredInstances, profile.MinInstances, profile.MaxInstances);
        var current = Math.Clamp(snapshot.CurrentInstances, profile.MinInstances, profile.MaxInstances);

        if (target > current)
        {
            var step = Math.Max(profile.ScaleOutStep, target - current);
            actions.Add(new ScalingAction
            {
                Action = "scale-out",
                Reason = $"Forecast requires {forecast.RequiredInstances} instances; currently at {current}",
                TargetInstances = Math.Min(profile.MaxInstances, current + step),
                EstimatedLatencyMs = Math.Max(50, snapshot.AverageLatencyMs * 0.8),
                EstimatedUtilization = Math.Min(60, snapshot.CpuPercentage * 0.8)
            });
        }
        else if (target < current)
        {
            var step = Math.Max(profile.ScaleInStep, current - target);
            actions.Add(new ScalingAction
            {
                Action = "scale-in",
                Reason = $"Forecast allows reduction to {forecast.RequiredInstances} instances from {current}",
                TargetInstances = Math.Max(profile.MinInstances, current - step),
                EstimatedLatencyMs = Math.Min(snapshot.AverageLatencyMs * 1.1, 250),
                EstimatedUtilization = Math.Min(75, snapshot.CpuPercentage * 1.1)
            });
        }
        else
        {
            actions.Add(new ScalingAction
            {
                Action = "hold",
                Reason = "Current capacity aligns with forecast",
                TargetInstances = current,
                EstimatedLatencyMs = snapshot.AverageLatencyMs,
                EstimatedUtilization = snapshot.CpuPercentage
            });
        }

        if (snapshot.ErrorRatePercent > 0.5)
        {
            actions.Add(new ScalingAction
            {
                Action = "stabilize",
                Reason = $"Error rate at {snapshot.ErrorRatePercent}% exceeds SLO",
                TargetInstances = Math.Min(profile.MaxInstances, current + profile.ScaleOutStep),
                EstimatedLatencyMs = Math.Max(40, snapshot.AverageLatencyMs * 0.7),
                EstimatedUtilization = Math.Min(55, snapshot.CpuPercentage * 0.7)
            });
        }

        return actions;
    }

    private static IReadOnlyCollection<string> BuildObservations(ScalingProfile profile, WorkloadSnapshot snapshot, CapacityForecast forecast, CostProjection costs)
    {
        var notes = new List<string>
        {
            $"Current utilization: CPU {snapshot.CpuPercentage:F1}% / Memory {snapshot.MemoryPercentage:F1}%",
            $"Requests per second: {snapshot.RequestsPerSecond:F1}; Forecast peak {forecast.PeakRequestsPerSecond:F1}",
            $"Forecast requires up to {forecast.RequiredInstances} instances within {forecast.ForecastHorizonMinutes} minutes"
        };

        if (costs.MonthlyCostForecast > costs.MonthlyCostCurrent)
        {
            notes.Add($"Projected monthly cost increase of {(costs.MonthlyCostForecast - costs.MonthlyCostCurrent):F2} currency units");
        }
        else if (costs.SavingsOpportunity > 0)
        {
            notes.Add($"Potential monthly savings of {costs.SavingsOpportunity:F2} by scaling in");
        }

        if (snapshot.AverageLatencyMs > 300)
        {
            notes.Add("Latency over 300ms suggests scale-out or caching optimizations");
        }

        if (profile.Regions.Count > 1)
        {
            notes.Add("Consider regional load balancing to leverage multi-region profile");
        }

        return notes;
    }
}
