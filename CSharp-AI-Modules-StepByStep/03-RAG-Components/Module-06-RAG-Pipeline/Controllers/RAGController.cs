using AI.RAGPipeline.Models;
using AI.RAGPipeline.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.RAGPipeline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RAGController : ControllerBase
{
    private readonly IRAGOrchestrator _orchestrator;
    private readonly IDocumentIngestionService _ingestionService;
    private readonly ILogger<RAGController> _logger;

    public RAGController(
        IRAGOrchestrator orchestrator,
        IDocumentIngestionService ingestionService,
        ILogger<RAGController> logger)
    {
        _orchestrator = orchestrator;
        _ingestionService = ingestionService;
        _logger = logger;
    }

    #region Document Ingestion

    [HttpPost("ingest")]
    public async Task<ActionResult<ApiResponse<DocumentIngestionResult>>> IngestDocument(
        [FromBody] DocumentIngestionRequest request)
    {
        try
        {
            var result = await _ingestionService.IngestDocumentAsync(request);
            return Ok(ApiResponse<DocumentIngestionResult>.SuccessResult(result, "Document ingested successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting document");
            return StatusCode(500, ApiResponse<DocumentIngestionResult>.FailureResult(
                "Error ingesting document", new List<string> { ex.Message }));
        }
    }

    [HttpPost("ingest/file")]
    public async Task<ActionResult<ApiResponse<DocumentIngestionResult>>> IngestDocumentFile(
        IFormFile file,
        [FromQuery] string? knowledgeBaseId = null,
        [FromQuery] ChunkingStrategy strategy = ChunkingStrategy.Recursive,
        [FromQuery] int chunkSize = 1000,
        [FromQuery] int chunkOverlap = 200)
    {
        try
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new DocumentIngestionRequest
            {
                FileName = file.FileName,
                FileContent = ms.ToArray(),
                ContentType = file.ContentType,
                KnowledgeBaseId = knowledgeBaseId,
                Config = new IngestionConfig
                {
                    Strategy = strategy,
                    ChunkSize = chunkSize,
                    ChunkOverlap = chunkOverlap,
                    ExtractMetadata = true,
                    GenerateEmbeddings = true,
                    StoreInVectorDb = true
                }
            };

            var result = await _ingestionService.IngestDocumentAsync(request);
            return Ok(ApiResponse<DocumentIngestionResult>.SuccessResult(result, "File ingested successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting file");
            return StatusCode(500, ApiResponse<DocumentIngestionResult>.FailureResult(
                "Error ingesting file", new List<string> { ex.Message }));
        }
    }

    #endregion

    #region Query Execution

    [HttpPost("query")]
    public async Task<ActionResult<ApiResponse<RAGResponse>>> ExecuteQuery([FromBody] RAGQuery query)
    {
        try
        {
            var response = await _orchestrator.ExecuteQueryAsync(query);
            return Ok(ApiResponse<RAGResponse>.SuccessResult(response, "Query executed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            return StatusCode(500, ApiResponse<RAGResponse>.FailureResult(
                "Error executing query", new List<string> { ex.Message }));
        }
    }

    [HttpPost("query/simple")]
    public async Task<ActionResult<ApiResponse<RAGResponse>>> ExecuteSimpleQuery(
        [FromBody] SimpleQueryRequest request)
    {
        try
        {
            var query = new RAGQuery
            {
                Query = request.Query,
                KnowledgeBaseId = request.KnowledgeBaseId,
                RetrievalConfig = new RetrievalConfig
                {
                    Strategy = request.Strategy ?? RAGStrategy.HybridSearch,
                    TopK = request.TopK ?? 5,
                    EnableReranking = true
                },
                GenerationConfig = new GenerationConfig
                {
                    Temperature = request.Temperature ?? 0.7f
                }
            };

            var response = await _orchestrator.ExecuteQueryAsync(query);
            return Ok(ApiResponse<RAGResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing simple query");
            return StatusCode(500, ApiResponse<RAGResponse>.FailureResult(
                "Error executing query", new List<string> { ex.Message }));
        }
    }

    [HttpPost("query/stream")]
    public async Task<IActionResult> ExecuteStreamingQuery([FromBody] RAGQuery query)
    {
        try
        {
            var streamingResponse = await _orchestrator.ExecuteStreamingQueryAsync(query);

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { queryId = streamingResponse.QueryId, sources = streamingResponse.Sources })}\n\n");
            await Response.Body.FlushAsync();

            if (streamingResponse.AnswerStream != null)
            {
                await foreach (var chunk in streamingResponse.AnswerStream)
                {
                    await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { content = chunk })}\n\n");
                    await Response.Body.FlushAsync();
                }
            }

            await Response.WriteAsync("data: [DONE]\n\n");
            await Response.Body.FlushAsync();

            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing streaming query");
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "Error executing streaming query", new List<string> { ex.Message }));
        }
    }

    [HttpPost("query/batch")]
    public async Task<ActionResult<ApiResponse<BatchRAGResponse>>> ExecuteBatchQueries(
        [FromBody] BatchRAGRequest request)
    {
        try
        {
            var response = await _orchestrator.ExecuteBatchQueriesAsync(request);
            return Ok(ApiResponse<BatchRAGResponse>.SuccessResult(
                response, $"Batch completed: {response.SuccessfulQueries}/{response.TotalQueries} successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing batch queries");
            return StatusCode(500, ApiResponse<BatchRAGResponse>.FailureResult(
                "Error executing batch queries", new List<string> { ex.Message }));
        }
    }

    #endregion

    #region Metrics & Health

    [HttpGet("metrics")]
    public async Task<ActionResult<ApiResponse<PipelineMetrics>>> GetMetrics()
    {
        try
        {
            var metrics = await _orchestrator.GetMetricsAsync();
            return Ok(ApiResponse<PipelineMetrics>.SuccessResult(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metrics");
            return StatusCode(500, ApiResponse<PipelineMetrics>.FailureResult(
                "Error retrieving metrics", new List<string> { ex.Message }));
        }
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "RAG Pipeline API",
            Timestamp = DateTime.UtcNow
        });
    }

    #endregion
}

public class SimpleQueryRequest
{
    public string Query { get; set; } = string.Empty;
    public string? KnowledgeBaseId { get; set; }
    public RAGStrategy? Strategy { get; set; }
    public int? TopK { get; set; }
    public float? Temperature { get; set; }
}
