using MongoDB.Bson;
using MongoDB.Driver;
using Module_06_Storage_Solutions.Models;
using System.Diagnostics;

namespace Module_06_Storage_Solutions.Services;

public interface IMongoDbService
{
    Task<MongoQueryResult> QueryAsync(MongoQuery query);
    Task<bool> InsertAsync(string collection, BsonDocument document);
    Task<bool> InsertManyAsync(string collection, List<BsonDocument> documents);
    Task<bool> UpdateAsync(string collection, string filter, BsonDocument update);
    Task<bool> DeleteAsync(string collection, string filter);
    Task<long> CountAsync(string collection, string filter);
    Task<List<string>> GetCollectionNamesAsync();
    Task<bool> CreateIndexAsync(string collection, string field, bool ascending = true);
    Task<List<BsonDocument>> AggregateAsync(string collection, List<BsonDocument> pipeline);
}

public class MongoDbService : IMongoDbService
{
    private readonly ILogger<MongoDbService> _logger;
    private readonly IMongoDatabase _database;
    private readonly MongoClient _client;

    public MongoDbService(ILogger<MongoDbService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var config = configuration.GetSection("MongoDB").Get<MongoDbConfig>()
            ?? throw new InvalidOperationException("MongoDB configuration not found");

        var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
        settings.MaxConnectionPoolSize = config.MaxConnectionPoolSize;
        settings.MinConnectionPoolSize = config.MinConnectionPoolSize;
        settings.MaxConnectionIdleTime = config.MaxConnectionIdleTime;
        settings.ConnectTimeout = config.ConnectTimeout;
        settings.SocketTimeout = config.SocketTimeout;

        _client = new MongoClient(settings);
        _database = _client.GetDatabase(config.Database);
    }

    public async Task<MongoQueryResult> QueryAsync(MongoQuery query)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new MongoQueryResult();

        try
        {
            var collection = _database.GetCollection<BsonDocument>(query.Collection);
            
            var filter = BsonDocument.Parse(query.Filter);
            var findOptions = new FindOptions<BsonDocument>
            {
                Limit = query.Limit,
                Skip = query.Skip
            };

            if (!string.IsNullOrEmpty(query.Projection))
            {
                findOptions.Projection = BsonDocument.Parse(query.Projection);
            }

            if (!string.IsNullOrEmpty(query.Sort))
            {
                findOptions.Sort = BsonDocument.Parse(query.Sort);
            }

            using var cursor = await collection.FindAsync(filter, findOptions);
            result.Documents = await cursor.ToListAsync();
            result.TotalCount = await collection.CountDocumentsAsync(filter);
            result.Success = true;

            _logger.LogInformation("MongoDB query executed successfully. Found {Count} documents in {Time}ms",
                result.Documents.Count, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MongoDB query");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<bool> InsertAsync(string collection, BsonDocument document)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            await coll.InsertOneAsync(document);

            _logger.LogInformation("Inserted document into {Collection}", collection);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting document into {Collection}", collection);
            return false;
        }
    }

    public async Task<bool> InsertManyAsync(string collection, List<BsonDocument> documents)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            await coll.InsertManyAsync(documents);

            _logger.LogInformation("Inserted {Count} documents into {Collection}",
                documents.Count, collection);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting documents into {Collection}", collection);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(string collection, string filter, BsonDocument update)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            var filterDoc = BsonDocument.Parse(filter);
            
            var result = await coll.UpdateManyAsync(filterDoc, update);

            _logger.LogInformation("Updated {Count} documents in {Collection}",
                result.ModifiedCount, collection);
            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating documents in {Collection}", collection);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string collection, string filter)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            var filterDoc = BsonDocument.Parse(filter);
            
            var result = await coll.DeleteManyAsync(filterDoc);

            _logger.LogInformation("Deleted {Count} documents from {Collection}",
                result.DeletedCount, collection);
            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting documents from {Collection}", collection);
            return false;
        }
    }

    public async Task<long> CountAsync(string collection, string filter)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            var filterDoc = BsonDocument.Parse(filter);
            
            var count = await coll.CountDocumentsAsync(filterDoc);

            _logger.LogInformation("Counted {Count} documents in {Collection}", count, collection);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting documents in {Collection}", collection);
            return 0;
        }
    }

    public async Task<List<string>> GetCollectionNamesAsync()
    {
        try
        {
            var collections = await _database.ListCollectionNamesAsync();
            var collectionList = await collections.ToListAsync();

            _logger.LogInformation("Retrieved {Count} collection names", collectionList.Count);
            return collectionList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection names");
            throw;
        }
    }

    public async Task<bool> CreateIndexAsync(string collection, string field, bool ascending = true)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            var indexKeys = ascending 
                ? Builders<BsonDocument>.IndexKeys.Ascending(field)
                : Builders<BsonDocument>.IndexKeys.Descending(field);

            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys);
            await coll.Indexes.CreateOneAsync(indexModel);

            _logger.LogInformation("Created index on {Field} in {Collection}", field, collection);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating index on {Field} in {Collection}", field, collection);
            return false;
        }
    }

    public async Task<List<BsonDocument>> AggregateAsync(string collection, List<BsonDocument> pipeline)
    {
        try
        {
            var coll = _database.GetCollection<BsonDocument>(collection);
            var pipelineDefinition = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline);
            
            var cursor = await coll.AggregateAsync(pipelineDefinition);
            var results = await cursor.ToListAsync();

            _logger.LogInformation("Aggregation pipeline executed on {Collection}, returned {Count} documents",
                collection, results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing aggregation pipeline on {Collection}", collection);
            throw;
        }
    }
}
