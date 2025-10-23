namespace AI.ETL.Models;

// Enums
public enum ETLJobStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled,
    PartiallyCompleted
}

public enum DataSourceType
{
    SqlServer,
    PostgreSQL,
    MongoDB,
    CsvFile,
    ExcelFile,
    JsonFile,
    XmlFile,
    RestApi,
    AzureBlob,
    AwsS3,
    FileSystem
}

public enum TransformationType
{
    Map,
    Filter,
    Aggregate,
    Join,
    Split,
    Merge,
    Validate,
    Cleanse,
    Enrich,
    Custom
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

// ETL Job Models
public class ETLJob
{
    public string JobId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ETLJobStatus Status { get; set; } = ETLJobStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public ETLConfiguration Configuration { get; set; } = new();
}

public class ETLConfiguration
{
    public DataSourceConfig Source { get; set; } = new();
    public DataSourceConfig Destination { get; set; } = new();
    public List<TransformationStep> Transformations { get; set; } = new();
    public ValidationConfig Validation { get; set; } = new();
    public ErrorHandlingConfig ErrorHandling { get; set; } = new();
    public PerformanceConfig Performance { get; set; } = new();
}

// Data Source Configuration
public class DataSourceConfig
{
    public DataSourceType Type { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new();
    public string Query { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

// Transformation Models
public class TransformationStep
{
    public string StepId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public TransformationType Type { get; set; }
    public int Order { get; set; }
    public bool Enabled { get; set; } = true;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string? CustomScript { get; set; }
}

public class TransformationResult
{
    public bool Success { get; set; }
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public int RecordsProcessed { get; set; }
    public int RecordsFailed { get; set; }
    public List<TransformationError> Errors { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class TransformationError
{
    public int RecordIndex { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public Dictionary<string, object> RecordData { get; set; } = new();
}

// Validation Models
public class ValidationConfig
{
    public bool Enabled { get; set; } = true;
    public List<ValidationRule> Rules { get; set; } = new();
    public bool FailOnError { get; set; } = true;
    public bool FailOnWarning { get; set; } = false;
    public int MaxErrors { get; set; } = 100;
}

public class ValidationRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty; // Required, Range, Pattern, Custom
    public Dictionary<string, object> Parameters { get; set; } = new();
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    public string? CustomValidator { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationError> Warnings { get; set; } = new();
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
}

public class ValidationError
{
    public int RecordIndex { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public object? ActualValue { get; set; }
    public object? ExpectedValue { get; set; }
}

// Error Handling Models
public class ErrorHandlingConfig
{
    public bool ContinueOnError { get; set; } = false;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 5;
    public bool LogErrors { get; set; } = true;
    public string? ErrorLogPath { get; set; }
    public bool CreateErrorFile { get; set; } = true;
}

// Performance Configuration
public class PerformanceConfig
{
    public int BatchSize { get; set; } = 1000;
    public int MaxDegreeOfParallelism { get; set; } = 4;
    public bool UseParallelProcessing { get; set; } = true;
    public int MemoryLimitMB { get; set; } = 1024;
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 60;
}

// Schema Mapping Models
public class SchemaMapping
{
    public string MappingId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> SourceSchema { get; set; } = new(); // FieldName -> DataType
    public Dictionary<string, string> DestinationSchema { get; set; } = new();
    public List<FieldMapping> FieldMappings { get; set; } = new();
}

public class FieldMapping
{
    public string SourceField { get; set; } = string.Empty;
    public string DestinationField { get; set; } = string.Empty;
    public string? DataType { get; set; }
    public string? DefaultValue { get; set; }
    public string? TransformExpression { get; set; }
    public bool IsRequired { get; set; }
    public bool AllowNull { get; set; } = true;
}

// Data Cleansing Models
public class DataCleansingRule
{
    public string RuleId { get; set; } = Guid.NewGuid().ToString();
    public string FieldName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // Trim, Replace, RemoveSpecialChars, etc.
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class DataCleansingResult
{
    public bool Success { get; set; }
    public int RecordsCleansed { get; set; }
    public int FieldsModified { get; set; }
    public List<string> ModifiedFields { get; set; } = new();
    public Dictionary<string, int> OperationCounts { get; set; } = new();
}

// Pipeline Execution Models
public class PipelineExecution
{
    public string ExecutionId { get; set; } = Guid.NewGuid().ToString();
    public string JobId { get; set; } = string.Empty;
    public ETLJobStatus Status { get; set; } = ETLJobStatus.Pending;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public List<StageExecution> Stages { get; set; } = new();
    public PipelineMetrics Metrics { get; set; } = new();
}

public class StageExecution
{
    public string StageId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public ETLJobStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsFailed { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PipelineMetrics
{
    public int TotalRecordsExtracted { get; set; }
    public int TotalRecordsTransformed { get; set; }
    public int TotalRecordsLoaded { get; set; }
    public int TotalRecordsFailed { get; set; }
    public double RecordsPerSecond { get; set; }
    public TimeSpan ExtractionTime { get; set; }
    public TimeSpan TransformationTime { get; set; }
    public TimeSpan LoadTime { get; set; }
    public long MemoryUsedMB { get; set; }
}

// Request/Response Models
public class ETLJobRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ETLConfiguration Configuration { get; set; } = new();
}

public class ETLJobResponse
{
    public string JobId { get; set; } = string.Empty;
    public ETLJobStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ExecutePipelineRequest
{
    public string JobId { get; set; } = string.Empty;
    public Dictionary<string, object>? RuntimeParameters { get; set; }
    public bool AsyncExecution { get; set; } = true;
}

public class ExecutePipelineResponse
{
    public string ExecutionId { get; set; } = string.Empty;
    public ETLJobStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public PipelineMetrics? Metrics { get; set; }
}

public class DataQualityReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString();
    public string JobId { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public double QualityScore { get; set; }
    public Dictionary<string, FieldQualityMetrics> FieldMetrics { get; set; } = new();
    public List<ValidationError> TopIssues { get; set; } = new();
}

public class FieldQualityMetrics
{
    public string FieldName { get; set; } = string.Empty;
    public int TotalValues { get; set; }
    public int NullValues { get; set; }
    public int UniqueValues { get; set; }
    public int DuplicateValues { get; set; }
    public double CompletenessPercentage { get; set; }
    public double UniquenessPercentage { get; set; }
}

// API Response Model
public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
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
