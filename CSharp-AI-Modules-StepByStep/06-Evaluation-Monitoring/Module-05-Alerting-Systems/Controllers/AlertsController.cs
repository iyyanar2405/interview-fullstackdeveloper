using Microsoft.AspNetCore.Mvc;
using Module_05_Alerting_Systems.Models;
using Module_05_Alerting_Systems.Services;

namespace Module_05_Alerting_Systems.Controllers;

[ApiController]
[Route("api/alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly AlertRuleService _rules;
    private readonly AlertEvaluationService _evaluation;
    private readonly AlertHistoryService _history;

    public AlertsController(
        AlertRuleService rules,
        AlertEvaluationService evaluation,
        AlertHistoryService history)
    {
        _rules = rules;
        _evaluation = evaluation;
        _history = history;
    }

    [HttpGet("rules")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlertRule>), StatusCodes.Status200OK)]
    public IActionResult GetRules() => Ok(_rules.GetAll());

    [HttpPost("rules")]
    [ProducesResponseType(typeof(AlertRule), StatusCodes.Status201Created)]
    public IActionResult CreateRule([FromBody] AlertRule rule)
    {
        var created = _rules.Add(rule);
        return CreatedAtAction(nameof(GetRule), new { ruleId = created.RuleId }, created);
    }

    [HttpGet("rules/{ruleId}")]
    [ProducesResponseType(typeof(AlertRule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRule([FromRoute] string ruleId)
    {
        var rule = _rules.Get(ruleId);
        return rule is null ? NotFound() : Ok(rule);
    }

    [HttpPut("rules/{ruleId}")]
    [ProducesResponseType(typeof(AlertRule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateRule([FromRoute] string ruleId, [FromBody] AlertRule rule)
    {
        var updated = _rules.Update(ruleId, rule);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("rules/{ruleId}/toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult ToggleRule([FromRoute] string ruleId, [FromQuery] bool enabled = true)
    {
        _rules.Toggle(ruleId, enabled);
        return NoContent();
    }

    [HttpDelete("rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteRule([FromRoute] string ruleId)
    {
        _rules.Delete(ruleId);
        return NoContent();
    }

    [HttpPost("metrics")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> IngestMetric([FromBody] MetricSample sample, CancellationToken cancellationToken)
    {
        await _evaluation.EvaluateAsync(sample, cancellationToken);
        return Accepted();
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlertNotification>), StatusCodes.Status200OK)]
    public IActionResult GetHistory([FromQuery] int count = 20)
    {
        count = Math.Clamp(count, 1, 200);
        return Ok(_history.GetRecent(count));
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(AlertSummary), StatusCodes.Status200OK)]
    public IActionResult GetSummary()
    {
        var rules = _rules.GetAll();
        var baseSummary = _history.GetSummary();

        var result = new AlertSummary
        {
            TotalRules = rules.Count,
            ActiveRules = rules.Count(r => r.Enabled),
            AlertsLastHour = baseSummary.AlertsLastHour,
            AlertsLast24Hours = baseSummary.AlertsLast24Hours,
            GeneratedAt = baseSummary.GeneratedAt
        };

        return Ok(result);
    }
}
