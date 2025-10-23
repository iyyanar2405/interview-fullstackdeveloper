namespace AI.Foundation.API.Models;

/// <summary>
/// Standard error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Error code (optional)
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Detailed error description
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for correlation
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Validation errors (if applicable)
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    /// <summary>
    /// Additional metadata about the error
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// API error codes enumeration
/// </summary>
public static class ErrorCodes
{
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string ModelNotFound = "MODEL_NOT_FOUND";
    public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string TimeoutError = "TIMEOUT_ERROR";
    public const string QuotaExceeded = "QUOTA_EXCEEDED";
}

/// <summary>
/// Exception details for debugging (only in development)
/// </summary>
public class ExceptionDetails
{
    /// <summary>
    /// Exception type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Exception message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Stack trace
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Inner exception details
    /// </summary>
    public ExceptionDetails? InnerException { get; set; }
}