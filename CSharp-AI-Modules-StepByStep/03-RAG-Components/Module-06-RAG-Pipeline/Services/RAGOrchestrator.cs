using AI.RAGPipeline.Models;
using System.Diagnostics;

namespace AI.RAGPipeline.Services;

public interface IRAGOrchestrator
{
    Task<RAGResponse> ExecuteQueryAsync(RAGQuery query);
    Task<StreamingRAGResponse> ExecuteStreamingQueryAsync(RAGQuery query);
    Task<BatchRAGResponse> ExecuteBatchQueriesAsync(BatchRAGRequest request);
    Task<PipelineMetrics> GetMetricsAsync();
}

public class RAGOrchestrator : IRAGOrchestrator
{
    private readonly IDocumentIngestionService _ingestionService;
    private readonly IQueryProcessingService _queryProcessingService;
    private readonly IContextRetrievalService _retrievalService;
    private readonly IResponseGenerationService _generationService;
    private readonly IEvaluationService _evaluationService;
    private readonly ILogger<RAGOrchestrator> _logger;
    private readonly bool _enableEvaluation;

    private static readonly List<PipelineExecutionContext> _executionHistory = new();
    private static readonly object _lock = new();

    public RAGOrchestrator(
        IDocumentIngestionService ingestionService,
        IQueryProcessingService queryProcessingService,
        IContextRetrievalService retrievalService,
        IResponseGenerationService generationService,
        IEvaluationService evaluationService,
        ILogger<RAGOrchestrator> logger,
        IConfiguration configuration)
    {
        _ingestionService = ingestionService;
        _queryProcessingService = queryProcessingService;
        _retrievalService = retrievalService;
        _generationService = generationService;
        _evaluationService = evaluationService;
        _logger = logger;
        _enableEvaluation = configuration.GetValue<bool>("RAGPipeline:EnableEvaluation");
    }

    public async Task<RAGResponse> ExecuteQueryAsync(RAGQuery query)
    {
        var executionContext = new PipelineExecutionContext
        {
            Query = query.Query
        };

        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting RAG pipeline execution for query: {Query}", query.Query);

            // Stage 1: Query Processing
            executionContext.CurrentStage = PipelineStage.QueryProcessing;
            var processedQuery = await ProcessQueryStageAsync(query);
            executionContext.ProcessedQuery = processedQuery;

            // Stage 2: Context Retrieval
            executionContext.CurrentStage = PipelineStage.Retrieval;
            var retrievedContext = await RetrievalStageAsync(processedQuery, query.RetrievalConfig);
            executionContext.RetrievedContext = retrievedContext;

            // Stage 3: Response Generation
            executionContext.CurrentStage = PipelineStage.Generation;
            var response = await GenerationStageAsync(query.Query, retrievedContext, query.GenerationConfig);

            // Stage 4: Evaluation (optional)
            if (_enableEvaluation)
            {
                executionContext.CurrentStage = PipelineStage.Evaluation;
                response.Evaluation = await EvaluationStageAsync(query.Query, response.Answer, retrievedContext.Chunks);
            }

            executionContext.CurrentStage = PipelineStage.Completed;
            executionContext.Response = response;
            executionContext.EndTime = DateTime.UtcNow;

            totalStopwatch.Stop();
            response.TotalTime = totalStopwatch.Elapsed;

            // Store execution context
            lock (_lock)
            {
                _executionHistory.Add(executionContext);
                if (_executionHistory.Count > 1000)
                {
                    _executionHistory.RemoveAt(0);
                }
            }

            _logger.LogInformation(
                "RAG pipeline completed in {Time}ms - Quality: {Quality}",
                response.TotalTime.TotalMilliseconds,
                response.Evaluation?.Quality ?? ResponseQuality.Good);

            return response;
        }
        catch (Exception ex)
        {
            executionContext.CurrentStage = PipelineStage.Failed;
            executionContext.Errors.Add(new PipelineError
            {
                Stage = executionContext.CurrentStage,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });

            _logger.LogError(ex, "RAG pipeline failed at stage: {Stage}", executionContext.CurrentStage);

            throw;
        }
    }

    public async Task<StreamingRAGResponse> ExecuteStreamingQueryAsync(RAGQuery query)
    {
        try
        {
            _logger.LogInformation("Starting streaming RAG pipeline for query: {Query}", query.Query);

            // Process query and retrieve context first
            var processedQuery = await ProcessQueryStageAsync(query);
            var retrievedContext = await RetrievalStageAsync(processedQuery, query.RetrievalConfig);

            // Stream the response
            var streamingResponse = new StreamingRAGResponse
            {
                QueryId = Guid.NewGuid().ToString(),
                Query = query.Query,
                Sources = retrievedContext.Chunks,
                AnswerStream = _generationService.GenerateStreamingResponseAsync(
                    query.Query, 
                    retrievedContext, 
                    query.GenerationConfig)
            };

            return streamingResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Streaming RAG pipeline failed");
            throw;
        }
    }

    public async Task<BatchRAGResponse> ExecuteBatchQueriesAsync(BatchRAGRequest request)
    {
        _logger.LogInformation("Processing batch of {Count} queries", request.Queries.Count);

        var stopwatch = Stopwatch.StartNew();
        var response = new BatchRAGResponse
        {
            TotalQueries = request.Queries.Count
        };

        var semaphore = new SemaphoreSlim(request.MaxConcurrency);
        var tasks = new List<Task>();

        foreach (var query in request.Queries)
        {
            await semaphore.WaitAsync();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var result = await ExecuteQueryAsync(query);
                    
                    lock (response)
                    {
                        response.Responses.Add(result);
                        response.SuccessfulQueries++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing query: {Query}", query.Query);
                    
                    lock (response)
                    {
                        response.FailedQueries++;
                        response.Errors.Add(new BatchQueryError
                        {
                            Query = query.Query,
                            Error = ex.Message,
                            QueryIndex = request.Queries.IndexOf(query)
                        });
                    }

                    if (!request.ContinueOnError)
                    {
                        throw;
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        response.TotalTime = stopwatch.Elapsed;

        _logger.LogInformation(
            "Batch processing completed: {Success}/{Total} successful in {Time}ms",
            response.SuccessfulQueries, response.TotalQueries, response.TotalTime.TotalMilliseconds);

        return response;
    }

    public async Task<PipelineMetrics> GetMetricsAsync()
    {
        await Task.CompletedTask;

        lock (_lock)
        {
            var completedExecutions = _executionHistory
                .Where(e => e.CurrentStage == PipelineStage.Completed)
                .ToList();

            var failedExecutions = _executionHistory
                .Where(e => e.CurrentStage == PipelineStage.Failed)
                .ToList();

            var metrics = new PipelineMetrics
            {
                TotalQueries = _executionHistory.Count,
                SuccessfulQueries = completedExecutions.Count,
                FailedQueries = failedExecutions.Count
            };

            if (completedExecutions.Any())
            {
                metrics.AverageResponseTime = (float)completedExecutions
                    .Average(e => (e.EndTime - e.StartTime)?.TotalMilliseconds ?? 0);

                metrics.AverageRetrievalTime = (float)completedExecutions
                    .Where(e => e.RetrievedContext != null)
                    .Average(e => e.RetrievedContext!.RetrievalTime.TotalMilliseconds);

                metrics.AverageGenerationTime = (float)completedExecutions
                    .Where(e => e.Response != null)
                    .Average(e => e.Response!.GenerationTime.TotalMilliseconds);

                metrics.AverageRelevanceScore = completedExecutions
                    .Where(e => e.Response?.Evaluation != null)
                    .Select(e => e.Response!.Evaluation!.RelevanceScore)
                    .DefaultIfEmpty(0)
                    .Average();

                metrics.TotalTokensUsed = completedExecutions
                    .Where(e => e.Response != null)
                    .Sum(e => e.Response!.TokensUsed);
            }

            metrics.ErrorsByStage = _executionHistory
                .SelectMany(e => e.Errors)
                .GroupBy(e => e.Stage)
                .ToDictionary(g => g.Key, g => g.Count());

            return metrics;
        }
    }

    private async Task<ProcessedQuery> ProcessQueryStageAsync(RAGQuery query)
    {
        try
        {
            // Rewrite query if conversation history exists
            var rewrittenQuery = query.Query;
            if (query.ConversationHistory.Any())
            {
                rewrittenQuery = await _queryProcessingService.RewriteQueryAsync(
                    query.Query, 
                    query.ConversationHistory);
            }

            // Process the query
            var processedQuery = await _queryProcessingService.ProcessQueryAsync(rewrittenQuery);
            processedQuery.OriginalQuery = query.Query;

            _logger.LogDebug("Query processing stage completed");

            return processedQuery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in query processing stage");
            throw;
        }
    }

    private async Task<RetrievedContext> RetrievalStageAsync(
        ProcessedQuery processedQuery, 
        RetrievalConfig config)
    {
        try
        {
            // Add knowledge base filter if specified
            if (!string.IsNullOrEmpty(processedQuery.Metadata.GetValueOrDefault("knowledgeBaseId")?.ToString()))
            {
                config.Filters["knowledgeBaseId"] = processedQuery.Metadata["knowledgeBaseId"];
            }

            var retrievedContext = await _retrievalService.RetrieveContextAsync(processedQuery, config);

            _logger.LogDebug(
                "Retrieval stage completed: {ChunkCount} chunks retrieved in {Time}ms",
                retrievedContext.Chunks.Count, retrievedContext.RetrievalTime.TotalMilliseconds);

            return retrievedContext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in retrieval stage");
            throw;
        }
    }

    private async Task<RAGResponse> GenerationStageAsync(
        string query, 
        RetrievedContext context, 
        GenerationConfig config)
    {
        try
        {
            var response = await _generationService.GenerateResponseAsync(query, context, config);

            _logger.LogDebug(
                "Generation stage completed: {AnswerLength} chars in {Time}ms",
                response.Answer.Length, response.GenerationTime.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in generation stage");
            throw;
        }
    }

    private async Task<RAGEvaluation> EvaluationStageAsync(
        string query, 
        string answer, 
        List<ContextChunk> context)
    {
        try
        {
            var evaluationRequest = new EvaluationRequest
            {
                Query = query,
                Answer = answer,
                Context = context
            };

            var evaluation = await _evaluationService.EvaluateResponseAsync(evaluationRequest);

            _logger.LogDebug(
                "Evaluation stage completed: Overall score={Score}, Quality={Quality}",
                evaluation.OverallScore, evaluation.Quality);

            return evaluation;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error in evaluation stage, skipping evaluation");
            return new RAGEvaluation { OverallScore = 0.5f, Quality = ResponseQuality.Fair };
        }
    }
}
