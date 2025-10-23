using Microsoft.AspNetCore.Mvc;
using Module_02_Quality_Assessment.Models;
using Module_02_Quality_Assessment.Services;

namespace Module_02_Quality_Assessment.Controllers;

[ApiController]
[Route("api/quality")]
public sealed class QualityAssessmentController : ControllerBase
{
    private readonly EvaluationPipelineService _pipeline;
    private readonly QualityMetricsService _metrics;
    private readonly FeedbackAggregationService _feedback;

    public QualityAssessmentController(
        EvaluationPipelineService pipeline,
        QualityMetricsService metrics,
        FeedbackAggregationService feedback)
    {
        _pipeline = pipeline;
        _metrics = metrics;
        _feedback = feedback;
    }

    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(EvaluationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Evaluate([FromBody] EvaluationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _pipeline.EvaluateAsync(request);
        return Ok(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(EvaluationSummary), StatusCodes.Status200OK)]
    public IActionResult GetSummary([FromQuery] int minutes = 60)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 5, 1440));
        var summary = _metrics.GetSummary(window);
        return Ok(summary);
    }

    [HttpGet("recent")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EvaluationResult>), StatusCodes.Status200OK)]
    public IActionResult GetRecent([FromQuery] int count = 10)
    {
        count = Math.Clamp(count, 1, 100);
        var recent = _metrics.GetRecent(count);
        return Ok(recent);
    }

    [HttpPost("feedback")]
    [ProducesResponseType(typeof(FeedbackAggregate), StatusCodes.Status200OK)]
    public IActionResult SubmitFeedback([FromBody] FeedbackSubmission submission)
    {
        if (submission.Rating is < 0 or > 5)
        {
            return BadRequest("Rating must be between 0 and 5");
        }

        var aggregate = _feedback.Submit(submission);
        return Ok(aggregate);
    }

    [HttpGet("feedback/{responseId}")]
    [ProducesResponseType(typeof(FeedbackAggregate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetFeedback([FromRoute] string responseId)
    {
        var aggregate = _feedback.GetAggregate(responseId);
        return aggregate is null ? NotFound() : Ok(aggregate);
    }
}
