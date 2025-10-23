using OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.OpenAI.Integration.Services;

/// <summary>
/// Service for interacting with OpenAI API
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Send a chat message and get response
    /// </summary>
    Task<ChatResponse> ChatAsync(string message, ChatOptions? options = null);

    /// <summary>
    /// Send a chat message with conversation context
    /// </summary>
    Task<ChatResponse> ChatWithContextAsync(List<ChatMessage> messages, ChatOptions? options = null);

    /// <summary>
    /// Stream chat response in real-time
    /// </summary>
    IAsyncEnumerable<ChatStreamChunk> ChatStreamAsync(string message, ChatOptions? options = null);

    /// <summary>
    /// Stream chat response with context
    /// </summary>
    IAsyncEnumerable<ChatStreamChunk> ChatStreamWithContextAsync(List<ChatMessage> messages, ChatOptions? options = null);

    /// <summary>
    /// Generate text completion
    /// </summary>
    Task<CompletionResponse> CompleteTextAsync(string prompt, CompletionOptions? options = null);

    /// <summary>
    /// Get available models
    /// </summary>
    Task<List<ModelInfo>> GetAvailableModelsAsync();

    /// <summary>
    /// Estimate token count for text
    /// </summary>
    int EstimateTokenCount(string text);
}

/// <summary>
/// OpenAI service implementation
/// </summary>
public class OpenAIService : IOpenAIService
{
    private readonly OpenAIClient _client;
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAIService> _logger;
    private readonly ITokenCountingService _tokenCountingService;

    public OpenAIService(
        IOptions<OpenAISettings> settings,
        ITokenCountingService tokenCountingService,
        ILogger<OpenAIService> logger)
    {
        _settings = settings.Value;
        _tokenCountingService = tokenCountingService;
        _logger = logger;

        var clientOptions = new OpenAIClientOptions();
        _client = new OpenAIClient(_settings.ApiKey, clientOptions);
    }

    public async Task<ChatResponse> ChatAsync(string message, ChatOptions? options = null)
    {
        var messages = new List<ChatMessage>
        {
            new() { Role = "user", Content = message }
        };

        return await ChatWithContextAsync(messages, options);
    }

    public async Task<ChatResponse> ChatWithContextAsync(List<ChatMessage> messages, ChatOptions? options = null)
    {
        try
        {
            _logger.LogInformation("Starting chat request with {MessageCount} messages", messages.Count);

            var chatOptions = options ?? new ChatOptions();
            var requestOptions = CreateChatRequestOptions(chatOptions);

            var chatMessages = messages.Select(m => new OpenAI.Chat.ChatMessage
            {
                Role = m.Role switch
                {
                    "system" => ChatMessageRole.System,
                    "user" => ChatMessageRole.User,
                    "assistant" => ChatMessageRole.Assistant,
                    _ => ChatMessageRole.User
                },
                Content = m.Content
            }).ToList();

            var startTime = DateTime.UtcNow;
            var response = await _client.ChatEndpoint.GetCompletionAsync(chatMessages, requestOptions);
            var endTime = DateTime.UtcNow;

            var result = new ChatResponse
            {
                Message = response.FirstChoice.Message.Content,
                Model = response.Model,
                TokensUsed = response.Usage?.TotalTokens ?? 0,
                FinishReason = response.FirstChoice.FinishReason,
                ProcessingTime = endTime - startTime,
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = endTime
            };

            _logger.LogInformation("Chat request completed. Tokens used: {TokensUsed}, Processing time: {ProcessingTime}ms",
                result.TokensUsed, result.ProcessingTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chat request");
            throw new OpenAIServiceException("Chat request failed", ex);
        }
    }

    public async IAsyncEnumerable<ChatStreamChunk> ChatStreamAsync(string message, ChatOptions? options = null)
    {
        var messages = new List<ChatMessage>
        {
            new() { Role = "user", Content = message }
        };

        await foreach (var chunk in ChatStreamWithContextAsync(messages, options))
        {
            yield return chunk;
        }
    }

    public async IAsyncEnumerable<ChatStreamChunk> ChatStreamWithContextAsync(List<ChatMessage> messages, ChatOptions? options = null)
    {
        try
        {
            _logger.LogInformation("Starting streaming chat request with {MessageCount} messages", messages.Count);

            var chatOptions = options ?? new ChatOptions();
            var requestOptions = CreateChatRequestOptions(chatOptions);

            var chatMessages = messages.Select(m => new OpenAI.Chat.ChatMessage
            {
                Role = m.Role switch
                {
                    "system" => ChatMessageRole.System,
                    "user" => ChatMessageRole.User,
                    "assistant" => ChatMessageRole.Assistant,
                    _ => ChatMessageRole.User
                },
                Content = m.Content
            }).ToList();

            var streamingResponse = _client.ChatEndpoint.StreamCompletionAsync(chatMessages, requestOptions);

            await foreach (var response in streamingResponse)
            {
                if (response?.Choices?.FirstOrDefault()?.Delta?.Content != null)
                {
                    yield return new ChatStreamChunk
                    {
                        Content = response.Choices.First().Delta.Content,
                        Model = response.Model,
                        FinishReason = response.Choices.First().FinishReason,
                        Timestamp = DateTime.UtcNow
                    };
                }
            }

            _logger.LogInformation("Streaming chat request completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during streaming chat request");
            throw new OpenAIServiceException("Streaming chat request failed", ex);
        }
    }

    public async Task<CompletionResponse> CompleteTextAsync(string prompt, CompletionOptions? options = null)
    {
        try
        {
            _logger.LogInformation("Starting text completion request");

            var completionOptions = options ?? new CompletionOptions();

            // Note: This is a simplified example. OpenAI's newer models primarily use chat completions
            // For actual text completion, you might want to use the chat endpoint with appropriate prompting
            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new() { Role = ChatMessageRole.User, Content = prompt }
            };

            var requestOptions = new ChatRequest(
                messages,
                model: completionOptions.Model ?? _settings.Model,
                maxTokens: completionOptions.MaxTokens ?? _settings.MaxTokens,
                temperature: completionOptions.Temperature ?? _settings.Temperature
            );

            var startTime = DateTime.UtcNow;
            var response = await _client.ChatEndpoint.GetCompletionAsync(requestOptions);
            var endTime = DateTime.UtcNow;

            var result = new CompletionResponse
            {
                CompletedText = response.FirstChoice.Message.Content,
                Model = response.Model,
                TokensUsed = response.Usage?.TotalTokens ?? 0,
                FinishReason = response.FirstChoice.FinishReason,
                ProcessingTime = endTime - startTime,
                Timestamp = endTime
            };

            _logger.LogInformation("Text completion request completed. Tokens used: {TokensUsed}",
                result.TokensUsed);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during text completion request");
            throw new OpenAIServiceException("Text completion request failed", ex);
        }
    }

    public async Task<List<ModelInfo>> GetAvailableModelsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving available models");

            var models = await _client.ModelsEndpoint.GetModelsAsync();

            var result = models.Select(model => new ModelInfo
            {
                Id = model.Id,
                Object = model.Object,
                Created = model.Created,
                OwnedBy = model.OwnedBy
            }).ToList();

            _logger.LogInformation("Retrieved {ModelCount} available models", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available models");
            throw new OpenAIServiceException("Failed to retrieve available models", ex);
        }
    }

    public int EstimateTokenCount(string text)
    {
        return _tokenCountingService.CountTokens(text, _settings.Model);
    }

    private ChatRequest CreateChatRequestOptions(ChatOptions options)
    {
        return new ChatRequest(
            messages: new List<OpenAI.Chat.ChatMessage>(), // Will be set by caller
            model: options.Model ?? _settings.Model,
            maxTokens: options.MaxTokens ?? _settings.MaxTokens,
            temperature: options.Temperature ?? _settings.Temperature,
            topP: options.TopP ?? _settings.TopP,
            frequencyPenalty: options.FrequencyPenalty ?? _settings.FrequencyPenalty,
            presencePenalty: options.PresencePenalty ?? _settings.PresencePenalty,
            stop: options.StopSequences?.ToArray()
        );
    }
}

/// <summary>
/// OpenAI service exception
/// </summary>
public class OpenAIServiceException : Exception
{
    public OpenAIServiceException(string message) : base(message) { }
    public OpenAIServiceException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// OpenAI configuration settings
/// </summary>
public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-3.5-turbo";
    public int MaxTokens { get; set; } = 1000;
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 1.0;
    public double FrequencyPenalty { get; set; } = 0.0;
    public double PresencePenalty { get; set; } = 0.0;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetryAttempts { get; set; } = 3;
    public string? OrganizationId { get; set; }
}

/// <summary>
/// Chat options for requests
/// </summary>
public class ChatOptions
{
    public string? Model { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public List<string>? StopSequences { get; set; }
    public string? SystemMessage { get; set; }
}

/// <summary>
/// Completion options for text completion requests
/// </summary>
public class CompletionOptions
{
    public string? Model { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public List<string>? StopSequences { get; set; }
}

/// <summary>
/// Chat message model
/// </summary>
public class ChatMessage
{
    public string Role { get; set; } = "user"; // system, user, assistant
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Chat response model
/// </summary>
public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public string? FinishReason { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Streaming chat chunk model
/// </summary>
public class ChatStreamChunk
{
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? FinishReason { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Text completion response model
/// </summary>
public class CompletionResponse
{
    public string CompletedText { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public string? FinishReason { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Model information
/// </summary>
public class ModelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string OwnedBy { get; set; } = string.Empty;
}