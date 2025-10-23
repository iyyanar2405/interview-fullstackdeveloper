using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Module_01_Containerization.Models;
using Module_01_Containerization.Services;

namespace Module_01_Containerization.Controllers;

[ApiController]
[Route("api/containerization")]
public sealed class ContainerizationController : ControllerBase
{
    private readonly DockerfileTemplateService _templates;
    private readonly ContainerBuildPlannerService _planner;
    private readonly ContainerBuildQueueService _queue;
    private readonly ILogger<ContainerizationController> _logger;
    private readonly ContainerizationOptions _options;

    public ContainerizationController(
        DockerfileTemplateService templates,
        ContainerBuildPlannerService planner,
        ContainerBuildQueueService queue,
        IOptions<ContainerizationOptions> options,
        ILogger<ContainerizationController> logger)
    {
        _templates = templates;
        _planner = planner;
        _queue = queue;
        _options = options.Value;
        _logger = logger;
    }

    [HttpGet("templates")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DockerfileTemplate>), StatusCodes.Status200OK)]
    public IActionResult GetTemplates()
    {
        return Ok(_templates.GetAll());
    }

    [HttpGet("templates/{name}")]
    [ProducesResponseType(typeof(DockerfileTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTemplate([FromRoute] string name)
    {
        var template = _templates.Get(name);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost("templates")]
    [ProducesResponseType(typeof(DockerfileTemplate), StatusCodes.Status201Created)]
    public IActionResult CreateTemplate([FromBody] DockerfileTemplate template)
    {
        var created = _templates.Add(template);
        return CreatedAtAction(nameof(GetTemplate), new { name = created.Name }, created);
    }

    [HttpDelete("templates/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteTemplate([FromRoute] string name)
    {
        _templates.Delete(name);
        return NoContent();
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(ContainerBuildPlan), StatusCodes.Status200OK)]
    public IActionResult GeneratePlan([FromBody] ContainerBuildPlanRequest request)
    {
        request ??= new ContainerBuildPlanRequest();
        var plan = _planner.CreatePlan(request.TemplateName, request.Request, request.ComposeProfile);
        return Ok(plan);
    }

    [HttpGet("compose-profiles")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ComposeProfile>), StatusCodes.Status200OK)]
    public IActionResult GetComposeProfiles()
    {
        return Ok(_options.ComposeProfiles);
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> QueueBuild([FromBody] QueueBuildRequest request, CancellationToken cancellationToken)
    {
        request ??= new QueueBuildRequest();
        var item = new BuildQueueItem
        {
            Request = request.Request,
            RequestedBy = request.RequestedBy,
            TemplateName = request.TemplateName,
            ComposeProfile = request.ComposeProfile
        };

        await _queue.QueueAsync(item, cancellationToken);
        _logger.LogInformation("Queued build for {Project}", item.Request.ProjectName);
        return Accepted(new { status = "queued", project = item.Request.ProjectName });
    }
}
