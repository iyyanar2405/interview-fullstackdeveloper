namespace Module_06_Production_Monitoring.Models;

public sealed class MonitoringOptions
{
    public List<DashboardDefinition> Dashboards { get; set; } = new();
    public List<AlertRule> AlertRules { get; set; } = new();
    public List<ServiceLevelObjective> ServiceLevelObjectives { get; set; } = new();
    public List<MonitoringReportDefinition> Reports { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int MetricIntervalSeconds { get; set; } = 5;
    public double IncidentLikelihood { get; set; } = 0.1;
    public List<string> Regions { get; set; } = new() { "eastus", "westeurope" };
    public List<string> Services { get; set; } = new() { "payments", "checkout" };
    public int? Seed { get; set; }
}

public sealed class DashboardDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WidgetDefinition> Widgets { get; set; } = new();
}

public sealed class WidgetDefinition
{
    public string Title { get; set; } = string.Empty;
    public string Visualization { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
}

public sealed class AlertRule
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public string TargetEntityId { get; set; } = string.Empty;
    public AlertOperator Operator { get; set; } = AlertOperator.GreaterThan;
    public double Threshold { get; set; } = 0;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
}

public enum AlertOperator
{
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public sealed class ServiceLevelObjective
{
    public string Name { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public SloDirection Direction { get; set; } = SloDirection.GreaterThanOrEqual;
    public double Target { get; set; } = 99.0;
    public string Aggregation { get; set; } = "avg";
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(5);
    public string Period { get; set; } = "rolling";
}

public enum SloDirection
{
    GreaterThanOrEqual,
    LessThanOrEqual
}

public sealed class TelemetryMetric
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, string> Dimensions { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class TelemetryLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, string> Context { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class TelemetryTrace
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset StartTimestamp { get; set; } = DateTimeOffset.UtcNow;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public string OperationName { get; set; } = string.Empty;
    public string RootService { get; set; } = string.Empty;
    public string RootServiceId { get; set; } = string.Empty;
    public List<TelemetrySpan> Spans { get; set; } = new();
}

public sealed class TelemetrySpan
{
    public string SpanId { get; set; } = Guid.NewGuid().ToString("N");
    public string ParentSpanId { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DateTimeOffset Start { get; set; } = DateTimeOffset.UtcNow;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public Dictionary<string, string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class TelemetryEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Message { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class MonitoringSnapshot
{
    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<TelemetryMetric> ActiveMetrics { get; set; } = new();
    public List<AlertInstance> Alerts { get; set; } = new();
    public List<AlertInstance> AlertsTriggered { get; set; } = new();
    public List<ActiveIncident> CurrentIncidents { get; set; } = new();
    public List<TelemetryEvent> RecentEvents { get; set; } = new();
    public List<SloStatus> Slos { get; set; } = new();
    public ServiceHealthScore ServiceHealth { get; set; } = new();
}

public sealed class ServiceHealthScore
{
    public string ServiceName { get; set; } = string.Empty;
    public double CurrentScore { get; set; } = 1;
    public string Trend { get; set; } = "steady";
}

public sealed class AlertInstance
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
    public AlertStatus Status { get; set; } = AlertStatus.Active;
    public DateTimeOffset TriggeredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastTriggeredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; set; }
    public string EvaluatedMetric { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double Threshold { get; set; }
    public AlertOperator Operator { get; set; } = AlertOperator.GreaterThan;
    public string TargetEntity { get; set; } = string.Empty;
    public string TargetEntityId { get; set; } = string.Empty;
}

public enum AlertStatus
{
    Active,
    Resolved
}

public sealed class ActiveIncident
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string AlertId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;
    public DateTimeOffset DeclaredAt { get; set; } = DateTimeOffset.UtcNow;
    public string Owner { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string Mitigation { get; set; } = string.Empty;
}

public sealed class PastIncident
{
    public string Id { get; set; } = string.Empty;
    public string AlertId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;
    public DateTimeOffset DeclaredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ResolvedAt { get; set; } = DateTimeOffset.UtcNow;
    public string ResolutionSummary { get; set; } = string.Empty;
    public PostMortemSummary? PostMortem { get; set; }
}

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public sealed class PostMortemSummary
{
    public string IncidentId { get; set; } = string.Empty;
    public string IncidentSummary { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string ImpactSummary { get; set; } = string.Empty;
    public List<string> FollowUpActions { get; set; } = new();
}

public sealed class MonitoringReport
{
    public string ReportType { get; set; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public MonitoringSnapshot Snapshot { get; set; } = new();
    public List<SloStatus> SloStatuses { get; set; } = new();
    public List<AlertInstance> ActiveAlerts { get; set; } = new();
    public List<ActiveIncident> ActiveIncidents { get; set; } = new();
    public List<MonitoringReportSection> Sections { get; set; } = new();
}

public sealed class MonitoringReportSection
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public sealed class MonitoringReportDefinition
{
    public string Type { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IncludeTelemetrySummary { get; set; } = true;
    public bool IncludeSloStatus { get; set; } = true;
    public bool IncludeAlertSummary { get; set; } = true;
    public bool IncludeIncidentSummary { get; set; } = true;
}

public sealed class SloStatus
{
    public string Name { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double Target { get; set; }
    public TimeSpan RollingWindow { get; set; } = TimeSpan.FromMinutes(5);
    public string Period { get; set; } = string.Empty;
    public SloState Status { get; set; } = SloState.Unknown;
    public string StatusReason { get; set; } = string.Empty;
}

public enum SloState
{
    Healthy,
    Breaching,
    Unknown
}

public sealed class TelemetryIngestionRequest
{
    public List<TelemetryMetric> Metrics { get; set; } = new();
    public List<TelemetryLog> Logs { get; set; } = new();
    public List<TelemetryTrace> Traces { get; set; } = new();
    public List<TelemetryEvent> Events { get; set; } = new();
}
