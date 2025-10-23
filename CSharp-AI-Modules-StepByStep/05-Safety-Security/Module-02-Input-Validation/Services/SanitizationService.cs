using Ganss.Xss;
using Module_02_Input_Validation.Models;
using System.Text.RegularExpressions;
using System.Web;

namespace Module_02_Input_Validation.Services;

public interface ISanitizationService
{
    SanitizationResult SanitizeHtml(string input, SanitizationOptions? options = null);
    string SanitizeForSql(string input);
    string SanitizeForXml(string input);
    string SanitizeForJson(string input);
    string SanitizeFileName(string fileName);
    string SanitizeUrl(string url);
    string RemoveHtml(string input);
    string EncodeHtml(string input);
    string EncodeJavaScript(string input);
    string EncodeCss(string input);
}

public class SanitizationService : ISanitizationService
{
    private readonly ILogger<SanitizationService> _logger;
    private readonly HtmlSanitizer _htmlSanitizer;

    public SanitizationService(ILogger<SanitizationService> logger)
    {
        _logger = logger;
        _htmlSanitizer = new HtmlSanitizer();
        ConfigureHtmlSanitizer();
    }

    private void ConfigureHtmlSanitizer()
    {
        // Configure allowed tags
        _htmlSanitizer.AllowedTags.Clear();
        _htmlSanitizer.AllowedTags.Add("p");
        _htmlSanitizer.AllowedTags.Add("br");
        _htmlSanitizer.AllowedTags.Add("strong");
        _htmlSanitizer.AllowedTags.Add("em");
        _htmlSanitizer.AllowedTags.Add("u");
        _htmlSanitizer.AllowedTags.Add("a");
        _htmlSanitizer.AllowedTags.Add("ul");
        _htmlSanitizer.AllowedTags.Add("ol");
        _htmlSanitizer.AllowedTags.Add("li");
        _htmlSanitizer.AllowedTags.Add("h1");
        _htmlSanitizer.AllowedTags.Add("h2");
        _htmlSanitizer.AllowedTags.Add("h3");

        // Configure allowed attributes
        _htmlSanitizer.AllowedAttributes.Clear();
        _htmlSanitizer.AllowedAttributes.Add("href");
        _htmlSanitizer.AllowedAttributes.Add("title");
        _htmlSanitizer.AllowedAttributes.Add("class");

        // Configure allowed protocols for links
        _htmlSanitizer.AllowedSchemes.Clear();
        _htmlSanitizer.AllowedSchemes.Add("http");
        _htmlSanitizer.AllowedSchemes.Add("https");
        _htmlSanitizer.AllowedSchemes.Add("mailto");
    }

    public SanitizationResult SanitizeHtml(string input, SanitizationOptions? options = null)
    {
        try
        {
            if (string.IsNullOrEmpty(input))
            {
                return new SanitizationResult
                {
                    OriginalValue = input,
                    SanitizedValue = input,
                    WasModified = false
                };
            }

            options ??= new SanitizationOptions();
            var sanitizer = new HtmlSanitizer();

            // Configure based on options
            if (options.AllowedTags.Count > 0)
            {
                sanitizer.AllowedTags.Clear();
                foreach (var tag in options.AllowedTags)
                {
                    sanitizer.AllowedTags.Add(tag);
                }
            }

            if (options.AllowedAttributes.Count > 0)
            {
                sanitizer.AllowedAttributes.Clear();
                foreach (var attr in options.AllowedAttributes)
                {
                    sanitizer.AllowedAttributes.Add(attr);
                }
            }

            if (options.AllowedProtocols.Count > 0)
            {
                sanitizer.AllowedSchemes.Clear();
                foreach (var protocol in options.AllowedProtocols)
                {
                    sanitizer.AllowedSchemes.Add(protocol);
                }
            }

            var sanitized = sanitizer.Sanitize(input);
            var removedElements = FindRemovedElements(input, sanitized);

            return new SanitizationResult
            {
                OriginalValue = input,
                SanitizedValue = sanitized,
                WasModified = input != sanitized,
                RemovedElements = removedElements
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sanitizing HTML input");
            return new SanitizationResult
            {
                OriginalValue = input,
                SanitizedValue = RemoveHtml(input),
                WasModified = true,
                Warnings = new List<string> { "Error during sanitization, all HTML removed" }
            };
        }
    }

    public string SanitizeForSql(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove SQL keywords and dangerous characters
        var sanitized = input
            .Replace("'", "''")  // Escape single quotes
            .Replace(";", "")    // Remove semicolons
            .Replace("--", "")   // Remove SQL comments
            .Replace("/*", "")   // Remove block comment start
            .Replace("*/", "")   // Remove block comment end
            .Replace("xp_", "")  // Remove extended stored procedure prefix
            .Replace("sp_", ""); // Remove stored procedure prefix

        // Remove SQL keywords
        var sqlKeywords = new[]
        {
            "DROP", "DELETE", "TRUNCATE", "EXEC", "EXECUTE",
            "UNION", "INSERT", "UPDATE", "DECLARE", "CAST"
        };

        foreach (var keyword in sqlKeywords)
        {
            sanitized = Regex.Replace(sanitized, $@"\b{keyword}\b", "", RegexOptions.IgnoreCase);
        }

        return sanitized;
    }

    public string SanitizeForXml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    public string SanitizeForJson(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    public string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;

        // Remove path traversal attempts
        fileName = Path.GetFileName(fileName);

        // Remove invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars));

        // Remove potentially dangerous extensions
        var dangerousExtensions = new[] { ".exe", ".bat", ".cmd", ".sh", ".ps1", ".vbs", ".js" };
        foreach (var ext in dangerousExtensions)
        {
            if (sanitized.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            {
                sanitized = sanitized.Substring(0, sanitized.Length - ext.Length) + ".txt";
            }
        }

        return sanitized;
    }

    public string SanitizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        try
        {
            var uri = new Uri(url);

            // Only allow http and https
            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                return string.Empty;
            }

            // Remove javascript: and data: schemes
            if (url.Contains("javascript:", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("data:", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return uri.ToString();
        }
        catch
        {
            _logger.LogWarning("Invalid URL format: {Url}", url);
            return string.Empty;
        }
    }

    public string RemoveHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove HTML tags
        var noHtml = Regex.Replace(input, @"<[^>]+>", string.Empty);

        // Decode HTML entities
        return HttpUtility.HtmlDecode(noHtml);
    }

    public string EncodeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return HttpUtility.HtmlEncode(input);
    }

    public string EncodeJavaScript(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return HttpUtility.JavaScriptStringEncode(input);
    }

    public string EncodeCss(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // CSS encoding: escape special characters
        return Regex.Replace(input, @"[^\w\s-]", match =>
            $"\\{((int)match.Value[0]):X}");
    }

    private List<string> FindRemovedElements(string original, string sanitized)
    {
        var removed = new List<string>();

        // Find removed script tags
        var scriptMatches = Regex.Matches(original, @"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (scriptMatches.Count > 0)
        {
            removed.Add($"{scriptMatches.Count} script tag(s)");
        }

        // Find removed style tags
        var styleMatches = Regex.Matches(original, @"<style[^>]*>.*?</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (styleMatches.Count > 0)
        {
            removed.Add($"{styleMatches.Count} style tag(s)");
        }

        // Find removed event handlers
        var eventMatches = Regex.Matches(original, @"on\w+\s*=", RegexOptions.IgnoreCase);
        if (eventMatches.Count > 0)
        {
            removed.Add($"{eventMatches.Count} event handler(s)");
        }

        return removed;
    }
}
