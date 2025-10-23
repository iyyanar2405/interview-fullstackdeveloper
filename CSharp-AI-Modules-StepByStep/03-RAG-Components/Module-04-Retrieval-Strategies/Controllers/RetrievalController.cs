using AI.RetrievalStrategies.Models;
using AI.RetrievalStrategies.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.RetrievalStrategies.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RetrievalController : ControllerBase
{
    private readonly IRetrievalService _retrievalService;
    private readonly IReRankingService _reRankingService;
    private readonly IQueryExpansionService _queryExpansionService;
    private readonly IContextOptimizationService _contextOptimizationService;
    private readonly ISelfQueryService _selfQueryService;

    public RetrievalController(
        IRetrievalService retrievalService,
        IReRankingService reRankingService,
        IQueryExpansionService queryExpansionService,
        IContextOptimizationService contextOptimizationService,
        ISelfQueryService selfQueryService)
    {
        _retrievalService = retrievalService;
        _reRankingService = reRankingService;
        _queryExpansionService = queryExpansionService;
        _contextOptimizationService = contextOptimizationService;
        _selfQueryService = selfQueryService;
    }

    #region Retrieval Endpoints

    [HttpPost("semantic-search")]
    public async Task<IActionResult> SemanticSearch([FromBody] RetrievalQuery query)
    {
        var result = await _retrievalService.SemanticSearchAsync(query);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("hybrid-search")]
    public async Task<IActionResult> HybridSearch([FromBody] HybridSearchRequest request)
    {
        var result = await _retrievalService.HybridSearchAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("multi-query")]
    public async Task<IActionResult> MultiQuery([FromBody] MultiQueryRequest request)
    {
        var result = await _retrievalService.MultiQueryRetrievalAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("hyde")]
    public async Task<IActionResult> HyDE([FromBody] HyDERequest request)
    {
        var result = await _retrievalService.HyDERetrievalAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("parent-document")]
    public async Task<IActionResult> ParentDocument([FromBody] ParentDocumentRequest request)
    {
        var result = await _retrievalService.ParentDocumentRetrievalAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    #endregion

    #region Re-ranking Endpoints

    [HttpPost("rerank")]
    public async Task<IActionResult> ReRank([FromBody] ReRankRequest request)
    {
        var result = await _reRankingService.ReRankAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("rerank/rrf")]
    public async Task<IActionResult> ReciprocalRankFusion(
        [FromBody] List<List<RetrievedDocument>> rankedLists,
        [FromQuery] int topK = 10)
    {
        var result = await _reRankingService.ReciprocalRankFusionAsync(rankedLists, topK);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("rerank/mmr")]
    public async Task<IActionResult> MaximalMarginalRelevance([FromBody] DiversityRequest request)
    {
        var result = await _reRankingService.MaximalMarginalRelevanceAsync(request);
        return Ok(result);
    }

    [HttpPost("rerank/bm25")]
    public async Task<IActionResult> BM25ReRank(
        [FromQuery] string query,
        [FromBody] List<RetrievedDocument> documents,
        [FromQuery] int topK = 10)
    {
        var result = await _reRankingService.BM25ReRankAsync(query, documents, topK);
        return Ok(result);
    }

    #endregion

    #region Query Expansion Endpoints

    [HttpPost("query/expand")]
    public async Task<IActionResult> ExpandQuery([FromBody] QueryExpansionRequest request)
    {
        var result = await _queryExpansionService.ExpandQueryAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("query/synonyms")]
    public async Task<IActionResult> GenerateSynonyms(
        [FromQuery] string query,
        [FromQuery] int maxExpansions = 5)
    {
        var result = await _queryExpansionService.GenerateSynonymsAsync(query, maxExpansions);
        return Ok(new { query, synonyms = result });
    }

    [HttpPost("query/paraphrases")]
    public async Task<IActionResult> GenerateParaphrases(
        [FromQuery] string query,
        [FromQuery] int maxExpansions = 5)
    {
        var result = await _queryExpansionService.GenerateParaphrasesAsync(query, maxExpansions);
        return Ok(new { query, paraphrases = result });
    }

    [HttpPost("query/perspectives")]
    public async Task<IActionResult> GenerateMultiPerspective(
        [FromQuery] string query,
        [FromQuery] int maxExpansions = 5)
    {
        var result = await _queryExpansionService.GenerateMultiPerspectiveQueriesAsync(query, maxExpansions);
        return Ok(new { query, perspectives = result });
    }

    [HttpPost("query/parse")]
    public async Task<IActionResult> ParseQuery([FromQuery] string query)
    {
        var result = await _selfQueryService.ParseNaturalLanguageQueryAsync(query);
        return Ok(result);
    }

    #endregion

    #region Context Optimization Endpoints

    [HttpPost("context/optimize")]
    public async Task<IActionResult> OptimizeContext([FromBody] ContextOptimizationRequest request)
    {
        var result = await _contextOptimizationService.OptimizeContextAsync(request);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    [HttpPost("context/estimate-tokens")]
    public IActionResult EstimateTokens([FromBody] string text)
    {
        var tokenCount = _contextOptimizationService.EstimateTokenCount(text);
        return Ok(new { text = text.Substring(0, Math.Min(100, text.Length)) + "...", tokenCount });
    }

    #endregion

    #region Combined Workflow Endpoints

    [HttpPost("retrieve-and-rerank")]
    public async Task<IActionResult> RetrieveAndReRank(
        [FromBody] RetrievalQuery query,
        [FromQuery] ReRankingAlgorithm algorithm = ReRankingAlgorithm.ReciprocalRankFusion)
    {
        // Step 1: Retrieve
        var retrievalResult = await _retrievalService.SemanticSearchAsync(query);
        
        if (!retrievalResult.Success)
            return BadRequest(retrievalResult);

        // Step 2: Re-rank
        var reRankRequest = new ReRankRequest
        {
            Query = query.QueryText,
            Documents = retrievalResult.Documents,
            Algorithm = algorithm,
            TopK = query.TopK
        };

        var reRankResult = await _reRankingService.ReRankAsync(reRankRequest);
        
        return Ok(new
        {
            retrieval = retrievalResult,
            reRanking = reRankResult,
            finalDocuments = reRankResult.ReRankedDocuments
        });
    }

    [HttpPost("expand-and-retrieve")]
    public async Task<IActionResult> ExpandAndRetrieve(
        [FromQuery] string query,
        [FromQuery] QueryExpansionMethod method = QueryExpansionMethod.Paraphrasing,
        [FromQuery] int maxExpansions = 3,
        [FromQuery] int topK = 10)
    {
        // Step 1: Expand query
        var expansionRequest = new QueryExpansionRequest
        {
            OriginalQuery = query,
            Method = method,
            MaxExpansions = maxExpansions
        };

        var expansionResult = await _queryExpansionService.ExpandQueryAsync(expansionRequest);
        
        if (!expansionResult.Success)
            return BadRequest(expansionResult);

        // Step 2: Multi-query retrieval
        var multiQueryRequest = new MultiQueryRequest
        {
            OriginalQuery = query,
            ExpandedQueries = expansionResult.ExpandedQueries,
            TopK = topK
        };

        var retrievalResult = await _retrievalService.MultiQueryRetrievalAsync(multiQueryRequest);
        
        return Ok(new
        {
            expansion = expansionResult,
            retrieval = retrievalResult,
            finalDocuments = retrievalResult.MergedDocuments
        });
    }

    [HttpPost("full-pipeline")]
    public async Task<IActionResult> FullRetrievalPipeline(
        [FromQuery] string query,
        [FromQuery] int maxTokens = 4000,
        [FromQuery] int topK = 10)
    {
        // Step 1: Expand query
        var expansionResult = await _queryExpansionService.ExpandQueryAsync(new QueryExpansionRequest
        {
            OriginalQuery = query,
            Method = QueryExpansionMethod.Paraphrasing,
            MaxExpansions = 3
        });

        // Step 2: Multi-query retrieval
        var retrievalResult = await _retrievalService.MultiQueryRetrievalAsync(new MultiQueryRequest
        {
            OriginalQuery = query,
            ExpandedQueries = expansionResult.ExpandedQueries,
            TopK = topK * 2 // Get more for re-ranking
        });

        // Step 3: Re-rank
        var reRankResult = await _reRankingService.ReRankAsync(new ReRankRequest
        {
            Query = query,
            Documents = retrievalResult.MergedDocuments,
            Algorithm = ReRankingAlgorithm.MaximalMarginalRelevance,
            TopK = topK
        });

        // Step 4: Optimize context
        var contextResult = await _contextOptimizationService.OptimizeContextAsync(new ContextOptimizationRequest
        {
            Documents = reRankResult.ReRankedDocuments,
            MaxTokens = maxTokens,
            Strategy = ContextOptimizationStrategy.Hybrid,
            QueryContext = query
        });

        return Ok(new
        {
            query,
            expansion = new { expandedQueries = expansionResult.ExpandedQueries },
            retrieval = new { totalFound = retrievalResult.MergedDocuments.Count },
            reRanking = new { algorithm = reRankResult.Algorithm },
            context = new 
            { 
                documents = contextResult.OptimizedContext.Documents.Count,
                tokens = contextResult.OptimizedTokenCount,
                compressionRatio = contextResult.CompressionRatio
            },
            finalContext = contextResult.OptimizedContext
        });
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
            service = "Retrieval Strategies API"
        });
    }

    #endregion
}
