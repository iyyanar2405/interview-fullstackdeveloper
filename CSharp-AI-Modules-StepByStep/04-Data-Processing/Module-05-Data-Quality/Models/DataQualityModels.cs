namespace AI.DataQuality.Models;

// Enums
public enum DataQualityDimension
{
    Accuracy,
    Completeness,
    Consistency,
    Timeliness,
    Validity,
    Uniqueness,
    Integrity
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public enum AnomalyType
{
    Outlier,
    Missing,
    Duplicate,
    Format,
    Range,
    Statistical,
    Pattern
}

public enum ProfileType
{
    Column,
    Table,
    Dataset,
    Schema
}

public enum DataType
{
    String,
    Integer,
    Decimal,
    Boolean,
    DateTime,
    Json,
    Xml,
    Binary,
    Unknown
}

// Data Quality Models
public class DataQualityMetrics
{
    public string DatasetName { get; set; } = string.Empty;
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public double OverallScore { get; set; }
    public Dictionary<DataQualityDimension, double> DimensionScores { get; set; } = new();
    public long TotalRecords { get; set; }
    public long ValidRecords { get; set; }
    public long InvalidRecords { get; set; }
    public double CompletenessPercentage { get; set; }
    public double AccuracyPercentage { get; set; }
    public double ConsistencyPercentage { get; set; }
    public double UniquenessPercentage { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class DataQualityRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString();
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DataQualityDimension Dimension { get; set; }
    public ValidationSeverity Severity { get; set; }
    public string Expression { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public TimeSpan ValidationTime { get; set; }
}

public class ValidationError
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Value { get; set; }
    public int? RowNumber { get; set; }
}

public class ValidationWarning
{
    public string Message { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public int? RowNumber { get; set; }
}

// Data Profiling Models
public class DataProfile
{
    public string DatasetName { get; set; } = string.Empty;
    public ProfileType ProfileType { get; set; }
    public DateTime ProfileDate { get; set; } = DateTime.UtcNow;
    public long TotalRecords { get; set; }
    public int TotalColumns { get; set; }
    public List<ColumnProfile> Columns { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class ColumnProfile
{
    public string ColumnName { get; set; } = string.Empty;
    public DataType DataType { get; set; }
    public long TotalValues { get; set; }
    public long NullCount { get; set; }
    public long DistinctCount { get; set; }
    public double NullPercentage { get; set; }
    public double UniquenessRatio { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
    public object? MostCommonValue { get; set; }
    public int MostCommonValueCount { get; set; }
    public double? Mean { get; set; }
    public double? Median { get; set; }
    public double? StandardDeviation { get; set; }
    public List<string> SampleValues { get; set; } = new();
    public Dictionary<string, int> ValueDistribution { get; set; } = new();
}

// Anomaly Detection Models
public class AnomalyDetectionRequest
{
    public string DatasetName { get; set; } = string.Empty;
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<string> NumericColumns { get; set; } = new();
    public double Sensitivity { get; set; } = 0.95;
    public int WindowSize { get; set; } = 100;
}

public class AnomalyDetectionResult
{
    public string DatasetName { get; set; } = string.Empty;
    public DateTime DetectionDate { get; set; } = DateTime.UtcNow;
    public int TotalRecords { get; set; }
    public int AnomaliesDetected { get; set; }
    public double AnomalyPercentage { get; set; }
    public List<Anomaly> Anomalies { get; set; } = new();
}

public class Anomaly
{
    public string AnomalyId { get; set; } = Guid.NewGuid().ToString();
    public AnomalyType Type { get; set; }
    public int RowNumber { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public object? Value { get; set; }
    public object? ExpectedValue { get; set; }
    public double AnomalyScore { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

// Data Governance Models
public class DataGovernancePolicy
{
    public string PolicyId { get; set; } = Guid.NewGuid().ToString();
    public string PolicyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public List<DataQualityRule> Rules { get; set; } = new();
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ComplianceReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString();
    public string DatasetName { get; set; } = string.Empty;
    public string PolicyId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public bool IsCompliant { get; set; }
    public double ComplianceScore { get; set; }
    public List<ComplianceViolation> Violations { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ComplianceViolation
{
    public string ViolationId { get; set; } = Guid.NewGuid().ToString();
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public int AffectedRecords { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

// Data Quality Dashboard Models
public class DataQualityDashboard
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<DatasetQualitySummary> Datasets { get; set; } = new();
    public Dictionary<DataQualityDimension, double> OverallDimensionScores { get; set; } = new();
    public List<TrendPoint> QualityTrends { get; set; } = new();
    public List<TopIssue> TopIssues { get; set; } = new();
}

public class DatasetQualitySummary
{
    public string DatasetName { get; set; } = string.Empty;
    public double QualityScore { get; set; }
    public long RecordCount { get; set; }
    public int IssueCount { get; set; }
    public DateTime LastAssessment { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TrendPoint
{
    public DateTime Date { get; set; }
    public double QualityScore { get; set; }
    public long RecordCount { get; set; }
}

public class TopIssue
{
    public string IssueName { get; set; } = string.Empty;
    public int Occurrences { get; set; }
    public ValidationSeverity Severity { get; set; }
    public List<string> AffectedDatasets { get; set; } = new();
}

// Data Cleansing Models
public class DataCleansingRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<CleansingRule> Rules { get; set; } = new();
    public bool RemoveDuplicates { get; set; } = true;
    public bool FillMissingValues { get; set; } = true;
    public Dictionary<string, object> DefaultValues { get; set; } = new();
}

public class CleansingRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString();
    public string ColumnName { get; set; } = string.Empty;
    public CleansingOperation Operation { get; set; }
    public object? Parameter { get; set; }
}

public enum CleansingOperation
{
    Trim,
    UpperCase,
    LowerCase,
    RemoveSpecialCharacters,
    ReplaceValue,
    FillNull,
    RemoveDuplicates,
    FormatDate,
    Round,
    Standardize
}

public class DataCleansingResult
{
    public List<Dictionary<string, object>> CleanedData { get; set; } = new();
    public int OriginalRecordCount { get; set; }
    public int CleanedRecordCount { get; set; }
    public int RecordsRemoved { get; set; }
    public int RecordsModified { get; set; }
    public List<CleansingAction> ActionsApplied { get; set; } = new();
}

public class CleansingAction
{
    public string RuleId { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public CleansingOperation Operation { get; set; }
    public int RecordsAffected { get; set; }
}

// Schema Validation Models
public class SchemaDefinition
{
    public string SchemaName { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public List<FieldDefinition> Fields { get; set; } = new();
    public List<string> RequiredFields { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class FieldDefinition
{
    public string FieldName { get; set; } = string.Empty;
    public DataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUnique { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public List<object>? AllowedValues { get; set; }
    public string? Description { get; set; }
}

public class SchemaValidationResult
{
    public bool IsValid { get; set; }
    public List<SchemaViolation> Violations { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class SchemaViolation
{
    public string FieldName { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RowNumber { get; set; }
    public object? ActualValue { get; set; }
    public object? ExpectedValue { get; set; }
}

// API Response Model
public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public List<string> Warnings { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponseModel<T> SuccessResponse(T data)
    {
        return new ApiResponseModel<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponseModel<T> ErrorResponse(string error)
    {
        return new ApiResponseModel<T>
        {
            Success = false,
            Error = error
        };
    }
}
