using AI.ResponseProcessing.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using CsvHelper;
using System.Globalization;

namespace AI.ResponseProcessing.Services;

#region Validation Service

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync(ValidationRequest request);
    Task<ValidationResult> ValidateJsonSchemaAsync(string json, string schema);
    Task<bool> ValidateFieldAsync(string fieldPath, object value, ValidationRule rule);
    Task<SanitizedResponse> SanitizeAsync(SanitizationRequest request);
}

public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationRequest request)
    {
        var result = new ValidationResult
        {
            IsValid = true,
            ValidatedData = new Dictionary<string, object>()
        };

        foreach (var rule in request.Rules)
        {
            var isValid = await ValidateFieldAsync(rule.FieldPath, 
                GetValueByPath(request.Data, rule.FieldPath), rule);

            if (!isValid)
            {
                var error = new ValidationError
                {
                    Field = rule.FieldPath,
                    Message = rule.ErrorMessage ?? $"Validation failed for {rule.FieldPath}",
                    Severity = rule.Severity,
                    Code = rule.RuleType
                };

                if (rule.Severity == ValidationSeverity.Error)
                {
                    result.Errors.Add(error);
                    result.IsValid = false;

                    if (request.StopOnFirstError)
                        break;
                }
                else
                {
                    result.Warnings.Add(error);
                }
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateJsonSchemaAsync(string json, string schema)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            var jObject = JObject.Parse(json);
            var jSchema = JObject.Parse(schema);

            // Basic schema validation (simplified)
            var requiredFields = jSchema["required"]?.ToObject<List<string>>() ?? new List<string>();
            
            foreach (var field in requiredFields)
            {
                if (jObject[field] == null)
                {
                    result.IsValid = false;
                    result.Errors.Add(new ValidationError
                    {
                        Field = field,
                        Message = $"Required field '{field}' is missing",
                        Severity = ValidationSeverity.Error,
                        Code = "REQUIRED_FIELD_MISSING"
                    });
                }
            }

            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JSON schema validation failed");
            result.IsValid = false;
            result.Errors.Add(new ValidationError
            {
                Message = ex.Message,
                Severity = ValidationSeverity.Error,
                Code = "SCHEMA_VALIDATION_ERROR"
            });
            return result;
        }
    }

    public async Task<bool> ValidateFieldAsync(string fieldPath, object value, ValidationRule rule)
    {
        return await Task.FromResult(rule.RuleType switch
        {
            "Required" => value != null && !string.IsNullOrWhiteSpace(value.ToString()),
            "MinLength" => ValidateMinLength(value, rule.Value),
            "MaxLength" => ValidateMaxLength(value, rule.Value),
            "Pattern" => ValidatePattern(value, rule.Value),
            "Range" => ValidateRange(value, rule.Value),
            "Email" => ValidateEmail(value),
            "Url" => ValidateUrl(value),
            "Custom" => true, // Custom validation would be implemented by caller
            _ => true
        });
    }

    public async Task<SanitizedResponse> SanitizeAsync(SanitizationRequest request)
    {
        var cleanContent = request.Content;
        var removedElements = new List<string>();
        var wasModified = false;

        if (request.Options.RemoveHtmlTags)
        {
            var htmlPattern = @"<[^>]*>";
            var matches = Regex.Matches(cleanContent, htmlPattern);
            if (matches.Count > 0)
            {
                removedElements.AddRange(matches.Select(m => m.Value));
                cleanContent = Regex.Replace(cleanContent, htmlPattern, "");
                wasModified = true;
            }
        }

        if (request.Options.RemoveScripts)
        {
            var scriptPattern = @"<script[\s\S]*?</script>";
            if (Regex.IsMatch(cleanContent, scriptPattern, RegexOptions.IgnoreCase))
            {
                removedElements.Add("script tags");
                cleanContent = Regex.Replace(cleanContent, scriptPattern, "", RegexOptions.IgnoreCase);
                wasModified = true;
            }
        }

        if (request.Options.RemoveSqlInjection)
        {
            var sqlPatterns = new[]
            {
                @"\b(DROP|DELETE|INSERT|UPDATE|SELECT)\b.*\b(TABLE|DATABASE|FROM|WHERE)\b",
                @"--",
                @"';",
                @"OR\s+1\s*=\s*1"
            };

            foreach (var pattern in sqlPatterns)
            {
                if (Regex.IsMatch(cleanContent, pattern, RegexOptions.IgnoreCase))
                {
                    removedElements.Add($"SQL pattern: {pattern}");
                    cleanContent = Regex.Replace(cleanContent, pattern, "", RegexOptions.IgnoreCase);
                    wasModified = true;
                }
            }
        }

        if (request.Options.RemoveXss)
        {
            var xssPatterns = new[]
            {
                @"javascript:",
                @"onerror\s*=",
                @"onclick\s*=",
                @"onload\s*="
            };

            foreach (var pattern in xssPatterns)
            {
                if (Regex.IsMatch(cleanContent, pattern, RegexOptions.IgnoreCase))
                {
                    removedElements.Add($"XSS pattern: {pattern}");
                    cleanContent = Regex.Replace(cleanContent, pattern, "", RegexOptions.IgnoreCase);
                    wasModified = true;
                }
            }
        }

        if (request.Options.TrimWhitespace)
        {
            cleanContent = cleanContent.Trim();
        }

        return await Task.FromResult(new SanitizedResponse
        {
            CleanContent = cleanContent,
            RemovedElements = removedElements,
            WasModified = wasModified
        });
    }

    #region Private Validation Methods

    private object? GetValueByPath(object data, string path)
    {
        try
        {
            var parts = path.Split('.');
            object? current = data;

            foreach (var part in parts)
            {
                if (current == null) return null;

                var type = current.GetType();
                var property = type.GetProperty(part);
                
                if (property != null)
                {
                    current = property.GetValue(current);
                }
                else if (current is JObject jObj)
                {
                    current = jObj[part];
                }
                else
                {
                    return null;
                }
            }

            return current;
        }
        catch
        {
            return null;
        }
    }

    private bool ValidateMinLength(object? value, object? minLength)
    {
        if (value == null || minLength == null) return false;
        var length = value.ToString()?.Length ?? 0;
        return length >= Convert.ToInt32(minLength);
    }

    private bool ValidateMaxLength(object? value, object? maxLength)
    {
        if (value == null || maxLength == null) return true;
        var length = value.ToString()?.Length ?? 0;
        return length <= Convert.ToInt32(maxLength);
    }

    private bool ValidatePattern(object? value, object? pattern)
    {
        if (value == null || pattern == null) return false;
        return Regex.IsMatch(value.ToString() ?? "", pattern.ToString() ?? "");
    }

    private bool ValidateRange(object? value, object? range)
    {
        if (value == null || range == null) return false;
        
        try
        {
            var numValue = Convert.ToDouble(value);
            var rangeStr = range.ToString()?.Split(',');
            if (rangeStr?.Length == 2)
            {
                var min = Convert.ToDouble(rangeStr[0]);
                var max = Convert.ToDouble(rangeStr[1]);
                return numValue >= min && numValue <= max;
            }
        }
        catch { }

        return false;
    }

    private bool ValidateEmail(object? value)
    {
        if (value == null) return false;
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(value.ToString() ?? "", emailPattern);
    }

    private bool ValidateUrl(object? value)
    {
        if (value == null) return false;
        return Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    #endregion
}

#endregion

#region Output Formatting Service

public interface IOutputFormattingService
{
    Task<FormattedResponse> FormatAsync(FormattingRequest request);
    Task<string> FormatAsJsonAsync(object data, FormattingOptions? options = null);
    Task<string> FormatAsXmlAsync(object data, FormattingOptions? options = null);
    Task<string> FormatAsYamlAsync(object data, FormattingOptions? options = null);
    Task<string> FormatAsCsvAsync<T>(List<T> data, FormattingOptions? options = null);
    Task<string> FormatAsMarkdownTableAsync<T>(List<T> data);
}

public class OutputFormattingService : IOutputFormattingService
{
    private readonly ILogger<OutputFormattingService> _logger;
    private readonly ISerializer _yamlSerializer;

    public OutputFormattingService(ILogger<OutputFormattingService> logger)
    {
        _logger = logger;
        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public async Task<FormattedResponse> FormatAsync(FormattingRequest request)
    {
        try
        {
            var content = request.TargetFormat switch
            {
                ResponseFormat.Json => await FormatAsJsonAsync(request.Data, request.Options),
                ResponseFormat.Xml => await FormatAsXmlAsync(request.Data, request.Options),
                ResponseFormat.Yaml => await FormatAsYamlAsync(request.Data, request.Options),
                ResponseFormat.Markdown => await FormatAsMarkdownAsync(request.Data),
                ResponseFormat.PlainText => request.Data.ToString() ?? "",
                _ => JsonConvert.SerializeObject(request.Data)
            };

            return new FormattedResponse
            {
                Content = content,
                Format = request.TargetFormat,
                ContentLength = content.Length,
                ContentType = GetContentType(request.TargetFormat),
                Headers = GetHeaders(request.TargetFormat)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting response to {Format}", request.TargetFormat);
            throw;
        }
    }

    public async Task<string> FormatAsJsonAsync(object data, FormattingOptions? options = null)
    {
        options ??= new FormattingOptions();

        var settings = new JsonSerializerSettings
        {
            Formatting = options.PrettyPrint ? Formatting.Indented : Formatting.None,
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = options.DateFormat ?? "yyyy-MM-dd HH:mm:ss"
        };

        return await Task.FromResult(JsonConvert.SerializeObject(data, settings));
    }

    public async Task<string> FormatAsXmlAsync(object data, FormattingOptions? options = null)
    {
        options ??= new FormattingOptions();

        try
        {
            var json = JsonConvert.SerializeObject(data);
            var doc = JsonConvert.DeserializeXNode(json, "root");
            
            return await Task.FromResult(options.PrettyPrint 
                ? doc?.ToString() ?? "" 
                : doc?.ToString(SaveOptions.DisableFormatting) ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting as XML");
            throw;
        }
    }

    public async Task<string> FormatAsYamlAsync(object data, FormattingOptions? options = null)
    {
        try
        {
            return await Task.FromResult(_yamlSerializer.Serialize(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting as YAML");
            throw;
        }
    }

    public async Task<string> FormatAsCsvAsync<T>(List<T> data, FormattingOptions? options = null)
    {
        try
        {
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(data);
            return writer.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting as CSV");
            throw;
        }
    }

    public async Task<string> FormatAsMarkdownTableAsync<T>(List<T> data)
    {
        if (!data.Any())
            return "| No Data |\n|---------|";

        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        // Headers
        sb.Append("| ");
        sb.AppendJoin(" | ", properties.Select(p => p.Name));
        sb.AppendLine(" |");

        // Separator
        sb.Append("| ");
        sb.AppendJoin(" | ", properties.Select(_ => "---"));
        sb.AppendLine(" |");

        // Rows
        foreach (var item in data)
        {
            sb.Append("| ");
            sb.AppendJoin(" | ", properties.Select(p => p.GetValue(item)?.ToString() ?? ""));
            sb.AppendLine(" |");
        }

        return await Task.FromResult(sb.ToString());
    }

    #region Private Helper Methods

    private async Task<string> FormatAsMarkdownAsync(object data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        return await Task.FromResult($"```json\n{json}\n```");
    }

    private string GetContentType(ResponseFormat format)
    {
        return format switch
        {
            ResponseFormat.Json => "application/json",
            ResponseFormat.Xml => "application/xml",
            ResponseFormat.Yaml => "application/x-yaml",
            ResponseFormat.Csv => "text/csv",
            ResponseFormat.Html => "text/html",
            ResponseFormat.Markdown => "text/markdown",
            _ => "text/plain"
        };
    }

    private Dictionary<string, string> GetHeaders(ResponseFormat format)
    {
        return new Dictionary<string, string>
        {
            { "Content-Type", GetContentType(format) },
            { "X-Format", format.ToString() }
        };
    }

    #endregion
}

#endregion
