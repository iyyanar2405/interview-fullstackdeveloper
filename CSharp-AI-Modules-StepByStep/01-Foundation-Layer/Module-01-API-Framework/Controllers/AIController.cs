using Microsoft.AspNetCore.Mvc;
using AI.Foundation.API.Models;

namespace AI.Foundation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("AI")]
public class AIController : ControllerBase
{
    private readonly ILogger<AIController> _logger;

    public AIController(ILogger<AIController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Process a chat message using AI (demo implementation)
    /// </summary>
    /// <param name="request">The chat request</param>
    /// <returns>AI response</returns>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(APIResponse<ChatResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse<ChatResponseModel>>> ProcessChat([FromBody] APIRequest<ChatRequestModel> request)
    {
        try
        {
            _logger.LogInformation("Processing chat request: {Message}", request.Data?.Message);

            if (request.Data?.Message == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Error = "Message is required",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Simulate AI processing
            await Task.Delay(100);

            var response = new ChatResponseModel
            {
                Message = $"AI Response: {request.Data.Message}",
                Confidence = Random.Shared.NextDouble(),
                ProcessingTime = TimeSpan.FromMilliseconds(Random.Shared.Next(50, 200)),
                Model = "demo-model-v1"
            };

            var apiResponse = new APIResponse<ChatResponseModel>
            {
                Data = response,
                Success = true,
                Timestamp = DateTime.UtcNow,
                RequestId = request.RequestId
            };

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, new ErrorResponse
            {
                Error = "Internal server error",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get available AI models
    /// </summary>
    /// <returns>List of available models</returns>
    [HttpGet("models")]
    [ProducesResponseType(typeof(APIResponse<List<AIModelInfo>>), StatusCodes.Status200OK)]
    public ActionResult<APIResponse<List<AIModelInfo>>> GetModels()
    {
        var models = new List<AIModelInfo>
        {
            new() { Name = "demo-model-v1", Version = "1.0", Description = "Demo model for testing" },
            new() { Name = "demo-model-v2", Version = "2.0", Description = "Advanced demo model" }
        };

        var response = new APIResponse<List<AIModelInfo>>
        {
            Data = models,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Generate text completion
    /// </summary>
    /// <param name="request">Text completion request</param>
    /// <returns>Generated text</returns>
    [HttpPost("complete")]
    [ProducesResponseType(typeof(APIResponse<TextCompletionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<TextCompletionResponse>>> CompleteText([FromBody] APIRequest<TextCompletionRequest> request)
    {
        if (request.Data?.Prompt == null)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Prompt is required",
                Timestamp = DateTime.UtcNow
            });
        }

        // Simulate text completion
        await Task.Delay(200);

        var completion = new TextCompletionResponse
        {
            CompletedText = $"{request.Data.Prompt} [AI generated continuation...]",
            TokensUsed = Random.Shared.Next(10, 100),
            Model = request.Data.Model ?? "demo-model-v1"
        };

        var response = new APIResponse<TextCompletionResponse>
        {
            Data = completion,
            Success = true,
            Timestamp = DateTime.UtcNow,
            RequestId = request.RequestId
        };

        return Ok(response);
    }
}