using AI.RAGPipeline.Models;
using System.Diagnostics;

namespace AI.RAGPipeline.Services;

public interface IContextRetrievalService
{
    Task<RetrievedContext> RetrieveContextAsync(ProcessedQuery query, RetrievalConfig config);
    Task<List<ContextChunk>> SemanticSearchAsync(float[] queryEmbedding, int topK, Dictionary<string, object>? filters = null);
    Task<List<ContextChunk>> HybridSearchAsync(ProcessedQuery query, int topK);
    Task<List<ContextChunk>> MultiQueryRetrievalAsync(List<string> queries, int topK);
    Task<List<ContextChunk>> RerankContextAsync(List<ContextChunk> contexts, string query, string algorithm);
}

public class ContextRetrievalService : IContextRetrievalService
{
    private readonly IVectorStorageService _vectorService;
    private readonly IKeywordSearchService _keywordSearchService;
    private readonly ILogger<ContextRetrievalService> _logger;

    public ContextRetrievalService(
        IVectorStorageService vectorService,
        IKeywordSearchService keywordSearchService,
        ILogger<ContextRetrievalService> logger)
    {
        _vectorService = vectorService;
        _keywordSearchService = keywordSearchService;
        _logger = logger;
    }

    public async Task<RetrievedContext> RetrieveContextAsync(ProcessedQuery query, RetrievalConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var retrievedContext = new RetrievedContext();

        try
        {
            _logger.LogInformation("Retrieving context using strategy: {Strategy}", config.Strategy);

            List<ContextChunk> chunks = config.Strategy switch
            {
                RAGStrategy.Simple => await SemanticSearchAsync(query.QueryEmbedding, config.TopK, config.Filters),
                RAGStrategy.HybridSearch => await HybridSearchAsync(query, config.TopK),
                RAGStrategy.MultiQuery => await MultiQueryRetrievalAsync(query.ExpandedQueries, config.TopK),
                RAGStrategy.HyDE => await HyDERetrievalAsync(query, config.TopK),
                _ => await SemanticSearchAsync(query.QueryEmbedding, config.TopK, config.Filters)
            };

            retrievedContext.TotalRetrieved = chunks.Count;
            retrievedContext.Chunks = chunks;

            // Apply reranking if enabled
            if (config.EnableReranking && chunks.Count > 0)
            {
                chunks = await RerankContextAsync(chunks, query.OriginalQuery, config.RerankingAlgorithm);
                retrievedContext.AfterReranking = chunks.Count;
            }

            // Filter by minimum score
            chunks = chunks.Where(c => c.Score >= config.MinimumScore).ToList();
            retrievedContext.Chunks = chunks;

            stopwatch.Stop();
            retrievedContext.RetrievalTime = stopwatch.Elapsed;

            _logger.LogInformation(
                "Retrieved {Count} chunks in {Time}ms (after reranking: {AfterReranking})",
                retrievedContext.TotalRetrieved, 
                retrievedContext.RetrievalTime.TotalMilliseconds,
                retrievedContext.AfterReranking);

            return retrievedContext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving context");
            throw;
        }
    }

    public async Task<List<ContextChunk>> SemanticSearchAsync(
        float[] queryEmbedding, 
        int topK, 
        Dictionary<string, object>? filters = null)
    {
        var results = await _vectorService.SearchVectorsAsync(queryEmbedding, topK, filters);
        
        return results.Select(r => new ContextChunk
        {
            Id = r.Id,
            DocumentId = r.DocumentId,
            Content = r.Content,
            Score = r.Metadata.ContainsKey("score") ? Convert.ToSingle(r.Metadata["score"]) : 0f,
            RelevanceScore = r.Metadata.ContainsKey("score") ? Convert.ToSingle(r.Metadata["score"]) : 0f,
            ChunkIndex = r.ChunkIndex,
            Metadata = r.Metadata
        }).ToList();
    }

    public async Task<List<ContextChunk>> HybridSearchAsync(ProcessedQuery query, int topK)
    {
        // Combine semantic and keyword search
        var semanticResults = await SemanticSearchAsync(query.QueryEmbedding, topK);
        var keywordResults = await _keywordSearchService.SearchAsync(query.OriginalQuery, topK);

        // Merge results using Reciprocal Rank Fusion (RRF)
        var mergedResults = ReciprocalRankFusion(
            new[] { semanticResults, keywordResults },
            topK
        );

        _logger.LogDebug("Hybrid search: {Semantic} semantic + {Keyword} keyword = {Merged} merged",
            semanticResults.Count, keywordResults.Count, mergedResults.Count);

        return mergedResults;
    }

    public async Task<List<ContextChunk>> MultiQueryRetrievalAsync(List<string> queries, int topK)
    {
        var allResults = new List<List<ContextChunk>>();

        foreach (var query in queries)
        {
            var embedding = await GenerateQueryEmbedding(query);
            var results = await SemanticSearchAsync(embedding, topK);
            allResults.Add(results);
        }

        // Merge using RRF
        var merged = ReciprocalRankFusion(allResults.ToArray(), topK);

        _logger.LogDebug("Multi-query retrieval: {QueryCount} queries, {ResultCount} results",
            queries.Count, merged.Count);

        return merged;
    }

    private async Task<List<ContextChunk>> HyDERetrievalAsync(ProcessedQuery query, int topK)
    {
        // Hypothetical Document Embeddings (HyDE)
        // Generate a hypothetical answer, then use it for retrieval
        var hypotheticalAnswer = await GenerateHypotheticalAnswer(query.OriginalQuery);
        var embedding = await GenerateQueryEmbedding(hypotheticalAnswer);
        
        return await SemanticSearchAsync(embedding, topK);
    }

    private async Task<string> GenerateHypotheticalAnswer(string query)
    {
        // Placeholder - in production, use OpenAI to generate hypothetical answer
        await Task.CompletedTask;
        return $"A comprehensive answer to: {query}";
    }

    private async Task<float[]> GenerateQueryEmbedding(string query)
    {
        // Placeholder - inject embedding service in production
        await Task.CompletedTask;
        return new float[1536]; // Default embedding size
    }

    public async Task<List<ContextChunk>> RerankContextAsync(
        List<ContextChunk> contexts, 
        string query, 
        string algorithm)
    {
        await Task.CompletedTask;

        switch (algorithm.ToUpperInvariant())
        {
            case "RRF":
                return contexts; // Already using RRF in hybrid search

            case "MMR":
                return MaximalMarginalRelevance(contexts, query, 0.7f);

            case "DIVERSITY":
                return DiversityReranking(contexts);

            default:
                return contexts.OrderByDescending(c => c.Score).ToList();
        }
    }

    private List<ContextChunk> ReciprocalRankFusion(List<ContextChunk>[] resultLists, int topK)
    {
        const int k = 60; // RRF constant
        var rrfScores = new Dictionary<string, float>();
        var chunkMap = new Dictionary<string, ContextChunk>();

        foreach (var results in resultLists)
        {
            for (int rank = 0; rank < results.Count; rank++)
            {
                var chunk = results[rank];
                var rrfScore = 1.0f / (k + rank + 1);

                if (!rrfScores.ContainsKey(chunk.Id))
                {
                    rrfScores[chunk.Id] = 0;
                    chunkMap[chunk.Id] = chunk;
                }

                rrfScores[chunk.Id] += rrfScore;
            }
        }

        return rrfScores
            .OrderByDescending(kvp => kvp.Value)
            .Take(topK)
            .Select(kvp =>
            {
                var chunk = chunkMap[kvp.Key];
                chunk.Score = kvp.Value;
                return chunk;
            })
            .ToList();
    }

    private List<ContextChunk> MaximalMarginalRelevance(
        List<ContextChunk> contexts, 
        string query, 
        float lambda = 0.7f)
    {
        if (contexts.Count <= 1) return contexts;

        var selected = new List<ContextChunk> { contexts[0] };
        var remaining = contexts.Skip(1).ToList();

        while (remaining.Any() && selected.Count < contexts.Count)
        {
            var maxScore = float.MinValue;
            ContextChunk? maxChunk = null;
            int maxIndex = -1;

            for (int i = 0; i < remaining.Count; i++)
            {
                var candidate = remaining[i];
                
                // Relevance to query
                var relevance = candidate.Score;

                // Similarity to selected chunks (diversity penalty)
                var maxSimilarity = selected.Max(s => 
                    CosineSimilarity(s.Content, candidate.Content));

                // MMR score
                var mmrScore = lambda * relevance - (1 - lambda) * maxSimilarity;

                if (mmrScore > maxScore)
                {
                    maxScore = mmrScore;
                    maxChunk = candidate;
                    maxIndex = i;
                }
            }

            if (maxChunk != null)
            {
                selected.Add(maxChunk);
                remaining.RemoveAt(maxIndex);
            }
            else
            {
                break;
            }
        }

        return selected;
    }

    private List<ContextChunk> DiversityReranking(List<ContextChunk> contexts)
    {
        // Group by document and ensure diversity
        var grouped = contexts.GroupBy(c => c.DocumentId).ToList();
        var diversified = new List<ContextChunk>();

        int maxPerDocument = Math.Max(1, contexts.Count / grouped.Count);
        
        foreach (var group in grouped)
        {
            diversified.AddRange(group.Take(maxPerDocument));
        }

        return diversified.OrderByDescending(c => c.Score).ToList();
    }

    private float CosineSimilarity(string text1, string text2)
    {
        // Simple word-based similarity
        var words1 = text1.ToLowerInvariant().Split(' ').ToHashSet();
        var words2 = text2.ToLowerInvariant().Split(' ').ToHashSet();

        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();

        return union > 0 ? (float)intersection / union : 0;
    }
}

public interface IKeywordSearchService
{
    Task<List<ContextChunk>> SearchAsync(string query, int topK);
}

public class KeywordSearchService : IKeywordSearchService
{
    private readonly ILogger<KeywordSearchService> _logger;
    // In production, use Lucene.NET or Elasticsearch
    private static readonly Dictionary<string, DocumentChunk> _documents = new();

    public KeywordSearchService(ILogger<KeywordSearchService> logger)
    {
        _logger = logger;
    }

    public async Task<List<ContextChunk>> SearchAsync(string query, int topK)
    {
        await Task.CompletedTask;

        var queryTerms = query.ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();

        var results = _documents.Values
            .Select(doc => new
            {
                Chunk = doc,
                Score = CalculateBM25Score(doc.Content, queryTerms)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => new ContextChunk
            {
                Id = x.Chunk.Id,
                DocumentId = x.Chunk.DocumentId,
                Content = x.Chunk.Content,
                Score = x.Score,
                RelevanceScore = x.Score,
                ChunkIndex = x.Chunk.ChunkIndex,
                Metadata = x.Chunk.Metadata
            })
            .ToList();

        _logger.LogDebug("Keyword search returned {Count} results for query: {Query}", 
            results.Count, query);

        return results;
    }

    private float CalculateBM25Score(string document, HashSet<string> queryTerms)
    {
        const float k1 = 1.5f;
        const float b = 0.75f;
        const float avgDocLength = 1000f;

        var docTerms = document.ToLowerInvariant().Split(' ');
        var docLength = docTerms.Length;
        var score = 0f;

        foreach (var term in queryTerms)
        {
            var termFreq = docTerms.Count(t => t == term);
            if (termFreq == 0) continue;

            var idf = (float)Math.Log(1 + (_documents.Count - termFreq + 0.5) / (termFreq + 0.5));
            var tf = (termFreq * (k1 + 1)) / (termFreq + k1 * (1 - b + b * docLength / avgDocLength));
            
            score += idf * tf;
        }

        return score;
    }
}
