using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class ChannelRegistryService
{
    private readonly ConcurrentDictionary<string, CommunicationChannelDefinition> _channels = new(StringComparer.OrdinalIgnoreCase);

    public ChannelRegistryService(IOptions<MultiAgentOptions> options)
    {
        foreach (var channel in options.Value.Channels)
        {
            _channels[channel.ChannelId] = Clone(channel);
        }
    }

    public IReadOnlyCollection<CommunicationChannelDefinition> GetChannels()
    {
        return _channels.Values.Select(Clone).ToArray();
    }

    public CommunicationChannelDefinition? GetChannel(string channelId)
    {
        return _channels.TryGetValue(channelId, out var channel) ? Clone(channel) : null;
    }

    private static CommunicationChannelDefinition Clone(CommunicationChannelDefinition channel)
    {
        return new CommunicationChannelDefinition
        {
            ChannelId = channel.ChannelId,
            Type = channel.Type,
            Description = channel.Description,
            SupportedMessageTypes = channel.SupportedMessageTypes.ToList(),
            Persistent = channel.Persistent
        };
    }
}
