using AI.Anthropic.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace AI.Anthropic.Services;

/// <summary>
/// Anthropic Claude service interface
/// </summary>
public interface IClaudeService
{
    /// <summary>
    /// Send a chat completion request to Claude
    /// </summary>
    Task<ClaudeChatResponse> ChatAsync(ClaudeChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a chat completion response from Claude
    /// </summary>
    IAsyncEnumerable<ClaudeStreamChunk> ChatStreamAsync(ClaudeChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process long context content with Claude
    /// </summary>
    Task<LongContextResponse> ProcessLongContextAsync(LongContextRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate response against Constitutional AI principles
    /// </summary>
    Task<ConstitutionalValidationResult> ValidateResponseAsync(string response, List<string>? principles = null);

    /// <summary>
    /// Get available models
    /// </summary>
    Task<List<string>> GetAvailableModelsAsync();

    /// <summary>
    /// Count tokens in text (estimation)
    /// </summary>
    Task<int> CountTokensAsync(string text);

    /// <summary>
    /// Estimate cost for token usage
    /// </summary>
    Task<decimal> EstimateCostAsync(int inputTokens, int outputTokens, string model);
}

/// <summary>
/// Anthropic Claude service implementation
/// </summary>
public class ClaudeService : IClaudeService
{
    private readonly HttpClient _httpClient;
    private readonly AnthropicSettings _settings;
    private readonly ILogger<ClaudeService> _logger;
    private readonly IConstitutionalAIService _constitutionalAIService;
    private readonly ITokenService _tokenService;

    public ClaudeService(
        HttpClient httpClient,
        IOptions<AnthropicSettings> settings,
        IConstitutionalAIService constitutionalAIService,
        ITokenService tokenService,
        ILogger<ClaudeService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _constitutionalAIService = constitutionalAIService ?? throw new ArgumentNullException(nameof(constitutionalAIService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfigureHttpClient();
    }

    public async Task<ClaudeChatResponse> ChatAsync(ClaudeChatRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting Claude chat completion request");

            var startTime = DateTime.UtcNow;
            var claudeRequest = await BuildClaudeRequestAsync(request);

            var response = await SendRequestAsync<ClaudeResponse>("/v1/messages", claudeRequest, cancellationToken);
            var endTime = DateTime.UtcNow;

            var result = new ClaudeChatResponse
            {
                Text = ExtractTextFromContent(response.Content),
                Model = response.Model,
                Usage = response.Usage,
                StopReason = response.StopReason,
                ProcessingTime = endTime - startTime,
                Id = response.Id,
                Timestamp = endTime
            };

            // Apply Constitutional AI validation if enabled
            if (_settings.ConstitutionalAI.Enabled)
            {
                var principles = request.Principles ?? _settings.ConstitutionalAI.Principles;
                result.ConstitutionalValidation = await _constitutionalAIService.ValidateAsync(result.Text, principles);

                if (!result.ConstitutionalValidation.IsValid && _settings.ConstitutionalAI.ValidateResponses)
                {
                    _logger.LogWarning("Response failed Constitutional AI validation: {Violations}",
                        string.Join(", ", result.ConstitutionalValidation.ViolatedPrinciples));
                    
                    throw new ConstitutionalAIException(
                        "Response violates Constitutional AI principles",
                        result.ConstitutionalValidation.ViolatedPrinciples);
                }
            }

            // Log usage metrics
            await LogUsageMetricsAsync(result);

            _logger.LogInformation("Claude chat completion completed successfully. Tokens: {InputTokens}/{OutputTokens}, Time: {ProcessingTime}ms",
                result.Usage.InputTokens, result.Usage.OutputTokens, result.ProcessingTime.TotalMilliseconds);

            return result;
        }
        catch (ConstitutionalAIException)
        {
            throw; // Re-throw Constitutional AI exceptions
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error in Claude chat completion");
            throw new ClaudeException($"API request failed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Claude chat completion");
            throw new ClaudeException($"Chat completion failed: {ex.Message}", ex);
        }
    }

    public async IAsyncEnumerable<ClaudeStreamChunk> ChatStreamAsync(
        ClaudeChatRequest request, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ClaudeResponse? finalMessage = null;
        var contentBuilder = new StringBuilder();

        try
        {
            _logger.LogInformation("Starting Claude streaming chat completion");

            request.Stream = true;
            var claudeRequest = await BuildClaudeRequestAsync(request);

            await foreach (var streamEvent in StreamRequestAsync("/v1/messages", claudeRequest, cancellationToken))
            {
                var chunk = ProcessStreamEvent(streamEvent, contentBuilder, ref finalMessage);
                if (chunk != null)
                {
                    yield return chunk;
                }
            }

            // Apply Constitutional AI validation if enabled and we have final content
            if (_settings.ConstitutionalAI.Enabled && contentBuilder.Length > 0)
            {
                var principles = request.Principles ?? _settings.ConstitutionalAI.Principles;
                var validation = await _constitutionalAIService.ValidateAsync(contentBuilder.ToString(), principles);

                if (!validation.IsValid && _settings.ConstitutionalAI.ValidateResponses)
                {
                    _logger.LogWarning("Streamed response failed Constitutional AI validation: {Violations}",
                        string.Join(", ", validation.ViolatedPrinciples));

                    yield return new ClaudeStreamChunk
                    {
                        Content = $"[Constitutional AI Violation: {string.Join(", ", validation.ViolatedPrinciples)}]",
                        Type = "error",
                        IsFinal = true,
                        Timestamp = DateTime.UtcNow
                    };
                    yield break;
                }
            }

            _logger.LogInformation("Claude streaming chat completion completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Claude streaming chat completion");
            yield return new ClaudeStreamChunk
            {
                Content = $"Error: {ex.Message}",
                Type = "error",
                IsFinal = true,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<LongContextResponse> ProcessLongContextAsync(LongContextRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing long context content. Length: {ContentLength}", request.Content.Length);

            var startTime = DateTime.UtcNow;
            var totalTokens = 0;
            var chunksProcessed = 0;

            string result;
            if (request.UseChunking && request.Content.Length > request.ChunkSize)
            {
                result = await ProcessInChunksAsync(request, cancellationToken);
                chunksProcessed = (int)Math.Ceiling((double)request.Content.Length / request.ChunkSize);
            }
            else
            {
                var chatRequest = new ClaudeChatRequest
                {
                    Message = $"Task: {request.Task}\n\nContent:\n{request.Content}",
                    SystemPrompt = request.Instructions ?? "You are processing a large document. Provide accurate and comprehensive analysis.",
                    Model = request.Model ?? _settings.DefaultModel
                };

                var response = await ChatAsync(chatRequest, cancellationToken);
                result = response.Text;
                totalTokens = response.Usage.TotalTokens;
                chunksProcessed = 1;
            }

            var endTime = DateTime.UtcNow;

            return new LongContextResponse
            {
                Result = result,
                Summary = await GenerateSummaryAsync(result, cancellationToken),
                ChunksProcessed = chunksProcessed,
                TotalTokens = totalTokens,
                ProcessingTime = endTime - startTime,
                Model = request.Model ?? _settings.DefaultModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing long context");
            throw new ClaudeException($"Long context processing failed: {ex.Message}", ex);
        }
    }

    public async Task<ConstitutionalValidationResult> ValidateResponseAsync(string response, List<string>? principles = null)
    {
        var validationPrinciples = principles ?? _settings.ConstitutionalAI.Principles;
        return await _constitutionalAIService.ValidateAsync(response, validationPrinciples);
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        // Anthropic doesn't have a public models endpoint, so we return known models
        await Task.CompletedTask;
        return new List<string>
        {
            "claude-3-5-sonnet-20241022",
            "claude-3-5-haiku-20241022",
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307"
        };
    }

    public async Task<int> CountTokensAsync(string text)
    {
        return await _tokenService.CountTokensAsync(text);
    }

    public async Task<decimal> EstimateCostAsync(int inputTokens, int outputTokens, string model)
    {
        return await _tokenService.EstimateCostAsync(inputTokens, outputTokens, model);
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Claude-CSharp-Client/1.0");
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    private async Task<ClaudeRequest> BuildClaudeRequestAsync(ClaudeChatRequest request)
    {
        var messages = new List<ClaudeMessage>();

        // Add conversation history
        if (request.History != null)
        {
            foreach (var historyMessage in request.History)
            {
                messages.Add(new ClaudeMessage
                {
                    Role = historyMessage.Role,
                    Content = historyMessage.Content
                });
            }
        }

        // Add current user message
        messages.Add(new ClaudeMessage
        {
            Role = "user",
            Content = request.Message
        });

        return new ClaudeRequest
        {
            Model = request.Model ?? _settings.DefaultModel,
            MaxTokens = request.MaxTokens ?? _settings.MaxTokens,
            System = request.SystemPrompt,
            Messages = messages,
            Temperature = request.Temperature ?? _settings.DefaultTemperature,
            Stream = request.Stream,
            Metadata = request.Metadata
        };
    }

    private async Task<T> SendRequestAsync<T>(string endpoint, object requestBody, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ClaudeError>(responseContent);
            throw new ClaudeException($"API error: {error?.Message ?? response.ReasonPhrase}");
        }

        var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        return result ?? throw new ClaudeException("Failed to deserialize response");
    }

    private async IAsyncEnumerable<ClaudeStreamEvent> StreamRequestAsync(
        string endpoint, 
        object requestBody, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(endpoint, content, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var error = JsonSerializer.Deserialize<ClaudeError>(errorContent);
            throw new ClaudeException($"API error: {error?.Message ?? response.ReasonPhrase}");
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
        {
            if (line.StartsWith("data: "))
            {
                var eventData = line.Substring(6);
                if (eventData == "[DONE]")
                    break;

                if (!string.IsNullOrEmpty(eventData))
                {
                    var streamEvent = JsonSerializer.Deserialize<ClaudeStreamEvent>(eventData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (streamEvent != null)
                        yield return streamEvent;
                }
            }
        }
    }

    private static ClaudeStreamChunk? ProcessStreamEvent(ClaudeStreamEvent streamEvent, StringBuilder contentBuilder, ref ClaudeResponse? finalMessage)
    {
        switch (streamEvent.Type)
        {
            case "message_start":
                finalMessage = streamEvent.Message;
                return null;

            case "content_block_delta":
                if (streamEvent.Delta?.Text != null)
                {
                    contentBuilder.Append(streamEvent.Delta.Text);
                    return new ClaudeStreamChunk
                    {
                        Content = streamEvent.Delta.Text,
                        Type = "content",
                        IsFinal = false,
                        Timestamp = DateTime.UtcNow
                    };
                }
                return null;

            case "message_delta":
                if (streamEvent.Usage != null && finalMessage != null)
                {
                    return new ClaudeStreamChunk
                    {
                        Content = "",
                        Type = "final",
                        IsFinal = true,
                        Usage = streamEvent.Usage,
                        StopReason = finalMessage.StopReason,
                        Timestamp = DateTime.UtcNow
                    };
                }
                return null;

            case "message_stop":
                return new ClaudeStreamChunk
                {
                    Content = "",
                    Type = "stop",
                    IsFinal = true,
                    Timestamp = DateTime.UtcNow
                };

            default:
                return null;
        }
    }

    private static string ExtractTextFromContent(List<ClaudeContent> content)
    {
        var textContent = content
            .Where(c => c.Type == "text" && !string.IsNullOrEmpty(c.Text))
            .Select(c => c.Text!)
            .ToArray();

        return string.Join("\n", textContent);
    }

    private async Task<string> ProcessInChunksAsync(LongContextRequest request, CancellationToken cancellationToken)
    {
        var chunks = SplitIntoChunks(request.Content, request.ChunkSize, request.ChunkOverlap);
        var results = new List<string>();

        foreach (var (chunk, index) in chunks.Select((chunk, index) => (chunk, index)))
        {
            _logger.LogDebug("Processing chunk {Index}/{Total}", index + 1, chunks.Count);

            var chatRequest = new ClaudeChatRequest
            {
                Message = $"Task: {request.Task}\n\nThis is chunk {index + 1} of {chunks.Count}:\n\n{chunk}",
                SystemPrompt = request.Instructions ?? "You are processing a chunk of a larger document. Provide analysis for this specific section.",
                Model = request.Model ?? _settings.DefaultModel
            };

            var response = await ChatAsync(chatRequest, cancellationToken);
            results.Add(response.Text);
        }

        // Combine results
        var combinedResults = string.Join("\n\n", results);
        
        // Generate final synthesis
        var synthesisRequest = new ClaudeChatRequest
        {
            Message = $"Synthesize the following analysis results into a coherent response for the task: {request.Task}\n\nResults:\n{combinedResults}",
            SystemPrompt = "You are synthesizing multiple analysis results into a final comprehensive response.",
            Model = request.Model ?? _settings.DefaultModel
        };

        var finalResponse = await ChatAsync(synthesisRequest, cancellationToken);
        return finalResponse.Text;
    }

    private static List<string> SplitIntoChunks(string text, int chunkSize, int overlap)
    {
        var chunks = new List<string>();
        var start = 0;

        while (start < text.Length)
        {
            var end = Math.Min(start + chunkSize, text.Length);
            var chunk = text.Substring(start, end - start);
            chunks.Add(chunk);

            if (end >= text.Length)
                break;

            start = end - overlap;
        }

        return chunks;
    }

    private async Task<string> GenerateSummaryAsync(string content, CancellationToken cancellationToken)
    {
        try
        {
            var summaryRequest = new ClaudeChatRequest
            {
                Message = $"Please provide a concise summary of the following analysis:\n\n{content}",
                SystemPrompt = "You are generating a brief summary of an analysis. Keep it concise but comprehensive.",
                Model = _settings.DefaultModel,
                MaxTokens = 500
            };

            var response = await ChatAsync(summaryRequest, cancellationToken);
            return response.Text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate summary");
            return "Summary generation failed.";
        }
    }

    private async Task LogUsageMetricsAsync(ClaudeChatResponse response)
    {
        try
        {
            _logger.LogInformation("Claude Usage - Model: {Model}, Input: {InputTokens}, Output: {OutputTokens}, Total: {TotalTokens}",
                response.Model, response.Usage.InputTokens, response.Usage.OutputTokens, response.Usage.TotalTokens);

            var cost = await EstimateCostAsync(response.Usage.InputTokens, response.Usage.OutputTokens, response.Model);
            _logger.LogInformation("Estimated cost: ${Cost:F6}", cost);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log usage metrics");
        }
    }
}

/// <summary>
/// Claude service exception
/// </summary>
public class ClaudeException : Exception
{
    public ClaudeException(string message) : base(message) { }
    public ClaudeException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Constitutional AI exception
/// </summary>
public class ConstitutionalAIException : Exception
{
    public List<string> ViolatedPrinciples { get; }

    public ConstitutionalAIException(string message, List<string> violatedPrinciples) : base(message)
    {
        ViolatedPrinciples = violatedPrinciples;
    }
}