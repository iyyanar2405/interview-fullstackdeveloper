using AI.DataSources.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace AI.DataSources.Services;

public interface IMongoDbService
{
    Task<ConnectionTestResult> TestConnectionAsync(string connectionString);
    Task<QueryResult> FindAsync(string connectionString, string database, MongoQueryRequest request);
    Task<int> InsertManyAsync(string connectionString, string database, MongoInsertRequest request);
    Task<long> UpdateAsync(string connectionString, string database, MongoUpdateRequest request);
    Task<long> DeleteAsync(string connectionString, string database, MongoDeleteRequest request);
    Task<List<string>> ListCollectionsAsync(string connectionString, string database);
}

public class MongoDbService : IMongoDbService
{
    private readonly ILogger<MongoDbService> _logger;

    public MongoDbService(ILogger<MongoDbService> logger)
    {
        _logger = logger;
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ConnectionTestResult();

        try
        {
            var client = new MongoClient(connectionString);
            var databases = await client.ListDatabaseNamesAsync();
            await databases.FirstOrDefaultAsync();

            result.IsSuccessful = true;
            result.Message = "Connection successful";
            result.Metadata["ServerVersion"] = "Connected";

            _logger.LogInformation("MongoDB connection test successful");
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Connection failed: {ex.Message}";
            _logger.LogError(ex, "MongoDB connection test failed");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<QueryResult> FindAsync(string connectionString, string database, MongoQueryRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new QueryResult();

        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            var collection = db.GetCollection<BsonDocument>(request.CollectionName);

            var filter = BsonDocument.Parse(request.FilterJson);
            var findOptions = new FindOptions<BsonDocument>();

            if (!string.IsNullOrEmpty(request.ProjectionJson))
            {
                findOptions.Projection = BsonDocument.Parse(request.ProjectionJson);
            }

            if (!string.IsNullOrEmpty(request.SortJson))
            {
                findOptions.Sort = BsonDocument.Parse(request.SortJson);
            }

            if (request.Limit.HasValue)
            {
                findOptions.Limit = request.Limit.Value;
            }

            if (request.Skip.HasValue)
            {
                findOptions.Skip = request.Skip.Value;
            }

            var cursor = await collection.FindAsync(filter, findOptions);
            var documents = await cursor.ToListAsync();

            result.Rows = documents.Select(doc => BsonDocumentToDictionary(doc)).ToList();
            result.RowCount = result.Rows.Count;
            result.IsSuccessful = true;

            _logger.LogInformation("MongoDB find completed: {RowCount} documents", result.RowCount);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error executing MongoDB find");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<int> InsertManyAsync(string connectionString, string database, MongoInsertRequest request)
    {
        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            var collection = db.GetCollection<BsonDocument>(request.CollectionName);

            var documents = request.Documents.Select(DictionaryToBsonDocument).ToList();
            await collection.InsertManyAsync(documents);

            _logger.LogInformation("MongoDB insert completed: {Count} documents", documents.Count);

            return documents.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting documents to MongoDB");
            throw;
        }
    }

    public async Task<long> UpdateAsync(string connectionString, string database, MongoUpdateRequest request)
    {
        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            var collection = db.GetCollection<BsonDocument>(request.CollectionName);

            var filter = BsonDocument.Parse(request.FilterJson);
            var update = BsonDocument.Parse(request.UpdateJson);

            UpdateResult updateResult;
            if (request.UpdateMany)
            {
                updateResult = await collection.UpdateManyAsync(filter, update);
            }
            else
            {
                updateResult = await collection.UpdateOneAsync(filter, update);
            }

            _logger.LogInformation("MongoDB update completed: {ModifiedCount} documents", updateResult.ModifiedCount);

            return updateResult.ModifiedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MongoDB documents");
            throw;
        }
    }

    public async Task<long> DeleteAsync(string connectionString, string database, MongoDeleteRequest request)
    {
        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            var collection = db.GetCollection<BsonDocument>(request.CollectionName);

            var filter = BsonDocument.Parse(request.FilterJson);

            DeleteResult deleteResult;
            if (request.DeleteMany)
            {
                deleteResult = await collection.DeleteManyAsync(filter);
            }
            else
            {
                deleteResult = await collection.DeleteOneAsync(filter);
            }

            _logger.LogInformation("MongoDB delete completed: {DeletedCount} documents", deleteResult.DeletedCount);

            return deleteResult.DeletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MongoDB documents");
            throw;
        }
    }

    public async Task<List<string>> ListCollectionsAsync(string connectionString, string database)
    {
        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            var collections = await db.ListCollectionNamesAsync();
            return await collections.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing MongoDB collections");
            throw;
        }
    }

    private Dictionary<string, object> BsonDocumentToDictionary(BsonDocument document)
    {
        var dict = new Dictionary<string, object>();
        foreach (var element in document.Elements)
        {
            dict[element.Name] = BsonValueToObject(element.Value);
        }
        return dict;
    }

    private object BsonValueToObject(BsonValue value)
    {
        return value.BsonType switch
        {
            BsonType.String => value.AsString,
            BsonType.Int32 => value.AsInt32,
            BsonType.Int64 => value.AsInt64,
            BsonType.Double => value.AsDouble,
            BsonType.Boolean => value.AsBoolean,
            BsonType.DateTime => value.ToUniversalTime(),
            BsonType.ObjectId => value.AsObjectId.ToString(),
            BsonType.Document => BsonDocumentToDictionary(value.AsBsonDocument),
            BsonType.Array => value.AsBsonArray.Select(BsonValueToObject).ToList(),
            BsonType.Null => null!,
            _ => value.ToString()!
        };
    }

    private BsonDocument DictionaryToBsonDocument(Dictionary<string, object> dict)
    {
        var document = new BsonDocument();
        foreach (var kvp in dict)
        {
            document[kvp.Key] = BsonValue.Create(kvp.Value);
        }
        return document;
    }
}
