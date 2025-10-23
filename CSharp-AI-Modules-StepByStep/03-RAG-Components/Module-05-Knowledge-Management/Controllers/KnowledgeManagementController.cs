using AI.KnowledgeManagement.Models;
using AI.KnowledgeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.KnowledgeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KnowledgeManagementController : ControllerBase
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private readonly IDocumentService _documentService;
    private readonly IVersioningService _versioningService;
    private readonly IAccessControlService _accessControlService;
    private readonly IKnowledgeSearchService _searchService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ISyncService _syncService;
    private readonly IChangeTrackingService _changeTrackingService;
    private readonly ILogger<KnowledgeManagementController> _logger;

    public KnowledgeManagementController(
        IKnowledgeBaseService knowledgeBaseService,
        IDocumentService documentService,
        IVersioningService versioningService,
        IAccessControlService accessControlService,
        IKnowledgeSearchService searchService,
        IAnalyticsService analyticsService,
        ISyncService syncService,
        IChangeTrackingService changeTrackingService,
        ILogger<KnowledgeManagementController> logger)
    {
        _knowledgeBaseService = knowledgeBaseService;
        _documentService = documentService;
        _versioningService = versioningService;
        _accessControlService = accessControlService;
        _searchService = searchService;
        _analyticsService = analyticsService;
        _syncService = syncService;
        _changeTrackingService = changeTrackingService;
        _logger = logger;
    }

    #region Knowledge Base Management

    [HttpPost("knowledge-bases")]
    public async Task<ActionResult<ApiResponse<KnowledgeBase>>> CreateKnowledgeBase(
        [FromBody] CreateKnowledgeBaseRequest request)
    {
        var kb = await _knowledgeBaseService.CreateKnowledgeBaseAsync(request);
        return Ok(ApiResponse<KnowledgeBase>.Success(kb, "Knowledge base created successfully"));
    }

    [HttpGet("knowledge-bases/{id}")]
    public async Task<ActionResult<ApiResponse<KnowledgeBase>>> GetKnowledgeBase(string id)
    {
        var kb = await _knowledgeBaseService.GetKnowledgeBaseAsync(id);
        if (kb == null)
            return NotFound(ApiResponse<KnowledgeBase>.Failure("Knowledge base not found"));

        return Ok(ApiResponse<KnowledgeBase>.Success(kb));
    }

    [HttpGet("knowledge-bases")]
    public async Task<ActionResult<ApiResponse<PagedResult<KnowledgeBase>>>> ListKnowledgeBases(
        [FromQuery] string? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _knowledgeBaseService.ListKnowledgeBasesAsync(userId, page, pageSize);
        return Ok(ApiResponse<PagedResult<KnowledgeBase>>.Success(result));
    }

    [HttpPut("knowledge-bases/{id}")]
    public async Task<ActionResult<ApiResponse<KnowledgeBase>>> UpdateKnowledgeBase(
        string id,
        [FromBody] CreateKnowledgeBaseRequest request)
    {
        var kb = await _knowledgeBaseService.UpdateKnowledgeBaseAsync(id, request);
        if (kb == null)
            return NotFound(ApiResponse<KnowledgeBase>.Failure("Knowledge base not found"));

        return Ok(ApiResponse<KnowledgeBase>.Success(kb, "Knowledge base updated successfully"));
    }

    [HttpDelete("knowledge-bases/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteKnowledgeBase(string id)
    {
        var result = await _knowledgeBaseService.DeleteKnowledgeBaseAsync(id);
        if (!result)
            return NotFound(ApiResponse<bool>.Failure("Knowledge base not found"));

        return Ok(ApiResponse<bool>.Success(true, "Knowledge base deleted successfully"));
    }

    #endregion

    #region Document Management

    [HttpPost("documents")]
    public async Task<ActionResult<ApiResponse<Document>>> CreateDocument(
        [FromBody] CreateDocumentRequest request)
    {
        var document = await _documentService.CreateDocumentAsync(request);
        return Ok(ApiResponse<Document>.Success(document, "Document created successfully"));
    }

    [HttpGet("documents/{id}")]
    public async Task<ActionResult<ApiResponse<Document>>> GetDocument(string id)
    {
        var document = await _documentService.GetDocumentAsync(id);
        if (document == null)
            return NotFound(ApiResponse<Document>.Failure("Document not found"));

        return Ok(ApiResponse<Document>.Success(document));
    }

    [HttpGet("knowledge-bases/{knowledgeBaseId}/documents")]
    public async Task<ActionResult<ApiResponse<PagedResult<Document>>>> ListDocuments(
        string knowledgeBaseId,
        [FromQuery] DocumentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _documentService.ListDocumentsAsync(knowledgeBaseId, status, page, pageSize);
        return Ok(ApiResponse<PagedResult<Document>>.Success(result));
    }

    [HttpPut("documents/{id}")]
    public async Task<ActionResult<ApiResponse<Document>>> UpdateDocument(
        string id,
        [FromBody] UpdateDocumentRequest request)
    {
        var document = await _documentService.UpdateDocumentAsync(id, request);
        if (document == null)
            return NotFound(ApiResponse<Document>.Failure("Document not found"));

        return Ok(ApiResponse<Document>.Success(document, "Document updated successfully"));
    }

    [HttpPost("documents/{id}/publish")]
    public async Task<ActionResult<ApiResponse<Document>>> PublishDocument(string id, [FromQuery] string publishedBy)
    {
        var document = await _documentService.PublishDocumentAsync(id, publishedBy);
        if (document == null)
            return NotFound(ApiResponse<Document>.Failure("Document not found"));

        return Ok(ApiResponse<Document>.Success(document, "Document published successfully"));
    }

    [HttpPost("documents/{id}/archive")]
    public async Task<ActionResult<ApiResponse<Document>>> ArchiveDocument(string id)
    {
        var document = await _documentService.ArchiveDocumentAsync(id);
        if (document == null)
            return NotFound(ApiResponse<Document>.Failure("Document not found"));

        return Ok(ApiResponse<Document>.Success(document, "Document archived successfully"));
    }

    [HttpDelete("documents/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDocument(string id)
    {
        var result = await _documentService.DeleteDocumentAsync(id);
        if (!result)
            return NotFound(ApiResponse<bool>.Failure("Document not found"));

        return Ok(ApiResponse<bool>.Success(true, "Document deleted successfully"));
    }

    #endregion

    #region Versioning

    [HttpGet("documents/{documentId}/versions")]
    public async Task<ActionResult<ApiResponse<List<DocumentVersion>>>> GetDocumentVersions(string documentId)
    {
        var versions = await _versioningService.GetDocumentVersionsAsync(documentId);
        return Ok(ApiResponse<List<DocumentVersion>>.Success(versions));
    }

    [HttpGet("versions/{versionId}")]
    public async Task<ActionResult<ApiResponse<DocumentVersion>>> GetVersion(string versionId)
    {
        var version = await _versioningService.GetVersionAsync(versionId);
        if (version == null)
            return NotFound(ApiResponse<DocumentVersion>.Failure("Version not found"));

        return Ok(ApiResponse<DocumentVersion>.Success(version));
    }

    [HttpPost("documents/{documentId}/versions/{versionId}/restore")]
    public async Task<ActionResult<ApiResponse<Document>>> RestoreVersion(
        string documentId,
        string versionId,
        [FromQuery] string restoredBy)
    {
        var document = await _versioningService.RestoreVersionAsync(documentId, versionId, restoredBy);
        if (document == null)
            return NotFound(ApiResponse<Document>.Failure("Version not found"));

        return Ok(ApiResponse<Document>.Success(document, "Version restored successfully"));
    }

    [HttpGet("versions/compare")]
    public async Task<ActionResult<ApiResponse<VersionComparisonResult>>> CompareVersions(
        [FromQuery] string version1Id,
        [FromQuery] string version2Id)
    {
        var result = await _versioningService.CompareVersionsAsync(version1Id, version2Id);
        if (result == null)
            return NotFound(ApiResponse<VersionComparisonResult>.Failure("Versions not found"));

        return Ok(ApiResponse<VersionComparisonResult>.Success(result));
    }

    #endregion

    #region Access Control

    [HttpPost("access/grant")]
    public async Task<ActionResult<ApiResponse<AccessPermission>>> GrantAccess(
        [FromBody] GrantAccessRequest request,
        [FromQuery] string grantedBy)
    {
        var permission = await _accessControlService.GrantAccessAsync(request, grantedBy);
        return Ok(ApiResponse<AccessPermission>.Success(permission, "Access granted successfully"));
    }

    [HttpDelete("access/{permissionId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeAccess(string permissionId)
    {
        var result = await _accessControlService.RevokeAccessAsync(permissionId);
        if (!result)
            return NotFound(ApiResponse<bool>.Failure("Permission not found"));

        return Ok(ApiResponse<bool>.Success(true, "Access revoked successfully"));
    }

    [HttpGet("access/check")]
    public async Task<ActionResult<ApiResponse<AccessCheckResult>>> CheckAccess(
        [FromQuery] string resourceId,
        [FromQuery] string userId,
        [FromQuery] AccessLevel requiredLevel)
    {
        var result = await _accessControlService.CheckAccessAsync(resourceId, userId, requiredLevel);
        return Ok(ApiResponse<AccessCheckResult>.Success(result));
    }

    [HttpGet("users/{userId}/permissions")]
    public async Task<ActionResult<ApiResponse<List<AccessPermission>>>> GetUserPermissions(string userId)
    {
        var permissions = await _accessControlService.GetUserPermissionsAsync(userId);
        return Ok(ApiResponse<List<AccessPermission>>.Success(permissions));
    }

    [HttpGet("resources/{resourceId}/permissions")]
    public async Task<ActionResult<ApiResponse<List<AccessPermission>>>> GetResourcePermissions(string resourceId)
    {
        var permissions = await _accessControlService.GetResourcePermissionsAsync(resourceId);
        return Ok(ApiResponse<List<AccessPermission>>.Success(permissions));
    }

    #endregion

    #region Search

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<KnowledgeSearchResponse>>> Search(
        [FromBody] KnowledgeSearchRequest request)
    {
        var result = await _searchService.SearchAsync(request);
        return Ok(ApiResponse<KnowledgeSearchResponse>.Success(result));
    }

    [HttpGet("knowledge-bases/{knowledgeBaseId}/search/tags")]
    public async Task<ActionResult<ApiResponse<List<Document>>>> SearchByTags(
        string knowledgeBaseId,
        [FromQuery] List<string> tags)
    {
        var documents = await _searchService.SearchByTagsAsync(knowledgeBaseId, tags);
        return Ok(ApiResponse<List<Document>>.Success(documents));
    }

    #endregion

    #region Analytics

    [HttpGet("analytics/documents/{documentId}")]
    public async Task<ActionResult<ApiResponse<DocumentAnalytics>>> GetDocumentAnalytics(string documentId)
    {
        var analytics = await _analyticsService.GetDocumentAnalyticsAsync(documentId);
        return Ok(ApiResponse<DocumentAnalytics>.Success(analytics));
    }

    [HttpPost("analytics/knowledge-bases")]
    public async Task<ActionResult<ApiResponse<KnowledgeBaseAnalytics>>> GetKnowledgeBaseAnalytics(
        [FromBody] AnalyticsQuery query)
    {
        var analytics = await _analyticsService.GetKnowledgeBaseAnalyticsAsync(query);
        return Ok(ApiResponse<KnowledgeBaseAnalytics>.Success(analytics));
    }

    #endregion

    #region Sync

    [HttpPost("sync")]
    public async Task<ActionResult<ApiResponse<SyncJob>>> StartSync(
        [FromBody] SyncRequest request,
        [FromQuery] string startedBy)
    {
        var job = await _syncService.StartSyncAsync(request, startedBy);
        return Ok(ApiResponse<SyncJob>.Success(job, "Sync job started successfully"));
    }

    [HttpGet("sync/{jobId}")]
    public async Task<ActionResult<ApiResponse<SyncJob>>> GetSyncJob(string jobId)
    {
        var job = await _syncService.GetSyncJobAsync(jobId);
        if (job == null)
            return NotFound(ApiResponse<SyncJob>.Failure("Sync job not found"));

        return Ok(ApiResponse<SyncJob>.Success(job));
    }

    [HttpGet("knowledge-bases/{knowledgeBaseId}/sync")]
    public async Task<ActionResult<ApiResponse<List<SyncJob>>>> GetSyncJobs(string knowledgeBaseId)
    {
        var jobs = await _syncService.GetSyncJobsAsync(knowledgeBaseId);
        return Ok(ApiResponse<List<SyncJob>>.Success(jobs));
    }

    #endregion

    #region Change Tracking

    [HttpGet("changes/{resourceId}")]
    public async Task<ActionResult<ApiResponse<List<ChangeLog>>>> GetChangeHistory(string resourceId)
    {
        var changes = await _changeTrackingService.GetChangeHistoryAsync(resourceId);
        return Ok(ApiResponse<List<ChangeLog>>.Success(changes));
    }

    [HttpGet("users/{userId}/activity")]
    public async Task<ActionResult<ApiResponse<List<ChangeLog>>>> GetUserActivity(
        string userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var activity = await _changeTrackingService.GetUserActivityAsync(userId, startDate, endDate);
        return Ok(ApiResponse<List<ChangeLog>>.Success(activity));
    }

    #endregion
}
