using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.Controllers;

[ApiController]
[Route("api/monitoring/telemetry")]
public sealed class TelemetryController : ControllerBase
{
    private readonly TelemetryStoreService _telemetry;
    private readonly SloEvaluationService _sloEvaluator;
    private readonly AlertingService _alertingService;
    private readonly IncidentService _incidentService;

    public TelemetryController(
        TelemetryStoreService telemetry,
        SloEvaluationService sloEvaluator,
        AlertingService alertingService,
        IncidentService incidentService)
    {
        _telemetry = telemetry;
        _sloEvaluator = sloEvaluator;
        _alertingService = alertingService;
        _incidentService = incidentService;
    }

    [HttpPost("ingest")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Ingest([FromBody] TelemetryIngestionRequest request)
    {
        request ??= new TelemetryIngestionRequest();

        foreach (var metric in request.Metrics)
        {
            _telemetry.IngestMetric(metric);
        }

        foreach (var log in request.Logs)
        {
            _telemetry.IngestLog(log);
        }

        foreach (var trace in request.Traces)
        {
            _telemetry.IngestTrace(trace);
        }

        foreach (var evt in request.Events)
        {
            _telemetry.IngestEvent(evt);
        }

        var alerts = _alertingService.EvaluateAlerts();
        if (alerts.Count > 0)
        {
            foreach (var alert in alerts)
            {
                _incidentService.DeclareIncident(alert.Id, $"Auto-incident for alert {alert.Name}", IncidentSeverity.High);
            }
        }

        return Accepted(new { status = "ingested" });
    }

    [HttpGet("snapshot")]
    [ProducesResponseType(typeof(MonitoringSnapshot), StatusCodes.Status200OK)]
    public IActionResult Snapshot()
    {
        var snapshot = _telemetry.CaptureSnapshot();
        snapshot.Slos = _sloEvaluator.Evaluate().ToList();
        snapshot.Alerts = _alertingService.GetActiveAlerts().ToList();
        snapshot.CurrentIncidents = _incidentService.GetActiveIncidents().ToList();
        return Ok(snapshot);
    }
}
