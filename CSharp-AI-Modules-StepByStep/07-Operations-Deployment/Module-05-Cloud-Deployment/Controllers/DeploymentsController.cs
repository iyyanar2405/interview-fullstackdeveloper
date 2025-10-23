using Microsoft.AspNetCore.Mvc;
using Module_05_Cloud_Deployment.Models;
using Module_05_Cloud_Deployment.Services;

namespace Module_05_Cloud_Deployment.Controllers;

[ApiController]
[Route("api/cloud-deployments")]
public sealed class DeploymentsController : ControllerBase
{
    private readonly DeploymentPlanService _plans;
    private readonly DeploymentQueueService _queue;
    private readonly DeploymentHistoryService _history;

    public DeploymentsController(
        DeploymentPlanService plans,
        DeploymentQueueService queue,
        DeploymentHistoryService history)
    {
        _plans = plans;
        _queue = queue;
        _history = history;
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(DeploymentPlan), StatusCodes.Status200OK)]
    public IActionResult Plan([FromBody] DeploymentPlanRequest request)
    {
        request ??= new DeploymentPlanRequest();
        request.Overrides ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var plan = _plans.BuildPlan(request);
        return Ok(plan);
    }

    [HttpPost("preview")]
    [ProducesResponseType(typeof(DeploymentPlan), StatusCodes.Status200OK)]
    public IActionResult Preview([FromBody] PreviewRequest request)
    {
        request ??= new PreviewRequest();
        request.Overrides ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var plan = _plans.BuildPreview(request);
        return Ok(plan);
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Queue([FromBody] DeploymentPlanRequest request, CancellationToken cancellationToken)
    {
        request ??= new DeploymentPlanRequest();
        request.Overrides ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var plan = _plans.BuildPlan(request);
        var item = new DeploymentQueueItem
        {
            Plan = plan,
            Request = request,
            RequestedAt = DateTimeOffset.UtcNow
        };

        await _queue.QueueAsync(item, cancellationToken);
        return Accepted(new { item.RunId, status = "queued" });
    }

    [HttpGet("runs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DeploymentRunRecord>), StatusCodes.Status200OK)]
    public IActionResult Runs([FromQuery] int take = 20)
    {
        take = Math.Clamp(take, 1, 100);
        return Ok(_history.GetRecent(take));
    }

    [HttpGet("runs/{id:guid}")]
    [ProducesResponseType(typeof(DeploymentRunRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Run([FromRoute] Guid id)
    {
        var record = _history.Get(id);
        return record is null ? NotFound() : Ok(record);
    }
}
