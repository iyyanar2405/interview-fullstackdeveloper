using Microsoft.AspNetCore.Mvc;
using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;

namespace Module_02_Multi_Agent_Systems.Controllers;

[ApiController]
[Route("api/multi-agent/channels")]
public sealed class ChannelsController : ControllerBase
{
    private readonly ChannelRegistryService _channels;
    private readonly MessageRouterService _router;

    public ChannelsController(ChannelRegistryService channels, MessageRouterService router)
    {
        _channels = channels;
        _router = router;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CommunicationChannelDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetChannels()
    {
        return Ok(_channels.GetChannels());
    }

    [HttpGet("{channelId}/messages")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ChannelMessage>), StatusCodes.Status200OK)]
    public IActionResult GetMessages(string channelId, [FromQuery] int limit = 20)
    {
        return Ok(_router.GetRecent(channelId, limit));
    }

    [HttpPost("{channelId}/messages")]
    [ProducesResponseType(typeof(ChannelMessage), StatusCodes.Status201Created)]
    public IActionResult Publish(string channelId, [FromBody] ChannelMessageRequest request)
    {
        var message = new ChannelMessage
        {
            ChannelId = channelId,
            SenderRoleId = request.SenderRoleId,
            Type = request.Type,
            Content = request.Content
        };

        _router.Publish(message);
        return CreatedAtAction(nameof(GetMessages), new { channelId, limit = 1 }, message);
    }
}

public sealed class ChannelMessageRequest
{
    public string SenderRoleId { get; set; } = string.Empty;
    public string Type { get; set; } = "chat";
    public string Content { get; set; } = string.Empty;
}
