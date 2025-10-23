namespace AI.VectorDatabases.Models;

#region Enums

public enum VectorDatabaseProvider
{
    Qdrant,
    Pinecone,
    InMemory,
    Weaviate,
    Milvus,
    Custom
}

public enum DistanceMetric
{
    Cosine,
    Euclidean,
    DotProduct,
    Manhattan
}

public enum IndexStatus
{
    Ready,
    Initializing,
    Error,
    Deleting
}

#endregion

#region Vector Point Models

public class VectorPoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public float[] Vector { get; set; } = Array.Empty<float>();
    public Dictionary<string, object> Payload { get; set; } = new();
}

public class VectorPointWithScore : VectorPoint
{
    public float Score { get; set; }
}

#endregion

#region Collection/Index Models

public class CollectionInfo
{
    public string Name { get; set; } = string.Empty;
    public int VectorSize { get; set; }
    public DistanceMetric Distance { get; set; }
    public long PointsCount { get; set; }
    public IndexStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class CreateCollectionRequest
{
    public string Name { get; set; } = string.Empty;
    public int VectorSize { get; set; }
    public DistanceMetric Distance { get; set; } = DistanceMetric.Cosine;
    public Dictionary<string, object> Config { get; set; } = new();
}

#endregion

#region Upsert Models

public class UpsertRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<VectorPoint> Points { get; set; } = new();
    public bool Wait { get; set; } = true;
}

public class UpsertResponse
{
    public bool Success { get; set; }
    public int ProcessedCount { get; set; }
    public List<string> ProcessedIds { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public TimeSpan Duration { get; set; }
}

#endregion

#region Search Models

public class SearchRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public float[] QueryVector { get; set; } = Array.Empty<float>();
    public int Limit { get; set; } = 10;
    public float? ScoreThreshold { get; set; }
    public SearchFilter? Filter { get; set; }
    public bool WithPayload { get; set; } = true;
    public bool WithVector { get; set; } = false;
}

public class SearchResponse
{
    public List<VectorPointWithScore> Results { get; set; } = new();
    public int TotalFound { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class SearchFilter
{
    public List<FilterCondition> Must { get; set; } = new();
    public List<FilterCondition> Should { get; set; } = new();
    public List<FilterCondition> MustNot { get; set; } = new();
}

public class FilterCondition
{
    public string Key { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public object? Value { get; set; }
}

public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    In,
    NotIn,
    Contains,
    Range
}

#endregion

#region Batch Search Models

public class BatchSearchRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<float[]> QueryVectors { get; set; } = new();
    public int Limit { get; set; } = 10;
    public float? ScoreThreshold { get; set; }
    public SearchFilter? Filter { get; set; }
}

public class BatchSearchResponse
{
    public List<SearchResponse> Results { get; set; } = new();
    public int TotalQueries { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Delete Models

public class DeleteRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<string>? PointIds { get; set; }
    public SearchFilter? Filter { get; set; }
}

public class DeleteResponse
{
    public bool Success { get; set; }
    public int DeletedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

#endregion

#region Retrieve Models

public class RetrieveRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<string> PointIds { get; set; } = new();
    public bool WithPayload { get; set; } = true;
    public bool WithVector { get; set; } = true;
}

public class RetrieveResponse
{
    public List<VectorPoint> Points { get; set; } = new();
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

#endregion

#region Scroll/Pagination Models

public class ScrollRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public int Limit { get; set; } = 100;
    public string? Offset { get; set; }
    public SearchFilter? Filter { get; set; }
    public bool WithPayload { get; set; } = true;
    public bool WithVector { get; set; } = false;
}

public class ScrollResponse
{
    public List<VectorPoint> Points { get; set; } = new();
    public string? NextOffset { get; set; }
    public bool HasMore { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Update Models

public class UpdatePayloadRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<string> PointIds { get; set; } = new();
    public Dictionary<string, object> Payload { get; set; } = new();
    public bool Overwrite { get; set; } = false;
}

public class UpdatePayloadResponse
{
    public bool Success { get; set; }
    public int UpdatedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

#endregion

#region Count Models

public class CountRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public SearchFilter? Filter { get; set; }
    public bool Exact { get; set; } = false;
}

public class CountResponse
{
    public long Count { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Recommendations Models

public class RecommendRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<string> PositiveIds { get; set; } = new();
    public List<string> NegativeIds { get; set; } = new();
    public int Limit { get; set; } = 10;
    public SearchFilter? Filter { get; set; }
}

public class RecommendResponse
{
    public List<VectorPointWithScore> Results { get; set; } = new();
    public bool Success { get; set; }
}

#endregion

#region Snapshot Models

public class SnapshotRequest
{
    public string CollectionName { get; set; } = string.Empty;
}

public class SnapshotResponse
{
    public string SnapshotName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Success { get; set; }
}

#endregion

#region Statistics Models

public class CollectionStatistics
{
    public string CollectionName { get; set; } = string.Empty;
    public long TotalPoints { get; set; }
    public long VectorsDimension { get; set; }
    public DistanceMetric Distance { get; set; }
    public IndexStatus Status { get; set; }
    public long IndexedVectorsCount { get; set; }
    public Dictionary<string, long> PayloadSchema { get; set; } = new();
    public long StorageSize { get; set; }
}

#endregion

#region Configuration Models

public class VectorDatabaseSettings
{
    public VectorDatabaseProvider DefaultProvider { get; set; } = VectorDatabaseProvider.InMemory;
    
    // Qdrant Settings
    public string? QdrantHost { get; set; }
    public int QdrantPort { get; set; } = 6334;
    public string? QdrantApiKey { get; set; }
    public bool QdrantUseTls { get; set; } = false;
    
    // Pinecone Settings
    public string? PineconeApiKey { get; set; }
    public string? PineconeEnvironment { get; set; }
    public string? PineconeProjectId { get; set; }
    
    // In-Memory Settings
    public int InMemoryMaxPoints { get; set; } = 100000;
    public bool EnablePersistence { get; set; } = false;
    public string PersistencePath { get; set; } = "data";
    
    // General Settings
    public int DefaultLimit { get; set; } = 10;
    public int MaxBatchSize { get; set; } = 1000;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public bool EnableMetrics { get; set; } = true;
}

#endregion

#region Cluster Models

public class ClusterInfo
{
    public string Status { get; set; } = string.Empty;
    public List<NodeInfo> Nodes { get; set; } = new();
    public string Version { get; set; } = string.Empty;
}

public class NodeInfo
{
    public string Id { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsLeader { get; set; }
    public string Status { get; set; } = string.Empty;
}

#endregion

#region Hybrid Search Models

public class HybridSearchRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public float[] QueryVector { get; set; } = Array.Empty<float>();
    public string? QueryText { get; set; }
    public int Limit { get; set; } = 10;
    public SearchFilter? Filter { get; set; }
    public float VectorWeight { get; set; } = 0.7f;
    public float TextWeight { get; set; } = 0.3f;
}

#endregion

#region Index Management Models

public class IndexConfiguration
{
    public string Name { get; set; } = string.Empty;
    public int Dimension { get; set; }
    public DistanceMetric Metric { get; set; } = DistanceMetric.Cosine;
    public int Replicas { get; set; } = 1;
    public Dictionary<string, object> AdditionalConfig { get; set; } = new();
}

public class IndexOptimizationRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public bool Wait { get; set; } = true;
}

#endregion

#region Analytics Models

public class VectorDatabaseAnalytics
{
    public int TotalCollections { get; set; }
    public long TotalPoints { get; set; }
    public long TotalSearches { get; set; }
    public long TotalUpserts { get; set; }
    public long TotalDeletes { get; set; }
    public TimeSpan AverageSearchTime { get; set; }
    public Dictionary<string, long> CollectionSizes { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

#endregion
