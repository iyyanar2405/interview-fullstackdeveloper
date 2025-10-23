using System.Collections.Concurrent;
using System.Linq;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class MessageRouterService
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<ChannelMessage>> _channels = new(StringComparer.OrdinalIgnoreCase);

    public void Publish(ChannelMessage message)
    {
        var queue = _channels.GetOrAdd(message.ChannelId, _ => new ConcurrentQueue<ChannelMessage>());
        queue.Enqueue(Clone(message));
        Trim(queue, 200);
    }

    public IReadOnlyCollection<ChannelMessage> GetRecent(string channelId, int limit = 50)
    {
        if (!_channels.TryGetValue(channelId, out var queue))
        {
            return Array.Empty<ChannelMessage>();
        }

        return queue.Reverse().Take(limit).Select(Clone).ToArray();
    }

    private static void Trim(ConcurrentQueue<ChannelMessage> queue, int maxSize)
    {
        while (queue.Count > maxSize && queue.TryDequeue(out _))
        {
        }
    }

    private static ChannelMessage Clone(ChannelMessage message)
    {
        return new ChannelMessage
        {
            MessageId = message.MessageId,
            ChannelId = message.ChannelId,
            SenderRoleId = message.SenderRoleId,
            Type = message.Type,
            Content = message.Content,
            Timestamp = message.Timestamp
        };
    }
}
