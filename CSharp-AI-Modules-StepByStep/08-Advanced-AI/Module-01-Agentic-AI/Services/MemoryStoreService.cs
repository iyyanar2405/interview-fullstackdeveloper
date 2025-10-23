using System.Collections.Concurrent;
using System.Linq;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class MemoryStoreService
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<AgentMemoryEntry>> _memories = new(StringComparer.OrdinalIgnoreCase);

    public void AddMemory(AgentMemoryEntry entry)
    {
        var queue = _memories.GetOrAdd(entry.AgentId, _ => new ConcurrentQueue<AgentMemoryEntry>());
        queue.Enqueue(Clone(entry));
        Trim(queue, 200);
    }

    public IReadOnlyCollection<AgentMemoryEntry> GetMemories(string agentId, int limit = 100)
    {
        if (!_memories.TryGetValue(agentId, out var queue))
        {
            return Array.Empty<AgentMemoryEntry>();
        }

        return queue.Reverse().Take(limit).Select(Clone).ToArray();
    }

    private static void Trim(ConcurrentQueue<AgentMemoryEntry> queue, int maxSize)
    {
        while (queue.Count > maxSize && queue.TryDequeue(out _))
        {
        }
    }

    private static AgentMemoryEntry Clone(AgentMemoryEntry entry)
    {
        return new AgentMemoryEntry
        {
            AgentId = entry.AgentId,
            Channel = entry.Channel,
            Content = entry.Content,
            Salience = entry.Salience,
            Timestamp = entry.Timestamp
        };
    }
}
