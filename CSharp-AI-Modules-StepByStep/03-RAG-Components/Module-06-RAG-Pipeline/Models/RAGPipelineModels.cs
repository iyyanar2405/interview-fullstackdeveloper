namespace AI.RAGPipeline.Models;

#region Enums

public enum RAGStrategy
{
    Simple,
    HybridSearch,
    MultiQuery,
    HyDE,
    ParentDocument,
    SelfQuery
}

public enum ResponseQuality
{
    Poor,
    Fair,
    Good,
    Excellent
}

public enum PipelineStage
{
    Ingestion,
    QueryProcessing,
    Retrieval,
    ContextAssembly,
    Generation,
    Evaluation,
    Completed,
    Failed
}

public enum ChunkingStrategy
{
    FixedSize,
    Sentence,
    Paragraph,
    Recursive,
    Semantic,
    Sliding
}

#endregion

#region Configuration Models

public class RAGPipelineSettings
{
    public string OpenAIApiKey { get; set; } = string.Empty;
    public string AzureOpenAIEndpoint { get; set; } = string.Empty;
    public string AzureOpenAIKey { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public string ChatModel { get; set; } = "gpt-4";
    public int EmbeddingDimensions { get; set; } = 1536;
    public string VectorDatabaseUrl { get; set; } = "http://localhost:6333";
    public string CollectionName { get; set; } = "rag_documents";
    public RAGStrategy DefaultStrategy { get; set; } = RAGStrategy.HybridSearch;
    public int DefaultTopK { get; set; } = 5;
    public float DefaultTemperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 2000;
    public bool EnableCaching { get; set; } = true;
    public bool EnableEvaluation { get; set; } = true;
    public string RedisConnectionString { get; set; } = "localhost:6379";
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class IngestionConfig
{
    public ChunkingStrategy Strategy { get; set; } = ChunkingStrategy.Recursive;
    public int ChunkSize { get; set; } = 1000;
    public int ChunkOverlap { get; set; } = 200;
    public bool ExtractMetadata { get; set; } = true;
    public bool GenerateEmbeddings { get; set; } = true;
    public bool StoreInVectorDb { get; set; } = true;
    public int BatchSize { get; set; } = 10;
}

public class RetrievalConfig
{
    public RAGStrategy Strategy { get; set; } = RAGStrategy.HybridSearch;
    public int TopK { get; set; } = 5;
    public float MinimumScore { get; set; } = 0.7f;
    public bool EnableReranking { get; set; } = true;
    public string RerankingAlgorithm { get; set; } = "RRF";
    public Dictionary<string, object> Filters { get; set; } = new();
}

public class GenerationConfig
{
    public string Model { get; set; } = "gpt-4";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 2000;
    public float TopP { get; set; } = 1.0f;
    public float FrequencyPenalty { get; set; } = 0.0f;
    public float PresencePenalty { get; set; } = 0.0f;
    public bool Stream { get; set; } = false;
    public string SystemPrompt { get; set; } = "You are a helpful assistant that answers questions based on the provided context.";
}

#endregion

#region Document Ingestion Models

public class DocumentIngestionRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public IngestionConfig Config { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string? KnowledgeBaseId { get; set; }
}

public class DocumentIngestionResult
{
    public string DocumentId { get; set; } = string.Empty;
    public int ChunksCreated { get; set; }
    public int EmbeddingsGenerated { get; set; }
    public int VectorsStored { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public List<string> ChunkIds { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DocumentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DocumentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

#endregion

#region Query Processing Models

public class RAGQuery
{
    public string Query { get; set; } = string.Empty;
    public string? KnowledgeBaseId { get; set; }
    public RetrievalConfig RetrievalConfig { get; set; } = new();
    public GenerationConfig GenerationConfig { get; set; } = new();
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public List<ConversationMessage> ConversationHistory { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ProcessedQuery
{
    public string OriginalQuery { get; set; } = string.Empty;
    public string RewrittenQuery { get; set; } = string.Empty;
    public List<string> ExpandedQueries { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public float[] QueryEmbedding { get; set; } = Array.Empty<float>();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ConversationMessage
{
    public string Role { get; set; } = string.Empty; // "user", "assistant", "system"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

#endregion

#region Retrieval Models

public class RetrievedContext
{
    public List<ContextChunk> Chunks { get; set; } = new();
    public int TotalRetrieved { get; set; }
    public int AfterReranking { get; set; }
    public TimeSpan RetrievalTime { get; set; }
    public Dictionary<string, float> StrategyScores { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ContextChunk
{
    public string Id { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float Score { get; set; }
    public float RelevanceScore { get; set; }
    public int ChunkIndex { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

#endregion

#region Response Generation Models

public class RAGResponse
{
    public string QueryId { get; set; } = Guid.NewGuid().ToString();
    public string Query { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public List<ContextChunk> Sources { get; set; } = new();
    public int TokensUsed { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan RetrievalTime { get; set; }
    public TimeSpan GenerationTime { get; set; }
    public RAGEvaluation? Evaluation { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class StreamingRAGResponse
{
    public string QueryId { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public IAsyncEnumerable<string>? AnswerStream { get; set; }
    public List<ContextChunk> Sources { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

#endregion

#region Evaluation Models

public class RAGEvaluation
{
    public string QueryId { get; set; } = string.Empty;
    public float FaithfulnessScore { get; set; }
    public float RelevanceScore { get; set; }
    public float AnswerQualityScore { get; set; }
    public float ContextPrecisionScore { get; set; }
    public float ContextRecallScore { get; set; }
    public float OverallScore { get; set; }
    public ResponseQuality Quality { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public Dictionary<string, float> DetailedMetrics { get; set; } = new();
}

public class EvaluationRequest
{
    public string Query { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public List<ContextChunk> Context { get; set; } = new();
    public string? GroundTruth { get; set; }
}

#endregion

#region Pipeline Orchestration Models

public class PipelineExecutionContext
{
    public string ExecutionId { get; set; } = Guid.NewGuid().ToString();
    public string Query { get; set; } = string.Empty;
    public PipelineStage CurrentStage { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public ProcessedQuery? ProcessedQuery { get; set; }
    public RetrievedContext? RetrievedContext { get; set; }
    public RAGResponse? Response { get; set; }
    public List<PipelineError> Errors { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class PipelineError
{
    public PipelineStage Stage { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PipelineMetrics
{
    public int TotalQueries { get; set; }
    public int SuccessfulQueries { get; set; }
    public int FailedQueries { get; set; }
    public float AverageResponseTime { get; set; }
    public float AverageRetrievalTime { get; set; }
    public float AverageGenerationTime { get; set; }
    public float AverageRelevanceScore { get; set; }
    public int TotalTokensUsed { get; set; }
    public Dictionary<PipelineStage, int> ErrorsByStage { get; set; } = new();
    public Dictionary<string, int> QueriesByStrategy { get; set; } = new();
}

#endregion

#region Batch Processing Models

public class BatchRAGRequest
{
    public List<RAGQuery> Queries { get; set; } = new();
    public int MaxConcurrency { get; set; } = 5;
    public bool ContinueOnError { get; set; } = true;
}

public class BatchRAGResponse
{
    public int TotalQueries { get; set; }
    public int SuccessfulQueries { get; set; }
    public int FailedQueries { get; set; }
    public List<RAGResponse> Responses { get; set; } = new();
    public List<BatchQueryError> Errors { get; set; } = new();
    public TimeSpan TotalTime { get; set; }
}

public class BatchQueryError
{
    public string Query { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int QueryIndex { get; set; }
}

#endregion

#region API Response Models

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();

    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

#endregion
