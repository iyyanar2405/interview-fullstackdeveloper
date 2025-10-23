using AI.Anthropic.Models;
using AI.Anthropic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace AI.Anthropic.Controllers;

/// <summary>
/// Claude API controller for chat completions and Constitutional AI features
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClaudeController : ControllerBase
{
    private readonly IClaudeService _claudeService;
    private readonly IConstitutionalAIService _constitutionalService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ClaudeController> _logger;

    public ClaudeController(
        IClaudeService claudeService,
        IConstitutionalAIService constitutionalService,
        ITokenService tokenService,
        ILogger<ClaudeController> logger)
    {
        _claudeService = claudeService;
        _constitutionalService = constitutionalService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a chat completion using Claude
    /// </summary>
    /// <param name="request">Chat completion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Claude response</returns>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ClaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClaudeResponse>> ChatAsync(
        [FromBody] ClaudeRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing chat completion request for model: {Model}", request.Model);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Estimate tokens and cost
            var inputTokens = await _tokenService.EstimateTokenCountAsync(
                string.Join(" ", request.Messages.Select(m => m.Content)), request.Model);

            _logger.LogDebug("Estimated input tokens: {InputTokens}", inputTokens);

            // Check context limits
            var exceedsLimit = await _tokenService.ExceedsContextLimitAsync(
                string.Join(" ", request.Messages.Select(m => m.Content)), request.Model);

            if (exceedsLimit)
            {
                return BadRequest("Request exceeds model's context limit. Consider using the long-context endpoint.");
            }

            // Process request
            var response = await _claudeService.ChatCompletionAsync(request, cancellationToken);

            _logger.LogInformation("Chat completion successful. Response ID: {Id}", response.Id);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters");
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            _logger.LogWarning(ex, "Rate limit exceeded");
            return StatusCode(StatusCodes.Status429TooManyRequests, "Rate limit exceeded. Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during Claude API call");
            return StatusCode(StatusCodes.Status502BadGateway, "Error communicating with Claude API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during chat completion");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Generate a streaming chat completion using Claude
    /// </summary>
    /// <param name="request">Streaming chat completion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Streaming response</returns>
    [HttpPost("chat/stream")]
    [ProducesResponseType(typeof(IAsyncEnumerable<ClaudeStreamEvent>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> ChatStreamAsync(
        [FromBody] ClaudeStreamRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing streaming chat completion request for model: {Model}", request.Model);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set response headers for streaming
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            // Process streaming request
            await foreach (var streamEvent in _claudeService.ChatCompletionStreamAsync(request, cancellationToken))
            {
                var eventData = System.Text.Json.JsonSerializer.Serialize(streamEvent);
                await Response.WriteAsync($"data: {eventData}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            _logger.LogInformation("Streaming chat completion completed");
            return new EmptyResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid streaming request parameters");
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            _logger.LogWarning(ex, "Rate limit exceeded for streaming request");
            return StatusCode(StatusCodes.Status429TooManyRequests, "Rate limit exceeded. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during streaming chat completion");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Process long context documents with intelligent chunking
    /// </summary>
    /// <param name="request">Long context request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synthesized response from multiple chunks</returns>
    [HttpPost("long-context")]
    [ProducesResponseType(typeof(LongContextResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LongContextResponse>> ProcessLongContextAsync(
        [FromBody] LongContextRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing long context request with {DocumentLength} characters", 
                request.Document.Length);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Document))
            {
                return BadRequest("Document content is required");
            }

            if (string.IsNullOrEmpty(request.Query))
            {
                return BadRequest("Query is required");
            }

            // Process long context
            var response = await _claudeService.ProcessLongContextAsync(request, cancellationToken);

            _logger.LogInformation("Long context processing completed. Processed {ChunkCount} chunks", 
                response.ChunksProcessed);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid long context request parameters");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during long context processing");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Validate content using Constitutional AI principles
    /// </summary>
    /// <param name="request">Constitutional validation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    [HttpPost("constitutional/validate")]
    [ProducesResponseType(typeof(ConstitutionalValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConstitutionalValidationResult>> ValidateConstitutionalAsync(
        [FromBody] ConstitutionalValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing Constitutional AI validation for content with {ContentLength} characters", 
                request.Content.Length);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                return BadRequest("Content is required for validation");
            }

            if (request.Principles?.Any() != true)
            {
                return BadRequest("At least one Constitutional AI principle is required");
            }

            // Perform Constitutional AI validation
            var result = await _constitutionalService.ValidateAsync(request.Content, request.Principles);

            _logger.LogInformation("Constitutional validation completed. Valid: {IsValid}, Score: {Score:F2}", 
                result.IsValid, result.Score);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid Constitutional validation request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Constitutional validation");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Apply Constitutional AI feedback to improve content
    /// </summary>
    /// <param name="request">Constitutional feedback request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Improved content</returns>
    [HttpPost("constitutional/improve")]
    [ProducesResponseType(typeof(ConstitutionalImprovementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConstitutionalImprovementResponse>> ImproveConstitutionalAsync(
        [FromBody] ConstitutionalImprovementRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing Constitutional AI improvement for content with {ContentLength} characters", 
                request.Content.Length);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                return BadRequest("Content is required for improvement");
            }

            if (request.Principles?.Any() != true)
            {
                return BadRequest("At least one Constitutional AI principle is required");
            }

            // Validate original content
            var originalValidation = await _constitutionalService.ValidateAsync(request.Content, request.Principles);

            // Apply Constitutional AI feedback
            var improvedContent = await _constitutionalService.ApplyConstitutionalFeedbackAsync(
                request.Content, request.Principles);

            // Validate improved content
            var improvedValidation = await _constitutionalService.ValidateAsync(improvedContent, request.Principles);

            var response = new ConstitutionalImprovementResponse
            {
                OriginalContent = request.Content,
                ImprovedContent = improvedContent,
                OriginalValidation = originalValidation,
                ImprovedValidation = improvedValidation,
                ImprovementApplied = !string.Equals(request.Content, improvedContent, StringComparison.Ordinal),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Constitutional improvement completed. Applied: {ImprovementApplied}, " +
                "Score improved from {OriginalScore:F2} to {ImprovedScore:F2}", 
                response.ImprovementApplied, originalValidation.Score, improvedValidation.Score);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid Constitutional improvement request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Constitutional improvement");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Generate Constitutional AI critique for content
    /// </summary>
    /// <param name="request">Constitutional critique request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Constitutional critique</returns>
    [HttpPost("constitutional/critique")]
    [ProducesResponseType(typeof(ConstitutionalCritiqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConstitutionalCritiqueResponse>> GenerateConstitutionalCritiqueAsync(
        [FromBody] ConstitutionalCritiqueRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating Constitutional AI critique for content with {ContentLength} characters", 
                request.Content.Length);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                return BadRequest("Content is required for critique");
            }

            if (request.Principles?.Any() != true)
            {
                return BadRequest("At least one Constitutional AI principle is required");
            }

            // Generate critique
            var critique = await _constitutionalService.GenerateCritiqueAsync(request.Content, request.Principles);

            // Get validation details
            var validation = await _constitutionalService.ValidateAsync(request.Content, request.Principles);

            var response = new ConstitutionalCritiqueResponse
            {
                Content = request.Content,
                Critique = critique,
                Validation = validation,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Constitutional critique generated successfully");

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid Constitutional critique request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Constitutional critique generation");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Estimate token count and cost for text
    /// </summary>
    /// <param name="request">Token estimation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token count and cost estimation</returns>
    [HttpPost("tokens/estimate")]
    [ProducesResponseType(typeof(TokenEstimationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenEstimationResponse>> EstimateTokensAsync(
        [FromBody] TokenEstimationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Estimating tokens for text with {TextLength} characters", request.Text.Length);

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("Text is required for token estimation");
            }

            // Estimate tokens
            var tokenCount = await _tokenService.EstimateTokenCountAsync(request.Text, request.Model);

            // Calculate cost (assuming some output tokens for estimation)
            var estimatedOutputTokens = request.EstimatedOutputTokens ?? (tokenCount / 4); // Conservative estimate
            var costEstimate = await _tokenService.CalculateCostAsync(tokenCount, estimatedOutputTokens, request.Model);

            // Check context limits
            var exceedsLimit = await _tokenService.ExceedsContextLimitAsync(request.Text, request.Model);
            var contextLimit = await _tokenService.GetModelContextLimitAsync(request.Model);

            // Estimate processing time
            var processingTime = await _tokenService.EstimateProcessingTimeAsync(tokenCount, estimatedOutputTokens, request.Model);

            var response = new TokenEstimationResponse
            {
                Text = request.Text,
                Model = request.Model,
                TokenCount = tokenCount,
                CostEstimate = costEstimate,
                ExceedsContextLimit = exceedsLimit,
                ContextLimit = contextLimit,
                EstimatedProcessingTime = processingTime,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token estimation");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Get available Claude models and their capabilities
    /// </summary>
    /// <returns>List of available models</returns>
    [HttpGet("models")]
    [ProducesResponseType(typeof(List<ModelInfo>), StatusCodes.Status200OK)]
    public ActionResult<List<ModelInfo>> GetModels()
    {
        try
        {
            var models = new List<ModelInfo>
            {
                new()
                {
                    Id = "claude-3-opus-20240229",
                    Name = "Claude 3 Opus",
                    Description = "Most capable model, best for complex tasks requiring deep reasoning",
                    MaxContextTokens = 200000,
                    MaxOutputTokens = 4096,
                    InputCostPer1KTokens = 0.015,
                    OutputCostPer1KTokens = 0.075,
                    Features = new[] { "Constitutional AI", "Long Context", "Advanced Reasoning", "Creative Writing" }
                },
                new()
                {
                    Id = "claude-3-sonnet-20240229",
                    Name = "Claude 3 Sonnet",
                    Description = "Balanced model for most use cases, good performance and speed",
                    MaxContextTokens = 200000,
                    MaxOutputTokens = 4096,
                    InputCostPer1KTokens = 0.003,
                    OutputCostPer1KTokens = 0.015,
                    Features = new[] { "Constitutional AI", "Long Context", "Balanced Performance", "Cost Effective" }
                },
                new()
                {
                    Id = "claude-3-haiku-20240307",
                    Name = "Claude 3 Haiku",
                    Description = "Fastest model, optimized for speed and efficiency",
                    MaxContextTokens = 200000,
                    MaxOutputTokens = 4096,
                    InputCostPer1KTokens = 0.00025,
                    OutputCostPer1KTokens = 0.00125,
                    Features = new[] { "Constitutional AI", "High Speed", "Low Cost", "Efficient Processing" }
                }
            };

            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model information");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Service health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthCheckResponse> HealthCheck()
    {
        try
        {
            var response = new HealthCheckResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Services = new Dictionary<string, string>
                {
                    ["ClaudeService"] = "Available",
                    ["ConstitutionalAIService"] = "Available",
                    ["TokenService"] = "Available"
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(StatusCodes.Status500InternalServerError, "Health check failed");
        }
    }
}

/// <summary>
/// Token estimation request model
/// </summary>
public class TokenEstimationRequest
{
    [Required]
    public string Text { get; set; } = string.Empty;

    public string Model { get; set; } = "claude-3-sonnet-20240229";

    public int? EstimatedOutputTokens { get; set; }
}

/// <summary>
/// Token estimation response model
/// </summary>
public class TokenEstimationResponse
{
    public string Text { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public CostEstimate CostEstimate { get; set; } = new();
    public bool ExceedsContextLimit { get; set; }
    public int ContextLimit { get; set; }
    public TimeSpan EstimatedProcessingTime { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Constitutional validation request model
/// </summary>
public class ConstitutionalValidationRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public List<string> Principles { get; set; } = new();
}

/// <summary>
/// Constitutional improvement request model
/// </summary>
public class ConstitutionalImprovementRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public List<string> Principles { get; set; } = new();
}

/// <summary>
/// Constitutional improvement response model
/// </summary>
public class ConstitutionalImprovementResponse
{
    public string OriginalContent { get; set; } = string.Empty;
    public string ImprovedContent { get; set; } = string.Empty;
    public ConstitutionalValidationResult OriginalValidation { get; set; } = new();
    public ConstitutionalValidationResult ImprovedValidation { get; set; } = new();
    public bool ImprovementApplied { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Constitutional critique request model
/// </summary>
public class ConstitutionalCritiqueRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public List<string> Principles { get; set; } = new();
}

/// <summary>
/// Constitutional critique response model
/// </summary>
public class ConstitutionalCritiqueResponse
{
    public string Content { get; set; } = string.Empty;
    public string Critique { get; set; } = string.Empty;
    public ConstitutionalValidationResult Validation { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Model information model
/// </summary>
public class ModelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxContextTokens { get; set; }
    public int MaxOutputTokens { get; set; }
    public double InputCostPer1KTokens { get; set; }
    public double OutputCostPer1KTokens { get; set; }
    public string[] Features { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Health check response model
/// </summary>
public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
    public Dictionary<string, string> Services { get; set; } = new();
}