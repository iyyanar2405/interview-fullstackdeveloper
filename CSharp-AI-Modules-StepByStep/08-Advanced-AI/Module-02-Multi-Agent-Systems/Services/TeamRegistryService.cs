using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class TeamRegistryService
{
    private readonly ConcurrentDictionary<string, AgentTeamDefinition> _teams = new(StringComparer.OrdinalIgnoreCase);

    public TeamRegistryService(IOptions<MultiAgentOptions> options)
    {
        foreach (var team in options.Value.Teams)
        {
            _teams[team.Name] = Clone(team);
        }
    }

    public IReadOnlyCollection<AgentTeamDefinition> GetTeams()
    {
        return _teams.Values.Select(Clone).ToArray();
    }

    public AgentTeamDefinition? GetTeam(string teamName)
    {
        return _teams.TryGetValue(teamName, out var team) ? Clone(team) : null;
    }

    private static AgentTeamDefinition Clone(AgentTeamDefinition team)
    {
        return new AgentTeamDefinition
        {
            Name = team.Name,
            Mission = team.Mission,
            Roles = team.Roles.Select(role => new AgentRoleDefinition
            {
                RoleId = role.RoleId,
                Persona = role.Persona,
                Skills = role.Skills.ToList(),
                Capacity = role.Capacity,
                DecisionRights = role.DecisionRights.ToList()
            }).ToList(),
            PreferredChannels = team.PreferredChannels.ToList(),
            Playbooks = team.Playbooks.ToList()
        };
    }
}
