using AI.PromptEngineering.Models;
using AI.PromptEngineering.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.PromptEngineering.Controllers;

/// <summary>
/// Prompt engineering controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PromptController : ControllerBase
{
    private readonly IPromptTemplateService _templateService;
    private readonly IFewShotService _fewShotService;
    private readonly IChainOfThoughtService _chainOfThoughtService;
    private readonly IPromptOptimizationService _optimizationService;
    private readonly ILogger<PromptController> _logger;

    public PromptController(
        IPromptTemplateService templateService,
        IFewShotService fewShotService,
        IChainOfThoughtService chainOfThoughtService,
        IPromptOptimizationService optimizationService,
        ILogger<PromptController> logger)
    {
        _templateService = templateService;
        _fewShotService = fewShotService;
        _chainOfThoughtService = chainOfThoughtService;
        _optimizationService = optimizationService;
        _logger = logger;
    }

    #region Template Management

    [HttpPost("templates")]
    public async Task<ActionResult<PromptTemplate>> CreateTemplate([FromBody] PromptTemplate template)
    {
        try
        {
            var created = await _templateService.CreateTemplateAsync(template);
            return CreatedAtAction(nameof(GetTemplate), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("templates/{id}")]
    public async Task<ActionResult<PromptTemplate>> GetTemplate(string id)
    {
        var template = await _templateService.GetTemplateAsync(id);
        return template == null ? NotFound() : Ok(template);
    }

    [HttpPut("templates/{id}")]
    public async Task<ActionResult<PromptTemplate>> UpdateTemplate(string id, [FromBody] PromptTemplate template)
    {
        try
        {
            var updated = await _templateService.UpdateTemplateAsync(id, template);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("templates/{id}")]
    public async Task<IActionResult> DeleteTemplate(string id)
    {
        var deleted = await _templateService.DeleteTemplateAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("templates/search")]
    public async Task<ActionResult<PromptSearchResponse>> SearchTemplates([FromBody] PromptSearchRequest request)
    {
        var results = await _templateService.SearchTemplatesAsync(request);
        return Ok(results);
    }

    [HttpPost("templates/{id}/render")]
    public async Task<ActionResult<PromptTemplateResponse>> RenderTemplate(string id, [FromBody] Dictionary<string, object> variables)
    {
        try
        {
            var request = new PromptTemplateRequest { TemplateId = id, Variables = variables };
            var response = await _templateService.RenderTemplateAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("templates/validate")]
    public async Task<ActionResult<PromptValidationResult>> ValidateTemplate([FromBody] PromptTemplate template)
    {
        var result = await _templateService.ValidateTemplateAsync(template);
        return Ok(result);
    }

    [HttpGet("templates/{id}/versions")]
    public async Task<ActionResult<List<PromptVersion>>> GetTemplateVersions(string id)
    {
        var versions = await _templateService.GetTemplateVersionsAsync(id);
        return Ok(versions);
    }

    #endregion

    #region Few-Shot Learning

    [HttpPost("few-shot")]
    public async Task<ActionResult<FewShotPromptResponse>> GenerateFewShotPrompt([FromBody] FewShotPromptRequest request)
    {
        try
        {
            var response = await _fewShotService.GenerateFewShotPromptAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("few-shot/examples")]
    public async Task<ActionResult<FewShotExample>> AddExample([FromBody] FewShotExample example)
    {
        var added = await _fewShotService.AddExampleAsync(example);
        return CreatedAtAction(nameof(GetExamplesByTags), new { tags = example.Tags }, added);
    }

    [HttpPost("few-shot/examples/search")]
    public async Task<ActionResult<List<FewShotExample>>> GetExamplesByTags([FromBody] List<string> tags)
    {
        var examples = await _fewShotService.GetExamplesByTagsAsync(tags);
        return Ok(examples);
    }

    #endregion

    #region Chain-of-Thought

    [HttpPost("chain-of-thought")]
    public async Task<ActionResult<ChainOfThoughtPromptResponse>> GenerateChainOfThoughtPrompt([FromBody] ChainOfThoughtPromptRequest request)
    {
        try
        {
            var response = await _chainOfThoughtService.GenerateChainOfThoughtPromptAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("chain-of-thought/parse")]
    public async Task<ActionResult<List<ChainOfThoughtStep>>> ParseChainOfThoughtResponse([FromBody] string response)
    {
        var steps = await _chainOfThoughtService.ParseChainOfThoughtResponseAsync(response);
        return Ok(steps);
    }

    #endregion

    #region Optimization

    [HttpPost("optimize")]
    public async Task<ActionResult<PromptOptimizationResult>> OptimizePrompt([FromBody] OptimizePromptRequest request)
    {
        try
        {
            var result = await _optimizationService.OptimizePromptAsync(request.Prompt, request.Options);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("optimize/suggestions")]
    public async Task<ActionResult<List<PromptImprovementSuggestion>>> GetImprovementSuggestions([FromBody] string prompt)
    {
        var suggestions = await _optimizationService.GenerateImprovementSuggestionsAsync(prompt);
        return Ok(suggestions);
    }

    [HttpPost("ab-test")]
    public async Task<ActionResult<PromptABTest>> CreateABTest([FromBody] CreateABTestRequest request)
    {
        var test = await _optimizationService.CreateABTestAsync(
            request.VariantA, request.VariantB, request.Name, request.Description);
        return CreatedAtAction(nameof(GetABTestWinner), new { id = test.Id }, test);
    }

    [HttpPost("ab-test/{id}/record")]
    public async Task<IActionResult> RecordABTestResult(string id, [FromBody] ABTestResultRequest request)
    {
        try
        {
            await _optimizationService.RecordABTestResultAsync(
                id, request.Variant, request.QualityScore, request.ResponseTime, request.Cost, request.Success);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("ab-test/{id}/winner")]
    public async Task<ActionResult<string>> GetABTestWinner(string id)
    {
        try
        {
            var winner = await _optimizationService.GetABTestWinnerAsync(id);
            return winner == null ? NotFound("Insufficient data or no clear winner") : Ok(new { Winner = winner });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Health & Info

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Features = new[] { "Templates", "Few-Shot", "Chain-of-Thought", "Optimization", "A/B Testing" }
        });
    }

    #endregion
}

public class OptimizePromptRequest
{
    public string Prompt { get; set; } = string.Empty;
    public PromptOptimizationOptions Options { get; set; } = new();
}

public class CreateABTestRequest
{
    public string VariantA { get; set; } = string.Empty;
    public string VariantB { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ABTestResultRequest
{
    public string Variant { get; set; } = string.Empty;
    public double QualityScore { get; set; }
    public double ResponseTime { get; set; }
    public double Cost { get; set; }
    public bool Success { get; set; }
}
