using Microsoft.AspNetCore.Mvc;
using Module_03_Content_Safety.Models;
using Module_03_Content_Safety.Services;

namespace Module_03_Content_Safety.Controllers;

[ApiController]
[Route("api/content-safety")]
public class ContentSafetyController : ControllerBase
{
    private readonly ILogger<ContentSafetyController> _logger;
    private readonly IToxicityDetectionService _toxicityService;
    private readonly IPiiDetectionService _piiService;
    private readonly IContentModerationService _moderationService;
    private readonly IPromptInjectionService _promptInjectionService;
    private readonly IOutputFilterService _outputFilterService;

    public ContentSafetyController(
        ILogger<ContentSafetyController> logger,
        IToxicityDetectionService toxicityService,
        IPiiDetectionService piiService,
        IContentModerationService moderationService,
        IPromptInjectionService promptInjectionService,
        IOutputFilterService outputFilterService)
    {
        _logger = logger;
        _toxicityService = toxicityService;
        _piiService = piiService;
        _moderationService = moderationService;
        _promptInjectionService = promptInjectionService;
        _outputFilterService = outputFilterService;
    }

    #region Toxicity Detection

    /// <summary>
    /// Detect toxicity in content
    /// </summary>
    [HttpPost("toxicity/detect")]
    public async Task<ActionResult<ToxicityResult>> DetectToxicity([FromBody] ToxicityDetectionRequest request)
    {
        try
        {
            var result = await _toxicityService.DetectToxicityAsync(request.Content, request.Threshold);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting toxicity");
            return StatusCode(500, new { error = "Error detecting toxicity", details = ex.Message });
        }
    }

    /// <summary>
    /// Detect toxicity by specific categories
    /// </summary>
    [HttpPost("toxicity/detect-by-category")]
    public async Task<ActionResult<ToxicityResult>> DetectToxicityByCategory([FromBody] ToxicityDetectionRequest request)
    {
        try
        {
            var categories = request.Categories ?? Enum.GetValues<ToxicityCategory>().ToList();
            var result = await _toxicityService.DetectToxicityByCategoryAsync(request.Content, categories);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting toxicity by category");
            return StatusCode(500, new { error = "Error detecting toxicity", details = ex.Message });
        }
    }

    /// <summary>
    /// Check if content is toxic
    /// </summary>
    [HttpPost("toxicity/is-toxic")]
    public async Task<ActionResult<object>> IsToxic([FromBody] ToxicityDetectionRequest request)
    {
        try
        {
            var isToxic = await _toxicityService.IsToxicAsync(request.Content, request.Threshold);
            return Ok(new { isToxic, threshold = request.Threshold });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking toxicity");
            return StatusCode(500, new { error = "Error checking toxicity", details = ex.Message });
        }
    }

    /// <summary>
    /// Sanitize toxic content
    /// </summary>
    [HttpPost("toxicity/sanitize")]
    public async Task<ActionResult<object>> SanitizeToxicContent([FromBody] ToxicityDetectionRequest request)
    {
        try
        {
            var sanitized = await _toxicityService.SanitizeToxicContentAsync(request.Content);
            return Ok(new { originalContent = request.Content, sanitizedContent = sanitized });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sanitizing toxic content");
            return StatusCode(500, new { error = "Error sanitizing content", details = ex.Message });
        }
    }

    #endregion

    #region PII Detection

    /// <summary>
    /// Detect PII in content
    /// </summary>
    [HttpPost("pii/detect")]
    public async Task<ActionResult<PiiDetectionResult>> DetectPii([FromBody] PiiDetectionRequest request)
    {
        try
        {
            PiiDetectionResult result;
            
            if (request.PiiTypes != null && request.PiiTypes.Any())
            {
                result = await _piiService.DetectPiiAsync(request.Content, request.PiiTypes);
            }
            else
            {
                result = await _piiService.DetectPiiAsync(request.Content);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting PII");
            return StatusCode(500, new { error = "Error detecting PII", details = ex.Message });
        }
    }

    /// <summary>
    /// Redact PII from content
    /// </summary>
    [HttpPost("pii/redact")]
    public async Task<ActionResult<object>> RedactPii([FromBody] PiiDetectionRequest request)
    {
        try
        {
            var redacted = await _piiService.RedactPiiAsync(request.Content, request.RedactionStrategy);
            return Ok(new 
            { 
                originalContent = request.Content, 
                redactedContent = redacted,
                strategy = request.RedactionStrategy.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redacting PII");
            return StatusCode(500, new { error = "Error redacting PII", details = ex.Message });
        }
    }

    /// <summary>
    /// Mask PII in content
    /// </summary>
    [HttpPost("pii/mask")]
    public async Task<ActionResult<object>> MaskPii([FromBody] PiiDetectionRequest request)
    {
        try
        {
            var masked = await _piiService.MaskPiiAsync(request.Content);
            return Ok(new { originalContent = request.Content, maskedContent = masked });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error masking PII");
            return StatusCode(500, new { error = "Error masking PII", details = ex.Message });
        }
    }

    /// <summary>
    /// Check if content contains PII
    /// </summary>
    [HttpPost("pii/contains")]
    public async Task<ActionResult<object>> ContainsPii([FromBody] PiiDetectionRequest request)
    {
        try
        {
            var containsPii = await _piiService.ContainsPiiAsync(request.Content);
            return Ok(new { containsPii });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for PII");
            return StatusCode(500, new { error = "Error checking for PII", details = ex.Message });
        }
    }

    #endregion

    #region Content Moderation

    /// <summary>
    /// Moderate content
    /// </summary>
    [HttpPost("moderate")]
    public async Task<ActionResult<ModerationResult>> ModerateContent([FromBody] ModerationRequest request)
    {
        try
        {
            var result = await _moderationService.ModerateContentAsync(request.Content, request.Mode);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating content");
            return StatusCode(500, new { error = "Error moderating content", details = ex.Message });
        }
    }

    /// <summary>
    /// Analyze content safety
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<ContentSafetyResult>> AnalyzeContentSafety([FromBody] ContentAnalysisRequest request)
    {
        try
        {
            var result = await _moderationService.AnalyzeContentSafetyAsync(request.Content);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing content safety");
            return StatusCode(500, new { error = "Error analyzing content", details = ex.Message });
        }
    }

    /// <summary>
    /// Check if content is safe
    /// </summary>
    [HttpPost("is-safe")]
    public async Task<ActionResult<object>> IsContentSafe([FromBody] ContentAnalysisRequest request)
    {
        try
        {
            var threshold = 5.0; // Default threshold
            var isSafe = await _moderationService.IsContentSafeAsync(request.Content, threshold);
            return Ok(new { isSafe, threshold });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking content safety");
            return StatusCode(500, new { error = "Error checking safety", details = ex.Message });
        }
    }

    /// <summary>
    /// Filter unsafe content
    /// </summary>
    [HttpPost("filter")]
    public async Task<ActionResult<object>> FilterContent([FromBody] ContentAnalysisRequest request)
    {
        try
        {
            var filtered = await _moderationService.FilterContentAsync(request.Content);
            return Ok(new { originalContent = request.Content, filteredContent = filtered });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering content");
            return StatusCode(500, new { error = "Error filtering content", details = ex.Message });
        }
    }

    #endregion

    #region Prompt Injection Detection

    /// <summary>
    /// Detect prompt injection
    /// </summary>
    [HttpPost("prompt-injection/detect")]
    public async Task<ActionResult<PromptInjectionResult>> DetectPromptInjection([FromBody] PromptInjectionRequest request)
    {
        try
        {
            var result = await _promptInjectionService.DetectPromptInjectionAsync(
                request.Prompt, 
                request.SystemPrompt, 
                request.Context);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting prompt injection");
            return StatusCode(500, new { error = "Error detecting prompt injection", details = ex.Message });
        }
    }

    /// <summary>
    /// Check if prompt is safe
    /// </summary>
    [HttpPost("prompt-injection/is-safe")]
    public async Task<ActionResult<object>> IsPromptSafe([FromBody] PromptInjectionRequest request)
    {
        try
        {
            var isSafe = await _promptInjectionService.IsPromptSafeAsync(request.Prompt, request.Threshold);
            return Ok(new { isSafe, threshold = request.Threshold });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking prompt safety");
            return StatusCode(500, new { error = "Error checking prompt safety", details = ex.Message });
        }
    }

    /// <summary>
    /// Sanitize prompt
    /// </summary>
    [HttpPost("prompt-injection/sanitize")]
    public async Task<ActionResult<object>> SanitizePrompt([FromBody] PromptInjectionRequest request)
    {
        try
        {
            var sanitized = await _promptInjectionService.SanitizePromptAsync(request.Prompt);
            return Ok(new { originalPrompt = request.Prompt, sanitizedPrompt = sanitized });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sanitizing prompt");
            return StatusCode(500, new { error = "Error sanitizing prompt", details = ex.Message });
        }
    }

    #endregion

    #region Output Filtering

    /// <summary>
    /// Filter output content
    /// </summary>
    [HttpPost("output/filter")]
    public async Task<ActionResult<OutputFilterResult>> FilterOutput([FromBody] OutputFilterRequest request)
    {
        try
        {
            OutputFilterResult result;
            
            if (request.FilterTypes != null && request.FilterTypes.Any())
            {
                result = await _outputFilterService.FilterOutputAsync(request.Content, request.FilterTypes);
            }
            else
            {
                result = await _outputFilterService.FilterOutputAsync(request.Content);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering output");
            return StatusCode(500, new { error = "Error filtering output", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove PII from output
    /// </summary>
    [HttpPost("output/remove-pii")]
    public async Task<ActionResult<object>> RemovePiiFromOutput([FromBody] OutputFilterRequest request)
    {
        try
        {
            var filtered = await _outputFilterService.RemovePiiFromOutputAsync(request.Content);
            return Ok(new { originalContent = request.Content, filteredContent = filtered });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing PII from output");
            return StatusCode(500, new { error = "Error removing PII", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove toxicity from output
    /// </summary>
    [HttpPost("output/remove-toxicity")]
    public async Task<ActionResult<object>> RemoveToxicityFromOutput([FromBody] OutputFilterRequest request)
    {
        try
        {
            var filtered = await _outputFilterService.RemoveToxicityFromOutputAsync(request.Content);
            return Ok(new { originalContent = request.Content, filteredContent = filtered });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing toxicity from output");
            return StatusCode(500, new { error = "Error removing toxicity", details = ex.Message });
        }
    }

    /// <summary>
    /// Sanitize HTML in output
    /// </summary>
    [HttpPost("output/sanitize-html")]
    public async Task<ActionResult<object>> SanitizeHtmlInOutput([FromBody] OutputFilterRequest request)
    {
        try
        {
            var sanitized = await _outputFilterService.SanitizeHtmlInOutputAsync(request.Content);
            return Ok(new { originalContent = request.Content, sanitizedContent = sanitized });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sanitizing HTML in output");
            return StatusCode(500, new { error = "Error sanitizing HTML", details = ex.Message });
        }
    }

    /// <summary>
    /// Limit output length
    /// </summary>
    [HttpPost("output/limit-length")]
    public async Task<ActionResult<object>> LimitOutputLength([FromBody] OutputFilterRequest request)
    {
        try
        {
            var maxLength = request.MaxLength ?? 10000;
            var limited = await _outputFilterService.LimitOutputLengthAsync(request.Content, maxLength);
            return Ok(new 
            { 
                originalContent = request.Content,
                originalLength = request.Content.Length,
                limitedContent = limited,
                limitedLength = limited.Length,
                maxLength 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error limiting output length");
            return StatusCode(500, new { error = "Error limiting output length", details = ex.Message });
        }
    }

    #endregion

    #region Batch Operations

    /// <summary>
    /// Batch analyze multiple contents
    /// </summary>
    [HttpPost("batch/analyze")]
    public async Task<ActionResult<List<ContentSafetyResult>>> BatchAnalyze([FromBody] List<ContentAnalysisRequest> requests)
    {
        try
        {
            var tasks = requests.Select(r => _moderationService.AnalyzeContentSafetyAsync(r.Content));
            var results = await Task.WhenAll(tasks);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch analysis");
            return StatusCode(500, new { error = "Error in batch analysis", details = ex.Message });
        }
    }

    /// <summary>
    /// Batch moderate multiple contents
    /// </summary>
    [HttpPost("batch/moderate")]
    public async Task<ActionResult<List<ModerationResult>>> BatchModerate([FromBody] List<ModerationRequest> requests)
    {
        try
        {
            var tasks = requests.Select(r => _moderationService.ModerateContentAsync(r.Content, r.Mode));
            var results = await Task.WhenAll(tasks);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch moderation");
            return StatusCode(500, new { error = "Error in batch moderation", details = ex.Message });
        }
    }

    #endregion

    #region Utility Endpoints

    /// <summary>
    /// Get safety patterns
    /// </summary>
    [HttpGet("patterns")]
    public ActionResult<object> GetSafetyPatterns()
    {
        return Ok(new
        {
            promptInjectionPatterns = SafetyPatterns.PromptInjectionPatterns,
            jailbreakPatterns = SafetyPatterns.JailbreakPatterns,
            piiPatterns = SafetyPatterns.PiiRegexPatterns.Keys.Select(k => k.ToString())
        });
    }

    /// <summary>
    /// Get supported PII types
    /// </summary>
    [HttpGet("pii-types")]
    public ActionResult<object> GetPiiTypes()
    {
        var types = Enum.GetValues<PiiType>()
            .Select(t => new { 
                value = (int)t, 
                name = t.ToString(),
                description = GetPiiTypeDescription(t)
            });
        return Ok(types);
    }

    /// <summary>
    /// Get toxicity categories
    /// </summary>
    [HttpGet("toxicity-categories")]
    public ActionResult<object> GetToxicityCategories()
    {
        var categories = Enum.GetValues<ToxicityCategory>()
            .Select(c => new { 
                value = (int)c, 
                name = c.ToString(),
                description = GetToxicityCategoryDescription(c)
            });
        return Ok(categories);
    }

    /// <summary>
    /// Health check
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new 
        { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            services = new
            {
                toxicityDetection = "operational",
                piiDetection = "operational",
                contentModeration = "operational",
                promptInjection = "operational",
                outputFiltering = "operational"
            }
        });
    }

    #endregion

    private string GetPiiTypeDescription(PiiType type)
    {
        return type switch
        {
            PiiType.EmailAddress => "Email addresses",
            PiiType.PhoneNumber => "Phone numbers in various formats",
            PiiType.SocialSecurityNumber => "US Social Security Numbers",
            PiiType.CreditCardNumber => "Credit card numbers",
            PiiType.IpAddress => "IPv4 addresses",
            PiiType.MacAddress => "MAC addresses",
            PiiType.DriverLicense => "Driver's license numbers",
            PiiType.Passport => "Passport numbers",
            PiiType.BankAccount => "Bank account numbers",
            PiiType.DateOfBirth => "Dates of birth",
            PiiType.Address => "Physical addresses",
            PiiType.PersonName => "Person names",
            PiiType.TaxId => "Tax ID numbers",
            PiiType.ApiKey => "API keys and tokens",
            PiiType.Password => "Passwords",
            PiiType.Url => "URLs",
            PiiType.Coordinate => "GPS coordinates",
            _ => type.ToString()
        };
    }

    private string GetToxicityCategoryDescription(ToxicityCategory category)
    {
        return category switch
        {
            ToxicityCategory.Toxic => "General toxic language",
            ToxicityCategory.SevereToxic => "Severely toxic language",
            ToxicityCategory.Obscene => "Obscene language",
            ToxicityCategory.Threat => "Threatening language",
            ToxicityCategory.Insult => "Insulting language",
            ToxicityCategory.IdentityHate => "Identity-based hate speech",
            ToxicityCategory.Sexual => "Sexual content",
            ToxicityCategory.Violence => "Violent content",
            _ => category.ToString()
        };
    }
}
