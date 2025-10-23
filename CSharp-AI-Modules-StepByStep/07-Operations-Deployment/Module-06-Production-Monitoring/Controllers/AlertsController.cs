using Microsoft.AspNetCore.Mvc;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.Controllers;

[ApiController]
[Route("api/monitoring/alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly AlertingService _alerting;

    public AlertsController(AlertingService alerting)
    {
        _alerting = alerting;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlertInstance>), StatusCodes.Status200OK)]
    public IActionResult GetActive()
    {
        return Ok(_alerting.GetActiveAlerts());
    }

    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlertInstance>), StatusCodes.Status200OK)]
    public IActionResult Evaluate()
    {
        var triggered = _alerting.EvaluateAlerts();
        return Ok(triggered);
    }
}
