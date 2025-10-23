using Microsoft.AspNetCore.Mvc;
using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;

namespace Module_02_Multi_Agent_Systems.Controllers;

[ApiController]
[Route("api/multi-agent/playbooks")]
public sealed class PlaybooksController : ControllerBase
{
    private readonly PlaybookLibraryService _playbooks;

    public PlaybooksController(PlaybookLibraryService playbooks)
    {
        _playbooks = playbooks;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlaybookDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetPlaybooks()
    {
        return Ok(_playbooks.GetPlaybooks());
    }

    [HttpGet("{playbookId}")]
    [ProducesResponseType(typeof(PlaybookDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(string playbookId)
    {
        var playbook = _playbooks.GetPlaybook(playbookId);
        return playbook is null ? NotFound() : Ok(playbook);
    }
}
