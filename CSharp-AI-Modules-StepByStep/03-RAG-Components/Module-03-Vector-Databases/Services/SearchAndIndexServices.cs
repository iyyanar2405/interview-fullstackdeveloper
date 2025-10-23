using AI.VectorDatabases.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.VectorDatabases.Services;

public class SearchService
{
    private readonly IVectorDatabaseServiceFactory _factory;
    private readonly VectorDatabaseSettings _settings;

    public SearchService(
        IVectorDatabaseServiceFactory factory,
        IOptions<VectorDatabaseSettings> settings)
    {
        _factory = factory;
        _settings = settings.Value;
    }

    public async Task<SearchResponse> VectorSearchAsync(
        string collectionName,
        float[] queryVector,
        int limit = 10,
        float? scoreThreshold = null,
        SearchFilter? filter = null,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        
        var request = new SearchRequest
        {
            CollectionName = collectionName,
            QueryVector = queryVector,
            Limit = limit,
            ScoreThreshold = scoreThreshold,
            Filter = filter,
            WithPayload = true,
            WithVector = false
        };

        return await service.SearchAsync(request);
    }

    public async Task<SearchResponse> HybridSearchAsync(HybridSearchRequest request)
    {
        var service = _factory.GetService(request.Provider);
        
        // Perform vector search
        var vectorResults = await service.SearchAsync(new SearchRequest
        {
            CollectionName = request.CollectionName,
            QueryVector = request.QueryVector,
            Limit = request.Limit * 2, // Get more results for reranking
            Filter = request.Filter,
            WithPayload = true,
            WithVector = true
        });

        if (!vectorResults.Success)
            return vectorResults;

        // Apply keyword filtering if provided
        if (!string.IsNullOrEmpty(request.KeywordQuery))
        {
            vectorResults.Results = vectorResults.Results
                .Where(r => MatchesKeyword(r, request.KeywordQuery, request.KeywordFields))
                .ToList();
        }

        // Combine scores with weights
        var finalResults = vectorResults.Results
            .Select(r => new VectorPointWithScore
            {
                Id = r.Id,
                Score = CalculateHybridScore(r, request),
                Vector = r.Vector,
                Payload = r.Payload
            })
            .OrderByDescending(r => r.Score)
            .Take(request.Limit)
            .ToList();

        return new SearchResponse
        {
            Results = finalResults,
            TotalFound = finalResults.Count,
            Success = true,
            Duration = vectorResults.Duration
        };
    }

    public async Task<RecommendResponse> RecommendAsync(RecommendRequest request)
    {
        var service = _factory.GetService(request.Provider);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Retrieve positive and negative examples
            var positiveVectors = new List<float[]>();
            var negativeVectors = new List<float[]>();

            if (request.PositiveIds.Any())
            {
                var retrieveRequest = new RetrieveRequest
                {
                    CollectionName = request.CollectionName,
                    PointIds = request.PositiveIds,
                    WithVector = true
                };

                var positivePoints = await service.RetrieveAsync(retrieveRequest);
                if (positivePoints.Success)
                {
                    positiveVectors.AddRange(positivePoints.Points.Select(p => p.Vector));
                }
            }

            if (request.NegativeIds.Any())
            {
                var retrieveRequest = new RetrieveRequest
                {
                    CollectionName = request.CollectionName,
                    PointIds = request.NegativeIds,
                    WithVector = true
                };

                var negativePoints = await service.RetrieveAsync(retrieveRequest);
                if (negativePoints.Success)
                {
                    negativeVectors.AddRange(negativePoints.Points.Select(p => p.Vector));
                }
            }

            // Calculate centroid from positive examples
            var queryVector = CalculateCentroid(positiveVectors);

            // If negative examples exist, adjust the query vector
            if (negativeVectors.Any())
            {
                var negativeCentroid = CalculateCentroid(negativeVectors);
                queryVector = AdjustVectorAwayFrom(queryVector, negativeCentroid, 0.5f);
            }

            // Perform search
            var searchRequest = new SearchRequest
            {
                CollectionName = request.CollectionName,
                QueryVector = queryVector,
                Limit = request.Limit,
                Filter = request.Filter,
                WithPayload = true,
                WithVector = false
            };

            var searchResults = await service.SearchAsync(searchRequest);

            stopwatch.Stop();

            return new RecommendResponse
            {
                Recommendations = searchResults.Results,
                TotalFound = searchResults.TotalFound,
                Success = searchResults.Success,
                Duration = stopwatch.Elapsed,
                Errors = searchResults.Errors
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new RecommendResponse
            {
                Success = false,
                Duration = stopwatch.Elapsed,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<SearchResponse> MultiVectorSearchAsync(
        string collectionName,
        List<float[]> queryVectors,
        int limit = 10,
        SearchFilter? filter = null,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        
        // Perform batch search
        var batchRequest = new BatchSearchRequest
        {
            CollectionName = collectionName,
            QueryVectors = queryVectors,
            Limit = limit,
            Filter = filter
        };

        var batchResults = await service.BatchSearchAsync(batchRequest);

        if (!batchResults.Success)
        {
            return new SearchResponse
            {
                Success = false,
                Errors = new List<string> { "Batch search failed" }
            };
        }

        // Combine and deduplicate results
        var allResults = new Dictionary<string, VectorPointWithScore>();
        
        foreach (var result in batchResults.Results)
        {
            foreach (var point in result.Results)
            {
                if (!allResults.ContainsKey(point.Id) || allResults[point.Id].Score < point.Score)
                {
                    allResults[point.Id] = point;
                }
            }
        }

        var finalResults = allResults.Values
            .OrderByDescending(p => p.Score)
            .Take(limit)
            .ToList();

        return new SearchResponse
        {
            Results = finalResults,
            TotalFound = finalResults.Count,
            Success = true,
            Duration = batchResults.Duration
        };
    }

    public async Task<VectorDatabaseAnalytics> GetSearchAnalyticsAsync(
        string collectionName,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        
        var collectionInfo = await service.GetCollectionInfoAsync(collectionName);
        
        if (collectionInfo == null)
        {
            return new VectorDatabaseAnalytics
            {
                TotalCollections = 0,
                TotalVectors = 0
            };
        }

        return new VectorDatabaseAnalytics
        {
            TotalCollections = 1,
            TotalVectors = collectionInfo.PointsCount,
            TotalStorageBytes = collectionInfo.PointsCount * collectionInfo.VectorSize * sizeof(float),
            Provider = provider ?? _settings.DefaultProvider
        };
    }

    #region Helper Methods

    private bool MatchesKeyword(VectorPointWithScore point, string keyword, List<string>? fields = null)
    {
        var searchFields = fields ?? point.Payload.Keys.ToList();
        
        return searchFields.Any(field =>
        {
            if (point.Payload.TryGetValue(field, out var value))
            {
                var valueStr = value?.ToString() ?? "";
                return valueStr.Contains(keyword, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        });
    }

    private float CalculateHybridScore(VectorPointWithScore point, HybridSearchRequest request)
    {
        var vectorScore = point.Score * request.VectorWeight;
        
        // Simple keyword score (could be enhanced with TF-IDF, BM25, etc.)
        var keywordScore = 0.0f;
        if (!string.IsNullOrEmpty(request.KeywordQuery))
        {
            keywordScore = MatchesKeyword(point, request.KeywordQuery, request.KeywordFields) 
                ? 1.0f 
                : 0.0f;
        }
        
        var weightedKeywordScore = keywordScore * request.KeywordWeight;

        return vectorScore + weightedKeywordScore;
    }

    private float[] CalculateCentroid(List<float[]> vectors)
    {
        if (!vectors.Any())
            return Array.Empty<float>();

        var dimension = vectors[0].Length;
        var centroid = new float[dimension];

        foreach (var vector in vectors)
        {
            for (int i = 0; i < dimension; i++)
            {
                centroid[i] += vector[i];
            }
        }

        for (int i = 0; i < dimension; i++)
        {
            centroid[i] /= vectors.Count;
        }

        return centroid;
    }

    private float[] AdjustVectorAwayFrom(float[] vector, float[] awayFrom, float strength)
    {
        var adjusted = new float[vector.Length];
        
        for (int i = 0; i < vector.Length; i++)
        {
            var diff = vector[i] - awayFrom[i];
            adjusted[i] = vector[i] + (diff * strength);
        }

        // Normalize
        var magnitude = (float)Math.Sqrt(adjusted.Sum(v => v * v));
        if (magnitude > 0)
        {
            for (int i = 0; i < adjusted.Length; i++)
            {
                adjusted[i] /= magnitude;
            }
        }

        return adjusted;
    }

    #endregion
}

public class IndexManagementService
{
    private readonly IVectorDatabaseServiceFactory _factory;
    private readonly VectorDatabaseSettings _settings;

    public IndexManagementService(
        IVectorDatabaseServiceFactory factory,
        IOptions<VectorDatabaseSettings> settings)
    {
        _factory = factory;
        _settings = settings.Value;
    }

    public async Task<bool> CreateIndexAsync(
        string collectionName,
        int vectorSize,
        DistanceMetric distance = DistanceMetric.Cosine,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        
        var request = new CreateCollectionRequest
        {
            Name = collectionName,
            VectorSize = vectorSize,
            Distance = distance
        };

        return await service.CreateCollectionAsync(request);
    }

    public async Task<bool> DeleteIndexAsync(
        string collectionName,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        return await service.DeleteCollectionAsync(collectionName);
    }

    public async Task<CollectionInfo?> GetIndexInfoAsync(
        string collectionName,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        return await service.GetCollectionInfoAsync(collectionName);
    }

    public async Task<List<CollectionInfo>> ListIndexesAsync(VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        return await service.ListCollectionsAsync();
    }

    public async Task<CollectionStatistics> GetIndexStatisticsAsync(
        string collectionName,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var info = await service.GetCollectionInfoAsync(collectionName);

        if (info == null)
        {
            return new CollectionStatistics
            {
                CollectionName = collectionName,
                TotalPoints = 0
            };
        }

        var countRequest = new CountRequest
        {
            CollectionName = collectionName,
            Exact = true
        };

        var countResult = await service.CountAsync(countRequest);

        return new CollectionStatistics
        {
            CollectionName = collectionName,
            TotalPoints = countResult.Count,
            IndexedPoints = info.IndexedVectorsCount ?? info.PointsCount,
            VectorDimensions = info.VectorSize,
            DistanceMetric = info.Distance.ToString(),
            Status = info.Status.ToString()
        };
    }

    public async Task<bool> OptimizeIndexAsync(
        IndexOptimizationRequest request,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        
        // For in-memory, no optimization needed
        if ((provider ?? _settings.DefaultProvider) == VectorDatabaseProvider.InMemory)
        {
            return true;
        }

        // Optimization logic would be provider-specific
        // This is a placeholder for future implementation
        return await Task.FromResult(true);
    }

    public async Task<long> GetIndexSizeAsync(
        string collectionName,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);
        var info = await service.GetCollectionInfoAsync(collectionName);

        if (info == null)
            return 0;

        // Estimate: vectors + metadata overhead
        var vectorBytes = info.PointsCount * info.VectorSize * sizeof(float);
        var metadataOverhead = info.PointsCount * 1024; // Rough estimate: 1KB per point for metadata
        
        return vectorBytes + metadataOverhead;
    }

    public async Task<bool> RecreateIndexAsync(
        string collectionName,
        int newVectorSize,
        DistanceMetric newDistance,
        VectorDatabaseProvider? provider = null)
    {
        var service = _factory.GetService(provider);

        // Delete existing index
        await service.DeleteCollectionAsync(collectionName);

        // Create new index
        var request = new CreateCollectionRequest
        {
            Name = collectionName,
            VectorSize = newVectorSize,
            Distance = newDistance
        };

        return await service.CreateCollectionAsync(request);
    }
}
