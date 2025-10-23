using AI.DataSources.Models;
using AI.DataSources.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DataSources.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataSourceController : ControllerBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly IPostgreSqlService _postgreSqlService;
    private readonly IMongoDbService _mongoDbService;
    private readonly IAzureBlobStorageService _azureBlobStorageService;
    private readonly IAwsS3Service _awsS3Service;
    private readonly IApiClientService _apiClientService;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<DataSourceController> _logger;

    public DataSourceController(
        ISqlServerService sqlServerService,
        IPostgreSqlService postgreSqlService,
        IMongoDbService mongoDbService,
        IAzureBlobStorageService azureBlobStorageService,
        IAwsS3Service awsS3Service,
        IApiClientService apiClientService,
        IFileSystemService fileSystemService,
        ILogger<DataSourceController> logger)
    {
        _sqlServerService = sqlServerService;
        _postgreSqlService = postgreSqlService;
        _mongoDbService = mongoDbService;
        _azureBlobStorageService = azureBlobStorageService;
        _awsS3Service = awsS3Service;
        _apiClientService = apiClientService;
        _fileSystemService = fileSystemService;
        _logger = logger;
    }

    // SQL Server endpoints
    [HttpPost("sqlserver/test")]
    public async Task<ActionResult<ApiResponseModel<ConnectionTestResult>>> TestSqlServerConnection([FromBody] SqlServerConnectionConfig config)
    {
        try
        {
            var result = await _sqlServerService.TestConnectionAsync(config);
            return Ok(ApiResponseModel<ConnectionTestResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing SQL Server connection");
            return BadRequest(ApiResponseModel<ConnectionTestResult>.Fail(ex.Message));
        }
    }

    [HttpPost("sqlserver/query")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> ExecuteSqlServerQuery([FromBody] QueryRequest request)
    {
        try
        {
            var config = new SqlServerConnectionConfig
            {
                Server = request.ConnectionConfig.GetValueOrDefault("Server", ""),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                UserId = request.ConnectionConfig.GetValueOrDefault("UserId", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _sqlServerService.ExecuteQueryAsync(config, request.Query, request.Parameters);
            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL Server query");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("sqlserver/execute")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> ExecuteSqlServerNonQuery([FromBody] QueryRequest request)
    {
        try
        {
            var config = new SqlServerConnectionConfig
            {
                Server = request.ConnectionConfig.GetValueOrDefault("Server", ""),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                UserId = request.ConnectionConfig.GetValueOrDefault("UserId", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _sqlServerService.ExecuteNonQueryAsync(config, request.Query, request.Parameters);
            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL Server non-query");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("sqlserver/bulk-insert")]
    public async Task<ActionResult<ApiResponseModel<BulkInsertResult>>> SqlServerBulkInsert([FromBody] BulkInsertRequest request)
    {
        try
        {
            var config = new SqlServerConnectionConfig
            {
                Server = request.ConnectionConfig.GetValueOrDefault("Server", ""),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                UserId = request.ConnectionConfig.GetValueOrDefault("UserId", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _sqlServerService.BulkInsertAsync(config, request.TableName, request.Data);
            return Ok(ApiResponseModel<BulkInsertResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL Server bulk insert");
            return BadRequest(ApiResponseModel<BulkInsertResult>.Fail(ex.Message));
        }
    }

    // PostgreSQL endpoints
    [HttpPost("postgresql/test")]
    public async Task<ActionResult<ApiResponseModel<ConnectionTestResult>>> TestPostgreSqlConnection([FromBody] PostgreSqlConnectionConfig config)
    {
        try
        {
            var result = await _postgreSqlService.TestConnectionAsync(config);
            return Ok(ApiResponseModel<ConnectionTestResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing PostgreSQL connection");
            return BadRequest(ApiResponseModel<ConnectionTestResult>.Fail(ex.Message));
        }
    }

    [HttpPost("postgresql/query")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> ExecutePostgreSqlQuery([FromBody] QueryRequest request)
    {
        try
        {
            var config = new PostgreSqlConnectionConfig
            {
                Host = request.ConnectionConfig.GetValueOrDefault("Host", ""),
                Port = int.Parse(request.ConnectionConfig.GetValueOrDefault("Port", "5432")),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                Username = request.ConnectionConfig.GetValueOrDefault("Username", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _postgreSqlService.ExecuteQueryAsync(config, request.Query, request.Parameters);
            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PostgreSQL query");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("postgresql/execute")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> ExecutePostgreSqlNonQuery([FromBody] QueryRequest request)
    {
        try
        {
            var config = new PostgreSqlConnectionConfig
            {
                Host = request.ConnectionConfig.GetValueOrDefault("Host", ""),
                Port = int.Parse(request.ConnectionConfig.GetValueOrDefault("Port", "5432")),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                Username = request.ConnectionConfig.GetValueOrDefault("Username", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _postgreSqlService.ExecuteNonQueryAsync(config, request.Query, request.Parameters);
            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PostgreSQL non-query");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("postgresql/bulk-insert")]
    public async Task<ActionResult<ApiResponseModel<BulkInsertResult>>> PostgreSqlBulkInsert([FromBody] BulkInsertRequest request)
    {
        try
        {
            var config = new PostgreSqlConnectionConfig
            {
                Host = request.ConnectionConfig.GetValueOrDefault("Host", ""),
                Port = int.Parse(request.ConnectionConfig.GetValueOrDefault("Port", "5432")),
                Database = request.ConnectionConfig.GetValueOrDefault("Database", ""),
                Username = request.ConnectionConfig.GetValueOrDefault("Username", ""),
                Password = request.ConnectionConfig.GetValueOrDefault("Password", "")
            };

            var result = await _postgreSqlService.BulkInsertAsync(config, request.TableName, request.Data);
            return Ok(ApiResponseModel<BulkInsertResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PostgreSQL bulk insert");
            return BadRequest(ApiResponseModel<BulkInsertResult>.Fail(ex.Message));
        }
    }

    // MongoDB endpoints
    [HttpPost("mongodb/test")]
    public async Task<ActionResult<ApiResponseModel<ConnectionTestResult>>> TestMongoDbConnection([FromBody] MongoDbConnectionConfig config)
    {
        try
        {
            var result = await _mongoDbService.TestConnectionAsync(config);
            return Ok(ApiResponseModel<ConnectionTestResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing MongoDB connection");
            return BadRequest(ApiResponseModel<ConnectionTestResult>.Fail(ex.Message));
        }
    }

    [HttpPost("mongodb/find")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> MongoDbFind([FromBody] MongoQueryRequest request)
    {
        try
        {
            var config = new MongoDbConnectionConfig
            {
                ConnectionString = request.ConnectionString,
                Database = request.Database
            };

            var result = await _mongoDbService.FindAsync(
                config,
                request.Collection,
                request.FilterJson,
                request.ProjectionJson,
                request.SortJson,
                request.Limit,
                request.Skip);

            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MongoDB find");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("mongodb/insert")]
    public async Task<ActionResult<ApiResponseModel<BulkInsertResult>>> MongoDbInsert([FromBody] MongoInsertRequest request)
    {
        try
        {
            var config = new MongoDbConnectionConfig
            {
                ConnectionString = request.ConnectionString,
                Database = request.Database
            };

            var result = await _mongoDbService.InsertManyAsync(config, request.Collection, request.Documents);
            return Ok(ApiResponseModel<BulkInsertResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MongoDB insert");
            return BadRequest(ApiResponseModel<BulkInsertResult>.Fail(ex.Message));
        }
    }

    [HttpPost("mongodb/update")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> MongoDbUpdate([FromBody] MongoUpdateRequest request)
    {
        try
        {
            var config = new MongoDbConnectionConfig
            {
                ConnectionString = request.ConnectionString,
                Database = request.Database
            };

            var result = await _mongoDbService.UpdateAsync(
                config,
                request.Collection,
                request.FilterJson,
                request.UpdateJson,
                request.UpdateMany,
                request.Upsert);

            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MongoDB update");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    [HttpPost("mongodb/delete")]
    public async Task<ActionResult<ApiResponseModel<QueryResult>>> MongoDbDelete([FromBody] MongoDeleteRequest request)
    {
        try
        {
            var config = new MongoDbConnectionConfig
            {
                ConnectionString = request.ConnectionString,
                Database = request.Database
            };

            var result = await _mongoDbService.DeleteAsync(config, request.Collection, request.FilterJson, request.DeleteMany);
            return Ok(ApiResponseModel<QueryResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MongoDB delete");
            return BadRequest(ApiResponseModel<QueryResult>.Fail(ex.Message));
        }
    }

    // Azure Blob Storage endpoints
    [HttpPost("azure/test")]
    public async Task<ActionResult<ApiResponseModel<ConnectionTestResult>>> TestAzureBlobStorage([FromBody] AzureBlobStorageConfig config)
    {
        try
        {
            var result = await _azureBlobStorageService.TestConnectionAsync(config);
            return Ok(ApiResponseModel<ConnectionTestResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Azure Blob Storage connection");
            return BadRequest(ApiResponseModel<ConnectionTestResult>.Fail(ex.Message));
        }
    }

    [HttpPost("azure/upload")]
    public async Task<ActionResult<ApiResponseModel<BlobUploadResult>>> UploadToAzure([FromBody] BlobUploadRequest request)
    {
        try
        {
            var config = new AzureBlobStorageConfig
            {
                ConnectionString = request.ConnectionString,
                ContainerName = request.ContainerName
            };

            var result = await _azureBlobStorageService.UploadBlobAsync(config, request.BlobName, request.Content, request.ContentType, request.Metadata);
            return Ok(ApiResponseModel<BlobUploadResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to Azure Blob Storage");
            return BadRequest(ApiResponseModel<BlobUploadResult>.Fail(ex.Message));
        }
    }

    [HttpPost("azure/download")]
    public async Task<ActionResult<ApiResponseModel<BlobDownloadResult>>> DownloadFromAzure([FromBody] BlobDownloadRequest request)
    {
        try
        {
            var config = new AzureBlobStorageConfig
            {
                ConnectionString = request.ConnectionString,
                ContainerName = request.ContainerName
            };

            var result = await _azureBlobStorageService.DownloadBlobAsync(config, request.BlobName);
            return Ok(ApiResponseModel<BlobDownloadResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading from Azure Blob Storage");
            return BadRequest(ApiResponseModel<BlobDownloadResult>.Fail(ex.Message));
        }
    }

    [HttpPost("azure/list")]
    public async Task<ActionResult<ApiResponseModel<BlobListResult>>> ListAzureBlobs([FromBody] BlobListRequest request)
    {
        try
        {
            var config = new AzureBlobStorageConfig
            {
                ConnectionString = request.ConnectionString,
                ContainerName = request.ContainerName
            };

            var result = await _azureBlobStorageService.ListBlobsAsync(config, request.Prefix, request.MaxResults, request.ContinuationToken);
            return Ok(ApiResponseModel<BlobListResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing Azure blobs");
            return BadRequest(ApiResponseModel<BlobListResult>.Fail(ex.Message));
        }
    }

    [HttpDelete("azure/{blobName}")]
    public async Task<ActionResult<ApiResponseModel<bool>>> DeleteAzureBlob([FromBody] BlobDownloadRequest request)
    {
        try
        {
            var config = new AzureBlobStorageConfig
            {
                ConnectionString = request.ConnectionString,
                ContainerName = request.ContainerName
            };

            var result = await _azureBlobStorageService.DeleteBlobAsync(config, request.BlobName);
            return Ok(ApiResponseModel<bool>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Azure blob");
            return BadRequest(ApiResponseModel<bool>.Fail(ex.Message));
        }
    }

    // AWS S3 endpoints
    [HttpPost("aws/test")]
    public async Task<ActionResult<ApiResponseModel<ConnectionTestResult>>> TestAwsS3([FromBody] AwsS3Config config)
    {
        try
        {
            var result = await _awsS3Service.TestConnectionAsync(config);
            return Ok(ApiResponseModel<ConnectionTestResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing AWS S3 connection");
            return BadRequest(ApiResponseModel<ConnectionTestResult>.Fail(ex.Message));
        }
    }

    [HttpPost("aws/upload")]
    public async Task<ActionResult<ApiResponseModel<BlobUploadResult>>> UploadToAws([FromBody] BlobUploadRequest request)
    {
        try
        {
            var config = new AwsS3Config
            {
                BucketName = request.ContainerName,
                Region = request.ConnectionString.Split(';')[0],
                AccessKeyId = request.ConnectionString.Split(';')[1],
                SecretAccessKey = request.ConnectionString.Split(';')[2]
            };

            var result = await _awsS3Service.UploadObjectAsync(config, request.BlobName, request.Content, request.ContentType, request.Metadata);
            return Ok(ApiResponseModel<BlobUploadResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to AWS S3");
            return BadRequest(ApiResponseModel<BlobUploadResult>.Fail(ex.Message));
        }
    }

    [HttpPost("aws/download")]
    public async Task<ActionResult<ApiResponseModel<BlobDownloadResult>>> DownloadFromAws([FromBody] BlobDownloadRequest request)
    {
        try
        {
            var config = new AwsS3Config
            {
                BucketName = request.ContainerName,
                Region = request.ConnectionString.Split(';')[0],
                AccessKeyId = request.ConnectionString.Split(';')[1],
                SecretAccessKey = request.ConnectionString.Split(';')[2]
            };

            var result = await _awsS3Service.DownloadObjectAsync(config, request.BlobName);
            return Ok(ApiResponseModel<BlobDownloadResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading from AWS S3");
            return BadRequest(ApiResponseModel<BlobDownloadResult>.Fail(ex.Message));
        }
    }

    [HttpPost("aws/list")]
    public async Task<ActionResult<ApiResponseModel<BlobListResult>>> ListAwsObjects([FromBody] BlobListRequest request)
    {
        try
        {
            var config = new AwsS3Config
            {
                BucketName = request.ContainerName,
                Region = request.ConnectionString.Split(';')[0],
                AccessKeyId = request.ConnectionString.Split(';')[1],
                SecretAccessKey = request.ConnectionString.Split(';')[2]
            };

            var result = await _awsS3Service.ListObjectsAsync(config, request.Prefix, request.MaxResults, request.ContinuationToken);
            return Ok(ApiResponseModel<BlobListResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing AWS S3 objects");
            return BadRequest(ApiResponseModel<BlobListResult>.Fail(ex.Message));
        }
    }

    [HttpDelete("aws/{key}")]
    public async Task<ActionResult<ApiResponseModel<bool>>> DeleteAwsObject([FromBody] BlobDownloadRequest request)
    {
        try
        {
            var config = new AwsS3Config
            {
                BucketName = request.ContainerName,
                Region = request.ConnectionString.Split(';')[0],
                AccessKeyId = request.ConnectionString.Split(';')[1],
                SecretAccessKey = request.ConnectionString.Split(';')[2]
            };

            var result = await _awsS3Service.DeleteObjectAsync(config, request.BlobName);
            return Ok(ApiResponseModel<bool>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting AWS S3 object");
            return BadRequest(ApiResponseModel<bool>.Fail(ex.Message));
        }
    }

    // API Client endpoints
    [HttpPost("api/request")]
    public async Task<ActionResult<ApiResponseModel<ApiResponse>>> SendApiRequest([FromBody] ApiRequest request)
    {
        try
        {
            var config = new ApiConnectionConfig
            {
                BaseUrl = request.Headers?.GetValueOrDefault("BaseUrl", ""),
                Timeout = 30
            };

            var result = await _apiClientService.SendRequestAsync(config, request);
            return Ok(ApiResponseModel<ApiResponse>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending API request");
            return BadRequest(ApiResponseModel<ApiResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("api/graphql")]
    public async Task<ActionResult<ApiResponseModel<GraphQLResponse>>> SendGraphQLRequest([FromBody] GraphQLRequest request)
    {
        try
        {
            var endpoint = request.Variables?.GetValueOrDefault("endpoint", "")?.ToString() ?? "";
            var result = await _apiClientService.SendGraphQLRequestAsync(endpoint, request);
            return Ok(ApiResponseModel<GraphQLResponse>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending GraphQL request");
            return BadRequest(ApiResponseModel<GraphQLResponse>.Fail(ex.Message));
        }
    }

    // File System endpoints
    [HttpPost("file/read")]
    public async Task<ActionResult<ApiResponseModel<FileReadResult>>> ReadFile([FromBody] FileReadRequest request)
    {
        try
        {
            var result = await _fileSystemService.ReadFileAsync(request);
            return Ok(ApiResponseModel<FileReadResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file");
            return BadRequest(ApiResponseModel<FileReadResult>.Fail(ex.Message));
        }
    }

    [HttpPost("file/write")]
    public async Task<ActionResult<ApiResponseModel<FileWriteResult>>> WriteFile([FromBody] FileWriteRequest request)
    {
        try
        {
            var result = await _fileSystemService.WriteFileAsync(request);
            return Ok(ApiResponseModel<FileWriteResult>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing file");
            return BadRequest(ApiResponseModel<FileWriteResult>.Fail(ex.Message));
        }
    }

    [HttpGet("file/list")]
    public async Task<ActionResult<ApiResponseModel<List<string>>>> ListFiles([FromQuery] string directory, [FromQuery] string pattern = "*")
    {
        try
        {
            var result = await _fileSystemService.ListFilesAsync(directory, pattern);
            return Ok(ApiResponseModel<List<string>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files");
            return BadRequest(ApiResponseModel<List<string>>.Fail(ex.Message));
        }
    }

    [HttpDelete("file")]
    public async Task<ActionResult<ApiResponseModel<bool>>> DeleteFile([FromQuery] string filePath)
    {
        try
        {
            var result = await _fileSystemService.DeleteFileAsync(filePath);
            return Ok(ApiResponseModel<bool>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return BadRequest(ApiResponseModel<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("file/exists")]
    public async Task<ActionResult<ApiResponseModel<bool>>> FileExists([FromQuery] string filePath)
    {
        try
        {
            var result = await _fileSystemService.FileExistsAsync(filePath);
            return Ok(ApiResponseModel<bool>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence");
            return BadRequest(ApiResponseModel<bool>.Fail(ex.Message));
        }
    }
}
