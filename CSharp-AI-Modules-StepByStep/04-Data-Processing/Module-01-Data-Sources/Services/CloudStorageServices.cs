using AI.DataSources.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Amazon.S3;
using Amazon.S3.Model;
using System.Diagnostics;

namespace AI.DataSources.Services;

public interface IAzureBlobStorageService
{
    Task<ConnectionTestResult> TestConnectionAsync(string connectionString, string containerName);
    Task<BlobUploadResult> UploadBlobAsync(string connectionString, string containerName, BlobUploadRequest request);
    Task<BlobDownloadResult> DownloadBlobAsync(string connectionString, string containerName, BlobDownloadRequest request);
    Task<BlobListResult> ListBlobsAsync(string connectionString, string containerName, BlobListRequest request);
    Task<bool> DeleteBlobAsync(string connectionString, string containerName, string blobName);
}

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString, string containerName)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ConnectionTestResult();

        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var exists = await containerClient.ExistsAsync();

            result.IsSuccessful = exists.Value;
            result.Message = exists.Value ? "Container accessible" : "Container not found";
            result.Metadata["AccountName"] = blobServiceClient.AccountName;

            _logger.LogInformation("Azure Blob Storage test: {Success}", result.IsSuccessful);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Connection failed: {ex.Message}";
            _logger.LogError(ex, "Azure Blob Storage test failed");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<BlobUploadResult> UploadBlobAsync(string connectionString, string containerName, BlobUploadRequest request)
    {
        var result = new BlobUploadResult();

        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(request.BlobName);

            using var stream = new MemoryStream(request.Content);
            var uploadResult = await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = request.ContentType
            });

            if (request.Metadata.Any())
            {
                await blobClient.SetMetadataAsync(request.Metadata);
            }

            result.IsSuccessful = true;
            result.BlobUrl = blobClient.Uri.ToString();
            result.Size = request.Content.Length;
            result.ETag = uploadResult.Value.ETag.ToString();
            result.UploadedAt = DateTime.UtcNow;

            _logger.LogInformation("Blob uploaded: {BlobName}, Size: {Size} bytes", request.BlobName, result.Size);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error uploading blob: {BlobName}", request.BlobName);
            throw;
        }

        return result;
    }

    public async Task<BlobDownloadResult> DownloadBlobAsync(string connectionString, string containerName, BlobDownloadRequest request)
    {
        var result = new BlobDownloadResult();

        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(request.BlobName);

            var download = await blobClient.DownloadContentAsync();
            var properties = await blobClient.GetPropertiesAsync();

            result.IsSuccessful = true;
            result.Content = download.Value.Content.ToArray();
            result.ContentType = properties.Value.ContentType;
            result.Size = properties.Value.ContentLength;
            result.Metadata = properties.Value.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _logger.LogInformation("Blob downloaded: {BlobName}, Size: {Size} bytes", request.BlobName, result.Size);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error downloading blob: {BlobName}", request.BlobName);
            throw;
        }

        return result;
    }

    public async Task<BlobListResult> ListBlobsAsync(string connectionString, string containerName, BlobListRequest request)
    {
        var result = new BlobListResult();

        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobs = containerClient.GetBlobsAsync(prefix: request.Prefix)
                .AsPages(request.ContinuationToken, request.MaxResults);

            await foreach (var page in blobs)
            {
                foreach (var blob in page.Values)
                {
                    result.Blobs.Add(new BlobItem
                    {
                        Name = blob.Name,
                        Size = blob.Properties.ContentLength ?? 0,
                        ContentType = blob.Properties.ContentType ?? "",
                        CreatedAt = blob.Properties.CreatedOn?.UtcDateTime ?? DateTime.MinValue,
                        ModifiedAt = blob.Properties.LastModified?.UtcDateTime ?? DateTime.MinValue,
                        ETag = blob.Properties.ETag.ToString() ?? "",
                        Metadata = blob.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    });
                }

                result.ContinuationToken = page.ContinuationToken;
                result.HasMore = !string.IsNullOrEmpty(page.ContinuationToken);
                break; // Only first page
            }

            _logger.LogInformation("Listed {Count} blobs", result.Blobs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs");
            throw;
        }

        return result;
    }

    public async Task<bool> DeleteBlobAsync(string connectionString, string containerName, string blobName)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation("Blob deleted: {BlobName}, Success: {Success}", blobName, response.Value);

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob: {BlobName}", blobName);
            throw;
        }
    }
}

public interface IAwsS3Service
{
    Task<ConnectionTestResult> TestConnectionAsync(AwsS3Config config);
    Task<BlobUploadResult> UploadObjectAsync(AwsS3Config config, BlobUploadRequest request);
    Task<BlobDownloadResult> DownloadObjectAsync(AwsS3Config config, BlobDownloadRequest request);
    Task<BlobListResult> ListObjectsAsync(AwsS3Config config, BlobListRequest request);
    Task<bool> DeleteObjectAsync(AwsS3Config config, string key);
}

public class AwsS3Service : IAwsS3Service
{
    private readonly ILogger<AwsS3Service> _logger;

    public AwsS3Service(ILogger<AwsS3Service> logger)
    {
        _logger = logger;
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(AwsS3Config config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ConnectionTestResult();

        try
        {
            var s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));

            var bucketRequest = new GetBucketLocationRequest { BucketName = config.BucketName };
            var bucketResponse = await s3Client.GetBucketLocationAsync(bucketRequest);

            result.IsSuccessful = true;
            result.Message = "Bucket accessible";
            result.Metadata["BucketLocation"] = bucketResponse.Location.Value;

            _logger.LogInformation("AWS S3 test successful: {Bucket}", config.BucketName);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Connection failed: {ex.Message}";
            _logger.LogError(ex, "AWS S3 test failed");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<BlobUploadResult> UploadObjectAsync(AwsS3Config config, BlobUploadRequest request)
    {
        var result = new BlobUploadResult();

        try
        {
            var s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));

            using var stream = new MemoryStream(request.Content);
            var putRequest = new PutObjectRequest
            {
                BucketName = config.BucketName,
                Key = request.BlobName,
                InputStream = stream,
                ContentType = request.ContentType
            };

            foreach (var meta in request.Metadata)
            {
                putRequest.Metadata.Add(meta.Key, meta.Value);
            }

            var response = await s3Client.PutObjectAsync(putRequest);

            result.IsSuccessful = true;
            result.BlobUrl = $"https://{config.BucketName}.s3.{config.Region}.amazonaws.com/{request.BlobName}";
            result.Size = request.Content.Length;
            result.ETag = response.ETag;
            result.UploadedAt = DateTime.UtcNow;

            _logger.LogInformation("S3 object uploaded: {Key}, Size: {Size} bytes", request.BlobName, result.Size);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error uploading S3 object: {Key}", request.BlobName);
            throw;
        }

        return result;
    }

    public async Task<BlobDownloadResult> DownloadObjectAsync(AwsS3Config config, BlobDownloadRequest request)
    {
        var result = new BlobDownloadResult();

        try
        {
            var s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));

            var getRequest = new GetObjectRequest
            {
                BucketName = config.BucketName,
                Key = request.BlobName
            };

            using var response = await s3Client.GetObjectAsync(getRequest);
            using var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms);

            result.IsSuccessful = true;
            result.Content = ms.ToArray();
            result.ContentType = response.Headers.ContentType;
            result.Size = response.ContentLength;
            result.Metadata = response.Metadata.Keys.ToDictionary(k => k, k => response.Metadata[k]);

            _logger.LogInformation("S3 object downloaded: {Key}, Size: {Size} bytes", request.BlobName, result.Size);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error downloading S3 object: {Key}", request.BlobName);
            throw;
        }

        return result;
    }

    public async Task<BlobListResult> ListObjectsAsync(AwsS3Config config, BlobListRequest request)
    {
        var result = new BlobListResult();

        try
        {
            var s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));

            var listRequest = new ListObjectsV2Request
            {
                BucketName = config.BucketName,
                Prefix = request.Prefix,
                MaxKeys = request.MaxResults,
                ContinuationToken = request.ContinuationToken
            };

            var response = await s3Client.ListObjectsV2Async(listRequest);

            result.Blobs = response.S3Objects.Select(obj => new BlobItem
            {
                Name = obj.Key,
                Size = obj.Size,
                ModifiedAt = obj.LastModified,
                ETag = obj.ETag
            }).ToList();

            result.ContinuationToken = response.NextContinuationToken;
            result.HasMore = response.IsTruncated;

            _logger.LogInformation("Listed {Count} S3 objects", result.Blobs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing S3 objects");
            throw;
        }

        return result;
    }

    public async Task<bool> DeleteObjectAsync(AwsS3Config config, string key)
    {
        try
        {
            var s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = config.BucketName,
                Key = key
            };

            await s3Client.DeleteObjectAsync(deleteRequest);

            _logger.LogInformation("S3 object deleted: {Key}", key);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting S3 object: {Key}", key);
            throw;
        }
    }
}
