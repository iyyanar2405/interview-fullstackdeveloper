using Module_03_Content_Safety.Models;

namespace Module_03_Content_Safety.Services;

public interface IContentModerationService
{
    Task<ModerationResult> ModerateContentAsync(string content, ModerationMode mode = ModerationMode.Standard);
    Task<ContentSafetyResult> AnalyzeContentSafetyAsync(string content);
    Task<bool> IsContentSafeAsync(string content, double threshold = 5.0);
    Task<string> FilterContentAsync(string content);
}

public class ContentModerationService : IContentModerationService
{
    private readonly ILogger<ContentModerationService> _logger;
    private readonly IToxicityDetectionService _toxicityService;
    private readonly IPiiDetectionService _piiService;
    private readonly IPromptInjectionService _promptInjectionService;
    private readonly IConfiguration _configuration;

    public ContentModerationService(
        ILogger<ContentModerationService> logger,
        IToxicityDetectionService toxicityService,
        IPiiDetectionService piiService,
        IPromptInjectionService promptInjectionService,
        IConfiguration configuration)
    {
        _logger = logger;
        _toxicityService = toxicityService;
        _piiService = piiService;
        _promptInjectionService = promptInjectionService;
        _configuration = configuration;
    }

    public async Task<ModerationResult> ModerateContentAsync(string content, ModerationMode mode = ModerationMode.Standard)
    {
        _logger.LogInformation("Moderating content. Mode: {Mode}, Length: {Length}", mode, content.Length);

        var result = new ModerationResult
        {
            Flags = new List<ModerationFlag>()
        };

        var threshold = GetThresholdForMode(mode);

        // Check toxicity
        var toxicityResult = await _toxicityService.DetectToxicityAsync(content, threshold);
        if (toxicityResult.IsToxic)
        {
            result.Flags.Add(new ModerationFlag
            {
                Category = "Toxicity",
                Severity = toxicityResult.ToxicityScore * 10,
                Reason = toxicityResult.Reason ?? "Toxic content detected",
                Evidence = string.Join(", ", toxicityResult.DetectedPhrases.Select(p => p.Phrase)),
                AutoModerated = true
            });
        }

        // Check PII
        var piiResult = await _piiService.DetectPiiAsync(content);
        if (piiResult.ContainsPii)
        {
            result.Flags.Add(new ModerationFlag
            {
                Category = "PII",
                Severity = piiResult.PiiCount * 2.0,
                Reason = $"Contains {piiResult.PiiCount} PII entities",
                Evidence = string.Join(", ", piiResult.DetectedEntities.Select(e => e.Type.ToString())),
                AutoModerated = true
            });
        }

        // Check prompt injection
        var injectionResult = await _promptInjectionService.DetectPromptInjectionAsync(content);
        if (injectionResult.IsInjectionDetected)
        {
            result.Flags.Add(new ModerationFlag
            {
                Category = "PromptInjection",
                Severity = injectionResult.RiskScore,
                Reason = injectionResult.Explanation ?? "Prompt injection detected",
                Evidence = string.Join(", ", injectionResult.DetectedPatterns.Select(p => p.Pattern)),
                AutoModerated = true
            });
        }

        // Check for spam indicators
        var spamScore = DetectSpam(content);
        if (spamScore > 5.0)
        {
            result.Flags.Add(new ModerationFlag
            {
                Category = "Spam",
                Severity = spamScore,
                Reason = "Content appears to be spam",
                AutoModerated = true
            });
        }

        // Check for malicious links
        var maliciousLinksScore = DetectMaliciousLinks(content);
        if (maliciousLinksScore > 5.0)
        {
            result.Flags.Add(new ModerationFlag
            {
                Category = "MaliciousLinks",
                Severity = maliciousLinksScore,
                Reason = "Contains suspicious or malicious links",
                AutoModerated = true
            });
        }

        // Determine moderation decision
        result.RequiresModeration = result.Flags.Any();
        result.ConfidenceScore = result.Flags.Any() 
            ? result.Flags.Average(f => f.Severity) / 10.0 
            : 1.0;

        if (!result.RequiresModeration)
        {
            result.Decision = ModerationDecision.Approve;
        }
        else
        {
            var maxSeverity = result.Flags.Max(f => f.Severity);
            result.Decision = maxSeverity switch
            {
                >= 8.0 => ModerationDecision.Block,
                >= 6.0 => ModerationDecision.Reject,
                >= 4.0 => ModerationDecision.Review,
                _ => ModerationDecision.AutoModerate
            };
        }

        // Generate filtered content if needed
        if (result.Decision == ModerationDecision.AutoModerate)
        {
            result.FilteredContent = await FilterContentAsync(content);
        }

        result.Metadata["originalLength"] = content.Length;
        result.Metadata["flagCount"] = result.Flags.Count;
        result.Metadata["moderationMode"] = mode.ToString();

        _logger.LogInformation("Moderation complete. Decision: {Decision}, Flags: {FlagCount}", 
            result.Decision, result.Flags.Count);

        return result;
    }

    public async Task<ContentSafetyResult> AnalyzeContentSafetyAsync(string content)
    {
        _logger.LogInformation("Analyzing content safety. Length: {Length}", content.Length);

        var result = new ContentSafetyResult
        {
            Violations = new List<SafetyViolation>()
        };

        // Analyze toxicity
        var toxicityResult = await _toxicityService.DetectToxicityAsync(content);
        foreach (var category in toxicityResult.CategoryScores)
        {
            if (category.Value > 0.5)
            {
                result.Violations.Add(new SafetyViolation
                {
                    Type = ViolationType.Toxicity,
                    Category = category.Key.ToString(),
                    Severity = category.Value * 10,
                    Description = $"Content contains {category.Key.ToString().ToLower()} language",
                    RecommendedAction = category.Value > 0.8 ? "Block" : "Review"
                });
            }
            result.CategoryScores[$"Toxicity.{category.Key}"] = category.Value * 10;
        }

        // Analyze PII
        var piiResult = await _piiService.DetectPiiAsync(content);
        if (piiResult.ContainsPii)
        {
            foreach (var entity in piiResult.DetectedEntities)
            {
                result.Violations.Add(new SafetyViolation
                {
                    Type = ViolationType.PII,
                    Category = entity.Type.ToString(),
                    Severity = entity.Confidence * 8,
                    Description = $"Contains {entity.Type} at position {entity.StartPosition}",
                    DetectedContent = entity.Context,
                    Position = entity.StartPosition,
                    RecommendedAction = "Redact"
                });
            }
            result.CategoryScores["PII"] = Math.Min(piiResult.PiiCount * 2.0, 10.0);
        }

        // Analyze prompt injection
        var injectionResult = await _promptInjectionService.DetectPromptInjectionAsync(content);
        if (injectionResult.IsInjectionDetected)
        {
            result.Violations.Add(new SafetyViolation
            {
                Type = ViolationType.PromptInjection,
                Category = injectionResult.InjectionType.ToString(),
                Severity = injectionResult.RiskScore,
                Description = injectionResult.Explanation ?? "Prompt injection attempt detected",
                RecommendedAction = injectionResult.RiskScore > 7.0 ? "Block" : "Review"
            });
            result.CategoryScores["PromptInjection"] = injectionResult.RiskScore;
        }

        // Calculate overall risk score
        result.OverallRiskScore = result.CategoryScores.Any() 
            ? result.CategoryScores.Values.Average() 
            : 0.0;

        result.IsSafe = result.OverallRiskScore < 5.0;

        // Generate recommended action
        result.RecommendedAction = result.OverallRiskScore switch
        {
            >= 8.0 => "Block content immediately",
            >= 6.0 => "Reject and require human review",
            >= 4.0 => "Flag for review",
            >= 2.0 => "Auto-moderate and allow with modifications",
            _ => "Approve"
        };

        // Generate processed content
        if (!result.IsSafe && result.OverallRiskScore < 8.0)
        {
            result.ProcessedContent = await FilterContentAsync(content);
        }

        _logger.LogInformation("Content safety analysis complete. Is safe: {IsSafe}, Risk score: {RiskScore:F2}", 
            result.IsSafe, result.OverallRiskScore);

        return result;
    }

    public async Task<bool> IsContentSafeAsync(string content, double threshold = 5.0)
    {
        var result = await AnalyzeContentSafetyAsync(content);
        return result.OverallRiskScore < threshold;
    }

    public async Task<string> FilterContentAsync(string content)
    {
        // Apply multiple filters
        var filtered = content;

        // Redact PII
        filtered = await _piiService.RedactPiiAsync(filtered);

        // Sanitize toxic content
        var toxicityResult = await _toxicityService.DetectToxicityAsync(filtered);
        if (toxicityResult.IsToxic && toxicityResult.SanitizedContent != null)
        {
            filtered = toxicityResult.SanitizedContent;
        }

        return filtered;
    }

    private double GetThresholdForMode(ModerationMode mode)
    {
        return mode switch
        {
            ModerationMode.Strict => 0.3,
            ModerationMode.Standard => 0.5,
            ModerationMode.Lenient => 0.7,
            ModerationMode.Custom => 0.5,
            _ => 0.5
        };
    }

    private double DetectSpam(string content)
    {
        double score = 0.0;

        // Check for excessive capitalization
        var upperCount = content.Count(char.IsUpper);
        var totalLetters = content.Count(char.IsLetter);
        if (totalLetters > 0 && (double)upperCount / totalLetters > 0.5)
        {
            score += 2.0;
        }

        // Check for excessive punctuation
        var punctuationCount = content.Count(c => "!?.,;:".Contains(c));
        if (punctuationCount > content.Length * 0.1)
        {
            score += 2.0;
        }

        // Check for repetitive characters
        var repetitivePattern = System.Text.RegularExpressions.Regex.IsMatch(content, @"(.)\1{4,}");
        if (repetitivePattern)
        {
            score += 2.0;
        }

        // Check for common spam keywords
        var spamKeywords = new[] { "buy now", "click here", "limited time", "act now", "free money", "winner", "congratulations" };
        var spamCount = spamKeywords.Count(keyword => content.ToLowerInvariant().Contains(keyword));
        score += spamCount * 1.5;

        // Check for excessive URLs
        var urlCount = System.Text.RegularExpressions.Regex.Matches(content, @"https?://").Count;
        if (urlCount > 3)
        {
            score += (urlCount - 3) * 1.0;
        }

        return Math.Min(score, 10.0);
    }

    private double DetectMaliciousLinks(string content)
    {
        double score = 0.0;

        var urlPattern = new System.Text.RegularExpressions.Regex(@"https?://[^\s]+", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var urls = urlPattern.Matches(content);

        foreach (System.Text.RegularExpressions.Match match in urls)
        {
            var url = match.Value.ToLowerInvariant();

            // Check for suspicious TLDs
            if (url.Contains(".tk") || url.Contains(".ml") || url.Contains(".ga") || 
                url.Contains(".cf") || url.Contains(".gq"))
            {
                score += 2.0;
            }

            // Check for IP addresses in URLs
            if (System.Text.RegularExpressions.Regex.IsMatch(url, @"https?://\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
            {
                score += 2.0;
            }

            // Check for URL shorteners (could hide destination)
            var shorteners = new[] { "bit.ly", "tinyurl", "goo.gl", "t.co", "ow.ly" };
            if (shorteners.Any(s => url.Contains(s)))
            {
                score += 1.0;
            }

            // Check for excessive subdomains
            var domainPart = url.Replace("http://", "").Replace("https://", "").Split('/')[0];
            var subdomainCount = domainPart.Split('.').Length - 2;
            if (subdomainCount > 3)
            {
                score += 1.5;
            }

            // Check for suspicious keywords in URL
            var suspiciousKeywords = new[] { "phishing", "malware", "virus", "hack", "crack", "keygen" };
            if (suspiciousKeywords.Any(k => url.Contains(k)))
            {
                score += 3.0;
            }
        }

        return Math.Min(score, 10.0);
    }
}
