using Microsoft.AspNetCore.Mvc;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MemoryRetrievalController : ControllerBase
{
    private readonly MemoryRetrievalService _retrievalService;

    public MemoryRetrievalController(MemoryRetrievalService retrievalService)
    {
        _retrievalService = retrievalService;
    }

    [HttpPost("retrieve")]
    public ActionResult<MemoryRetrievalResponse> Retrieve([FromBody] MemoryRetrievalRequest request)
    {
        var response = _retrievalService.RetrieveMemories(request);
        return Ok(response);
    }
}
