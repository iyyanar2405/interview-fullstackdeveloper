using System.ComponentModel.DataAnnotations;

namespace AI.Configuration.Models;

/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseSettings
{
    public const string SectionName = "Database";

    /// <summary>
    /// Primary database connection string
    /// </summary>
    [Required(ErrorMessage = "Primary connection string is required")]
    public string PrimaryConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Read-only database connection string (optional)
    /// </summary>
    public string? ReadOnlyConnectionString { get; set; }

    /// <summary>
    /// Database provider (SqlServer, PostgreSQL, MySQL, etc.)
    /// </summary>
    [Required(ErrorMessage = "Database provider is required")]
    public string Provider { get; set; } = "SqlServer";

    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    [Range(1, 300, ErrorMessage = "CommandTimeout must be between 1 and 300 seconds")]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts for database operations
    /// </summary>
    [Range(0, 10, ErrorMessage = "MaxRetryAttempts must be between 0 and 10")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Enable sensitive data logging (development only)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Enable detailed errors (development only)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Connection pool settings
    /// </summary>
    public ConnectionPoolSettings ConnectionPool { get; set; } = new();
}

/// <summary>
/// Connection pool configuration
/// </summary>
public class ConnectionPoolSettings
{
    /// <summary>
    /// Maximum pool size
    /// </summary>
    [Range(1, 1000, ErrorMessage = "MaxPoolSize must be between 1 and 1000")]
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Minimum pool size
    /// </summary>
    [Range(0, 100, ErrorMessage = "MinPoolSize must be between 0 and 100")]
    public int MinPoolSize { get; set; } = 0;

    /// <summary>
    /// Connection lifetime in seconds
    /// </summary>
    [Range(0, 86400, ErrorMessage = "ConnectionLifetimeSeconds must be between 0 and 86400")]
    public int ConnectionLifetimeSeconds { get; set; } = 0;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    [Range(1, 300, ErrorMessage = "ConnectionTimeoutSeconds must be between 1 and 300")]
    public int ConnectionTimeoutSeconds { get; set; } = 15;
}

/// <summary>
/// Redis cache configuration
/// </summary>
public class CacheSettings
{
    public const string SectionName = "Cache";

    /// <summary>
    /// Redis connection string
    /// </summary>
    [Required(ErrorMessage = "Redis connection string is required")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Default expiration time in minutes
    /// </summary>
    [Range(1, 10080, ErrorMessage = "DefaultExpirationMinutes must be between 1 and 10080 (1 week)")]
    public int DefaultExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Key prefix for all cache entries
    /// </summary>
    public string KeyPrefix { get; set; } = "ai-app";

    /// <summary>
    /// Enable compression for cached values
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Serialization format (Json, MessagePack, etc.)
    /// </summary>
    public string SerializationFormat { get; set; } = "Json";

    /// <summary>
    /// Maximum retries for cache operations
    /// </summary>
    [Range(0, 10, ErrorMessage = "MaxRetries must be between 0 and 10")]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Retry delay in milliseconds
    /// </summary>
    [Range(100, 5000, ErrorMessage = "RetryDelayMilliseconds must be between 100 and 5000")]
    public int RetryDelayMilliseconds { get; set; } = 1000;
}