using AI.DataSources.Models;
using RestSharp;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Diagnostics;

namespace AI.DataSources.Services;

public interface IApiClientService
{
    Task<ApiResponse> SendRequestAsync(ApiConnectionConfig config, ApiRequest request);
    Task<GraphQLResponse> SendGraphQLRequestAsync(string endpoint, GraphQLRequest request);
}

public class ApiClientService : IApiClientService
{
    private readonly ILogger<ApiClientService> _logger;

    public ApiClientService(ILogger<ApiClientService> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponse> SendRequestAsync(ApiConnectionConfig config, ApiRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ApiResponse();

        try
        {
            var options = new RestClientOptions(config.BaseUrl)
            {
                MaxTimeout = config.Timeout * 1000
            };

            var client = new RestClient(options);
            var restRequest = new RestRequest(request.Endpoint);

            // Set HTTP method
            restRequest.Method = request.Method.ToUpperInvariant() switch
            {
                "GET" => Method.Get,
                "POST" => Method.Post,
                "PUT" => Method.Put,
                "DELETE" => Method.Delete,
                "PATCH" => Method.Patch,
                _ => Method.Get
            };

            // Add headers
            foreach (var header in config.Headers)
            {
                restRequest.AddHeader(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                restRequest.AddHeader("Authorization", $"Bearer {config.ApiKey}");
            }

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    restRequest.AddHeader(header.Key, header.Value);
                }
            }

            // Add query parameters
            if (request.QueryParameters != null)
            {
                foreach (var param in request.QueryParameters)
                {
                    restRequest.AddQueryParameter(param.Key, param.Value);
                }
            }

            // Add body
            if (request.Body != null)
            {
                restRequest.AddJsonBody(request.Body);
            }

            var response = await client.ExecuteAsync(restRequest);

            result.IsSuccessful = response.IsSuccessful;
            result.StatusCode = (int)response.StatusCode;
            result.Content = response.Content ?? string.Empty;
            result.Headers = response.Headers?
                .ToDictionary(h => h.Name ?? "", h => h.Value?.ToString() ?? "") ?? new();

            if (!response.IsSuccessful)
            {
                result.ErrorMessage = response.ErrorMessage ?? $"Request failed with status {response.StatusCode}";
            }

            _logger.LogInformation("API request: {Method} {Endpoint}, Status: {Status}",
                request.Method, request.Endpoint, result.StatusCode);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error sending API request");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<GraphQLResponse> SendGraphQLRequestAsync(string endpoint, GraphQLRequest request)
    {
        var result = new GraphQLResponse();

        try
        {
            using var graphQLClient = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer());

            var graphQLRequest = new GraphQL.GraphQLRequest
            {
                Query = request.Query,
                Variables = request.Variables,
                OperationName = request.OperationName
            };

            var response = await graphQLClient.SendQueryAsync<object>(graphQLRequest);

            result.IsSuccessful = response.Errors == null || !response.Errors.Any();
            result.Data = response.Data;

            if (response.Errors != null)
            {
                result.Errors = response.Errors.Select(e => new GraphQLError
                {
                    Message = e.Message,
                    Locations = e.Locations?.Select(l => new GraphQLLocation
                    {
                        Line = l.Line,
                        Column = l.Column
                    }).ToList() ?? new(),
                    Path = e.Path?.ToList<object>() ?? new()
                }).ToList();
            }

            _logger.LogInformation("GraphQL request completed: Success={Success}", result.IsSuccessful);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Errors.Add(new GraphQLError { Message = ex.Message });
            _logger.LogError(ex, "Error sending GraphQL request");
        }

        return result;
    }
}

public interface IFileSystemService
{
    Task<FileReadResult> ReadFileAsync(FileReadRequest request);
    Task<FileWriteResult> WriteFileAsync(FileWriteRequest request);
    Task<List<string>> ListFilesAsync(string directory, string pattern = "*");
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
}

public class FileSystemService : IFileSystemService
{
    private readonly ILogger<FileSystemService> _logger;

    public FileSystemService(ILogger<FileSystemService> logger)
    {
        _logger = logger;
    }

    public async Task<FileReadResult> ReadFileAsync(FileReadRequest request)
    {
        var result = new FileReadResult();

        try
        {
            if (!File.Exists(request.FilePath))
            {
                result.IsSuccessful = false;
                return result;
            }

            var fileInfo = new FileInfo(request.FilePath);
            result.Size = fileInfo.Length;
            result.ModifiedAt = fileInfo.LastWriteTimeUtc;

            if (request.ReadAsBinary)
            {
                result.BinaryContent = await File.ReadAllBytesAsync(request.FilePath);
            }
            else
            {
                var encoding = System.Text.Encoding.GetEncoding(request.Encoding);
                result.TextContent = await File.ReadAllTextAsync(request.FilePath, encoding);
            }

            result.IsSuccessful = true;

            _logger.LogInformation("File read: {FilePath}, Size: {Size} bytes", request.FilePath, result.Size);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error reading file: {FilePath}", request.FilePath);
            throw;
        }

        return result;
    }

    public async Task<FileWriteResult> WriteFileAsync(FileWriteRequest request)
    {
        var result = new FileWriteResult();

        try
        {
            var directory = Path.GetDirectoryName(request.FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (request.BinaryContent != null)
            {
                await File.WriteAllBytesAsync(request.FilePath, request.BinaryContent);
                result.BytesWritten = request.BinaryContent.Length;
            }
            else if (request.TextContent != null)
            {
                var encoding = System.Text.Encoding.GetEncoding(request.Encoding);
                await File.WriteAllTextAsync(request.FilePath, request.TextContent, encoding);
                result.BytesWritten = encoding.GetByteCount(request.TextContent);
            }

            result.IsSuccessful = true;
            result.FilePath = request.FilePath;

            _logger.LogInformation("File written: {FilePath}, Size: {Size} bytes", request.FilePath, result.BytesWritten);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            _logger.LogError(ex, "Error writing file: {FilePath}", request.FilePath);
            throw;
        }

        return result;
    }

    public async Task<List<string>> ListFilesAsync(string directory, string pattern = "*")
    {
        await Task.CompletedTask;

        try
        {
            if (!Directory.Exists(directory))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly).ToList();

            _logger.LogInformation("Listed {Count} files in: {Directory}", files.Count, directory);

            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in: {Directory}", directory);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        await Task.CompletedTask;

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        await Task.CompletedTask;
        return File.Exists(filePath);
    }
}
