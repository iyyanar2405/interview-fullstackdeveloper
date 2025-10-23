using Microsoft.AspNetCore.Mvc;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MemoryConsolidationController : ControllerBase
{
    private readonly MemoryConsolidationService _consolidationService;

    public MemoryConsolidationController(MemoryConsolidationService consolidationService)
    {
        _consolidationService = consolidationService;
    }

    [HttpGet("events")]
    public ActionResult<IReadOnlyCollection<MemoryConsolidationEvent>> GetEvents()
    {
        return Ok(_consolidationService.GetEvents());
    }
}
