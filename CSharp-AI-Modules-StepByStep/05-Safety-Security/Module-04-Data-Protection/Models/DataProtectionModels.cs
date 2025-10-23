namespace Module_04_Data_Protection.Models;

/// <summary>
/// Encryption result
/// </summary>
public class EncryptionResult
{
    public string EncryptedData { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public string? InitializationVector { get; set; }
    public string? KeyId { get; set; }
    public DateTime EncryptedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Decryption result
/// </summary>
public class DecryptionResult
{
    public string DecryptedData { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
    public DateTime DecryptedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Encryption request
/// </summary>
public class EncryptionRequest
{
    public string Data { get; set; } = string.Empty;
    public EncryptionAlgorithm Algorithm { get; set; } = EncryptionAlgorithm.AES256;
    public string? KeyId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Decryption request
/// </summary>
public class DecryptionRequest
{
    public string EncryptedData { get; set; } = string.Empty;
    public string? InitializationVector { get; set; }
    public string? KeyId { get; set; }
    public EncryptionAlgorithm Algorithm { get; set; } = EncryptionAlgorithm.AES256;
}

/// <summary>
/// Encryption algorithms
/// </summary>
public enum EncryptionAlgorithm
{
    AES128,
    AES192,
    AES256,
    RSA2048,
    RSA4096,
    ChaCha20
}

/// <summary>
/// Key management result
/// </summary>
public class KeyManagementResult
{
    public string KeyId { get; set; } = string.Empty;
    public string KeyType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public KeyStatus Status { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Key status
/// </summary>
public enum KeyStatus
{
    Active,
    Expired,
    Revoked,
    Pending,
    Disabled
}

/// <summary>
/// Key rotation result
/// </summary>
public class KeyRotationResult
{
    public string OldKeyId { get; set; } = string.Empty;
    public string NewKeyId { get; set; } = string.Empty;
    public DateTime RotatedAt { get; set; } = DateTime.UtcNow;
    public int ReencryptedRecordsCount { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// GDPR data subject
/// </summary>
public class DataSubject
{
    public string SubjectId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public Dictionary<string, object> PersonalData { get; set; } = new();
    public List<ConsentRecord> Consents { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAccessedAt { get; set; }
}

/// <summary>
/// Consent record
/// </summary>
public class ConsentRecord
{
    public string ConsentId { get; set; } = Guid.NewGuid().ToString();
    public string Purpose { get; set; } = string.Empty;
    public bool Granted { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// GDPR request types
/// </summary>
public enum GdprRequestType
{
    DataAccess,
    DataErasure,
    DataPortability,
    DataRectification,
    ConsentWithdrawal,
    ProcessingRestriction
}

/// <summary>
/// GDPR request
/// </summary>
public class GdprRequest
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string SubjectId { get; set; } = string.Empty;
    public GdprRequestType RequestType { get; set; }
    public string? Reason { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public GdprRequestStatus Status { get; set; } = GdprRequestStatus.Pending;
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// GDPR request status
/// </summary>
public enum GdprRequestStatus
{
    Pending,
    InProgress,
    Completed,
    Rejected,
    PartiallyCompleted
}

/// <summary>
/// GDPR data export
/// </summary>
public class GdprDataExport
{
    public string SubjectId { get; set; } = string.Empty;
    public Dictionary<string, object> PersonalData { get; set; } = new();
    public List<ConsentRecord> Consents { get; set; } = new();
    public List<AuditLogEntry> ActivityLog { get; set; } = new();
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public string Format { get; set; } = "JSON";
}

/// <summary>
/// Anonymization result
/// </summary>
public class AnonymizationResult
{
    public string AnonymizedData { get; set; } = string.Empty;
    public AnonymizationMethod Method { get; set; }
    public Dictionary<string, string> FieldMappings { get; set; } = new();
    public int FieldsAnonymized { get; set; }
    public bool Reversible { get; set; }
    public DateTime AnonymizedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Anonymization methods
/// </summary>
public enum AnonymizationMethod
{
    Masking,
    Pseudonymization,
    Generalization,
    Perturbation,
    Suppression,
    KAnonymity,
    Tokenization
}

/// <summary>
/// Anonymization request
/// </summary>
public class AnonymizationRequest
{
    public string Data { get; set; } = string.Empty;
    public AnonymizationMethod Method { get; set; } = AnonymizationMethod.Pseudonymization;
    public List<string>? FieldsToAnonymize { get; set; }
    public Dictionary<string, string>? CustomRules { get; set; }
    public bool Reversible { get; set; } = false;
}

/// <summary>
/// Audit log entry
/// </summary>
public class AuditLogEntry
{
    public string LogId { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? ResourceId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public AuditSeverity Severity { get; set; } = AuditSeverity.Information;
    public bool Success { get; set; } = true;
    public string? Hash { get; set; }
}

/// <summary>
/// Audit severity levels
/// </summary>
public enum AuditSeverity
{
    Debug,
    Information,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Audit query request
/// </summary>
public class AuditQueryRequest
{
    public string? UserId { get; set; }
    public string? EventType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AuditSeverity? MinimumSeverity { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// Audit query result
/// </summary>
public class AuditQueryResult
{
    public List<AuditLogEntry> Entries { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Data retention policy
/// </summary>
public class DataRetentionPolicy
{
    public string PolicyId { get; set; } = Guid.NewGuid().ToString();
    public string DataType { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public bool AutoDelete { get; set; } = true;
    public RetentionAction Action { get; set; } = RetentionAction.Delete;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Retention actions
/// </summary>
public enum RetentionAction
{
    Delete,
    Archive,
    Anonymize,
    Encrypt
}

/// <summary>
/// Data classification
/// </summary>
public class DataClassification
{
    public string DataId { get; set; } = string.Empty;
    public ClassificationLevel Level { get; set; }
    public List<string> Categories { get; set; } = new();
    public bool ContainsPii { get; set; }
    public bool ContainsSensitiveData { get; set; }
    public Dictionary<string, object> Tags { get; set; } = new();
}

/// <summary>
/// Classification levels
/// </summary>
public enum ClassificationLevel
{
    Public,
    Internal,
    Confidential,
    Restricted,
    TopSecret
}

/// <summary>
/// Encryption configuration
/// </summary>
public class EncryptionConfiguration
{
    public EncryptionAlgorithm DefaultAlgorithm { get; set; } = EncryptionAlgorithm.AES256;
    public int KeyRotationDays { get; set; } = 90;
    public bool EncryptAtRest { get; set; } = true;
    public bool EncryptInTransit { get; set; } = true;
    public string? KeyVaultUrl { get; set; }
    public Dictionary<string, string> AlgorithmSettings { get; set; } = new();
}

/// <summary>
/// GDPR configuration
/// </summary>
public class GdprConfiguration
{
    public int DataRetentionDays { get; set; } = 365;
    public int RequestResponseDays { get; set; } = 30;
    public bool EnableConsentTracking { get; set; } = true;
    public bool EnableDataPortability { get; set; } = true;
    public List<string> RequiredConsents { get; set; } = new();
    public string DataProtectionOfficerEmail { get; set; } = string.Empty;
}

/// <summary>
/// Compliance report
/// </summary>
public class ComplianceReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ComplianceFramework Framework { get; set; }
    public Dictionary<string, ComplianceStatus> Controls { get; set; } = new();
    public List<ComplianceIssue> Issues { get; set; } = new();
    public double ComplianceScore { get; set; }
}

/// <summary>
/// Compliance frameworks
/// </summary>
public enum ComplianceFramework
{
    GDPR,
    CCPA,
    HIPAA,
    SOC2,
    ISO27001,
    PCI_DSS
}

/// <summary>
/// Compliance status
/// </summary>
public enum ComplianceStatus
{
    Compliant,
    PartiallyCompliant,
    NonCompliant,
    NotApplicable
}

/// <summary>
/// Compliance issue
/// </summary>
public class ComplianceIssue
{
    public string IssueId { get; set; } = Guid.NewGuid().ToString();
    public string Control { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public DateTime IdentifiedAt { get; set; } = DateTime.UtcNow;
    public string? Remediation { get; set; }
    public IssueStatus Status { get; set; } = IssueStatus.Open;
}

/// <summary>
/// Issue severity
/// </summary>
public enum IssueSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Issue status
/// </summary>
public enum IssueStatus
{
    Open,
    InProgress,
    Resolved,
    Accepted
}

/// <summary>
/// Data breach notification
/// </summary>
public class DataBreachNotification
{
    public string BreachId { get; set; } = Guid.NewGuid().ToString();
    public DateTime OccurredAt { get; set; }
    public DateTime DiscoveredAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public int AffectedSubjects { get; set; }
    public List<string> DataTypes { get; set; } = new();
    public BreachSeverity Severity { get; set; }
    public bool AuthoritiesNotified { get; set; }
    public bool SubjectsNotified { get; set; }
    public string? MitigationSteps { get; set; }
}

/// <summary>
/// Breach severity
/// </summary>
public enum BreachSeverity
{
    Minor,
    Moderate,
    Serious,
    Critical
}

/// <summary>
/// Hash result
/// </summary>
public class HashResult
{
    public string Hash { get; set; } = string.Empty;
    public HashAlgorithm Algorithm { get; set; }
    public string? Salt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Hash algorithms
/// </summary>
public enum HashAlgorithm
{
    SHA256,
    SHA512,
    BCrypt,
    Argon2,
    PBKDF2
}

/// <summary>
/// Secure delete result
/// </summary>
public class SecureDeleteResult
{
    public string ResourceId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int OverwritePasses { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    public string? AuditLogId { get; set; }
}

/// <summary>
/// Key backup result
/// </summary>
public class KeyBackupResult
{
    public string BackupId { get; set; } = Guid.NewGuid().ToString();
    public string KeyId { get; set; } = string.Empty;
    public string BackupLocation { get; set; } = string.Empty;
    public DateTime BackedUpAt { get; set; } = DateTime.UtcNow;
    public bool Encrypted { get; set; } = true;
}

/// <summary>
/// Access control policy
/// </summary>
public class AccessControlPolicy
{
    public string PolicyId { get; set; } = Guid.NewGuid().ToString();
    public string ResourceType { get; set; } = string.Empty;
    public List<string> AllowedRoles { get; set; } = new();
    public List<string> DeniedRoles { get; set; } = new();
    public Dictionary<string, List<string>> Permissions { get; set; } = new();
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
}
