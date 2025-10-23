using Microsoft.AspNetCore.Mvc;
using Module_06_Storage_Solutions.Models;
using Module_06_Storage_Solutions.Services;
using MongoDB.Bson;
using System.Text.Json;

namespace Module_06_Storage_Solutions.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly ILogger<StorageController> _logger;
    private readonly ISqlServerService _sqlServerService;
    private readonly IPostgreSqlService _postgreSqlService;
    private readonly IMongoDbService _mongoDbService;
    private readonly IRedisService _redisService;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IAwsStorageService _awsStorageService;
    private readonly ICachingService _cachingService;
    private readonly IStorageManagerService _storageManagerService;

    public StorageController(
        ILogger<StorageController> logger,
        ISqlServerService sqlServerService,
        IPostgreSqlService postgreSqlService,
        IMongoDbService mongoDbService,
        IRedisService redisService,
        IAzureStorageService azureStorageService,
        IAwsStorageService awsStorageService,
        ICachingService cachingService,
        IStorageManagerService storageManagerService)
    {
        _logger = logger;
        _sqlServerService = sqlServerService;
        _postgreSqlService = postgreSqlService;
        _mongoDbService = mongoDbService;
        _redisService = redisService;
        _azureStorageService = azureStorageService;
        _awsStorageService = awsStorageService;
        _cachingService = cachingService;
        _storageManagerService = storageManagerService;
    }

    #region SQL Server Endpoints

    [HttpPost("sqlserver/query")]
    public async Task<ActionResult<SqlQueryResult>> ExecuteSqlQuery([FromBody] SqlQuery query)
    {
        var result = await _sqlServerService.ExecuteQueryAsync(query);
        return Ok(result);
    }

    [HttpPost("sqlserver/execute")]
    public async Task<ActionResult<int>> ExecuteSqlCommand([FromBody] SqlQuery query)
    {
        var rowsAffected = await _sqlServerService.ExecuteAsync(query);
        return Ok(new { rowsAffected });
    }

    [HttpPost("sqlserver/bulk-insert")]
    public async Task<ActionResult<bool>> SqlServerBulkInsert([FromBody] BulkInsertRequest request)
    {
        var success = await _sqlServerService.BulkInsertAsync(request.Data, request.Options);
        return Ok(new { success });
    }

    [HttpPost("sqlserver/transaction")]
    public async Task<ActionResult<bool>> ExecuteSqlTransaction([FromBody] List<SqlQuery> queries)
    {
        var success = await _sqlServerService.ExecuteTransactionAsync(queries);
        return Ok(new { success });
    }

    [HttpPost("sqlserver/stored-procedure")]
    public async Task<ActionResult<SqlQueryResult>> ExecuteStoredProcedure([FromBody] StoredProcedureRequest request)
    {
        var result = await _sqlServerService.ExecuteStoredProcedureAsync(request.ProcedureName, request.Parameters);
        return Ok(result);
    }

    [HttpGet("sqlserver/index-suggestions/{tableName}")]
    public async Task<ActionResult<List<IndexSuggestion>>> GetIndexSuggestions(string tableName)
    {
        var suggestions = await _sqlServerService.GetIndexSuggestionsAsync(tableName);
        return Ok(suggestions);
    }

    #endregion

    #region PostgreSQL Endpoints

    [HttpPost("postgresql/query")]
    public async Task<ActionResult<SqlQueryResult>> ExecutePostgreSqlQuery([FromBody] PostgreSqlQuery query)
    {
        var result = await _postgreSqlService.ExecuteQueryAsync(query);
        return Ok(result);
    }

    [HttpPost("postgresql/execute")]
    public async Task<ActionResult<int>> ExecutePostgreSqlCommand([FromBody] PostgreSqlQuery query)
    {
        var rowsAffected = await _postgreSqlService.ExecuteAsync(query);
        return Ok(new { rowsAffected });
    }

    [HttpGet("postgresql/tables")]
    public async Task<ActionResult<List<string>>> GetPostgreSqlTables()
    {
        var tables = await _postgreSqlService.GetTableNamesAsync();
        return Ok(tables);
    }

    [HttpPost("postgresql/create-index")]
    public async Task<ActionResult<bool>> CreatePostgreSqlIndex([FromBody] CreateIndexRequest request)
    {
        var success = await _postgreSqlService.CreateIndexAsync(request.TableName, request.Columns, request.IndexName);
        return Ok(new { success });
    }

    #endregion

    #region MongoDB Endpoints

    [HttpPost("mongodb/query")]
    public async Task<ActionResult<MongoQueryResult>> ExecuteMongoQuery([FromBody] MongoQuery query)
    {
        var result = await _mongoDbService.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost("mongodb/insert")]
    public async Task<ActionResult<bool>> InsertMongoDocument([FromBody] MongoInsertRequest request)
    {
        var document = BsonDocument.Parse(JsonSerializer.Serialize(request.Document));
        var success = await _mongoDbService.InsertAsync(request.Collection, document);
        return Ok(new { success });
    }

    [HttpPost("mongodb/update")]
    public async Task<ActionResult<bool>> UpdateMongoDocument([FromBody] MongoUpdateRequest request)
    {
        var update = BsonDocument.Parse(request.Update);
        var success = await _mongoDbService.UpdateAsync(request.Collection, request.Filter, update);
        return Ok(new { success });
    }

    [HttpPost("mongodb/delete")]
    public async Task<ActionResult<bool>> DeleteMongoDocument([FromBody] MongoDeleteRequest request)
    {
        var success = await _mongoDbService.DeleteAsync(request.Collection, request.Filter);
        return Ok(new { success });
    }

    [HttpGet("mongodb/collections")]
    public async Task<ActionResult<List<string>>> GetMongoCollections()
    {
        var collections = await _mongoDbService.GetCollectionNamesAsync();
        return Ok(collections);
    }

    [HttpPost("mongodb/aggregate")]
    public async Task<ActionResult<List<BsonDocument>>> MongoAggregate([FromBody] MongoAggregateRequest request)
    {
        var pipeline = request.Pipeline.Select(p => BsonDocument.Parse(JsonSerializer.Serialize(p))).ToList();
        var results = await _mongoDbService.AggregateAsync(request.Collection, pipeline);
        return Ok(results);
    }

    #endregion

    #region Redis Endpoints

    [HttpGet("redis/get/{key}")]
    public async Task<ActionResult<object>> GetRedisValue(string key)
    {
        var value = await _redisService.GetAsync<object>(key);
        return Ok(new { key, value });
    }

    [HttpPost("redis/set")]
    public async Task<ActionResult<bool>> SetRedisValue([FromBody] RedisSetRequest request)
    {
        var success = await _redisService.SetAsync(request.Key, request.Value, request.Expiration);
        return Ok(new { success });
    }

    [HttpDelete("redis/{key}")]
    public async Task<ActionResult<bool>> DeleteRedisKey(string key)
    {
        var success = await _redisService.DeleteAsync(key);
        return Ok(new { success });
    }

    [HttpGet("redis/exists/{key}")]
    public async Task<ActionResult<bool>> RedisKeyExists(string key)
    {
        var exists = await _redisService.ExistsAsync(key);
        return Ok(new { key, exists });
    }

    [HttpPost("redis/hash/set")]
    public async Task<ActionResult<bool>> SetRedisHash([FromBody] RedisHashRequest request)
    {
        var success = await _redisService.SetHashAsync(request.Key, request.Hash);
        return Ok(new { success });
    }

    [HttpGet("redis/hash/get/{key}")]
    public async Task<ActionResult<Dictionary<string, string>?>> GetRedisHash(string key)
    {
        var hash = await _redisService.GetHashAsync(key);
        return Ok(hash);
    }

    [HttpPost("redis/list/add")]
    public async Task<ActionResult<bool>> AddToRedisList([FromBody] RedisListRequest request)
    {
        var success = await _redisService.AddToListAsync(request.Key, request.Value);
        return Ok(new { success });
    }

    [HttpGet("redis/list/{key}")]
    public async Task<ActionResult<List<string>>> GetRedisList(string key)
    {
        var list = await _redisService.GetListAsync(key);
        return Ok(list);
    }

    #endregion

    #region Azure Storage Endpoints

    [HttpPost("azure/blob/upload")]
    public async Task<ActionResult<bool>> UploadBlob([FromForm] IFormFile file, [FromForm] string blobName)
    {
        using var stream = file.OpenReadStream();
        var options = new BlobUploadOptions
        {
            BlobName = blobName,
            ContentType = file.ContentType
        };
        var success = await _azureStorageService.UploadBlobAsync(stream, options);
        return Ok(new { success });
    }

    [HttpGet("azure/blob/download/{blobName}")]
    public async Task<IActionResult> DownloadBlob(string blobName)
    {
        var options = new BlobDownloadOptions { BlobName = blobName };
        var stream = await _azureStorageService.DownloadBlobAsync(options);
        
        if (stream == null)
            return NotFound();

        return File(stream, "application/octet-stream", blobName);
    }

    [HttpGet("azure/blob/list")]
    public async Task<ActionResult<List<string>>> ListBlobs([FromQuery] string? prefix = null)
    {
        var blobs = await _azureStorageService.ListBlobsAsync(prefix);
        return Ok(blobs);
    }

    [HttpDelete("azure/blob/{blobName}")]
    public async Task<ActionResult<bool>> DeleteBlob(string blobName)
    {
        var success = await _azureStorageService.DeleteBlobAsync(blobName);
        return Ok(new { success });
    }

    [HttpPost("azure/datalake/upload")]
    public async Task<ActionResult<bool>> UploadToDataLake([FromForm] IFormFile file, [FromForm] string path)
    {
        using var stream = file.OpenReadStream();
        var success = await _azureStorageService.UploadToDataLakeAsync(path, stream);
        return Ok(new { success });
    }

    [HttpGet("azure/datalake/list")]
    public async Task<ActionResult<List<DataLakeFileInfo>>> ListDataLakeFiles([FromQuery] string path)
    {
        var files = await _azureStorageService.ListDataLakeFilesAsync(path);
        return Ok(files);
    }

    #endregion

    #region AWS Storage Endpoints

    [HttpPost("aws/s3/upload")]
    public async Task<ActionResult<bool>> UploadToS3([FromForm] IFormFile file, [FromForm] string key)
    {
        using var stream = file.OpenReadStream();
        var options = new S3UploadOptions
        {
            Key = key,
            ContentType = file.ContentType
        };
        var success = await _awsStorageService.UploadToS3Async(stream, options);
        return Ok(new { success });
    }

    [HttpGet("aws/s3/download/{key}")]
    public async Task<IActionResult> DownloadFromS3(string key)
    {
        var stream = await _awsStorageService.DownloadFromS3Async(key);
        
        if (stream == null)
            return NotFound();

        return File(stream, "application/octet-stream", key);
    }

    [HttpGet("aws/s3/list")]
    public async Task<ActionResult<List<string>>> ListS3Objects([FromQuery] string? prefix = null)
    {
        var keys = await _awsStorageService.ListS3ObjectsAsync(prefix);
        return Ok(keys);
    }

    [HttpDelete("aws/s3/{key}")]
    public async Task<ActionResult<bool>> DeleteFromS3(string key)
    {
        var success = await _awsStorageService.DeleteFromS3Async(key);
        return Ok(new { success });
    }

    [HttpPost("aws/dynamodb/put")]
    public async Task<ActionResult<bool>> PutDynamoDbItem([FromBody] DynamoDbPutRequest request)
    {
        var success = await _awsStorageService.PutDynamoDbItemAsync(request.TableName, request.Item);
        return Ok(new { success });
    }

    [HttpPost("aws/dynamodb/get")]
    public async Task<ActionResult<Dictionary<string, object>?>> GetDynamoDbItem([FromBody] DynamoDbGetRequest request)
    {
        var item = await _awsStorageService.GetDynamoDbItemAsync(request.TableName, request.Key);
        return Ok(item);
    }

    #endregion

    #region Caching Endpoints

    [HttpGet("cache/get/{key}")]
    public async Task<ActionResult<object?>> GetCachedValue(string key)
    {
        var value = await _cachingService.GetAsync<object>(key);
        return Ok(new { key, value });
    }

    [HttpPost("cache/set")]
    public async Task<ActionResult<bool>> SetCachedValue([FromBody] CacheEntry entry)
    {
        var success = await _cachingService.SetAsync(entry.Key, entry.Value, entry);
        return Ok(new { success });
    }

    [HttpDelete("cache/{key}")]
    public async Task<ActionResult<bool>> RemoveCachedValue(string key)
    {
        var success = await _cachingService.RemoveAsync(key);
        return Ok(new { success });
    }

    [HttpGet("cache/stats")]
    public async Task<ActionResult<Dictionary<string, long>>> GetCacheStats()
    {
        var stats = await _cachingService.GetCacheStatsAsync();
        return Ok(stats);
    }

    #endregion

    #region Storage Manager Endpoints

    [HttpPost("manager/execute")]
    public async Task<ActionResult<StorageOperation>> ExecuteStorageOperation([FromBody] StorageOperation operation)
    {
        var result = await _storageManagerService.ExecuteOperationAsync(operation);
        return Ok(result);
    }

    [HttpGet("manager/metrics/{storageType}")]
    public async Task<ActionResult<StorageMetrics>> GetStorageMetrics(StorageType storageType)
    {
        var metrics = await _storageManagerService.GetMetricsAsync(storageType);
        return Ok(metrics);
    }

    [HttpGet("manager/metrics")]
    public async Task<ActionResult<Dictionary<StorageType, StorageMetrics>>> GetAllStorageMetrics()
    {
        var metrics = await _storageManagerService.GetAllMetricsAsync();
        return Ok(metrics);
    }

    [HttpPost("manager/test-connection/{storageType}")]
    public async Task<ActionResult<bool>> TestStorageConnection(StorageType storageType)
    {
        var success = await _storageManagerService.TestConnectionAsync(storageType);
        return Ok(new { storageType, connected = success });
    }

    [HttpPost("manager/backup")]
    public async Task<ActionResult<BackupResult>> CreateBackup([FromBody] BackupRequest request)
    {
        var result = await _storageManagerService.CreateBackupAsync(request.StorageType, request.Config);
        return Ok(result);
    }

    #endregion

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}

#region Request Models

public class BulkInsertRequest
{
    public List<object> Data { get; set; } = new();
    public BulkInsertOptions Options { get; set; } = new();
}

public class StoredProcedureRequest
{
    public string ProcedureName { get; set; } = string.Empty;
    public Dictionary<string, object?> Parameters { get; set; } = new();
}

public class CreateIndexRequest
{
    public string TableName { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public string IndexName { get; set; } = string.Empty;
}

public class MongoInsertRequest
{
    public string Collection { get; set; } = string.Empty;
    public Dictionary<string, object> Document { get; set; } = new();
}

public class MongoUpdateRequest
{
    public string Collection { get; set; } = string.Empty;
    public string Filter { get; set; } = "{}";
    public string Update { get; set; } = "{}";
}

public class MongoDeleteRequest
{
    public string Collection { get; set; } = string.Empty;
    public string Filter { get; set; } = "{}";
}

public class MongoAggregateRequest
{
    public string Collection { get; set; } = string.Empty;
    public List<object> Pipeline { get; set; } = new();
}

public class RedisSetRequest
{
    public string Key { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public TimeSpan? Expiration { get; set; }
}

public class RedisHashRequest
{
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, string> Hash { get; set; } = new();
}

public class RedisListRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class DynamoDbPutRequest
{
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object> Item { get; set; } = new();
}

public class DynamoDbGetRequest
{
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object> Key { get; set; } = new();
}

public class BackupRequest
{
    public StorageType StorageType { get; set; }
    public BackupConfig Config { get; set; } = new();
}

#endregion
