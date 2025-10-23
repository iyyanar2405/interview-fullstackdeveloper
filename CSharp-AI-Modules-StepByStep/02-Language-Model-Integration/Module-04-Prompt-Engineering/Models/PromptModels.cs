using System.Text.Json.Serialization;

namespace AI.PromptEngineering.Models;

/// <summary>
/// Prompt engineering settings
/// </summary>
public class PromptEngineeringSettings
{
    public string TemplateStoragePath { get; set; } = "Templates";
    public string DefaultTemplateEngine { get; set; } = "Scriban";
    public bool EnableCaching { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 60;
    public bool EnableOptimization { get; set; } = true;
    public bool EnableVersioning { get; set; } = true;
}

/// <summary>
/// Prompt template model
/// </summary>
public class PromptTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public TemplateEngine Engine { get; set; } = TemplateEngine.Scriban;
    public List<TemplateVariable> Variables { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Template engine types
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TemplateEngine
{
    Scriban,
    Handlebars,
    Simple
}

/// <summary>
/// Template variable definition
/// </summary>
public class TemplateVariable
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public string Description { get; set; } = string.Empty;
    public bool Required { get; set; } = true;
    public string? DefaultValue { get; set; }
    public List<string>? AllowedValues { get; set; }
    public string? ValidationPattern { get; set; }
}

/// <summary>
/// Prompt template request
/// </summary>
public class PromptTemplateRequest
{
    public string TemplateId { get; set; } = string.Empty;
    public Dictionary<string, object> Variables { get; set; } = new();
    public PromptOptimizationOptions? OptimizationOptions { get; set; }
}

/// <summary>
/// Prompt template response
/// </summary>
public class PromptTemplateResponse
{
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string RenderedPrompt { get; set; } = string.Empty;
    public Dictionary<string, object> UsedVariables { get; set; } = new();
    public int TokenCount { get; set; }
    public double EstimatedCost { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public PromptMetrics? Metrics { get; set; }
}

/// <summary>
/// Few-shot learning example
/// </summary>
public class FewShotExample
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public double Quality { get; set; } = 1.0;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Few-shot learning configuration
/// </summary>
public class FewShotConfig
{
    public string TaskDescription { get; set; } = string.Empty;
    public List<FewShotExample> Examples { get; set; } = new();
    public int MaxExamples { get; set; } = 5;
    public ExampleSelectionStrategy SelectionStrategy { get; set; } = ExampleSelectionStrategy.MostRelevant;
    public bool ShuffleExamples { get; set; } = false;
    public string? ExampleFormat { get; set; }
}

/// <summary>
/// Example selection strategy
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExampleSelectionStrategy
{
    MostRelevant,
    MostRecent,
    HighestQuality,
    Random,
    Diverse
}

/// <summary>
/// Few-shot prompt request
/// </summary>
public class FewShotPromptRequest
{
    public string TaskDescription { get; set; } = string.Empty;
    public string UserInput { get; set; } = string.Empty;
    public List<FewShotExample>? Examples { get; set; }
    public FewShotConfig? Config { get; set; }
}

/// <summary>
/// Few-shot prompt response
/// </summary>
public class FewShotPromptResponse
{
    public string Prompt { get; set; } = string.Empty;
    public List<FewShotExample> SelectedExamples { get; set; } = new();
    public int ExampleCount { get; set; }
    public int TokenCount { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Chain-of-thought step
/// </summary>
public class ChainOfThoughtStep
{
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Result { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

/// <summary>
/// Chain-of-thought configuration
/// </summary>
public class ChainOfThoughtConfig
{
    public string Problem { get; set; } = string.Empty;
    public List<string>? GuidingQuestions { get; set; }
    public bool IncludeExplanations { get; set; } = true;
    public bool ShowIntermediateSteps { get; set; } = true;
    public int MaxSteps { get; set; } = 10;
    public ChainOfThoughtStyle Style { get; set; } = ChainOfThoughtStyle.Detailed;
}

/// <summary>
/// Chain-of-thought style
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChainOfThoughtStyle
{
    Concise,
    Detailed,
    Structured,
    Conversational
}

/// <summary>
/// Chain-of-thought prompt request
/// </summary>
public class ChainOfThoughtPromptRequest
{
    public string Problem { get; set; } = string.Empty;
    public ChainOfThoughtConfig? Config { get; set; }
    public List<FewShotExample>? Examples { get; set; }
}

/// <summary>
/// Chain-of-thought prompt response
/// </summary>
public class ChainOfThoughtPromptResponse
{
    public string Prompt { get; set; } = string.Empty;
    public List<ChainOfThoughtStep> Steps { get; set; } = new();
    public int TokenCount { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Prompt optimization options
/// </summary>
public class PromptOptimizationOptions
{
    public bool OptimizeForTokens { get; set; } = true;
    public bool OptimizeForClarity { get; set; } = true;
    public bool OptimizeForCost { get; set; } = false;
    public bool RemoveRedundancy { get; set; } = true;
    public int? MaxTokens { get; set; }
    public string? TargetModel { get; set; }
}

/// <summary>
/// Prompt optimization result
/// </summary>
public class PromptOptimizationResult
{
    public string OriginalPrompt { get; set; } = string.Empty;
    public string OptimizedPrompt { get; set; } = string.Empty;
    public PromptMetrics OriginalMetrics { get; set; } = new();
    public PromptMetrics OptimizedMetrics { get; set; } = new();
    public List<string> OptimizationsApplied { get; set; } = new();
    public double ImprovementPercentage { get; set; }
    public DateTime OptimizedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Prompt metrics
/// </summary>
public class PromptMetrics
{
    public int TokenCount { get; set; }
    public int CharacterCount { get; set; }
    public int WordCount { get; set; }
    public double ReadabilityScore { get; set; }
    public double ClarityScore { get; set; }
    public double EstimatedCost { get; set; }
    public Dictionary<string, double> CustomMetrics { get; set; } = new();
}

/// <summary>
/// Prompt A/B test configuration
/// </summary>
public class PromptABTest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VariantA { get; set; } = string.Empty;
    public string VariantB { get; set; } = string.Empty;
    public ABTestMetrics MetricsA { get; set; } = new();
    public ABTestMetrics MetricsB { get; set; } = new();
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? WinningVariant { get; set; }
}

/// <summary>
/// A/B test metrics
/// </summary>
public class ABTestMetrics
{
    public int RequestCount { get; set; }
    public double AverageResponseTime { get; set; }
    public double AverageQualityScore { get; set; }
    public double SuccessRate { get; set; }
    public double AverageCost { get; set; }
    public int ErrorCount { get; set; }
}

/// <summary>
/// Prompt validation result
/// </summary>
public class PromptValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationIssue> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public PromptMetrics Metrics { get; set; } = new();
}

/// <summary>
/// Validation issue
/// </summary>
public class ValidationIssue
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string? Location { get; set; }
    public string? SuggestedFix { get; set; }
}

/// <summary>
/// Validation severity
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Prompt library category
/// </summary>
public class PromptCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int TemplateCount { get; set; }
}

/// <summary>
/// Prompt template search request
/// </summary>
public class PromptSearchRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public List<string>? Tags { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public PromptSortBy SortBy { get; set; } = PromptSortBy.Relevance;
    public bool IncludeInactive { get; set; } = false;
}

/// <summary>
/// Prompt sort options
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromptSortBy
{
    Relevance,
    Name,
    CreatedDate,
    UpdatedDate,
    PopularityScore
}

/// <summary>
/// Prompt template search response
/// </summary>
public class PromptSearchResponse
{
    public List<PromptTemplate> Templates { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Prompt performance metrics
/// </summary>
public class PromptPerformanceMetrics
{
    public string PromptId { get; set; } = string.Empty;
    public int TotalExecutions { get; set; }
    public double AverageTokenCount { get; set; }
    public double AverageCost { get; set; }
    public double AverageResponseTime { get; set; }
    public double SuccessRate { get; set; }
    public double QualityScore { get; set; }
    public Dictionary<string, double> CustomMetrics { get; set; } = new();
    public DateTime LastExecuted { get; set; }
}

/// <summary>
/// Cost estimate for prompt
/// </summary>
public class PromptCostEstimate
{
    public string PromptId { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int EstimatedOutputTokens { get; set; }
    public string Model { get; set; } = string.Empty;
    public double InputCost { get; set; }
    public double OutputCost { get; set; }
    public double TotalCost { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Prompt execution context
/// </summary>
public class PromptExecutionContext
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Prompt version
/// </summary>
public class PromptVersion
{
    public string VersionNumber { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ChangeDescription { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
}

/// <summary>
/// Prompt improvement suggestion
/// </summary>
public class PromptImprovementSuggestion
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OriginalText { get; set; } = string.Empty;
    public string SuggestedText { get; set; } = string.Empty;
    public double ImpactScore { get; set; }
    public string Rationale { get; set; } = string.Empty;
}

/// <summary>
/// Batch prompt request
/// </summary>
public class BatchPromptRequest
{
    public string TemplateId { get; set; } = string.Empty;
    public List<Dictionary<string, object>> VariableSets { get; set; } = new();
    public bool ParallelExecution { get; set; } = true;
    public int MaxConcurrency { get; set; } = 5;
}

/// <summary>
/// Batch prompt response
/// </summary>
public class BatchPromptResponse
{
    public string TemplateId { get; set; } = string.Empty;
    public List<PromptTemplateResponse> Results { get; set; } = new();
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}