namespace AI.RetrievalStrategies.Models;

#region Enums

public enum RetrievalStrategy
{
    Semantic,
    Hybrid,
    MultiQuery,
    HyDE,
    ParentDocument,
    SelfQuery
}

public enum ReRankingAlgorithm
{
    ReciprocalRankFusion,
    CrossEncoder,
    Diversity,
    MaximalMarginalRelevance,
    BM25,
    Cohere
}

public enum QueryExpansionMethod
{
    Synonyms,
    Paraphrasing,
    QuestionGeneration,
    MultiPerspective,
    ContextualExpansion
}

public enum ContextOptimizationStrategy
{
    MaxTokens,
    Relevance,
    Diversity,
    Recency,
    Hybrid
}

#endregion

#region Query Models

public class RetrievalQuery
{
    public string QueryText { get; set; } = string.Empty;
    public string[]? QueryEmbedding { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public RetrievalStrategy Strategy { get; set; } = RetrievalStrategy.Semantic;
    public int TopK { get; set; } = 10;
    public float? ScoreThreshold { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}

public class MultiQueryRequest
{
    public string OriginalQuery { get; set; } = string.Empty;
    public List<string> ExpandedQueries { get; set; } = new();
    public int TopK { get; set; } = 10;
    public bool DeduplicateResults { get; set; } = true;
    public Dictionary<string, string> Filters { get; set; } = new();
}

public class HybridSearchRequest
{
    public string QueryText { get; set; } = string.Empty;
    public float[]? QueryEmbedding { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public int TopK { get; set; } = 10;
    public float SemanticWeight { get; set; } = 0.7f;
    public float KeywordWeight { get; set; } = 0.3f;
    public Dictionary<string, string> Filters { get; set; } = new();
}

public class HyDERequest
{
    public string Query { get; set; } = string.Empty;
    public int NumHypotheticalDocs { get; set; } = 3;
    public int TopK { get; set; } = 10;
    public string CollectionName { get; set; } = string.Empty;
}

#endregion

#region Document Models

public class RetrievedDocument
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float Score { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public float[]? Embedding { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime? Timestamp { get; set; }
    public int? ChunkIndex { get; set; }
    public int? TotalChunks { get; set; }
}

public class DocumentChunk
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ParentDocumentId { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public float[]? Embedding { get; set; }
}

#endregion

#region Retrieval Response Models

public class RetrievalResponse
{
    public List<RetrievedDocument> Documents { get; set; } = new();
    public string Query { get; set; } = string.Empty;
    public RetrievalStrategy Strategy { get; set; }
    public int TotalFound { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class MultiQueryResponse
{
    public List<RetrievalResponse> QueryResults { get; set; } = new();
    public List<RetrievedDocument> MergedDocuments { get; set; } = new();
    public string OriginalQuery { get; set; } = string.Empty;
    public int TotalQueries { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Re-ranking Models

public class ReRankRequest
{
    public string Query { get; set; } = string.Empty;
    public List<RetrievedDocument> Documents { get; set; } = new();
    public ReRankingAlgorithm Algorithm { get; set; } = ReRankingAlgorithm.ReciprocalRankFusion;
    public int TopK { get; set; } = 10;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ReRankResponse
{
    public List<RetrievedDocument> ReRankedDocuments { get; set; } = new();
    public ReRankingAlgorithm Algorithm { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public Dictionary<string, float> ScoreDistribution { get; set; } = new();
}

public class RRFResult
{
    public string DocumentId { get; set; } = string.Empty;
    public float FusionScore { get; set; }
    public Dictionary<string, int> Rankings { get; set; } = new();
}

#endregion

#region Context Optimization Models

public class ContextWindow
{
    public List<RetrievedDocument> Documents { get; set; } = new();
    public int TotalTokens { get; set; }
    public int MaxTokens { get; set; }
    public ContextOptimizationStrategy Strategy { get; set; }
    public bool IsTruncated { get; set; }
}

public class ContextOptimizationRequest
{
    public List<RetrievedDocument> Documents { get; set; } = new();
    public int MaxTokens { get; set; } = 4000;
    public ContextOptimizationStrategy Strategy { get; set; } = ContextOptimizationStrategy.Relevance;
    public bool PreserveMetadata { get; set; } = true;
    public string? QueryContext { get; set; }
}

public class ContextOptimizationResponse
{
    public ContextWindow OptimizedContext { get; set; } = new();
    public int OriginalTokenCount { get; set; }
    public int OptimizedTokenCount { get; set; }
    public float CompressionRatio { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Query Expansion Models

public class QueryExpansionRequest
{
    public string OriginalQuery { get; set; } = string.Empty;
    public QueryExpansionMethod Method { get; set; } = QueryExpansionMethod.Synonyms;
    public int MaxExpansions { get; set; } = 5;
    public string? Language { get; set; } = "en";
}

public class QueryExpansionResponse
{
    public string OriginalQuery { get; set; } = string.Empty;
    public List<string> ExpandedQueries { get; set; } = new();
    public QueryExpansionMethod Method { get; set; }
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
}

#endregion

#region Diversity Models

public class DiversityRequest
{
    public List<RetrievedDocument> Documents { get; set; } = new();
    public int TopK { get; set; } = 10;
    public float LambdaParameter { get; set; } = 0.5f; // MMR lambda
    public string? QueryText { get; set; }
}

public class MMRResult
{
    public List<RetrievedDocument> DiverseDocuments { get; set; } = new();
    public float AverageDiversity { get; set; }
    public Dictionary<string, float> DiversityScores { get; set; } = new();
}

#endregion

#region Self-Query Models

public class SelfQueryRequest
{
    public string NaturalLanguageQuery { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
    public int TopK { get; set; } = 10;
}

public class ParsedQuery
{
    public string QueryText { get; set; } = string.Empty;
    public Dictionary<string, string> ExtractedFilters { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> Keywords { get; set; } = new();
}

#endregion

#region Analytics Models

public class RetrievalAnalytics
{
    public int TotalQueries { get; set; }
    public int TotalDocumentsRetrieved { get; set; }
    public double AverageRetrievalTime { get; set; }
    public Dictionary<RetrievalStrategy, int> StrategyUsage { get; set; } = new();
    public Dictionary<ReRankingAlgorithm, int> ReRankingUsage { get; set; } = new();
    public double AverageScore { get; set; }
    public int CacheHits { get; set; }
    public int CacheMisses { get; set; }
}

#endregion

#region Settings

public class RetrievalSettings
{
    public string? OpenAIApiKey { get; set; }
    public string? OpenAIEndpoint { get; set; }
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public string ChatModel { get; set; } = "gpt-4";
    
    public int DefaultTopK { get; set; } = 10;
    public float DefaultScoreThreshold { get; set; } = 0.7f;
    
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 30;
    
    public int MaxContextTokens { get; set; } = 4000;
    public int MaxQueryExpansions { get; set; } = 5;
    
    public float HybridSemanticWeight { get; set; } = 0.7f;
    public float HybridKeywordWeight { get; set; } = 0.3f;
    
    public float MMRLambda { get; set; } = 0.5f;
    public int RRFConstant { get; set; } = 60;
    
    public string? VectorDatabaseEndpoint { get; set; }
    public string? RedisConnectionString { get; set; }
}

#endregion

#region BM25 Models

public class BM25Parameters
{
    public float K1 { get; set; } = 1.5f;
    public float B { get; set; } = 0.75f;
}

public class BM25Result
{
    public string DocumentId { get; set; } = string.Empty;
    public float Score { get; set; }
    public Dictionary<string, float> TermScores { get; set; } = new();
}

#endregion

#region Parent Document Models

public class ParentDocumentRequest
{
    public string Query { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
    public int TopK { get; set; } = 10;
    public bool RetrieveFullParent { get; set; } = true;
}

public class ParentDocumentResponse
{
    public List<RetrievedDocument> ChildChunks { get; set; } = new();
    public List<RetrievedDocument> ParentDocuments { get; set; } = new();
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
}

#endregion
