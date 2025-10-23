using System.ComponentModel.DataAnnotations;

namespace AI.Foundation.API.Models;

/// <summary>
/// Generic API request wrapper
/// </summary>
/// <typeparam name="T">The type of data being sent</typeparam>
public class APIRequest<T>
{
    /// <summary>
    /// Unique request identifier
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when the request was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The actual data payload
    /// </summary>
    [Required]
    public T? Data { get; set; }

    /// <summary>
    /// Optional metadata for the request
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Chat request model
/// </summary>
public class ChatRequestModel
{
    /// <summary>
    /// The user's message
    /// </summary>
    [Required]
    [StringLength(4000, ErrorMessage = "Message cannot exceed 4000 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional conversation context
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Model to use for processing
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Temperature for response generation (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "Temperature must be between 0.0 and 1.0")]
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Maximum tokens in response
    /// </summary>
    [Range(1, 4000, ErrorMessage = "Max tokens must be between 1 and 4000")]
    public int MaxTokens { get; set; } = 1000;
}

/// <summary>
/// Text completion request model
/// </summary>
public class TextCompletionRequest
{
    /// <summary>
    /// The prompt to complete
    /// </summary>
    [Required]
    [StringLength(4000, ErrorMessage = "Prompt cannot exceed 4000 characters")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Model to use for completion
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Maximum tokens to generate
    /// </summary>
    [Range(1, 2000, ErrorMessage = "Max tokens must be between 1 and 2000")]
    public int MaxTokens { get; set; } = 100;

    /// <summary>
    /// Temperature for generation
    /// </summary>
    [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
    public double Temperature { get; set; } = 1.0;

    /// <summary>
    /// Stop sequences
    /// </summary>
    public List<string>? Stop { get; set; }
}