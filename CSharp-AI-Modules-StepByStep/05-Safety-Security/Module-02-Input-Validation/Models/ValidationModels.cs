namespace Module_02_Input_Validation.Models;

#region Validation Result Models

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

#endregion

#region Request Models

public class UserRegistrationRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Website { get; set; }
    public string? Bio { get; set; }
}

public class CommentRequest
{
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Website { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<string>? Filters { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class FileUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string? Description { get; set; }
}

public class SqlQueryRequest
{
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
}

public class HtmlContentRequest
{
    public string Content { get; set; } = string.Empty;
    public bool AllowScripts { get; set; } = false;
    public bool AllowStyles { get; set; } = true;
    public List<string>? AllowedTags { get; set; }
}

#endregion

#region Schema Models

public class JsonSchemaValidationRequest
{
    public string JsonData { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
}

public class SchemaValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string SchemaVersion { get; set; } = string.Empty;
}

#endregion

#region Sanitization Models

public class SanitizationResult
{
    public string OriginalValue { get; set; } = string.Empty;
    public string SanitizedValue { get; set; } = string.Empty;
    public bool WasModified { get; set; }
    public List<string> RemovedElements { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class SanitizationOptions
{
    public bool RemoveScripts { get; set; } = true;
    public bool RemoveStyles { get; set; } = false;
    public bool RemoveLinks { get; set; } = false;
    public bool RemoveImages { get; set; } = false;
    public bool EncodeHtml { get; set; } = true;
    public bool RemoveSqlKeywords { get; set; } = true;
    public List<string> AllowedTags { get; set; } = new();
    public List<string> AllowedAttributes { get; set; } = new();
    public List<string> AllowedProtocols { get; set; } = new() { "http", "https", "mailto" };
}

#endregion

#region Security Models

public class InjectionDetectionResult
{
    public bool IsSuspicious { get; set; }
    public InjectionType Type { get; set; }
    public List<string> DetectedPatterns { get; set; } = new();
    public double RiskScore { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? SanitizedInput { get; set; }
}

public enum InjectionType
{
    None,
    SqlInjection,
    XssAttack,
    CommandInjection,
    LdapInjection,
    XPathInjection,
    ScriptInjection,
    PathTraversal
}

public class XssDetectionResult
{
    public bool ContainsXss { get; set; }
    public List<string> DetectedPatterns { get; set; } = new();
    public string SanitizedContent { get; set; } = string.Empty;
    public int RiskLevel { get; set; } // 0-10 scale
    public List<XssVector> Vectors { get; set; } = new();
}

public class XssVector
{
    public string Pattern { get; set; } = string.Empty;
    public int Position { get; set; }
    public string Context { get; set; } = string.Empty;
}

#endregion

#region File Validation Models

public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DetectedMimeType { get; set; } = string.Empty;
    public string DeclaredMimeType { get; set; } = string.Empty;
    public bool MimeTypeMismatch { get; set; }
    public long FileSize { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public Dictionary<string, string> FileProperties { get; set; } = new();
}

public class FileValidationRules
{
    public List<string> AllowedExtensions { get; set; } = new();
    public List<string> AllowedMimeTypes { get; set; } = new();
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB default
    public bool CheckMimeType { get; set; } = true;
    public bool CheckFileSignature { get; set; } = true;
    public bool AllowExecutables { get; set; } = false;
}

#endregion

#region URL Validation Models

public class UrlValidationResult
{
    public bool IsValid { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string NormalizedUrl { get; set; } = string.Empty;
    public string Protocol { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public bool IsSecure { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public bool IsWhitelisted { get; set; }
    public bool IsBlacklisted { get; set; }
}

public class UrlValidationRules
{
    public List<string> AllowedProtocols { get; set; } = new() { "http", "https" };
    public List<string> AllowedDomains { get; set; } = new();
    public List<string> BlockedDomains { get; set; } = new();
    public bool RequireHttps { get; set; } = false;
    public bool AllowLocalhost { get; set; } = true;
    public bool AllowIpAddresses { get; set; } = false;
}

#endregion

#region Validation Configuration

public class ValidationConfiguration
{
    public bool EnableAutoValidation { get; set; } = true;
    public bool StrictMode { get; set; } = false;
    public bool LogValidationErrors { get; set; } = true;
    public bool ThrowOnValidationError { get; set; } = false;
    public int MaxInputLength { get; set; } = 10000;
    public int MaxCollectionSize { get; set; } = 1000;
    public Dictionary<string, string> CustomMessages { get; set; } = new();
}

#endregion

#region Pattern Models

public class ValidationPattern
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}

public static class CommonPatterns
{
    // Email patterns
    public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    
    // Phone patterns
    public const string PhoneUS = @"^\+?1?\d{10}$";
    public const string PhoneInternational = @"^\+?[1-9]\d{1,14}$";
    
    // Password patterns
    public const string PasswordStrong = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    
    // Username patterns
    public const string Username = @"^[a-zA-Z0-9_-]{3,16}$";
    
    // URL patterns
    public const string Url = @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$";
    
    // IP address patterns
    public const string IPv4 = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
    public const string IPv6 = @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4})$";
    
    // Credit card patterns
    public const string CreditCard = @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13})$";
    
    // Date patterns
    public const string DateISO = @"^\d{4}-\d{2}-\d{2}$";
    
    // SQL Injection patterns
    public const string SqlInjection = @"(\bOR\b|\bAND\b|\bUNION\b|\bSELECT\b|\bDROP\b|\bINSERT\b|\bUPDATE\b|\bDELETE\b|--|\/\*|\*\/|xp_|sp_)";
    
    // XSS patterns
    public const string XssScript = @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>";
    public const string XssEvent = @"on\w+\s*=";
    public const string XssJavascript = @"javascript:";
}

#endregion
