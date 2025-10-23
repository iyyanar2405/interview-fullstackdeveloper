using Microsoft.AspNetCore.Mvc;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.Controllers;

[ApiController]
[Route("api/monitoring/incidents")]
public sealed class IncidentsController : ControllerBase
{
    private readonly IncidentService _incidents;

    public IncidentsController(IncidentService incidents)
    {
        _incidents = incidents;
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ActiveIncident>), StatusCodes.Status200OK)]
    public IActionResult GetActive()
    {
        return Ok(_incidents.GetActiveIncidents());
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PastIncident>), StatusCodes.Status200OK)]
    public IActionResult GetHistory()
    {
        return Ok(_incidents.GetIncidentHistory());
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActiveIncident), StatusCodes.Status200OK)]
    public IActionResult Declare([FromBody] IncidentDeclarationRequest request)
    {
        request ??= new IncidentDeclarationRequest();
        var incident = _incidents.DeclareIncident(request.AlertId, request.Description, request.Severity);
        return Ok(incident);
    }

    [HttpPost("{incidentId}/resolve")]
    [ProducesResponseType(typeof(PastIncident), StatusCodes.Status200OK)]
    public IActionResult Resolve(string incidentId, [FromBody] IncidentResolutionRequest request)
    {
        request ??= new IncidentResolutionRequest();
        var resolved = _incidents.ResolveIncident(incidentId, request.ResolutionSummary);
        return Ok(resolved);
    }
}

public sealed class IncidentDeclarationRequest
{
    public string AlertId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;
}

public sealed class IncidentResolutionRequest
{
    public string ResolutionSummary { get; set; } = string.Empty;
}
