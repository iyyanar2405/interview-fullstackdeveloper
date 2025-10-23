using AI.KnowledgeManagement.Models;
using AI.KnowledgeManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.KnowledgeManagement.Services;

public interface IKnowledgeBaseService
{
    Task<KnowledgeBase> CreateKnowledgeBaseAsync(CreateKnowledgeBaseRequest request, string ownerId);
    Task<KnowledgeBase?> GetKnowledgeBaseAsync(string id);
    Task<PagedResult<KnowledgeBase>> ListKnowledgeBasesAsync(int pageNumber, int pageSize, string? userId = null);
    Task<bool> UpdateKnowledgeBaseAsync(string id, CreateKnowledgeBaseRequest request);
    Task<bool> DeleteKnowledgeBaseAsync(string id);
}

public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly KnowledgeDbContext _context;
    private readonly KnowledgeManagementSettings _settings;
    private readonly ILogger<KnowledgeBaseService> _logger;

    public KnowledgeBaseService(
        KnowledgeDbContext context,
        IOptions<KnowledgeManagementSettings> settings,
        ILogger<KnowledgeBaseService> logger)
    {
        _context = context;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<KnowledgeBase> CreateKnowledgeBaseAsync(CreateKnowledgeBaseRequest request, string ownerId)
    {
        var knowledgeBase = new KnowledgeBase
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = ownerId,
            IsPublic = request.IsPublic,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Settings = new KnowledgeBaseSettings
            {
                EnableVersioning = _settings.EnableVersioning,
                MaxVersionsPerDocument = _settings.MaxVersionsPerDocument,
                EnableAccessControl = _settings.EnableAccessControl,
                EnableAnalytics = _settings.EnableAnalytics
            }
        };

        _context.KnowledgeBases.Add(knowledgeBase);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Knowledge base created: {Id}, Name: {Name}", knowledgeBase.Id, knowledgeBase.Name);

        return knowledgeBase;
    }

    public async Task<KnowledgeBase?> GetKnowledgeBaseAsync(string id)
    {
        return await _context.KnowledgeBases
            .FirstOrDefaultAsync(kb => kb.Id == id);
    }

    public async Task<PagedResult<KnowledgeBase>> ListKnowledgeBasesAsync(
        int pageNumber,
        int pageSize,
        string? userId = null)
    {
        var query = _context.KnowledgeBases.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(kb => kb.OwnerId == userId || kb.IsPublic);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(kb => kb.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<KnowledgeBase>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> UpdateKnowledgeBaseAsync(string id, CreateKnowledgeBaseRequest request)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return false;

        knowledgeBase.Name = request.Name;
        knowledgeBase.Description = request.Description;
        knowledgeBase.IsPublic = request.IsPublic;
        knowledgeBase.Tags = request.Tags;
        knowledgeBase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Knowledge base updated: {Id}", id);

        return true;
    }

    public async Task<bool> DeleteKnowledgeBaseAsync(string id)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return false;

        _context.KnowledgeBases.Remove(knowledgeBase);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Knowledge base deleted: {Id}", id);

        return true;
    }
}

public interface IDocumentService
{
    Task<Document> CreateDocumentAsync(CreateDocumentRequest request, string authorId);
    Task<Document?> GetDocumentAsync(string id);
    Task<PagedResult<Document>> ListDocumentsAsync(string knowledgeBaseId, int pageNumber, int pageSize, DocumentStatus? status = null);
    Task<Document?> UpdateDocumentAsync(UpdateDocumentRequest request, string userId);
    Task<bool> DeleteDocumentAsync(string id);
    Task<bool> PublishDocumentAsync(string id, string publishedBy);
    Task<bool> ArchiveDocumentAsync(string id);
}

public class DocumentService : IDocumentService
{
    private readonly KnowledgeDbContext _context;
    private readonly IVersioningService _versioningService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        KnowledgeDbContext context,
        IVersioningService versioningService,
        ILogger<DocumentService> logger)
    {
        _context = context;
        _versioningService = versioningService;
        _logger = logger;
    }

    public async Task<Document> CreateDocumentAsync(CreateDocumentRequest request, string authorId)
    {
        var document = new Document
        {
            KnowledgeBaseId = request.KnowledgeBaseId,
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary ?? "",
            AuthorId = authorId,
            Status = request.AutoPublish ? DocumentStatus.Published : DocumentStatus.Draft,
            Tags = request.Tags,
            Metadata = request.Metadata,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (request.AutoPublish)
        {
            document.PublishedAt = DateTime.UtcNow;
            document.PublishedBy = authorId;
        }

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // Create initial version
        await _versioningService.CreateVersionAsync(document, authorId, "Initial version");

        _logger.LogInformation("Document created: {Id}, Title: {Title}", document.Id, document.Title);

        return document;
    }

    public async Task<Document?> GetDocumentAsync(string id)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<PagedResult<Document>> ListDocumentsAsync(
        string knowledgeBaseId,
        int pageNumber,
        int pageSize,
        DocumentStatus? status = null)
    {
        var query = _context.Documents
            .Where(d => d.KnowledgeBaseId == knowledgeBaseId);

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(d => d.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Document>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Document?> UpdateDocumentAsync(UpdateDocumentRequest request, string userId)
    {
        var document = await _context.Documents.FindAsync(request.DocumentId);
        if (document == null)
            return null;

        // Create new version if requested
        if (request.CreateNewVersion)
        {
            await _versioningService.CreateVersionAsync(document, userId, request.VersionNotes);
        }

        // Update document
        if (request.Title != null)
            document.Title = request.Title;

        if (request.Content != null)
            document.Content = request.Content;

        if (request.Summary != null)
            document.Summary = request.Summary;

        if (request.Tags != null)
            document.Tags = request.Tags;

        if (request.Metadata != null)
            document.Metadata = request.Metadata;

        document.UpdatedAt = DateTime.UtcNow;

        if (request.CreateNewVersion)
        {
            document.CurrentVersion++;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Document updated: {Id}, Version: {Version}", document.Id, document.CurrentVersion);

        return document;
    }

    public async Task<bool> DeleteDocumentAsync(string id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
            return false;

        document.Status = DocumentStatus.Deleted;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Document deleted: {Id}", id);

        return true;
    }

    public async Task<bool> PublishDocumentAsync(string id, string publishedBy)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
            return false;

        document.Status = DocumentStatus.Published;
        document.PublishedAt = DateTime.UtcNow;
        document.PublishedBy = publishedBy;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Document published: {Id}", id);

        return true;
    }

    public async Task<bool> ArchiveDocumentAsync(string id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
            return false;

        document.Status = DocumentStatus.Archived;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Document archived: {Id}", id);

        return true;
    }
}

public interface IKnowledgeSearchService
{
    Task<KnowledgeSearchResponse> SearchAsync(KnowledgeSearchRequest request);
    Task<List<Document>> SearchByTagsAsync(string knowledgeBaseId, List<string> tags);
}

public class KnowledgeSearchService : IKnowledgeSearchService
{
    private readonly KnowledgeDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KnowledgeManagementSettings _settings;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<KnowledgeSearchService> _logger;

    public KnowledgeSearchService(
        KnowledgeDbContext context,
        IHttpClientFactory httpClientFactory,
        IOptions<KnowledgeManagementSettings> settings,
        IAnalyticsService analyticsService,
        ILogger<KnowledgeSearchService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public async Task<KnowledgeSearchResponse> SearchAsync(KnowledgeSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new KnowledgeSearchResponse
        {
            Query = request.Query
        };

        try
        {
            // Build query
            var query = _context.Documents
                .Where(d => d.KnowledgeBaseId == request.KnowledgeBaseId);

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status.Value);
            }

            if (request.Tags != null && request.Tags.Any())
            {
                query = query.Where(d => d.Tags.Any(t => request.Tags.Contains(t)));
            }

            // Simple text search (in production, use full-text search or vector search)
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(d =>
                    d.Title.Contains(request.Query) ||
                    d.Content.Contains(request.Query) ||
                    d.Summary.Contains(request.Query));
            }

            response.Documents = await query
                .OrderByDescending(d => d.UpdatedAt)
                .Take(request.TopK)
                .ToListAsync();

            response.TotalFound = response.Documents.Count;
            response.Success = true;

            stopwatch.Stop();
            response.Duration = stopwatch.Elapsed;

            // Track analytics
            if (_settings.EnableAnalytics && request.UserId != null)
            {
                var analyticsId = await _analyticsService.TrackSearchAsync(new SearchAnalytics
                {
                    KnowledgeBaseId = request.KnowledgeBaseId,
                    Query = request.Query,
                    UserId = request.UserId,
                    ResultsCount = response.TotalFound,
                    ResponseTime = response.Duration
                });

                response.AnalyticsId = analyticsId;
            }

            _logger.LogInformation(
                "Search completed: KB={KB}, Query='{Query}', Results={Count}, Duration={Duration}ms",
                request.KnowledgeBaseId, request.Query, response.TotalFound, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            response.Success = false;
            stopwatch.Stop();
            response.Duration = stopwatch.Elapsed;

            _logger.LogError(ex, "Search failed: KB={KB}, Query='{Query}'",
                request.KnowledgeBaseId, request.Query);
        }

        return response;
    }

    public async Task<List<Document>> SearchByTagsAsync(string knowledgeBaseId, List<string> tags)
    {
        return await _context.Documents
            .Where(d => d.KnowledgeBaseId == knowledgeBaseId &&
                       d.Tags.Any(t => tags.Contains(t)))
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync();
    }
}
