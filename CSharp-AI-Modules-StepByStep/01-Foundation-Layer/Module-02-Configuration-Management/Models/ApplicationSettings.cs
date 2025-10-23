using System.ComponentModel.DataAnnotations;

namespace AI.Configuration.Models;

/// <summary>
/// Main application configuration settings
/// </summary>
public class ApplicationSettings
{
    public const string SectionName = "Application";

    /// <summary>
    /// Application name
    /// </summary>
    [Required(ErrorMessage = "Application Name is required")]
    public string Name { get; set; } = "AI Application";

    /// <summary>
    /// Application version
    /// </summary>
    [Required(ErrorMessage = "Application Version is required")]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Environment (Development, Staging, Production)
    /// </summary>
    [Required(ErrorMessage = "Environment is required")]
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Base URL for the application
    /// </summary>
    [Url(ErrorMessage = "BaseUrl must be a valid URL")]
    public string BaseUrl { get; set; } = "https://localhost:7001";

    /// <summary>
    /// Enable detailed logging
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Enable performance monitoring
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// Enable health checks
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// Enable metrics collection
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Feature flags
    /// </summary>
    public FeatureFlags Features { get; set; } = new();

    /// <summary>
    /// Monitoring settings
    /// </summary>
    public MonitoringSettings Monitoring { get; set; } = new();

    /// <summary>
    /// Notification settings
    /// </summary>
    public NotificationSettings Notifications { get; set; } = new();
}

/// <summary>
/// Feature flags configuration
/// </summary>
public class FeatureFlags
{
    /// <summary>
    /// Enable AI chat functionality
    /// </summary>
    public bool EnableAIChat { get; set; } = true;

    /// <summary>
    /// Enable document processing
    /// </summary>
    public bool EnableDocumentProcessing { get; set; } = true;

    /// <summary>
    /// Enable function calling
    /// </summary>
    public bool EnableFunctionCalling { get; set; } = false;

    /// <summary>
    /// Enable streaming responses
    /// </summary>
    public bool EnableStreamingResponses { get; set; } = true;

    /// <summary>
    /// Enable advanced analytics
    /// </summary>
    public bool EnableAdvancedAnalytics { get; set; } = false;

    /// <summary>
    /// Enable A/B testing
    /// </summary>
    public bool EnableABTesting { get; set; } = false;

    /// <summary>
    /// Enable beta features
    /// </summary>
    public bool EnableBetaFeatures { get; set; } = false;
}

/// <summary>
/// Monitoring and observability settings
/// </summary>
public class MonitoringSettings
{
    /// <summary>
    /// Application Insights connection string
    /// </summary>
    public string? ApplicationInsightsConnectionString { get; set; }

    /// <summary>
    /// Enable distributed tracing
    /// </summary>
    public bool EnableDistributedTracing { get; set; } = true;

    /// <summary>
    /// Trace sample rate (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "TraceSampleRate must be between 0.0 and 1.0")]
    public double TraceSampleRate { get; set; } = 0.1;

    /// <summary>
    /// Enable custom metrics
    /// </summary>
    public bool EnableCustomMetrics { get; set; } = true;

    /// <summary>
    /// Metrics collection interval in seconds
    /// </summary>
    [Range(5, 300, ErrorMessage = "MetricsIntervalSeconds must be between 5 and 300")]
    public int MetricsIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Log retention days
    /// </summary>
    [Range(1, 365, ErrorMessage = "LogRetentionDays must be between 1 and 365")]
    public int LogRetentionDays { get; set; } = 30;
}

/// <summary>
/// Notification configuration
/// </summary>
public class NotificationSettings
{
    /// <summary>
    /// Email notification settings
    /// </summary>
    public EmailSettings Email { get; set; } = new();

    /// <summary>
    /// Slack notification settings
    /// </summary>
    public SlackSettings Slack { get; set; } = new();

    /// <summary>
    /// SMS notification settings
    /// </summary>
    public SmsSettings Sms { get; set; } = new();
}

/// <summary>
/// Email notification settings
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Enable email notifications
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// SMTP server host
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    [Range(1, 65535, ErrorMessage = "SmtpPort must be between 1 and 65535")]
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// SMTP username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password (should be in secrets)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Use SSL/TLS
    /// </summary>
    public bool UseSSL { get; set; } = true;

    /// <summary>
    /// From email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Default recipients for alerts
    /// </summary>
    public List<string> DefaultRecipients { get; set; } = new();
}

/// <summary>
/// Slack notification settings
/// </summary>
public class SlackSettings
{
    /// <summary>
    /// Enable Slack notifications
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Slack webhook URL (should be in secrets)
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Default channel for notifications
    /// </summary>
    public string DefaultChannel { get; set; } = "#alerts";

    /// <summary>
    /// Bot username
    /// </summary>
    public string Username { get; set; } = "AI-Bot";

    /// <summary>
    /// Bot icon emoji
    /// </summary>
    public string IconEmoji { get; set; } = ":robot_face:";
}

/// <summary>
/// SMS notification settings
/// </summary>
public class SmsSettings
{
    /// <summary>
    /// Enable SMS notifications
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// SMS provider (Twilio, Azure, etc.)
    /// </summary>
    public string Provider { get; set; } = "Twilio";

    /// <summary>
    /// Provider-specific settings
    /// </summary>
    public Dictionary<string, string> ProviderSettings { get; set; } = new();

    /// <summary>
    /// Default phone numbers for alerts
    /// </summary>
    public List<string> DefaultRecipients { get; set; } = new();
}