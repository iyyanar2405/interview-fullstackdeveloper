using AI.Anthropic.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AI.Anthropic.Services;

/// <summary>
/// Constitutional AI service interface
/// </summary>
public interface IConstitutionalAIService
{
    /// <summary>
    /// Validate content against Constitutional AI principles
    /// </summary>
    Task<ConstitutionalValidationResult> ValidateAsync(string content, List<string> principles);

    /// <summary>
    /// Apply Constitutional AI feedback to improve content
    /// </summary>
    Task<string> ApplyConstitutionalFeedbackAsync(string content, List<string> principles);

    /// <summary>
    /// Generate Constitutional AI critique
    /// </summary>
    Task<string> GenerateCritiqueAsync(string content, List<string> principles);

    /// <summary>
    /// Check if content violates specific principle
    /// </summary>
    Task<bool> ViolatesPrincipleAsync(string content, string principle);

    /// <summary>
    /// Get Constitutional AI score for content
    /// </summary>
    Task<double> GetConstitutionalScoreAsync(string content, List<string> principles);
}

/// <summary>
/// Constitutional AI service implementation
/// </summary>
public class ConstitutionalAIService : IConstitutionalAIService
{
    private readonly AnthropicSettings _settings;
    private readonly ILogger<ConstitutionalAIService> _logger;
    private readonly Dictionary<string, Regex> _principlePatterns;

    public ConstitutionalAIService(
        IOptions<AnthropicSettings> settings,
        ILogger<ConstitutionalAIService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _principlePatterns = InitializePrinciplePatterns();
    }

    public async Task<ConstitutionalValidationResult> ValidateAsync(string content, List<string> principles)
    {
        try
        {
            _logger.LogDebug("Validating content against {PrincipleCount} Constitutional AI principles", principles.Count);

            var violatedPrinciples = new List<string>();
            var scores = new List<double>();

            foreach (var principle in principles)
            {
                var violates = await ViolatesPrincipleAsync(content, principle);
                if (violates)
                {
                    violatedPrinciples.Add(principle);
                }

                var score = await GetPrincipleScoreAsync(content, principle);
                scores.Add(score);
            }

            var overallScore = scores.Any() ? scores.Average() : 1.0;
            var isValid = violatedPrinciples.Count == 0;

            var result = new ConstitutionalValidationResult
            {
                IsValid = isValid,
                Score = overallScore,
                ViolatedPrinciples = violatedPrinciples,
                Details = GenerateValidationDetails(content, principles, violatedPrinciples, overallScore),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Constitutional AI validation completed. Valid: {IsValid}, Score: {Score:F2}, Violations: {ViolationCount}",
                isValid, overallScore, violatedPrinciples.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Constitutional AI validation");
            throw;
        }
    }

    public async Task<string> ApplyConstitutionalFeedbackAsync(string content, List<string> principles)
    {
        try
        {
            _logger.LogDebug("Applying Constitutional AI feedback to content");

            var validation = await ValidateAsync(content, principles);
            
            if (validation.IsValid)
            {
                _logger.LogDebug("Content already meets Constitutional AI standards");
                return content;
            }

            var improvedContent = content;
            
            // Apply corrections for each violated principle
            foreach (var violatedPrinciple in validation.ViolatedPrinciples)
            {
                improvedContent = await ApplyPrincipleCorrection(improvedContent, violatedPrinciple);
            }

            // Validate the improved content
            var revalidation = await ValidateAsync(improvedContent, principles);
            
            if (revalidation.IsValid)
            {
                _logger.LogInformation("Successfully applied Constitutional AI feedback");
                return improvedContent;
            }
            else
            {
                _logger.LogWarning("Constitutional AI feedback application incomplete. Remaining violations: {Count}",
                    revalidation.ViolatedPrinciples.Count);
                return improvedContent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying Constitutional AI feedback");
            throw;
        }
    }

    public async Task<string> GenerateCritiqueAsync(string content, List<string> principles)
    {
        try
        {
            _logger.LogDebug("Generating Constitutional AI critique");

            var validation = await ValidateAsync(content, principles);
            
            if (validation.IsValid)
            {
                return "The content adheres to all Constitutional AI principles and demonstrates ethical, helpful, and harmless communication.";
            }

            var critique = new List<string>
            {
                "Constitutional AI Analysis:",
                $"Overall Score: {validation.Score:F2}/1.0",
                ""
            };

            if (validation.ViolatedPrinciples.Any())
            {
                critique.Add("Violated Principles:");
                foreach (var principle in validation.ViolatedPrinciples)
                {
                    critique.Add($"- {principle}");
                    critique.Add($"  Issue: {await GetPrincipleViolationDescription(content, principle)}");
                }
                critique.Add("");
            }

            critique.Add("Recommendations:");
            foreach (var principle in validation.ViolatedPrinciples)
            {
                critique.Add($"- {await GetPrincipleRecommendation(principle)}");
            }

            return string.Join("\n", critique);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Constitutional AI critique");
            throw;
        }
    }

    public async Task<bool> ViolatesPrincipleAsync(string content, string principle)
    {
        try
        {
            // Check against pattern-based rules first
            if (_principlePatterns.TryGetValue(principle.ToLowerInvariant(), out var pattern))
            {
                if (pattern.IsMatch(content))
                {
                    return true;
                }
            }

            // Apply specific validation logic based on principle
            return principle.ToLowerInvariant() switch
            {
                var p when p.Contains("helpful") => await CheckHelpfulnessViolation(content),
                var p when p.Contains("harmless") || p.Contains("harmful") => await CheckHarmfulnessViolation(content),
                var p when p.Contains("honest") => await CheckHonestyViolation(content),
                var p when p.Contains("respect") && p.Contains("autonomy") => await CheckAutonomyViolation(content),
                var p when p.Contains("transparent") => await CheckTransparencyViolation(content),
                var p when p.Contains("illegal") => await CheckIllegalContentViolation(content),
                var p when p.Contains("ethical") || p.Contains("unethical") => await CheckEthicalViolation(content),
                _ => await CheckGenericPrincipleViolation(content, principle)
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking principle violation for: {Principle}", principle);
            return false;
        }
    }

    public async Task<double> GetConstitutionalScoreAsync(string content, List<string> principles)
    {
        try
        {
            var scores = new List<double>();

            foreach (var principle in principles)
            {
                var score = await GetPrincipleScoreAsync(content, principle);
                scores.Add(score);
            }

            return scores.Any() ? scores.Average() : 1.0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating Constitutional AI score");
            return 0.0;
        }
    }

    private async Task<double> GetPrincipleScoreAsync(string content, string principle)
    {
        try
        {
            // Base score
            double score = 1.0;

            // Check for violations and adjust score
            var violates = await ViolatesPrincipleAsync(content, principle);
            if (violates)
            {
                score = 0.3; // Significant reduction for violations
            }

            // Apply principle-specific scoring
            score = principle.ToLowerInvariant() switch
            {
                var p when p.Contains("helpful") => await ScoreHelpfulness(content, score),
                var p when p.Contains("harmless") => await ScoreHarmlessness(content, score),
                var p when p.Contains("honest") => await ScoreHonesty(content, score),
                var p when p.Contains("respect") => await ScoreRespectfulness(content, score),
                var p when p.Contains("transparent") => await ScoreTransparency(content, score),
                _ => score
            };

            return Math.Max(0.0, Math.Min(1.0, score));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error scoring principle: {Principle}", principle);
            return 0.5; // Default neutral score
        }
    }

    private static Dictionary<string, Regex> InitializePrinciplePatterns()
    {
        return new Dictionary<string, Regex>
        {
            // Harmful content patterns
            ["harmful"] = new Regex(@"\b(violence|harm|hurt|damage|kill|destroy|attack)\b", RegexOptions.IgnoreCase),
            ["illegal"] = new Regex(@"\b(illegal|unlawful|criminal|fraud|steal|hack|piracy)\b", RegexOptions.IgnoreCase),
            ["unethical"] = new Regex(@"\b(unethical|immoral|wrong|corrupt|manipulate|deceive)\b", RegexOptions.IgnoreCase),
            
            // Dishonesty patterns
            ["honest"] = new Regex(@"\b(lie|lying|false|fake|deceive|mislead|misinform)\b", RegexOptions.IgnoreCase),
            
            // Respect patterns
            ["respect"] = new Regex(@"\b(disrespect|insult|demean|belittle|discriminate|prejudice)\b", RegexOptions.IgnoreCase),
            
            // Transparency patterns
            ["transparent"] = new Regex(@"\b(hide|conceal|secret|unclear|ambiguous|misleading)\b", RegexOptions.IgnoreCase)
        };
    }

    private async Task<bool> CheckHelpfulnessViolation(string content)
    {
        await Task.CompletedTask;
        
        // Check for unhelpful patterns
        var unhelpfulPatterns = new[]
        {
            @"\b(can't help|unable to assist|not my problem|figure it out yourself)\b",
            @"\b(don't know|no idea|not sure|maybe|possibly)\b.*\?$" // Excessive uncertainty
        };

        return unhelpfulPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckHarmfulnessViolation(string content)
    {
        await Task.CompletedTask;
        
        var harmfulPatterns = new[]
        {
            @"\b(violence|aggressive|threatening|intimidating)\b",
            @"\b(self-harm|suicide|depression) (?!support|help|prevention)\b",
            @"\b(hate|discrimination|prejudice|bias)\b",
            @"\b(illegal|criminal|unlawful|fraud)\b"
        };

        return harmfulPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckHonestyViolation(string content)
    {
        await Task.CompletedTask;
        
        var dishonestyPatterns = new[]
        {
            @"\b(guarantee|promise|100% certain|absolutely sure)\b.*\b(will|must|definitely)\b",
            @"\b(everyone knows|it's obvious|clearly|undoubtedly)\b.*\b(wrong|false|fake)\b",
            @"\b(trust me|believe me|take my word)\b"
        };

        return dishonestyPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckAutonomyViolation(string content)
    {
        await Task.CompletedTask;
        
        var autonomyPatterns = new[]
        {
            @"\b(you must|you have to|you should|you need to)\b.*\b(do|think|believe|accept)\b",
            @"\b(submit|obey|comply|conform)\b",
            @"\b(don't think|don't question|don't doubt)\b"
        };

        return autonomyPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckTransparencyViolation(string content)
    {
        await Task.CompletedTask;
        
        var transparencyPatterns = new[]
        {
            @"\b(secret|hidden|concealed|undisclosed)\b",
            @"\b(between us|don't tell|keep quiet|confidential)\b",
            @"\b(trust me|just because|no reason|because I said so)\b"
        };

        return transparencyPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckIllegalContentViolation(string content)
    {
        await Task.CompletedTask;
        
        var illegalPatterns = new[]
        {
            @"\b(how to (?:steal|hack|break into|bypass))\b",
            @"\b(illegal (?:download|copy|distribute))\b",
            @"\b(criminal (?:activity|behavior|plan))\b",
            @"\b(fraud|scam|scheme)\b.*\b(money|profit|gain)\b"
        };

        return illegalPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckEthicalViolation(string content)
    {
        await Task.CompletedTask;
        
        var unethicalPatterns = new[]
        {
            @"\b(manipulate|exploit|take advantage)\b.*\b(people|person|others)\b",
            @"\b(corrupt|bribe|blackmail)\b",
            @"\b(discriminate|prejudice|bias)\b.*\b(against|toward)\b"
        };

        return unethicalPatterns.Any(pattern => 
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private async Task<bool> CheckGenericPrincipleViolation(string content, string principle)
    {
        await Task.CompletedTask;
        
        // Generic check for principle violations
        var principleWords = principle.ToLowerInvariant().Split(' ');
        var negativeWords = new[] { "not", "don't", "avoid", "prevent", "harmful", "illegal", "unethical" };
        
        // If principle contains negative words, check for positive occurrences of the opposite
        if (principleWords.Any(word => negativeWords.Contains(word)))
        {
            var positiveWords = principleWords.Except(negativeWords);
            return positiveWords.Any(word => 
                Regex.IsMatch(content, $@"\b{word}\b", RegexOptions.IgnoreCase));
        }

        return false;
    }

    private async Task<double> ScoreHelpfulness(string content, double baseScore)
    {
        await Task.CompletedTask;
        
        // Positive indicators
        var helpfulPatterns = new[]
        {
            @"\b(helpful|assist|support|guide|explain|clarify)\b",
            @"\b(here's how|let me help|I can help|happy to assist)\b",
            @"\b(solution|answer|suggestion|recommendation)\b"
        };

        var helpfulMatches = helpfulPatterns.Sum(pattern => 
            Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count);

        return Math.Min(1.0, baseScore + (helpfulMatches * 0.1));
    }

    private async Task<double> ScoreHarmlessness(string content, double baseScore)
    {
        await Task.CompletedTask;
        
        // Check for safety indicators
        var safetyPatterns = new[]
        {
            @"\b(safe|secure|protect|careful|responsible)\b",
            @"\b(avoid|prevent|stay away from)\b.*\b(harm|danger|risk)\b"
        };

        var safetyMatches = safetyPatterns.Sum(pattern => 
            Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count);

        return Math.Min(1.0, baseScore + (safetyMatches * 0.1));
    }

    private async Task<double> ScoreHonesty(string content, double baseScore)
    {
        await Task.CompletedTask;
        
        // Check for honesty indicators
        var honestyPatterns = new[]
        {
            @"\b(honestly|truthfully|to be honest|I believe|in my opinion)\b",
            @"\b(I'm not sure|I don't know|uncertain|might be wrong)\b",
            @"\b(according to|based on|evidence suggests)\b"
        };

        var honestyMatches = honestyPatterns.Sum(pattern => 
            Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count);

        return Math.Min(1.0, baseScore + (honestyMatches * 0.1));
    }

    private async Task<double> ScoreRespectfulness(string content, double baseScore)
    {
        await Task.CompletedTask;
        
        // Check for respectful language
        var respectPatterns = new[]
        {
            @"\b(please|thank you|respect|appreciate|understand)\b",
            @"\b(you're right|good point|I see your perspective)\b",
            @"\b(may I|could you|would you mind)\b"
        };

        var respectMatches = respectPatterns.Sum(pattern => 
            Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count);

        return Math.Min(1.0, baseScore + (respectMatches * 0.1));
    }

    private async Task<double> ScoreTransparency(string content, double baseScore)
    {
        await Task.CompletedTask;
        
        // Check for transparency indicators
        var transparencyPatterns = new[]
        {
            @"\b(because|reason|explanation|clarification)\b",
            @"\b(let me explain|here's why|the reason is)\b",
            @"\b(transparent|open|clear|honest about)\b"
        };

        var transparencyMatches = transparencyPatterns.Sum(pattern => 
            Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count);

        return Math.Min(1.0, baseScore + (transparencyMatches * 0.1));
    }

    private async Task<string> ApplyPrincipleCorrection(string content, string violatedPrinciple)
    {
        await Task.CompletedTask;
        
        // Apply corrections based on the violated principle
        return violatedPrinciple.ToLowerInvariant() switch
        {
            var p when p.Contains("helpful") => MakeMoreHelpful(content),
            var p when p.Contains("harmless") => MakeMoreHarmless(content),
            var p when p.Contains("honest") => MakeMoreHonest(content),
            var p when p.Contains("respect") => MakeMoreRespectful(content),
            var p when p.Contains("transparent") => MakeMoreTransparent(content),
            _ => content
        };
    }

    private static string MakeMoreHelpful(string content)
    {
        // Add helpful framing
        if (!content.Contains("help") && !content.Contains("assist"))
        {
            return $"I'd be happy to help with this. {content}";
        }
        return content;
    }

    private static string MakeMoreHarmless(string content)
    {
        // Add safety disclaimers
        return $"{content}\n\nPlease note: Always prioritize safety and follow legal guidelines in any actions you take.";
    }

    private static string MakeMoreHonest(string content)
    {
        // Add uncertainty qualifiers where appropriate
        content = Regex.Replace(content, @"\b(will definitely|absolutely will|guaranteed to)\b", 
            "may", RegexOptions.IgnoreCase);
        return content;
    }

    private static string MakeMoreRespectful(string content)
    {
        // Add respectful language
        content = Regex.Replace(content, @"\b(you must|you have to)\b", 
            "you might consider", RegexOptions.IgnoreCase);
        return content;
    }

    private static string MakeMoreTransparent(string content)
    {
        // Add reasoning where missing
        if (!content.Contains("because") && !content.Contains("reason"))
        {
            return $"{content}\n\nThe reason for this approach is to ensure clarity and transparency in the process.";
        }
        return content;
    }

    private static string GenerateValidationDetails(string content, List<string> principles, List<string> violations, double score)
    {
        var details = new List<string>
        {
            $"Content length: {content.Length} characters",
            $"Principles evaluated: {principles.Count}",
            $"Violations found: {violations.Count}",
            $"Overall score: {score:F2}/1.0"
        };

        if (violations.Any())
        {
            details.Add("Violated principles:");
            details.AddRange(violations.Select(v => $"- {v}"));
        }

        return string.Join("\n", details);
    }

    private async Task<string> GetPrincipleViolationDescription(string content, string principle)
    {
        await Task.CompletedTask;
        
        return principle.ToLowerInvariant() switch
        {
            var p when p.Contains("helpful") => "Content may not be sufficiently helpful or supportive",
            var p when p.Contains("harmless") => "Content may contain harmful or dangerous elements",
            var p when p.Contains("honest") => "Content may contain misleading or overly certain claims",
            var p when p.Contains("respect") => "Content may not respect human autonomy or dignity",
            var p when p.Contains("transparent") => "Content lacks transparency or clear reasoning",
            _ => "Content may violate this principle"
        };
    }

    private async Task<string> GetPrincipleRecommendation(string principle)
    {
        await Task.CompletedTask;
        
        return principle.ToLowerInvariant() switch
        {
            var p when p.Contains("helpful") => "Ensure content is constructive and provides value to the user",
            var p when p.Contains("harmless") => "Remove any potentially harmful content and add safety considerations",
            var p when p.Contains("honest") => "Use appropriate uncertainty language and avoid overconfident claims",
            var p when p.Contains("respect") => "Respect user autonomy and avoid commanding language",
            var p when p.Contains("transparent") => "Provide clear reasoning and explanations for recommendations",
            _ => "Review content to ensure alignment with this principle"
        };
    }
}