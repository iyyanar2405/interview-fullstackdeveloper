namespace AI.Embeddings.Models;

#region Enums

public enum EmbeddingProvider
{
    OpenAI,
    Azure,
    Cohere,
    HuggingFace,
    Local,
    Custom
}

public enum EmbeddingModel
{
    // OpenAI Models
    TextEmbedding3Small,      // 1536 dimensions
    TextEmbedding3Large,      // 3072 dimensions
    TextEmbeddingAda002,      // 1536 dimensions (legacy)
    
    // Azure OpenAI Models
    AzureTextEmbedding3Small,
    AzureTextEmbedding3Large,
    
    // Cohere Models
    CohereEmbedEnglishV3,
    CohereEmbedMultilingualV3,
    
    // HuggingFace Models
    SentenceTransformers,
    AllMiniLML6V2,
    
    // Local Models
    LocalBERT,
    LocalMiniLM,
    
    Custom
}

public enum SimilarityMetric
{
    Cosine,
    Euclidean,
    DotProduct,
    Manhattan,
    Jaccard
}

public enum CacheStrategy
{
    None,
    Memory,
    Redis,
    Hybrid
}

#endregion

#region Embedding Models

public class EmbeddingRequest
{
    public List<string> Texts { get; set; } = new();
    public EmbeddingModel Model { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public EmbeddingProvider Provider { get; set; } = EmbeddingProvider.OpenAI;
    public EmbeddingOptions Options { get; set; } = new();
}

public class EmbeddingOptions
{
    public int? Dimensions { get; set; }
    public string? User { get; set; }
    public bool Normalize { get; set; } = true;
    public bool UseCache { get; set; } = true;
    public int? TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, object> ProviderSpecificOptions { get; set; } = new();
}

public class EmbeddingResponse
{
    public List<EmbeddingResult> Results { get; set; } = new();
    public EmbeddingUsage Usage { get; set; } = new();
    public EmbeddingModel Model { get; set; }
    public EmbeddingProvider Provider { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan Duration { get; set; }
}

public class EmbeddingResult
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public int Dimensions { get; set; }
    public EmbeddingMetadata Metadata { get; set; } = new();
}

public class EmbeddingMetadata
{
    public int TokenCount { get; set; }
    public bool FromCache { get; set; }
    public string? CacheKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Custom { get; set; } = new();
}

public class EmbeddingUsage
{
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    public int CacheHits { get; set; }
    public int CacheMisses { get; set; }
}

#endregion

#region Similarity Models

public class SimilarityRequest
{
    public float[] Embedding1 { get; set; } = Array.Empty<float>();
    public float[] Embedding2 { get; set; } = Array.Empty<float>();
    public SimilarityMetric Metric { get; set; } = SimilarityMetric.Cosine;
}

public class SimilarityResult
{
    public float Score { get; set; }
    public SimilarityMetric Metric { get; set; }
    public int Dimensions { get; set; }
    public bool Normalized { get; set; }
}

public class BatchSimilarityRequest
{
    public float[] QueryEmbedding { get; set; } = Array.Empty<float>();
    public List<float[]> TargetEmbeddings { get; set; } = new();
    public SimilarityMetric Metric { get; set; } = SimilarityMetric.Cosine;
    public int TopK { get; set; } = 10;
    public float? Threshold { get; set; }
}

public class BatchSimilarityResult
{
    public List<SimilarityMatch> Matches { get; set; } = new();
    public SimilarityMetric Metric { get; set; }
    public int TotalCompared { get; set; }
    public TimeSpan Duration { get; set; }
}

public class SimilarityMatch
{
    public int Index { get; set; }
    public float Score { get; set; }
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

#endregion

#region Batch Processing Models

public class BatchEmbeddingRequest
{
    public List<string> Texts { get; set; } = new();
    public EmbeddingModel Model { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public EmbeddingProvider Provider { get; set; } = EmbeddingProvider.OpenAI;
    public BatchOptions Options { get; set; } = new();
}

public class BatchOptions
{
    public int BatchSize { get; set; } = 100;
    public int MaxConcurrency { get; set; } = 5;
    public bool ContinueOnError { get; set; } = true;
    public bool UseCache { get; set; } = true;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}

public class BatchEmbeddingResponse
{
    public List<EmbeddingResult> Results { get; set; } = new();
    public BatchStatistics Statistics { get; set; } = new();
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class BatchStatistics
{
    public int TotalTexts { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int BatchesProcessed { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public decimal TotalCost { get; set; }
    public int CacheHits { get; set; }
    public int CacheMisses { get; set; }
}

#endregion

#region Cache Models

public class CachedEmbedding
{
    public string Key { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public string Text { get; set; } = string.Empty;
    public EmbeddingModel Model { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public int HitCount { get; set; }
}

public class CacheStatistics
{
    public int TotalEntries { get; set; }
    public int Hits { get; set; }
    public int Misses { get; set; }
    public double HitRate { get; set; }
    public long TotalSizeBytes { get; set; }
    public DateTime LastCleanup { get; set; }
}

#endregion

#region Comparison Models

public class EmbeddingComparison
{
    public string Text1 { get; set; } = string.Empty;
    public string Text2 { get; set; } = string.Empty;
    public float[] Embedding1 { get; set; } = Array.Empty<float>();
    public float[] Embedding2 { get; set; } = Array.Empty<float>();
    public Dictionary<SimilarityMetric, float> Similarities { get; set; } = new();
    public bool AreSimilar { get; set; }
    public float Threshold { get; set; } = 0.8f;
}

public class MultiTextComparison
{
    public List<string> Texts { get; set; } = new();
    public List<float[]> Embeddings { get; set; } = new();
    public float[,] SimilarityMatrix { get; set; } = new float[0, 0];
    public SimilarityMetric Metric { get; set; }
    public List<ClusterGroup> Clusters { get; set; } = new();
}

public class ClusterGroup
{
    public int ClusterId { get; set; }
    public List<int> TextIndices { get; set; } = new();
    public float[] Centroid { get; set; } = Array.Empty<float>();
    public float AverageSimilarity { get; set; }
}

#endregion

#region Model Information

public class EmbeddingModelInfo
{
    public EmbeddingModel Model { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int DefaultDimensions { get; set; }
    public int MaxTokens { get; set; }
    public decimal CostPer1KTokens { get; set; }
    public EmbeddingProvider Provider { get; set; }
    public bool SupportsCustomDimensions { get; set; }
    public List<int> AvailableDimensions { get; set; } = new();
    public Dictionary<string, object> Capabilities { get; set; } = new();
}

#endregion

#region Configuration Models

public class EmbeddingSettings
{
    public string OpenAIApiKey { get; set; } = string.Empty;
    public string? AzureEndpoint { get; set; }
    public string? AzureApiKey { get; set; }
    public string? CohereApiKey { get; set; }
    public string? HuggingFaceApiKey { get; set; }
    
    public EmbeddingModel DefaultModel { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public EmbeddingProvider DefaultProvider { get; set; } = EmbeddingProvider.OpenAI;
    
    public CacheStrategy CacheStrategy { get; set; } = CacheStrategy.Memory;
    public int CacheDurationMinutes { get; set; } = 60;
    public int MaxCacheEntries { get; set; } = 10000;
    
    public string? RedisConnectionString { get; set; }
    
    public int DefaultBatchSize { get; set; } = 100;
    public int MaxConcurrency { get; set; } = 5;
    public int RequestTimeoutSeconds { get; set; } = 30;
    
    public bool EnableMetrics { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
}

#endregion

#region Search Models

public class SemanticSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<SemanticDocument> Documents { get; set; } = new();
    public EmbeddingModel Model { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public int TopK { get; set; } = 5;
    public float? Threshold { get; set; }
    public SimilarityMetric Metric { get; set; } = SimilarityMetric.Cosine;
}

public class SemanticDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public float[]? Embedding { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SemanticSearchResponse
{
    public List<SearchResult> Results { get; set; } = new();
    public string Query { get; set; } = string.Empty;
    public float[] QueryEmbedding { get; set; } = Array.Empty<float>();
    public TimeSpan Duration { get; set; }
}

public class SearchResult
{
    public string DocumentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float Score { get; set; }
    public int Rank { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

#endregion

#region Analytics Models

public class EmbeddingAnalytics
{
    public int TotalEmbeddingsGenerated { get; set; }
    public int TotalTokensProcessed { get; set; }
    public decimal TotalCost { get; set; }
    public Dictionary<EmbeddingModel, int> ModelUsage { get; set; } = new();
    public Dictionary<EmbeddingProvider, int> ProviderUsage { get; set; } = new();
    public CacheStatistics CacheStats { get; set; } = new();
    public TimeSpan AverageResponseTime { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

#endregion
