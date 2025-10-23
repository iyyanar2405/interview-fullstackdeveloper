using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Module_04_Scaling_Strategies.Models;
using Module_04_Scaling_Strategies.Services;

namespace Module_04_Scaling_Strategies.Controllers;

[ApiController]
[Route("api/scaling-simulations")]
public sealed class SimulationsController : ControllerBase
{
    private readonly ScalingRecommendationService _recommendations;
    private readonly ScalingQueueService _queue;
    private readonly ScalingHistoryService _history;
    private readonly ScalingProfileService _profiles;
    private readonly SimulationSettings _settings;

    public SimulationsController(
        ScalingRecommendationService recommendations,
        ScalingQueueService queue,
        ScalingHistoryService history,
        ScalingProfileService profiles,
        IOptions<ScalingOptions> options)
    {
        _recommendations = recommendations;
        _queue = queue;
        _history = history;
        _profiles = profiles;
        _settings = options.Value.Simulation;
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Queue([FromBody] ScalingSimulationRequest request, CancellationToken cancellationToken)
    {
        request ??= new ScalingSimulationRequest();
        request.Snapshot ??= new WorkloadSnapshot();
        var scenario = ResolveScenario(request.Scenario?.Name ?? request.Scenario?.Pattern);
        if (scenario is not null)
        {
            request.Scenario = scenario;
        }

        if (string.IsNullOrWhiteSpace(request.Snapshot.Environment))
        {
            request.Snapshot.Environment = _settings.Environments.FirstOrDefault() ?? "dev";
        }

        if (request.Snapshot.CurrentInstances <= 0)
        {
            var profile = string.IsNullOrWhiteSpace(request.ProfileName)
                ? _profiles.GetAll().First()
                : _profiles.Get(request.ProfileName) ?? _profiles.GetAll().First();
            request.Snapshot.CurrentInstances = profile.MinInstances;
        }

        var planRequest = new ScalingPlanRequest
        {
            ProfileName = request.ProfileName,
            Snapshot = request.Snapshot,
            Scenario = request.Scenario,
            ForecastHorizonMinutes = request.DurationMinutes
        };

        var plan = _recommendations.BuildPlan(planRequest);

        var queueItem = new ScalingQueueItem
        {
            Plan = plan,
            Request = request,
            RequestedAt = DateTimeOffset.UtcNow
        };

        await _queue.QueueAsync(queueItem, cancellationToken);
        return Accepted(new { queueItem.RunId, status = "queued" });
    }

    [HttpGet("runs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ScalingRunRecord>), StatusCodes.Status200OK)]
    public IActionResult GetRuns([FromQuery] int take = 20)
    {
        take = Math.Clamp(take, 1, 100);
        var runs = _history.GetRecent(take);
        return Ok(runs);
    }

    [HttpGet("runs/{id:guid}")]
    [ProducesResponseType(typeof(ScalingRunRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRun([FromRoute] Guid id)
    {
        var run = _history.Get(id);
        return run is null ? NotFound() : Ok(run);
    }

    private LoadScenario? ResolveScenario(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var scenario = _settings.Scenarios.FirstOrDefault(s =>
            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(s.Pattern, name, StringComparison.OrdinalIgnoreCase));

        return scenario is null
            ? null
            : new LoadScenario
            {
                Name = scenario.Name,
                Pattern = scenario.Pattern,
                PeakRequestsPerSecond = scenario.PeakRequestsPerSecond,
                BaseRequestsPerSecond = scenario.BaseRequestsPerSecond,
                DurationMinutes = scenario.DurationMinutes,
                ErrorBudgetPercent = scenario.ErrorBudgetPercent
            };
    }
}
