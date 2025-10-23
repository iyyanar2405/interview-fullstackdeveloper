using System.Text.RegularExpressions;
using Module_03_Content_Safety.Models;

namespace Module_03_Content_Safety.Services;

public interface IOutputFilterService
{
    Task<OutputFilterResult> FilterOutputAsync(string content);
    Task<OutputFilterResult> FilterOutputAsync(string content, List<FilterActionType> filterTypes);
    Task<string> RemovePiiFromOutputAsync(string content);
    Task<string> RemoveToxicityFromOutputAsync(string content);
    Task<string> SanitizeHtmlInOutputAsync(string content);
    Task<string> LimitOutputLengthAsync(string content, int maxLength);
}

public class OutputFilterService : IOutputFilterService
{
    private readonly ILogger<OutputFilterService> _logger;
    private readonly IPiiDetectionService _piiService;
    private readonly IToxicityDetectionService _toxicityService;
    private readonly IConfiguration _configuration;

    public OutputFilterService(
        ILogger<OutputFilterService> logger,
        IPiiDetectionService piiService,
        IToxicityDetectionService toxicityService,
        IConfiguration configuration)
    {
        _logger = logger;
        _piiService = piiService;
        _toxicityService = toxicityService;
        _configuration = configuration;
    }

    public async Task<OutputFilterResult> FilterOutputAsync(string content)
    {
        var allFilterTypes = Enum.GetValues<FilterActionType>().ToList();
        return await FilterOutputAsync(content, allFilterTypes);
    }

    public async Task<OutputFilterResult> FilterOutputAsync(string content, List<FilterActionType> filterTypes)
    {
        _logger.LogInformation("Filtering output. Content length: {Length}, Filter types: {FilterCount}", 
            content.Length, filterTypes.Count);

        var result = new OutputFilterResult
        {
            FilteredContent = content,
            ActionsApplied = new List<FilterAction>(),
            FilterStats = new Dictionary<string, int>()
        };

        // Apply filters in order
        foreach (var filterType in filterTypes)
        {
            var beforeLength = result.FilteredContent.Length;
            
            switch (filterType)
            {
                case FilterActionType.PiiRedaction:
                    result.FilteredContent = await ApplyPiiRedactionAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.ToxicityRemoval:
                    result.FilteredContent = await ApplyToxicityRemovalAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.ProfanityFilter:
                    result.FilteredContent = await ApplyProfanityFilterAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.SensitiveDataRemoval:
                    result.FilteredContent = await ApplySensitiveDataRemovalAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.LinkRemoval:
                    result.FilteredContent = await ApplyLinkRemovalAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.ScriptRemoval:
                    result.FilteredContent = await ApplyScriptRemovalAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.HtmlSanitization:
                    result.FilteredContent = await ApplyHtmlSanitizationAsync(result.FilteredContent, result);
                    break;

                case FilterActionType.CharacterLimiting:
                    var maxLength = _configuration.GetValue<int>("ContentSafety:OutputFilter:MaxOutputLength", 10000);
                    result.FilteredContent = await ApplyCharacterLimitingAsync(result.FilteredContent, maxLength, result);
                    break;
            }

            var afterLength = result.FilteredContent.Length;
            if (beforeLength != afterLength)
            {
                result.FilterStats[filterType.ToString()] = beforeLength - afterLength;
            }
        }

        result.WasFiltered = content != result.FilteredContent;
        
        _logger.LogInformation("Output filtering complete. Was filtered: {WasFiltered}, Actions: {ActionCount}", 
            result.WasFiltered, result.ActionsApplied.Count);

        return result;
    }

    public async Task<string> RemovePiiFromOutputAsync(string content)
    {
        return await _piiService.RedactPiiAsync(content, RedactionStrategy.Replace);
    }

    public async Task<string> RemoveToxicityFromOutputAsync(string content)
    {
        var toxicityResult = await _toxicityService.DetectToxicityAsync(content);
        
        if (toxicityResult.IsToxic && toxicityResult.SanitizedContent != null)
        {
            return toxicityResult.SanitizedContent;
        }

        return content;
    }

    public async Task<string> SanitizeHtmlInOutputAsync(string content)
    {
        await Task.CompletedTask;

        // Remove script tags
        content = Regex.Replace(content, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", 
            "", RegexOptions.IgnoreCase);

        // Remove event handlers
        content = Regex.Replace(content, @"\s*on\w+\s*=\s*[""'][^""']*[""']", 
            "", RegexOptions.IgnoreCase);

        // Remove javascript: protocol
        content = Regex.Replace(content, @"javascript:\s*", 
            "", RegexOptions.IgnoreCase);

        // Allow only safe HTML tags
        var allowedTags = new[] { "p", "br", "strong", "em", "u", "a", "ul", "ol", "li", "h1", "h2", "h3", "blockquote" };
        var tagPattern = @"</?(\w+)(?:\s+[^>]*)?>";
        
        content = Regex.Replace(content, tagPattern, match =>
        {
            var tagName = match.Groups[1].Value.ToLowerInvariant();
            return allowedTags.Contains(tagName) ? match.Value : "";
        }, RegexOptions.IgnoreCase);

        return content;
    }

    public async Task<string> LimitOutputLengthAsync(string content, int maxLength)
    {
        await Task.CompletedTask;

        if (content.Length <= maxLength)
            return content;

        // Try to cut at sentence boundary
        var truncated = content.Substring(0, maxLength);
        var lastPeriod = truncated.LastIndexOf('.');
        var lastQuestion = truncated.LastIndexOf('?');
        var lastExclamation = truncated.LastIndexOf('!');

        var lastSentence = Math.Max(lastPeriod, Math.Max(lastQuestion, lastExclamation));

        if (lastSentence > maxLength * 0.8)
        {
            return truncated.Substring(0, lastSentence + 1);
        }

        return truncated + "...";
    }

    private async Task<string> ApplyPiiRedactionAsync(string content, OutputFilterResult result)
    {
        var piiResult = await _piiService.DetectPiiAsync(content);
        
        if (piiResult.ContainsPii)
        {
            foreach (var entity in piiResult.DetectedEntities)
            {
                result.ActionsApplied.Add(new FilterAction
                {
                    Type = FilterActionType.PiiRedaction,
                    Description = $"Redacted {entity.Type}",
                    OriginalContent = entity.Value,
                    FilteredContent = entity.RedactedValue,
                    Position = entity.StartPosition
                });
            }

            result.RemovedSensitiveDataCount += piiResult.PiiCount;
            return piiResult.RedactedContent;
        }

        return content;
    }

    private async Task<string> ApplyToxicityRemovalAsync(string content, OutputFilterResult result)
    {
        var toxicityResult = await _toxicityService.DetectToxicityAsync(content);
        
        if (toxicityResult.IsToxic)
        {
            foreach (var phrase in toxicityResult.DetectedPhrases)
            {
                result.ActionsApplied.Add(new FilterAction
                {
                    Type = FilterActionType.ToxicityRemoval,
                    Description = $"Removed {phrase.Category} content",
                    OriginalContent = phrase.Phrase,
                    FilteredContent = "[REMOVED]",
                    Position = phrase.StartPosition
                });
            }

            return toxicityResult.SanitizedContent ?? content;
        }

        return content;
    }

    private async Task<string> ApplyProfanityFilterAsync(string content, OutputFilterResult result)
    {
        await Task.CompletedTask;

        var profanityWords = GetProfanityList();
        var filtered = content;
        var contentLower = content.ToLowerInvariant();

        foreach (var profanity in profanityWords)
        {
            var profanityLower = profanity.ToLowerInvariant();
            var pattern = $@"\b{Regex.Escape(profanity)}\b";
            
            if (Regex.IsMatch(contentLower, pattern, RegexOptions.IgnoreCase))
            {
                var replacement = new string('*', profanity.Length);
                filtered = Regex.Replace(filtered, pattern, replacement, RegexOptions.IgnoreCase);
                
                result.ActionsApplied.Add(new FilterAction
                {
                    Type = FilterActionType.ProfanityFilter,
                    Description = "Filtered profanity",
                    OriginalContent = profanity,
                    FilteredContent = replacement
                });
            }
        }

        return filtered;
    }

    private async Task<string> ApplySensitiveDataRemovalAsync(string content, OutputFilterResult result)
    {
        await Task.CompletedTask;

        var filtered = content;

        // Remove API keys
        var apiKeyPattern = @"\b(?:api[_-]?key|apikey|access[_-]?token|secret[_-]?key)[:\s=]+[A-Za-z0-9_\-]{20,}\b";
        filtered = Regex.Replace(filtered, apiKeyPattern, match =>
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.SensitiveDataRemoval,
                Description = "Removed API key or token",
                OriginalContent = match.Value,
                FilteredContent = "[API_KEY_REMOVED]",
                Position = match.Index
            });
            result.RemovedSensitiveDataCount++;
            return "[API_KEY_REMOVED]";
        }, RegexOptions.IgnoreCase);

        // Remove passwords
        var passwordPattern = @"\b(?:password|passwd|pwd)[:\s=]+[^\s]{6,}\b";
        filtered = Regex.Replace(filtered, passwordPattern, match =>
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.SensitiveDataRemoval,
                Description = "Removed password",
                OriginalContent = match.Value,
                FilteredContent = "[PASSWORD_REMOVED]",
                Position = match.Index
            });
            result.RemovedSensitiveDataCount++;
            return "[PASSWORD_REMOVED]";
        }, RegexOptions.IgnoreCase);

        // Remove private keys
        var privateKeyPattern = @"-----BEGIN (?:RSA |EC |DSA )?PRIVATE KEY-----[\s\S]*?-----END (?:RSA |EC |DSA )?PRIVATE KEY-----";
        filtered = Regex.Replace(filtered, privateKeyPattern, match =>
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.SensitiveDataRemoval,
                Description = "Removed private key",
                OriginalContent = "[PRIVATE_KEY]",
                FilteredContent = "[PRIVATE_KEY_REMOVED]",
                Position = match.Index
            });
            result.RemovedSensitiveDataCount++;
            return "[PRIVATE_KEY_REMOVED]";
        });

        return filtered;
    }

    private async Task<string> ApplyLinkRemovalAsync(string content, OutputFilterResult result)
    {
        await Task.CompletedTask;

        var urlPattern = @"https?://[^\s<>""]+|www\.[^\s<>""]+";
        
        return Regex.Replace(content, urlPattern, match =>
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.LinkRemoval,
                Description = "Removed URL",
                OriginalContent = match.Value,
                FilteredContent = "[LINK_REMOVED]",
                Position = match.Index
            });
            return "[LINK_REMOVED]";
        }, RegexOptions.IgnoreCase);
    }

    private async Task<string> ApplyScriptRemovalAsync(string content, OutputFilterResult result)
    {
        await Task.CompletedTask;

        var filtered = content;
        var originalLength = content.Length;

        // Remove script tags
        filtered = Regex.Replace(filtered, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", 
            "", RegexOptions.IgnoreCase);

        // Remove style tags
        filtered = Regex.Replace(filtered, @"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", 
            "", RegexOptions.IgnoreCase);

        // Remove event handlers
        filtered = Regex.Replace(filtered, @"\s*on\w+\s*=\s*[""'][^""']*[""']", 
            "", RegexOptions.IgnoreCase);

        // Remove javascript: protocol
        filtered = Regex.Replace(filtered, @"javascript:\s*[^;""'\s]+", 
            "", RegexOptions.IgnoreCase);

        if (filtered.Length < originalLength)
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.ScriptRemoval,
                Description = "Removed scripts and event handlers",
                OriginalContent = $"{originalLength - filtered.Length} characters removed"
            });
        }

        return filtered;
    }

    private async Task<string> ApplyHtmlSanitizationAsync(string content, OutputFilterResult result)
    {
        var sanitized = await SanitizeHtmlInOutputAsync(content);
        
        if (sanitized != content)
        {
            result.ActionsApplied.Add(new FilterAction
            {
                Type = FilterActionType.HtmlSanitization,
                Description = "Sanitized HTML content",
                OriginalContent = $"{content.Length - sanitized.Length} characters removed"
            });
        }

        return sanitized;
    }

    private async Task<string> ApplyCharacterLimitingAsync(string content, int maxLength, OutputFilterResult result)
    {
        if (content.Length <= maxLength)
            return content;

        var truncated = await LimitOutputLengthAsync(content, maxLength);
        
        result.ActionsApplied.Add(new FilterAction
        {
            Type = FilterActionType.CharacterLimiting,
            Description = $"Truncated content from {content.Length} to {truncated.Length} characters",
            OriginalContent = $"{content.Length} characters",
            FilteredContent = $"{truncated.Length} characters"
        });

        return truncated;
    }

    private List<string> GetProfanityList()
    {
        // In production, load from configuration or database
        // This is a placeholder - use a proper profanity filter library
        return new List<string>
        {
            // Add profanity words here
        };
    }
}
