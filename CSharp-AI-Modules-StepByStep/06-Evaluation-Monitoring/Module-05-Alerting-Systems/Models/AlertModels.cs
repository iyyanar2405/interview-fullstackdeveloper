namespace Module_05_Alerting_Systems.Models;

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum AlertChannel
{
    Email,
    Slack,
    Webhook,
    PagerDuty
}

public sealed class AlertRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public double Threshold { get; set; }
        = 0;
    public string Comparison { get; set; } = ">"; // >, >=, <, <=, ==
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(5);
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
    public List<AlertChannel> Channels { get; set; } = new();
    public bool Enabled { get; set; } = true;
    public int CooldownMinutes { get; set; } = 10;
}

public sealed class MetricSample
{
    public string Metric { get; set; } = string.Empty;
    public double Value { get; set; }
        = 0;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Dimensions { get; set; } = new();
}

public sealed class AlertNotification
{
    public string AlertId { get; set; } = Guid.NewGuid().ToString("N");
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
        = AlertSeverity.Medium;
    public string Message { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public double Value { get; set; }
        = 0;
    public double Threshold { get; set; }
        = 0;
    public DateTimeOffset TriggeredAt { get; set; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<AlertChannel> Channels { get; set; } = Array.Empty<AlertChannel>();
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public sealed class AlertSummary
{
    public int TotalRules { get; init; }
        = 0;
    public int ActiveRules { get; init; }
        = 0;
    public int AlertsLastHour { get; init; }
        = 0;
    public int AlertsLast24Hours { get; init; }
        = 0;
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class AlertingOptions
{
    public List<AlertRule> SeedRules { get; set; } = new();
    public Dictionary<string, string> Channels { get; set; } = new();
    public int HistorySize { get; set; } = 2500;
}
