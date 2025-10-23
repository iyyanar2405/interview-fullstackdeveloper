using AI.VectorDatabases.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AI.VectorDatabases.Services;

public class InMemoryVectorDatabaseService : IVectorDatabaseService
{
    private readonly ConcurrentDictionary<string, InMemoryCollection> _collections;
    private readonly VectorDatabaseSettings _settings;

    public InMemoryVectorDatabaseService(IOptions<VectorDatabaseSettings> settings)
    {
        _settings = settings.Value;
        _collections = new ConcurrentDictionary<string, InMemoryCollection>();
    }

    public Task<bool> CreateCollectionAsync(CreateCollectionRequest request)
    {
        var collection = new InMemoryCollection
        {
            Name = request.Name,
            VectorSize = request.VectorSize,
            Distance = request.Distance,
            Points = new ConcurrentDictionary<string, VectorPoint>(),
            CreatedAt = DateTime.UtcNow,
            Status = IndexStatus.Ready
        };

        var result = _collections.TryAdd(request.Name, collection);
        return Task.FromResult(result);
    }

    public Task<bool> DeleteCollectionAsync(string collectionName)
    {
        var result = _collections.TryRemove(collectionName, out _);
        return Task.FromResult(result);
    }

    public Task<CollectionInfo?> GetCollectionInfoAsync(string collectionName)
    {
        if (!_collections.TryGetValue(collectionName, out var collection))
        {
            return Task.FromResult<CollectionInfo?>(null);
        }

        var info = new CollectionInfo
        {
            Name = collection.Name,
            VectorSize = collection.VectorSize,
            Distance = collection.Distance,
            PointsCount = collection.Points.Count,
            Status = collection.Status,
            IndexedVectorsCount = collection.Points.Count
        };

        return Task.FromResult<CollectionInfo?>(info);
    }

    public Task<List<CollectionInfo>> ListCollectionsAsync()
    {
        var collections = _collections.Values.Select(c => new CollectionInfo
        {
            Name = c.Name,
            VectorSize = c.VectorSize,
            Distance = c.Distance,
            PointsCount = c.Points.Count,
            Status = c.Status,
            IndexedVectorsCount = c.Points.Count
        }).ToList();

        return Task.FromResult(collections);
    }

    public Task<UpsertResponse> UpsertAsync(UpsertRequest request)
    {
        var response = new UpsertResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            // Validate vector dimensions
            foreach (var point in request.Points)
            {
                if (point.Vector.Length != collection.VectorSize)
                {
                    response.Errors.Add($"Point {point.Id}: Vector size mismatch. Expected {collection.VectorSize}, got {point.Vector.Length}");
                    continue;
                }

                collection.Points.AddOrUpdate(point.Id, point, (key, oldValue) => point);
                response.ProcessedIds.Add(point.Id);
            }

            response.ProcessedCount = response.ProcessedIds.Count;
            response.Success = response.ProcessedCount > 0;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return Task.FromResult(response);
    }

    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        var response = new SearchResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            // Validate query vector dimension
            if (request.QueryVector.Length != collection.VectorSize)
            {
                response.Success = false;
                response.Errors.Add($"Query vector size mismatch. Expected {collection.VectorSize}, got {request.QueryVector.Length}");
                return Task.FromResult(response);
            }

            // Get all points
            var points = collection.Points.Values.ToList();

            // Apply filter if provided
            if (request.Filter != null)
            {
                points = ApplyFilter(points, request.Filter);
            }

            // Calculate similarity scores
            var scoredPoints = points.Select(p => new VectorPointWithScore
            {
                Id = p.Id,
                Score = CalculateSimilarity(request.QueryVector, p.Vector, collection.Distance),
                Vector = request.WithVector ? p.Vector : Array.Empty<float>(),
                Payload = request.WithPayload ? p.Payload : new Dictionary<string, object>()
            }).ToList();

            // Sort by score (descending)
            scoredPoints = scoredPoints.OrderByDescending(p => p.Score).ToList();

            // Apply score threshold
            if (request.ScoreThreshold.HasValue)
            {
                scoredPoints = scoredPoints.Where(p => p.Score >= request.ScoreThreshold.Value).ToList();
            }

            // Apply limit
            scoredPoints = scoredPoints.Take(request.Limit).ToList();

            response.Results = scoredPoints;
            response.TotalFound = scoredPoints.Count;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return Task.FromResult(response);
    }

    public async Task<BatchSearchResponse> BatchSearchAsync(BatchSearchRequest request)
    {
        var response = new BatchSearchResponse
        {
            TotalQueries = request.QueryVectors.Count
        };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var tasks = request.QueryVectors.Select(async queryVector =>
            {
                var searchRequest = new SearchRequest
                {
                    CollectionName = request.CollectionName,
                    QueryVector = queryVector,
                    Limit = request.Limit,
                    ScoreThreshold = request.ScoreThreshold,
                    Filter = request.Filter
                };

                return await SearchAsync(searchRequest);
            });

            response.Results = (await Task.WhenAll(tasks)).ToList();
            response.Success = response.Results.All(r => r.Success);
        }
        catch (Exception)
        {
            response.Success = false;
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return response;
    }

    public Task<DeleteResponse> DeleteAsync(DeleteRequest request)
    {
        var response = new DeleteResponse();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            if (request.PointIds != null && request.PointIds.Any())
            {
                var deletedCount = 0;
                foreach (var pointId in request.PointIds)
                {
                    if (collection.Points.TryRemove(pointId, out _))
                    {
                        deletedCount++;
                    }
                }
                response.DeletedCount = deletedCount;
            }
            else if (request.Filter != null)
            {
                var points = collection.Points.Values.ToList();
                var filteredPoints = ApplyFilter(points, request.Filter);
                
                var deletedCount = 0;
                foreach (var point in filteredPoints)
                {
                    if (collection.Points.TryRemove(point.Id, out _))
                    {
                        deletedCount++;
                    }
                }
                response.DeletedCount = deletedCount;
            }

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return Task.FromResult(response);
    }

    public Task<RetrieveResponse> RetrieveAsync(RetrieveRequest request)
    {
        var response = new RetrieveResponse();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            var points = new List<VectorPoint>();
            foreach (var pointId in request.PointIds)
            {
                if (collection.Points.TryGetValue(pointId, out var point))
                {
                    points.Add(new VectorPoint
                    {
                        Id = point.Id,
                        Vector = request.WithVector ? point.Vector : Array.Empty<float>(),
                        Payload = request.WithPayload ? point.Payload : new Dictionary<string, object>()
                    });
                }
            }

            response.Points = points;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return Task.FromResult(response);
    }

    public Task<ScrollResponse> ScrollAsync(ScrollRequest request)
    {
        var response = new ScrollResponse();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            var points = collection.Points.Values.ToList();

            // Apply filter if provided
            if (request.Filter != null)
            {
                points = ApplyFilter(points, request.Filter);
            }

            // Apply offset
            if (!string.IsNullOrEmpty(request.Offset))
            {
                var offsetIndex = points.FindIndex(p => p.Id == request.Offset);
                if (offsetIndex >= 0)
                {
                    points = points.Skip(offsetIndex + 1).ToList();
                }
            }

            // Apply limit
            var resultPoints = points.Take(request.Limit).ToList();

            response.Points = resultPoints.Select(p => new VectorPoint
            {
                Id = p.Id,
                Vector = request.WithVector ? p.Vector : Array.Empty<float>(),
                Payload = request.WithPayload ? p.Payload : new Dictionary<string, object>()
            }).ToList();

            response.NextOffset = response.Points.LastOrDefault()?.Id;
            response.HasMore = points.Count > request.Limit;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return Task.FromResult(response);
    }

    public Task<UpdatePayloadResponse> UpdatePayloadAsync(UpdatePayloadRequest request)
    {
        var response = new UpdatePayloadResponse();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            var updatedCount = 0;
            foreach (var pointId in request.PointIds)
            {
                if (collection.Points.TryGetValue(pointId, out var point))
                {
                    if (request.Overwrite)
                    {
                        point.Payload = new Dictionary<string, object>(request.Payload);
                    }
                    else
                    {
                        foreach (var kvp in request.Payload)
                        {
                            point.Payload[kvp.Key] = kvp.Value;
                        }
                    }
                    updatedCount++;
                }
            }

            response.UpdatedCount = updatedCount;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return Task.FromResult(response);
    }

    public Task<CountResponse> CountAsync(CountRequest request)
    {
        var response = new CountResponse();

        try
        {
            if (!_collections.TryGetValue(request.CollectionName, out var collection))
            {
                response.Success = false;
                response.Errors.Add($"Collection '{request.CollectionName}' not found");
                return Task.FromResult(response);
            }

            var points = collection.Points.Values.ToList();

            if (request.Filter != null)
            {
                points = ApplyFilter(points, request.Filter);
            }

            response.Count = points.Count;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return Task.FromResult(response);
    }

    #region Helper Methods

    private float CalculateSimilarity(float[] vector1, float[] vector2, DistanceMetric metric)
    {
        return metric switch
        {
            DistanceMetric.Cosine => CosineSimilarity(vector1, vector2),
            DistanceMetric.Euclidean => 1.0f / (1.0f + EuclideanDistance(vector1, vector2)),
            DistanceMetric.DotProduct => DotProduct(vector1, vector2),
            DistanceMetric.Manhattan => 1.0f / (1.0f + ManhattanDistance(vector1, vector2)),
            _ => CosineSimilarity(vector1, vector2)
        };
    }

    private float CosineSimilarity(float[] vector1, float[] vector2)
    {
        var dotProduct = 0.0f;
        var magnitude1 = 0.0f;
        var magnitude2 = 0.0f;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        magnitude1 = (float)Math.Sqrt(magnitude1);
        magnitude2 = (float)Math.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;

        return dotProduct / (magnitude1 * magnitude2);
    }

    private float DotProduct(float[] vector1, float[] vector2)
    {
        var result = 0.0f;
        for (int i = 0; i < vector1.Length; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

    private float EuclideanDistance(float[] vector1, float[] vector2)
    {
        var sum = 0.0f;
        for (int i = 0; i < vector1.Length; i++)
        {
            var diff = vector1[i] - vector2[i];
            sum += diff * diff;
        }
        return (float)Math.Sqrt(sum);
    }

    private float ManhattanDistance(float[] vector1, float[] vector2)
    {
        var sum = 0.0f;
        for (int i = 0; i < vector1.Length; i++)
        {
            sum += Math.Abs(vector1[i] - vector2[i]);
        }
        return sum;
    }

    private List<VectorPoint> ApplyFilter(List<VectorPoint> points, SearchFilter filter)
    {
        var result = points;

        // Apply Must conditions (AND)
        if (filter.Must.Any())
        {
            foreach (var condition in filter.Must)
            {
                result = result.Where(p => MatchesCondition(p, condition)).ToList();
            }
        }

        // Apply Should conditions (OR)
        if (filter.Should.Any())
        {
            result = result.Where(p => filter.Should.Any(c => MatchesCondition(p, c))).ToList();
        }

        // Apply MustNot conditions (NOT)
        if (filter.MustNot.Any())
        {
            foreach (var condition in filter.MustNot)
            {
                result = result.Where(p => !MatchesCondition(p, condition)).ToList();
            }
        }

        return result;
    }

    private bool MatchesCondition(VectorPoint point, FilterCondition condition)
    {
        if (!point.Payload.TryGetValue(condition.Key, out var value))
            return false;

        var valueStr = value?.ToString() ?? "";
        var conditionValueStr = condition.Value?.ToString() ?? "";

        return condition.Operator switch
        {
            FilterOperator.Equals => valueStr.Equals(conditionValueStr, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotEquals => !valueStr.Equals(conditionValueStr, StringComparison.OrdinalIgnoreCase),
            FilterOperator.Contains => valueStr.Contains(conditionValueStr, StringComparison.OrdinalIgnoreCase),
            FilterOperator.GreaterThan => double.TryParse(valueStr, out var v1) && 
                                          double.TryParse(conditionValueStr, out var v2) && v1 > v2,
            FilterOperator.LessThan => double.TryParse(valueStr, out var v3) && 
                                       double.TryParse(conditionValueStr, out var v4) && v3 < v4,
            FilterOperator.In => conditionValueStr.Split(',').Contains(valueStr, StringComparer.OrdinalIgnoreCase),
            _ => false
        };
    }

    #endregion
}

#region In-Memory Models

internal class InMemoryCollection
{
    public string Name { get; set; } = string.Empty;
    public int VectorSize { get; set; }
    public DistanceMetric Distance { get; set; }
    public ConcurrentDictionary<string, VectorPoint> Points { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public IndexStatus Status { get; set; }
}

#endregion
