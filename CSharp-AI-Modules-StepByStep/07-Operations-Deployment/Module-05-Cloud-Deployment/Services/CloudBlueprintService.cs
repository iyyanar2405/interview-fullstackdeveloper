using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class CloudBlueprintService
{
    private readonly ConcurrentDictionary<string, CloudBlueprint> _blueprints = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<CloudBlueprintService> _logger;

    public CloudBlueprintService(IOptions<CloudDeploymentOptions> options, ILogger<CloudBlueprintService> logger)
    {
        _logger = logger;
        foreach (var blueprint in options.Value.Blueprints)
        {
            var clone = Clone(blueprint);
            _blueprints[clone.Name] = clone;
        }

        if (_blueprints.IsEmpty)
        {
            SeedFallback(options.Value);
        }
    }

    public IReadOnlyCollection<CloudBlueprint> GetAll()
    {
        return _blueprints.Values.Select(Clone).ToArray();
    }

    public CloudBlueprint? Get(string name)
    {
        return _blueprints.TryGetValue(name, out var blueprint) ? Clone(blueprint) : null;
    }

    public CloudBlueprint Upsert(CloudBlueprint blueprint)
    {
        var clone = Clone(blueprint);
        _blueprints[clone.Name] = clone;
        _logger.LogInformation("Blueprint {Blueprint} saved for provider {Provider}", clone.Name, clone.Provider);
        return Clone(clone);
    }

    public void Delete(string name)
    {
        if (_blueprints.TryRemove(name, out _))
        {
            _logger.LogInformation("Blueprint {Blueprint} deleted", name);
        }
    }

    private void SeedFallback(CloudDeploymentOptions options)
    {
        var fallback = new CloudBlueprint
        {
            Name = "azure-appservice",
            Provider = "azure",
            Environment = "staging",
            DeploymentModel = "blue-green",
            Components =
            {
                new DeploymentComponent
                {
                    Name = "api",
                    Type = "app-service",
                    Image = options.Integrations.Registry + ":api-latest",
                    Size = "P1v3",
                    Configuration =
                    {
                        ["autoscale"] = "enabled",
                        ["healthProbe"] = "/health"
                    }
                },
                new DeploymentComponent
                {
                    Name = "worker",
                    Type = "container-app",
                    Image = options.Integrations.Registry + ":worker-latest",
                    Size = "D4",
                    Configuration =
                    {
                        ["concurrency"] = "5"
                    }
                }
            },
            TrafficStrategies =
            {
                new TrafficStrategy
                {
                    Name = "blue-green",
                    Pattern = "blue-green",
                    Steps =
                    {
                        "Deploy green slot",
                        "Run smoke tests",
                        "Swap traffic"
                    }
                }
            }
        };

        _blueprints[fallback.Name] = fallback;
        _logger.LogWarning("No blueprints configured; fallback blueprint {Blueprint} seeded", fallback.Name);
    }

    private static CloudBlueprint Clone(CloudBlueprint blueprint)
    {
        return new CloudBlueprint
        {
            Name = blueprint.Name,
            Provider = blueprint.Provider,
            Environment = blueprint.Environment,
            DeploymentModel = blueprint.DeploymentModel,
            Tags = new Dictionary<string, string>(blueprint.Tags, StringComparer.OrdinalIgnoreCase),
            Components = blueprint.Components
                .Select(component => new DeploymentComponent
                {
                    Name = component.Name,
                    Type = component.Type,
                    Image = component.Image,
                    Size = component.Size,
                    Configuration = new Dictionary<string, string>(component.Configuration, StringComparer.OrdinalIgnoreCase),
                    Observability = new ObservabilityConfig
                    {
                        EnableMetrics = component.Observability.EnableMetrics,
                        EnableLogs = component.Observability.EnableLogs,
                        EnableTracing = component.Observability.EnableTracing,
                        Exporter = component.Observability.Exporter
                    }
                })
                .ToList(),
            TrafficStrategies = blueprint.TrafficStrategies
                .Select(strategy => new TrafficStrategy
                {
                    Name = strategy.Name,
                    Pattern = strategy.Pattern,
                    Description = strategy.Description,
                    Steps = new List<string>(strategy.Steps)
                })
                .ToList()
        };
    }
}
