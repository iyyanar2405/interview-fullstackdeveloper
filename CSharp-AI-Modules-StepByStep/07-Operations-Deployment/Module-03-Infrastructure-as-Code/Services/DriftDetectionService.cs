using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class DriftDetectionService
{
    private readonly SimulationOptions _simulation;

    public DriftDetectionService(IOptions<InfrastructureOptions> options)
    {
        _simulation = options.Value.Simulation;
    }

    public IReadOnlyCollection<DriftFinding> Estimate(InfrastructurePlan plan)
    {
        if (plan.Resources.Count == 0)
        {
            return Array.Empty<DriftFinding>();
        }

        var findings = new List<DriftFinding>();
        var seed = CreateSeed(plan.ProjectName + plan.Environment + plan.Provider);
        var random = new Random(seed);

        foreach (var resource in plan.Resources)
        {
            if (random.NextDouble() > _simulation.DriftLikelihood)
            {
                continue;
            }

            var property = resource.Properties.Keys.FirstOrDefault() ?? "configuration";
            var desired = resource.Properties.TryGetValue(property, out var value) ? value : "configured";
            var actual = GenerateVariant(desired);

            findings.Add(new DriftFinding
            {
                ResourceName = resource.Name,
                Property = property,
                DesiredValue = desired,
                ActualValue = actual,
                Impact = InferImpact(property)
            });
        }

        return findings;
    }

    private static int CreateSeed(string input)
    {
        unchecked
        {
            var seed = 17;
            foreach (var character in input)
            {
                seed = seed * 23 + character;
            }

            return seed;
        }
    }

    private static string GenerateVariant(string desired)
    {
        if (bool.TryParse(desired, out var boolean))
        {
            return boolean ? "false" : "true";
        }

        if (desired.Contains("premium", StringComparison.OrdinalIgnoreCase))
        {
            return desired.Replace("premium", "standard", StringComparison.OrdinalIgnoreCase);
        }

        if (desired.Contains("https", StringComparison.OrdinalIgnoreCase))
        {
            return desired.Replace("https", "http", StringComparison.OrdinalIgnoreCase);
        }

        return desired + "-drift";
    }

    private static string InferImpact(string property)
    {
        if (property.Contains("https", StringComparison.OrdinalIgnoreCase) ||
            property.Contains("encryption", StringComparison.OrdinalIgnoreCase))
        {
            return "high";
        }

        if (property.Contains("sku", StringComparison.OrdinalIgnoreCase) ||
            property.Contains("size", StringComparison.OrdinalIgnoreCase))
        {
            return "medium";
        }

        return "low";
    }
}
