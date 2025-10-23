using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Module_06_Storage_Solutions.Models;

namespace Module_06_Storage_Solutions.Services;

public interface IAwsStorageService
{
    Task<bool> UploadToS3Async(Stream data, S3UploadOptions options);
    Task<Stream?> DownloadFromS3Async(string key);
    Task<bool> DeleteFromS3Async(string key);
    Task<List<string>> ListS3ObjectsAsync(string? prefix = null);
    Task<bool> S3ObjectExistsAsync(string key);
    Task<bool> PutDynamoDbItemAsync(string tableName, Dictionary<string, object> item);
    Task<Dictionary<string, object>?> GetDynamoDbItemAsync(string tableName, Dictionary<string, object> key);
    Task<List<Dictionary<string, object>>> QueryDynamoDbAsync(DynamoDbQuery query);
    Task<bool> DeleteDynamoDbItemAsync(string tableName, Dictionary<string, object> key);
}

public class AwsStorageService : IAwsStorageService
{
    private readonly ILogger<AwsStorageService> _logger;
    private readonly IAmazonS3? _s3Client;
    private readonly IAmazonDynamoDB? _dynamoDbClient;
    private readonly string? _bucketName;

    public AwsStorageService(ILogger<AwsStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;

        var s3Config = configuration.GetSection("AwsS3").Get<AwsS3Config>();
        if (s3Config != null)
        {
            _s3Client = new AmazonS3Client(
                s3Config.AccessKey,
                s3Config.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(s3Config.Region));
            _bucketName = s3Config.BucketName;
        }

        var dynamoConfig = configuration.GetSection("DynamoDB").Get<DynamoDbConfig>();
        if (dynamoConfig != null)
        {
            _dynamoDbClient = new AmazonDynamoDBClient(
                dynamoConfig.AccessKey,
                dynamoConfig.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(dynamoConfig.Region));
        }
    }

    public async Task<bool> UploadToS3Async(Stream data, S3UploadOptions options)
    {
        try
        {
            if (_s3Client == null || _bucketName == null)
            {
                throw new InvalidOperationException("S3 not configured");
            }

            if (options.UseMultipartUpload && data.Length > options.MultipartThresholdMB * 1024 * 1024)
            {
                var transferUtility = new TransferUtility(_s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = options.Key,
                    InputStream = data,
                    ContentType = options.ContentType,
                    ServerSideEncryptionMethod = options.ServerSideEncryption != null
                        ? ServerSideEncryptionMethod.FindValue(options.ServerSideEncryption)
                        : null
                };

                foreach (var metadata in options.Metadata)
                {
                    uploadRequest.Metadata.Add(metadata.Key, metadata.Value);
                }

                await transferUtility.UploadAsync(uploadRequest);
            }
            else
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = options.Key,
                    InputStream = data,
                    ContentType = options.ContentType,
                    ServerSideEncryptionMethod = options.ServerSideEncryption != null
                        ? ServerSideEncryptionMethod.FindValue(options.ServerSideEncryption)
                        : null
                };

                foreach (var metadata in options.Metadata)
                {
                    putRequest.Metadata.Add(metadata.Key, metadata.Value);
                }

                await _s3Client.PutObjectAsync(putRequest);
            }

            _logger.LogInformation("Uploaded object to S3 with key {Key}", options.Key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to S3 with key {Key}", options.Key);
            return false;
        }
    }

    public async Task<Stream?> DownloadFromS3Async(string key)
    {
        try
        {
            if (_s3Client == null || _bucketName == null)
            {
                throw new InvalidOperationException("S3 not configured");
            }

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);
            _logger.LogInformation("Downloaded object from S3 with key {Key}", key);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading from S3 with key {Key}", key);
            return null;
        }
    }

    public async Task<bool> DeleteFromS3Async(string key)
    {
        try
        {
            if (_s3Client == null || _bucketName == null)
            {
                throw new InvalidOperationException("S3 not configured");
            }

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
            _logger.LogInformation("Deleted object from S3 with key {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting from S3 with key {Key}", key);
            return false;
        }
    }

    public async Task<List<string>> ListS3ObjectsAsync(string? prefix = null)
    {
        var keys = new List<string>();

        try
        {
            if (_s3Client == null || _bucketName == null)
            {
                throw new InvalidOperationException("S3 not configured");
            }

            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                keys.AddRange(response.S3Objects.Select(obj => obj.Key));
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            _logger.LogInformation("Listed {Count} objects from S3 with prefix {Prefix}",
                keys.Count, prefix ?? "none");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing S3 objects with prefix {Prefix}", prefix);
        }

        return keys;
    }

    public async Task<bool> S3ObjectExistsAsync(string key)
    {
        try
        {
            if (_s3Client == null || _bucketName == null)
            {
                throw new InvalidOperationException("S3 not configured");
            }

            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking S3 object existence with key {Key}", key);
            return false;
        }
    }

    public async Task<bool> PutDynamoDbItemAsync(string tableName, Dictionary<string, object> item)
    {
        try
        {
            if (_dynamoDbClient == null)
            {
                throw new InvalidOperationException("DynamoDB not configured");
            }

            var attributeValues = new Dictionary<string, AttributeValue>();
            foreach (var kvp in item)
            {
                attributeValues[kvp.Key] = ConvertToAttributeValue(kvp.Value);
            }

            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = attributeValues
            };

            await _dynamoDbClient.PutItemAsync(request);
            _logger.LogInformation("Put item to DynamoDB table {TableName}", tableName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error putting item to DynamoDB table {TableName}", tableName);
            return false;
        }
    }

    public async Task<Dictionary<string, object>?> GetDynamoDbItemAsync(string tableName, Dictionary<string, object> key)
    {
        try
        {
            if (_dynamoDbClient == null)
            {
                throw new InvalidOperationException("DynamoDB not configured");
            }

            var keyAttributes = new Dictionary<string, AttributeValue>();
            foreach (var kvp in key)
            {
                keyAttributes[kvp.Key] = ConvertToAttributeValue(kvp.Value);
            }

            var request = new GetItemRequest
            {
                TableName = tableName,
                Key = keyAttributes
            };

            var response = await _dynamoDbClient.GetItemAsync(request);
            
            if (!response.IsItemSet)
            {
                return null;
            }

            var result = new Dictionary<string, object>();
            foreach (var kvp in response.Item)
            {
                result[kvp.Key] = ConvertFromAttributeValue(kvp.Value);
            }

            _logger.LogInformation("Retrieved item from DynamoDB table {TableName}", tableName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item from DynamoDB table {TableName}", tableName);
            return null;
        }
    }

    public async Task<List<Dictionary<string, object>>> QueryDynamoDbAsync(DynamoDbQuery query)
    {
        var results = new List<Dictionary<string, object>>();

        try
        {
            if (_dynamoDbClient == null)
            {
                throw new InvalidOperationException("DynamoDB not configured");
            }

            var request = new QueryRequest
            {
                TableName = query.TableName,
                ConsistentRead = query.ConsistentRead,
                Limit = query.Limit
            };

            if (query.ProjectionAttributes != null && query.ProjectionAttributes.Any())
            {
                request.ProjectionExpression = string.Join(", ", query.ProjectionAttributes);
            }

            var response = await _dynamoDbClient.QueryAsync(request);

            foreach (var item in response.Items)
            {
                var result = new Dictionary<string, object>();
                foreach (var kvp in item)
                {
                    result[kvp.Key] = ConvertFromAttributeValue(kvp.Value);
                }
                results.Add(result);
            }

            _logger.LogInformation("Queried DynamoDB table {TableName}, found {Count} items",
                query.TableName, results.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying DynamoDB table {TableName}", query.TableName);
        }

        return results;
    }

    public async Task<bool> DeleteDynamoDbItemAsync(string tableName, Dictionary<string, object> key)
    {
        try
        {
            if (_dynamoDbClient == null)
            {
                throw new InvalidOperationException("DynamoDB not configured");
            }

            var keyAttributes = new Dictionary<string, AttributeValue>();
            foreach (var kvp in key)
            {
                keyAttributes[kvp.Key] = ConvertToAttributeValue(kvp.Value);
            }

            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = keyAttributes
            };

            await _dynamoDbClient.DeleteItemAsync(request);
            _logger.LogInformation("Deleted item from DynamoDB table {TableName}", tableName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item from DynamoDB table {TableName}", tableName);
            return false;
        }
    }

    private AttributeValue ConvertToAttributeValue(object value)
    {
        return value switch
        {
            string s => new AttributeValue { S = s },
            int i => new AttributeValue { N = i.ToString() },
            long l => new AttributeValue { N = l.ToString() },
            double d => new AttributeValue { N = d.ToString() },
            bool b => new AttributeValue { BOOL = b },
            _ => new AttributeValue { S = value.ToString() }
        };
    }

    private object ConvertFromAttributeValue(AttributeValue value)
    {
        if (value.S != null) return value.S;
        if (value.N != null) return double.Parse(value.N);
        if (value.BOOL) return value.BOOL;
        if (value.NULL) return null!;
        return value.ToString() ?? string.Empty;
    }
}
