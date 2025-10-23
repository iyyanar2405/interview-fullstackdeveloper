using AI.ResponseProcessing.Models;
using AI.ResponseProcessing.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace AI.ResponseProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponseProcessingController : ControllerBase
{
    private readonly IResponseParserService _parser;
    private readonly IValidationService _validation;
    private readonly IOutputFormattingService _formatting;
    private readonly IErrorHandlingService _errorHandling;
    private readonly IRateLimitingService _rateLimiting;
    private readonly ILogger<ResponseProcessingController> _logger;

    public ResponseProcessingController(
        IResponseParserService parser,
        IValidationService validation,
        IOutputFormattingService formatting,
        IErrorHandlingService errorHandling,
        IRateLimitingService rateLimiting,
        ILogger<ResponseProcessingController> logger)
    {
        _parser = parser;
        _validation = validation;
        _formatting = formatting;
        _errorHandling = errorHandling;
        _rateLimiting = rateLimiting;
        _logger = logger;
    }

    #region Parsing Endpoints

    /// <summary>
    /// Parse JSON content
    /// </summary>
    [HttpPost("parse/json")]
    [ProducesResponseType(typeof(ParsedResponse<JObject>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ParseJson([FromBody] ParsingRequest request)
    {
        var result = await _parser.ParseJsonAsync(request.Content, request.Options);
        return Ok(result);
    }

    /// <summary>
    /// Parse XML content
    /// </summary>
    [HttpPost("parse/xml")]
    [ProducesResponseType(typeof(ParsedResponse<XDocument>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ParseXml([FromBody] ParsingRequest request)
    {
        var result = await _parser.ParseXmlAsync(request.Content, request.Options);
        return Ok(result);
    }

    /// <summary>
    /// Parse Markdown content
    /// </summary>
    [HttpPost("parse/markdown")]
    [ProducesResponseType(typeof(ParsedResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ParseMarkdown([FromBody] ParsingRequest request)
    {
        var result = await _parser.ParseMarkdownAsync(request.Content, request.Options);
        return Ok(result);
    }

    /// <summary>
    /// Extract code blocks from content
    /// </summary>
    [HttpPost("parse/code-blocks")]
    [ProducesResponseType(typeof(List<CodeBlock>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExtractCodeBlocks([FromBody] string content)
    {
        var codeBlocks = await _parser.ExtractCodeBlocksAsync(content);
        return Ok(codeBlocks);
    }

    /// <summary>
    /// Extract links from content
    /// </summary>
    [HttpPost("parse/links")]
    [ProducesResponseType(typeof(List<ExtractedLink>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExtractLinks([FromBody] string content)
    {
        var links = await _parser.ExtractLinksAsync(content);
        return Ok(links);
    }

    /// <summary>
    /// Extract tables from markdown
    /// </summary>
    [HttpPost("parse/tables")]
    [ProducesResponseType(typeof(List<ExtractedTable>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExtractTables([FromBody] string markdown)
    {
        var tables = await _parser.ExtractTablesAsync(markdown);
        return Ok(tables);
    }

    /// <summary>
    /// Extract structured data
    /// </summary>
    [HttpPost("parse/structured")]
    [ProducesResponseType(typeof(StructuredData), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExtractStructuredData([FromBody] ParsingRequest request)
    {
        var data = await _parser.ExtractStructuredDataAsync(request.Content, request.Format);
        return Ok(data);
    }

    #endregion

    #region Validation Endpoints

    /// <summary>
    /// Validate data against rules
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate([FromBody] ValidationRequest request)
    {
        var result = await _validation.ValidateAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Validate JSON against schema
    /// </summary>
    [HttpPost("validate/json-schema")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateJsonSchema(
        [FromBody] JsonSchemaValidationRequest request)
    {
        var result = await _validation.ValidateJsonSchemaAsync(request.Json, request.Schema);
        return Ok(result);
    }

    /// <summary>
    /// Sanitize content
    /// </summary>
    [HttpPost("validate/sanitize")]
    [ProducesResponseType(typeof(SanitizedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Sanitize([FromBody] SanitizationRequest request)
    {
        var result = await _validation.SanitizeAsync(request);
        return Ok(result);
    }

    #endregion

    #region Formatting Endpoints

    /// <summary>
    /// Format data to specified format
    /// </summary>
    [HttpPost("format")]
    [ProducesResponseType(typeof(FormattedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Format([FromBody] FormattingRequest request)
    {
        var result = await _formatting.FormatAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Format as JSON
    /// </summary>
    [HttpPost("format/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> FormatAsJson(
        [FromBody] object data,
        [FromQuery] bool prettyPrint = true)
    {
        var options = new FormattingOptions { PrettyPrint = prettyPrint };
        var result = await _formatting.FormatAsJsonAsync(data, options);
        return Content(result, "application/json");
    }

    /// <summary>
    /// Format as XML
    /// </summary>
    [HttpPost("format/xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> FormatAsXml(
        [FromBody] object data,
        [FromQuery] bool prettyPrint = true)
    {
        var options = new FormattingOptions { PrettyPrint = prettyPrint };
        var result = await _formatting.FormatAsXmlAsync(data, options);
        return Content(result, "application/xml");
    }

    /// <summary>
    /// Format as YAML
    /// </summary>
    [HttpPost("format/yaml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> FormatAsYaml([FromBody] object data)
    {
        var result = await _formatting.FormatAsYamlAsync(data);
        return Content(result, "application/x-yaml");
    }

    /// <summary>
    /// Format as Markdown table
    /// </summary>
    [HttpPost("format/markdown-table")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> FormatAsMarkdownTable<T>([FromBody] List<T> data)
    {
        var result = await _formatting.FormatAsMarkdownTableAsync(data);
        return Content(result, "text/markdown");
    }

    #endregion

    #region Error Handling Endpoints

    /// <summary>
    /// Get recent errors
    /// </summary>
    [HttpGet("errors/recent")]
    [ProducesResponseType(typeof(List<ErrorContext>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentErrors([FromQuery] int count = 10)
    {
        var errors = await _errorHandling.GetRecentErrorsAsync(count);
        return Ok(errors);
    }

    /// <summary>
    /// Test error handling with retry
    /// </summary>
    [HttpPost("errors/test-retry")]
    [ProducesResponseType(typeof(RetryResult<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestRetry([FromQuery] int failureCount = 2)
    {
        var attempts = 0;
        var policy = new RetryPolicy
        {
            MaxAttempts = 5,
            InitialDelay = TimeSpan.FromMilliseconds(100),
            BackoffMultiplier = 2.0
        };

        var result = await _errorHandling.ExecuteWithRetryAsync(async () =>
        {
            attempts++;
            if (attempts <= failureCount)
            {
                throw new Exception($"Simulated failure {attempts}");
            }
            return await Task.FromResult($"Success after {attempts} attempts");
        }, policy);

        return Ok(result);
    }

    #endregion

    #region Rate Limiting Endpoints

    /// <summary>
    /// Check rate limit
    /// </summary>
    [HttpPost("ratelimit/check")]
    [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRateLimit([FromBody] RateLimitRequest request)
    {
        var result = await _rateLimiting.CheckRateLimitAsync(request);
        
        Response.Headers.Add("X-RateLimit-Limit", result.RemainingRequests.ToString());
        Response.Headers.Add("X-RateLimit-Remaining", result.RemainingRequests.ToString());
        Response.Headers.Add("X-RateLimit-Reset", result.ResetTime.ToString("o"));

        if (!result.Allowed)
        {
            Response.Headers.Add("Retry-After", ((int)result.RetryAfter.TotalSeconds).ToString());
            return StatusCode(429, new { message = "Rate limit exceeded", result });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get rate limit info
    /// </summary>
    [HttpGet("ratelimit/info/{key}")]
    [ProducesResponseType(typeof(RateLimitInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRateLimitInfo(string key)
    {
        var info = await _rateLimiting.GetRateLimitInfoAsync(key);
        return Ok(info);
    }

    /// <summary>
    /// Reset rate limit for a key
    /// </summary>
    [HttpPost("ratelimit/reset/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetRateLimit(string key)
    {
        await _rateLimiting.ResetRateLimitAsync(key);
        return Ok(new { message = $"Rate limit reset for key: {key}" });
    }

    /// <summary>
    /// Get all rate limits
    /// </summary>
    [HttpGet("ratelimit/all")]
    [ProducesResponseType(typeof(Dictionary<string, RateLimitInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRateLimits()
    {
        var limits = await _rateLimiting.GetAllRateLimitsAsync();
        return Ok(limits);
    }

    #endregion

    #region Batch Processing Endpoints

    /// <summary>
    /// Parse multiple responses in batch
    /// </summary>
    [HttpPost("batch/parse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BatchParse([FromBody] List<ParsingRequest> requests)
    {
        var results = new List<object>();

        foreach (var request in requests)
        {
            try
            {
                var result = request.Format switch
                {
                    ResponseFormat.Json => await _parser.ParseJsonAsync(request.Content, request.Options),
                    ResponseFormat.Xml => await _parser.ParseXmlAsync(request.Content, request.Options),
                    ResponseFormat.Markdown => await _parser.ParseMarkdownAsync(request.Content, request.Options),
                    _ => null
                };

                results.Add(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                results.Add(new { success = false, error = ex.Message });
            }
        }

        return Ok(results);
    }

    /// <summary>
    /// Validate multiple items in batch
    /// </summary>
    [HttpPost("batch/validate")]
    [ProducesResponseType(typeof(List<ValidationResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BatchValidate([FromBody] List<ValidationRequest> requests)
    {
        var results = new List<ValidationResult>();

        foreach (var request in requests)
        {
            var result = await _validation.ValidateAsync(request);
            results.Add(result);
        }

        return Ok(results);
    }

    #endregion

    #region Health & Utility Endpoints

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            services = new
            {
                parser = "operational",
                validation = "operational",
                formatting = "operational",
                errorHandling = "operational",
                rateLimiting = "operational"
            }
        });
    }

    /// <summary>
    /// Get service statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var errors = await _errorHandling.GetRecentErrorsAsync(100);
        
        return Ok(new
        {
            totalErrors = errors.Count,
            errorsByType = errors.GroupBy(e => e.Exception.GetType().Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            recentErrors = errors.Take(5).Select(e => new
            {
                e.Operation,
                e.Timestamp,
                error = e.Exception.Message
            })
        });
    }

    #endregion
}

#region Helper Models

public class JsonSchemaValidationRequest
{
    public string Json { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
}

#endregion
