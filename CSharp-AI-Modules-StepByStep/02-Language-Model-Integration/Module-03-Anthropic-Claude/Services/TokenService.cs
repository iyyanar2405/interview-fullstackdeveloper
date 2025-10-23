using AI.Anthropic.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AI.Anthropic.Services;

/// <summary>
/// Token counting and estimation service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Estimate token count for text
    /// </summary>
    Task<int> EstimateTokenCountAsync(string text, string model = "claude-3-sonnet-20240229");

    /// <summary>
    /// Calculate cost estimate for request
    /// </summary>
    Task<CostEstimate> CalculateCostAsync(int inputTokens, int outputTokens, string model = "claude-3-sonnet-20240229");

    /// <summary>
    /// Check if text exceeds model's context limit
    /// </summary>
    Task<bool> ExceedsContextLimitAsync(string text, string model = "claude-3-sonnet-20240229");

    /// <summary>
    /// Get model's maximum context length
    /// </summary>
    Task<int> GetModelContextLimitAsync(string model = "claude-3-sonnet-20240229");

    /// <summary>
    /// Split text into chunks that fit within context limit
    /// </summary>
    Task<List<string>> SplitIntoChunksAsync(string text, string model = "claude-3-sonnet-20240229", int overlap = 100);

    /// <summary>
    /// Estimate processing time based on token count
    /// </summary>
    Task<TimeSpan> EstimateProcessingTimeAsync(int inputTokens, int outputTokens, string model = "claude-3-sonnet-20240229");
}

/// <summary>
/// Token counting and estimation service implementation
/// </summary>
public class TokenService : ITokenService
{
    private readonly AnthropicSettings _settings;
    private readonly ILogger<TokenService> _logger;
    private readonly Dictionary<string, ModelLimits> _modelLimits;
    private readonly Dictionary<string, CostStructure> _costStructures;

    public TokenService(
        IOptions<AnthropicSettings> settings,
        ILogger<TokenService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _modelLimits = InitializeModelLimits();
        _costStructures = InitializeCostStructures();
    }

    public async Task<int> EstimateTokenCountAsync(string text, string model = "claude-3-sonnet-20240229")
    {
        try
        {
            await Task.CompletedTask;

            if (string.IsNullOrEmpty(text))
                return 0;

            // Claude tokenization approximation
            // This is an estimation based on observed patterns
            var estimatedTokens = EstimateTokensUsingHeuristics(text);

            _logger.LogDebug("Estimated {TokenCount} tokens for text of length {TextLength}", 
                estimatedTokens, text.Length);

            return estimatedTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating token count");
            throw;
        }
    }

    public async Task<CostEstimate> CalculateCostAsync(int inputTokens, int outputTokens, string model = "claude-3-sonnet-20240229")
    {
        try
        {
            await Task.CompletedTask;

            if (!_costStructures.TryGetValue(model, out var costs))
            {
                _logger.LogWarning("Cost structure not found for model: {Model}. Using default.", model);
                costs = _costStructures["claude-3-sonnet-20240229"];
            }

            var inputCost = (inputTokens / 1000.0) * costs.InputCostPer1KTokens;
            var outputCost = (outputTokens / 1000.0) * costs.OutputCostPer1KTokens;
            var totalCost = inputCost + outputCost;

            var estimate = new CostEstimate
            {
                Model = model,
                InputTokens = inputTokens,
                OutputTokens = outputTokens,
                InputCost = inputCost,
                OutputCost = outputCost,
                TotalCost = totalCost,
                Currency = "USD",
                Timestamp = DateTime.UtcNow
            };

            _logger.LogDebug("Cost estimate: ${TotalCost:F4} for {InputTokens} input + {OutputTokens} output tokens",
                totalCost, inputTokens, outputTokens);

            return estimate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating cost estimate");
            throw;
        }
    }

    public async Task<bool> ExceedsContextLimitAsync(string text, string model = "claude-3-sonnet-20240229")
    {
        try
        {
            var tokenCount = await EstimateTokenCountAsync(text, model);
            var contextLimit = await GetModelContextLimitAsync(model);

            var exceeds = tokenCount > contextLimit;
            
            if (exceeds)
            {
                _logger.LogWarning("Text exceeds context limit: {TokenCount} > {ContextLimit} for model {Model}",
                    tokenCount, contextLimit, model);
            }

            return exceeds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking context limit");
            throw;
        }
    }

    public async Task<int> GetModelContextLimitAsync(string model = "claude-3-sonnet-20240229")
    {
        try
        {
            await Task.CompletedTask;

            if (_modelLimits.TryGetValue(model, out var limits))
            {
                return limits.MaxContextTokens;
            }

            _logger.LogWarning("Model limits not found for: {Model}. Using default.", model);
            return _modelLimits["claude-3-sonnet-20240229"].MaxContextTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model context limit");
            throw;
        }
    }

    public async Task<List<string>> SplitIntoChunksAsync(string text, string model = "claude-3-sonnet-20240229", int overlap = 100)
    {
        try
        {
            var contextLimit = await GetModelContextLimitAsync(model);
            var chunks = new List<string>();

            if (string.IsNullOrEmpty(text))
                return chunks;

            var totalTokens = await EstimateTokenCountAsync(text, model);
            
            if (totalTokens <= contextLimit)
            {
                chunks.Add(text);
                return chunks;
            }

            _logger.LogDebug("Splitting text into chunks. Total tokens: {TotalTokens}, Context limit: {ContextLimit}",
                totalTokens, contextLimit);

            // Calculate chunk size with safety margin
            var safetyMargin = 1000; // Reserve tokens for system prompts and responses
            var chunkSizeTokens = contextLimit - safetyMargin;

            // Split by paragraphs first for better coherence
            var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.None);
            
            var currentChunk = new List<string>();
            var currentTokenCount = 0;

            foreach (var paragraph in paragraphs)
            {
                var paragraphTokens = await EstimateTokenCountAsync(paragraph, model);

                // If single paragraph exceeds chunk size, split it further
                if (paragraphTokens > chunkSizeTokens)
                {
                    // Add current chunk if it has content
                    if (currentChunk.Any())
                    {
                        chunks.Add(string.Join("\n\n", currentChunk));
                        currentChunk.Clear();
                        currentTokenCount = 0;
                    }

                    // Split large paragraph by sentences
                    var sentenceChunks = await SplitParagraphBySentences(paragraph, chunkSizeTokens, model, overlap);
                    chunks.AddRange(sentenceChunks);
                }
                else if (currentTokenCount + paragraphTokens > chunkSizeTokens)
                {
                    // Current chunk is full, start new chunk
                    if (currentChunk.Any())
                    {
                        var chunkText = string.Join("\n\n", currentChunk);
                        chunks.Add(chunkText);

                        // Handle overlap
                        if (overlap > 0 && currentChunk.Count > 1)
                        {
                            var lastParagraph = currentChunk.Last();
                            var overlapTokens = await EstimateTokenCountAsync(lastParagraph, model);
                            
                            if (overlapTokens <= overlap)
                            {
                                currentChunk = new List<string> { lastParagraph, paragraph };
                                currentTokenCount = overlapTokens + paragraphTokens;
                            }
                            else
                            {
                                currentChunk = new List<string> { paragraph };
                                currentTokenCount = paragraphTokens;
                            }
                        }
                        else
                        {
                            currentChunk = new List<string> { paragraph };
                            currentTokenCount = paragraphTokens;
                        }
                    }
                }
                else
                {
                    // Add paragraph to current chunk
                    currentChunk.Add(paragraph);
                    currentTokenCount += paragraphTokens;
                }
            }

            // Add remaining chunk
            if (currentChunk.Any())
            {
                chunks.Add(string.Join("\n\n", currentChunk));
            }

            _logger.LogInformation("Split text into {ChunkCount} chunks", chunks.Count);
            return chunks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error splitting text into chunks");
            throw;
        }
    }

    public async Task<TimeSpan> EstimateProcessingTimeAsync(int inputTokens, int outputTokens, string model = "claude-3-sonnet-20240229")
    {
        try
        {
            await Task.CompletedTask;

            if (!_modelLimits.TryGetValue(model, out var limits))
            {
                limits = _modelLimits["claude-3-sonnet-20240229"];
            }

            // Estimation based on observed performance patterns
            var inputProcessingTime = inputTokens / limits.TokensPerSecondInput;
            var outputGenerationTime = outputTokens / limits.TokensPerSecondOutput;
            var networkLatency = 2.0; // Base network overhead in seconds

            var totalSeconds = inputProcessingTime + outputGenerationTime + networkLatency;
            var estimatedTime = TimeSpan.FromSeconds(totalSeconds);

            _logger.LogDebug("Estimated processing time: {ProcessingTime} for {InputTokens} input + {OutputTokens} output tokens",
                estimatedTime, inputTokens, outputTokens);

            return estimatedTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating processing time");
            throw;
        }
    }

    private static int EstimateTokensUsingHeuristics(string text)
    {
        // Claude tokenization approximation
        // Based on empirical observations and common patterns

        if (string.IsNullOrEmpty(text))
            return 0;

        // Base character-to-token ratio (approximately 4 characters per token)
        var baseTokens = text.Length / 4.0;

        // Adjustments for different content types
        var adjustmentFactor = 1.0;

        // Code content typically has more tokens per character
        if (IsCodeContent(text))
        {
            adjustmentFactor = 1.3;
        }
        // Natural language is more efficient
        else if (IsNaturalLanguage(text))
        {
            adjustmentFactor = 0.85;
        }
        // Mixed content
        else
        {
            adjustmentFactor = 1.0;
        }

        // Account for special characters and punctuation
        var specialCharCount = Regex.Matches(text, @"[^\w\s]").Count;
        var specialCharTokens = specialCharCount * 0.5; // Special chars often create partial tokens

        // Account for whitespace and line breaks
        var whitespaceCount = Regex.Matches(text, @"\s").Count;
        var whitespaceTokens = whitespaceCount * 0.1; // Whitespace contributes minimally

        var totalTokens = (baseTokens * adjustmentFactor) + specialCharTokens + whitespaceTokens;

        return Math.Max(1, (int)Math.Ceiling(totalTokens));
    }

    private static bool IsCodeContent(string text)
    {
        var codeIndicators = new[]
        {
            @"\b(function|class|interface|import|export|const|let|var)\b",
            @"\{.*\}",
            @"\[.*\]",
            @"[;{}()]+",
            @"//.*|/\*[\s\S]*?\*/"
        };

        var codeMatches = codeIndicators.Sum(pattern => 
            Regex.Matches(text, pattern, RegexOptions.IgnoreCase).Count);

        return (codeMatches / (double)text.Length) > 0.05; // 5% threshold
    }

    private static bool IsNaturalLanguage(string text)
    {
        var naturalLanguageIndicators = new[]
        {
            @"\b(the|and|or|but|in|on|at|to|for|of|with|by)\b",
            @"[.!?]+",
            @"\b\w{4,}\b" // Longer words indicate natural language
        };

        var naturalMatches = naturalLanguageIndicators.Sum(pattern => 
            Regex.Matches(text, pattern, RegexOptions.IgnoreCase).Count);

        return (naturalMatches / (double)text.Length) > 0.15; // 15% threshold
    }

    private async Task<List<string>> SplitParagraphBySentences(string paragraph, int maxTokens, string model, int overlap)
    {
        var chunks = new List<string>();
        var sentences = SplitIntoSentences(paragraph);
        
        var currentChunk = new List<string>();
        var currentTokenCount = 0;

        foreach (var sentence in sentences)
        {
            var sentenceTokens = await EstimateTokenCountAsync(sentence, model);

            if (currentTokenCount + sentenceTokens > maxTokens && currentChunk.Any())
            {
                // Current chunk is full
                chunks.Add(string.Join(" ", currentChunk));

                // Handle overlap
                if (overlap > 0 && currentChunk.Count > 1)
                {
                    var lastSentence = currentChunk.Last();
                    var overlapTokens = await EstimateTokenCountAsync(lastSentence, model);
                    
                    if (overlapTokens <= overlap)
                    {
                        currentChunk = new List<string> { lastSentence, sentence };
                        currentTokenCount = overlapTokens + sentenceTokens;
                    }
                    else
                    {
                        currentChunk = new List<string> { sentence };
                        currentTokenCount = sentenceTokens;
                    }
                }
                else
                {
                    currentChunk = new List<string> { sentence };
                    currentTokenCount = sentenceTokens;
                }
            }
            else
            {
                currentChunk.Add(sentence);
                currentTokenCount += sentenceTokens;
            }
        }

        if (currentChunk.Any())
        {
            chunks.Add(string.Join(" ", currentChunk));
        }

        return chunks;
    }

    private static List<string> SplitIntoSentences(string text)
    {
        // Simple sentence splitting with improved handling of edge cases
        var sentencePattern = @"(?<=[.!?])\s+(?=[A-Z])";
        var sentences = Regex.Split(text, sentencePattern)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return sentences;
    }

    private static Dictionary<string, ModelLimits> InitializeModelLimits()
    {
        return new Dictionary<string, ModelLimits>
        {
            ["claude-3-opus-20240229"] = new ModelLimits
            {
                MaxContextTokens = 200000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 1000,
                TokensPerSecondOutput = 50
            },
            ["claude-3-sonnet-20240229"] = new ModelLimits
            {
                MaxContextTokens = 200000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 1200,
                TokensPerSecondOutput = 60
            },
            ["claude-3-haiku-20240307"] = new ModelLimits
            {
                MaxContextTokens = 200000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 1500,
                TokensPerSecondOutput = 80
            },
            ["claude-2.1"] = new ModelLimits
            {
                MaxContextTokens = 200000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 800,
                TokensPerSecondOutput = 40
            },
            ["claude-2.0"] = new ModelLimits
            {
                MaxContextTokens = 100000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 700,
                TokensPerSecondOutput = 35
            },
            ["claude-instant-1.2"] = new ModelLimits
            {
                MaxContextTokens = 100000,
                MaxOutputTokens = 4096,
                TokensPerSecondInput = 2000,
                TokensPerSecondOutput = 100
            }
        };
    }

    private static Dictionary<string, CostStructure> InitializeCostStructures()
    {
        return new Dictionary<string, CostStructure>
        {
            ["claude-3-opus-20240229"] = new CostStructure
            {
                InputCostPer1KTokens = 0.015,
                OutputCostPer1KTokens = 0.075
            },
            ["claude-3-sonnet-20240229"] = new CostStructure
            {
                InputCostPer1KTokens = 0.003,
                OutputCostPer1KTokens = 0.015
            },
            ["claude-3-haiku-20240307"] = new CostStructure
            {
                InputCostPer1KTokens = 0.00025,
                OutputCostPer1KTokens = 0.00125
            },
            ["claude-2.1"] = new CostStructure
            {
                InputCostPer1KTokens = 0.008,
                OutputCostPer1KTokens = 0.024
            },
            ["claude-2.0"] = new CostStructure
            {
                InputCostPer1KTokens = 0.008,
                OutputCostPer1KTokens = 0.024
            },
            ["claude-instant-1.2"] = new CostStructure
            {
                InputCostPer1KTokens = 0.0008,
                OutputCostPer1KTokens = 0.0024
            }
        };
    }

    private class ModelLimits
    {
        public int MaxContextTokens { get; set; }
        public int MaxOutputTokens { get; set; }
        public double TokensPerSecondInput { get; set; }
        public double TokensPerSecondOutput { get; set; }
    }

    private class CostStructure
    {
        public double InputCostPer1KTokens { get; set; }
        public double OutputCostPer1KTokens { get; set; }
    }
}