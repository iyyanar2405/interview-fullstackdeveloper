using Microsoft.AspNetCore.Mvc;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MemoryProfilesController : ControllerBase
{
    private readonly MemoryProfileCatalogService _catalog;
    private readonly MemoryStoreService _storeService;

    public MemoryProfilesController(MemoryProfileCatalogService catalog, MemoryStoreService storeService)
    {
        _catalog = catalog;
        _storeService = storeService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<MemoryProfileDefinition>> GetProfiles()
    {
        return Ok(_catalog.GetProfiles());
    }

    [HttpGet("{profileId}")]
    public ActionResult<MemoryProfileDefinition> GetProfile(string profileId)
    {
        var profile = _catalog.GetProfile(profileId);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpGet("{profileId}/summary")]
    public ActionResult<MemoryProfileSummary> GetSummary(string profileId)
    {
        var summary = _storeService.GetProfileSummary(profileId);
        return Ok(summary);
    }
}
