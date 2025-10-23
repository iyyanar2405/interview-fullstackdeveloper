using Module_02_Input_Validation.Models;
using Ganss.Xss;
using System.Text.RegularExpressions;

namespace Module_02_Input_Validation.Services;

public interface IXssProtectionService
{
    XssDetectionResult DetectXss(string input);
    string SanitizeForDisplay(string input);
    string SanitizeForAttribute(string input);
    string SanitizeForJavaScript(string input);
    bool ContainsXss(string input);
    XssDetectionResult AnalyzeHtml(string html);
}

public class XssProtectionService : IXssProtectionService
{
    private readonly ILogger<XssProtectionService> _logger;
    private readonly HtmlSanitizer _htmlSanitizer;

    private static readonly List<(string Pattern, int RiskLevel, string Description)> XssPatterns = new()
    {
        (@"<script[^>]*>", 10, "Script tag"),
        (@"javascript:", 9, "JavaScript protocol"),
        (@"on\w+\s*=", 8, "Event handler"),
        (@"<iframe", 9, "IFrame tag"),
        (@"<object", 8, "Object tag"),
        (@"<embed", 8, "Embed tag"),
        (@"<applet", 8, "Applet tag"),
        (@"vbscript:", 9, "VBScript protocol"),
        (@"data:text/html", 7, "Data URI HTML"),
        (@"<svg[^>]*>.*onload", 8, "SVG with onload"),
        (@"<img[^>]+src\s*=\s*[""']?javascript:", 9, "Image with JavaScript"),
        (@"<meta", 6, "Meta tag"),
        (@"<link", 6, "Link tag"),
        (@"<style[^>]*>.*expression\(", 7, "CSS expression"),
        (@"<base", 7, "Base tag"),
        (@"<form", 5, "Form tag"),
        (@"onerror\s*=", 8, "OnError handler"),
        (@"onload\s*=", 8, "OnLoad handler"),
        (@"onclick\s*=", 7, "OnClick handler"),
        (@"onmouseover\s*=", 7, "OnMouseOver handler")
    };

    public XssProtectionService(ILogger<XssProtectionService> logger)
    {
        _logger = logger;
        _htmlSanitizer = new HtmlSanitizer();
        ConfigureSanitizer();
    }

    private void ConfigureSanitizer()
    {
        // Configure safe tags
        _htmlSanitizer.AllowedTags.Clear();
        var safeTags = new[] { "p", "br", "strong", "em", "u", "a", "ul", "ol", "li", "h1", "h2", "h3" };
        foreach (var tag in safeTags)
        {
            _htmlSanitizer.AllowedTags.Add(tag);
        }

        // Configure safe attributes
        _htmlSanitizer.AllowedAttributes.Clear();
        _htmlSanitizer.AllowedAttributes.Add("href");
        _htmlSanitizer.AllowedAttributes.Add("title");

        // Only allow safe protocols
        _htmlSanitizer.AllowedSchemes.Clear();
        _htmlSanitizer.AllowedSchemes.Add("http");
        _htmlSanitizer.AllowedSchemes.Add("https");
        _htmlSanitizer.AllowedSchemes.Add("mailto");
    }

    public XssDetectionResult DetectXss(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new XssDetectionResult
            {
                ContainsXss = false,
                SanitizedContent = input,
                RiskLevel = 0
            };
        }

        var detectedPatterns = new List<string>();
        var vectors = new List<XssVector>();
        var totalRiskLevel = 0;

        foreach (var (pattern, riskLevel, description) in XssPatterns)
        {
            var matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            if (matches.Count > 0)
            {
                detectedPatterns.Add(description);
                totalRiskLevel += riskLevel;

                foreach (Match match in matches)
                {
                    vectors.Add(new XssVector
                    {
                        Pattern = description,
                        Position = match.Index,
                        Context = GetContext(input, match.Index, 50)
                    });
                }
            }
        }

        var containsXss = detectedPatterns.Count > 0;
        var finalRiskLevel = Math.Min(totalRiskLevel, 10);

        if (containsXss)
        {
            _logger.LogWarning("XSS patterns detected. Risk level: {RiskLevel}, Patterns: {Patterns}",
                finalRiskLevel, string.Join(", ", detectedPatterns));
        }

        return new XssDetectionResult
        {
            ContainsXss = containsXss,
            DetectedPatterns = detectedPatterns,
            SanitizedContent = _htmlSanitizer.Sanitize(input),
            RiskLevel = finalRiskLevel,
            Vectors = vectors
        };
    }

    public string SanitizeForDisplay(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return _htmlSanitizer.Sanitize(input);
    }

    public string SanitizeForAttribute(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // For HTML attributes, encode everything
        return System.Web.HttpUtility.HtmlAttributeEncode(input);
    }

    public string SanitizeForJavaScript(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // JavaScript string encoding
        return System.Web.HttpUtility.JavaScriptStringEncode(input);
    }

    public bool ContainsXss(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return XssPatterns.Any(pattern => 
            Regex.IsMatch(input, pattern.Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline));
    }

    public XssDetectionResult AnalyzeHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return new XssDetectionResult
            {
                ContainsXss = false,
                SanitizedContent = html,
                RiskLevel = 0
            };
        }

        var result = DetectXss(html);

        // Additional HTML-specific checks
        var additionalRisks = new List<string>();

        // Check for inline styles with expressions
        if (Regex.IsMatch(html, @"style\s*=\s*[""'][^""']*expression\(", RegexOptions.IgnoreCase))
        {
            additionalRisks.Add("CSS expression in inline style");
            result.RiskLevel = Math.Min(result.RiskLevel + 2, 10);
        }

        // Check for data URIs with HTML
        if (Regex.IsMatch(html, @"data:text/html", RegexOptions.IgnoreCase))
        {
            additionalRisks.Add("Data URI with HTML content");
            result.RiskLevel = Math.Min(result.RiskLevel + 2, 10);
        }

        // Check for base64 encoded scripts
        if (Regex.IsMatch(html, @"data:text/javascript;base64,", RegexOptions.IgnoreCase))
        {
            additionalRisks.Add("Base64 encoded JavaScript");
            result.RiskLevel = Math.Min(result.RiskLevel + 3, 10);
        }

        // Check for SVG with scripts
        if (Regex.IsMatch(html, @"<svg[^>]*>.*<script", RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            additionalRisks.Add("SVG with embedded script");
            result.RiskLevel = Math.Min(result.RiskLevel + 3, 10);
        }

        // Check for HTML entities that might hide XSS
        if (Regex.IsMatch(html, @"&#\d+;|&#x[0-9a-f]+;", RegexOptions.IgnoreCase))
        {
            var decoded = System.Web.HttpUtility.HtmlDecode(html);
            if (ContainsXss(decoded))
            {
                additionalRisks.Add("XSS hidden in HTML entities");
                result.RiskLevel = Math.Min(result.RiskLevel + 2, 10);
            }
        }

        if (additionalRisks.Count > 0)
        {
            result.DetectedPatterns.AddRange(additionalRisks);
            result.ContainsXss = true;
        }

        return result;
    }

    private string GetContext(string input, int position, int contextLength)
    {
        var start = Math.Max(0, position - contextLength / 2);
        var length = Math.Min(contextLength, input.Length - start);
        var context = input.Substring(start, length);

        if (start > 0)
            context = "..." + context;
        
        if (start + length < input.Length)
            context = context + "...";

        return context;
    }
}
