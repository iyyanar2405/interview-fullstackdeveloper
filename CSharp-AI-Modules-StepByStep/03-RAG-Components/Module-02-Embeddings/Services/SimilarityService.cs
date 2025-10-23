using AI.Embeddings.Models;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace AI.Embeddings.Services;

#region Similarity Service

public interface ISimilarityService
{
    float CalculateSimilarity(float[] embedding1, float[] embedding2, SimilarityMetric metric = SimilarityMetric.Cosine);
    Task<BatchSimilarityResult> FindSimilarAsync(BatchSimilarityRequest request);
    float CosineSimilarity(float[] a, float[] b);
    float EuclideanDistance(float[] a, float[] b);
    float DotProduct(float[] a, float[] b);
    float ManhattanDistance(float[] a, float[] b);
}

public class SimilarityService : ISimilarityService
{
    public float CalculateSimilarity(float[] embedding1, float[] embedding2, SimilarityMetric metric = SimilarityMetric.Cosine)
    {
        if (embedding1.Length != embedding2.Length)
        {
            throw new ArgumentException("Embeddings must have the same dimensions");
        }

        return metric switch
        {
            SimilarityMetric.Cosine => CosineSimilarity(embedding1, embedding2),
            SimilarityMetric.Euclidean => 1f / (1f + EuclideanDistance(embedding1, embedding2)),
            SimilarityMetric.DotProduct => DotProduct(embedding1, embedding2),
            SimilarityMetric.Manhattan => 1f / (1f + ManhattanDistance(embedding1, embedding2)),
            _ => throw new NotSupportedException($"Similarity metric {metric} is not supported")
        };
    }

    public async Task<BatchSimilarityResult> FindSimilarAsync(BatchSimilarityRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var matches = new List<SimilarityMatch>();

        for (int i = 0; i < request.TargetEmbeddings.Count; i++)
        {
            var score = CalculateSimilarity(request.QueryEmbedding, request.TargetEmbeddings[i], request.Metric);

            if (!request.Threshold.HasValue || score >= request.Threshold.Value)
            {
                matches.Add(new SimilarityMatch
                {
                    Index = i,
                    Score = score,
                    Embedding = request.TargetEmbeddings[i]
                });
            }
        }

        // Sort by score descending and take top K
        var topMatches = matches
            .OrderByDescending(m => m.Score)
            .Take(request.TopK)
            .ToList();

        stopwatch.Stop();

        return await Task.FromResult(new BatchSimilarityResult
        {
            Matches = topMatches,
            Metric = request.Metric,
            TotalCompared = request.TargetEmbeddings.Count,
            Duration = stopwatch.Elapsed
        });
    }

    public float CosineSimilarity(float[] a, float[] b)
    {
        var dotProduct = a.Zip(b, (x, y) => x * y).Sum();
        var magnitudeA = Math.Sqrt(a.Sum(x => x * x));
        var magnitudeB = Math.Sqrt(b.Sum(x => x * x));

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0f;

        return (float)(dotProduct / (magnitudeA * magnitudeB));
    }

    public float EuclideanDistance(float[] a, float[] b)
    {
        return (float)Math.Sqrt(a.Zip(b, (x, y) => Math.Pow(x - y, 2)).Sum());
    }

    public float DotProduct(float[] a, float[] b)
    {
        return a.Zip(b, (x, y) => x * y).Sum();
    }

    public float ManhattanDistance(float[] a, float[] b)
    {
        return a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();
    }
}

#endregion

#region Batch Processing Service

public interface IBatchEmbeddingService
{
    Task<BatchEmbeddingResponse> ProcessBatchAsync(BatchEmbeddingRequest request);
}

public class BatchEmbeddingService : IBatchEmbeddingService
{
    private readonly IEmbeddingServiceFactory _embeddingServiceFactory;

    public BatchEmbeddingService(IEmbeddingServiceFactory embeddingServiceFactory)
    {
        _embeddingServiceFactory = embeddingServiceFactory;
    }

    public async Task<BatchEmbeddingResponse> ProcessBatchAsync(BatchEmbeddingRequest request)
    {
        var response = new BatchEmbeddingResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var embeddingService = _embeddingServiceFactory.GetService(request.Provider);
            var batches = CreateBatches(request.Texts, request.Options.BatchSize);
            
            response.Statistics.TotalTexts = request.Texts.Count;

            var semaphore = new SemaphoreSlim(request.Options.MaxConcurrency);
            var tasks = new List<Task<EmbeddingResponse>>();

            foreach (var batch in batches)
            {
                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        var batchRequest = new EmbeddingRequest
                        {
                            Texts = batch,
                            Model = request.Model,
                            Provider = request.Provider,
                            Options = new EmbeddingOptions
                            {
                                UseCache = request.Options.UseCache
                            }
                        };

                        var batchResponse = await embeddingService.GenerateEmbeddingsAsync(batchRequest);
                        return batchResponse;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            var batchResponses = await Task.WhenAll(tasks);

            foreach (var batchResponse in batchResponses)
            {
                if (batchResponse.Success)
                {
                    response.Results.AddRange(batchResponse.Results);
                    response.Statistics.SuccessCount += batchResponse.Results.Count;
                    response.Statistics.TotalCost += batchResponse.Usage.EstimatedCost;
                    response.Statistics.CacheHits += batchResponse.Usage.CacheHits;
                    response.Statistics.CacheMisses += batchResponse.Usage.CacheMisses;
                }
                else
                {
                    response.Statistics.FailureCount += batchResponse.Results.Count;
                    response.Errors.AddRange(batchResponse.Errors);
                }

                response.Statistics.BatchesProcessed++;
            }

            response.Success = response.Statistics.FailureCount == 0 || request.Options.ContinueOnError;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"Batch processing failed: {ex.Message}");
        }

        stopwatch.Stop();
        response.Statistics.TotalDuration = stopwatch.Elapsed;

        return response;
    }

    private List<List<string>> CreateBatches(List<string> items, int batchSize)
    {
        var batches = new List<List<string>>();

        for (int i = 0; i < items.Count; i += batchSize)
        {
            var batch = items.Skip(i).Take(batchSize).ToList();
            batches.Add(batch);
        }

        return batches;
    }
}

#endregion

#region Caching Service

public interface IEmbeddingCacheService
{
    Task<CachedEmbedding?> GetAsync(string text, EmbeddingModel model);
    Task SetAsync(string text, EmbeddingModel model, float[] embedding, int expirationMinutes);
    Task<CacheStatistics> GetStatisticsAsync();
    Task ClearAsync();
}

public class MemoryEmbeddingCacheService : IEmbeddingCacheService
{
    private readonly IMemoryCache _cache;
    private readonly EmbeddingSettings _settings;
    private int _hits;
    private int _misses;

    public MemoryEmbeddingCacheService(IMemoryCache cache, IOptions<EmbeddingSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }

    public async Task<CachedEmbedding?> GetAsync(string text, EmbeddingModel model)
    {
        var key = GenerateCacheKey(text, model);
        
        if (_cache.TryGetValue(key, out CachedEmbedding? cached))
        {
            Interlocked.Increment(ref _hits);
            if (cached != null)
            {
                cached.HitCount++;
            }
            return cached;
        }

        Interlocked.Increment(ref _misses);
        return await Task.FromResult<CachedEmbedding?>(null);
    }

    public async Task SetAsync(string text, EmbeddingModel model, float[] embedding, int expirationMinutes)
    {
        var key = GenerateCacheKey(text, model);
        var cached = new CachedEmbedding
        {
            Key = key,
            Text = text,
            Embedding = embedding,
            Model = model,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes),
            Size = embedding.Length * sizeof(float)
        };

        _cache.Set(key, cached, cacheOptions);
        await Task.CompletedTask;
    }

    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        var stats = new CacheStatistics
        {
            Hits = _hits,
            Misses = _misses,
            HitRate = _hits + _misses > 0 ? (double)_hits / (_hits + _misses) : 0,
            LastCleanup = DateTime.UtcNow
        };

        return await Task.FromResult(stats);
    }

    public async Task ClearAsync()
    {
        if (_cache is MemoryCache memCache)
        {
            memCache.Compact(1.0);
        }
        
        _hits = 0;
        _misses = 0;
        await Task.CompletedTask;
    }

    private string GenerateCacheKey(string text, EmbeddingModel model)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text));
        var hashString = Convert.ToBase64String(hash);
        return $"embedding:{model}:{hashString}";
    }
}

public class RedisEmbeddingCacheService : IEmbeddingCacheService
{
    private readonly IDistributedCache _cache;
    private readonly EmbeddingSettings _settings;

    public RedisEmbeddingCacheService(IDistributedCache cache, IOptions<EmbeddingSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }

    public async Task<CachedEmbedding?> GetAsync(string text, EmbeddingModel model)
    {
        var key = GenerateCacheKey(text, model);
        var data = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(data))
            return null;

        return JsonSerializer.Deserialize<CachedEmbedding>(data);
    }

    public async Task SetAsync(string text, EmbeddingModel model, float[] embedding, int expirationMinutes)
    {
        var key = GenerateCacheKey(text, model);
        var cached = new CachedEmbedding
        {
            Key = key,
            Text = text,
            Embedding = embedding,
            Model = model,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
        };

        var data = JsonSerializer.Serialize(cached);
        await _cache.SetStringAsync(key, data, options);
    }

    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        // Redis statistics would require additional implementation
        return await Task.FromResult(new CacheStatistics());
    }

    public async Task ClearAsync()
    {
        // Redis clear would require additional implementation
        await Task.CompletedTask;
    }

    private string GenerateCacheKey(string text, EmbeddingModel model)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text));
        var hashString = Convert.ToBase64String(hash);
        return $"embedding:{model}:{hashString}";
    }
}

#endregion

#region Semantic Search Service

public interface ISemanticSearchService
{
    Task<SemanticSearchResponse> SearchAsync(SemanticSearchRequest request);
}

public class SemanticSearchService : ISemanticSearchService
{
    private readonly IEmbeddingServiceFactory _embeddingServiceFactory;
    private readonly ISimilarityService _similarityService;

    public SemanticSearchService(
        IEmbeddingServiceFactory embeddingServiceFactory,
        ISimilarityService similarityService)
    {
        _embeddingServiceFactory = embeddingServiceFactory;
        _similarityService = similarityService;
    }

    public async Task<SemanticSearchResponse> SearchAsync(SemanticSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        // Generate query embedding
        var embeddingService = _embeddingServiceFactory.GetService(EmbeddingProvider.OpenAI);
        var queryResult = await embeddingService.GenerateSingleEmbeddingAsync(
            request.Query,
            request.Model,
            EmbeddingProvider.OpenAI);

        // Generate embeddings for documents that don't have them
        var documentsNeedingEmbeddings = request.Documents.Where(d => d.Embedding == null).ToList();
        if (documentsNeedingEmbeddings.Any())
        {
            var embeddingRequest = new EmbeddingRequest
            {
                Texts = documentsNeedingEmbeddings.Select(d => d.Content).ToList(),
                Model = request.Model,
                Provider = EmbeddingProvider.OpenAI
            };

            var embeddingResponse = await embeddingService.GenerateEmbeddingsAsync(embeddingRequest);

            for (int i = 0; i < documentsNeedingEmbeddings.Count; i++)
            {
                documentsNeedingEmbeddings[i].Embedding = embeddingResponse.Results[i].Embedding;
            }
        }

        // Calculate similarities
        var similarities = new List<(SemanticDocument doc, float score)>();
        foreach (var doc in request.Documents)
        {
            if (doc.Embedding != null)
            {
                var score = _similarityService.CalculateSimilarity(
                    queryResult.Embedding,
                    doc.Embedding,
                    request.Metric);

                if (!request.Threshold.HasValue || score >= request.Threshold.Value)
                {
                    similarities.Add((doc, score));
                }
            }
        }

        // Sort and take top K
        var topResults = similarities
            .OrderByDescending(s => s.score)
            .Take(request.TopK)
            .Select((s, index) => new SearchResult
            {
                DocumentId = s.doc.Id,
                Content = s.doc.Content,
                Score = s.score,
                Rank = index + 1,
                Metadata = s.doc.Metadata
            })
            .ToList();

        stopwatch.Stop();

        return new SemanticSearchResponse
        {
            Results = topResults,
            Query = request.Query,
            QueryEmbedding = queryResult.Embedding,
            Duration = stopwatch.Elapsed
        };
    }
}

#endregion

#region Analytics Service

public interface IEmbeddingAnalyticsService
{
    Task RecordEmbeddingAsync(EmbeddingModel model, EmbeddingProvider provider, int tokens, decimal cost);
    Task<EmbeddingAnalytics> GetAnalyticsAsync(DateTime? start = null, DateTime? end = null);
}

public class EmbeddingAnalyticsService : IEmbeddingAnalyticsService
{
    private readonly List<EmbeddingRecord> _records = new();
    private readonly IEmbeddingCacheService _cacheService;

    public EmbeddingAnalyticsService(IEmbeddingCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task RecordEmbeddingAsync(EmbeddingModel model, EmbeddingProvider provider, int tokens, decimal cost)
    {
        _records.Add(new EmbeddingRecord
        {
            Model = model,
            Provider = provider,
            Tokens = tokens,
            Cost = cost,
            Timestamp = DateTime.UtcNow
        });

        await Task.CompletedTask;
    }

    public async Task<EmbeddingAnalytics> GetAnalyticsAsync(DateTime? start = null, DateTime? end = null)
    {
        var startDate = start ?? DateTime.UtcNow.AddDays(-30);
        var endDate = end ?? DateTime.UtcNow;

        var filteredRecords = _records
            .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate)
            .ToList();

        var analytics = new EmbeddingAnalytics
        {
            PeriodStart = startDate,
            PeriodEnd = endDate,
            TotalEmbeddingsGenerated = filteredRecords.Count,
            TotalTokensProcessed = filteredRecords.Sum(r => r.Tokens),
            TotalCost = filteredRecords.Sum(r => r.Cost),
            ModelUsage = filteredRecords.GroupBy(r => r.Model).ToDictionary(g => g.Key, g => g.Count()),
            ProviderUsage = filteredRecords.GroupBy(r => r.Provider).ToDictionary(g => g.Key, g => g.Count()),
            CacheStats = await _cacheService.GetStatisticsAsync(),
            AverageResponseTime = TimeSpan.FromMilliseconds(
                filteredRecords.Any() ? filteredRecords.Average(r => 100) : 0) // Placeholder
        };

        return analytics;
    }

    private class EmbeddingRecord
    {
        public EmbeddingModel Model { get; set; }
        public EmbeddingProvider Provider { get; set; }
        public int Tokens { get; set; }
        public decimal Cost { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

#endregion
