using AI.KnowledgeManagement.Models;
using AI.KnowledgeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace AI.KnowledgeManagement.Services;

public interface IAccessControlService
{
    Task<AccessPermission> GrantAccessAsync(GrantAccessRequest request, string grantedBy);
    Task<bool> RevokeAccessAsync(string permissionId);
    Task<AccessCheckResult> CheckAccessAsync(string resourceId, string userId, AccessLevel requiredLevel);
    Task<List<AccessPermission>> GetUserPermissionsAsync(string userId);
    Task<List<AccessPermission>> GetResourcePermissionsAsync(string resourceId);
}

public class AccessControlService : IAccessControlService
{
    private readonly KnowledgeDbContext _context;
    private readonly ILogger<AccessControlService> _logger;

    public AccessControlService(
        KnowledgeDbContext context,
        ILogger<AccessControlService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccessPermission> GrantAccessAsync(GrantAccessRequest request, string grantedBy)
    {
        var permission = new AccessPermission
        {
            ResourceId = request.ResourceId,
            ResourceType = request.ResourceType,
            UserId = request.UserId,
            AccessLevel = request.AccessLevel,
            GrantedBy = grantedBy,
            GrantedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt
        };

        _context.AccessPermissions.Add(permission);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Access granted: Resource={ResourceId}, User={UserId}, Level={Level}",
            request.ResourceId, request.UserId, request.AccessLevel);

        return permission;
    }

    public async Task<bool> RevokeAccessAsync(string permissionId)
    {
        var permission = await _context.AccessPermissions.FindAsync(permissionId);
        if (permission == null)
            return false;

        _context.AccessPermissions.Remove(permission);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Access revoked: Permission={PermissionId}", permissionId);

        return true;
    }

    public async Task<AccessCheckResult> CheckAccessAsync(
        string resourceId,
        string userId,
        AccessLevel requiredLevel)
    {
        // Check if user has permission
        var permission = await _context.AccessPermissions
            .Where(p => p.ResourceId == resourceId && p.UserId == userId)
            .Where(p => !p.ExpiresAt.HasValue || p.ExpiresAt.Value > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (permission == null)
        {
            // Check if resource is owned by user
            var document = await _context.Documents.FindAsync(resourceId);
            if (document != null && document.AuthorId == userId)
            {
                return new AccessCheckResult
                {
                    HasAccess = true,
                    GrantedLevel = AccessLevel.Owner,
                    Reason = "Owner"
                };
            }

            var kb = await _context.KnowledgeBases.FindAsync(resourceId);
            if (kb != null)
            {
                if (kb.OwnerId == userId)
                {
                    return new AccessCheckResult
                    {
                        HasAccess = true,
                        GrantedLevel = AccessLevel.Owner,
                        Reason = "Owner"
                    };
                }

                if (kb.IsPublic && requiredLevel == AccessLevel.Read)
                {
                    return new AccessCheckResult
                    {
                        HasAccess = true,
                        GrantedLevel = AccessLevel.Read,
                        Reason = "Public resource"
                    };
                }
            }

            return new AccessCheckResult
            {
                HasAccess = false,
                GrantedLevel = AccessLevel.None,
                Reason = "No permission found"
            };
        }

        var hasRequiredLevel = (permission.AccessLevel & requiredLevel) == requiredLevel;

        return new AccessCheckResult
        {
            HasAccess = hasRequiredLevel,
            GrantedLevel = permission.AccessLevel,
            Reason = hasRequiredLevel ? "Permission granted" : "Insufficient permissions"
        };
    }

    public async Task<List<AccessPermission>> GetUserPermissionsAsync(string userId)
    {
        return await _context.AccessPermissions
            .Where(p => p.UserId == userId)
            .Where(p => !p.ExpiresAt.HasValue || p.ExpiresAt.Value > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<List<AccessPermission>> GetResourcePermissionsAsync(string resourceId)
    {
        return await _context.AccessPermissions
            .Where(p => p.ResourceId == resourceId)
            .Where(p => !p.ExpiresAt.HasValue || p.ExpiresAt.Value > DateTime.UtcNow)
            .ToListAsync();
    }
}

public interface IAnalyticsService
{
    Task<string> TrackSearchAsync(SearchAnalytics analytics);
    Task TrackDocumentViewAsync(string documentId, string userId);
    Task TrackDocumentClickAsync(string analyticsId, string documentId);
    Task<DocumentAnalytics> GetDocumentAnalyticsAsync(string documentId);
    Task<KnowledgeBaseAnalytics> GetKnowledgeBaseAnalyticsAsync(AnalyticsQuery query);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly KnowledgeDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        KnowledgeDbContext context,
        ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> TrackSearchAsync(SearchAnalytics analytics)
    {
        _context.SearchAnalytics.Add(analytics);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Search tracked: Query='{Query}', Results={Count}", analytics.Query, analytics.ResultsCount);

        return analytics.Id;
    }

    public async Task TrackDocumentViewAsync(string documentId, string userId)
    {
        // In production, use a more efficient tracking mechanism
        var analytics = new SearchAnalytics
        {
            Query = "document-view",
            UserId = userId,
            Metadata = new Dictionary<string, object> { { "documentId", documentId } }
        };

        await TrackSearchAsync(analytics);
    }

    public async Task TrackDocumentClickAsync(string analyticsId, string documentId)
    {
        var analytics = await _context.SearchAnalytics.FindAsync(analyticsId);
        if (analytics != null)
        {
            analytics.ClickedDocumentIds.Add(documentId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DocumentAnalytics> GetDocumentAnalyticsAsync(string documentId)
    {
        var views = await _context.SearchAnalytics
            .Where(sa => sa.ClickedDocumentIds.Contains(documentId))
            .ToListAsync();

        var appearances = await _context.SearchAnalytics
            .Where(sa => sa.Metadata.ContainsKey("documentId") &&
                        sa.Metadata["documentId"].ToString() == documentId)
            .CountAsync();

        return new DocumentAnalytics
        {
            DocumentId = documentId,
            ViewCount = views.Count,
            SearchAppearances = appearances,
            ClickThroughCount = views.Count,
            ClickThroughRate = appearances > 0 ? (float)views.Count / appearances : 0,
            FirstViewed = views.Any() ? views.Min(v => v.Timestamp) : DateTime.MinValue,
            LastViewed = views.Any() ? views.Max(v => v.Timestamp) : DateTime.MinValue,
            TopQueries = views.Select(v => v.Query).Distinct().Take(10).ToList()
        };
    }

    public async Task<KnowledgeBaseAnalytics> GetKnowledgeBaseAnalyticsAsync(AnalyticsQuery query)
    {
        var analytics = new KnowledgeBaseAnalytics
        {
            KnowledgeBaseId = query.KnowledgeBaseId
        };

        // Document counts
        var docsQuery = _context.Documents
            .Where(d => d.KnowledgeBaseId == query.KnowledgeBaseId);

        analytics.TotalDocuments = await docsQuery.CountAsync();
        analytics.PublishedDocuments = await docsQuery.CountAsync(d => d.Status == DocumentStatus.Published);
        analytics.DraftDocuments = await docsQuery.CountAsync(d => d.Status == DocumentStatus.Draft);

        // Version count
        var docIds = await docsQuery.Select(d => d.Id).ToListAsync();
        analytics.TotalVersions = await _context.DocumentVersions
            .Where(v => docIds.Contains(v.DocumentId))
            .CountAsync();

        // Search analytics
        var searchQuery = _context.SearchAnalytics
            .Where(sa => sa.KnowledgeBaseId == query.KnowledgeBaseId);

        if (query.StartDate.HasValue)
        {
            searchQuery = searchQuery.Where(sa => sa.Timestamp >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            searchQuery = searchQuery.Where(sa => sa.Timestamp <= query.EndDate.Value);
        }

        analytics.TotalSearches = await searchQuery.CountAsync();

        var searches = await searchQuery.ToListAsync();

        analytics.TotalViews = searches.Sum(s => s.ClickedDocumentIds.Count);
        analytics.ActiveUsers = searches.Select(s => s.UserId).Distinct().Count();
        analytics.AverageResponseTime = searches.Any() ?
            (float)searches.Average(s => s.ResponseTime.TotalMilliseconds) : 0;

        analytics.TopQueries = searches
            .GroupBy(s => s.Query)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToList();

        return analytics;
    }
}
