using Microsoft.AspNetCore.Mvc;
using Module_02_Input_Validation.Models;
using Module_02_Input_Validation.Services;
using FluentValidation;

namespace Module_02_Input_Validation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly ILogger<ValidationController> _logger;
    private readonly ISanitizationService _sanitizationService;
    private readonly IInjectionDetectionService _injectionDetectionService;
    private readonly IXssProtectionService _xssProtectionService;
    private readonly ISchemaValidationService _schemaValidationService;
    private readonly IFileValidationService _fileValidationService;
    private readonly IValidator<UserRegistrationRequest> _userValidator;
    private readonly IValidator<CommentRequest> _commentValidator;
    private readonly IValidator<SearchRequest> _searchValidator;
    private readonly IValidator<FileUploadRequest> _fileUploadValidator;

    public ValidationController(
        ILogger<ValidationController> logger,
        ISanitizationService sanitizationService,
        IInjectionDetectionService injectionDetectionService,
        IXssProtectionService xssProtectionService,
        ISchemaValidationService schemaValidationService,
        IFileValidationService fileValidationService,
        IValidator<UserRegistrationRequest> userValidator,
        IValidator<CommentRequest> commentValidator,
        IValidator<SearchRequest> searchValidator,
        IValidator<FileUploadRequest> fileUploadValidator)
    {
        _logger = logger;
        _sanitizationService = sanitizationService;
        _injectionDetectionService = injectionDetectionService;
        _xssProtectionService = xssProtectionService;
        _schemaValidationService = schemaValidationService;
        _fileValidationService = fileValidationService;
        _userValidator = userValidator;
        _commentValidator = commentValidator;
        _searchValidator = searchValidator;
        _fileUploadValidator = fileUploadValidator;
    }

    #region User Registration Validation

    [HttpPost("user/register")]
    public async Task<ActionResult<ValidationResult>> ValidateUserRegistration([FromBody] UserRegistrationRequest request)
    {
        var validationResult = await _userValidator.ValidateAsync(request);

        var result = new ValidationResult
        {
            IsValid = validationResult.IsValid,
            Message = validationResult.IsValid ? "Validation passed" : "Validation failed",
            Errors = validationResult.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                ErrorCode = e.ErrorCode,
                AttemptedValue = e.AttemptedValue,
                Severity = ValidationSeverity.Error
            }).ToList()
        };

        return validationResult.IsValid ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region Comment Validation

    [HttpPost("comment/validate")]
    public async Task<ActionResult<ValidationResult>> ValidateComment([FromBody] CommentRequest request)
    {
        var validationResult = await _commentValidator.ValidateAsync(request);

        var result = new ValidationResult
        {
            IsValid = validationResult.IsValid,
            Message = validationResult.IsValid ? "Comment is valid" : "Comment validation failed",
            Errors = validationResult.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                ErrorCode = e.ErrorCode,
                AttemptedValue = e.AttemptedValue
            }).ToList()
        };

        return validationResult.IsValid ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region Search Validation

    [HttpPost("search/validate")]
    public async Task<ActionResult<ValidationResult>> ValidateSearch([FromBody] SearchRequest request)
    {
        var validationResult = await _searchValidator.ValidateAsync(request);

        var result = new ValidationResult
        {
            IsValid = validationResult.IsValid,
            Message = validationResult.IsValid ? "Search request is valid" : "Search validation failed",
            Errors = validationResult.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            }).ToList()
        };

        return validationResult.IsValid ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region Sanitization

    [HttpPost("sanitize/html")]
    public ActionResult<SanitizationResult> SanitizeHtml([FromBody] HtmlContentRequest request)
    {
        var result = _sanitizationService.SanitizeHtml(request.Content, new SanitizationOptions
        {
            RemoveScripts = !request.AllowScripts,
            RemoveStyles = !request.AllowStyles,
            AllowedTags = request.AllowedTags ?? new List<string>()
        });

        return Ok(result);
    }

    [HttpPost("sanitize/sql")]
    public ActionResult<string> SanitizeForSql([FromBody] SqlQueryRequest request)
    {
        var sanitized = _sanitizationService.SanitizeForSql(request.Query);
        return Ok(new { Original = request.Query, Sanitized = sanitized });
    }

    [HttpPost("sanitize/filename")]
    public ActionResult<string> SanitizeFileName([FromQuery] string fileName)
    {
        var sanitized = _sanitizationService.SanitizeFileName(fileName);
        return Ok(new { Original = fileName, Sanitized = sanitized });
    }

    [HttpPost("sanitize/url")]
    public ActionResult<string> SanitizeUrl([FromQuery] string url)
    {
        var sanitized = _sanitizationService.SanitizeUrl(url);
        return Ok(new { Original = url, Sanitized = sanitized });
    }

    #endregion

    #region Injection Detection

    [HttpPost("detect/sql-injection")]
    public ActionResult<InjectionDetectionResult> DetectSqlInjection([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectSqlInjection(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    [HttpPost("detect/xss")]
    public ActionResult<InjectionDetectionResult> DetectXssInjection([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectXss(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    [HttpPost("detect/command-injection")]
    public ActionResult<InjectionDetectionResult> DetectCommandInjection([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectCommandInjection(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    [HttpPost("detect/ldap-injection")]
    public ActionResult<InjectionDetectionResult> DetectLdapInjection([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectLdapInjection(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    [HttpPost("detect/path-traversal")]
    public ActionResult<InjectionDetectionResult> DetectPathTraversal([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectPathTraversal(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    [HttpPost("detect/all-injections")]
    public ActionResult<InjectionDetectionResult> DetectAllInjections([FromQuery] string input)
    {
        var result = _injectionDetectionService.DetectAllInjections(input);
        return result.IsSuspicious ? BadRequest(result) : Ok(result);
    }

    #endregion

    #region XSS Protection

    [HttpPost("xss/detect")]
    public ActionResult<XssDetectionResult> DetectXssInContent([FromQuery] string input)
    {
        var result = _xssProtectionService.DetectXss(input);
        return result.ContainsXss ? BadRequest(result) : Ok(result);
    }

    [HttpPost("xss/analyze-html")]
    public ActionResult<XssDetectionResult> AnalyzeHtml([FromBody] string html)
    {
        var result = _xssProtectionService.AnalyzeHtml(html);
        return result.ContainsXss ? BadRequest(result) : Ok(result);
    }

    [HttpPost("xss/sanitize-display")]
    public ActionResult<string> SanitizeForDisplay([FromQuery] string input)
    {
        var sanitized = _xssProtectionService.SanitizeForDisplay(input);
        return Ok(new { Original = input, Sanitized = sanitized });
    }

    [HttpPost("xss/sanitize-attribute")]
    public ActionResult<string> SanitizeForAttribute([FromQuery] string input)
    {
        var sanitized = _xssProtectionService.SanitizeForAttribute(input);
        return Ok(new { Original = input, Sanitized = sanitized });
    }

    [HttpPost("xss/sanitize-javascript")]
    public ActionResult<string> SanitizeForJavaScript([FromQuery] string input)
    {
        var sanitized = _xssProtectionService.SanitizeForJavaScript(input);
        return Ok(new { Original = input, Sanitized = sanitized });
    }

    #endregion

    #region Schema Validation

    [HttpPost("schema/validate")]
    public async Task<ActionResult<SchemaValidationResult>> ValidateJsonSchema([FromBody] JsonSchemaValidationRequest request)
    {
        var result = await _schemaValidationService.ValidateJsonSchemaAsync(request.JsonData, request.Schema);
        return result.IsValid ? Ok(result) : BadRequest(result);
    }

    [HttpPost("schema/generate/{typeName}")]
    public async Task<ActionResult<string>> GenerateSchema(string typeName)
    {
        try
        {
            string schema = typeName.ToLower() switch
            {
                "userregistration" => await _schemaValidationService.GenerateSchemaAsync<UserRegistrationRequest>(),
                "comment" => await _schemaValidationService.GenerateSchemaAsync<CommentRequest>(),
                "search" => await _schemaValidationService.GenerateSchemaAsync<SearchRequest>(),
                _ => throw new ArgumentException($"Unknown type: {typeName}")
            };

            return Ok(schema);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("schema/is-valid-json")]
    public ActionResult<bool> IsValidJson([FromBody] string jsonString)
    {
        var isValid = _schemaValidationService.IsValidJson(jsonString);
        return Ok(new { IsValid = isValid });
    }

    #endregion

    #region File Validation

    [HttpPost("file/validate")]
    public async Task<ActionResult<FileValidationResult>> ValidateFileUpload([FromBody] FileUploadRequest request)
    {
        // First validate the request model
        var modelValidation = await _fileUploadValidator.ValidateAsync(request);
        if (!modelValidation.IsValid)
        {
            return BadRequest(new
            {
                IsValid = false,
                Errors = modelValidation.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        // Then validate file content and security
        var rules = new FileValidationRules
        {
            AllowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".txt" },
            AllowedMimeTypes = new List<string> 
            { 
                "image/jpeg", "image/png", "image/gif", 
                "application/pdf", 
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "text/plain"
            },
            MaxFileSizeBytes = 10 * 1024 * 1024, // 10 MB
            CheckMimeType = true,
            CheckFileSignature = true,
            AllowExecutables = false
        };

        var result = _fileValidationService.ValidateFile(
            request.FileContent,
            request.FileName,
            request.ContentType,
            rules);

        return result.IsValid ? Ok(result) : BadRequest(result);
    }

    [HttpPost("file/detect-mime-type")]
    public ActionResult<string> DetectMimeType([FromBody] FileUploadRequest request)
    {
        var mimeType = _fileValidationService.DetectMimeType(request.FileContent, request.FileName);
        return Ok(new { FileName = request.FileName, DetectedMimeType = mimeType });
    }

    [HttpPost("file/calculate-hash")]
    public ActionResult<string> CalculateFileHash([FromBody] FileUploadRequest request)
    {
        var hash = _fileValidationService.CalculateFileHash(request.FileContent);
        return Ok(new { FileName = request.FileName, SHA256Hash = hash });
    }

    [HttpGet("file/is-executable")]
    public ActionResult<bool> IsExecutableFile([FromQuery] string fileName)
    {
        var isExecutable = _fileValidationService.IsExecutable(fileName);
        return Ok(new { FileName = fileName, IsExecutable = isExecutable });
    }

    #endregion

    #region Utility Endpoints

    [HttpGet("patterns/common")]
    public ActionResult<object> GetCommonPatterns()
    {
        return Ok(new
        {
            Email = CommonPatterns.Email,
            PhoneUS = CommonPatterns.PhoneUS,
            PhoneInternational = CommonPatterns.PhoneInternational,
            PasswordStrong = CommonPatterns.PasswordStrong,
            Username = CommonPatterns.Username,
            Url = CommonPatterns.Url,
            IPv4 = CommonPatterns.IPv4,
            IPv6 = CommonPatterns.IPv6,
            CreditCard = CommonPatterns.CreditCard,
            DateISO = CommonPatterns.DateISO
        });
    }

    [HttpPost("validate/email")]
    public ActionResult<ValidationResult> ValidateEmail([FromQuery] string email)
    {
        var isValid = System.Text.RegularExpressions.Regex.IsMatch(email, CommonPatterns.Email);
        return Ok(new ValidationResult
        {
            IsValid = isValid,
            Message = isValid ? "Valid email address" : "Invalid email address"
        });
    }

    [HttpPost("validate/url")]
    public ActionResult<ValidationResult> ValidateUrlFormat([FromQuery] string url)
    {
        var isValid = System.Text.RegularExpressions.Regex.IsMatch(url, CommonPatterns.Url);
        return Ok(new ValidationResult
        {
            IsValid = isValid,
            Message = isValid ? "Valid URL format" : "Invalid URL format"
        });
    }

    #endregion
}
