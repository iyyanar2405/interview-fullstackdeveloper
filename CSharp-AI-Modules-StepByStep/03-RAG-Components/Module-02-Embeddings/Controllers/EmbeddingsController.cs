using AI.Embeddings.Models;
using AI.Embeddings.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.Embeddings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingsController : ControllerBase
{
    private readonly IEmbeddingServiceFactory _embeddingServiceFactory;
    private readonly ISimilarityService _similarityService;
    private readonly IBatchEmbeddingService _batchEmbeddingService;
    private readonly ISemanticSearchService _semanticSearchService;
    private readonly IEmbeddingCacheService _cacheService;
    private readonly IEmbeddingAnalyticsService _analyticsService;

    public EmbeddingsController(
        IEmbeddingServiceFactory embeddingServiceFactory,
        ISimilarityService similarityService,
        IBatchEmbeddingService batchEmbeddingService,
        ISemanticSearchService semanticSearchService,
        IEmbeddingCacheService cacheService,
        IEmbeddingAnalyticsService analyticsService)
    {
        _embeddingServiceFactory = embeddingServiceFactory;
        _similarityService = similarityService;
        _batchEmbeddingService = batchEmbeddingService;
        _semanticSearchService = semanticSearchService;
        _cacheService = cacheService;
        _analyticsService = analyticsService;
    }

    #region Embedding Generation Endpoints

    /// <summary>
    /// Generate embeddings for texts
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<EmbeddingResponse>> GenerateEmbeddings([FromBody] EmbeddingRequest request)
    {
        var service = _embeddingServiceFactory.GetService(request.Provider);
        var response = await service.GenerateEmbeddingsAsync(request);

        if (!response.Success)
            return BadRequest(response);

        // Record analytics
        await _analyticsService.RecordEmbeddingAsync(
            request.Model,
            request.Provider,
            response.Usage.TotalTokens,
            response.Usage.EstimatedCost);

        return Ok(response);
    }

    /// <summary>
    /// Generate embedding for a single text
    /// </summary>
    [HttpPost("generate/single")]
    public async Task<ActionResult<EmbeddingResult>> GenerateSingleEmbedding(
        [FromBody] string text,
        [FromQuery] EmbeddingModel model = EmbeddingModel.TextEmbedding3Small,
        [FromQuery] EmbeddingProvider provider = EmbeddingProvider.OpenAI)
    {
        var service = _embeddingServiceFactory.GetService(provider);
        var result = await service.GenerateSingleEmbeddingAsync(text, model, provider);

        return Ok(result);
    }

    /// <summary>
    /// Generate embeddings in batch
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<BatchEmbeddingResponse>> GenerateBatchEmbeddings([FromBody] BatchEmbeddingRequest request)
    {
        var response = await _batchEmbeddingService.ProcessBatchAsync(request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    #endregion

    #region Similarity Endpoints

    /// <summary>
    /// Calculate similarity between two embeddings
    /// </summary>
    [HttpPost("similarity")]
    public ActionResult<SimilarityResult> CalculateSimilarity([FromBody] SimilarityRequest request)
    {
        var score = _similarityService.CalculateSimilarity(
            request.Embedding1,
            request.Embedding2,
            request.Metric);

        var result = new SimilarityResult
        {
            Score = score,
            Metric = request.Metric,
            Dimensions = request.Embedding1.Length,
            Normalized = true
        };

        return Ok(result);
    }

    /// <summary>
    /// Calculate similarity between two texts
    /// </summary>
    [HttpPost("similarity/text")]
    public async Task<ActionResult<SimilarityResult>> CalculateTextSimilarity(
        [FromBody] TextSimilarityRequest request)
    {
        var service = _embeddingServiceFactory.GetService(request.Provider);

        var embedding1Task = service.GenerateSingleEmbeddingAsync(request.Text1, request.Model, request.Provider);
        var embedding2Task = service.GenerateSingleEmbeddingAsync(request.Text2, request.Model, request.Provider);

        await Task.WhenAll(embedding1Task, embedding2Task);

        var score = _similarityService.CalculateSimilarity(
            embedding1Task.Result.Embedding,
            embedding2Task.Result.Embedding,
            request.Metric);

        var result = new SimilarityResult
        {
            Score = score,
            Metric = request.Metric,
            Dimensions = embedding1Task.Result.Dimensions,
            Normalized = true
        };

        return Ok(result);
    }

    /// <summary>
    /// Find similar embeddings
    /// </summary>
    [HttpPost("similarity/find")]
    public async Task<ActionResult<BatchSimilarityResult>> FindSimilar([FromBody] BatchSimilarityRequest request)
    {
        var result = await _similarityService.FindSimilarAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Calculate cosine similarity
    /// </summary>
    [HttpPost("similarity/cosine")]
    public ActionResult<float> CosineSimilarity([FromBody] SimilarityRequest request)
    {
        var score = _similarityService.CosineSimilarity(request.Embedding1, request.Embedding2);
        return Ok(score);
    }

    #endregion

    #region Semantic Search Endpoints

    /// <summary>
    /// Perform semantic search
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<SemanticSearchResponse>> SemanticSearch([FromBody] SemanticSearchRequest request)
    {
        var response = await _semanticSearchService.SearchAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Perform simple semantic search with query string
    /// </summary>
    [HttpPost("search/simple")]
    public async Task<ActionResult<SemanticSearchResponse>> SimpleSearch(
        [FromBody] List<SemanticDocument> documents,
        [FromQuery] string query,
        [FromQuery] int topK = 5)
    {
        var request = new SemanticSearchRequest
        {
            Query = query,
            Documents = documents,
            TopK = topK
        };

        var response = await _semanticSearchService.SearchAsync(request);
        return Ok(response);
    }

    #endregion

    #region Model Information Endpoints

    /// <summary>
    /// Get information about an embedding model
    /// </summary>
    [HttpGet("models/{model}")]
    public ActionResult<EmbeddingModelInfo> GetModelInfo(EmbeddingModel model)
    {
        var service = _embeddingServiceFactory.GetService(EmbeddingProvider.OpenAI);
        var modelInfo = service.GetModelInfo(model);
        return Ok(modelInfo);
    }

    /// <summary>
    /// Get all available models
    /// </summary>
    [HttpGet("models")]
    public ActionResult<List<EmbeddingModelInfo>> GetAvailableModels(
        [FromQuery] EmbeddingProvider? provider = null)
    {
        var targetProvider = provider ?? EmbeddingProvider.OpenAI;
        var service = _embeddingServiceFactory.GetService(targetProvider);
        var models = service.GetAvailableModels();
        return Ok(models);
    }

    #endregion

    #region Cache Management Endpoints

    /// <summary>
    /// Get cache statistics
    /// </summary>
    [HttpGet("cache/stats")]
    public async Task<ActionResult<CacheStatistics>> GetCacheStatistics()
    {
        var stats = await _cacheService.GetStatisticsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Clear embedding cache
    /// </summary>
    [HttpDelete("cache")]
    public async Task<ActionResult> ClearCache()
    {
        await _cacheService.ClearAsync();
        return Ok(new { message = "Cache cleared successfully" });
    }

    #endregion

    #region Analytics Endpoints

    /// <summary>
    /// Get embedding analytics
    /// </summary>
    [HttpGet("analytics")]
    public async Task<ActionResult<EmbeddingAnalytics>> GetAnalytics(
        [FromQuery] DateTime? start = null,
        [FromQuery] DateTime? end = null)
    {
        var analytics = await _analyticsService.GetAnalyticsAsync(start, end);
        return Ok(analytics);
    }

    #endregion

    #region Utility Endpoints

    /// <summary>
    /// Compare multiple texts
    /// </summary>
    [HttpPost("compare")]
    public async Task<ActionResult<MultiTextComparison>> CompareTexts(
        [FromBody] List<string> texts,
        [FromQuery] EmbeddingModel model = EmbeddingModel.TextEmbedding3Small,
        [FromQuery] SimilarityMetric metric = SimilarityMetric.Cosine)
    {
        var service = _embeddingServiceFactory.GetService(EmbeddingProvider.OpenAI);

        var embeddingRequest = new EmbeddingRequest
        {
            Texts = texts,
            Model = model,
            Provider = EmbeddingProvider.OpenAI
        };

        var embeddingResponse = await service.GenerateEmbeddingsAsync(embeddingRequest);

        if (!embeddingResponse.Success)
            return BadRequest(embeddingResponse);

        var embeddings = embeddingResponse.Results.Select(r => r.Embedding).ToList();

        // Build similarity matrix
        var n = embeddings.Count;
        var matrix = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = 1.0f;
                }
                else if (i < j)
                {
                    var similarity = _similarityService.CalculateSimilarity(embeddings[i], embeddings[j], metric);
                    matrix[i, j] = similarity;
                    matrix[j, i] = similarity;
                }
            }
        }

        var comparison = new MultiTextComparison
        {
            Texts = texts,
            Embeddings = embeddings,
            SimilarityMatrix = matrix,
            Metric = metric
        };

        return Ok(comparison);
    }

    /// <summary>
    /// Check if two texts are similar
    /// </summary>
    [HttpPost("similarity/check")]
    public async Task<ActionResult<EmbeddingComparison>> CheckSimilarity(
        [FromBody] CheckSimilarityRequest request)
    {
        var service = _embeddingServiceFactory.GetService(request.Provider);

        var embedding1Task = service.GenerateSingleEmbeddingAsync(request.Text1, request.Model, request.Provider);
        var embedding2Task = service.GenerateSingleEmbeddingAsync(request.Text2, request.Model, request.Provider);

        await Task.WhenAll(embedding1Task, embedding2Task);

        var similarities = new Dictionary<SimilarityMetric, float>();
        foreach (SimilarityMetric metric in Enum.GetValues(typeof(SimilarityMetric)))
        {
            var score = _similarityService.CalculateSimilarity(
                embedding1Task.Result.Embedding,
                embedding2Task.Result.Embedding,
                metric);
            similarities[metric] = score;
        }

        var cosineSimilarity = similarities[SimilarityMetric.Cosine];

        var comparison = new EmbeddingComparison
        {
            Text1 = request.Text1,
            Text2 = request.Text2,
            Embedding1 = embedding1Task.Result.Embedding,
            Embedding2 = embedding2Task.Result.Embedding,
            Similarities = similarities,
            AreSimilar = cosineSimilarity >= request.Threshold,
            Threshold = request.Threshold
        };

        return Ok(comparison);
    }

    #endregion
}

#region Request Models

public class TextSimilarityRequest
{
    public string Text1 { get; set; } = string.Empty;
    public string Text2 { get; set; } = string.Empty;
    public EmbeddingModel Model { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public EmbeddingProvider Provider { get; set; } = EmbeddingProvider.OpenAI;
    public SimilarityMetric Metric { get; set; } = SimilarityMetric.Cosine;
}

public class CheckSimilarityRequest
{
    public string Text1 { get; set; } = string.Empty;
    public string Text2 { get; set; } = string.Empty;
    public EmbeddingModel Model { get; set; } = EmbeddingModel.TextEmbedding3Small;
    public EmbeddingProvider Provider { get; set; } = EmbeddingProvider.OpenAI;
    public float Threshold { get; set; } = 0.8f;
}

#endregion
