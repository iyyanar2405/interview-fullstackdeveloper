using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Module_06_Storage_Solutions.Models;

#region Connection Models

public class ConnectionInfo
{
    public string ConnectionString { get; set; } = string.Empty;
    public StorageType StorageType { get; set; }
    public string Database { get; set; } = string.Empty;
    public Dictionary<string, string> AdditionalSettings { get; set; } = new();
    public ConnectionPoolSettings? PoolSettings { get; set; }
}

public class ConnectionPoolSettings
{
    public int MinPoolSize { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 100;
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(10);
    public bool EnableConnectionPooling { get; set; } = true;
}

public enum StorageType
{
    SqlServer,
    PostgreSQL,
    MongoDB,
    Redis,
    AzureBlob,
    AzureDataLake,
    AwsS3,
    DynamoDB,
    Elasticsearch,
    InMemory
}

#endregion

#region SQL Server Models

public class SqlServerConfig
{
    public string Server { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool TrustServerCertificate { get; set; } = true;
    public bool MultipleActiveResultSets { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int MinPoolSize { get; set; } = 5;
    public int CommandTimeout { get; set; } = 30;
}

public class SqlQuery
{
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public int? Timeout { get; set; }
    public bool UseTransaction { get; set; }
}

public class BulkInsertOptions
{
    public string TableName { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
    public int Timeout { get; set; } = 300;
    public bool EnableStreaming { get; set; } = true;
    public bool CheckConstraints { get; set; } = true;
    public bool FireTriggers { get; set; } = true;
}

public class SqlQueryResult
{
    public bool Success { get; set; }
    public int RowsAffected { get; set; }
    public List<Dictionary<string, object?>> Data { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

#endregion

#region PostgreSQL Models

public class PostgreSqlConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Pooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int MinPoolSize { get; set; } = 5;
    public int CommandTimeout { get; set; } = 30;
}

public class PostgreSqlQuery
{
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public bool UsePreparedStatement { get; set; } = true;
    public int? Timeout { get; set; }
}

#endregion

#region MongoDB Models

public class MongoDbConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public int MaxConnectionPoolSize { get; set; } = 100;
    public int MinConnectionPoolSize { get; set; } = 5;
    public TimeSpan MaxConnectionIdleTime { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan SocketTimeout { get; set; } = TimeSpan.FromMinutes(5);
}

public class MongoQuery
{
    public string Collection { get; set; } = string.Empty;
    public string Filter { get; set; } = "{}";
    public string? Projection { get; set; }
    public string? Sort { get; set; }
    public int? Limit { get; set; }
    public int? Skip { get; set; }
}

public class MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("data")]
    public Dictionary<string, object?> Data { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class MongoQueryResult
{
    public bool Success { get; set; }
    public List<BsonDocument> Documents { get; set; } = new();
    public long TotalCount { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

#endregion

#region Redis Models

public class RedisConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int Database { get; set; } = 0;
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan SyncTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public int ConnectRetry { get; set; } = 3;
    public bool AbortOnConnectFail { get; set; } = false;
    public bool AllowAdmin { get; set; } = false;
}

public class CacheEntry
{
    public string Key { get; set; } = string.Empty;
    public object? Value { get; set; }
    public TimeSpan? Expiration { get; set; }
    public CacheExpirationMode ExpirationMode { get; set; } = CacheExpirationMode.Absolute;
}

public enum CacheExpirationMode
{
    Absolute,
    Sliding
}

public class CacheOptions
{
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableCompression { get; set; } = false;
    public bool EnableDistributedLock { get; set; } = false;
    public TimeSpan LockTimeout { get; set; } = TimeSpan.FromSeconds(10);
}

#endregion

#region Azure Storage Models

public class AzureBlobConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
}

public class BlobUploadOptions
{
    public string BlobName { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string? ContentType { get; set; }
    public bool Overwrite { get; set; } = true;
    public int? MaxConcurrentConnections { get; set; }
}

public class BlobDownloadOptions
{
    public string BlobName { get; set; } = string.Empty;
    public long? Offset { get; set; }
    public long? Length { get; set; }
}

public class AzureDataLakeConfig
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string FileSystemName { get; set; } = string.Empty;
    public string ServiceUri { get; set; } = string.Empty;
}

public class DataLakeFileInfo
{
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public bool IsDirectory { get; set; }
}

#endregion

#region AWS Models

public class AwsS3Config
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public bool UseAccelerateEndpoint { get; set; } = false;
}

public class S3UploadOptions
{
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string? ContentType { get; set; }
    public string? ServerSideEncryption { get; set; }
    public bool UseMultipartUpload { get; set; } = true;
    public long MultipartThresholdMB { get; set; } = 100;
}

public class DynamoDbConfig
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string TableName { get; set; } = string.Empty;
}

public class DynamoDbQuery
{
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object> KeyConditions { get; set; } = new();
    public Dictionary<string, object>? FilterExpression { get; set; }
    public List<string>? ProjectionAttributes { get; set; }
    public int? Limit { get; set; }
    public bool ConsistentRead { get; set; } = false;
}

#endregion

#region Elasticsearch Models

public class ElasticsearchConfig
{
    public string Url { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string DefaultIndex { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

public class ElasticsearchQuery
{
    public string Index { get; set; } = string.Empty;
    public object Query { get; set; } = new { };
    public int From { get; set; } = 0;
    public int Size { get; set; } = 10;
    public List<string>? SourceIncludes { get; set; }
    public object? Sort { get; set; }
}

public class ElasticsearchDocument
{
    public string Id { get; set; } = string.Empty;
    public string Index { get; set; } = string.Empty;
    public Dictionary<string, object?> Source { get; set; } = new();
    public float? Score { get; set; }
}

#endregion

#region Storage Operations Models

public class StorageOperation
{
    public Guid OperationId { get; set; } = Guid.NewGuid();
    public StorageType StorageType { get; set; }
    public OperationType OperationType { get; set; }
    public string? EntityName { get; set; }
    public object? Data { get; set; }
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum OperationType
{
    Read,
    Write,
    Update,
    Delete,
    Query,
    BulkInsert,
    Transaction
}

public class StorageMetrics
{
    public StorageType StorageType { get; set; }
    public long TotalOperations { get; set; }
    public long SuccessfulOperations { get; set; }
    public long FailedOperations { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public DateTime LastOperationTime { get; set; }
    public Dictionary<OperationType, long> OperationCounts { get; set; } = new();
    public long CurrentConnections { get; set; }
    public long TotalDataTransferred { get; set; }
}

#endregion

#region Query Optimization Models

public class QueryOptimizationHint
{
    public string HintType { get; set; } = string.Empty;
    public string HintValue { get; set; } = string.Empty;
}

public class IndexSuggestion
{
    public string TableName { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public string IndexType { get; set; } = "NonClustered";
    public double EstimatedImprovement { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class QueryPerformanceStats
{
    public string Query { get; set; } = string.Empty;
    public TimeSpan ExecutionTime { get; set; }
    public long RowsAffected { get; set; }
    public long LogicalReads { get; set; }
    public long PhysicalReads { get; set; }
    public List<IndexSuggestion> IndexSuggestions { get; set; } = new();
}

#endregion

#region Data Warehouse Models

public class DataWarehouseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public bool EnablePartitioning { get; set; } = true;
    public PartitionStrategy PartitionStrategy { get; set; } = PartitionStrategy.Monthly;
    public bool EnableCompression { get; set; } = true;
}

public enum PartitionStrategy
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    Custom
}

public class DataLakeQuery
{
    public string Path { get; set; } = string.Empty;
    public string? FilePattern { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? Columns { get; set; }
    public string? Filter { get; set; }
}

#endregion

#region Backup and Recovery Models

public class BackupConfig
{
    public string BackupPath { get; set; } = string.Empty;
    public BackupType BackupType { get; set; } = BackupType.Full;
    public bool Compress { get; set; } = true;
    public bool Verify { get; set; } = true;
    public RetentionPolicy RetentionPolicy { get; set; } = new();
}

public enum BackupType
{
    Full,
    Differential,
    Incremental,
    Transaction
}

public class RetentionPolicy
{
    public int DaysToKeep { get; set; } = 30;
    public int WeeklyBackupsToKeep { get; set; } = 4;
    public int MonthlyBackupsToKeep { get; set; } = 12;
}

public class BackupResult
{
    public bool Success { get; set; }
    public string BackupPath { get; set; } = string.Empty;
    public long BackupSizeBytes { get; set; }
    public DateTime BackupTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ErrorMessage { get; set; }
}

#endregion
