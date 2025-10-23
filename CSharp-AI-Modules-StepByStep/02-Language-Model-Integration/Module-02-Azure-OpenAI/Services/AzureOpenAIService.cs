using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.AzureOpenAI.Services;

/// <summary>
/// Azure OpenAI service interface
/// </summary>
public interface IAzureOpenAIService
{
    /// <summary>
    /// Send chat completion request
    /// </summary>
    Task<AzureChatResponse> ChatCompletionAsync(AzureChatRequest request);

    /// <summary>
    /// Stream chat completion
    /// </summary>
    IAsyncEnumerable<AzureChatStreamChunk> ChatCompletionStreamAsync(AzureChatRequest request);

    /// <summary>
    /// Generate embeddings
    /// </summary>
    Task<AzureEmbeddingResponse> GetEmbeddingsAsync(AzureEmbeddingRequest request);

    /// <summary>
    /// Get available deployments
    /// </summary>
    Task<List<AzureDeploymentInfo>> GetDeploymentsAsync();

    /// <summary>
    /// Check content filtering result
    /// </summary>
    Task<ContentFilterResult> CheckContentFilterAsync(string content);

    /// <summary>
    /// Get usage metrics
    /// </summary>
    Task<AzureUsageMetrics> GetUsageMetricsAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Azure OpenAI service implementation
/// </summary>
public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAISettings _settings;
    private readonly ILogger<AzureOpenAIService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IContentFilterService _contentFilterService;

    public AzureOpenAIService(
        IOptions<AzureOpenAISettings> settings,
        ITokenService tokenService,
        IContentFilterService contentFilterService,
        ILogger<AzureOpenAIService> logger)
    {
        _settings = settings.Value;
        _tokenService = tokenService;
        _contentFilterService = contentFilterService;
        _logger = logger;

        _client = CreateOpenAIClient();
    }

    public async Task<AzureChatResponse> ChatCompletionAsync(AzureChatRequest request)
    {
        try
        {
            _logger.LogInformation("Starting Azure OpenAI chat completion request");

            // Validate and filter content if enabled
            if (_settings.ContentFilter.Enabled)
            {
                var filterResult = await _contentFilterService.ValidateContentAsync(request.Messages);
                if (filterResult.IsBlocked)
                {
                    throw new ContentFilteredException($"Content blocked: {filterResult.Reason}");
                }
            }

            var chatCompletionsOptions = new ChatCompletionsOptions(_settings.DeploymentName, request.Messages)
            {
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature,
                FrequencyPenalty = request.FrequencyPenalty,
                PresencePenalty = request.PresencePenalty,
                NucleusSamplingFactor = request.TopP
            };

            if (request.StopSequences?.Any() == true)
            {
                foreach (var stop in request.StopSequences)
                {
                    chatCompletionsOptions.StopSequences.Add(stop);
                }
            }

            var startTime = DateTime.UtcNow;
            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
            var endTime = DateTime.UtcNow;

            var result = new AzureChatResponse
            {
                Message = response.Value.Choices[0].Message.Content,
                Model = response.Value.Model,
                Usage = MapUsage(response.Value.Usage),
                FinishReason = response.Value.Choices[0].FinishReason?.ToString(),
                ProcessingTime = endTime - startTime,
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = endTime,
                ContentFilterResults = MapContentFilterResults(response.Value.Choices[0].ContentFilterResults)
            };

            // Log usage metrics
            await LogUsageMetricsAsync(result.Usage);

            _logger.LogInformation("Azure OpenAI chat completion completed. Tokens: {TokensUsed}, Time: {ProcessingTime}ms",
                result.Usage.TotalTokens, result.ProcessingTime.TotalMilliseconds);

            return result;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI request failed with status {StatusCode}", ex.Status);
            throw new AzureOpenAIException($"Azure OpenAI request failed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Azure OpenAI chat completion");
            throw new AzureOpenAIException($"Chat completion failed: {ex.Message}", ex);
        }
    }

    public async IAsyncEnumerable<AzureChatStreamChunk> ChatCompletionStreamAsync(AzureChatRequest request)
    {
        Response<StreamingChatCompletions>? streamingResponse = null;
        
        try
        {
            _logger.LogInformation("Starting Azure OpenAI streaming chat completion");

            var chatCompletionsOptions = new ChatCompletionsOptions(_settings.DeploymentName, request.Messages)
            {
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature,
                FrequencyPenalty = request.FrequencyPenalty,
                PresencePenalty = request.PresencePenalty,
                NucleusSamplingFactor = request.TopP
            };

            streamingResponse = await _client.GetChatCompletionsStreamingAsync(chatCompletionsOptions);

            await foreach (var streamingChoice in streamingResponse.Value.GetChoicesStreaming())
            {
                await foreach (var message in streamingChoice.GetMessageStreaming())
                {
                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        yield return new AzureChatStreamChunk
                        {
                            Content = message.Content,
                            Model = streamingResponse.Value.Model,
                            FinishReason = streamingChoice.FinishReason?.ToString(),
                            Timestamp = DateTime.UtcNow
                        };
                    }
                }
            }

            _logger.LogInformation("Azure OpenAI streaming chat completion finished");
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI streaming request failed with status {StatusCode}", ex.Status);
            throw new AzureOpenAIException($"Streaming request failed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Azure OpenAI streaming");
            throw new AzureOpenAIException($"Streaming chat completion failed: {ex.Message}", ex);
        }
        finally
        {
            streamingResponse?.Dispose();
        }
    }

    public async Task<AzureEmbeddingResponse> GetEmbeddingsAsync(AzureEmbeddingRequest request)
    {
        try
        {
            _logger.LogInformation("Getting embeddings for {InputCount} inputs", request.Input.Count);

            var embeddingsOptions = new EmbeddingsOptions(_settings.EmbeddingDeploymentName, request.Input);

            var response = await _client.GetEmbeddingsAsync(embeddingsOptions);

            var result = new AzureEmbeddingResponse
            {
                Embeddings = response.Value.Data.Select(d => d.Embedding.ToArray()).ToList(),
                Model = response.Value.Model,
                Usage = MapUsage(response.Value.Usage),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Embeddings generated successfully. Tokens used: {TokensUsed}",
                result.Usage.TotalTokens);

            return result;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI embeddings request failed with status {StatusCode}", ex.Status);
            throw new AzureOpenAIException($"Embeddings request failed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting embeddings");
            throw new AzureOpenAIException($"Get embeddings failed: {ex.Message}", ex);
        }
    }

    public async Task<List<AzureDeploymentInfo>> GetDeploymentsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving Azure OpenAI deployments");

            // Note: This is a placeholder as the actual API for getting deployments
            // is typically done through Azure Resource Manager APIs, not the OpenAI client
            var deployments = new List<AzureDeploymentInfo>
            {
                new()
                {
                    Name = _settings.DeploymentName,
                    Model = "gpt-35-turbo",
                    Status = "Succeeded",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            if (!string.IsNullOrEmpty(_settings.EmbeddingDeploymentName))
            {
                deployments.Add(new AzureDeploymentInfo
                {
                    Name = _settings.EmbeddingDeploymentName,
                    Model = "text-embedding-ada-002",
                    Status = "Succeeded",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                });
            }

            await Task.CompletedTask; // Placeholder for async operation
            return deployments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deployments");
            throw new AzureOpenAIException($"Failed to retrieve deployments: {ex.Message}", ex);
        }
    }

    public async Task<ContentFilterResult> CheckContentFilterAsync(string content)
    {
        return await _contentFilterService.CheckContentAsync(content);
    }

    public async Task<AzureUsageMetrics> GetUsageMetricsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Retrieving usage metrics from {StartDate} to {EndDate}",
                startDate, endDate);

            // Note: This would typically integrate with Azure Monitor or Cost Management APIs
            // This is a placeholder implementation
            var metrics = new AzureUsageMetrics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRequests = Random.Shared.Next(1000, 10000),
                TotalTokens = Random.Shared.Next(100000, 1000000),
                TotalCost = Random.Shared.NextSingle() * 100,
                AverageResponseTime = TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000))
            };

            await Task.CompletedTask; // Placeholder for async operation
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usage metrics");
            throw new AzureOpenAIException($"Failed to retrieve usage metrics: {ex.Message}", ex);
        }
    }

    private OpenAIClient CreateOpenAIClient()
    {
        var endpoint = new Uri(_settings.Endpoint);

        if (_settings.UseManagedIdentity)
        {
            _logger.LogInformation("Using managed identity for Azure OpenAI authentication");
            var credential = new DefaultAzureCredential();
            return new OpenAIClient(endpoint, credential);
        }
        else if (!string.IsNullOrEmpty(_settings.ApiKey))
        {
            _logger.LogInformation("Using API key for Azure OpenAI authentication");
            return new OpenAIClient(endpoint, new AzureKeyCredential(_settings.ApiKey));
        }
        else
        {
            throw new InvalidOperationException("Either API key or managed identity must be configured");
        }
    }

    private static AzureTokenUsage MapUsage(CompletionsUsage? usage)
    {
        if (usage == null)
            return new AzureTokenUsage();

        return new AzureTokenUsage
        {
            PromptTokens = usage.PromptTokens,
            CompletionTokens = usage.CompletionTokens,
            TotalTokens = usage.TotalTokens
        };
    }

    private static List<AzureContentFilterResult> MapContentFilterResults(ContentFilterResultsForChoice? filterResults)
    {
        var results = new List<AzureContentFilterResult>();

        if (filterResults?.Hate != null)
        {
            results.Add(new AzureContentFilterResult
            {
                Category = "hate",
                Severity = filterResults.Hate.Severity.ToString(),
                Filtered = filterResults.Hate.Filtered
            });
        }

        if (filterResults?.SelfHarm != null)
        {
            results.Add(new AzureContentFilterResult
            {
                Category = "self_harm",
                Severity = filterResults.SelfHarm.Severity.ToString(),
                Filtered = filterResults.SelfHarm.Filtered
            });
        }

        if (filterResults?.Sexual != null)
        {
            results.Add(new AzureContentFilterResult
            {
                Category = "sexual",
                Severity = filterResults.Sexual.Severity.ToString(),
                Filtered = filterResults.Sexual.Filtered
            });
        }

        if (filterResults?.Violence != null)
        {
            results.Add(new AzureContentFilterResult
            {
                Category = "violence",
                Severity = filterResults.Violence.Severity.ToString(),
                Filtered = filterResults.Violence.Filtered
            });
        }

        return results;
    }

    private async Task LogUsageMetricsAsync(AzureTokenUsage usage)
    {
        try
        {
            // Log usage metrics for monitoring and billing
            _logger.LogInformation("Usage: Prompt={PromptTokens}, Completion={CompletionTokens}, Total={TotalTokens}",
                usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);

            // You could also send this to Application Insights, Azure Monitor, or a custom metrics service
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log usage metrics");
        }
    }
}

/// <summary>
/// Azure OpenAI exception
/// </summary>
public class AzureOpenAIException : Exception
{
    public AzureOpenAIException(string message) : base(message) { }
    public AzureOpenAIException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Content filtered exception
/// </summary>
public class ContentFilteredException : Exception
{
    public ContentFilteredException(string message) : base(message) { }
}