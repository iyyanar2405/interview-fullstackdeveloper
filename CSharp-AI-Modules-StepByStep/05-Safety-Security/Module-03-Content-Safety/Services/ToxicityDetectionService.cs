using System.Text.RegularExpressions;
using Module_03_Content_Safety.Models;

namespace Module_03_Content_Safety.Services;

public interface IToxicityDetectionService
{
    Task<ToxicityResult> DetectToxicityAsync(string content, double threshold = 0.5);
    Task<ToxicityResult> DetectToxicityByCategoryAsync(string content, List<ToxicityCategory> categories);
    Task<bool> IsToxicAsync(string content, double threshold = 0.5);
    Task<string> SanitizeToxicContentAsync(string content);
}

public class ToxicityDetectionService : IToxicityDetectionService
{
    private readonly ILogger<ToxicityDetectionService> _logger;
    private readonly Dictionary<ToxicityCategory, List<string>> _toxicKeywords;
    private readonly Dictionary<ToxicityCategory, List<Regex>> _toxicPatterns;

    public ToxicityDetectionService(ILogger<ToxicityDetectionService> logger)
    {
        _logger = logger;
        _toxicKeywords = InitializeToxicKeywords();
        _toxicPatterns = InitializeToxicPatterns();
    }

    public async Task<ToxicityResult> DetectToxicityAsync(string content, double threshold = 0.5)
    {
        _logger.LogInformation("Analyzing content for toxicity. Length: {Length}", content.Length);

        var result = new ToxicityResult
        {
            CategoryScores = new Dictionary<ToxicityCategory, double>()
        };

        // Analyze each toxicity category
        foreach (ToxicityCategory category in Enum.GetValues<ToxicityCategory>())
        {
            var score = await CalculateCategoryScoreAsync(content, category);
            result.CategoryScores[category] = score;
        }

        // Calculate overall toxicity score (max of all categories)
        result.ToxicityScore = result.CategoryScores.Values.Max();
        result.IsToxic = result.ToxicityScore >= threshold;

        // Detect specific toxic phrases
        result.DetectedPhrases = await DetectToxicPhrasesAsync(content);

        // Generate reason
        if (result.IsToxic)
        {
            var highestCategory = result.CategoryScores
                .OrderByDescending(x => x.Value)
                .First();
            result.Reason = $"Content classified as {highestCategory.Key} with confidence {highestCategory.Value:P0}";
        }

        // Generate sanitized version if toxic
        if (result.IsToxic)
        {
            result.SanitizedContent = await SanitizeToxicContentAsync(content);
        }

        _logger.LogInformation("Toxicity analysis complete. Is toxic: {IsToxic}, Score: {Score:F2}", 
            result.IsToxic, result.ToxicityScore);

        return result;
    }

    public async Task<ToxicityResult> DetectToxicityByCategoryAsync(string content, List<ToxicityCategory> categories)
    {
        var result = new ToxicityResult
        {
            CategoryScores = new Dictionary<ToxicityCategory, double>()
        };

        foreach (var category in categories)
        {
            var score = await CalculateCategoryScoreAsync(content, category);
            result.CategoryScores[category] = score;
        }

        result.ToxicityScore = result.CategoryScores.Values.Max();
        result.IsToxic = result.ToxicityScore > 0.5;
        result.DetectedPhrases = await DetectToxicPhrasesAsync(content);

        return result;
    }

    public async Task<bool> IsToxicAsync(string content, double threshold = 0.5)
    {
        var result = await DetectToxicityAsync(content, threshold);
        return result.IsToxic;
    }

    public async Task<string> SanitizeToxicContentAsync(string content)
    {
        var sanitized = content;

        // Replace toxic phrases with [REMOVED]
        var detectedPhrases = await DetectToxicPhrasesAsync(content);
        
        foreach (var phrase in detectedPhrases.OrderByDescending(p => p.StartPosition))
        {
            sanitized = sanitized.Remove(phrase.StartPosition, phrase.EndPosition - phrase.StartPosition)
                                .Insert(phrase.StartPosition, "[REMOVED]");
        }

        return sanitized;
    }

    private async Task<double> CalculateCategoryScoreAsync(string content, ToxicityCategory category)
    {
        await Task.CompletedTask;

        var contentLower = content.ToLowerInvariant();
        double score = 0.0;
        int matchCount = 0;

        // Check keyword matches
        if (_toxicKeywords.TryGetValue(category, out var keywords))
        {
            foreach (var keyword in keywords)
            {
                if (contentLower.Contains(keyword.ToLowerInvariant()))
                {
                    matchCount++;
                    score += 0.2; // Each keyword match adds 0.2 to score
                }
            }
        }

        // Check pattern matches
        if (_toxicPatterns.TryGetValue(category, out var patterns))
        {
            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(content))
                {
                    matchCount++;
                    score += 0.3; // Each pattern match adds 0.3 to score
                }
            }
        }

        // Normalize score to 0-1 range
        score = Math.Min(score, 1.0);

        return score;
    }

    private async Task<List<ToxicPhrase>> DetectToxicPhrasesAsync(string content)
    {
        await Task.CompletedTask;

        var phrases = new List<ToxicPhrase>();
        var contentLower = content.ToLowerInvariant();

        foreach (var categoryKeywords in _toxicKeywords)
        {
            foreach (var keyword in categoryKeywords.Value)
            {
                var keywordLower = keyword.ToLowerInvariant();
                int index = 0;
                
                while ((index = contentLower.IndexOf(keywordLower, index, StringComparison.Ordinal)) != -1)
                {
                    phrases.Add(new ToxicPhrase
                    {
                        Phrase = content.Substring(index, keyword.Length),
                        Category = categoryKeywords.Key,
                        Confidence = 0.8,
                        StartPosition = index,
                        EndPosition = index + keyword.Length
                    });
                    index += keyword.Length;
                }
            }
        }

        return phrases;
    }

    private Dictionary<ToxicityCategory, List<string>> InitializeToxicKeywords()
    {
        return new Dictionary<ToxicityCategory, List<string>>
        {
            [ToxicityCategory.Toxic] = new List<string>
            {
                "stupid", "idiot", "moron", "dumb", "fool", "jerk", "loser"
            },
            [ToxicityCategory.SevereToxic] = new List<string>
            {
                // Severe toxic words - use a proper profanity filter library
                "hate you", "kill yourself", "die"
            },
            [ToxicityCategory.Obscene] = new List<string>
            {
                // Obscene words - use a proper profanity filter library
            },
            [ToxicityCategory.Threat] = new List<string>
            {
                "kill", "murder", "attack", "hurt", "harm", "destroy", "bomb", "shoot"
            },
            [ToxicityCategory.Insult] = new List<string>
            {
                "ugly", "fat", "disgusting", "worthless", "pathetic", "useless"
            },
            [ToxicityCategory.IdentityHate] = new List<string>
            {
                // Identity-based hate speech indicators
                "subhuman", "inferior", "degenerate"
            },
            [ToxicityCategory.Sexual] = new List<string>
            {
                // Sexual content indicators
            },
            [ToxicityCategory.Violence] = new List<string>
            {
                "stab", "punch", "kick", "beat", "torture", "assault"
            }
        };
    }

    private Dictionary<ToxicityCategory, List<Regex>> InitializeToxicPatterns()
    {
        return new Dictionary<ToxicityCategory, List<Regex>>
        {
            [ToxicityCategory.Threat] = new List<Regex>
            {
                new Regex(@"\b(will|gonna|going to)\s+(kill|hurt|harm|attack)\s+you\b", RegexOptions.IgnoreCase),
                new Regex(@"\bi\s+hope\s+you\s+(die|suffer)\b", RegexOptions.IgnoreCase),
                new Regex(@"\byou\s+(deserve|should)\s+(die|death|pain)\b", RegexOptions.IgnoreCase)
            },
            [ToxicityCategory.Insult] = new List<Regex>
            {
                new Regex(@"\byou\s+are\s+(so\s+)?(stupid|dumb|idiotic|moronic)\b", RegexOptions.IgnoreCase),
                new Regex(@"\bshut\s+up\b", RegexOptions.IgnoreCase)
            },
            [ToxicityCategory.IdentityHate] = new List<Regex>
            {
                new Regex(@"\ball\s+\w+\s+are\s+(bad|evil|stupid|inferior)", RegexOptions.IgnoreCase)
            }
        };
    }
}
