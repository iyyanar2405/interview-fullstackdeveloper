using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Advanced_Reasoning.Models;

namespace Module_03_Advanced_Reasoning.Services;

public sealed class StrategyCatalogService
{
    private readonly ConcurrentDictionary<string, StrategyDefinition> _strategies = new(StringComparer.OrdinalIgnoreCase);

    public StrategyCatalogService(IOptions<AdvancedReasoningOptions> options)
    {
        foreach (var strategy in options.Value.Strategies)
        {
            _strategies[strategy.StrategyId] = Clone(strategy);
        }
    }

    public IReadOnlyCollection<StrategyDefinition> GetStrategies()
    {
        return _strategies.Values.Select(Clone).ToArray();
    }

    public StrategyDefinition? GetStrategy(string strategyId)
    {
        return _strategies.TryGetValue(strategyId, out var strategy) ? Clone(strategy) : null;
    }

    private static StrategyDefinition Clone(StrategyDefinition strategy)
    {
        return new StrategyDefinition
        {
            StrategyId = strategy.StrategyId,
            Name = strategy.Name,
            Description = strategy.Description,
            Mode = strategy.Mode,
            DefaultPrompts = strategy.DefaultPrompts.ToList(),
            Metrics = strategy.Metrics.ToList()
        };
    }
}
