using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class AgentCatalogService
{
    private readonly ConcurrentDictionary<string, AgentDefinition> _agents = new(StringComparer.OrdinalIgnoreCase);

    public AgentCatalogService(IOptions<AgenticOptions> options)
    {
        foreach (var agent in options.Value.Agents)
        {
            _agents[agent.Id] = Clone(agent);
        }
    }

    public IReadOnlyCollection<AgentDefinition> GetAgents()
    {
        return _agents.Values.Select(Clone).ToArray();
    }

    public AgentDefinition? GetAgent(string agentId)
    {
        return _agents.TryGetValue(agentId, out var agent) ? Clone(agent) : null;
    }

    public AgentDefinition Upsert(AgentDefinition definition)
    {
        var clone = Clone(definition);
        _agents[clone.Id] = clone;
        return Clone(clone);
    }

    private static AgentDefinition Clone(AgentDefinition definition)
    {
        return new AgentDefinition
        {
            Id = definition.Id,
            Name = definition.Name,
            Persona = definition.Persona,
            Goals = definition.Goals.ToList(),
            Capabilities = definition.Capabilities.ToList(),
            AllowedTools = definition.AllowedTools.ToList(),
            MemoryChannels = definition.MemoryChannels.ToList()
        };
    }
}
