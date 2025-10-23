using AI.PromptEngineering.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace AI.PromptEngineering.Services;

/// <summary>
/// Prompt optimization service interface
/// </summary>
public interface IPromptOptimizationService
{
    /// <summary>
    /// Optimize a prompt
    /// </summary>
    Task<PromptOptimizationResult> OptimizePromptAsync(string prompt, PromptOptimizationOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate improvement suggestions
    /// </summary>
    Task<List<PromptImprovementSuggestion>> GenerateImprovementSuggestionsAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create A/B test
    /// </summary>
    Task<PromptABTest> CreateABTestAsync(string variantA, string variantB, string testName, string description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record A/B test result
    /// </summary>
    Task RecordABTestResultAsync(string testId, string variant, double qualityScore, double responseTime, double cost, bool success, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get A/B test winner
    /// </summary>
    Task<string?> GetABTestWinnerAsync(string testId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Prompt optimization service implementation
/// </summary>
public class PromptOptimizationService : IPromptOptimizationService
{
    private readonly ILogger<PromptOptimizationService> _logger;
    private readonly ConcurrentDictionary<string, PromptABTest> _abTests;

    public PromptOptimizationService(ILogger<PromptOptimizationService> logger)
    {
        _logger = logger;
        _abTests = new ConcurrentDictionary<string, PromptABTest>();
    }

    public async Task<PromptOptimizationResult> OptimizePromptAsync(string prompt, PromptOptimizationOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Optimizing prompt with {Length} characters", prompt.Length);

            var originalMetrics = CalculateMetrics(prompt);
            var optimizedPrompt = prompt;

            var optimizationsApplied = new List<string>();

            // Apply optimizations based on options
            if (options.OptimizeForTokens)
            {
                optimizedPrompt = OptimizeForTokens(optimizedPrompt);
                optimizationsApplied.Add("Token optimization");
            }

            if (options.RemoveRedundancy)
            {
                optimizedPrompt = RemoveRedundancy(optimizedPrompt);
                optimizationsApplied.Add("Redundancy removal");
            }

            if (options.OptimizeForClarity)
            {
                optimizedPrompt = OptimizeForClarity(optimizedPrompt);
                optimizationsApplied.Add("Clarity enhancement");
            }

            if (options.MaxTokens.HasValue)
            {
                optimizedPrompt = TruncateToTokenLimit(optimizedPrompt, options.MaxTokens.Value);
                optimizationsApplied.Add($"Token limit enforcement ({options.MaxTokens.Value})");
            }

            if (options.OptimizeForCost)
            {
                optimizedPrompt = OptimizeForCost(optimizedPrompt);
                optimizationsApplied.Add("Cost optimization");
            }

            var optimizedMetrics = CalculateMetrics(optimizedPrompt);
            var improvement = CalculateImprovement(originalMetrics, optimizedMetrics);

            var result = new PromptOptimizationResult
            {
                OriginalPrompt = prompt,
                OptimizedPrompt = optimizedPrompt,
                OriginalMetrics = originalMetrics,
                OptimizedMetrics = optimizedMetrics,
                OptimizationsApplied = optimizationsApplied,
                ImprovementPercentage = improvement,
                OptimizedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Prompt optimized with {Improvement:F2}% improvement. Applied {Count} optimizations",
                improvement, optimizationsApplied.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing prompt");
            throw;
        }
    }

    public async Task<List<PromptImprovementSuggestion>> GenerateImprovementSuggestionsAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Generating improvement suggestions for prompt");

            var suggestions = new List<PromptImprovementSuggestion>();

            // Check for verbosity
            var wordCount = prompt.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount > 200)
            {
                suggestions.Add(new PromptImprovementSuggestion
                {
                    Type = "Verbosity",
                    Description = "Prompt is verbose and could be more concise",
                    OriginalText = prompt,
                    SuggestedText = TruncateToWordCount(prompt, 150),
                    ImpactScore = 0.7,
                    Rationale = "Shorter prompts reduce token usage and improve clarity"
                });
            }

            // Check for redundant phrases
            var redundantPhrases = FindRedundantPhrases(prompt);
            if (redundantPhrases.Any())
            {
                var cleaned = prompt;
                foreach (var phrase in redundantPhrases)
                {
                    cleaned = Regex.Replace(cleaned, Regex.Escape(phrase), string.Empty, RegexOptions.IgnoreCase);
                }

                suggestions.Add(new PromptImprovementSuggestion
                {
                    Type = "Redundancy",
                    Description = $"Found {redundantPhrases.Count} redundant phrases",
                    OriginalText = prompt,
                    SuggestedText = cleaned,
                    ImpactScore = 0.6,
                    Rationale = "Removing redundant phrases improves clarity and reduces tokens"
                });
            }

            // Check for unclear instructions
            if (!ContainsClearInstructions(prompt))
            {
                suggestions.Add(new PromptImprovementSuggestion
                {
                    Type = "Clarity",
                    Description = "Instructions could be clearer",
                    OriginalText = prompt,
                    SuggestedText = AddClearInstructions(prompt),
                    ImpactScore = 0.8,
                    Rationale = "Clear instructions improve model understanding and output quality"
                });
            }

            // Check for missing structure
            if (!HasGoodStructure(prompt))
            {
                suggestions.Add(new PromptImprovementSuggestion
                {
                    Type = "Structure",
                    Description = "Prompt lacks clear structure",
                    OriginalText = prompt,
                    SuggestedText = AddStructure(prompt),
                    ImpactScore = 0.75,
                    Rationale = "Well-structured prompts are easier to follow and produce better results"
                });
            }

            // Check for specific vs vague language
            if (ContainsVagueLanguage(prompt))
            {
                suggestions.Add(new PromptImprovementSuggestion
                {
                    Type = "Specificity",
                    Description = "Prompt contains vague language",
                    OriginalText = prompt,
                    SuggestedText = MakeMoreSpecific(prompt),
                    ImpactScore = 0.85,
                    Rationale = "Specific language reduces ambiguity and improves consistency"
                });
            }

            _logger.LogInformation("Generated {Count} improvement suggestions", suggestions.Count);

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating improvement suggestions");
            throw;
        }
    }

    public async Task<PromptABTest> CreateABTestAsync(string variantA, string variantB, string testName, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Creating A/B test: {TestName}", testName);

            var test = new PromptABTest
            {
                Id = Guid.NewGuid().ToString(),
                Name = testName,
                Description = description,
                VariantA = variantA,
                VariantB = variantB,
                MetricsA = new ABTestMetrics(),
                MetricsB = new ABTestMetrics(),
                StartedAt = DateTime.UtcNow
            };

            _abTests[test.Id] = test;

            _logger.LogInformation("A/B test {TestId} created: {TestName}", test.Id, testName);

            return test;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating A/B test");
            throw;
        }
    }

    public async Task RecordABTestResultAsync(string testId, string variant, double qualityScore, double responseTime, double cost, bool success, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            if (!_abTests.TryGetValue(testId, out var test))
            {
                throw new ArgumentException($"A/B test {testId} not found");
            }

            var metrics = variant.ToLower() == "a" ? test.MetricsA : test.MetricsB;

            // Update metrics
            metrics.RequestCount++;
            metrics.AverageResponseTime = UpdateRunningAverage(metrics.AverageResponseTime, responseTime, metrics.RequestCount);
            metrics.AverageQualityScore = UpdateRunningAverage(metrics.AverageQualityScore, qualityScore, metrics.RequestCount);
            metrics.AverageCost = UpdateRunningAverage(metrics.AverageCost, cost, metrics.RequestCount);
            
            if (success)
            {
                metrics.SuccessRate = (metrics.SuccessRate * (metrics.RequestCount - 1) + 1.0) / metrics.RequestCount;
            }
            else
            {
                metrics.SuccessRate = (metrics.SuccessRate * (metrics.RequestCount - 1)) / metrics.RequestCount;
                metrics.ErrorCount++;
            }

            _logger.LogDebug("Recorded result for variant {Variant} in test {TestId}", variant, testId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording A/B test result");
            throw;
        }
    }

    public async Task<string?> GetABTestWinnerAsync(string testId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            if (!_abTests.TryGetValue(testId, out var test))
            {
                throw new ArgumentException($"A/B test {testId} not found");
            }

            // Require minimum sample size
            if (test.MetricsA.RequestCount < 10 || test.MetricsB.RequestCount < 10)
            {
                _logger.LogWarning("Insufficient data for A/B test {TestId}", testId);
                return null;
            }

            // Calculate composite scores
            var scoreA = CalculateCompositeScore(test.MetricsA);
            var scoreB = CalculateCompositeScore(test.MetricsB);

            // Determine winner with minimum difference threshold
            var minDifference = 0.05; // 5% minimum improvement
            
            if (Math.Abs(scoreA - scoreB) < minDifference)
            {
                _logger.LogInformation("A/B test {TestId}: No clear winner (scores too close)", testId);
                return null;
            }

            var winner = scoreA > scoreB ? "A" : "B";
            test.WinningVariant = winner;
            test.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation("A/B test {TestId} winner: Variant {Winner} (Score: {ScoreA:F2} vs {ScoreB:F2})",
                testId, winner, scoreA, scoreB);

            return winner;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining A/B test winner");
            throw;
        }
    }

    private static PromptMetrics CalculateMetrics(string prompt)
    {
        var words = prompt.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        return new PromptMetrics
        {
            CharacterCount = prompt.Length,
            WordCount = words.Length,
            TokenCount = EstimateTokenCount(prompt),
            ReadabilityScore = CalculateReadability(prompt),
            ClarityScore = CalculateClarity(prompt),
            EstimatedCost = EstimateCost(prompt)
        };
    }

    private static string OptimizeForTokens(string prompt)
    {
        // Remove excessive whitespace
        var optimized = Regex.Replace(prompt, @"\s+", " ");
        
        // Remove filler words
        var fillerWords = new[] { "basically", "actually", "literally", "very", "really", "just" };
        foreach (var word in fillerWords)
        {
            optimized = Regex.Replace(optimized, $@"\b{word}\b", string.Empty, RegexOptions.IgnoreCase);
        }

        return optimized.Trim();
    }

    private static string RemoveRedundancy(string prompt)
    {
        var sentences = prompt.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();

        foreach (var sentence in sentences)
        {
            var normalized = sentence.Trim().ToLowerInvariant();
            if (!unique.Contains(normalized))
            {
                unique.Add(normalized);
                result.Add(sentence.Trim());
            }
        }

        return string.Join(". ", result) + ".";
    }

    private static string OptimizeForClarity(string prompt)
    {
        // Add clear structure if missing
        if (!prompt.Contains("\n\n") && prompt.Length > 200)
        {
            var sentences = prompt.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var structured = new List<string>();
            
            for (int i = 0; i < sentences.Length; i++)
            {
                structured.Add(sentences[i].Trim() + ".");
                if (i % 3 == 2) structured.Add("\n");
            }

            return string.Join(" ", structured).Trim();
        }

        return prompt;
    }

    private static string TruncateToTokenLimit(string prompt, int maxTokens)
    {
        var estimatedTokens = EstimateTokenCount(prompt);
        if (estimatedTokens <= maxTokens)
        {
            return prompt;
        }

        var ratio = maxTokens / (double)estimatedTokens;
        var targetLength = (int)(prompt.Length * ratio);
        
        return prompt.Substring(0, Math.Min(targetLength, prompt.Length));
    }

    private static string OptimizeForCost(string prompt)
    {
        // Combine token and redundancy optimization for cost reduction
        var optimized = OptimizeForTokens(prompt);
        optimized = RemoveRedundancy(optimized);
        return optimized;
    }

    private static double CalculateImprovement(PromptMetrics original, PromptMetrics optimized)
    {
        var tokenImprovement = (original.TokenCount - optimized.TokenCount) / (double)original.TokenCount;
        var costImprovement = (original.EstimatedCost - optimized.EstimatedCost) / original.EstimatedCost;
        var clarityImprovement = (optimized.ClarityScore - original.ClarityScore) / 100.0;

        return (tokenImprovement + costImprovement + clarityImprovement) / 3.0 * 100.0;
    }

    private static List<string> FindRedundantPhrases(string prompt)
    {
        var redundant = new List<string>();
        var commonRedundancies = new[]
        {
            "please note that",
            "it is important to",
            "you should know that",
            "as you can see",
            "in order to",
            "due to the fact that"
        };

        foreach (var phrase in commonRedundancies)
        {
            if (prompt.Contains(phrase, StringComparison.OrdinalIgnoreCase))
            {
                redundant.Add(phrase);
            }
        }

        return redundant;
    }

    private static bool ContainsClearInstructions(string prompt)
    {
        var instructionWords = new[] { "please", "you should", "must", "need to", "required to", "analyze", "generate", "create", "write" };
        return instructionWords.Any(word => prompt.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    private static string AddClearInstructions(string prompt)
    {
        return $"Task: {prompt}\n\nPlease provide a clear and detailed response following these instructions.";
    }

    private static bool HasGoodStructure(string prompt)
    {
        return prompt.Contains("\n") || prompt.Contains("1.") || prompt.Contains("•") || prompt.Contains("-");
    }

    private static string AddStructure(string prompt)
    {
        var sentences = prompt.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (sentences.Length <= 1)
            return prompt;

        var structured = "Instructions:\n";
        for (int i = 0; i < sentences.Length; i++)
        {
            structured += $"{i + 1}. {sentences[i].Trim()}\n";
        }

        return structured;
    }

    private static bool ContainsVagueLanguage(string prompt)
    {
        var vagueWords = new[] { "something", "somehow", "maybe", "perhaps", "possibly", "some", "any" };
        return vagueWords.Any(word => Regex.IsMatch(prompt, $@"\b{word}\b", RegexOptions.IgnoreCase));
    }

    private static string MakeMoreSpecific(string prompt)
    {
        var replacements = new Dictionary<string, string>
        {
            { "something", "a specific item" },
            { "somehow", "through a defined method" },
            { "maybe", "if applicable" },
            { "some", "several" },
            { "any", "all relevant" }
        };

        var result = prompt;
        foreach (var replacement in replacements)
        {
            result = Regex.Replace(result, $@"\b{replacement.Key}\b", replacement.Value, RegexOptions.IgnoreCase);
        }

        return result;
    }

    private static string TruncateToWordCount(string prompt, int maxWords)
    {
        var words = prompt.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length <= maxWords)
            return prompt;

        return string.Join(" ", words.Take(maxWords));
    }

    private static double UpdateRunningAverage(double currentAverage, double newValue, int count)
    {
        return (currentAverage * (count - 1) + newValue) / count;
    }

    private static double CalculateCompositeScore(ABTestMetrics metrics)
    {
        // Weighted composite score
        return (metrics.AverageQualityScore * 0.4) +
               (metrics.SuccessRate * 100 * 0.3) +
               ((1.0 / metrics.AverageResponseTime) * 10 * 0.2) +
               ((1.0 / metrics.AverageCost) * 0.1);
    }

    private static int EstimateTokenCount(string text)
    {
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    private static double CalculateReadability(string text)
    {
        var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (sentences.Length == 0) return 100.0;

        var avgWordsPerSentence = words.Length / (double)sentences.Length;
        var score = 206.835 - 1.015 * avgWordsPerSentence;
        
        return Math.Max(0, Math.Min(100, score));
    }

    private static double CalculateClarity(string text)
    {
        var score = 100.0;
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var avgSentenceLength = sentences.Average(s => s.Length);
        
        if (avgSentenceLength > 100) score -= 20;
        if (avgSentenceLength > 150) score -= 20;

        return Math.Max(0, score);
    }

    private static double EstimateCost(string prompt, string model = "gpt-4")
    {
        var tokens = EstimateTokenCount(prompt);
        var costPer1KTokens = 0.03;
        return (tokens / 1000.0) * costPer1KTokens;
    }
}