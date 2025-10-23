using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Module_06_Storage_Solutions.Models;

namespace Module_06_Storage_Solutions.Services;

public interface IAzureStorageService
{
    Task<bool> UploadBlobAsync(Stream data, BlobUploadOptions options);
    Task<Stream?> DownloadBlobAsync(BlobDownloadOptions options);
    Task<bool> DeleteBlobAsync(string blobName);
    Task<List<string>> ListBlobsAsync(string? prefix = null);
    Task<bool> BlobExistsAsync(string blobName);
    Task<BlobProperties?> GetBlobPropertiesAsync(string blobName);
    Task<bool> UploadToDataLakeAsync(string path, Stream data, Dictionary<string, string>? metadata = null);
    Task<Stream?> DownloadFromDataLakeAsync(string path);
    Task<List<DataLakeFileInfo>> ListDataLakeFilesAsync(string path);
    Task<bool> DeleteDataLakeFileAsync(string path);
}

public class AzureStorageService : IAzureStorageService
{
    private readonly ILogger<AzureStorageService> _logger;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly DataLakeFileSystemClient? _dataLakeFileSystemClient;

    public AzureStorageService(ILogger<AzureStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        var blobConfig = configuration.GetSection("AzureBlob").Get<AzureBlobConfig>();
        if (blobConfig != null)
        {
            _blobContainerClient = new BlobContainerClient(
                blobConfig.ConnectionString,
                blobConfig.ContainerName);
            _blobContainerClient.CreateIfNotExists();
        }
        else
        {
            _blobContainerClient = null!;
        }

        var dataLakeConfig = configuration.GetSection("AzureDataLake").Get<AzureDataLakeConfig>();
        if (dataLakeConfig != null)
        {
            var serviceClient = new DataLakeServiceClient(
                new Uri(dataLakeConfig.ServiceUri),
                new Azure.Storage.StorageSharedKeyCredential(
                    dataLakeConfig.AccountName,
                    dataLakeConfig.AccountKey));

            _dataLakeFileSystemClient = serviceClient.GetFileSystemClient(dataLakeConfig.FileSystemName);
            _dataLakeFileSystemClient.CreateIfNotExists();
        }
    }

    public async Task<bool> UploadBlobAsync(Stream data, BlobUploadOptions options)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(options.BlobName);

            var uploadOptions = new Azure.Storage.Blobs.Models.BlobUploadOptions
            {
                Metadata = options.Metadata,
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = options.ContentType
                }
            };

            if (options.MaxConcurrentConnections.HasValue)
            {
                uploadOptions.TransferOptions = new Azure.Storage.StorageTransferOptions
                {
                    MaximumConcurrency = options.MaxConcurrentConnections.Value
                };
            }

            await blobClient.UploadAsync(data, uploadOptions);

            _logger.LogInformation("Uploaded blob {BlobName} successfully", options.BlobName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blob {BlobName}", options.BlobName);
            return false;
        }
    }

    public async Task<Stream?> DownloadBlobAsync(BlobDownloadOptions options)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(options.BlobName);

            var downloadOptions = new BlobDownloadOptions();
            if (options.Offset.HasValue || options.Length.HasValue)
            {
                downloadOptions.Range = new Azure.HttpRange(
                    options.Offset ?? 0,
                    options.Length);
            }

            var response = await blobClient.DownloadStreamingAsync(downloadOptions);
            var stream = response.Value.Content;

            _logger.LogInformation("Downloaded blob {BlobName} successfully", options.BlobName);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading blob {BlobName}", options.BlobName);
            return null;
        }
    }

    public async Task<bool> DeleteBlobAsync(string blobName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation("Deleted blob {BlobName}", blobName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob {BlobName}", blobName);
            return false;
        }
    }

    public async Task<List<string>> ListBlobsAsync(string? prefix = null)
    {
        var blobNames = new List<string>();

        try
        {
            await foreach (var blobItem in _blobContainerClient.GetBlobsAsync(prefix: prefix))
            {
                blobNames.Add(blobItem.Name);
            }

            _logger.LogInformation("Listed {Count} blobs with prefix {Prefix}",
                blobNames.Count, prefix ?? "none");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs with prefix {Prefix}", prefix);
        }

        return blobNames;
    }

    public async Task<bool> BlobExistsAsync(string blobName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking blob existence {BlobName}", blobName);
            return false;
        }
    }

    public async Task<BlobProperties?> GetBlobPropertiesAsync(string blobName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var properties = await blobClient.GetPropertiesAsync();

            _logger.LogInformation("Retrieved properties for blob {BlobName}", blobName);
            return properties.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blob properties {BlobName}", blobName);
            return null;
        }
    }

    public async Task<bool> UploadToDataLakeAsync(string path, Stream data, Dictionary<string, string>? metadata = null)
    {
        try
        {
            if (_dataLakeFileSystemClient == null)
            {
                throw new InvalidOperationException("Data Lake not configured");
            }

            var fileClient = _dataLakeFileSystemClient.GetFileClient(path);
            await fileClient.CreateAsync();

            await fileClient.AppendAsync(data, 0);
            await fileClient.FlushAsync(data.Length);

            if (metadata != null && metadata.Any())
            {
                await fileClient.SetMetadataAsync(metadata);
            }

            _logger.LogInformation("Uploaded file to Data Lake at {Path}", path);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to Data Lake at {Path}", path);
            return false;
        }
    }

    public async Task<Stream?> DownloadFromDataLakeAsync(string path)
    {
        try
        {
            if (_dataLakeFileSystemClient == null)
            {
                throw new InvalidOperationException("Data Lake not configured");
            }

            var fileClient = _dataLakeFileSystemClient.GetFileClient(path);
            var response = await fileClient.ReadAsync();

            _logger.LogInformation("Downloaded file from Data Lake at {Path}", path);
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading from Data Lake at {Path}", path);
            return null;
        }
    }

    public async Task<List<DataLakeFileInfo>> ListDataLakeFilesAsync(string path)
    {
        var files = new List<DataLakeFileInfo>();

        try
        {
            if (_dataLakeFileSystemClient == null)
            {
                throw new InvalidOperationException("Data Lake not configured");
            }

            await foreach (var pathItem in _dataLakeFileSystemClient.GetPathsAsync(path))
            {
                files.Add(new DataLakeFileInfo
                {
                    Path = pathItem.Name,
                    Size = pathItem.ContentLength ?? 0,
                    LastModified = pathItem.LastModified.DateTime,
                    IsDirectory = pathItem.IsDirectory ?? false
                });
            }

            _logger.LogInformation("Listed {Count} files in Data Lake path {Path}",
                files.Count, path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing Data Lake files at {Path}", path);
        }

        return files;
    }

    public async Task<bool> DeleteDataLakeFileAsync(string path)
    {
        try
        {
            if (_dataLakeFileSystemClient == null)
            {
                throw new InvalidOperationException("Data Lake not configured");
            }

            var fileClient = _dataLakeFileSystemClient.GetFileClient(path);
            await fileClient.DeleteIfExistsAsync();

            _logger.LogInformation("Deleted file from Data Lake at {Path}", path);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Data Lake file at {Path}", path);
            return false;
        }
    }
}
