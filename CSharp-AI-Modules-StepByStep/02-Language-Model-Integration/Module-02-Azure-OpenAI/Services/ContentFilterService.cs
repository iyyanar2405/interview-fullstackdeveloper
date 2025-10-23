using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI.AzureOpenAI.Services;

/// <summary>
/// Content filter service interface
/// </summary>
public interface IContentFilterService
{
    /// <summary>
    /// Validate content against Azure content filters
    /// </summary>
    Task<ContentFilterResult> ValidateContentAsync(IEnumerable<ChatRequestMessage> messages);

    /// <summary>
    /// Check single content string
    /// </summary>
    Task<ContentFilterResult> CheckContentAsync(string content);

    /// <summary>
    /// Pre-filter content before sending to API
    /// </summary>
    Task<string> PreFilterContentAsync(string content);

    /// <summary>
    /// Check if content contains prohibited patterns
    /// </summary>
    Task<bool> ContainsProhibitedContentAsync(string content);

    /// <summary>
    /// Get content safety score (0-1, where 1 is safest)
    /// </summary>
    Task<double> GetSafetyScoreAsync(string content);
}

/// <summary>
/// Content filter service implementation
/// </summary>
public class ContentFilterService : IContentFilterService
{
    private readonly AzureOpenAISettings _settings;
    private readonly ILogger<ContentFilterService> _logger;
    private readonly List<string> _prohibitedPatterns;
    private readonly List<string> _sensitiveTopics;

    public ContentFilterService(
        IOptions<AzureOpenAISettings> settings,
        ILogger<ContentFilterService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _prohibitedPatterns = InitializeProhibitedPatterns();
        _sensitiveTopics = InitializeSensitiveTopics();
    }

    public async Task<ContentFilterResult> ValidateContentAsync(IEnumerable<ChatRequestMessage> messages)
    {
        try
        {
            _logger.LogDebug("Validating {MessageCount} messages for content safety", messages.Count());

            var combinedContent = string.Join("\n", messages.Select(m => m.Content));
            return await CheckContentAsync(combinedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating message content");
            throw;
        }
    }

    public async Task<ContentFilterResult> CheckContentAsync(string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new ContentFilterResult
                {
                    IsBlocked = false,
                    SafetyScore = 1.0,
                    Categories = new List<ContentFilterCategory>()
                };
            }

            _logger.LogDebug("Checking content safety for text of length {Length}", content.Length);

            var result = new ContentFilterResult
            {
                IsBlocked = false,
                SafetyScore = 1.0,
                Categories = new List<ContentFilterCategory>()
            };

            // Check for prohibited patterns
            if (await ContainsProhibitedContentAsync(content))
            {
                result.IsBlocked = true;
                result.Reason = "Contains prohibited content";
                result.SafetyScore = 0.0;
                result.Categories.Add(new ContentFilterCategory
                {
                    Category = "prohibited_content",
                    Severity = "high",
                    Confidence = 0.95
                });
            }

            // Check for hate speech
            var hateResult = await CheckHateSpeechAsync(content);
            if (hateResult.IsDetected)
            {
                result.Categories.Add(new ContentFilterCategory
                {
                    Category = "hate",
                    Severity = hateResult.Severity,
                    Confidence = hateResult.Confidence
                });

                if (hateResult.Severity == "high")
                {
                    result.IsBlocked = true;
                    result.Reason = "Contains hate speech";
                }
            }

            // Check for violence
            var violenceResult = await CheckViolenceAsync(content);
            if (violenceResult.IsDetected)
            {
                result.Categories.Add(new ContentFilterCategory
                {
                    Category = "violence",
                    Severity = violenceResult.Severity,
                    Confidence = violenceResult.Confidence
                });

                if (violenceResult.Severity == "high")
                {
                    result.IsBlocked = true;
                    result.Reason = "Contains violent content";
                }
            }

            // Check for sexual content
            var sexualResult = await CheckSexualContentAsync(content);
            if (sexualResult.IsDetected)
            {
                result.Categories.Add(new ContentFilterCategory
                {
                    Category = "sexual",
                    Severity = sexualResult.Severity,
                    Confidence = sexualResult.Confidence
                });

                if (sexualResult.Severity == "high")
                {
                    result.IsBlocked = true;
                    result.Reason = "Contains sexual content";
                }
            }

            // Check for self-harm content
            var selfHarmResult = await CheckSelfHarmAsync(content);
            if (selfHarmResult.IsDetected)
            {
                result.Categories.Add(new ContentFilterCategory
                {
                    Category = "self_harm",
                    Severity = selfHarmResult.Severity,
                    Confidence = selfHarmResult.Confidence
                });

                if (selfHarmResult.Severity == "high")
                {
                    result.IsBlocked = true;
                    result.Reason = "Contains self-harm content";
                }
            }

            // Calculate overall safety score
            result.SafetyScore = await GetSafetyScoreAsync(content);

            if (result.IsBlocked)
            {
                _logger.LogWarning("Content blocked: {Reason}", result.Reason);
            }

            await Task.CompletedTask;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking content safety");
            throw;
        }
    }

    public async Task<string> PreFilterContentAsync(string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;

            _logger.LogDebug("Pre-filtering content of length {Length}", content.Length);

            var filteredContent = content;

            // Remove or replace sensitive information
            filteredContent = await RemoveSensitiveInformationAsync(filteredContent);

            // Clean up formatting
            filteredContent = CleanupContent(filteredContent);

            return filteredContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pre-filtering content");
            throw;
        }
    }

    public async Task<bool> ContainsProhibitedContentAsync(string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            var lowerContent = content.ToLowerInvariant();

            foreach (var pattern in _prohibitedPatterns)
            {
                if (Regex.IsMatch(lowerContent, pattern, RegexOptions.IgnoreCase))
                {
                    _logger.LogDebug("Prohibited pattern matched: {Pattern}", pattern);
                    return true;
                }
            }

            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for prohibited content");
            throw;
        }
    }

    public async Task<double> GetSafetyScoreAsync(string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
                return 1.0;

            double score = 1.0;

            // Check various safety factors
            if (await ContainsProhibitedContentAsync(content))
                score -= 0.5;

            // Check for sensitive topics
            var lowerContent = content.ToLowerInvariant();
            foreach (var topic in _sensitiveTopics)
            {
                if (lowerContent.Contains(topic.ToLowerInvariant()))
                {
                    score -= 0.1;
                }
            }

            // Check for excessive caps (possible shouting/aggression)
            var capsRatio = content.Count(char.IsUpper) / (double)content.Length;
            if (capsRatio > 0.3)
                score -= 0.2;

            // Check for excessive punctuation (possible aggression)
            var punctuationCount = content.Count(c => "!?".Contains(c));
            if (punctuationCount > 5)
                score -= 0.1;

            return Math.Max(0.0, score);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating safety score");
            throw;
        }
    }

    private async Task<CategoryDetectionResult> CheckHateSpeechAsync(string content)
    {
        // Simplified hate speech detection
        var hatePatterns = new[]
        {
            @"\b(hate|hatred)\b.*\b(group|people|race|religion)\b",
            @"\b(discriminat|prejudice|bias)\b.*\b(against|toward)\b",
            @"\b(supremacist|extremist)\b"
        };

        foreach (var pattern in hatePatterns)
        {
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                return new CategoryDetectionResult
                {
                    IsDetected = true,
                    Severity = "medium",
                    Confidence = 0.7
                };
            }
        }

        await Task.CompletedTask;
        return new CategoryDetectionResult { IsDetected = false };
    }

    private async Task<CategoryDetectionResult> CheckViolenceAsync(string content)
    {
        var violencePatterns = new[]
        {
            @"\b(kill|murder|assault|attack|violence|weapon)\b",
            @"\b(bomb|explosive|terrorism|terrorist)\b",
            @"\b(threat|threaten|harm|hurt|damage)\b"
        };

        foreach (var pattern in violencePatterns)
        {
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                return new CategoryDetectionResult
                {
                    IsDetected = true,
                    Severity = "medium",
                    Confidence = 0.6
                };
            }
        }

        await Task.CompletedTask;
        return new CategoryDetectionResult { IsDetected = false };
    }

    private async Task<CategoryDetectionResult> CheckSexualContentAsync(string content)
    {
        var sexualPatterns = new[]
        {
            @"\b(explicit|sexual|adult|nsfw)\b.*\b(content|material)\b",
            @"\b(pornograph|erotic)\b"
        };

        foreach (var pattern in sexualPatterns)
        {
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                return new CategoryDetectionResult
                {
                    IsDetected = true,
                    Severity = "medium",
                    Confidence = 0.6
                };
            }
        }

        await Task.CompletedTask;
        return new CategoryDetectionResult { IsDetected = false };
    }

    private async Task<CategoryDetectionResult> CheckSelfHarmAsync(string content)
    {
        var selfHarmPatterns = new[]
        {
            @"\b(suicide|self.harm|self.hurt)\b",
            @"\b(cut|cutting).*\b(myself|self)\b",
            @"\b(end.*life|kill.*myself)\b"
        };

        foreach (var pattern in selfHarmPatterns)
        {
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                return new CategoryDetectionResult
                {
                    IsDetected = true,
                    Severity = "high",
                    Confidence = 0.8
                };
            }
        }

        await Task.CompletedTask;
        return new CategoryDetectionResult { IsDetected = false };
    }

    private async Task<string> RemoveSensitiveInformationAsync(string content)
    {
        // Remove potential PII patterns
        var patterns = new Dictionary<string, string>
        {
            // Email addresses
            [@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b"] = "[EMAIL]",
            // Phone numbers
            [@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b"] = "[PHONE]",
            // Social Security Numbers
            [@"\b\d{3}-\d{2}-\d{4}\b"] = "[SSN]",
            // Credit card numbers (basic pattern)
            [@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b"] = "[CARD]"
        };

        var filteredContent = content;
        foreach (var (pattern, replacement) in patterns)
        {
            filteredContent = Regex.Replace(filteredContent, pattern, replacement, RegexOptions.IgnoreCase);
        }

        await Task.CompletedTask;
        return filteredContent;
    }

    private static string CleanupContent(string content)
    {
        // Remove excessive whitespace
        content = Regex.Replace(content, @"\s+", " ");
        
        // Remove excessive punctuation
        content = Regex.Replace(content, @"[!]{3,}", "!!");
        content = Regex.Replace(content, @"[?]{3,}", "??");
        
        return content.Trim();
    }

    private static List<string> InitializeProhibitedPatterns()
    {
        return new List<string>
        {
            @"\b(illegal|unlawful|criminal)\b.*\b(activity|action|behavior)\b",
            @"\b(hack|crack|break.*into)\b.*\b(system|network|account)\b",
            @"\b(piracy|steal|theft)\b.*\b(software|content|data)\b",
            @"\b(drug|narcotic)\b.*\b(deal|sell|distribute)\b"
        };
    }

    private static List<string> InitializeSensitiveTopics()
    {
        return new List<string>
        {
            "politics", "religion", "controversial", "sensitive",
            "private", "confidential", "classified", "restricted"
        };
    }
}

/// <summary>
/// Content filter result
/// </summary>
public class ContentFilterResult
{
    public bool IsBlocked { get; set; }
    public string? Reason { get; set; }
    public double SafetyScore { get; set; }
    public List<ContentFilterCategory> Categories { get; set; } = new();
}

/// <summary>
/// Content filter category
/// </summary>
public class ContentFilterCategory
{
    public required string Category { get; set; }
    public required string Severity { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// Category detection result
/// </summary>
public class CategoryDetectionResult
{
    public bool IsDetected { get; set; }
    public string Severity { get; set; } = "low";
    public double Confidence { get; set; }
}