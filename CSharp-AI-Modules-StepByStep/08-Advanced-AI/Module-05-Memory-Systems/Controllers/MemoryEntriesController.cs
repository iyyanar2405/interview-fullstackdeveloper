using Microsoft.AspNetCore.Mvc;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MemoryEntriesController : ControllerBase
{
    private readonly MemoryStoreService _storeService;

    public MemoryEntriesController(MemoryStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost("ingest")]
    public ActionResult<MemoryEntry> IngestMemory([FromBody] MemoryIngestionRequest request)
    {
        var entry = _storeService.IngestMemory(request);
        return CreatedAtAction(nameof(GetEntries), new { profileId = entry.ProfileId, storeId = entry.StoreId }, entry);
    }

    [HttpPost("ingest/batch")]
    public ActionResult<IReadOnlyCollection<MemoryEntry>> IngestBatch([FromBody] MemoryBatchIngestionRequest request)
    {
        var entries = _storeService.IngestBatch(request);
        return Accepted(entries);
    }

    [HttpGet("{profileId}/{storeId}/snapshot")]
    public ActionResult<MemoryStoreSnapshot> GetSnapshot(string profileId, string storeId)
    {
        var snapshot = _storeService.GetStoreSnapshot(profileId, storeId);
        return Ok(snapshot);
    }

    [HttpGet("{profileId}/{storeId}/entries")]
    public ActionResult<IReadOnlyCollection<MemoryEntry>> GetEntries(string profileId, string storeId, [FromQuery] int limit = 50)
    {
        limit = Math.Clamp(limit, 1, 200);
        var entries = _storeService.GetEntriesForStore(profileId, storeId, _ => true)
            .OrderByDescending(entry => entry.CreatedAt)
            .Take(limit)
            .ToArray();

        return Ok(entries);
    }
}
