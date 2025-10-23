using Module_06_Monitoring_Alerts.Models;
using System.Collections.Concurrent;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service responsible for creating and dispatching alerts across multiple channels
/// </summary>
public class AlertService
{
    private readonly ConcurrentDictionary<string, Alert> _alerts = new();
    private readonly ConcurrentDictionary<string, AlertRule> _alertRules = new();
    private readonly ConcurrentDictionary<string, NotificationChannel> _channelOverrides = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastTriggered = new();
    private readonly NotificationSettings _settings;

    public AlertService(NotificationSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Register a new alert rule
    /// </summary>
    public Task<AlertRule> CreateAlertRuleAsync(AlertRule rule)
    {
        rule.RuleId = rule.RuleId ?? Guid.NewGuid().ToString();
        _alertRules[rule.RuleId] = rule;
        return Task.FromResult(rule);
    }

    /// <summary>
    /// Get all alert rules
    /// </summary>
    public Task<List<AlertRule>> GetAlertRulesAsync()
    {
        return Task.FromResult(_alertRules.Values.OrderBy(r => r.Name).ToList());
    }

    /// <summary>
    /// Delete alert rule by ID
    /// </summary>
    public Task<bool> DeleteAlertRuleAsync(string ruleId)
    {
        return Task.FromResult(_alertRules.TryRemove(ruleId, out _));
    }

    /// <summary>
    /// Trigger an alert manually or via rule evaluation
    /// </summary>
    public async Task<Alert> TriggerAlertAsync(Alert alert)
    {
        await Task.CompletedTask;

        alert.AlertId = alert.AlertId ?? Guid.NewGuid().ToString();
        alert.TriggeredAt = DateTime.UtcNow;
        alert.Status = AlertStatus.Active;

        if (alert.NotificationChannels.Count == 0)
        {
            alert.NotificationChannels = ResolveNotificationChannels(alert.Severity);
        }

        _alerts[alert.AlertId] = alert;

        await DispatchNotificationsAsync(alert);

        return alert;
    }

    /// <summary>
    /// Acknowledge an alert
    /// </summary>
    public Task<Alert?> AcknowledgeAlertAsync(string alertId, string acknowledgedBy)
    {
        if (_alerts.TryGetValue(alertId, out var alert))
        {
            alert.Status = AlertStatus.Acknowledged;
            alert.AcknowledgedBy = acknowledgedBy;
            alert.AcknowledgedAt = DateTime.UtcNow;
        }

        return Task.FromResult(alert);
    }

    /// <summary>
    /// Resolve an alert
    /// </summary>
    public Task<Alert?> ResolveAlertAsync(string alertId, string resolvedBy)
    {
        if (_alerts.TryGetValue(alertId, out var alert))
        {
            alert.Status = AlertStatus.Resolved;
            alert.ResolvedBy = resolvedBy;
            alert.ResolvedAt = DateTime.UtcNow;
        }

        return Task.FromResult(alert);
    }

    /// <summary>
    /// Get active alerts
    /// </summary>
    public Task<List<Alert>> GetActiveAlertsAsync()
    {
        return Task.FromResult(_alerts.Values
            .Where(a => a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged)
            .OrderByDescending(a => a.TriggeredAt)
            .ToList());
    }

    /// <summary>
    /// Get alert history
    /// </summary>
    public Task<List<Alert>> GetAlertHistoryAsync(DateTime? start = null, DateTime? end = null)
    {
        var alerts = _alerts.Values.AsEnumerable();

        if (start.HasValue)
            alerts = alerts.Where(a => a.TriggeredAt >= start.Value);

        if (end.HasValue)
            alerts = alerts.Where(a => a.TriggeredAt <= end.Value);

        return Task.FromResult(alerts.OrderByDescending(a => a.TriggeredAt).ToList());
    }

    /// <summary>
    /// Evaluate rules against incoming metrics/events
    /// </summary>
    public async Task EvaluateRulesAsync(string contextKey, double value, Dictionary<string, object> metadata)
    {
        foreach (var rule in _alertRules.Values.Where(r => r.Enabled))
        {
            if (IsThrottled(rule.RuleId, rule.ThrottleMinutes))
                continue;

            if (EvaluateCondition(rule.Condition, contextKey, value, metadata))
            {
                var alert = new Alert
                {
                    Name = rule.Name,
                    Message = rule.Description,
                    Severity = rule.Severity,
                    Type = AlertType.ThresholdExceeded,
                    Context = metadata,
                    NotificationChannels = rule.NotificationChannels
                };

                await TriggerAlertAsync(alert);
                _lastTriggered[rule.RuleId] = DateTime.UtcNow;
            }
        }
    }

    // Helper methods

    private bool EvaluateCondition(string condition, string contextKey, double value, Dictionary<string, object> metadata)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return false;

        var tokens = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length < 3)
            return false;

        var left = tokens[0];
        var op = tokens[1];
        var right = tokens[2];

        double leftValue = 0;

        if (string.Equals(left, "value", StringComparison.OrdinalIgnoreCase))
        {
            leftValue = value;
        }
        else if (metadata.TryGetValue(left, out var metaValue) && double.TryParse(metaValue?.ToString(), out var parsed))
        {
            leftValue = parsed;
        }
        else if (string.Equals(left, "context", StringComparison.OrdinalIgnoreCase) && double.TryParse(contextKey, out var contextValue))
        {
            leftValue = contextValue;
        }

        if (!double.TryParse(right, out var rightValue))
            return false;

        return op switch
        {
            ">" => leftValue > rightValue,
            ">=" => leftValue >= rightValue,
            "<" => leftValue < rightValue,
            "<=" => leftValue <= rightValue,
            "==" => Math.Abs(leftValue - rightValue) < 0.0001,
            "!=" => Math.Abs(leftValue - rightValue) > 0.0001,
            _ => false
        };
    }

    private bool IsThrottled(string ruleId, int throttleMinutes)
    {
        if (!_lastTriggered.TryGetValue(ruleId, out var lastTriggered))
            return false;

        return (DateTime.UtcNow - lastTriggered).TotalMinutes < throttleMinutes;
    }

    private List<string> ResolveNotificationChannels(AlertSeverity severity)
    {
        if (_channelOverrides.TryGetValue(severity.ToString(), out var channel))
        {
            return new List<string> { channel.ToString() };
        }

        return severity switch
        {
            AlertSeverity.Low => new List<string> { NotificationChannel.Email.ToString() },
            AlertSeverity.Medium => new List<string> { NotificationChannel.Email.ToString(), NotificationChannel.Slack.ToString() },
            AlertSeverity.High => new List<string> { NotificationChannel.Email.ToString(), NotificationChannel.Slack.ToString(), NotificationChannel.SMS.ToString() },
            AlertSeverity.Critical => new List<string> { NotificationChannel.Email.ToString(), NotificationChannel.Slack.ToString(), NotificationChannel.SMS.ToString(), NotificationChannel.PagerDuty.ToString() },
            AlertSeverity.Emergency => new List<string> { NotificationChannel.Email.ToString(), NotificationChannel.Slack.ToString(), NotificationChannel.SMS.ToString(), NotificationChannel.PagerDuty.ToString(), NotificationChannel.Teams.ToString() },
            _ => new List<string> { NotificationChannel.Email.ToString() }
        };
    }

    private async Task DispatchNotificationsAsync(Alert alert)
    {
        foreach (var channelName in alert.NotificationChannels)
        {
            Enum.TryParse<NotificationChannel>(channelName, ignoreCase: true, out var channel);

            switch (channel)
            {
                case NotificationChannel.Email:
                    await SendEmailNotificationAsync(alert);
                    break;
                case NotificationChannel.SMS:
                    await SendSmsNotificationAsync(alert);
                    break;
                case NotificationChannel.Webhook:
                    await SendWebhookNotificationAsync(alert);
                    break;
                case NotificationChannel.Slack:
                case NotificationChannel.Teams:
                case NotificationChannel.PagerDuty:
                case NotificationChannel.InApp:
                    await LogNotificationAsync(alert, channel);
                    break;
            }
        }
    }

    private Task SendEmailNotificationAsync(Alert alert)
    {
        if (_settings.Email is null)
            return Task.CompletedTask;

        // Placeholder for actual email implementation
        return Task.CompletedTask;
    }

    private Task SendSmsNotificationAsync(Alert alert)
    {
        if (_settings.Sms is null)
            return Task.CompletedTask;

        // Placeholder for actual SMS implementation
        return Task.CompletedTask;
    }

    private Task SendWebhookNotificationAsync(Alert alert)
    {
        if (_settings.Webhook is null)
            return Task.CompletedTask;

        // Placeholder for webhook call
        return Task.CompletedTask;
    }

    private Task LogNotificationAsync(Alert alert, NotificationChannel channel)
    {
        // Placeholder for chat/incident tool integration
        return Task.CompletedTask;
    }
}
