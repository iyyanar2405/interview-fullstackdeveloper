namespace AI.KnowledgeManagement.Models;

#region Enums

public enum DocumentStatus
{
    Draft,
    Published,
    Archived,
    Deleted,
    UnderReview
}

public enum VersionStatus
{
    Draft,
    Published,
    Archived
}

public enum AccessLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Admin = 8,
    Owner = 16
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public enum ChangeType
{
    Created,
    Updated,
    Deleted,
    Restored,
    VersionCreated
}

#endregion

#region Knowledge Base Models

public class KnowledgeBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public KnowledgeBaseSettings Settings { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class KnowledgeBaseSettings
{
    public bool EnableVersioning { get; set; } = true;
    public int MaxVersionsPerDocument { get; set; } = 10;
    public bool EnableAccessControl { get; set; } = true;
    public bool EnableAnalytics { get; set; } = true;
    public bool AutoArchiveOldVersions { get; set; } = true;
    public int ArchiveAfterDays { get; set; } = 90;
}

public class CreateKnowledgeBaseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; } = new();
}

#endregion

#region Document Models

public class Document
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
    public int CurrentVersion { get; set; } = 1;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string? ParentDocumentId { get; set; }
    public List<string> RelatedDocumentIds { get; set; } = new();
    public string? VectorId { get; set; }
    public float[]? Embedding { get; set; }
}

public class CreateDocumentRequest
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool AutoPublish { get; set; } = false;
}

public class UpdateDocumentRequest
{
    public string DocumentId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public List<string>? Tags { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public bool CreateNewVersion { get; set; } = true;
    public string? VersionNotes { get; set; }
}

#endregion

#region Version Models

public class DocumentVersion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DocumentId { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public VersionStatus Status { get; set; } = VersionStatus.Draft;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? VersionNotes { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string? VectorId { get; set; }
}

public class VersionComparisonResult
{
    public DocumentVersion OldVersion { get; set; } = new();
    public DocumentVersion NewVersion { get; set; } = new();
    public List<VersionDiff> Differences { get; set; } = new();
    public float SimilarityScore { get; set; }
}

public class VersionDiff
{
    public string Field { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; } = string.Empty; // Added, Modified, Removed
}

#endregion

#region Access Control Models

public class AccessPermission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ResourceId { get; set; } = string.Empty; // KnowledgeBase or Document ID
    public string ResourceType { get; set; } = string.Empty; // "KnowledgeBase" or "Document"
    public string UserId { get; set; } = string.Empty;
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Read;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public string GrantedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class GrantAccessRequest
{
    public string ResourceId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Read;
    public DateTime? ExpiresAt { get; set; }
}

public class AccessCheckResult
{
    public bool HasAccess { get; set; }
    public AccessLevel GrantedLevel { get; set; }
    public string? Reason { get; set; }
}

#endregion

#region Sync Models

public class SyncJob
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public SyncStatus Status { get; set; } = SyncStatus.Pending;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int TotalDocuments { get; set; }
    public int ProcessedDocuments { get; set; }
    public int FailedDocuments { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SyncRequest
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public List<string>? DocumentIds { get; set; }
    public bool FullSync { get; set; } = false;
    public bool RegenerateEmbeddings { get; set; } = false;
}

#endregion

#region Change Tracking Models

public class ChangeLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ResourceId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> OldValues { get; set; } = new();
    public Dictionary<string, object> NewValues { get; set; } = new();
    public string? Notes { get; set; }
}

#endregion

#region Analytics Models

public class SearchAnalytics
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int ResultsCount { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public List<string> ClickedDocumentIds { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DocumentAnalytics
{
    public string DocumentId { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int SearchAppearances { get; set; }
    public int ClickThroughCount { get; set; }
    public float ClickThroughRate { get; set; }
    public DateTime FirstViewed { get; set; }
    public DateTime LastViewed { get; set; }
    public List<string> TopQueries { get; set; } = new();
}

public class KnowledgeBaseAnalytics
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public int TotalDocuments { get; set; }
    public int PublishedDocuments { get; set; }
    public int DraftDocuments { get; set; }
    public int TotalVersions { get; set; }
    public int TotalSearches { get; set; }
    public int TotalViews { get; set; }
    public int ActiveUsers { get; set; }
    public float AverageResponseTime { get; set; }
    public List<string> TopQueries { get; set; } = new();
    public List<string> MostViewedDocuments { get; set; } = new();
    public Dictionary<string, int> SearchTrends { get; set; } = new();
}

public class AnalyticsQuery
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? UserId { get; set; }
    public string? DocumentId { get; set; }
}

#endregion

#region Search Models

public class KnowledgeSearchRequest
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public int TopK { get; set; } = 10;
    public float? ScoreThreshold { get; set; }
    public List<string>? Tags { get; set; }
    public DocumentStatus? Status { get; set; }
    public bool IncludeVersions { get; set; } = false;
    public string? UserId { get; set; } // For access control
}

public class KnowledgeSearchResponse
{
    public List<Document> Documents { get; set; } = new();
    public int TotalFound { get; set; }
    public string Query { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public string? AnalyticsId { get; set; }
}

#endregion

#region Export/Import Models

public class ExportRequest
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public List<string>? DocumentIds { get; set; }
    public bool IncludeVersions { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
    public string Format { get; set; } = "json"; // json, csv, markdown
}

public class ExportResult
{
    public string ExportId { get; set; } = Guid.NewGuid().ToString();
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public long FileSizeBytes { get; set; }
    public int DocumentCount { get; set; }
}

public class ImportRequest
{
    public string KnowledgeBaseId { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string Format { get; set; } = "json";
    public bool CreateNewVersions { get; set; } = true;
    public bool OverwriteExisting { get; set; } = false;
}

public class ImportResult
{
    public string ImportId { get; set; } = Guid.NewGuid().ToString();
    public int TotalDocuments { get; set; }
    public int ImportedDocuments { get; set; }
    public int SkippedDocuments { get; set; }
    public int FailedDocuments { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public bool Success { get; set; }
}

#endregion

#region Settings

public class KnowledgeManagementSettings
{
    public string DatabaseConnectionString { get; set; } = string.Empty;
    public string VectorDatabaseEndpoint { get; set; } = string.Empty;
    public string EmbeddingServiceEndpoint { get; set; } = string.Empty;
    public string BlobStorageConnectionString { get; set; } = string.Empty;
    public string BlobStorageContainer { get; set; } = "knowledge-exports";
    
    public bool EnableVersioning { get; set; } = true;
    public int MaxVersionsPerDocument { get; set; } = 10;
    
    public bool EnableAccessControl { get; set; } = true;
    public bool EnableAnalytics { get; set; } = true;
    
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 30;
    
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
    
    public string? RedisConnectionString { get; set; }
    public string? ElasticsearchEndpoint { get; set; }
}

#endregion

#region Response Models

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

#endregion
