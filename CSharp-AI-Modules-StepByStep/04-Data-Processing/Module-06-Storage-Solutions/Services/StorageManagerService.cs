using Module_06_Storage_Solutions.Models;
using System.Diagnostics;

namespace Module_06_Storage_Solutions.Services;

public interface IStorageManagerService
{
    Task<StorageOperation> ExecuteOperationAsync(StorageOperation operation);
    Task<StorageMetrics> GetMetricsAsync(StorageType storageType);
    Task<Dictionary<StorageType, StorageMetrics>> GetAllMetricsAsync();
    Task<bool> TestConnectionAsync(StorageType storageType);
    Task<BackupResult> CreateBackupAsync(StorageType storageType, BackupConfig config);
    Task<bool> RestoreBackupAsync(StorageType storageType, string backupPath);
}

public class StorageManagerService : IStorageManagerService
{
    private readonly ILogger<StorageManagerService> _logger;
    private readonly ISqlServerService _sqlServerService;
    private readonly IPostgreSqlService _postgreSqlService;
    private readonly IMongoDbService _mongoDbService;
    private readonly IRedisService _redisService;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IAwsStorageService _awsStorageService;
    private readonly Dictionary<StorageType, StorageMetrics> _metrics;

    public StorageManagerService(
        ILogger<StorageManagerService> logger,
        ISqlServerService sqlServerService,
        IPostgreSqlService postgreSqlService,
        IMongoDbService mongoDbService,
        IRedisService redisService,
        IAzureStorageService azureStorageService,
        IAwsStorageService awsStorageService)
    {
        _logger = logger;
        _sqlServerService = sqlServerService;
        _postgreSqlService = postgreSqlService;
        _mongoDbService = mongoDbService;
        _redisService = redisService;
        _azureStorageService = azureStorageService;
        _awsStorageService = awsStorageService;
        _metrics = new Dictionary<StorageType, StorageMetrics>();

        InitializeMetrics();
    }

    private void InitializeMetrics()
    {
        foreach (StorageType type in Enum.GetValues(typeof(StorageType)))
        {
            _metrics[type] = new StorageMetrics
            {
                StorageType = type,
                OperationCounts = new Dictionary<OperationType, long>()
            };
        }
    }

    public async Task<StorageOperation> ExecuteOperationAsync(StorageOperation operation)
    {
        var stopwatch = Stopwatch.StartNew();
        operation.StartTime = DateTime.UtcNow;

        try
        {
            switch (operation.StorageType)
            {
                case StorageType.SqlServer:
                    await ExecuteSqlServerOperationAsync(operation);
                    break;
                case StorageType.PostgreSQL:
                    await ExecutePostgreSqlOperationAsync(operation);
                    break;
                case StorageType.MongoDB:
                    await ExecuteMongoDbOperationAsync(operation);
                    break;
                case StorageType.Redis:
                    await ExecuteRedisOperationAsync(operation);
                    break;
                case StorageType.AzureBlob:
                case StorageType.AzureDataLake:
                    await ExecuteAzureStorageOperationAsync(operation);
                    break;
                case StorageType.AwsS3:
                case StorageType.DynamoDB:
                    await ExecuteAwsStorageOperationAsync(operation);
                    break;
                default:
                    throw new NotSupportedException($"Storage type {operation.StorageType} not supported");
            }

            operation.Success = true;
            UpdateMetrics(operation.StorageType, operation.OperationType, true, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing storage operation for {StorageType}", operation.StorageType);
            operation.Success = false;
            operation.ErrorMessage = ex.Message;
            UpdateMetrics(operation.StorageType, operation.OperationType, false, stopwatch.Elapsed);
        }
        finally
        {
            stopwatch.Stop();
            operation.EndTime = DateTime.UtcNow;
        }

        return operation;
    }

    private async Task ExecuteSqlServerOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Query:
                if (operation.Data is SqlQuery query)
                {
                    var result = await _sqlServerService.ExecuteQueryAsync(query);
                    operation.Data = result;
                }
                break;
            case OperationType.Write:
            case OperationType.Update:
            case OperationType.Delete:
                if (operation.Data is SqlQuery cmd)
                {
                    await _sqlServerService.ExecuteAsync(cmd);
                }
                break;
            case OperationType.BulkInsert:
                // Handle bulk insert
                break;
        }
    }

    private async Task ExecutePostgreSqlOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Query:
                if (operation.Data is PostgreSqlQuery query)
                {
                    var result = await _postgreSqlService.ExecuteQueryAsync(query);
                    operation.Data = result;
                }
                break;
            case OperationType.Write:
            case OperationType.Update:
            case OperationType.Delete:
                if (operation.Data is PostgreSqlQuery cmd)
                {
                    await _postgreSqlService.ExecuteAsync(cmd);
                }
                break;
        }
    }

    private async Task ExecuteMongoDbOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Query:
                if (operation.Data is MongoQuery query)
                {
                    var result = await _mongoDbService.QueryAsync(query);
                    operation.Data = result;
                }
                break;
        }
    }

    private async Task ExecuteRedisOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Read:
                if (operation.Parameters.TryGetValue("key", out var key))
                {
                    var result = await _redisService.GetAsync<object>(key?.ToString() ?? string.Empty);
                    operation.Data = result;
                }
                break;
            case OperationType.Write:
                if (operation.Parameters.TryGetValue("key", out var writeKey) && operation.Data != null)
                {
                    await _redisService.SetAsync(writeKey?.ToString() ?? string.Empty, operation.Data);
                }
                break;
        }
    }

    private async Task ExecuteAzureStorageOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Query:
                if (operation.Parameters.TryGetValue("prefix", out var prefix))
                {
                    var result = await _azureStorageService.ListBlobsAsync(prefix?.ToString());
                    operation.Data = result;
                }
                break;
        }
    }

    private async Task ExecuteAwsStorageOperationAsync(StorageOperation operation)
    {
        switch (operation.OperationType)
        {
            case OperationType.Query:
                if (operation.Parameters.TryGetValue("prefix", out var prefix))
                {
                    var result = await _awsStorageService.ListS3ObjectsAsync(prefix?.ToString());
                    operation.Data = result;
                }
                break;
        }
    }

    public Task<StorageMetrics> GetMetricsAsync(StorageType storageType)
    {
        if (_metrics.TryGetValue(storageType, out var metrics))
        {
            return Task.FromResult(metrics);
        }

        return Task.FromResult(new StorageMetrics { StorageType = storageType });
    }

    public Task<Dictionary<StorageType, StorageMetrics>> GetAllMetricsAsync()
    {
        return Task.FromResult(_metrics);
    }

    public async Task<bool> TestConnectionAsync(StorageType storageType)
    {
        try
        {
            switch (storageType)
            {
                case StorageType.SqlServer:
                    var sqlQuery = new SqlQuery { Query = "SELECT 1", Parameters = new() };
                    await _sqlServerService.ExecuteScalarAsync<int>(sqlQuery);
                    break;
                case StorageType.PostgreSQL:
                    var pgQuery = new PostgreSqlQuery { Query = "SELECT 1", Parameters = new() };
                    await _postgreSqlService.ExecuteScalarAsync<int>(pgQuery);
                    break;
                case StorageType.MongoDB:
                    await _mongoDbService.GetCollectionNamesAsync();
                    break;
                case StorageType.Redis:
                    await _redisService.ExistsAsync("test");
                    break;
                default:
                    return false;
            }

            _logger.LogInformation("Connection test successful for {StorageType}", storageType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed for {StorageType}", storageType);
            return false;
        }
    }

    public async Task<BackupResult> CreateBackupAsync(StorageType storageType, BackupConfig config)
    {
        var result = new BackupResult
        {
            BackupTime = DateTime.UtcNow,
            BackupPath = config.BackupPath
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            switch (storageType)
            {
                case StorageType.SqlServer:
                    result = await CreateSqlServerBackupAsync(config);
                    break;
                case StorageType.PostgreSQL:
                    result = await CreatePostgreSqlBackupAsync(config);
                    break;
                case StorageType.MongoDB:
                    result = await CreateMongoDbBackupAsync(config);
                    break;
                default:
                    throw new NotSupportedException($"Backup not supported for {storageType}");
            }

            result.Success = true;
            _logger.LogInformation("Backup created successfully for {StorageType} at {Path}",
                storageType, result.BackupPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup for {StorageType}", storageType);
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    private async Task<BackupResult> CreateSqlServerBackupAsync(BackupConfig config)
    {
        var backupPath = Path.Combine(config.BackupPath, $"backup_{DateTime.UtcNow:yyyyMMddHHmmss}.bak");
        
        var query = new SqlQuery
        {
            Query = $"BACKUP DATABASE @DatabaseName TO DISK = @BackupPath WITH {(config.Compress ? "COMPRESSION," : "")} INIT",
            Parameters = new Dictionary<string, object?>
            {
                ["DatabaseName"] = "YourDatabase",
                ["BackupPath"] = backupPath
            }
        };

        await _sqlServerService.ExecuteAsync(query);

        return new BackupResult
        {
            Success = true,
            BackupPath = backupPath,
            BackupTime = DateTime.UtcNow
        };
    }

    private Task<BackupResult> CreatePostgreSqlBackupAsync(BackupConfig config)
    {
        // PostgreSQL backup typically done via pg_dump command
        var backupPath = Path.Combine(config.BackupPath, $"backup_{DateTime.UtcNow:yyyyMMddHHmmss}.dump");
        
        return Task.FromResult(new BackupResult
        {
            Success = true,
            BackupPath = backupPath,
            BackupTime = DateTime.UtcNow
        });
    }

    private Task<BackupResult> CreateMongoDbBackupAsync(BackupConfig config)
    {
        // MongoDB backup typically done via mongodump command
        var backupPath = Path.Combine(config.BackupPath, $"backup_{DateTime.UtcNow:yyyyMMddHHmmss}");
        
        return Task.FromResult(new BackupResult
        {
            Success = true,
            BackupPath = backupPath,
            BackupTime = DateTime.UtcNow
        });
    }

    public async Task<bool> RestoreBackupAsync(StorageType storageType, string backupPath)
    {
        try
        {
            switch (storageType)
            {
                case StorageType.SqlServer:
                    var query = new SqlQuery
                    {
                        Query = "RESTORE DATABASE @DatabaseName FROM DISK = @BackupPath WITH REPLACE",
                        Parameters = new Dictionary<string, object?>
                        {
                            ["DatabaseName"] = "YourDatabase",
                            ["BackupPath"] = backupPath
                        }
                    };
                    await _sqlServerService.ExecuteAsync(query);
                    break;
                default:
                    throw new NotSupportedException($"Restore not supported for {storageType}");
            }

            _logger.LogInformation("Backup restored successfully for {StorageType} from {Path}",
                storageType, backupPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring backup for {StorageType}", storageType);
            return false;
        }
    }

    private void UpdateMetrics(StorageType storageType, OperationType operationType, bool success, TimeSpan duration)
    {
        if (!_metrics.TryGetValue(storageType, out var metrics))
        {
            return;
        }

        metrics.TotalOperations++;
        if (success)
            metrics.SuccessfulOperations++;
        else
            metrics.FailedOperations++;

        if (!metrics.OperationCounts.ContainsKey(operationType))
        {
            metrics.OperationCounts[operationType] = 0;
        }
        metrics.OperationCounts[operationType]++;

        // Update average response time
        var totalTime = metrics.AverageResponseTime.TotalMilliseconds * (metrics.TotalOperations - 1) + duration.TotalMilliseconds;
        metrics.AverageResponseTime = TimeSpan.FromMilliseconds(totalTime / metrics.TotalOperations);
        metrics.LastOperationTime = DateTime.UtcNow;
    }
}
