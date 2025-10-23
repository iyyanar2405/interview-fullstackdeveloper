using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class PlaybookLibraryService
{
    private readonly ConcurrentDictionary<string, PlaybookDefinition> _playbooks = new(StringComparer.OrdinalIgnoreCase);

    public PlaybookLibraryService(IOptions<MultiAgentOptions> options)
    {
        foreach (var playbook in options.Value.Playbooks)
        {
            _playbooks[playbook.PlaybookId] = Clone(playbook);
        }
    }

    public IReadOnlyCollection<PlaybookDefinition> GetPlaybooks()
    {
        return _playbooks.Values.Select(Clone).ToArray();
    }

    public PlaybookDefinition? GetPlaybook(string playbookId)
    {
        return _playbooks.TryGetValue(playbookId, out var playbook) ? Clone(playbook) : null;
    }

    private static PlaybookDefinition Clone(PlaybookDefinition playbook)
    {
        return new PlaybookDefinition
        {
            PlaybookId = playbook.PlaybookId,
            Name = playbook.Name,
            Summary = playbook.Summary,
            Steps = playbook.Steps.Select(step => new CoordinationStep
            {
                StepId = step.StepId,
                OwnerRoleId = step.OwnerRoleId,
                Objective = step.Objective,
                RequiredSkills = step.RequiredSkills.ToList(),
                HandOff = step.HandOff
            }).ToList()
        };
    }
}
