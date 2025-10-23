using Azure.AI.OpenAI;

namespace AI.AzureOpenAI.Models;

/// <summary>
/// Azure OpenAI settings configuration
/// </summary>
public class AzureOpenAISettings
{
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// Azure OpenAI endpoint URL
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// API key for authentication (optional if using managed identity)
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Whether to use managed identity for authentication
    /// </summary>
    public bool UseManagedIdentity { get; set; } = true;

    /// <summary>
    /// Default deployment name for chat completions
    /// </summary>
    public required string DeploymentName { get; set; }

    /// <summary>
    /// Deployment name for embeddings (optional)
    /// </summary>
    public string? EmbeddingDeploymentName { get; set; }

    /// <summary>
    /// Default model name
    /// </summary>
    public string ModelName { get; set; } = "gpt-35-turbo";

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Content filter settings
    /// </summary>
    public ContentFilterSettings ContentFilter { get; set; } = new();

    /// <summary>
    /// Rate limiting settings
    /// </summary>
    public RateLimitSettings RateLimit { get; set; } = new();

    /// <summary>
    /// Logging settings
    /// </summary>
    public LoggingSettings Logging { get; set; } = new();
}

/// <summary>
/// Content filter settings
/// </summary>
public class ContentFilterSettings
{
    /// <summary>
    /// Whether content filtering is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Block threshold for hate content (Low, Medium, High)
    /// </summary>
    public string HateThreshold { get; set; } = "Medium";

    /// <summary>
    /// Block threshold for sexual content
    /// </summary>
    public string SexualThreshold { get; set; } = "Medium";

    /// <summary>
    /// Block threshold for violence content
    /// </summary>
    public string ViolenceThreshold { get; set; } = "Medium";

    /// <summary>
    /// Block threshold for self-harm content
    /// </summary>
    public string SelfHarmThreshold { get; set; } = "Low";

    /// <summary>
    /// Whether to log filtered content
    /// </summary>
    public bool LogFilteredContent { get; set; } = true;
}

/// <summary>
/// Rate limiting settings
/// </summary>
public class RateLimitSettings
{
    /// <summary>
    /// Whether rate limiting is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum requests per minute
    /// </summary>
    public int RequestsPerMinute { get; set; } = 60;

    /// <summary>
    /// Maximum tokens per minute
    /// </summary>
    public int TokensPerMinute { get; set; } = 60000;

    /// <summary>
    /// Whether to queue requests when rate limited
    /// </summary>
    public bool QueueRequests { get; set; } = true;
}

/// <summary>
/// Logging settings
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// Whether to log requests
    /// </summary>
    public bool LogRequests { get; set; } = true;

    /// <summary>
    /// Whether to log responses
    /// </summary>
    public bool LogResponses { get; set; } = true;

    /// <summary>
    /// Whether to log token usage
    /// </summary>
    public bool LogTokenUsage { get; set; } = true;

    /// <summary>
    /// Whether to log performance metrics
    /// </summary>
    public bool LogPerformance { get; set; } = true;

    /// <summary>
    /// Maximum content length to log
    /// </summary>
    public int MaxContentLength { get; set; } = 1000;
}

/// <summary>
/// Azure chat request model
/// </summary>
public class AzureChatRequest
{
    /// <summary>
    /// Chat messages
    /// </summary>
    public required List<ChatRequestMessage> Messages { get; set; }

    /// <summary>
    /// Maximum tokens to generate
    /// </summary>
    public int? MaxTokens { get; set; } = 1000;

    /// <summary>
    /// Sampling temperature (0.0 to 2.0)
    /// </summary>
    public float? Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Top-p sampling parameter
    /// </summary>
    public float? TopP { get; set; } = 1.0f;

    /// <summary>
    /// Frequency penalty (-2.0 to 2.0)
    /// </summary>
    public float? FrequencyPenalty { get; set; } = 0.0f;

    /// <summary>
    /// Presence penalty (-2.0 to 2.0)
    /// </summary>
    public float? PresencePenalty { get; set; } = 0.0f;

    /// <summary>
    /// Stop sequences
    /// </summary>
    public List<string>? StopSequences { get; set; }

    /// <summary>
    /// Whether to stream the response
    /// </summary>
    public bool Stream { get; set; } = false;

    /// <summary>
    /// User identifier for tracking
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Request metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Azure chat response model
/// </summary>
public class AzureChatResponse
{
    /// <summary>
    /// Generated message content
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Model used for generation
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Token usage information
    /// </summary>
    public required AzureTokenUsage Usage { get; set; }

    /// <summary>
    /// Finish reason (stop, length, content_filter, etc.)
    /// </summary>
    public string? FinishReason { get; set; }

    /// <summary>
    /// Processing time
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Request ID for tracking
    /// </summary>
    public required string RequestId { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Content filter results
    /// </summary>
    public List<AzureContentFilterResult> ContentFilterResults { get; set; } = new();

    /// <summary>
    /// Response metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Azure chat stream chunk
/// </summary>
public class AzureChatStreamChunk
{
    /// <summary>
    /// Chunk content
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Model name
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Finish reason (if this is the last chunk)
    /// </summary>
    public string? FinishReason { get; set; }

    /// <summary>
    /// Chunk timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Chunk index in the stream
    /// </summary>
    public int Index { get; set; }
}

/// <summary>
/// Azure embedding request model
/// </summary>
public class AzureEmbeddingRequest
{
    /// <summary>
    /// Input text(s) to embed
    /// </summary>
    public required List<string> Input { get; set; }

    /// <summary>
    /// User identifier for tracking
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Request metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Azure embedding response model
/// </summary>
public class AzureEmbeddingResponse
{
    /// <summary>
    /// Generated embeddings
    /// </summary>
    public required List<float[]> Embeddings { get; set; }

    /// <summary>
    /// Model used for embeddings
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Token usage information
    /// </summary>
    public required AzureTokenUsage Usage { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Response metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Azure token usage information
/// </summary>
public class AzureTokenUsage
{
    /// <summary>
    /// Tokens used in prompt
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// Tokens used in completion
    /// </summary>
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Total tokens used
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// Estimated cost in USD
    /// </summary>
    public decimal? EstimatedCost { get; set; }
}

/// <summary>
/// Azure content filter result
/// </summary>
public class AzureContentFilterResult
{
    /// <summary>
    /// Content category (hate, sexual, violence, self_harm)
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Severity level (safe, low, medium, high)
    /// </summary>
    public required string Severity { get; set; }

    /// <summary>
    /// Whether content was filtered
    /// </summary>
    public bool Filtered { get; set; }
}

/// <summary>
/// Azure deployment information
/// </summary>
public class AzureDeploymentInfo
{
    /// <summary>
    /// Deployment name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Model name
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Deployment status
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Scale settings
    /// </summary>
    public AzureScaleSettings? ScaleSettings { get; set; }
}

/// <summary>
/// Azure scale settings
/// </summary>
public class AzureScaleSettings
{
    /// <summary>
    /// Scale type (Standard, ProvisionedThroughput)
    /// </summary>
    public string ScaleType { get; set; } = "Standard";

    /// <summary>
    /// Capacity in Tokens Per Minute (TPM) for ProvisionedThroughput
    /// </summary>
    public int? Capacity { get; set; }
}

/// <summary>
/// Azure usage metrics
/// </summary>
public class AzureUsageMetrics
{
    /// <summary>
    /// Start date for metrics
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date for metrics
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Total number of requests
    /// </summary>
    public int TotalRequests { get; set; }

    /// <summary>
    /// Total tokens consumed
    /// </summary>
    public long TotalTokens { get; set; }

    /// <summary>
    /// Total cost in USD
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average response time
    /// </summary>
    public TimeSpan AverageResponseTime { get; set; }

    /// <summary>
    /// Error rate percentage
    /// </summary>
    public double ErrorRate { get; set; }

    /// <summary>
    /// Daily usage breakdown
    /// </summary>
    public List<DailyUsage>? DailyUsage { get; set; }
}

/// <summary>
/// Daily usage metrics
/// </summary>
public class DailyUsage
{
    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Number of requests
    /// </summary>
    public int Requests { get; set; }

    /// <summary>
    /// Tokens consumed
    /// </summary>
    public long Tokens { get; set; }

    /// <summary>
    /// Cost in USD
    /// </summary>
    public decimal Cost { get; set; }
}