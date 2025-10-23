namespace AI.Foundation.API.Models;

/// <summary>
/// Generic API response wrapper
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class APIResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// The actual data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if the request failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Timestamp when the response was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for correlation
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Optional metadata for the response
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Chat response model
/// </summary>
public class ChatResponseModel
{
    /// <summary>
    /// The AI's response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score of the response (0.0 to 1.0)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Time taken to process the request
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model used to generate the response
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Number of tokens used in processing
    /// </summary>
    public int TokensUsed { get; set; }

    /// <summary>
    /// Optional sources or references
    /// </summary>
    public List<string>? Sources { get; set; }
}

/// <summary>
/// Text completion response model
/// </summary>
public class TextCompletionResponse
{
    /// <summary>
    /// The completed text
    /// </summary>
    public string CompletedText { get; set; } = string.Empty;

    /// <summary>
    /// Number of tokens used
    /// </summary>
    public int TokensUsed { get; set; }

    /// <summary>
    /// Model used for completion
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Finish reason
    /// </summary>
    public string FinishReason { get; set; } = "completed";
}

/// <summary>
/// AI Model information
/// </summary>
public class AIModelInfo
{
    /// <summary>
    /// Model name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Model version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Model description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Maximum context length
    /// </summary>
    public int MaxContextLength { get; set; } = 4096;

    /// <summary>
    /// Supported capabilities
    /// </summary>
    public List<string> Capabilities { get; set; } = new();
}

/// <summary>
/// Health check response
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the check
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Application version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Environment name
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}

/// <summary>
/// Detailed health check response
/// </summary>
public class DetailedHealthCheckResponse : HealthCheckResponse
{
    /// <summary>
    /// System information
    /// </summary>
    public SystemInfo? SystemInfo { get; set; }

    /// <summary>
    /// Dependency health status
    /// </summary>
    public List<DependencyHealth> Dependencies { get; set; } = new();
}

/// <summary>
/// System information
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// Machine name
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Number of processors
    /// </summary>
    public int ProcessorCount { get; set; }

    /// <summary>
    /// Operating system version
    /// </summary>
    public string OSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Working set memory
    /// </summary>
    public long WorkingSet { get; set; }

    /// <summary>
    /// Application uptime
    /// </summary>
    public TimeSpan UpTime { get; set; }
}

/// <summary>
/// Dependency health information
/// </summary>
public class DependencyHealth
{
    /// <summary>
    /// Dependency name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Health status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Response time
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Additional details
    /// </summary>
    public string? Details { get; set; }
}