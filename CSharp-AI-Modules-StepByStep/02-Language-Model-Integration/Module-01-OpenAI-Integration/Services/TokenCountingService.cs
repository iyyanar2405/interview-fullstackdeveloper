using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AI.OpenAI.Integration.Services;

/// <summary>
/// Service for counting tokens in text
/// </summary>
public interface ITokenCountingService
{
    /// <summary>
    /// Count tokens in text for a specific model
    /// </summary>
    int CountTokens(string text, string model);

    /// <summary>
    /// Estimate cost for token usage
    /// </summary>
    decimal EstimateCost(int inputTokens, int outputTokens, string model);

    /// <summary>
    /// Check if text exceeds model's context limit
    /// </summary>
    bool ExceedsContextLimit(string text, string model);

    /// <summary>
    /// Get maximum context length for a model
    /// </summary>
    int GetMaxContextLength(string model);
}

/// <summary>
/// Token counting service implementation
/// </summary>
public class TokenCountingService : ITokenCountingService
{
    private readonly ILogger<TokenCountingService> _logger;
    private readonly Dictionary<string, ModelTokenLimits> _modelLimits;

    public TokenCountingService(ILogger<TokenCountingService> logger)
    {
        _logger = logger;
        _modelLimits = InitializeModelLimits();
    }

    public int CountTokens(string text, string model)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        try
        {
            // Simplified token counting algorithm
            // For production use, consider using libraries like SharpToken or tiktoken
            var estimatedTokens = EstimateTokensSimple(text);

            _logger.LogDebug("Estimated {TokenCount} tokens for {TextLength} characters using model {Model}",
                estimatedTokens, text.Length, model);

            return estimatedTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting tokens for model {Model}", model);
            // Fallback to simple character-based estimation
            return text.Length / 4; // Rough approximation: 1 token ≈ 4 characters
        }
    }

    public decimal EstimateCost(int inputTokens, int outputTokens, string model)
    {
        if (!_modelLimits.TryGetValue(model, out var limits))
        {
            _logger.LogWarning("Unknown model {Model}, using default pricing", model);
            limits = _modelLimits["gpt-3.5-turbo"];
        }

        var inputCost = (inputTokens / 1000m) * limits.InputCostPer1KTokens;
        var outputCost = (outputTokens / 1000m) * limits.OutputCostPer1KTokens;
        var totalCost = inputCost + outputCost;

        _logger.LogDebug("Estimated cost: ${Cost:F6} (Input: {InputTokens} tokens @ ${InputCost:F6}, Output: {OutputTokens} tokens @ ${OutputCost:F6})",
            totalCost, inputTokens, inputCost, outputTokens, outputCost);

        return totalCost;
    }

    public bool ExceedsContextLimit(string text, string model)
    {
        var tokenCount = CountTokens(text, model);
        var maxTokens = GetMaxContextLength(model);
        
        var exceeds = tokenCount > maxTokens;
        
        if (exceeds)
        {
            _logger.LogWarning("Text exceeds context limit for model {Model}: {TokenCount} > {MaxTokens}",
                model, tokenCount, maxTokens);
        }

        return exceeds;
    }

    public int GetMaxContextLength(string model)
    {
        if (_modelLimits.TryGetValue(model, out var limits))
        {
            return limits.MaxContextLength;
        }

        _logger.LogWarning("Unknown model {Model}, using default context length", model);
        return 4096; // Default fallback
    }

    private int EstimateTokensSimple(string text)
    {
        // Simplified token estimation algorithm
        // This is a rough approximation and should be replaced with proper tokenization
        
        // Remove extra whitespace
        text = Regex.Replace(text, @"\s+", " ");
        
        // Split on whitespace and punctuation
        var words = Regex.Split(text, @"[\s\p{P}]+")
                         .Where(w => !string.IsNullOrEmpty(w))
                         .ToList();

        // Estimate tokens based on word count and character count
        var wordBasedEstimate = words.Count;
        var charBasedEstimate = text.Length / 4; // Rough approximation

        // Use the higher of the two estimates for safety
        var estimate = Math.Max(wordBasedEstimate, charBasedEstimate);

        // Add some padding for special tokens
        return (int)(estimate * 1.1);
    }

    private Dictionary<string, ModelTokenLimits> InitializeModelLimits()
    {
        return new Dictionary<string, ModelTokenLimits>
        {
            ["gpt-3.5-turbo"] = new()
            {
                MaxContextLength = 4096,
                InputCostPer1KTokens = 0.0015m,
                OutputCostPer1KTokens = 0.002m
            },
            ["gpt-3.5-turbo-16k"] = new()
            {
                MaxContextLength = 16384,
                InputCostPer1KTokens = 0.003m,
                OutputCostPer1KTokens = 0.004m
            },
            ["gpt-4"] = new()
            {
                MaxContextLength = 8192,
                InputCostPer1KTokens = 0.03m,
                OutputCostPer1KTokens = 0.06m
            },
            ["gpt-4-32k"] = new()
            {
                MaxContextLength = 32768,
                InputCostPer1KTokens = 0.06m,
                OutputCostPer1KTokens = 0.12m
            },
            ["gpt-4-turbo"] = new()
            {
                MaxContextLength = 128000,
                InputCostPer1KTokens = 0.01m,
                OutputCostPer1KTokens = 0.03m
            },
            ["gpt-4o"] = new()
            {
                MaxContextLength = 128000,
                InputCostPer1KTokens = 0.005m,
                OutputCostPer1KTokens = 0.015m
            }
        };
    }
}

/// <summary>
/// Model token limits and pricing information
/// </summary>
public class ModelTokenLimits
{
    public int MaxContextLength { get; set; }
    public decimal InputCostPer1KTokens { get; set; }
    public decimal OutputCostPer1KTokens { get; set; }
}

/// <summary>
/// Token usage statistics
/// </summary>
public class TokenUsageStats
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    public string Model { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}