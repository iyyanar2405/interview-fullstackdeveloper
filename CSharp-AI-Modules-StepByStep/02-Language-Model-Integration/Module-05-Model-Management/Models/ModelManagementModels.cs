namespace AI.ModelManagement.Models;

#region Enums

public enum ModelProvider
{
    OpenAI,
    AzureOpenAI,
    Anthropic,
    Cohere,
    HuggingFace,
    Local
}

public enum ModelCapability
{
    TextGeneration,
    ChatCompletion,
    CodeGeneration,
    Summarization,
    Translation,
    Classification,
    Embedding,
    ImageGeneration,
    Reasoning
}

public enum SelectionStrategy
{
    CostOptimized,      // Cheapest option that meets requirements
    PerformanceOptimized, // Fastest/highest quality
    Balanced,           // Balance cost and performance
    CapabilityBased,    // Based on specific capabilities
    LoadBalanced,       // Distribute across models
    Weighted           // Weighted scoring
}

public enum FallbackStrategy
{
    Sequential,         // Try fallbacks in order
    LoadBased,         // Based on current load
    CostBased,         // Fallback to cheaper
    QualityBased,      // Maintain quality level
    Random,            // Random selection
    None              // No fallback
}

public enum ModelStatus
{
    Available,
    Unavailable,
    Degraded,
    Maintenance,
    Deprecated
}

public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown
}

#endregion

#region Model Configuration

public class ModelConfiguration
{
    public string ModelId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ModelProvider Provider { get; set; }
    public List<ModelCapability> Capabilities { get; set; } = new();
    public ModelPricing Pricing { get; set; } = new();
    public ModelLimits Limits { get; set; } = new();
    public ModelStatus Status { get; set; }
    public int Priority { get; set; } = 1;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ModelPricing
{
    public decimal InputTokenCost { get; set; }  // Cost per 1K input tokens
    public decimal OutputTokenCost { get; set; } // Cost per 1K output tokens
    public decimal RequestCost { get; set; }     // Cost per request
    public string Currency { get; set; } = "USD";
}

public class ModelLimits
{
    public int MaxTokens { get; set; }
    public int MaxRequestsPerMinute { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public int ContextWindow { get; set; }
}

#endregion

#region Selection Models

public class ModelSelectionRequest
{
    public string TaskDescription { get; set; } = string.Empty;
    public List<ModelCapability> RequiredCapabilities { get; set; } = new();
    public SelectionStrategy Strategy { get; set; } = SelectionStrategy.Balanced;
    public ModelConstraints? Constraints { get; set; }
    public Dictionary<string, double> WeightingFactors { get; set; } = new();
}

public class ModelConstraints
{
    public decimal? MaxCostPerRequest { get; set; }
    public int? MaxLatencyMs { get; set; }
    public int? MinContextWindow { get; set; }
    public List<ModelProvider>? PreferredProviders { get; set; }
    public List<ModelProvider>? ExcludedProviders { get; set; }
    public List<string>? ExcludedModels { get; set; }
}

public class ModelSelectionResponse
{
    public ModelConfiguration SelectedModel { get; set; } = new();
    public List<ModelConfiguration> FallbackModels { get; set; } = new();
    public SelectionReasoning Reasoning { get; set; } = new();
    public decimal EstimatedCost { get; set; }
    public int EstimatedLatencyMs { get; set; }
}

public class SelectionReasoning
{
    public string PrimaryReason { get; set; } = string.Empty;
    public Dictionary<string, double> Scores { get; set; } = new();
    public List<string> ConsideredFactors { get; set; } = new();
    public List<string> RejectedModels { get; set; } = new();
}

#endregion

#region Fallback Models

public class FallbackConfiguration
{
    public FallbackStrategy Strategy { get; set; }
    public List<FallbackRule> Rules { get; set; } = new();
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public bool EnableCircuitBreaker { get; set; } = true;
}

public class FallbackRule
{
    public string TriggerCondition { get; set; } = string.Empty;
    public List<string> FallbackModelIds { get; set; } = new();
    public int Priority { get; set; }
    public bool MaintainCapabilities { get; set; } = true;
}

public class FallbackRequest
{
    public string OriginalModelId { get; set; } = string.Empty;
    public string FailureReason { get; set; } = string.Empty;
    public List<ModelCapability> RequiredCapabilities { get; set; } = new();
    public ModelConstraints? Constraints { get; set; }
}

public class FallbackResponse
{
    public ModelConfiguration? FallbackModel { get; set; }
    public bool Success { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int AttemptNumber { get; set; }
    public List<string> TriedModels { get; set; } = new();
}

#endregion

#region Cost Optimization Models

public class CostOptimizationConfig
{
    public decimal DailyBudget { get; set; }
    public decimal MonthlyBudget { get; set; }
    public bool EnableAutoScaling { get; set; } = true;
    public bool EnableCostAlerts { get; set; } = true;
    public decimal AlertThresholdPercentage { get; set; } = 80m;
    public List<CostOptimizationRule> Rules { get; set; } = new();
}

public class CostOptimizationRule
{
    public string Name { get; set; } = string.Empty;
    public decimal ThresholdAmount { get; set; }
    public string Action { get; set; } = string.Empty; // SwitchToCheaper, ThrottleRequests, Alert
    public List<string> TargetModelIds { get; set; } = new();
}

public class CostTrackingRequest
{
    public string ModelId { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new();
}

public class CostReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCost { get; set; }
    public Dictionary<string, decimal> CostByModel { get; set; } = new();
    public Dictionary<string, decimal> CostByProvider { get; set; } = new();
    public Dictionary<string, decimal> CostByUser { get; set; } = new();
    public List<CostEntry> TopExpenses { get; set; } = new();
    public CostTrend Trend { get; set; } = new();
}

public class CostEntry
{
    public DateTime Timestamp { get; set; }
    public string ModelId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new();
}

public class CostTrend
{
    public decimal AverageDailyCost { get; set; }
    public decimal ProjectedMonthlyCost { get; set; }
    public double GrowthRate { get; set; }
    public List<CostAlert> Alerts { get; set; } = new();
}

public class CostAlert
{
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal CurrentAmount { get; set; }
    public decimal ThresholdAmount { get; set; }
    public DateTime Timestamp { get; set; }
}

#endregion

#region Performance Monitoring Models

public class PerformanceMetrics
{
    public string ModelId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int LatencyMs { get; set; }
    public int TokensPerSecond { get; set; }
    public bool Success { get; set; }
    public string? ErrorType { get; set; }
    public double? QualityScore { get; set; }
}

public class ModelHealthCheck
{
    public string ModelId { get; set; } = string.Empty;
    public HealthStatus Status { get; set; }
    public DateTime LastChecked { get; set; }
    public int ResponseTimeMs { get; set; }
    public double SuccessRate { get; set; }
    public double AverageLatencyMs { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class PerformanceReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, ModelPerformance> ModelPerformance { get; set; } = new();
    public PerformanceSummary Summary { get; set; } = new();
}

public class ModelPerformance
{
    public string ModelId { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double SuccessRate { get; set; }
    public double AverageLatencyMs { get; set; }
    public double P95LatencyMs { get; set; }
    public double P99LatencyMs { get; set; }
    public int TotalTokens { get; set; }
    public double AverageTokensPerSecond { get; set; }
    public double? AverageQualityScore { get; set; }
}

public class PerformanceSummary
{
    public int TotalRequests { get; set; }
    public double OverallSuccessRate { get; set; }
    public double AverageLatencyMs { get; set; }
    public string FastestModel { get; set; } = string.Empty;
    public string MostReliableModel { get; set; } = string.Empty;
    public string MostUsedModel { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}

#endregion

#region Load Balancing Models

public class LoadBalancingConfig
{
    public bool Enabled { get; set; } = true;
    public LoadBalancingStrategy Strategy { get; set; }
    public Dictionary<string, int> ModelWeights { get; set; } = new();
    public int HealthCheckIntervalSeconds { get; set; } = 60;
}

public enum LoadBalancingStrategy
{
    RoundRobin,
    LeastConnections,
    WeightedRoundRobin,
    ResponseTimeBased,
    Random
}

public class LoadBalancingRequest
{
    public List<string> AvailableModelIds { get; set; } = new();
    public List<ModelCapability> RequiredCapabilities { get; set; } = new();
    public LoadBalancingStrategy Strategy { get; set; }
}

public class LoadBalancingResponse
{
    public string SelectedModelId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Dictionary<string, int> CurrentLoad { get; set; } = new();
}

#endregion

#region Configuration Models

public class ModelManagementSettings
{
    public List<ModelConfiguration> Models { get; set; } = new();
    public FallbackConfiguration FallbackConfig { get; set; } = new();
    public CostOptimizationConfig CostConfig { get; set; } = new();
    public LoadBalancingConfig LoadBalancingConfig { get; set; } = new();
    public MonitoringSettings MonitoringSettings { get; set; } = new();
}

public class MonitoringSettings
{
    public bool EnablePerformanceTracking { get; set; } = true;
    public bool EnableCostTracking { get; set; } = true;
    public bool EnableHealthChecks { get; set; } = true;
    public int MetricsRetentionDays { get; set; } = 30;
    public int HealthCheckIntervalSeconds { get; set; } = 60;
}

#endregion
