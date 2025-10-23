using AI.VectorDatabases.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.VectorDatabases.Services;

#region Vector Database Interface

public interface IVectorDatabaseService
{
    Task<bool> CreateCollectionAsync(CreateCollectionRequest request);
    Task<bool> DeleteCollectionAsync(string collectionName);
    Task<CollectionInfo?> GetCollectionInfoAsync(string collectionName);
    Task<List<CollectionInfo>> ListCollectionsAsync();
    
    Task<UpsertResponse> UpsertAsync(UpsertRequest request);
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<BatchSearchResponse> BatchSearchAsync(BatchSearchRequest request);
    Task<DeleteResponse> DeleteAsync(DeleteRequest request);
    Task<RetrieveResponse> RetrieveAsync(RetrieveRequest request);
    Task<ScrollResponse> ScrollAsync(ScrollRequest request);
    Task<UpdatePayloadResponse> UpdatePayloadAsync(UpdatePayloadRequest request);
    Task<CountResponse> CountAsync(CountRequest request);
}

#endregion

#region Qdrant Service

public class QdrantVectorDatabaseService : IVectorDatabaseService
{
    private readonly QdrantClient _client;
    private readonly VectorDatabaseSettings _settings;

    public QdrantVectorDatabaseService(IOptions<VectorDatabaseSettings> settings)
    {
        _settings = settings.Value;
        
        var host = _settings.QdrantHost ?? "localhost";
        var port = _settings.QdrantPort;
        
        _client = new QdrantClient(
            host,
            port,
            https: _settings.QdrantUseTls,
            apiKey: _settings.QdrantApiKey);
    }

    public async Task<bool> CreateCollectionAsync(CreateCollectionRequest request)
    {
        try
        {
            var vectorParams = new VectorParams
            {
                Size = (ulong)request.VectorSize,
                Distance = MapDistanceMetric(request.Distance)
            };

            await _client.CreateCollectionAsync(
                request.Name,
                vectorParams);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteCollectionAsync(string collectionName)
    {
        try
        {
            await _client.DeleteCollectionAsync(collectionName);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<CollectionInfo?> GetCollectionInfoAsync(string collectionName)
    {
        try
        {
            var collection = await _client.GetCollectionInfoAsync(collectionName);
            
            return new CollectionInfo
            {
                Name = collectionName,
                VectorSize = (int)collection.Config.Params.VectorsConfig.Params.Size,
                Distance = MapDistanceMetric(collection.Config.Params.VectorsConfig.Params.Distance),
                PointsCount = (long)collection.PointsCount,
                Status = MapIndexStatus(collection.Status)
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<CollectionInfo>> ListCollectionsAsync()
    {
        try
        {
            var collections = await _client.ListCollectionsAsync();
            
            var result = new List<CollectionInfo>();
            foreach (var collection in collections)
            {
                var info = await GetCollectionInfoAsync(collection.Name);
                if (info != null)
                {
                    result.Add(info);
                }
            }
            
            return result;
        }
        catch (Exception)
        {
            return new List<CollectionInfo>();
        }
    }

    public async Task<UpsertResponse> UpsertAsync(UpsertRequest request)
    {
        var response = new UpsertResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var points = request.Points.Select(p => new PointStruct
            {
                Id = new PointId { Uuid = p.Id },
                Vectors = p.Vector,
                Payload = { p.Payload.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new Value { StringValue = kvp.Value?.ToString() ?? "" }
                )}
            }).ToList();

            await _client.UpsertAsync(
                request.CollectionName,
                points,
                wait: request.Wait);

            response.Success = true;
            response.ProcessedCount = request.Points.Count;
            response.ProcessedIds = request.Points.Select(p => p.Id).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return response;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        var response = new SearchResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var searchParams = new SearchParams
            {
                HnswEf = 128
            };

            var results = await _client.SearchAsync(
                request.CollectionName,
                request.QueryVector,
                limit: (ulong)request.Limit,
                scoreThreshold: request.ScoreThreshold,
                filter: MapFilter(request.Filter),
                searchParams: searchParams,
                withPayload: request.WithPayload,
                withVector: request.WithVector);

            response.Results = results.Select(r => new VectorPointWithScore
            {
                Id = r.Id.Uuid,
                Score = r.Score,
                Vector = request.WithVector ? r.Vectors.Vector.Data.ToArray() : Array.Empty<float>(),
                Payload = r.Payload.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)(kvp.Value.StringValue ?? ""))
            }).ToList();

            response.TotalFound = response.Results.Count;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return response;
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

    public async Task<DeleteResponse> DeleteAsync(DeleteRequest request)
    {
        var response = new DeleteResponse();

        try
        {
            if (request.PointIds != null && request.PointIds.Any())
            {
                var pointIds = request.PointIds.Select(id => new PointId { Uuid = id }).ToList();
                await _client.DeleteAsync(request.CollectionName, pointIds);
                response.DeletedCount = request.PointIds.Count;
            }
            else if (request.Filter != null)
            {
                await _client.DeleteAsync(request.CollectionName, MapFilter(request.Filter));
                response.DeletedCount = -1; // Unknown count when deleting by filter
            }

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return response;
    }

    public async Task<RetrieveResponse> RetrieveAsync(RetrieveRequest request)
    {
        var response = new RetrieveResponse();

        try
        {
            var pointIds = request.PointIds.Select(id => new PointId { Uuid = id }).ToList();
            
            var results = await _client.RetrieveAsync(
                request.CollectionName,
                pointIds,
                withPayload: request.WithPayload,
                withVector: request.WithVector);

            response.Points = results.Select(r => new VectorPoint
            {
                Id = r.Id.Uuid,
                Vector = request.WithVector ? r.Vectors.Vector.Data.ToArray() : Array.Empty<float>(),
                Payload = r.Payload.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)(kvp.Value.StringValue ?? ""))
            }).ToList();

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return response;
    }

    public async Task<ScrollResponse> ScrollAsync(ScrollRequest request)
    {
        var response = new ScrollResponse();

        try
        {
            var scrollResult = await _client.ScrollAsync(
                request.CollectionName,
                limit: (uint)request.Limit,
                offset: request.Offset != null ? new PointId { Uuid = request.Offset } : null,
                filter: MapFilter(request.Filter),
                withPayload: request.WithPayload,
                withVector: request.WithVector);

            response.Points = scrollResult.Select(r => new VectorPoint
            {
                Id = r.Id.Uuid,
                Vector = request.WithVector ? r.Vectors.Vector.Data.ToArray() : Array.Empty<float>(),
                Payload = r.Payload.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)(kvp.Value.StringValue ?? ""))
            }).ToList();

            response.NextOffset = response.Points.LastOrDefault()?.Id;
            response.HasMore = response.Points.Count == request.Limit;
            response.Success = true;
        }
        catch (Exception)
        {
            response.Success = false;
        }

        return response;
    }

    public async Task<UpdatePayloadResponse> UpdatePayloadAsync(UpdatePayloadRequest request)
    {
        var response = new UpdatePayloadResponse();

        try
        {
            var pointIds = request.PointIds.Select(id => new PointId { Uuid = id }).ToList();
            var payload = request.Payload.ToDictionary(
                kvp => kvp.Key,
                kvp => new Value { StringValue = kvp.Value?.ToString() ?? "" });

            if (request.Overwrite)
            {
                await _client.OverwritePayloadAsync(request.CollectionName, payload, pointIds);
            }
            else
            {
                await _client.SetPayloadAsync(request.CollectionName, payload, pointIds);
            }

            response.Success = true;
            response.UpdatedCount = request.PointIds.Count;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
        }

        return response;
    }

    public async Task<CountResponse> CountAsync(CountRequest request)
    {
        var response = new CountResponse();

        try
        {
            var result = await _client.CountAsync(
                request.CollectionName,
                filter: MapFilter(request.Filter),
                exact: request.Exact);

            response.Count = (long)result.Count;
            response.Success = true;
        }
        catch (Exception)
        {
            response.Success = false;
        }

        return response;
    }

    #region Helper Methods

    private Qdrant.Client.Grpc.Distance MapDistanceMetric(DistanceMetric metric)
    {
        return metric switch
        {
            DistanceMetric.Cosine => Qdrant.Client.Grpc.Distance.Cosine,
            DistanceMetric.Euclidean => Qdrant.Client.Grpc.Distance.Euclid,
            DistanceMetric.DotProduct => Qdrant.Client.Grpc.Distance.Dot,
            DistanceMetric.Manhattan => Qdrant.Client.Grpc.Distance.Manhattan,
            _ => Qdrant.Client.Grpc.Distance.Cosine
        };
    }

    private DistanceMetric MapDistanceMetric(Qdrant.Client.Grpc.Distance distance)
    {
        return distance switch
        {
            Qdrant.Client.Grpc.Distance.Cosine => DistanceMetric.Cosine,
            Qdrant.Client.Grpc.Distance.Euclid => DistanceMetric.Euclidean,
            Qdrant.Client.Grpc.Distance.Dot => DistanceMetric.DotProduct,
            Qdrant.Client.Grpc.Distance.Manhattan => DistanceMetric.Manhattan,
            _ => DistanceMetric.Cosine
        };
    }

    private IndexStatus MapIndexStatus(CollectionStatus status)
    {
        return status switch
        {
            CollectionStatus.Green => IndexStatus.Ready,
            CollectionStatus.Yellow => IndexStatus.Initializing,
            CollectionStatus.Red => IndexStatus.Error,
            _ => IndexStatus.Ready
        };
    }

    private Filter? MapFilter(SearchFilter? filter)
    {
        if (filter == null)
            return null;

        var qdrantFilter = new Filter();

        // Map Must conditions
        if (filter.Must.Any())
        {
            qdrantFilter.Must.AddRange(filter.Must.Select(MapCondition));
        }

        // Map Should conditions
        if (filter.Should.Any())
        {
            qdrantFilter.Should.AddRange(filter.Should.Select(MapCondition));
        }

        // Map MustNot conditions
        if (filter.MustNot.Any())
        {
            qdrantFilter.MustNot.AddRange(filter.MustNot.Select(MapCondition));
        }

        return qdrantFilter;
    }

    private Condition MapCondition(FilterCondition condition)
    {
        var qdrantCondition = new Condition();

        var match = new Match
        {
            Keyword = condition.Value?.ToString() ?? ""
        };

        qdrantCondition.Field = new FieldCondition
        {
            Key = condition.Key,
            Match = match
        };

        return qdrantCondition;
    }

    #endregion
}

#endregion

#region Service Factory

public interface IVectorDatabaseServiceFactory
{
    IVectorDatabaseService GetService(VectorDatabaseProvider? provider = null);
}

public class VectorDatabaseServiceFactory : IVectorDatabaseServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly VectorDatabaseSettings _settings;

    public VectorDatabaseServiceFactory(
        IServiceProvider serviceProvider,
        IOptions<VectorDatabaseSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    public IVectorDatabaseService GetService(VectorDatabaseProvider? provider = null)
    {
        var targetProvider = provider ?? _settings.DefaultProvider;

        return targetProvider switch
        {
            VectorDatabaseProvider.Qdrant => _serviceProvider.GetRequiredService<QdrantVectorDatabaseService>(),
            VectorDatabaseProvider.InMemory => _serviceProvider.GetRequiredService<InMemoryVectorDatabaseService>(),
            _ => throw new NotSupportedException($"Provider {targetProvider} is not supported")
        };
    }
}

#endregion
