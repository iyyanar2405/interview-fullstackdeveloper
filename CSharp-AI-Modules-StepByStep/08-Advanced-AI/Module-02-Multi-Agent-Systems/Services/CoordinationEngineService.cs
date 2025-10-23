using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_02_Multi_Agent_Systems.Models;
using System.Linq;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class CoordinationEngineService
{
    private readonly TeamRegistryService _teams;
    private readonly PlaybookLibraryService _playbooks;
    private readonly ChannelRegistryService _channels;
    private readonly MessageRouterService _router;
    private readonly TaskBoardService _board;
    private readonly ILogger<CoordinationEngineService> _logger;
    private readonly SimulationSettings _settings;
    private readonly Random _random = new();

    public CoordinationEngineService(
        TeamRegistryService teams,
        PlaybookLibraryService playbooks,
        ChannelRegistryService channels,
        MessageRouterService router,
        TaskBoardService board,
        IOptions<MultiAgentOptions> options,
        ILogger<CoordinationEngineService> logger)
    {
        _teams = teams;
        _playbooks = playbooks;
        _channels = channels;
        _router = router;
        _board = board;
        _logger = logger;
        _settings = options.Value.Simulation;
    }

    public async Task CoordinateAsync(MultiAgentTask task, CancellationToken cancellationToken)
    {
        var team = _teams.GetTeam(task.TeamName);
        var playbook = _playbooks.GetPlaybook(task.PlaybookId);

        if (team is null || playbook is null)
        {
            task.State = AgentTaskState.Failed;
            task.Timeline.Add(new CoordinationLog
            {
                Timestamp = DateTimeOffset.UtcNow,
                Message = "Team or playbook missing",
                RoleId = "system",
                ChannelId = "" 
            });
            _board.Update(task);
            return;
        }

        var primaryChannel = team.PreferredChannels.FirstOrDefault() ?? "coordination";
        var delay = TimeSpan.FromMilliseconds(Math.Max(50, _settings.CoordinationIntervalMilliseconds));

        task.State = AgentTaskState.InProgress;
        _board.Update(task);

        foreach (var step in playbook.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(delay, cancellationToken);

            var channel = _channels.GetChannel(primaryChannel) ?? new CommunicationChannelDefinition
            {
                ChannelId = primaryChannel,
                Type = "ad-hoc",
                Description = "Simulated channel"
            };

            var message = new ChannelMessage
            {
                ChannelId = channel.ChannelId,
                SenderRoleId = step.OwnerRoleId,
                Type = "step-update",
                Content = $"Executing objective '{step.Objective}'"
            };

            _router.Publish(message);

            task.Timeline.Add(new CoordinationLog
            {
                Timestamp = message.Timestamp,
                ChannelId = channel.ChannelId,
                RoleId = step.OwnerRoleId,
                Message = message.Content
            });

            if (!SimulateConsensus(task, step))
            {
                task.State = AgentTaskState.Escalated;
                task.Timeline.Add(new CoordinationLog
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    ChannelId = channel.ChannelId,
                    RoleId = "escalation",
                    Message = "Consensus failed; escalating"
                });
                _board.Update(task);
                return;
            }
        }

        task.State = AgentTaskState.Completed;
        task.CompletedAt = DateTimeOffset.UtcNow;
        task.Outputs["summary"] = $"Completed playbook {playbook.Name} with {playbook.Steps.Count} steps.";
        _board.Update(task);
    }

    private bool SimulateConsensus(MultiAgentTask task, CoordinationStep step)
    {
        var vote = new ConsensusVote
        {
            RoleId = step.OwnerRoleId,
            Approved = _random.NextDouble() >= _settings.ConsensusFailureRate,
            Rationale = "Simulated vote",
            Timestamp = DateTimeOffset.UtcNow
        };

        task.Votes.Add(vote);
        return vote.Approved;
    }
}
