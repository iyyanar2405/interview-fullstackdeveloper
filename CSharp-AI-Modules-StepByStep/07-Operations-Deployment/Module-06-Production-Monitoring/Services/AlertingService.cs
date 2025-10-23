using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_06_Production_Monitoring.Models;
using System.Linq;

namespace Module_06_Production_Monitoring.Services;

public sealed class AlertingService
{
    private readonly ILogger<AlertingService> _logger;
    private readonly TelemetryStoreService _telemetryStore;
    private readonly MonitoringOptions _options;
    private readonly List<AlertInstance> _activeAlerts = new();

    public AlertingService(
        ILogger<AlertingService> logger,
        TelemetryStoreService telemetryStore,
        IOptions<MonitoringOptions> options)
    {
        _logger = logger;
        _telemetryStore = telemetryStore;
        _options = options.Value;
    }

    public IReadOnlyCollection<AlertInstance> EvaluateAlerts()
    {
        var triggered = new List<AlertInstance>();
        foreach (var rule in _options.AlertRules)
        {
            var instance = EvaluateRule(rule);
            if (instance is not null)
            {
                triggered.Add(instance);
                TrackActiveAlert(instance);
            }
        }

        // Remove alerts that have resolved.
        _activeAlerts.RemoveAll(alert => alert.Status == AlertStatus.Resolved && alert.ResolvedAt < DateTimeOffset.UtcNow.AddMinutes(-5));
        return triggered;
    }

    public IReadOnlyCollection<AlertInstance> GetActiveAlerts()
    {
        return _activeAlerts.Select(Clone).ToArray();
    }

    private AlertInstance? EvaluateRule(AlertRule rule)
    {
        var metrics = _telemetryStore.FilterMetrics(metric =>
            metric.Name.Equals(rule.Metric, StringComparison.OrdinalIgnoreCase) &&
            metric.EntityId.Equals(rule.TargetEntityId, StringComparison.OrdinalIgnoreCase));

        if (metrics.Count == 0)
        {
            _logger.LogDebug("Alert rule {Rule} skipped. No metrics found.", rule.Name);
            return null;
        }

        var latest = metrics.OrderByDescending(m => m.Timestamp).First();
        var isTriggered = rule.Operator switch
        {
            AlertOperator.GreaterThan => latest.Value > rule.Threshold,
            AlertOperator.GreaterThanOrEqual => latest.Value >= rule.Threshold,
            AlertOperator.LessThan => latest.Value < rule.Threshold,
            AlertOperator.LessThanOrEqual => latest.Value <= rule.Threshold,
            _ => false
        };

        var existing = _activeAlerts.FirstOrDefault(alert => alert.Id == rule.Name);

        if (!isTriggered)
        {
            if (existing is not null && existing.Status == AlertStatus.Active)
            {
                existing.Status = AlertStatus.Resolved;
                existing.ResolvedAt = DateTimeOffset.UtcNow;
                _logger.LogInformation("Alert {Name} resolved.", rule.Name);
            }

            return null;
        }

        if (existing is not null && existing.Status == AlertStatus.Active)
        {
            existing.LastTriggeredAt = DateTimeOffset.UtcNow;
            existing.EvaluatedMetric = latest.Name;
            existing.CurrentValue = latest.Value;
            return Clone(existing);
        }

        var instance = new AlertInstance
        {
            Id = rule.Name,
            Name = rule.Name,
            Severity = rule.Severity,
            Status = AlertStatus.Active,
            TriggeredAt = DateTimeOffset.UtcNow,
            LastTriggeredAt = DateTimeOffset.UtcNow,
            EvaluatedMetric = latest.Name,
            CurrentValue = latest.Value,
            Threshold = rule.Threshold,
            Operator = rule.Operator,
            TargetEntity = rule.TargetEntity,
            TargetEntityId = rule.TargetEntityId
        };

        _logger.LogWarning("Alert {Name} triggered at value {Value}", rule.Name, latest.Value);
        return instance;
    }

    private void TrackActiveAlert(AlertInstance alert)
    {
        var existing = _activeAlerts.Find(a => a.Id == alert.Id);
        if (existing is null)
        {
            _activeAlerts.Add(Clone(alert));
        }
        else
        {
            existing.Status = alert.Status;
            existing.LastTriggeredAt = alert.LastTriggeredAt;
            existing.CurrentValue = alert.CurrentValue;
        }
    }

    private static AlertInstance Clone(AlertInstance alert)
    {
        return new AlertInstance
        {
            Id = alert.Id,
            Name = alert.Name,
            Severity = alert.Severity,
            Status = alert.Status,
            TriggeredAt = alert.TriggeredAt,
            LastTriggeredAt = alert.LastTriggeredAt,
            ResolvedAt = alert.ResolvedAt,
            EvaluatedMetric = alert.EvaluatedMetric,
            CurrentValue = alert.CurrentValue,
            Threshold = alert.Threshold,
            Operator = alert.Operator,
            TargetEntity = alert.TargetEntity,
            TargetEntityId = alert.TargetEntityId
        };
    }
}
