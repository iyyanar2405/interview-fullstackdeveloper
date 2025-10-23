using System.Text.Json.Serialization;

namespace AI.Anthropic.Models;

/// <summary>
/// Anthropic Claude settings configuration
/// </summary>
public class AnthropicSettings
{
    public const string SectionName = "Anthropic";

    /// <summary>
    /// Anthropic API key
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// Base URL for Anthropic API (default: https://api.anthropic.com)
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.anthropic.com";

    /// <summary>
    /// Default model to use
    /// </summary>
    public string DefaultModel { get; set; } = "claude-3-5-sonnet-20241022";

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Maximum tokens per request
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Default temperature for generation
    /// </summary>
    public double DefaultTemperature { get; set; } = 0.7;

    /// <summary>
    /// Rate limiting settings
    /// </summary>
    public RateLimitSettings RateLimit { get; set; } = new();

    /// <summary>
    /// Constitutional AI settings
    /// </summary>
    public ConstitutionalAISettings ConstitutionalAI { get; set; } = new();

    /// <summary>
    /// Logging settings
    /// </summary>
    public LoggingSettings Logging { get; set; } = new();
}

/// <summary>
/// Rate limiting settings
/// </summary>
public class RateLimitSettings
{
    public bool Enabled { get; set; } = true;
    public int RequestsPerMinute { get; set; } = 60;
    public int TokensPerMinute { get; set; } = 40000;
    public bool QueueRequests { get; set; } = true;
}

/// <summary>
/// Constitutional AI settings
/// </summary>
public class ConstitutionalAISettings
{
    public bool Enabled { get; set; } = true;
    public List<string> Principles { get; set; } = new()
    {
        "Be helpful, harmless, and honest",
        "Avoid generating harmful, illegal, or unethical content",
        "Respect human autonomy and dignity",
        "Be transparent about limitations"
    };
    public bool ValidateResponses { get; set; } = true;
}

/// <summary>
/// Logging settings
/// </summary>
public class LoggingSettings
{
    public bool LogRequests { get; set; } = true;
    public bool LogResponses { get; set; } = true;
    public bool LogTokenUsage { get; set; } = true;
    public bool LogPerformance { get; set; } = true;
    public int MaxContentLength { get; set; } = 1000;
}

/// <summary>
/// Claude chat request model
/// </summary>
public class ClaudeRequest
{
    /// <summary>
    /// Model to use for the request
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    /// <summary>
    /// Maximum number of tokens to generate
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// System prompt for the conversation
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; set; }

    /// <summary>
    /// Messages in the conversation
    /// </summary>
    [JsonPropertyName("messages")]
    public required List<ClaudeMessage> Messages { get; set; }

    /// <summary>
    /// Temperature for generation (0.0 to 1.0)
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// Top-p sampling parameter
    /// </summary>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// Top-k sampling parameter
    /// </summary>
    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    /// <summary>
    /// Stop sequences
    /// </summary>
    [JsonPropertyName("stop_sequences")]
    public List<string>? StopSequences { get; set; }

    /// <summary>
    /// Whether to stream the response
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    /// <summary>
    /// Metadata for the request
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Claude message model
/// </summary>
public class ClaudeMessage
{
    /// <summary>
    /// Role of the message (user, assistant)
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// Content of the message
    /// </summary>
    [JsonPropertyName("content")]
    public required object Content { get; set; }
}

/// <summary>
/// Claude response model
/// </summary>
public class ClaudeResponse
{
    /// <summary>
    /// Response ID
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Response type
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Model used for generation
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    /// <summary>
    /// Role of the response (typically "assistant")
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// Generated content
    /// </summary>
    [JsonPropertyName("content")]
    public required List<ClaudeContent> Content { get; set; }

    /// <summary>
    /// Stop reason
    /// </summary>
    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }

    /// <summary>
    /// Stop sequence that triggered the stop
    /// </summary>
    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; set; }

    /// <summary>
    /// Token usage information
    /// </summary>
    [JsonPropertyName("usage")]
    public required ClaudeUsage Usage { get; set; }
}

/// <summary>
/// Claude content model
/// </summary>
public class ClaudeContent
{
    /// <summary>
    /// Content type (text, image, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Text content
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Additional properties for other content types
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Claude usage model
/// </summary>
public class ClaudeUsage
{
    /// <summary>
    /// Input tokens count
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    /// <summary>
    /// Output tokens count
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }

    /// <summary>
    /// Total tokens count
    /// </summary>
    public int TotalTokens => InputTokens + OutputTokens;
}

/// <summary>
/// Claude stream event model
/// </summary>
public class ClaudeStreamEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Event data
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    /// <summary>
    /// Delta content for streaming
    /// </summary>
    [JsonPropertyName("delta")]
    public ClaudeContent? Delta { get; set; }

    /// <summary>
    /// Message for message_start events
    /// </summary>
    [JsonPropertyName("message")]
    public ClaudeResponse? Message { get; set; }

    /// <summary>
    /// Usage information for message_delta events
    /// </summary>
    [JsonPropertyName("usage")]
    public ClaudeUsage? Usage { get; set; }
}

/// <summary>
/// Claude error response model
/// </summary>
public class ClaudeError
{
    /// <summary>
    /// Error type
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}

/// <summary>
/// High-level Claude request model
/// </summary>
public class ClaudeChatRequest
{
    /// <summary>
    /// System prompt for the conversation
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// User message
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Conversation history
    /// </summary>
    public List<ConversationMessage>? History { get; set; }

    /// <summary>
    /// Model to use (optional, uses default if not specified)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Maximum tokens to generate
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Temperature for generation
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Whether to stream the response
    /// </summary>
    public bool Stream { get; set; } = false;

    /// <summary>
    /// Constitutional AI principles to apply
    /// </summary>
    public List<string>? Principles { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// High-level Claude response model
/// </summary>
public class ClaudeChatResponse
{
    /// <summary>
    /// Generated response text
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Model used for generation
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Token usage information
    /// </summary>
    public required ClaudeUsage Usage { get; set; }

    /// <summary>
    /// Stop reason
    /// </summary>
    public string? StopReason { get; set; }

    /// <summary>
    /// Processing time
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Response ID
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Constitutional AI validation results
    /// </summary>
    public ConstitutionalValidationResult? ConstitutionalValidation { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Conversation message model
/// </summary>
public class ConversationMessage
{
    /// <summary>
    /// Role of the message (user, assistant, system)
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Message timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Constitutional validation result
/// </summary>
public class ConstitutionalValidationResult
{
    /// <summary>
    /// Whether the response passes constitutional AI validation
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation score (0.0 to 1.0)
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Violated principles
    /// </summary>
    public List<string> ViolatedPrinciples { get; set; } = new();

    /// <summary>
    /// Validation details
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Validation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Claude stream chunk model
/// </summary>
public class ClaudeStreamChunk
{
    /// <summary>
    /// Chunk content
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Event type
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Whether this is the final chunk
    /// </summary>
    public bool IsFinal { get; set; }

    /// <summary>
    /// Chunk timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Usage information (for final chunk)
    /// </summary>
    public ClaudeUsage? Usage { get; set; }

    /// <summary>
    /// Stop reason (for final chunk)
    /// </summary>
    public string? StopReason { get; set; }
}

/// <summary>
/// Long context request model
/// </summary>
public class LongContextRequest
{
    /// <summary>
    /// Large text content to process
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Task to perform on the content
    /// </summary>
    public required string Task { get; set; }

    /// <summary>
    /// Additional instructions
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Model to use for processing
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Whether to chunk the content
    /// </summary>
    public bool UseChunking { get; set; } = false;

    /// <summary>
    /// Chunk size if chunking is enabled
    /// </summary>
    public int ChunkSize { get; set; } = 100000;

    /// <summary>
    /// Overlap size between chunks
    /// </summary>
    public int ChunkOverlap { get; set; } = 1000;
}

/// <summary>
/// Long context response model
/// </summary>
public class LongContextResponse
{
    /// <summary>
    /// Processed result
    /// </summary>
    public required string Result { get; set; }

    /// <summary>
    /// Summary of processing
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Number of chunks processed
    /// </summary>
    public int ChunksProcessed { get; set; }

    /// <summary>
    /// Total tokens processed
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// Processing time
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model used
    /// </summary>
    public required string Model { get; set; }
}