using Microsoft.AspNetCore.Mvc;
using Module_03_Infrastructure_as_Code.Models;
using Module_03_Infrastructure_as_Code.Services;

namespace Module_03_Infrastructure_as_Code.Controllers;

[ApiController]
[Route("api/infrastructure")]
public sealed class InfrastructureController : ControllerBase
{
    private readonly InfrastructureTemplateService _templates;
    private readonly InfrastructurePlanService _plans;

    public InfrastructureController(
        InfrastructureTemplateService templates,
        InfrastructurePlanService plans)
    {
        _templates = templates;
        _plans = plans;
    }

    [HttpGet("templates")]
    [ProducesResponseType(typeof(IReadOnlyCollection<IaCTemplate>), StatusCodes.Status200OK)]
    public IActionResult GetTemplates()
    {
        return Ok(_templates.GetAll());
    }

    [HttpGet("templates/{name}")]
    [ProducesResponseType(typeof(IaCTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTemplate([FromRoute] string name)
    {
        var template = _templates.Get(name);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost("templates")]
    [ProducesResponseType(typeof(IaCTemplate), StatusCodes.Status201Created)]
    public IActionResult UpsertTemplate([FromBody] IaCTemplate template)
    {
        var saved = _templates.Upsert(template);
        return CreatedAtAction(nameof(GetTemplate), new { name = saved.Name }, saved);
    }

    [HttpDelete("templates/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteTemplate([FromRoute] string name)
    {
        _templates.Delete(name);
        return NoContent();
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(InfrastructurePlan), StatusCodes.Status200OK)]
    public IActionResult GeneratePlan([FromBody] InfrastructurePlanRequest request)
    {
        request ??= new InfrastructurePlanRequest();
        request.Request ??= new InfrastructureRequest();
        var plan = _plans.CreatePlan(request.TemplateName, request.Request);
        return Ok(plan);
    }

    [HttpPost("preview")]
    [ProducesResponseType(typeof(PlanPreview), StatusCodes.Status200OK)]
    public IActionResult Preview([FromBody] InfrastructurePlanRequest request)
    {
        request ??= new InfrastructurePlanRequest();
        request.Request ??= new InfrastructureRequest();
        var preview = _plans.BuildPreview(request.TemplateName, request.Request);
        return Ok(preview);
    }
}
