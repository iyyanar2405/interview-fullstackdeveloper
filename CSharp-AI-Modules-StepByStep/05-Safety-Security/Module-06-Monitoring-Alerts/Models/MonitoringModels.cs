namespace Module_06_Monitoring_Alerts.Models;

// ===== Security Event Models =====

public class SecurityEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public EventType Type { get; set; }
    public EventSeverity Severity { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
    public bool IsAnomaly { get; set; }
    public string? CorrelationId { get; set; }
}

public enum EventType
{
    Authentication,
    Authorization,
    DataAccess,
    DataModification,
    SecurityViolation,
    RateLimitExceeded,
    DDoSDetected,
    AnomalyDetected,
    SystemError,
    ConfigurationChange,
    UserActivity,
    ApiCall
}

public enum EventSeverity
{
    Debug,
    Information,
    Warning,
    Error,
    Critical,
    Fatal
}

// ===== Metrics Models =====

public class MetricData
{
    public string MetricId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public MetricType Type { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Tags { get; set; } = new();
    public string? Unit { get; set; }
}

public enum MetricType
{
    Counter,
    Gauge,
    Histogram,
    Summary
}

public class TimeSeriesData
{
    public string SeriesId { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public List<DataPoint> DataPoints { get; set; } = new();
    public TimeSpan Interval { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class DataPoint
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public Dictionary<string, string>? Labels { get; set; }
}

// ===== Alert Models =====

public class Alert
{
    public string AlertId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public AlertStatus Status { get; set; } = AlertStatus.Active;
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? ResolvedBy { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
    public List<string> NotificationChannels { get; set; } = new();
    public int OccurrenceCount { get; set; } = 1;
}

public enum AlertType
{
    SecurityThreat,
    PerformanceDegradation,
    SystemFailure,
    AnomalyDetected,
    ThresholdExceeded,
    ComplianceViolation,
    ResourceExhaustion,
    DataBreach
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical,
    Emergency
}

public enum AlertStatus
{
    Active,
    Acknowledged,
    Resolved,
    Suppressed,
    Escalated
}

public class AlertRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string Condition { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public List<string> NotificationChannels { get; set; } = new();
    public int ThrottleMinutes { get; set; } = 5;
    public DateTime? LastTriggered { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

// ===== Anomaly Detection Models =====

public class AnomalyDetectionResult
{
    public string ResultId { get; set; } = Guid.NewGuid().ToString();
    public bool IsAnomaly { get; set; }
    public double AnomalyScore { get; set; }
    public double Confidence { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public double ObservedValue { get; set; }
    public double ExpectedValue { get; set; }
    public double Deviation { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public AnomalyType Type { get; set; }
    public string? Explanation { get; set; }
}

public enum AnomalyType
{
    Spike,
    Drop,
    Trend,
    Seasonal,
    Outlier,
    Pattern
}

public class BaselineMetrics
{
    public string MetricName { get; set; } = string.Empty;
    public double Mean { get; set; }
    public double Median { get; set; }
    public double StandardDeviation { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int SampleSize { get; set; }
    public DateTime CalculatedAt { get; set; }
}

// ===== Incident Models =====

public class Incident
{
    public string IncidentId { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public IncidentPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? AssignedTo { get; set; }
    public string? CreatedBy { get; set; }
    public List<string> RelatedAlertIds { get; set; } = new();
    public List<string> RelatedEventIds { get; set; } = new();
    public List<IncidentAction> Actions { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string? RootCause { get; set; }
    public string? Resolution { get; set; }
}

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentStatus
{
    Open,
    Acknowledged,
    InProgress,
    Resolved,
    Closed,
    Escalated
}

public enum IncidentPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Emergency = 5
}

public class IncidentAction
{
    public string ActionId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string PerformedBy { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
}

// ===== Compliance Models =====

public class ComplianceReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString();
    public string ReportName { get; set; } = string.Empty;
    public ComplianceFramework Framework { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ComplianceStatus OverallStatus { get; set; }
    public double ComplianceScore { get; set; }
    public Dictionary<string, ControlStatus> Controls { get; set; } = new();
    public List<ComplianceViolation> Violations { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public enum ComplianceFramework
{
    GDPR,
    CCPA,
    HIPAA,
    SOC2,
    ISO27001,
    PCIDSS,
    NIST
}

public enum ComplianceStatus
{
    Compliant,
    PartiallyCompliant,
    NonCompliant,
    NotApplicable
}

public class ControlStatus
{
    public string ControlId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ComplianceStatus Status { get; set; }
    public string? Evidence { get; set; }
    public DateTime LastAssessed { get; set; }
}

public class ComplianceViolation
{
    public string ViolationId { get; set; } = Guid.NewGuid().ToString();
    public string ControlId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ViolationSeverity Severity { get; set; }
    public DateTime DetectedAt { get; set; }
    public string? RemediationPlan { get; set; }
    public DateTime? RemediationDeadline { get; set; }
    public ViolationStatus Status { get; set; } = ViolationStatus.Open;
}

public enum ViolationSeverity
{
    Minor,
    Moderate,
    Major,
    Critical
}

public enum ViolationStatus
{
    Open,
    InRemediation,
    Resolved,
    Accepted
}

// ===== Notification Models =====

public class Notification
{
    public string NotificationId { get; set; } = Guid.NewGuid().ToString();
    public NotificationChannel Channel { get; set; }
    public NotificationPriority Priority { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Recipients { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum NotificationChannel
{
    Email,
    SMS,
    Webhook,
    Slack,
    Teams,
    PagerDuty,
    InApp
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Retrying
}

// ===== Dashboard Models =====

public class DashboardData
{
    public string DashboardId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public SystemHealthStatus SystemHealth { get; set; } = new();
    public SecurityMetrics SecurityMetrics { get; set; } = new();
    public PerformanceMetrics PerformanceMetrics { get; set; } = new();
    public List<Alert> RecentAlerts { get; set; } = new();
    public List<Incident> ActiveIncidents { get; set; } = new();
}

public class SystemHealthStatus
{
    public HealthStatus OverallStatus { get; set; }
    public Dictionary<string, HealthStatus> ComponentStatus { get; set; } = new();
    public DateTime LastChecked { get; set; }
}

public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown
}

public class SecurityMetrics
{
    public int TotalEvents { get; set; }
    public int SecurityViolations { get; set; }
    public int FailedAuthentications { get; set; }
    public int BlockedIps { get; set; }
    public int AnomaliesDetected { get; set; }
    public Dictionary<string, int> EventsByType { get; set; } = new();
}

public class PerformanceMetrics
{
    public double AverageResponseTime { get; set; }
    public int TotalRequests { get; set; }
    public int FailedRequests { get; set; }
    public double ErrorRate { get; set; }
    public double ThroughputPerSecond { get; set; }
    public Dictionary<string, double> EndpointPerformance { get; set; } = new();
}

// ===== Request/Response Models =====

public class EventQuery
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EventType? Type { get; set; }
    public EventSeverity? MinSeverity { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public bool? AnomaliesOnly { get; set; }
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 50;
}

public class MetricQuery
{
    public string MetricName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan? Interval { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}

public class AlertAcknowledgement
{
    public string AlertId { get; set; } = string.Empty;
    public string AcknowledgedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class IncidentUpdate
{
    public string IncidentId { get; set; } = string.Empty;
    public IncidentStatus? NewStatus { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

// ===== Configuration Models =====

public class MonitoringConfiguration
{
    public bool EnableEventMonitoring { get; set; } = true;
    public bool EnableAnomalyDetection { get; set; } = true;
    public bool EnableAlerts { get; set; } = true;
    public EventSeverity MinimumEventSeverity { get; set; } = EventSeverity.Information;
    public int EventRetentionDays { get; set; } = 90;
    public int MetricRetentionDays { get; set; } = 30;
}

public class AlertConfiguration
{
    public List<string> EnabledChannels { get; set; } = new();
    public Dictionary<AlertSeverity, List<string>> SeverityChannels { get; set; } = new();
    public int DefaultThrottleMinutes { get; set; } = 5;
    public bool EnableEscalation { get; set; } = true;
    public int EscalationDelayMinutes { get; set; } = 30;
}

public class NotificationSettings
{
    public EmailSettings? Email { get; set; }
    public SmsSettings? Sms { get; set; }
    public WebhookSettings? Webhook { get; set; }
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
}

public class SmsSettings
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;
}

public class WebhookSettings
{
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}
