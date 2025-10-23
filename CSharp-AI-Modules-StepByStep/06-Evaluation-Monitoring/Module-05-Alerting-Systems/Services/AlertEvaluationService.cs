using System.Collections.Concurrent;
using Module_05_Alerting_Systems.Models;

namespace Module_05_Alerting_Systems.Services;

public sealed class AlertEvaluationService
{
    private readonly AlertRuleService _rules;
    private readonly MetricRepository _metrics;
    private readonly AlertDispatchService _dispatch;
    private readonly AlertHistoryService _history;
    private readonly ILogger<AlertEvaluationService> _logger;
    private readonly ConcurrentDictionary<string, DateTimeOffset> _cooldowns = new();

    public AlertEvaluationService(
        AlertRuleService rules,
        MetricRepository metrics,
        AlertDispatchService dispatch,
        AlertHistoryService history,
        ILogger<AlertEvaluationService> logger)
    {
        _rules = rules;
        _metrics = metrics;
        _dispatch = dispatch;
        _history = history;
        _logger = logger;
    }

    public async Task EvaluateAsync(MetricSample sample, CancellationToken cancellationToken = default)
    {
        _metrics.AddSample(sample);
        var rules = _rules.GetAll().Where(r => r.Enabled && string.Equals(r.Metric, sample.Metric, StringComparison.OrdinalIgnoreCase)).ToList();
        if (rules.Count == 0)
        {
            return;
        }

        foreach (var rule in rules)
        {
            if (IsCoolingDown(rule))
            {
                continue;
            }

            var windowSamples = _metrics.GetSamples(rule.Metric, rule.Window);
            if (windowSamples.Count == 0)
            {
                continue;
            }

            var aggregate = windowSamples.Average(s => s.Value);
            if (IsTrigger(rule, aggregate))
            {
                var notification = new AlertNotification
                {
                    RuleId = rule.RuleId,
                    RuleName = rule.Name,
                    Severity = rule.Severity,
                    Metric = rule.Metric,
                    Threshold = rule.Threshold,
                    Value = Math.Round(aggregate, 2),
                    Channels = rule.Channels,
                    Message = $"Rule '{rule.Name}' triggered. {rule.Metric} {ComparisonDescription(rule)} {rule.Threshold}. Avg {aggregate:F2}"
                };

                _history.Record(notification);
                await _dispatch.DispatchAsync(notification, cancellationToken);
                _cooldowns[rule.RuleId] = DateTimeOffset.UtcNow;
                _logger.LogWarning("Alert triggered: {Message}", notification.Message);
            }
        }
    }

    private static bool IsTrigger(AlertRule rule, double aggregate)
    {
        return rule.Comparison switch
        {
            ">" => aggregate > rule.Threshold,
            ">=" => aggregate >= rule.Threshold,
            "<" => aggregate < rule.Threshold,
            "<=" => aggregate <= rule.Threshold,
            "==" => Math.Abs(aggregate - rule.Threshold) < 0.0001,
            _ => false
        };
    }

    private bool IsCoolingDown(AlertRule rule)
    {
        if (!_cooldowns.TryGetValue(rule.RuleId, out var last))
        {
            return false;
        }

        var elapsed = DateTimeOffset.UtcNow - last;
        return elapsed < TimeSpan.FromMinutes(Math.Max(1, rule.CooldownMinutes));
    }

    private static string ComparisonDescription(AlertRule rule)
    {
        return rule.Comparison switch
        {
            ">" => $"> {rule.Threshold}",
            ">=" => $">= {rule.Threshold}",
            "<" => $"< {rule.Threshold}",
            "<=" => $"<= {rule.Threshold}",
            "==" => $"== {rule.Threshold}",
            _ => ""
        };
    }
}
