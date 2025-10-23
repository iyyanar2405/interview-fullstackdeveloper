using AI.DataFormats.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AI.DataFormats.Services;

public interface IJsonService
{
    Task<DataFormatResponse<JToken>> ParseJsonAsync(string json, JsonProcessingOptions? options = null);
    Task<DataFormatResponse<string>> SerializeToJsonAsync(object data, JsonProcessingOptions? options = null);
    Task<JsonValidationResult> ValidateJsonAsync(string json, string schemaJson);
    Task<DataFormatResponse<JToken>> ReadJsonFileAsync(string filePath);
    Task<DataFormatResponse<bool>> WriteJsonFileAsync(string filePath, object data, JsonProcessingOptions? options = null);
    Task<DataFormatResponse<T>> DeserializeAsync<T>(string json);
}

public class JsonService : IJsonService
{
    private readonly ILogger<JsonService> _logger;

    public JsonService(ILogger<JsonService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<JToken>> ParseJsonAsync(string json, JsonProcessingOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<JToken>();

        try
        {
            options ??= new JsonProcessingOptions();

            // Parse JSON
            var token = JToken.Parse(json);

            // Validate schema if required
            if (options.ValidateSchema && !string.IsNullOrEmpty(options.SchemaPath))
            {
                var schemaJson = await File.ReadAllTextAsync(options.SchemaPath);
                var validationResult = await ValidateJsonAsync(json, schemaJson);

                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Error = string.Join("; ", validationResult.Errors);
                    return response;
                }
            }

            response.Success = true;
            response.Data = token;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(json);

            // Count records
            if (token is JArray array)
            {
                response.RecordCount = array.Count;
            }
            else if (token is JObject)
            {
                response.RecordCount = 1;
            }

            _logger.LogInformation("Parsed JSON successfully: {Size} bytes, {Records} records",
                response.SizeBytes, response.RecordCount);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error parsing JSON");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<string>> SerializeToJsonAsync(object data, JsonProcessingOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<string>();

        try
        {
            options ??= new JsonProcessingOptions();

            var settings = new JsonSerializerSettings
            {
                Formatting = options.Pretty ? Formatting.Indented : Formatting.None,
                NullValueHandling = options.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
                MaxDepth = options.MaxDepth
            };

            if (options.CamelCase)
            {
                settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            }

            var json = JsonConvert.SerializeObject(data, settings);

            response.Success = true;
            response.Data = json;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(json);

            _logger.LogInformation("Serialized to JSON: {Size} bytes", response.SizeBytes);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error serializing to JSON");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<JsonValidationResult> ValidateJsonAsync(string json, string schemaJson)
    {
        await Task.CompletedTask;
        var result = new JsonValidationResult();

        try
        {
            var schema = JSchema.Parse(schemaJson);
            var token = JToken.Parse(json);

            var isValid = token.IsValid(schema, out IList<string> errors);

            result.IsValid = isValid;
            result.Errors = errors.ToList();

            _logger.LogInformation("JSON validation: Valid={IsValid}, Errors={ErrorCount}",
                isValid, errors.Count);
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add(ex.Message);
            _logger.LogError(ex, "Error validating JSON");
        }

        return result;
    }

    public async Task<DataFormatResponse<JToken>> ReadJsonFileAsync(string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<JToken>();

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var parseResult = await ParseJsonAsync(json);

            response.Success = parseResult.Success;
            response.Data = parseResult.Data;
            response.Error = parseResult.Error;
            response.RecordCount = parseResult.RecordCount;
            response.SizeBytes = parseResult.SizeBytes;

            response.Metadata["FilePath"] = filePath;
            response.Metadata["FileName"] = Path.GetFileName(filePath);

            _logger.LogInformation("Read JSON file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading JSON file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteJsonFileAsync(
        string filePath,
        object data,
        JsonProcessingOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<bool>();

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializeResult = await SerializeToJsonAsync(data, options);

            if (!serializeResult.Success)
            {
                response.Success = false;
                response.Error = serializeResult.Error;
                return response;
            }

            await File.WriteAllTextAsync(filePath, serializeResult.Data);

            response.Success = true;
            response.Data = true;
            response.SizeBytes = serializeResult.SizeBytes;

            _logger.LogInformation("Wrote JSON file: {FilePath}, {Size} bytes", filePath, response.SizeBytes);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing JSON file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<T>> DeserializeAsync<T>(string json)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<T>();

        try
        {
            var data = JsonConvert.DeserializeObject<T>(json);

            response.Success = true;
            response.Data = data;

            _logger.LogInformation("Deserialized JSON to {Type}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error deserializing JSON to {Type}", typeof(T).Name);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }
}
