namespace AI.DataSources.Models;

#region Enums

public enum DataSourceType
{
    SqlServer,
    PostgreSQL,
    MongoDB,
    MySQL,
    Redis,
    AzureBlobStorage,
    AwsS3,
    FileSystem,
    RestApi,
    GraphQL,
    Kafka,
    EventHub
}

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Connected,
    Error,
    Timeout
}

public enum QueryResultFormat
{
    Json,
    Csv,
    Xml,
    DataTable,
    Dictionary
}

#endregion

#region Connection Models

public class DataSourceConnection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public DataSourceType Type { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public ConnectionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastConnectedAt { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
    public Dictionary<string, string> Credentials { get; set; } = new();
}

public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public string? Version { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SqlServerConnectionConfig
{
    public string Server { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IntegratedSecurity { get; set; }
    public int Timeout { get; set; } = 30;
    public bool MultipleActiveResultSets { get; set; } = true;
    public bool TrustServerCertificate { get; set; }
}

public class PostgreSqlConnectionConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
    public bool Pooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
}

public class MongoDbConnectionConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 27017;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? AuthDatabase { get; set; }
    public bool UseTls { get; set; }
}

#endregion

#region Query Models

public class QueryRequest
{
    public string QueryText { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int? Timeout { get; set; }
    public bool UsePreparedStatement { get; set; }
    public QueryResultFormat Format { get; set; } = QueryResultFormat.Json;
}

public class QueryResult
{
    public bool IsSuccessful { get; set; }
    public int RowCount { get; set; }
    public List<Dictionary<string, object>> Rows { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class BulkInsertRequest
{
    public string TableName { get; set; } = string.Empty;
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public int BatchSize { get; set; } = 1000;
    public bool UseTransaction { get; set; } = true;
}

public class BulkInsertResult
{
    public bool IsSuccessful { get; set; }
    public int RowsInserted { get; set; }
    public int RowsFailed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public List<string> Errors { get; set; } = new();
}

#endregion

#region Cloud Storage Models

public class AzureBlobStorageConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}

public class AwsS3Config
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
}

public class BlobUploadRequest
{
    public string BlobName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class BlobUploadResult
{
    public bool IsSuccessful { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ETag { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class BlobDownloadRequest
{
    public string BlobName { get; set; } = string.Empty;
    public long? Offset { get; set; }
    public long? Length { get; set; }
}

public class BlobDownloadResult
{
    public bool IsSuccessful { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class BlobListRequest
{
    public string? Prefix { get; set; }
    public int MaxResults { get; set; } = 100;
    public string? ContinuationToken { get; set; }
}

public class BlobListResult
{
    public List<BlobItem> Blobs { get; set; } = new();
    public string? ContinuationToken { get; set; }
    public bool HasMore { get; set; }
}

public class BlobItem
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string ETag { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

#endregion

#region API Models

public class ApiConnectionConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public int Timeout { get; set; } = 30;
    public bool EnableRetry { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
}

public class ApiRequest
{
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, string>? QueryParameters { get; set; }
    public object? Body { get; set; }
}

public class ApiResponse
{
    public bool IsSuccessful { get; set; }
    public int StatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public TimeSpan ResponseTime { get; set; }
    public string? ErrorMessage { get; set; }
}

public class GraphQLRequest
{
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object>? Variables { get; set; }
    public string? OperationName { get; set; }
}

public class GraphQLResponse
{
    public bool IsSuccessful { get; set; }
    public object? Data { get; set; }
    public List<GraphQLError> Errors { get; set; } = new();
}

public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public List<GraphQLLocation> Locations { get; set; } = new();
    public List<object> Path { get; set; } = new();
}

public class GraphQLLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}

#endregion

#region File System Models

public class FileSystemConfig
{
    public string RootPath { get; set; } = string.Empty;
    public bool EnableWatch { get; set; }
    public List<string> WatchPatterns { get; set; } = new();
}

public class FileReadRequest
{
    public string FilePath { get; set; } = string.Empty;
    public bool ReadAsBinary { get; set; }
    public string Encoding { get; set; } = "UTF-8";
}

public class FileReadResult
{
    public bool IsSuccessful { get; set; }
    public string? TextContent { get; set; }
    public byte[]? BinaryContent { get; set; }
    public long Size { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class FileWriteRequest
{
    public string FilePath { get; set; } = string.Empty;
    public string? TextContent { get; set; }
    public byte[]? BinaryContent { get; set; }
    public bool Overwrite { get; set; } = true;
    public string Encoding { get; set; } = "UTF-8";
}

public class FileWriteResult
{
    public bool IsSuccessful { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long BytesWritten { get; set; }
}

#endregion

#region MongoDB Models

public class MongoQueryRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public string FilterJson { get; set; } = "{}";
    public string? ProjectionJson { get; set; }
    public string? SortJson { get; set; }
    public int? Limit { get; set; }
    public int? Skip { get; set; }
}

public class MongoInsertRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public List<Dictionary<string, object>> Documents { get; set; } = new();
}

public class MongoUpdateRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public string FilterJson { get; set; } = "{}";
    public string UpdateJson { get; set; } = string.Empty;
    public bool UpdateMany { get; set; }
    public bool Upsert { get; set; }
}

public class MongoDeleteRequest
{
    public string CollectionName { get; set; } = string.Empty;
    public string FilterJson { get; set; } = "{}";
    public bool DeleteMany { get; set; }
}

#endregion

#region Data Transfer Models

public class DataTransferRequest
{
    public DataSourceConnection Source { get; set; } = new();
    public DataSourceConnection Destination { get; set; } = new();
    public string SourceQuery { get; set; } = string.Empty;
    public string DestinationTable { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
    public bool TruncateDestination { get; set; }
}

public class DataTransferResult
{
    public bool IsSuccessful { get; set; }
    public int RowsTransferred { get; set; }
    public int RowsFailed { get; set; }
    public TimeSpan TotalTime { get; set; }
    public List<string> Errors { get; set; } = new();
}

#endregion

#region Response Models

public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();

    public static ApiResponseModel<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponseModel<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponseModel<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ApiResponseModel<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

#endregion

#region Settings

public class DataSourcesSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    public int DefaultTimeout { get; set; } = 30;
    public int MaxConnectionPoolSize { get; set; } = 100;
    public bool EnableConnectionPooling { get; set; } = true;
    public bool EnableQueryLogging { get; set; } = true;
    public bool EnablePerformanceMetrics { get; set; } = true;
}

#endregion
