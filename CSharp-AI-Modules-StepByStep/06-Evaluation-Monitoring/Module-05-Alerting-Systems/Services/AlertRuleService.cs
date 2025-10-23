using System.Collections.Concurrent;
using Module_05_Alerting_Systems.Models;
using Microsoft.Extensions.Options;

namespace Module_05_Alerting_Systems.Services;

public sealed class AlertRuleService
{
    private readonly ConcurrentDictionary<string, AlertRule> _rules = new(StringComparer.OrdinalIgnoreCase);

    public AlertRuleService(IOptions<AlertingOptions> options)
    {
        foreach (var rule in options.Value.SeedRules)
        {
            _rules[rule.RuleId] = rule;
        }
    }

    public IReadOnlyCollection<AlertRule> GetAll() => _rules.Values.OrderBy(r => r.Name).ToArray();

    public AlertRule? Get(string ruleId) => _rules.TryGetValue(ruleId, out var rule) ? rule : null;

    public AlertRule Add(AlertRule rule)
    {
        rule.RuleId = string.IsNullOrWhiteSpace(rule.RuleId) ? Guid.NewGuid().ToString("N") : rule.RuleId;
        _rules[rule.RuleId] = rule;
        return rule;
    }

    public bool Delete(string ruleId) => _rules.TryRemove(ruleId, out _);

    public AlertRule? Update(string ruleId, AlertRule updated)
    {
        if (!_rules.ContainsKey(ruleId))
        {
            return null;
        }

        updated.RuleId = ruleId;
        _rules[ruleId] = updated;
        return updated;
    }

    public void Toggle(string ruleId, bool enabled)
    {
        if (_rules.TryGetValue(ruleId, out var rule))
        {
            rule.Enabled = enabled;
        }
    }
}
