using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingProfileService
{
    private readonly ConcurrentDictionary<string, ScalingProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<ScalingProfileService> _logger;

    public ScalingProfileService(IOptions<ScalingOptions> options, ILogger<ScalingProfileService> logger)
    {
        _logger = logger;
        foreach (var profile in options.Value.Profiles)
        {
            var clone = Clone(profile);
            _profiles[clone.Name] = clone;
        }

        if (_profiles.IsEmpty)
        {
            _logger.LogWarning("No scaling profiles configured; seeding fallback profile");
            var fallback = new ScalingProfile
            {
                Name = "default-web",
                Description = "Fallback web workload profile",
                WorkloadType = "web",
                ResourceType = "AzureAppService",
                MinInstances = 2,
                MaxInstances = 10,
                ScaleOutStep = 2,
                ScaleInStep = 1,
                Cpu = new MetricThreshold { ScaleOut = 70, ScaleIn = 40 },
                Memory = new MetricThreshold { ScaleOut = 75, ScaleIn = 45 },
                Requests = new MetricThreshold { ScaleOut = 250, ScaleIn = 80 },
                CostPerInstance = 0.25,
                Regions = new List<string> { "eastus", "westeurope" }
            };

            _profiles[fallback.Name] = fallback;
        }
    }

    public IReadOnlyCollection<ScalingProfile> GetAll()
    {
        return _profiles.Values.Select(Clone).ToArray();
    }

    public ScalingProfile? Get(string name)
    {
        return _profiles.TryGetValue(name, out var profile) ? Clone(profile) : null;
    }

    public ScalingProfile Upsert(ScalingProfile profile)
    {
        var clone = Clone(profile);
        _profiles[clone.Name] = clone;
        _logger.LogInformation("Scaling profile {Profile} saved (min {Min}/max {Max})", clone.Name, clone.MinInstances, clone.MaxInstances);
        return Clone(clone);
    }

    public void Delete(string name)
    {
        if (_profiles.TryRemove(name, out _))
        {
            _logger.LogInformation("Scaling profile {Profile} deleted", name);
        }
    }

    private static ScalingProfile Clone(ScalingProfile profile)
    {
        return new ScalingProfile
        {
            Name = profile.Name,
            Description = profile.Description,
            WorkloadType = profile.WorkloadType,
            ResourceType = profile.ResourceType,
            MinInstances = profile.MinInstances,
            MaxInstances = profile.MaxInstances,
            ScaleOutStep = profile.ScaleOutStep,
            ScaleInStep = profile.ScaleInStep,
            Cpu = new MetricThreshold
            {
                ScaleOut = profile.Cpu.ScaleOut,
                ScaleIn = profile.Cpu.ScaleIn
            },
            Memory = new MetricThreshold
            {
                ScaleOut = profile.Memory.ScaleOut,
                ScaleIn = profile.Memory.ScaleIn
            },
            Requests = new MetricThreshold
            {
                ScaleOut = profile.Requests.ScaleOut,
                ScaleIn = profile.Requests.ScaleIn
            },
            CostPerInstance = profile.CostPerInstance,
            Regions = new List<string>(profile.Regions),
            Tags = new Dictionary<string, string>(profile.Tags, StringComparer.OrdinalIgnoreCase)
        };
    }
}
