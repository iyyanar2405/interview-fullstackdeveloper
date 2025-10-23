using System.Linq;
using Microsoft.Extensions.Options;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class CapacityForecastService
{
    private readonly ILogger<CapacityForecastService> _logger;
    private readonly ScalingOptions _options;

    public CapacityForecastService(ILogger<CapacityForecastService> logger, IOptions<ScalingOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public CapacityForecast BuildForecast(ScalingProfile profile, WorkloadSnapshot snapshot, LoadScenario? scenario, int? horizonOverride)
    {
        var horizon = horizonOverride.GetValueOrDefault(_options.Forecast.ForecastHorizonMinutes);
        horizon = Math.Clamp(horizon, 15, 240);

        var timeline = new List<ForecastPoint>();
        var steps = Math.Max(1, horizon / 5);
        var baseRequests = scenario?.BaseRequestsPerSecond > 0 ? scenario.BaseRequestsPerSecond : snapshot.RequestsPerSecond;
        var peakRequests = scenario?.PeakRequestsPerSecond > 0 ? scenario.PeakRequestsPerSecond : snapshot.RequestsPerSecond * (1 + _options.Forecast.RequestGrowthPercentage / 100);
        var cpuGrowth = _options.Forecast.CpuGrowthPercentage / 100;

        for (var i = 0; i <= steps; i++)
        {
            var progress = i / (double)steps;
            var timestamp = DateTimeOffset.UtcNow.AddMinutes(progress * horizon);
            var requests = Lerp(baseRequests, peakRequests, ScenarioCurve(progress, scenario));
            var cpu = Math.Min(100, snapshot.CpuPercentage * (1 + cpuGrowth * progress));
            var instances = CalculateInstances(profile, requests, cpu);

            timeline.Add(new ForecastPoint
            {
                Timestamp = timestamp,
                RequestsPerSecond = Math.Round(requests, 2),
                CpuPercentage = Math.Round(cpu, 2),
                SuggestedInstances = instances
            });
        }

        var requiredInstances = timeline.Max(point => point.SuggestedInstances);
        var finalCpu = timeline.Max(point => point.CpuPercentage);
        var finalRequests = timeline.Max(point => point.RequestsPerSecond);

        _logger.LogInformation("Forecast built for profile {Profile} horizon {Horizon}m -> {Instances} instances", profile.Name, horizon, requiredInstances);

        return new CapacityForecast
        {
            ForecastHorizonMinutes = horizon,
            RequiredInstances = requiredInstances,
            PeakCpuPercentage = finalCpu,
            PeakRequestsPerSecond = finalRequests,
            Timeline = timeline
        };
    }

    private static double ScenarioCurve(double progress, LoadScenario? scenario)
    {
        if (scenario is null)
        {
            return progress;
        }

        return scenario.Pattern.ToLowerInvariant() switch
        {
            "spiky" => Math.Pow(progress, 0.5),
            "wave" => 0.5 + 0.5 * Math.Sin(progress * Math.PI),
            "bursty" => progress < 0.3 ? progress * 2 : 1,
            _ => progress
        };
    }

    private static double Lerp(double start, double end, double progress)
    {
        return start + (end - start) * progress;
    }

    private static int CalculateInstances(ScalingProfile profile, double requestsPerSecond, double cpuPercentage)
    {
        var cpuPerInstance = Math.Max(10, profile.Cpu.ScaleOut);
        var reqPerInstance = Math.Max(10, profile.Requests.ScaleOut);

        var cpuEstimate = Math.Ceiling(cpuPercentage / cpuPerInstance * profile.MinInstances);
        var requestEstimate = Math.Ceiling(requestsPerSecond / reqPerInstance);
        var instances = (int)Math.Max(cpuEstimate, requestEstimate);
        return Math.Clamp(instances, profile.MinInstances, profile.MaxInstances);
    }
}
