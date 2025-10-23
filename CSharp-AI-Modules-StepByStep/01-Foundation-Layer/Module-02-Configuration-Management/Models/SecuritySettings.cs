using System.ComponentModel.DataAnnotations;

namespace AI.Configuration.Models;

/// <summary>
/// Security configuration settings
/// </summary>
public class SecuritySettings
{
    public const string SectionName = "Security";

    /// <summary>
    /// JWT authentication settings
    /// </summary>
    public JwtSettings Jwt { get; set; } = new();

    /// <summary>
    /// API key settings
    /// </summary>
    public ApiKeySettings ApiKeys { get; set; } = new();

    /// <summary>
    /// CORS settings
    /// </summary>
    public CorsSettings Cors { get; set; } = new();

    /// <summary>
    /// Rate limiting settings
    /// </summary>
    public RateLimitSettings RateLimit { get; set; } = new();

    /// <summary>
    /// Content security settings
    /// </summary>
    public ContentSecuritySettings ContentSecurity { get; set; } = new();
}

/// <summary>
/// JWT token configuration
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// JWT signing key (should be stored in secrets)
    /// </summary>
    [Required(ErrorMessage = "JWT SecretKey is required")]
    [MinLength(32, ErrorMessage = "JWT SecretKey must be at least 32 characters")]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer
    /// </summary>
    [Required(ErrorMessage = "JWT Issuer is required")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience
    /// </summary>
    [Required(ErrorMessage = "JWT Audience is required")]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes
    /// </summary>
    [Range(1, 43200, ErrorMessage = "ExpirationMinutes must be between 1 and 43200 (30 days)")]
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days
    /// </summary>
    [Range(1, 365, ErrorMessage = "RefreshTokenExpirationDays must be between 1 and 365")]
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Clock skew tolerance in minutes
    /// </summary>
    [Range(0, 10, ErrorMessage = "ClockSkewMinutes must be between 0 and 10")]
    public int ClockSkewMinutes { get; set; } = 5;
}

/// <summary>
/// API key authentication settings
/// </summary>
public class ApiKeySettings
{
    /// <summary>
    /// Header name for API key
    /// </summary>
    public string HeaderName { get; set; } = "X-API-Key";

    /// <summary>
    /// Valid API keys (should be stored in secrets)
    /// </summary>
    public List<string> ValidKeys { get; set; } = new();

    /// <summary>
    /// Enable API key authentication
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Require API key for all endpoints
    /// </summary>
    public bool RequireForAllEndpoints { get; set; } = false;

    /// <summary>
    /// Endpoints that bypass API key requirement
    /// </summary>
    public List<string> BypassEndpoints { get; set; } = new()
    {
        "/health",
        "/swagger"
    };
}

/// <summary>
/// CORS configuration
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// Allowed origins
    /// </summary>
    public List<string> AllowedOrigins { get; set; } = new() { "*" };

    /// <summary>
    /// Allowed methods
    /// </summary>
    public List<string> AllowedMethods { get; set; } = new() { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

    /// <summary>
    /// Allowed headers
    /// </summary>
    public List<string> AllowedHeaders { get; set; } = new() { "*" };

    /// <summary>
    /// Allow credentials
    /// </summary>
    public bool AllowCredentials { get; set; } = false;

    /// <summary>
    /// Preflight max age in seconds
    /// </summary>
    [Range(0, 86400, ErrorMessage = "PreflightMaxAge must be between 0 and 86400")]
    public int PreflightMaxAge { get; set; } = 600;
}

/// <summary>
/// Rate limiting configuration
/// </summary>
public class RateLimitSettings
{
    /// <summary>
    /// Enable rate limiting
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// General rate limit rules
    /// </summary>
    public GeneralRateLimit General { get; set; } = new();

    /// <summary>
    /// Endpoint-specific rate limits
    /// </summary>
    public Dictionary<string, EndpointRateLimit> Endpoints { get; set; } = new();

    /// <summary>
    /// IP whitelist (bypass rate limiting)
    /// </summary>
    public List<string> WhitelistedIPs { get; set; } = new();
}

/// <summary>
/// General rate limiting rules
/// </summary>
public class GeneralRateLimit
{
    /// <summary>
    /// Requests per minute
    /// </summary>
    [Range(1, 10000, ErrorMessage = "RequestsPerMinute must be between 1 and 10000")]
    public int RequestsPerMinute { get; set; } = 60;

    /// <summary>
    /// Requests per hour
    /// </summary>
    [Range(1, 100000, ErrorMessage = "RequestsPerHour must be between 1 and 100000")]
    public int RequestsPerHour { get; set; } = 1000;

    /// <summary>
    /// Requests per day
    /// </summary>
    [Range(1, 1000000, ErrorMessage = "RequestsPerDay must be between 1 and 1000000")]
    public int RequestsPerDay { get; set; } = 10000;
}

/// <summary>
/// Endpoint-specific rate limiting
/// </summary>
public class EndpointRateLimit
{
    /// <summary>
    /// Endpoint pattern
    /// </summary>
    public string Pattern { get; set; } = string.Empty;

    /// <summary>
    /// Requests per minute for this endpoint
    /// </summary>
    public int RequestsPerMinute { get; set; } = 30;

    /// <summary>
    /// Requests per hour for this endpoint
    /// </summary>
    public int RequestsPerHour { get; set; } = 500;
}

/// <summary>
/// Content security settings
/// </summary>
public class ContentSecuritySettings
{
    /// <summary>
    /// Enable content filtering
    /// </summary>
    public bool EnableContentFiltering { get; set; } = true;

    /// <summary>
    /// Enable PII detection
    /// </summary>
    public bool EnablePIIDetection { get; set; } = true;

    /// <summary>
    /// Enable toxicity detection
    /// </summary>
    public bool EnableToxicityDetection { get; set; } = true;

    /// <summary>
    /// Maximum input length
    /// </summary>
    [Range(1, 100000, ErrorMessage = "MaxInputLength must be between 1 and 100000")]
    public int MaxInputLength { get; set; } = 10000;

    /// <summary>
    /// Blocked words list
    /// </summary>
    public List<string> BlockedWords { get; set; } = new();

    /// <summary>
    /// Content filter severity threshold (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "FilterThreshold must be between 0.0 and 1.0")]
    public double FilterThreshold { get; set; } = 0.7;
}