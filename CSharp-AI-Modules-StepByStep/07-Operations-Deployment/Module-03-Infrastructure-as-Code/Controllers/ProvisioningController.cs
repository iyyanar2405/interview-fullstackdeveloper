using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;
using Module_03_Infrastructure_as_Code.Services;

namespace Module_03_Infrastructure_as_Code.Controllers;

[ApiController]
[Route("api/provisioning")]
public sealed class ProvisioningController : ControllerBase
{
    private readonly InfrastructurePlanService _plans;
    private readonly ProvisioningQueueService _queue;
    private readonly ProvisioningHistoryService _history;
    private readonly SimulationOptions _simulation;

    public ProvisioningController(
        InfrastructurePlanService plans,
        ProvisioningQueueService queue,
        ProvisioningHistoryService history,
        IOptions<InfrastructureOptions> options)
    {
        _plans = plans;
        _queue = queue;
        _history = history;
        _simulation = options.Value.Simulation;
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> QueueProvisioning([FromBody] QueueProvisioningRequest request, CancellationToken cancellationToken)
    {
        request ??= new QueueProvisioningRequest();
        request.Request ??= new InfrastructureRequest();
        if (string.IsNullOrWhiteSpace(request.Request.Environment))
        {
            request.Request.Environment = _simulation.Environments.FirstOrDefault() ?? "dev";
        }

        var plan = _plans.CreatePlan(request.TemplateName, request.Request);
        var item = new ProvisioningQueueItem
        {
            Plan = plan,
            Request = request.Request,
            RequestedBy = string.IsNullOrWhiteSpace(request.RequestedBy)
                ? _simulation.DefaultRequestedBy
                : request.RequestedBy
        };

        await _queue.QueueAsync(item, cancellationToken);
        return Accepted(new { runId = item.RunId, status = "queued" });
    }

    [HttpGet("runs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProvisioningRunRecord>), StatusCodes.Status200OK)]
    public IActionResult GetRuns([FromQuery] int take = 20)
    {
        take = Math.Clamp(take, 1, 100);
        var records = _history.GetRecent(take);
        return Ok(records);
    }

    [HttpGet("runs/{id:guid}")]
    [ProducesResponseType(typeof(ProvisioningRunRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRun([FromRoute] Guid id)
    {
        var record = _history.Get(id);
        return record is null ? NotFound() : Ok(record);
    }
}
