namespace Module_05_Rate_Limiting.Models;

// ===== Rate Limiting Models =====

public class RateLimitRule
{
    public string RuleId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public RateLimitAlgorithm Algorithm { get; set; }
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
    public string? ClientId { get; set; }
    public string? IpAddress { get; set; }
    public int Priority { get; set; } = 0;
    public bool Enabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum RateLimitAlgorithm
{
    TokenBucket,
    SlidingWindow,
    FixedWindow,
    LeakyBucket,
    ConcurrentRequests
}

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public string? ReasonDenied { get; set; }
    public int RemainingRequests { get; set; }
    public DateTime? ResetTime { get; set; }
    public TimeSpan? RetryAfter { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class RateLimitStatus
{
    public string ClientId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int RemainingRequests { get; set; }
    public int Limit { get; set; }
    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
    public DateTime? NextReset { get; set; }
    public RateLimitAlgorithm Algorithm { get; set; }
}

// ===== Quota Management Models =====

public class QuotaPolicy
{
    public string PolicyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubscriptionTier Tier { get; set; }
    public int DailyLimit { get; set; }
    public int MonthlyLimit { get; set; }
    public int ConcurrentLimit { get; set; }
    public decimal CostPerRequest { get; set; }
    public decimal OverageCost { get; set; }
    public bool AllowOverage { get; set; }
    public List<string> AllowedEndpoints { get; set; } = new();
    public Dictionary<string, int> EndpointLimits { get; set; } = new();
}

public enum SubscriptionTier
{
    Free,
    Basic,
    Professional,
    Enterprise,
    Unlimited
}

public class QuotaUsage
{
    public string UsageId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string PolicyId { get; set; } = string.Empty;
    public DateTime Period { get; set; }
    public int RequestCount { get; set; }
    public int DailyUsed { get; set; }
    public int MonthlyUsed { get; set; }
    public int DailyRemaining { get; set; }
    public int MonthlyRemaining { get; set; }
    public decimal CurrentCost { get; set; }
    public bool IsOverQuota { get; set; }
    public DateTime LastRequestAt { get; set; }
}

public class QuotaAllocation
{
    public string ClientId { get; set; } = string.Empty;
    public SubscriptionTier Tier { get; set; }
    public int DailyQuota { get; set; }
    public int MonthlyQuota { get; set; }
    public int UsedToday { get; set; }
    public int UsedThisMonth { get; set; }
    public DateTime QuotaResetDate { get; set; }
    public bool HasOverageAllowance { get; set; }
}

// ===== DDoS Protection Models =====

public class DDoSDetectionResult
{
    public bool IsSuspicious { get; set; }
    public ThreatLevel ThreatLevel { get; set; }
    public List<string> DetectionReasons { get; set; } = new();
    public string IpAddress { get; set; } = string.Empty;
    public int RequestsPerSecond { get; set; }
    public int TotalRequests { get; set; }
    public TimeSpan ObservationPeriod { get; set; }
    public DDoSAction RecommendedAction { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public enum ThreatLevel
{
    None,
    Low,
    Medium,
    High,
    Critical
}

public enum DDoSAction
{
    Allow,
    Challenge,
    RateLimit,
    Throttle,
    Block,
    Blacklist
}

public class IpBlockRule
{
    public string RuleId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? IpRange { get; set; }
    public BlockReason Reason { get; set; }
    public DateTime BlockedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsPermanent { get; set; }
    public string? Notes { get; set; }
    public int ViolationCount { get; set; }
}

public enum BlockReason
{
    DDoSDetected,
    ExcessiveRequests,
    SuspiciousActivity,
    ManualBlock,
    Blacklist,
    BotDetected,
    InvalidRequests
}

public class TrafficAnalysis
{
    public string IpAddress { get; set; } = string.Empty;
    public int RequestsPerSecond { get; set; }
    public int RequestsPerMinute { get; set; }
    public int TotalRequests { get; set; }
    public int FailedRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public double FailureRate { get; set; }
    public List<string> AccessedEndpoints { get; set; } = new();
    public int UniqueEndpoints { get; set; }
    public Dictionary<string, int> EndpointFrequency { get; set; } = new();
    public DateTime AnalysisPeriodStart { get; set; }
    public DateTime AnalysisPeriodEnd { get; set; }
    public bool IsBot { get; set; }
    public string? UserAgent { get; set; }
}

// ===== Throttling Models =====

public class ThrottlePolicy
{
    public string PolicyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ThrottleStrategy Strategy { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public TimeSpan RequestTimeout { get; set; }
    public BackoffStrategy BackoffStrategy { get; set; }
    public int MaxRetries { get; set; }
    public PriorityLevel DefaultPriority { get; set; }
    public bool EnablePriorityQueue { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public enum ThrottleStrategy
{
    Simple,
    Adaptive,
    PriorityBased,
    ResourceBased,
    PredictiveThrottling
}

public enum BackoffStrategy
{
    Linear,
    Exponential,
    Fibonacci,
    Decorrelated,
    Fixed
}

public enum PriorityLevel
{
    Low = 0,
    Normal = 5,
    High = 10,
    Critical = 15
}

public class ThrottleResult
{
    public bool IsThrottled { get; set; }
    public string? Reason { get; set; }
    public TimeSpan? WaitTime { get; set; }
    public int QueuePosition { get; set; }
    public int EstimatedWaitSeconds { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? RetryAt { get; set; }
}

public class ResourceThrottle
{
    public string ResourceId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public int CurrentLoad { get; set; }
    public int MaxCapacity { get; set; }
    public double UtilizationPercentage { get; set; }
    public bool IsOverloaded { get; set; }
    public int QueuedRequests { get; set; }
    public DateTime LastUpdated { get; set; }
}

// ===== Usage Tracking Models =====

public class UsageRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime Timestamp { get; set; }
    public bool WasRateLimited { get; set; }
    public bool WasThrottled { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class UsageStatistics
{
    public string ClientId { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public int RateLimitedRequests { get; set; }
    public int ThrottledRequests { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public double RequestsPerSecond { get; set; }
    public Dictionary<string, int> EndpointUsage { get; set; } = new();
    public Dictionary<int, int> StatusCodeDistribution { get; set; } = new();
}

// ===== Fair Usage Models =====

public class FairUsagePolicy
{
    public string PolicyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int BaselineRequestsPerMinute { get; set; }
    public int BurstCapacity { get; set; }
    public TimeSpan BurstDuration { get; set; }
    public int CooldownPeriodMinutes { get; set; }
    public bool EnableDynamicAdjustment { get; set; }
    public Dictionary<string, int> EndpointWeights { get; set; } = new();
    public List<string> ExemptClients { get; set; } = new();
}

public class FairUsageViolation
{
    public string ViolationId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public ViolationType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public ViolationSeverity Severity { get; set; }
    public ViolationAction ActionTaken { get; set; }
    public TimeSpan? PenaltyDuration { get; set; }
}

public enum ViolationType
{
    ExcessiveRequests,
    BurstAbuse,
    QuotaExceeded,
    ResourceHogging,
    SuspiciousPattern,
    BotActivity
}

public enum ViolationSeverity
{
    Minor,
    Moderate,
    Serious,
    Severe,
    Critical
}

public enum ViolationAction
{
    Warning,
    TemporaryRateLimit,
    QuotaReduction,
    TemporarySuspension,
    AccountReview,
    PermanentBan
}

// ===== Request/Response Models =====

public class RateLimitRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}

public class QuotaCheckRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int RequestCount { get; set; } = 1;
}

public class QuotaCheckResponse
{
    public bool HasQuota { get; set; }
    public int RemainingDaily { get; set; }
    public int RemainingMonthly { get; set; }
    public DateTime QuotaResetTime { get; set; }
    public string? Message { get; set; }
}

public class PolicyUpdateRequest
{
    public string ClientId { get; set; } = string.Empty;
    public SubscriptionTier NewTier { get; set; }
    public int? CustomDailyLimit { get; set; }
    public int? CustomMonthlyLimit { get; set; }
}

public class IpBlockRequest
{
    public string IpAddress { get; set; } = string.Empty;
    public BlockReason Reason { get; set; }
    public bool IsPermanent { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
}

// ===== Configuration Models =====

public class RateLimitConfiguration
{
    public bool EnableRateLimiting { get; set; } = true;
    public RateLimitAlgorithm DefaultAlgorithm { get; set; } = RateLimitAlgorithm.SlidingWindow;
    public int DefaultLimit { get; set; } = 100;
    public int DefaultPeriodSeconds { get; set; } = 60;
    public bool EnableIpRateLimiting { get; set; } = true;
    public bool EnableClientRateLimiting { get; set; } = true;
    public bool EnableEndpointRateLimiting { get; set; } = true;
    public string RedisConnectionString { get; set; } = string.Empty;
    public bool UseDistributedCache { get; set; } = true;
    public List<string> WhitelistedIps { get; set; } = new();
    public List<string> WhitelistedClients { get; set; } = new();
}

public class DDoSConfiguration
{
    public bool EnableDDoSProtection { get; set; } = true;
    public int RequestsPerSecondThreshold { get; set; } = 100;
    public int RequestsPerMinuteThreshold { get; set; } = 1000;
    public TimeSpan AnalysisWindow { get; set; } = TimeSpan.FromMinutes(5);
    public bool AutoBlockSuspiciousIps { get; set; } = true;
    public int AutoBlockDurationMinutes { get; set; } = 60;
    public ThreatLevel AutoBlockThreshold { get; set; } = ThreatLevel.High;
    public bool EnableChallengeResponse { get; set; } = true;
}
