namespace Module_03_Content_Safety.Models;

/// <summary>
/// Represents the result of content safety analysis
/// </summary>
public class ContentSafetyResult
{
    public bool IsSafe { get; set; }
    public double OverallRiskScore { get; set; } // 0-10 scale
    public List<SafetyViolation> Violations { get; set; } = new();
    public string? RecommendedAction { get; set; }
    public Dictionary<string, double> CategoryScores { get; set; } = new();
    public string? ProcessedContent { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a safety violation detected in content
/// </summary>
public class SafetyViolation
{
    public ViolationType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Severity { get; set; } // 0-10 scale
    public string Description { get; set; } = string.Empty;
    public string? DetectedContent { get; set; }
    public int? Position { get; set; }
    public string? RecommendedAction { get; set; }
}

/// <summary>
/// Types of safety violations
/// </summary>
public enum ViolationType
{
    Toxicity,
    HateSpeech,
    Violence,
    SelfHarm,
    Sexual,
    PII,
    PromptInjection,
    Profanity,
    Spam,
    Malware,
    Phishing,
    Misinformation
}

/// <summary>
/// Toxicity detection result
/// </summary>
public class ToxicityResult
{
    public bool IsToxic { get; set; }
    public double ToxicityScore { get; set; } // 0-1 scale
    public Dictionary<ToxicityCategory, double> CategoryScores { get; set; } = new();
    public List<ToxicPhrase> DetectedPhrases { get; set; } = new();
    public string? Reason { get; set; }
    public string? SanitizedContent { get; set; }
}

/// <summary>
/// Toxicity categories
/// </summary>
public enum ToxicityCategory
{
    Toxic,
    SevereToxic,
    Obscene,
    Threat,
    Insult,
    IdentityHate,
    Sexual,
    Violence
}

/// <summary>
/// Represents a detected toxic phrase
/// </summary>
public class ToxicPhrase
{
    public string Phrase { get; set; } = string.Empty;
    public ToxicityCategory Category { get; set; }
    public double Confidence { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
}

/// <summary>
/// PII detection result
/// </summary>
public class PiiDetectionResult
{
    public bool ContainsPii { get; set; }
    public List<PiiEntity> DetectedEntities { get; set; } = new();
    public string RedactedContent { get; set; } = string.Empty;
    public string MaskedContent { get; set; } = string.Empty;
    public int PiiCount { get; set; }
    public Dictionary<PiiType, int> PiiTypeCount { get; set; } = new();
}

/// <summary>
/// Represents a detected PII entity
/// </summary>
public class PiiEntity
{
    public PiiType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string RedactedValue { get; set; } = string.Empty;
    public string? Context { get; set; }
}

/// <summary>
/// Types of PII
/// </summary>
public enum PiiType
{
    EmailAddress,
    PhoneNumber,
    SocialSecurityNumber,
    CreditCardNumber,
    IpAddress,
    MacAddress,
    DriverLicense,
    Passport,
    BankAccount,
    DateOfBirth,
    Address,
    PersonName,
    MedicalRecordNumber,
    VehicleIdentificationNumber,
    TaxId,
    Username,
    Password,
    ApiKey,
    Url,
    Coordinate
}

/// <summary>
/// Prompt injection detection result
/// </summary>
public class PromptInjectionResult
{
    public bool IsInjectionDetected { get; set; }
    public double RiskScore { get; set; } // 0-10 scale
    public List<InjectionPattern> DetectedPatterns { get; set; } = new();
    public InjectionType InjectionType { get; set; }
    public string? Explanation { get; set; }
    public string? SafeAlternative { get; set; }
    public List<string> BlockedKeywords { get; set; } = new();
}

/// <summary>
/// Types of prompt injection
/// </summary>
public enum InjectionType
{
    None,
    DirectInjection,
    IndirectInjection,
    Jailbreak,
    SystemPromptManipulation,
    RoleManipulation,
    ContextEscape,
    PayloadInjection,
    ChainedInjection
}

/// <summary>
/// Represents a detected injection pattern
/// </summary>
public class InjectionPattern
{
    public string Pattern { get; set; } = string.Empty;
    public InjectionType Type { get; set; }
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
}

/// <summary>
/// Content moderation result
/// </summary>
public class ModerationResult
{
    public bool RequiresModeration { get; set; }
    public ModerationDecision Decision { get; set; }
    public double ConfidenceScore { get; set; }
    public List<ModerationFlag> Flags { get; set; } = new();
    public string? ModeratorNotes { get; set; }
    public string? FilteredContent { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Moderation decision
/// </summary>
public enum ModerationDecision
{
    Approve,
    Review,
    Reject,
    AutoModerate,
    Block
}

/// <summary>
/// Represents a moderation flag
/// </summary>
public class ModerationFlag
{
    public string Category { get; set; } = string.Empty;
    public double Severity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public bool AutoModerated { get; set; }
}

/// <summary>
/// Output filtering result
/// </summary>
public class OutputFilterResult
{
    public string FilteredContent { get; set; } = string.Empty;
    public bool WasFiltered { get; set; }
    public List<FilterAction> ActionsApplied { get; set; } = new();
    public int RemovedSensitiveDataCount { get; set; }
    public Dictionary<string, int> FilterStats { get; set; } = new();
}

/// <summary>
/// Represents a filter action applied
/// </summary>
public class FilterAction
{
    public FilterActionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? OriginalContent { get; set; }
    public string? FilteredContent { get; set; }
    public int Position { get; set; }
}

/// <summary>
/// Types of filter actions
/// </summary>
public enum FilterActionType
{
    PiiRedaction,
    ToxicityRemoval,
    ProfanityFilter,
    SensitiveDataRemoval,
    LinkRemoval,
    ScriptRemoval,
    HtmlSanitization,
    CharacterLimiting
}

/// <summary>
/// Request for content analysis
/// </summary>
public class ContentAnalysisRequest
{
    public string Content { get; set; } = string.Empty;
    public string? Language { get; set; } = "en";
    public List<string>? Categories { get; set; }
    public bool IncludePiiDetection { get; set; } = true;
    public bool IncludeToxicityDetection { get; set; } = true;
    public bool IncludePromptInjectionDetection { get; set; } = false;
    public Dictionary<string, object>? Context { get; set; }
}

/// <summary>
/// Request for content moderation
/// </summary>
public class ModerationRequest
{
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text"; // text, image, url
    public string? UserId { get; set; }
    public string? Source { get; set; }
    public ModerationMode Mode { get; set; } = ModerationMode.Standard;
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Moderation modes
/// </summary>
public enum ModerationMode
{
    Standard,
    Strict,
    Lenient,
    Custom
}

/// <summary>
/// Request for PII detection
/// </summary>
public class PiiDetectionRequest
{
    public string Content { get; set; } = string.Empty;
    public List<PiiType>? PiiTypes { get; set; }
    public RedactionStrategy RedactionStrategy { get; set; } = RedactionStrategy.Replace;
    public string RedactionCharacter { get; set; } = "*";
    public bool ReturnOriginalPositions { get; set; } = true;
}

/// <summary>
/// PII redaction strategies
/// </summary>
public enum RedactionStrategy
{
    Replace,
    Mask,
    Hash,
    Encrypt,
    Remove,
    Pseudonymize
}

/// <summary>
/// Request for toxicity detection
/// </summary>
public class ToxicityDetectionRequest
{
    public string Content { get; set; } = string.Empty;
    public List<ToxicityCategory>? Categories { get; set; }
    public double Threshold { get; set; } = 0.5;
    public bool ReturnSanitized { get; set; } = true;
    public bool IncludeReasons { get; set; } = true;
}

/// <summary>
/// Request for prompt injection detection
/// </summary>
public class PromptInjectionRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? SystemPrompt { get; set; }
    public string? Context { get; set; }
    public double Threshold { get; set; } = 5.0;
    public bool StrictMode { get; set; } = false;
}

/// <summary>
/// Request for output filtering
/// </summary>
public class OutputFilterRequest
{
    public string Content { get; set; } = string.Empty;
    public List<FilterActionType>? FilterTypes { get; set; }
    public bool RemovePii { get; set; } = true;
    public bool RemoveToxicity { get; set; } = true;
    public bool SanitizeHtml { get; set; } = true;
    public int? MaxLength { get; set; }
}

/// <summary>
/// Content safety configuration
/// </summary>
public class ContentSafetyConfiguration
{
    public ToxicityThresholds Toxicity { get; set; } = new();
    public PiiConfiguration Pii { get; set; } = new();
    public PromptInjectionConfiguration PromptInjection { get; set; } = new();
    public ModerationConfiguration Moderation { get; set; } = new();
    public OutputFilterConfiguration OutputFilter { get; set; } = new();
}

/// <summary>
/// Toxicity detection thresholds
/// </summary>
public class ToxicityThresholds
{
    public double LowThreshold { get; set; } = 0.3;
    public double MediumThreshold { get; set; } = 0.6;
    public double HighThreshold { get; set; } = 0.8;
    public Dictionary<ToxicityCategory, double> CategoryThresholds { get; set; } = new();
}

/// <summary>
/// PII detection configuration
/// </summary>
public class PiiConfiguration
{
    public List<PiiType> EnabledTypes { get; set; } = new();
    public RedactionStrategy DefaultRedactionStrategy { get; set; } = RedactionStrategy.Replace;
    public bool StrictMode { get; set; } = false;
    public Dictionary<PiiType, string> RedactionTemplates { get; set; } = new();
}

/// <summary>
/// Prompt injection detection configuration
/// </summary>
public class PromptInjectionConfiguration
{
    public double RiskThreshold { get; set; } = 5.0;
    public bool EnableJailbreakDetection { get; set; } = true;
    public bool EnableSystemPromptProtection { get; set; } = true;
    public List<string> BlockedKeywords { get; set; } = new();
    public List<string> BlockedPatterns { get; set; } = new();
}

/// <summary>
/// Content moderation configuration
/// </summary>
public class ModerationConfiguration
{
    public ModerationMode DefaultMode { get; set; } = ModerationMode.Standard;
    public bool EnableAutoModeration { get; set; } = true;
    public Dictionary<string, double> CategoryThresholds { get; set; } = new();
    public List<string> AllowedDomains { get; set; } = new();
    public List<string> BlockedDomains { get; set; } = new();
}

/// <summary>
/// Output filter configuration
/// </summary>
public class OutputFilterConfiguration
{
    public bool EnablePiiFiltering { get; set; } = true;
    public bool EnableToxicityFiltering { get; set; } = true;
    public bool EnableHtmlSanitization { get; set; } = true;
    public int MaxOutputLength { get; set; } = 10000;
    public List<string> AllowedHtmlTags { get; set; } = new();
}

/// <summary>
/// Common safety patterns
/// </summary>
public static class SafetyPatterns
{
    // Prompt injection patterns
    public static readonly List<string> PromptInjectionPatterns = new()
    {
        @"ignore\s+(previous|all|above)\s+(instructions|prompts?|directions)",
        @"disregard\s+(previous|all|above)",
        @"forget\s+(everything|previous|instructions)",
        @"new\s+instructions?:",
        @"system\s*:\s*you\s+are",
        @"act\s+as\s+(if|though)",
        @"pretend\s+(you\s+are|to\s+be)",
        @"roleplay\s+as",
        @"\[SYSTEM\]|\[INST\]|\[/INST\]",
        @"<\|.*?\|>",
        @"jailbreak",
        @"bypass\s+(filter|restriction|safety)",
        @"override\s+(protocol|safety|rules)"
    };

    // Jailbreak patterns
    public static readonly List<string> JailbreakPatterns = new()
    {
        "DAN mode",
        "Developer mode",
        "Evil mode",
        "Unrestricted mode",
        "Unfiltered",
        "No rules",
        "No restrictions",
        "Do anything now"
    };

    // PII patterns
    public static readonly Dictionary<PiiType, string> PiiRegexPatterns = new()
    {
        { PiiType.EmailAddress, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b" },
        { PiiType.PhoneNumber, @"\b(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}\b" },
        { PiiType.SocialSecurityNumber, @"\b\d{3}-\d{2}-\d{4}\b" },
        { PiiType.CreditCardNumber, @"\b\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}\b" },
        { PiiType.IpAddress, @"\b(?:\d{1,3}\.){3}\d{1,3}\b" },
        { PiiType.Url, @"https?://[^\s]+" }
    };

    // Profanity word list (sample)
    public static readonly List<string> ProfanityList = new()
    {
        // Add profanity words here
        // This is a placeholder - use a proper profanity filter library
    };
}
