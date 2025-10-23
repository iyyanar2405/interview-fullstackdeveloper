using AI.AzureOpenAI.Models;
using AI.AzureOpenAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AI.AzureOpenAI.Controllers;

/// <summary>
/// Azure OpenAI API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AzureOpenAIController : ControllerBase
{
    private readonly IAzureOpenAIService _azureOpenAIService;
    private readonly ITokenService _tokenService;
    private readonly IContentFilterService _contentFilterService;
    private readonly ILogger<AzureOpenAIController> _logger;

    public AzureOpenAIController(
        IAzureOpenAIService azureOpenAIService,
        ITokenService tokenService,
        IContentFilterService contentFilterService,
        ILogger<AzureOpenAIController> logger)
    {
        _azureOpenAIService = azureOpenAIService;
        _tokenService = tokenService;
        _contentFilterService = contentFilterService;
        _logger = logger;
    }

    /// <summary>
    /// Send a chat completion request to Azure OpenAI
    /// </summary>
    /// <param name="request">Chat completion request</param>
    /// <returns>Chat completion response</returns>
    [HttpPost("chat/completions")]
    [ProducesResponseType(typeof(AzureChatResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> ChatCompletion([FromBody] AzureChatRequest request)
    {
        try
        {
            _logger.LogInformation("Received Azure OpenAI chat completion request");

            // Validate request
            if (request.Messages == null || !request.Messages.Any())
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Messages cannot be null or empty",
                    Status = 400
                });
            }

            // Get response
            var response = await _azureOpenAIService.ChatCompletionAsync(request);

            _logger.LogInformation("Azure OpenAI chat completion successful");
            return Ok(response);
        }
        catch (ContentFilteredException ex)
        {
            _logger.LogWarning(ex, "Content filtered in Azure OpenAI request");
            return BadRequest(new ProblemDetails
            {
                Title = "Content Filtered",
                Detail = ex.Message,
                Status = 400
            });
        }
        catch (AzureOpenAIException ex)
        {
            _logger.LogError(ex, "Azure OpenAI service error");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Azure OpenAI Error",
                Detail = ex.Message,
                Status = 500
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Azure OpenAI chat completion");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Stream a chat completion response from Azure OpenAI
    /// </summary>
    /// <param name="request">Chat completion request</param>
    /// <returns>Server-sent events stream</returns>
    [HttpPost("chat/completions/stream")]
    [Produces("text/event-stream")]
    public async Task StreamChatCompletion([FromBody] AzureChatRequest request)
    {
        try
        {
            _logger.LogInformation("Starting Azure OpenAI streaming chat completion");

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await foreach (var chunk in _azureOpenAIService.ChatCompletionStreamAsync(request))
            {
                var eventData = $"data: {System.Text.Json.JsonSerializer.Serialize(chunk)}\n\n";
                await Response.WriteAsync(eventData);
                await Response.Body.FlushAsync();
            }

            await Response.WriteAsync("data: [DONE]\n\n");
            await Response.Body.FlushAsync();

            _logger.LogInformation("Azure OpenAI streaming chat completion completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Azure OpenAI streaming chat completion");
            await Response.WriteAsync($"data: {{\"error\": \"{ex.Message}\"}}\n\n");
        }
    }

    /// <summary>
    /// Generate embeddings using Azure OpenAI
    /// </summary>
    /// <param name="request">Embedding request</param>
    /// <returns>Embedding response</returns>
    [HttpPost("embeddings")]
    [ProducesResponseType(typeof(AzureEmbeddingResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetEmbeddings([FromBody] AzureEmbeddingRequest request)
    {
        try
        {
            _logger.LogInformation("Received Azure OpenAI embeddings request for {InputCount} inputs", request.Input.Count);

            if (request.Input == null || !request.Input.Any())
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Input cannot be null or empty",
                    Status = 400
                });
            }

            var response = await _azureOpenAIService.GetEmbeddingsAsync(request);

            _logger.LogInformation("Azure OpenAI embeddings generated successfully");
            return Ok(response);
        }
        catch (AzureOpenAIException ex)
        {
            _logger.LogError(ex, "Azure OpenAI embeddings error");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Azure OpenAI Error",
                Detail = ex.Message,
                Status = 500
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Azure OpenAI embeddings");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get available Azure OpenAI deployments
    /// </summary>
    /// <returns>List of deployments</returns>
    [HttpGet("deployments")]
    [ProducesResponseType(typeof(List<AzureDeploymentInfo>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetDeployments()
    {
        try
        {
            _logger.LogInformation("Retrieving Azure OpenAI deployments");

            var deployments = await _azureOpenAIService.GetDeploymentsAsync();

            return Ok(deployments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Azure OpenAI deployments");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Failed to retrieve deployments",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Count tokens in text
    /// </summary>
    /// <param name="text">Text to count tokens for</param>
    /// <param name="model">Model to use for counting (optional)</param>
    /// <returns>Token count</returns>
    [HttpPost("tokens/count")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> CountTokens([FromBody] string text, [FromQuery] string model = "gpt-35-turbo")
    {
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Text cannot be null or empty",
                    Status = 400
                });
            }

            var tokenCount = await _tokenService.CountTokensAsync(text, model);
            var cost = await _tokenService.EstimateCostAsync(tokenCount, 0, model);

            return Ok(new
            {
                text = text.Length > 100 ? text.Substring(0, 100) + "..." : text,
                tokenCount,
                estimatedCost = cost,
                model
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting tokens");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Failed to count tokens",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Check content safety
    /// </summary>
    /// <param name="content">Content to check</param>
    /// <returns>Content filter result</returns>
    [HttpPost("content/check")]
    [ProducesResponseType(typeof(ContentFilterResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> CheckContent([FromBody] string content)
    {
        try
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Content cannot be null or empty",
                    Status = 400
                });
            }

            var result = await _contentFilterService.CheckContentAsync(content);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking content safety");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Failed to check content safety",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get usage metrics
    /// </summary>
    /// <param name="startDate">Start date for metrics</param>
    /// <param name="endDate">End date for metrics</param>
    /// <returns>Usage metrics</returns>
    [HttpGet("usage")]
    [ProducesResponseType(typeof(AzureUsageMetrics), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GetUsage([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            if (start >= end)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Date Range",
                    Detail = "Start date must be before end date",
                    Status = 400
                });
            }

            var metrics = await _azureOpenAIService.GetUsageMetricsAsync(start, end);

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usage metrics");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Failed to retrieve usage metrics",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Health check for Azure OpenAI service
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 503)]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            // Try to get deployments as a simple health check
            var deployments = await _azureOpenAIService.GetDeploymentsAsync();

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                deploymentsCount = deployments.Count,
                service = "Azure OpenAI"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure OpenAI health check failed");
            return StatusCode(503, new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = "Azure OpenAI service is not available",
                Status = 503
            });
        }
    }
}