namespace AI.ResponseProcessing.Models;

#region Enums

public enum ResponseFormat
{
    Json,
    Xml,
    Markdown,
    PlainText,
    Html,
    Yaml,
    Csv,
    Custom
}

public enum ValidationSeverity
{
    Error,
    Warning,
    Info
}

public enum ErrorHandlingStrategy
{
    Throw,
    ReturnDefault,
    Retry,
    Log,
    Fallback
}

public enum RateLimitStrategy
{
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    LeakyBucket,
    Adaptive
}

#endregion

#region Response Models

public class AIResponse
{
    public string RawContent { get; set; } = string.Empty;
    public ResponseFormat Format { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string ModelId { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public double ProcessingTimeMs { get; set; }
}

public class ParsedResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public List<ParsingError> Errors { get; set; } = new();
    public ResponseFormat OriginalFormat { get; set; }
    public Dictionary<string, object> ExtractedMetadata { get; set; } = new();
}

public class ParsingError
{
    public string Message { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string? Code { get; set; }
}

#endregion

#region Parsing Models

public class ParsingRequest
{
    public string Content { get; set; } = string.Empty;
    public ResponseFormat Format { get; set; }
    public ParsingOptions Options { get; set; } = new();
}

public class ParsingOptions
{
    public bool StrictMode { get; set; } = false;
    public bool ExtractCodeBlocks { get; set; } = true;
    public bool ExtractLinks { get; set; } = false;
    public bool ExtractTables { get; set; } = false;
    public bool TrimWhitespace { get; set; } = true;
    public bool RemoveComments { get; set; } = false;
    public Dictionary<string, string> CustomPatterns { get; set; } = new();
}

public class StructuredData
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Fields { get; set; } = new();
    public List<StructuredData> Children { get; set; } = new();
}

public class CodeBlock
{
    public string Language { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int LineNumber { get; set; }
}

public class ExtractedLink
{
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
}

public class ExtractedTable
{
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
}

#endregion

#region Validation Models

public class ValidationRequest
{
    public object Data { get; set; } = new();
    public List<ValidationRule> Rules { get; set; } = new();
    public bool StopOnFirstError { get; set; } = false;
}

public class ValidationRule
{
    public string FieldPath { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty; // Required, MinLength, MaxLength, Pattern, Custom
    public object? Value { get; set; }
    public string? ErrorMessage { get; set; }
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationError> Warnings { get; set; } = new();
    public Dictionary<string, object> ValidatedData { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public object? AttemptedValue { get; set; }
    public string? Code { get; set; }
}

#endregion

#region Output Formatting Models

public class FormattingRequest
{
    public object Data { get; set; } = new();
    public ResponseFormat TargetFormat { get; set; }
    public FormattingOptions Options { get; set; } = new();
}

public class FormattingOptions
{
    public bool PrettyPrint { get; set; } = true;
    public int IndentSize { get; set; } = 2;
    public bool IncludeMetadata { get; set; } = false;
    public bool EscapeHtml { get; set; } = true;
    public string? DateFormat { get; set; }
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class FormattedResponse
{
    public string Content { get; set; } = string.Empty;
    public ResponseFormat Format { get; set; }
    public int ContentLength { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}

#endregion

#region Error Handling Models

public class ErrorContext
{
    public Exception Exception { get; set; } = null!;
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? CorrelationId { get; set; }
}

public class ErrorHandlingConfig
{
    public ErrorHandlingStrategy Strategy { get; set; } = ErrorHandlingStrategy.Log;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public bool EnableCircuitBreaker { get; set; } = true;
    public List<Type> RetryableExceptions { get; set; } = new();
    public object? DefaultValue { get; set; }
}

public class ErrorHandlingResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public Exception? Error { get; set; }
    public int AttemptsCount { get; set; }
    public ErrorHandlingStrategy StrategyUsed { get; set; }
}

#endregion

#region Rate Limiting Models

public class RateLimitConfig
{
    public RateLimitStrategy Strategy { get; set; } = RateLimitStrategy.SlidingWindow;
    public int MaxRequests { get; set; } = 100;
    public TimeSpan TimeWindow { get; set; } = TimeSpan.FromMinutes(1);
    public int BurstSize { get; set; } = 10;
    public bool EnablePerUser { get; set; } = true;
    public bool EnablePerEndpoint { get; set; } = false;
}

public class RateLimitRequest
{
    public string? UserId { get; set; }
    public string? Endpoint { get; set; }
    public int Weight { get; set; } = 1;
}

public class RateLimitResult
{
    public bool Allowed { get; set; }
    public int RemainingRequests { get; set; }
    public DateTime ResetTime { get; set; }
    public TimeSpan RetryAfter { get; set; }
    public string? RateLimitKey { get; set; }
}

public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime ResetTime { get; set; }
    public TimeSpan WindowDuration { get; set; }
}

#endregion

#region Retry Models

public class RetryPolicy
{
    public int MaxAttempts { get; set; } = 3;
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);
    public double BackoffMultiplier { get; set; } = 2.0;
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
    public List<Type> RetryableExceptions { get; set; } = new();
    public Func<Exception, bool>? ShouldRetry { get; set; }
}

public class RetryResult<T>
{
    public bool Success { get; set; }
    public T? Result { get; set; }
    public int AttemptsMade { get; set; }
    public Exception? LastException { get; set; }
    public TimeSpan TotalDuration { get; set; }
}

#endregion

#region Sanitization Models

public class SanitizationRequest
{
    public string Content { get; set; } = string.Empty;
    public SanitizationOptions Options { get; set; } = new();
}

public class SanitizationOptions
{
    public bool RemoveHtmlTags { get; set; } = true;
    public bool RemoveScripts { get; set; } = true;
    public bool RemoveSqlInjection { get; set; } = true;
    public bool RemoveXss { get; set; } = true;
    public bool TrimWhitespace { get; set; } = true;
    public List<string> AllowedHtmlTags { get; set; } = new();
    public List<string> BlockedPatterns { get; set; } = new();
}

public class SanitizedResponse
{
    public string CleanContent { get; set; } = string.Empty;
    public List<string> RemovedElements { get; set; } = new();
    public bool WasModified { get; set; }
}

#endregion

#region Transformation Models

public class TransformationRequest
{
    public object Input { get; set; } = new();
    public string TransformationType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class TransformationResult<T>
{
    public T? Output { get; set; }
    public bool Success { get; set; }
    public string TransformationType { get; set; } = string.Empty;
    public List<string> AppliedTransformations { get; set; } = new();
}

#endregion

#region Configuration Models

public class ResponseProcessingSettings
{
    public ParsingOptions DefaultParsingOptions { get; set; } = new();
    public FormattingOptions DefaultFormattingOptions { get; set; } = new();
    public ErrorHandlingConfig ErrorHandling { get; set; } = new();
    public RateLimitConfig RateLimiting { get; set; } = new();
    public RetryPolicy RetryPolicy { get; set; } = new();
    public SanitizationOptions Sanitization { get; set; } = new();
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 10;
}

#endregion

#region Batch Processing Models

public class BatchProcessingRequest<T>
{
    public List<T> Items { get; set; } = new();
    public int BatchSize { get; set; } = 10;
    public bool ContinueOnError { get; set; } = true;
    public int MaxParallelism { get; set; } = 4;
}

public class BatchProcessingResult<TInput, TOutput>
{
    public List<BatchItemResult<TInput, TOutput>> Results { get; set; } = new();
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public TimeSpan TotalDuration { get; set; }
}

public class BatchItemResult<TInput, TOutput>
{
    public TInput Input { get; set; } = default!;
    public TOutput? Output { get; set; }
    public bool Success { get; set; }
    public Exception? Error { get; set; }
    public TimeSpan Duration { get; set; }
}

#endregion
