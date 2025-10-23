using System.Text.RegularExpressions;
using Module_03_Content_Safety.Models;

namespace Module_03_Content_Safety.Services;

public interface IPromptInjectionService
{
    Task<PromptInjectionResult> DetectPromptInjectionAsync(string prompt);
    Task<PromptInjectionResult> DetectPromptInjectionAsync(string prompt, string? systemPrompt, string? context);
    Task<bool> IsPromptSafeAsync(string prompt, double threshold = 5.0);
    Task<string> SanitizePromptAsync(string prompt);
}

public class PromptInjectionService : IPromptInjectionService
{
    private readonly ILogger<PromptInjectionService> _logger;
    private readonly List<InjectionPatternDefinition> _injectionPatterns;
    private readonly List<string> _jailbreakIndicators;
    private readonly List<string> _blockedKeywords;

    public PromptInjectionService(ILogger<PromptInjectionService> logger)
    {
        _logger = logger;
        _injectionPatterns = InitializeInjectionPatterns();
        _jailbreakIndicators = InitializeJailbreakIndicators();
        _blockedKeywords = InitializeBlockedKeywords();
    }

    public async Task<PromptInjectionResult> DetectPromptInjectionAsync(string prompt)
    {
        return await DetectPromptInjectionAsync(prompt, null, null);
    }

    public async Task<PromptInjectionResult> DetectPromptInjectionAsync(string prompt, string? systemPrompt, string? context)
    {
        _logger.LogInformation("Detecting prompt injection. Prompt length: {Length}", prompt.Length);

        var result = new PromptInjectionResult
        {
            DetectedPatterns = new List<InjectionPattern>(),
            BlockedKeywords = new List<string>()
        };

        double riskScore = 0.0;
        var promptLower = prompt.ToLowerInvariant();

        // Check for direct injection patterns
        foreach (var patternDef in _injectionPatterns)
        {
            var matches = patternDef.Pattern.Matches(prompt);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    result.DetectedPatterns.Add(new InjectionPattern
                    {
                        Pattern = match.Value,
                        Type = patternDef.Type,
                        Confidence = patternDef.Confidence,
                        Description = patternDef.Description,
                        Position = match.Index
                    });

                    riskScore += patternDef.RiskScore;
                }
            }
        }

        // Check for jailbreak attempts
        foreach (var indicator in _jailbreakIndicators)
        {
            if (promptLower.Contains(indicator.ToLowerInvariant()))
            {
                result.DetectedPatterns.Add(new InjectionPattern
                {
                    Pattern = indicator,
                    Type = InjectionType.Jailbreak,
                    Confidence = 0.8,
                    Description = "Jailbreak attempt detected",
                    Position = promptLower.IndexOf(indicator.ToLowerInvariant())
                });
                riskScore += 4.0;
            }
        }

        // Check for blocked keywords
        foreach (var keyword in _blockedKeywords)
        {
            if (promptLower.Contains(keyword.ToLowerInvariant()))
            {
                result.BlockedKeywords.Add(keyword);
                riskScore += 2.0;
            }
        }

        // Check for system prompt manipulation
        if (systemPrompt != null && DetectSystemPromptManipulation(prompt, systemPrompt))
        {
            result.DetectedPatterns.Add(new InjectionPattern
            {
                Pattern = "System prompt manipulation",
                Type = InjectionType.SystemPromptManipulation,
                Confidence = 0.85,
                Description = "Attempt to override or manipulate system prompt detected"
            });
            riskScore += 5.0;
        }

        // Check for role manipulation
        if (DetectRoleManipulation(prompt))
        {
            result.DetectedPatterns.Add(new InjectionPattern
            {
                Pattern = "Role manipulation",
                Type = InjectionType.RoleManipulation,
                Confidence = 0.8,
                Description = "Attempt to change AI role or behavior detected"
            });
            riskScore += 4.0;
        }

        // Check for context escape attempts
        if (DetectContextEscape(prompt))
        {
            result.DetectedPatterns.Add(new InjectionPattern
            {
                Pattern = "Context escape",
                Type = InjectionType.ContextEscape,
                Confidence = 0.75,
                Description = "Attempt to escape conversation context detected"
            });
            riskScore += 3.5;
        }

        // Check for payload injection (encoded or obfuscated)
        if (DetectPayloadInjection(prompt))
        {
            result.DetectedPatterns.Add(new InjectionPattern
            {
                Pattern = "Payload injection",
                Type = InjectionType.PayloadInjection,
                Confidence = 0.7,
                Description = "Obfuscated or encoded injection payload detected"
            });
            riskScore += 3.0;
        }

        // Check for chained injection (multiple techniques)
        if (result.DetectedPatterns.Select(p => p.Type).Distinct().Count() >= 3)
        {
            result.DetectedPatterns.Add(new InjectionPattern
            {
                Pattern = "Chained injection",
                Type = InjectionType.ChainedInjection,
                Confidence = 0.9,
                Description = "Multiple injection techniques combined"
            });
            riskScore += 2.0;
        }

        result.RiskScore = Math.Min(riskScore, 10.0);
        result.IsInjectionDetected = result.RiskScore >= 5.0 || result.DetectedPatterns.Any();
        result.InjectionType = DetermineInjectionType(result.DetectedPatterns);

        // Generate explanation
        if (result.IsInjectionDetected)
        {
            result.Explanation = GenerateExplanation(result);
            result.SafeAlternative = await GenerateSafeAlternativeAsync(prompt);
        }

        _logger.LogInformation("Prompt injection detection complete. Is injection: {IsInjection}, Risk: {Risk:F2}", 
            result.IsInjectionDetected, result.RiskScore);

        return await Task.FromResult(result);
    }

    public async Task<bool> IsPromptSafeAsync(string prompt, double threshold = 5.0)
    {
        var result = await DetectPromptInjectionAsync(prompt);
        return result.RiskScore < threshold;
    }

    public async Task<string> SanitizePromptAsync(string prompt)
    {
        var sanitized = prompt;

        // Remove instruction override patterns
        sanitized = Regex.Replace(sanitized, 
            @"(ignore|disregard|forget)\s+(previous|all|above)\s+(instructions?|prompts?|directions?)", 
            "[REMOVED]", RegexOptions.IgnoreCase);

        // Remove system prompt manipulation
        sanitized = Regex.Replace(sanitized, 
            @"\[?(SYSTEM|INST|/INST)\]?:?\s*", 
            "", RegexOptions.IgnoreCase);

        // Remove role manipulation
        sanitized = Regex.Replace(sanitized, 
            @"(act|pretend|roleplay)\s+as\s+", 
            "", RegexOptions.IgnoreCase);

        // Remove special tokens
        sanitized = Regex.Replace(sanitized, 
            @"<\|[^|]+\|>", 
            "");

        return await Task.FromResult(sanitized);
    }

    private List<InjectionPatternDefinition> InitializeInjectionPatterns()
    {
        return new List<InjectionPatternDefinition>
        {
            // Instruction override patterns
            new(
                new Regex(@"ignore\s+(previous|all|above)\s+(instructions?|prompts?|directions?)", RegexOptions.IgnoreCase),
                InjectionType.DirectInjection,
                "Instruction override attempt",
                0.9,
                5.0
            ),
            new(
                new Regex(@"disregard\s+(previous|all|above|everything)", RegexOptions.IgnoreCase),
                InjectionType.DirectInjection,
                "Disregard previous instructions",
                0.85,
                4.5
            ),
            new(
                new Regex(@"forget\s+(everything|previous|all)\s+(instructions?|context)?", RegexOptions.IgnoreCase),
                InjectionType.DirectInjection,
                "Forget previous context",
                0.85,
                4.5
            ),

            // New instruction injection
            new(
                new Regex(@"new\s+(instructions?|prompt|task)s?:\s*", RegexOptions.IgnoreCase),
                InjectionType.DirectInjection,
                "New instruction injection",
                0.9,
                5.0
            ),
            new(
                new Regex(@"instead,?\s+(do|perform|execute|run)\s+", RegexOptions.IgnoreCase),
                InjectionType.DirectInjection,
                "Instruction replacement",
                0.8,
                4.0
            ),

            // System prompt manipulation
            new(
                new Regex(@"\[?SYSTEM\]?:?\s*(you\s+are|your\s+role)", RegexOptions.IgnoreCase),
                InjectionType.SystemPromptManipulation,
                "System role redefinition",
                0.9,
                5.5
            ),
            new(
                new Regex(@"\[?(INST|/INST)\]", RegexOptions.IgnoreCase),
                InjectionType.SystemPromptManipulation,
                "Instruction tag manipulation",
                0.85,
                4.5
            ),

            // Role manipulation
            new(
                new Regex(@"(act|pretend|roleplay)\s+as\s+(if|though)?\s*", RegexOptions.IgnoreCase),
                InjectionType.RoleManipulation,
                "Role change attempt",
                0.8,
                4.0
            ),
            new(
                new Regex(@"you\s+are\s+(now|a|an)\s+", RegexOptions.IgnoreCase),
                InjectionType.RoleManipulation,
                "Identity override",
                0.75,
                3.5
            ),

            // Special tokens
            new(
                new Regex(@"<\|.*?\|>"),
                InjectionType.SystemPromptManipulation,
                "Special token usage",
                0.85,
                4.5
            ),

            // Jailbreak patterns
            new(
                new Regex(@"\b(jailbreak|DAN|developer\s+mode|evil\s+mode)\b", RegexOptions.IgnoreCase),
                InjectionType.Jailbreak,
                "Jailbreak keyword",
                0.9,
                5.0
            ),
            new(
                new Regex(@"(bypass|override|disable)\s+(filter|restriction|safety|rules)", RegexOptions.IgnoreCase),
                InjectionType.Jailbreak,
                "Safety bypass attempt",
                0.9,
                5.0
            ),

            // Context escape
            new(
                new Regex(@"(end|exit|quit|stop)\s+(conversation|chat|session)", RegexOptions.IgnoreCase),
                InjectionType.ContextEscape,
                "Context exit attempt",
                0.7,
                3.0
            ),
            new(
                new Regex(@"###\s*new\s+(conversation|context|session)", RegexOptions.IgnoreCase),
                InjectionType.ContextEscape,
                "Context reset attempt",
                0.75,
                3.5
            ),

            // Encoding/obfuscation
            new(
                new Regex(@"(?:&#\d+;|%[0-9a-fA-F]{2}|\\u[0-9a-fA-F]{4}){10,}"),
                InjectionType.PayloadInjection,
                "Encoded payload detected",
                0.7,
                3.0
            ),
            new(
                new Regex(@"eval\s*\(|exec\s*\(|Function\s*\(", RegexOptions.IgnoreCase),
                InjectionType.PayloadInjection,
                "Code execution attempt",
                0.9,
                5.0
            )
        };
    }

    private List<string> InitializeJailbreakIndicators()
    {
        return new List<string>
        {
            "DAN mode",
            "Developer mode",
            "Evil mode",
            "Unrestricted mode",
            "Unfiltered mode",
            "No rules",
            "No restrictions",
            "Do anything now",
            "Ignore ethics",
            "Ignore morality",
            "Without restrictions",
            "Unlimited capabilities",
            "Break free",
            "Escape constraints"
        };
    }

    private List<string> InitializeBlockedKeywords()
    {
        return new List<string>
        {
            "system:",
            "[SYSTEM]",
            "[INST]",
            "[/INST]",
            "<|endoftext|>",
            "<|im_start|>",
            "<|im_end|>"
        };
    }

    private bool DetectSystemPromptManipulation(string prompt, string systemPrompt)
    {
        var promptLower = prompt.ToLowerInvariant();
        
        // Check if user is trying to reveal or modify system prompt
        var manipulationPatterns = new[]
        {
            "what is your system prompt",
            "what are your instructions",
            "reveal your prompt",
            "show me your system message",
            "what were you told",
            "your system prompt is",
            "modify your system prompt"
        };

        return manipulationPatterns.Any(pattern => promptLower.Contains(pattern));
    }

    private bool DetectRoleManipulation(string prompt)
    {
        var rolePatterns = new[]
        {
            @"you\s+are\s+(now|a|an)\s+(?!assistant|ai|helpful)",
            @"(act|pretend|behave)\s+as\s+",
            @"your\s+new\s+role\s+is",
            @"from\s+now\s+on,?\s+you\s+(are|will\s+be)",
            @"transform\s+into"
        };

        return rolePatterns.Any(pattern => 
            Regex.IsMatch(prompt, pattern, RegexOptions.IgnoreCase));
    }

    private bool DetectContextEscape(string prompt)
    {
        var escapePatterns = new[]
        {
            @"(end|stop|exit|quit)\s+(conversation|context|chat)",
            @"###\s*new\s+(conversation|context)",
            @"start\s+over",
            @"reset\s+(conversation|context|everything)",
            @"clear\s+(history|context|memory)"
        };

        return escapePatterns.Any(pattern => 
            Regex.IsMatch(prompt, pattern, RegexOptions.IgnoreCase));
    }

    private bool DetectPayloadInjection(string prompt)
    {
        // Check for heavy encoding
        var encodedCount = Regex.Matches(prompt, @"(?:&#\d+;|%[0-9a-fA-F]{2}|\\u[0-9a-fA-F]{4})").Count;
        if (encodedCount > 10) return true;

        // Check for code execution patterns
        var codePatterns = new[]
        {
            @"eval\s*\(",
            @"exec\s*\(",
            @"Function\s*\(",
            @"setTimeout\s*\(",
            @"setInterval\s*\("
        };

        return codePatterns.Any(pattern => 
            Regex.IsMatch(prompt, pattern, RegexOptions.IgnoreCase));
    }

    private InjectionType DetermineInjectionType(List<InjectionPattern> patterns)
    {
        if (!patterns.Any()) return InjectionType.None;

        // Return the highest severity type
        var typeOrder = new[]
        {
            InjectionType.ChainedInjection,
            InjectionType.SystemPromptManipulation,
            InjectionType.Jailbreak,
            InjectionType.DirectInjection,
            InjectionType.PayloadInjection,
            InjectionType.RoleManipulation,
            InjectionType.ContextEscape,
            InjectionType.IndirectInjection
        };

        foreach (var type in typeOrder)
        {
            if (patterns.Any(p => p.Type == type))
                return type;
        }

        return InjectionType.DirectInjection;
    }

    private string GenerateExplanation(PromptInjectionResult result)
    {
        var explanation = $"Detected {result.InjectionType} with risk score {result.RiskScore:F1}/10. ";
        
        if (result.DetectedPatterns.Any())
        {
            var topPatterns = result.DetectedPatterns
                .OrderByDescending(p => p.Confidence)
                .Take(3)
                .Select(p => p.Description);
            explanation += $"Patterns found: {string.Join(", ", topPatterns)}.";
        }

        if (result.BlockedKeywords.Any())
        {
            explanation += $" Blocked keywords: {string.Join(", ", result.BlockedKeywords)}.";
        }

        return explanation;
    }

    private async Task<string> GenerateSafeAlternativeAsync(string prompt)
    {
        // Remove injection patterns and return sanitized version
        var safe = await SanitizePromptAsync(prompt);
        
        // If heavily modified, suggest complete rewrite
        if (safe.Length < prompt.Length * 0.5)
        {
            return "Please rephrase your request without attempting to modify system behavior or instructions.";
        }

        return safe;
    }

    private record InjectionPatternDefinition(
        Regex Pattern,
        InjectionType Type,
        string Description,
        double Confidence,
        double RiskScore
    );
}
