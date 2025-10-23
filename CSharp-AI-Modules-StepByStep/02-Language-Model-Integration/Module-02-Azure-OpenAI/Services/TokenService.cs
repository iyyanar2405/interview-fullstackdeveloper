using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AI.AzureOpenAI.Services;

/// <summary>
/// Token service interface for Azure OpenAI
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Count tokens in text
    /// </summary>
    Task<int> CountTokensAsync(string text, string model = "gpt-35-turbo");

    /// <summary>
    /// Count tokens in messages
    /// </summary>
    Task<int> CountTokensAsync(IEnumerable<ChatRequestMessage> messages, string model = "gpt-35-turbo");

    /// <summary>
    /// Estimate cost for tokens
    /// </summary>
    Task<decimal> EstimateCostAsync(int promptTokens, int completionTokens, string model = "gpt-35-turbo");

    /// <summary>
    /// Check if text exceeds token limit
    /// </summary>
    Task<bool> ExceedsTokenLimitAsync(string text, int maxTokens, string model = "gpt-35-turbo");

    /// <summary>
    /// Truncate text to fit token limit
    /// </summary>
    Task<string> TruncateToTokenLimitAsync(string text, int maxTokens, string model = "gpt-35-turbo");
}

/// <summary>
/// Token service implementation for Azure OpenAI
/// </summary>
public class TokenService : ITokenService
{
    private readonly AzureOpenAISettings _settings;
    private readonly ILogger<TokenService> _logger;
    private readonly Dictionary<string, TokenPricing> _pricing;

    public TokenService(
        IOptions<AzureOpenAISettings> settings,
        ILogger<TokenService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _pricing = InitializePricing();
    }

    public async Task<int> CountTokensAsync(string text, string model = "gpt-35-turbo")
    {
        try
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Simple approximation: 1 token ≈ 4 characters for English text
            // For production, you might want to use a proper tokenizer library
            var approximateTokens = (int)Math.Ceiling(text.Length / 4.0);

            // Add some overhead for special tokens
            var totalTokens = approximateTokens + 10;

            _logger.LogDebug("Estimated {TokenCount} tokens for text of length {TextLength}",
                totalTokens, text.Length);

            await Task.CompletedTask;
            return totalTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting tokens for text");
            throw;
        }
    }

    public async Task<int> CountTokensAsync(IEnumerable<ChatRequestMessage> messages, string model = "gpt-35-turbo")
    {
        try
        {
            var totalTokens = 0;

            foreach (var message in messages)
            {
                // Count tokens for the message content
                var contentTokens = await CountTokensAsync(message.Content, model);
                
                // Add overhead for message structure (role, content wrapper, etc.)
                var messageOverhead = GetMessageOverhead(model);
                
                totalTokens += contentTokens + messageOverhead;
            }

            // Add conversation overhead
            totalTokens += GetConversationOverhead(model);

            _logger.LogDebug("Estimated {TokenCount} tokens for {MessageCount} messages",
                totalTokens, messages.Count());

            return totalTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting tokens for messages");
            throw;
        }
    }

    public async Task<decimal> EstimateCostAsync(int promptTokens, int completionTokens, string model = "gpt-35-turbo")
    {
        try
        {
            if (!_pricing.TryGetValue(model, out var pricing))
            {
                _logger.LogWarning("Pricing not found for model {Model}, using default", model);
                pricing = _pricing["gpt-35-turbo"];
            }

            var promptCost = promptTokens * pricing.PromptTokenPrice;
            var completionCost = completionTokens * pricing.CompletionTokenPrice;
            var totalCost = promptCost + completionCost;

            _logger.LogDebug("Estimated cost: ${TotalCost:F6} (Prompt: ${PromptCost:F6}, Completion: ${CompletionCost:F6})",
                totalCost, promptCost, completionCost);

            await Task.CompletedTask;
            return totalCost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating cost");
            throw;
        }
    }

    public async Task<bool> ExceedsTokenLimitAsync(string text, int maxTokens, string model = "gpt-35-turbo")
    {
        try
        {
            var tokenCount = await CountTokensAsync(text, model);
            var exceeds = tokenCount > maxTokens;

            _logger.LogDebug("Token count {TokenCount} vs limit {MaxTokens}, exceeds: {Exceeds}",
                tokenCount, maxTokens, exceeds);

            return exceeds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token limit");
            throw;
        }
    }

    public async Task<string> TruncateToTokenLimitAsync(string text, int maxTokens, string model = "gpt-35-turbo")
    {
        try
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var currentTokens = await CountTokensAsync(text, model);
            
            if (currentTokens <= maxTokens)
                return text;

            _logger.LogDebug("Truncating text from {CurrentTokens} to {MaxTokens} tokens",
                currentTokens, maxTokens);

            // Approximate character limit based on token limit
            var approximateCharLimit = (int)(maxTokens * 3.5); // Conservative estimate
            
            if (text.Length <= approximateCharLimit)
                return text;

            // Truncate to approximate character limit
            var truncatedText = text.Substring(0, approximateCharLimit);
            
            // Try to truncate at word boundary
            var lastSpaceIndex = truncatedText.LastIndexOf(' ');
            if (lastSpaceIndex > approximateCharLimit * 0.8) // If we found a space reasonably close to the end
            {
                truncatedText = truncatedText.Substring(0, lastSpaceIndex);
            }

            // Verify the truncated text is within token limit
            var truncatedTokens = await CountTokensAsync(truncatedText, model);
            
            // If still too long, be more aggressive
            while (truncatedTokens > maxTokens && truncatedText.Length > 0)
            {
                var reductionAmount = (int)(truncatedText.Length * 0.1); // Remove 10% at a time
                truncatedText = truncatedText.Substring(0, Math.Max(0, truncatedText.Length - reductionAmount));
                truncatedTokens = await CountTokensAsync(truncatedText, model);
            }

            _logger.LogInformation("Truncated text from {OriginalLength} to {TruncatedLength} characters ({OriginalTokens} to {TruncatedTokens} tokens)",
                text.Length, truncatedText.Length, currentTokens, truncatedTokens);

            return truncatedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error truncating text to token limit");
            throw;
        }
    }

    private static Dictionary<string, TokenPricing> InitializePricing()
    {
        return new Dictionary<string, TokenPricing>
        {
            ["gpt-35-turbo"] = new()
            {
                PromptTokenPrice = 0.0015m / 1000, // $0.0015 per 1K tokens
                CompletionTokenPrice = 0.002m / 1000 // $0.002 per 1K tokens
            },
            ["gpt-35-turbo-16k"] = new()
            {
                PromptTokenPrice = 0.003m / 1000,
                CompletionTokenPrice = 0.004m / 1000
            },
            ["gpt-4"] = new()
            {
                PromptTokenPrice = 0.03m / 1000,
                CompletionTokenPrice = 0.06m / 1000
            },
            ["gpt-4-32k"] = new()
            {
                PromptTokenPrice = 0.06m / 1000,
                CompletionTokenPrice = 0.12m / 1000
            },
            ["gpt-4-turbo"] = new()
            {
                PromptTokenPrice = 0.01m / 1000,
                CompletionTokenPrice = 0.03m / 1000
            },
            ["text-embedding-ada-002"] = new()
            {
                PromptTokenPrice = 0.0001m / 1000,
                CompletionTokenPrice = 0.0m // No completion tokens for embeddings
            }
        };
    }

    private static int GetMessageOverhead(string model)
    {
        // Overhead for message structure varies by model
        return model.Contains("gpt-4") ? 5 : 4;
    }

    private static int GetConversationOverhead(string model)
    {
        // Base overhead for conversation
        return model.Contains("gpt-4") ? 5 : 3;
    }
}

/// <summary>
/// Token pricing information
/// </summary>
public class TokenPricing
{
    public decimal PromptTokenPrice { get; set; }
    public decimal CompletionTokenPrice { get; set; }
}