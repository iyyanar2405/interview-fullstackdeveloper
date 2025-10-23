using Microsoft.AspNetCore.Mvc;
using Module_04_Application_Insights.Services;

namespace Module_04_Application_Insights.Controllers;

[ApiController]
[Route("api/telemetry")]
public sealed class TelemetryController : ControllerBase
{
    private readonly TelemetryService _telemetry;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(TelemetryService telemetry, ILogger<TelemetryController> logger)
    {
        _telemetry = telemetry;
        _logger = logger;
    }

    [HttpPost("event")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult TrackEvent([FromBody] Dictionary<string, string>? properties)
    {
        _telemetry.TrackCustomEvent("custom.event", properties);
        return Accepted();
    }

    [HttpPost("metric")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult TrackMetric([FromQuery] string name, [FromQuery] double value)
    {
        _telemetry.TrackBusinessMetric(name, value, new Dictionary<string, string>
        {
            ["source"] = "manual",
            ["controller"] = nameof(TelemetryController)
        });
        return Accepted();
    }

    [HttpGet("dependency")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SimulateDependency([FromQuery] int delayMs = 200)
    {
        using (_telemetry.StartDependency("HTTP", "external-service", $"GET /simulate?delay={delayMs}"))
        {
            await Task.Delay(Math.Clamp(delayMs, 10, 5_000));
        }

        return Ok(new { Message = "Dependency simulated." });
    }

    [HttpGet("exception")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GenerateException()
    {
        try
        {
            throw new InvalidOperationException("Simulated exception for Application Insights testing.");
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                ["endpoint"] = "api/telemetry/exception"
            });
            _logger.LogError(ex, "Simulated exception captured");
            throw;
        }
    }
}
