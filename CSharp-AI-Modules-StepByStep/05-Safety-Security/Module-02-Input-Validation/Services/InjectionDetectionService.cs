using Module_02_Input_Validation.Models;
using System.Text.RegularExpressions;

namespace Module_02_Input_Validation.Services;

public interface IInjectionDetectionService
{
    InjectionDetectionResult DetectSqlInjection(string input);
    InjectionDetectionResult DetectXss(string input);
    InjectionDetectionResult DetectCommandInjection(string input);
    InjectionDetectionResult DetectLdapInjection(string input);
    InjectionDetectionResult DetectPathTraversal(string input);
    InjectionDetectionResult DetectAllInjections(string input);
    double CalculateRiskScore(string input);
}

public class InjectionDetectionService : IInjectionDetectionService
{
    private readonly ILogger<InjectionDetectionService> _logger;
    private readonly ISanitizationService _sanitizationService;

    // SQL Injection patterns
    private static readonly List<string> SqlInjectionPatterns = new()
    {
        @"\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|DECLARE)\b",
        @"(--|;|\/\*|\*\/)",
        @"(\bOR\b|\bAND\b).*=.*",
        @"(xp_|sp_)\w+",
        @"('\s*(OR|AND)\s*'?\d+'?\s*=\s*'?\d+'?)",
        @"(UNION\s+SELECT)",
        @"(CAST\s*\()",
        @"(CONVERT\s*\()",
        @"(EXEC\s*\()",
        @"(HAVING\s+\d+\s*=\s*\d+)"
    };

    // XSS patterns
    private static readonly List<string> XssPatterns = new()
    {
        @"<script[^>]*>.*?</script>",
        @"javascript:",
        @"on\w+\s*=",
        @"<iframe",
        @"<object",
        @"<embed",
        @"<applet",
        @"<meta",
        @"<link",
        @"<style[^>]*>.*?</style>",
        @"vbscript:",
        @"data:text/html",
        @"<img[^>]+src\s*=\s*[""']?javascript:",
        @"<svg[^>]*>.*?</svg>",
        @"onerror\s*=",
        @"onload\s*="
    };

    // Command Injection patterns
    private static readonly List<string> CommandInjectionPatterns = new()
    {
        @"[;&|]\s*(ls|cat|rm|mv|cp|wget|curl|nc|bash|sh|cmd|powershell)",
        @"(`|$\()",
        @"\$\{.*\}",
        @">\s*/dev/null",
        @"2>&1",
        @"\|\s*tee"
    };

    // LDAP Injection patterns
    private static readonly List<string> LdapInjectionPatterns = new()
    {
        @"\*\)",
        @"\(\|",
        @"\(&",
        @"[*()\\]"
    };

    // Path Traversal patterns
    private static readonly List<string> PathTraversalPatterns = new()
    {
        @"\.\./",
        @"\.\.\\",
        @"%2e%2e/",
        @"%2e%2e\\",
        @"\.\.%2f",
        @"\.\.%5c"
    };

    public InjectionDetectionService(
        ILogger<InjectionDetectionService> logger,
        ISanitizationService sanitizationService)
    {
        _logger = logger;
        _sanitizationService = sanitizationService;
    }

    public InjectionDetectionResult DetectSqlInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var detectedPatterns = new List<string>();

        foreach (var pattern in SqlInjectionPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                detectedPatterns.Add(pattern);
            }
        }

        var isSuspicious = detectedPatterns.Count > 0;
        var riskScore = CalculateSqlInjectionRisk(input, detectedPatterns.Count);

        if (isSuspicious)
        {
            _logger.LogWarning("SQL Injection detected in input. Patterns: {Patterns}", string.Join(", ", detectedPatterns));
        }

        return new InjectionDetectionResult
        {
            IsSuspicious = isSuspicious,
            Type = isSuspicious ? InjectionType.SqlInjection : InjectionType.None,
            DetectedPatterns = detectedPatterns,
            RiskScore = riskScore,
            Message = isSuspicious ? "Potential SQL injection detected" : "No SQL injection detected",
            SanitizedInput = isSuspicious ? _sanitizationService.SanitizeForSql(input) : input
        };
    }

    public InjectionDetectionResult DetectXss(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var detectedPatterns = new List<string>();

        foreach (var pattern in XssPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                detectedPatterns.Add(pattern);
            }
        }

        var isSuspicious = detectedPatterns.Count > 0;
        var riskScore = CalculateXssRisk(input, detectedPatterns.Count);

        if (isSuspicious)
        {
            _logger.LogWarning("XSS attack detected in input. Patterns: {Patterns}", string.Join(", ", detectedPatterns));
        }

        return new InjectionDetectionResult
        {
            IsSuspicious = isSuspicious,
            Type = isSuspicious ? InjectionType.XssAttack : InjectionType.None,
            DetectedPatterns = detectedPatterns,
            RiskScore = riskScore,
            Message = isSuspicious ? "Potential XSS attack detected" : "No XSS attack detected",
            SanitizedInput = isSuspicious ? _sanitizationService.EncodeHtml(input) : input
        };
    }

    public InjectionDetectionResult DetectCommandInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var detectedPatterns = new List<string>();

        foreach (var pattern in CommandInjectionPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                detectedPatterns.Add(pattern);
            }
        }

        var isSuspicious = detectedPatterns.Count > 0;
        var riskScore = CalculateCommandInjectionRisk(input, detectedPatterns.Count);

        if (isSuspicious)
        {
            _logger.LogWarning("Command injection detected in input. Patterns: {Patterns}", string.Join(", ", detectedPatterns));
        }

        return new InjectionDetectionResult
        {
            IsSuspicious = isSuspicious,
            Type = isSuspicious ? InjectionType.CommandInjection : InjectionType.None,
            DetectedPatterns = detectedPatterns,
            RiskScore = riskScore,
            Message = isSuspicious ? "Potential command injection detected" : "No command injection detected",
            SanitizedInput = isSuspicious ? RemoveCommandInjectionPatterns(input) : input
        };
    }

    public InjectionDetectionResult DetectLdapInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var detectedPatterns = new List<string>();

        foreach (var pattern in LdapInjectionPatterns)
        {
            if (Regex.IsMatch(input, pattern))
            {
                detectedPatterns.Add(pattern);
            }
        }

        var isSuspicious = detectedPatterns.Count > 0;
        var riskScore = detectedPatterns.Count * 2.5;

        if (isSuspicious)
        {
            _logger.LogWarning("LDAP injection detected in input");
        }

        return new InjectionDetectionResult
        {
            IsSuspicious = isSuspicious,
            Type = isSuspicious ? InjectionType.LdapInjection : InjectionType.None,
            DetectedPatterns = detectedPatterns,
            RiskScore = riskScore,
            Message = isSuspicious ? "Potential LDAP injection detected" : "No LDAP injection detected",
            SanitizedInput = isSuspicious ? Regex.Replace(input, @"[*()\\]", "") : input
        };
    }

    public InjectionDetectionResult DetectPathTraversal(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var detectedPatterns = new List<string>();

        foreach (var pattern in PathTraversalPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                detectedPatterns.Add(pattern);
            }
        }

        var isSuspicious = detectedPatterns.Count > 0;
        var riskScore = detectedPatterns.Count * 3.0;

        if (isSuspicious)
        {
            _logger.LogWarning("Path traversal detected in input");
        }

        return new InjectionDetectionResult
        {
            IsSuspicious = isSuspicious,
            Type = isSuspicious ? InjectionType.PathTraversal : InjectionType.None,
            DetectedPatterns = detectedPatterns,
            RiskScore = riskScore,
            Message = isSuspicious ? "Potential path traversal detected" : "No path traversal detected",
            SanitizedInput = isSuspicious ? _sanitizationService.SanitizeFileName(input) : input
        };
    }

    public InjectionDetectionResult DetectAllInjections(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return CreateSafeResult(input);
        }

        var results = new[]
        {
            DetectSqlInjection(input),
            DetectXss(input),
            DetectCommandInjection(input),
            DetectLdapInjection(input),
            DetectPathTraversal(input)
        };

        var suspiciousResults = results.Where(r => r.IsSuspicious).ToList();

        if (!suspiciousResults.Any())
        {
            return CreateSafeResult(input);
        }

        var allPatterns = suspiciousResults.SelectMany(r => r.DetectedPatterns).ToList();
        var maxRiskScore = suspiciousResults.Max(r => r.RiskScore);
        var primaryType = suspiciousResults.OrderByDescending(r => r.RiskScore).First().Type;

        _logger.LogWarning("Multiple injection types detected. Primary type: {Type}, Risk score: {RiskScore}",
            primaryType, maxRiskScore);

        return new InjectionDetectionResult
        {
            IsSuspicious = true,
            Type = primaryType,
            DetectedPatterns = allPatterns,
            RiskScore = maxRiskScore,
            Message = $"Multiple injection patterns detected: {string.Join(", ", suspiciousResults.Select(r => r.Type))}",
            SanitizedInput = _sanitizationService.RemoveHtml(input)
        };
    }

    public double CalculateRiskScore(string input)
    {
        if (string.IsNullOrEmpty(input))
            return 0;

        var result = DetectAllInjections(input);
        return result.RiskScore;
    }

    private InjectionDetectionResult CreateSafeResult(string input)
    {
        return new InjectionDetectionResult
        {
            IsSuspicious = false,
            Type = InjectionType.None,
            DetectedPatterns = new List<string>(),
            RiskScore = 0,
            Message = "Input is safe",
            SanitizedInput = input
        };
    }

    private double CalculateSqlInjectionRisk(string input, int patternCount)
    {
        double baseScore = patternCount * 2.0;

        // Increase score for multiple SQL keywords
        if (Regex.Matches(input, @"\b(SELECT|INSERT|UPDATE|DELETE|DROP)\b", RegexOptions.IgnoreCase).Count > 1)
            baseScore += 3.0;

        // Increase score for comment patterns
        if (input.Contains("--") || input.Contains("/*"))
            baseScore += 2.0;

        // Increase score for UNION attacks
        if (Regex.IsMatch(input, @"UNION.*SELECT", RegexOptions.IgnoreCase))
            baseScore += 4.0;

        return Math.Min(baseScore, 10.0);
    }

    private double CalculateXssRisk(string input, int patternCount)
    {
        double baseScore = patternCount * 2.5;

        // Increase score for script tags
        if (Regex.IsMatch(input, @"<script", RegexOptions.IgnoreCase))
            baseScore += 3.0;

        // Increase score for event handlers
        if (Regex.IsMatch(input, @"on\w+\s*=", RegexOptions.IgnoreCase))
            baseScore += 2.5;

        // Increase score for javascript: protocol
        if (input.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
            baseScore += 3.0;

        return Math.Min(baseScore, 10.0);
    }

    private double CalculateCommandInjectionRisk(string input, int patternCount)
    {
        double baseScore = patternCount * 3.0;

        // Increase score for pipe and semicolon
        if (input.Contains("|") || input.Contains(";"))
            baseScore += 2.0;

        return Math.Min(baseScore, 10.0);
    }

    private string RemoveCommandInjectionPatterns(string input)
    {
        var sanitized = input;

        foreach (var pattern in CommandInjectionPatterns)
        {
            sanitized = Regex.Replace(sanitized, pattern, "", RegexOptions.IgnoreCase);
        }

        return sanitized;
    }
}
