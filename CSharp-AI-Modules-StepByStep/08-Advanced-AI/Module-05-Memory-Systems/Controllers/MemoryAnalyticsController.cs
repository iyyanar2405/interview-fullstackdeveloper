using Microsoft.AspNetCore.Mvc;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MemoryAnalyticsController : ControllerBase
{
    private readonly MemoryAnalyticsService _analyticsService;

    public MemoryAnalyticsController(MemoryAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("{profileId}")]
    public ActionResult<MemoryAnalyticsSnapshot> GetAnalytics(string profileId)
    {
        var snapshot = _analyticsService.GetSnapshot(profileId);
        return Ok(snapshot);
    }
}
