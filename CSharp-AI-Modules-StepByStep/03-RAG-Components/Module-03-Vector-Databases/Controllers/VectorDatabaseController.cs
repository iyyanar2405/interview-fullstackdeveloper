using AI.VectorDatabases.Models;
using AI.VectorDatabases.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.VectorDatabases.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VectorDatabaseController : ControllerBase
{
    private readonly IVectorDatabaseServiceFactory _factory;
    private readonly SearchService _searchService;
    private readonly IndexManagementService _indexService;

    public VectorDatabaseController(
        IVectorDatabaseServiceFactory factory,
        SearchService searchService,
        IndexManagementService indexService)
    {
        _factory = factory;
        _searchService = searchService;
        _indexService = indexService;
    }

    #region Collection Management

    [HttpPost("collections")]
    public async Task<IActionResult> CreateCollection(
        [FromBody] CreateCollectionRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.CreateCollectionAsync(request);
        
        if (result)
            return Ok(new { success = true, message = $"Collection '{request.Name}' created successfully" });
        
        return BadRequest(new { success = false, message = $"Failed to create collection '{request.Name}'" });
    }

    [HttpDelete("collections/{collectionName}")]
    public async Task<IActionResult> DeleteCollection(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.DeleteCollectionAsync(collectionName);
        
        if (result)
            return Ok(new { success = true, message = $"Collection '{collectionName}' deleted successfully" });
        
        return NotFound(new { success = false, message = $"Collection '{collectionName}' not found" });
    }

    [HttpGet("collections/{collectionName}")]
    public async Task<IActionResult> GetCollectionInfo(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var info = await service.GetCollectionInfoAsync(collectionName);
        
        if (info != null)
            return Ok(info);
        
        return NotFound(new { success = false, message = $"Collection '{collectionName}' not found" });
    }

    [HttpGet("collections")]
    public async Task<IActionResult> ListCollections([FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var collections = await service.ListCollectionsAsync();
        return Ok(collections);
    }

    [HttpGet("collections/{collectionName}/statistics")]
    public async Task<IActionResult> GetCollectionStatistics(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var stats = await _indexService.GetIndexStatisticsAsync(collectionName, provider);
        return Ok(stats);
    }

    #endregion

    #region Vector Operations

    [HttpPost("vectors/upsert")]
    public async Task<IActionResult> UpsertVectors(
        [FromBody] UpsertRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.UpsertAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("vectors/retrieve")]
    public async Task<IActionResult> RetrieveVectors(
        [FromBody] RetrieveRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.RetrieveAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("vectors/delete")]
    public async Task<IActionResult> DeleteVectors(
        [FromBody] DeleteRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.DeleteAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("vectors/update-payload")]
    public async Task<IActionResult> UpdatePayload(
        [FromBody] UpdatePayloadRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.UpdatePayloadAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("vectors/count")]
    public async Task<IActionResult> CountVectors(
        [FromBody] CountRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.CountAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("vectors/scroll")]
    public async Task<IActionResult> ScrollVectors(
        [FromBody] ScrollRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.ScrollAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    #endregion

    #region Search Operations

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.SearchAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("search/batch")]
    public async Task<IActionResult> BatchSearch(
        [FromBody] BatchSearchRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var result = await service.BatchSearchAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("search/vector")]
    public async Task<IActionResult> VectorSearch(
        [FromQuery] string collectionName,
        [FromBody] float[] queryVector,
        [FromQuery] int limit = 10,
        [FromQuery] float? scoreThreshold = null,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _searchService.VectorSearchAsync(
            collectionName, 
            queryVector, 
            limit, 
            scoreThreshold,
            null,
            provider);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("search/hybrid")]
    public async Task<IActionResult> HybridSearch([FromBody] HybridSearchRequest request)
    {
        var result = await _searchService.HybridSearchAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("search/multi-vector")]
    public async Task<IActionResult> MultiVectorSearch(
        [FromQuery] string collectionName,
        [FromBody] List<float[]> queryVectors,
        [FromQuery] int limit = 10,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _searchService.MultiVectorSearchAsync(
            collectionName,
            queryVectors,
            limit,
            null,
            provider);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("recommend")]
    public async Task<IActionResult> Recommend([FromBody] RecommendRequest request)
    {
        var result = await _searchService.RecommendAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    #endregion

    #region Index Management

    [HttpPost("index/create")]
    public async Task<IActionResult> CreateIndex(
        [FromQuery] string collectionName,
        [FromQuery] int vectorSize,
        [FromQuery] DistanceMetric distance = DistanceMetric.Cosine,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _indexService.CreateIndexAsync(collectionName, vectorSize, distance, provider);
        
        if (result)
            return Ok(new { success = true, message = $"Index '{collectionName}' created successfully" });
        
        return BadRequest(new { success = false, message = $"Failed to create index '{collectionName}'" });
    }

    [HttpDelete("index/{collectionName}")]
    public async Task<IActionResult> DeleteIndex(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _indexService.DeleteIndexAsync(collectionName, provider);
        
        if (result)
            return Ok(new { success = true, message = $"Index '{collectionName}' deleted successfully" });
        
        return NotFound(new { success = false, message = $"Index '{collectionName}' not found" });
    }

    [HttpGet("index/{collectionName}")]
    public async Task<IActionResult> GetIndexInfo(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var info = await _indexService.GetIndexInfoAsync(collectionName, provider);
        
        if (info != null)
            return Ok(info);
        
        return NotFound(new { success = false, message = $"Index '{collectionName}' not found" });
    }

    [HttpGet("index")]
    public async Task<IActionResult> ListIndexes([FromQuery] VectorDatabaseProvider? provider = null)
    {
        var indexes = await _indexService.ListIndexesAsync(provider);
        return Ok(indexes);
    }

    [HttpGet("index/{collectionName}/size")]
    public async Task<IActionResult> GetIndexSize(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var size = await _indexService.GetIndexSizeAsync(collectionName, provider);
        return Ok(new { collectionName, sizeInBytes = size, sizeInMB = size / (1024.0 * 1024.0) });
    }

    [HttpPost("index/{collectionName}/optimize")]
    public async Task<IActionResult> OptimizeIndex(
        string collectionName,
        [FromBody] IndexOptimizationRequest request,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _indexService.OptimizeIndexAsync(request, provider);
        
        if (result)
            return Ok(new { success = true, message = $"Index '{collectionName}' optimized successfully" });
        
        return BadRequest(new { success = false, message = $"Failed to optimize index '{collectionName}'" });
    }

    [HttpPost("index/{collectionName}/recreate")]
    public async Task<IActionResult> RecreateIndex(
        string collectionName,
        [FromQuery] int newVectorSize,
        [FromQuery] DistanceMetric newDistance = DistanceMetric.Cosine,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var result = await _indexService.RecreateIndexAsync(collectionName, newVectorSize, newDistance, provider);
        
        if (result)
            return Ok(new { success = true, message = $"Index '{collectionName}' recreated successfully" });
        
        return BadRequest(new { success = false, message = $"Failed to recreate index '{collectionName}'" });
    }

    #endregion

    #region Analytics

    [HttpGet("analytics/{collectionName}")]
    public async Task<IActionResult> GetAnalytics(
        string collectionName,
        [FromQuery] VectorDatabaseProvider? provider = null)
    {
        var analytics = await _searchService.GetSearchAnalyticsAsync(collectionName, provider);
        return Ok(analytics);
    }

    #endregion

    #region Health Check

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "Vector Database API"
        });
    }

    #endregion
}
