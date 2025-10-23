using AI.KnowledgeManagement.Models;
using AI.KnowledgeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace AI.KnowledgeManagement.Services;

public interface IVersioningService
{
    Task<DocumentVersion> CreateVersionAsync(Document document, string createdBy, string? notes = null);
    Task<DocumentVersion?> GetVersionAsync(string versionId);
    Task<List<DocumentVersion>> GetDocumentVersionsAsync(string documentId);
    Task<Document?> RestoreVersionAsync(string documentId, int versionNumber, string restoredBy);
    Task<VersionComparisonResult> CompareVersionsAsync(string documentId, int version1, int version2);
    Task<bool> DeleteOldVersionsAsync(string documentId, int keepCount);
}

public class VersioningService : IVersioningService
{
    private readonly KnowledgeDbContext _context;
    private readonly ILogger<VersioningService> _logger;

    public VersioningService(
        KnowledgeDbContext context,
        ILogger<VersioningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentVersion> CreateVersionAsync(Document document, string createdBy, string? notes = null)
    {
        var version = new DocumentVersion
        {
            DocumentId = document.Id,
            VersionNumber = document.CurrentVersion,
            Title = document.Title,
            Content = document.Content,
            Summary = document.Summary,
            Status = document.Status == DocumentStatus.Published ?
                VersionStatus.Published : VersionStatus.Draft,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            VersionNotes = notes,
            Metadata = document.Metadata,
            Tags = document.Tags,
            VectorId = document.VectorId
        };

        _context.DocumentVersions.Add(version);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Version created: Document={DocumentId}, Version={VersionNumber}",
            document.Id, version.VersionNumber);

        return version;
    }

    public async Task<DocumentVersion?> GetVersionAsync(string versionId)
    {
        return await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.Id == versionId);
    }

    public async Task<List<DocumentVersion>> GetDocumentVersionsAsync(string documentId)
    {
        return await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<Document?> RestoreVersionAsync(string documentId, int versionNumber, string restoredBy)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
            return null;

        var version = await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == versionNumber);

        if (version == null)
            return null;

        // Create a new version from current state
        await CreateVersionAsync(document, restoredBy, $"Backup before restoring to v{versionNumber}");

        // Restore from selected version
        document.Title = version.Title;
        document.Content = version.Content;
        document.Summary = version.Summary;
        document.Tags = version.Tags;
        document.Metadata = version.Metadata;
        document.UpdatedAt = DateTime.UtcNow;
        document.CurrentVersion++;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Version restored: Document={DocumentId}, RestoredFrom={VersionNumber}, NewVersion={CurrentVersion}",
            documentId, versionNumber, document.CurrentVersion);

        return document;
    }

    public async Task<VersionComparisonResult> CompareVersionsAsync(
        string documentId,
        int version1,
        int version2)
    {
        var v1 = await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == version1);

        var v2 = await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == version2);

        if (v1 == null || v2 == null)
        {
            throw new ArgumentException("One or both versions not found");
        }

        var result = new VersionComparisonResult
        {
            OldVersion = v1,
            NewVersion = v2,
            Differences = new List<VersionDiff>()
        };

        // Compare fields
        if (v1.Title != v2.Title)
        {
            result.Differences.Add(new VersionDiff
            {
                Field = "Title",
                OldValue = v1.Title,
                NewValue = v2.Title,
                ChangeType = "Modified"
            });
        }

        if (v1.Content != v2.Content)
        {
            result.Differences.Add(new VersionDiff
            {
                Field = "Content",
                OldValue = v1.Content.Length > 100 ? v1.Content.Substring(0, 100) + "..." : v1.Content,
                NewValue = v2.Content.Length > 100 ? v2.Content.Substring(0, 100) + "..." : v2.Content,
                ChangeType = "Modified"
            });
        }

        // Calculate similarity
        result.SimilarityScore = CalculateSimilarity(v1.Content, v2.Content);

        _logger.LogInformation(
            "Versions compared: Document={DocumentId}, V1={V1}, V2={V2}, Diffs={DiffCount}",
            documentId, version1, version2, result.Differences.Count);

        return result;
    }

    public async Task<bool> DeleteOldVersionsAsync(string documentId, int keepCount)
    {
        var versions = await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();

        if (versions.Count <= keepCount)
            return false;

        var versionsToDelete = versions.Skip(keepCount).ToList();

        _context.DocumentVersions.RemoveRange(versionsToDelete);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Old versions deleted: Document={DocumentId}, Deleted={Count}",
            documentId, versionsToDelete.Count);

        return true;
    }

    private float CalculateSimilarity(string text1, string text2)
    {
        // Simple Jaccard similarity
        var words1 = text1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var words2 = text2.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();

        return union > 0 ? (float)intersection / union : 0;
    }
}

public interface ISyncService
{
    Task<SyncJob> StartSyncAsync(SyncRequest request);
    Task<SyncJob?> GetSyncJobAsync(string jobId);
    Task<List<SyncJob>> GetSyncJobsAsync(string knowledgeBaseId);
}

public class SyncService : ISyncService
{
    private readonly KnowledgeDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        KnowledgeDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<SyncService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<SyncJob> StartSyncAsync(SyncRequest request)
    {
        var syncJob = new SyncJob
        {
            KnowledgeBaseId = request.KnowledgeBaseId,
            Status = SyncStatus.Pending,
            StartedAt = DateTime.UtcNow
        };

        _context.SyncJobs.Add(syncJob);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Sync job created: {JobId}, KB={KB}", syncJob.Id, request.KnowledgeBaseId);

        // Start background sync (in production, use Hangfire or similar)
        _ = Task.Run(async () => await ExecuteSyncAsync(syncJob.Id, request));

        return syncJob;
    }

    public async Task<SyncJob?> GetSyncJobAsync(string jobId)
    {
        return await _context.SyncJobs.FindAsync(jobId);
    }

    public async Task<List<SyncJob>> GetSyncJobsAsync(string knowledgeBaseId)
    {
        return await _context.SyncJobs
            .Where(j => j.KnowledgeBaseId == knowledgeBaseId)
            .OrderByDescending(j => j.StartedAt)
            .Take(20)
            .ToListAsync();
    }

    private async Task ExecuteSyncAsync(string jobId, SyncRequest request)
    {
        var syncJob = await _context.SyncJobs.FindAsync(jobId);
        if (syncJob == null)
            return;

        try
        {
            syncJob.Status = SyncStatus.InProgress;
            await _context.SaveChangesAsync();

            // Get documents to sync
            var query = _context.Documents
                .Where(d => d.KnowledgeBaseId == request.KnowledgeBaseId);

            if (request.DocumentIds != null && request.DocumentIds.Any())
            {
                query = query.Where(d => request.DocumentIds.Contains(d.Id));
            }

            var documents = await query.ToListAsync();
            syncJob.TotalDocuments = documents.Count;

            // Process each document
            foreach (var document in documents)
            {
                try
                {
                    // Sync logic here (e.g., update embeddings, sync to vector DB)
                    await Task.Delay(100); // Simulate processing

                    syncJob.ProcessedDocuments++;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    syncJob.FailedDocuments++;
                    syncJob.ErrorMessages.Add($"Document {document.Id}: {ex.Message}");
                    _logger.LogError(ex, "Sync failed for document: {DocumentId}", document.Id);
                }
            }

            syncJob.Status = SyncStatus.Completed;
            syncJob.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Sync completed: Job={JobId}, Processed={Processed}, Failed={Failed}",
                jobId, syncJob.ProcessedDocuments, syncJob.FailedDocuments);
        }
        catch (Exception ex)
        {
            syncJob.Status = SyncStatus.Failed;
            syncJob.ErrorMessages.Add($"Sync job failed: {ex.Message}");
            _logger.LogError(ex, "Sync job failed: {JobId}", jobId);
        }
        finally
        {
            await _context.SaveChangesAsync();
        }
    }
}

public interface IChangeTrackingService
{
    Task<ChangeLog> TrackChangeAsync(ChangeLog changeLog);
    Task<List<ChangeLog>> GetChangeHistoryAsync(string resourceId, int limit = 50);
    Task<List<ChangeLog>> GetUserActivityAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
}

public class ChangeTrackingService : IChangeTrackingService
{
    private readonly KnowledgeDbContext _context;
    private readonly ILogger<ChangeTrackingService> _logger;

    public ChangeTrackingService(
        KnowledgeDbContext context,
        ILogger<ChangeTrackingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChangeLog> TrackChangeAsync(ChangeLog changeLog)
    {
        _context.ChangeLogs.Add(changeLog);
        await _context.SaveChangesAsync();

        _logger.LogDebug(
            "Change tracked: Resource={ResourceId}, Type={ChangeType}, User={UserId}",
            changeLog.ResourceId, changeLog.ChangeType, changeLog.ChangedBy);

        return changeLog;
    }

    public async Task<List<ChangeLog>> GetChangeHistoryAsync(string resourceId, int limit = 50)
    {
        return await _context.ChangeLogs
            .Where(cl => cl.ResourceId == resourceId)
            .OrderByDescending(cl => cl.ChangedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<ChangeLog>> GetUserActivityAsync(
        string userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.ChangeLogs
            .Where(cl => cl.ChangedBy == userId);

        if (startDate.HasValue)
        {
            query = query.Where(cl => cl.ChangedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(cl => cl.ChangedAt <= endDate.Value);
        }

        return await query
            .OrderByDescending(cl => cl.ChangedAt)
            .Take(100)
            .ToListAsync();
    }
}
