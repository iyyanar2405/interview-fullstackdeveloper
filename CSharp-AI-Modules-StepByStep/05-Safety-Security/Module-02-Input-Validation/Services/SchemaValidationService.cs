using Module_02_Input_Validation.Models;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System.Text.Json;

namespace Module_02_Input_Validation.Services;

public interface ISchemaValidationService
{
    Task<SchemaValidationResult> ValidateJsonSchemaAsync(string jsonData, string schemaJson);
    Task<SchemaValidationResult> ValidateAgainstSchemaAsync<T>(T data, string schemaJson);
    Task<string> GenerateSchemaAsync<T>();
    SchemaValidationResult ValidateJsonSchema(string jsonData, string schemaJson);
    bool IsValidJson(string jsonString);
}

public class SchemaValidationService : ISchemaValidationService
{
    private readonly ILogger<SchemaValidationService> _logger;

    public SchemaValidationService(ILogger<SchemaValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<SchemaValidationResult> ValidateJsonSchemaAsync(string jsonData, string schemaJson)
    {
        return await Task.Run(() => ValidateJsonSchema(jsonData, schemaJson));
    }

    public SchemaValidationResult ValidateJsonSchema(string jsonData, string schemaJson)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return new SchemaValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "JSON data is empty" }
                };
            }

            if (string.IsNullOrWhiteSpace(schemaJson))
            {
                return new SchemaValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "Schema is empty" }
                };
            }

            // Parse JSON data
            JToken jsonToken;
            try
            {
                jsonToken = JToken.Parse(jsonData);
            }
            catch (Exception ex)
            {
                return new SchemaValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Invalid JSON data: {ex.Message}" }
                };
            }

            // Parse schema
            JSchema schema;
            try
            {
                schema = JSchema.Parse(schemaJson);
            }
            catch (Exception ex)
            {
                return new SchemaValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Invalid JSON schema: {ex.Message}" }
                };
            }

            // Validate
            var errors = new List<string>();
            var warnings = new List<string>();
            var isValid = jsonToken.IsValid(schema, out IList<string> errorMessages);

            if (!isValid)
            {
                errors.AddRange(errorMessages);
            }

            _logger.LogInformation("Schema validation {Result}. Errors: {ErrorCount}",
                isValid ? "passed" : "failed", errors.Count);

            return new SchemaValidationResult
            {
                IsValid = isValid,
                Errors = errors,
                Warnings = warnings,
                SchemaVersion = schema.SchemaVersion?.ToString() ?? "draft-07"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during schema validation");
            return new SchemaValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Validation error: {ex.Message}" }
            };
        }
    }

    public async Task<SchemaValidationResult> ValidateAgainstSchemaAsync<T>(T data, string schemaJson)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            return await ValidateJsonSchemaAsync(jsonData, schemaJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating object against schema");
            return new SchemaValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Serialization error: {ex.Message}" }
            };
        }
    }

    public async Task<string> GenerateSchemaAsync<T>()
    {
        try
        {
            var schema = await NJsonSchema.JsonSchema.FromTypeAsync<T>();
            return schema.ToJson();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating schema for type {Type}", typeof(T).Name);
            throw;
        }
    }

    public bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return false;

        try
        {
            JToken.Parse(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
