namespace Module_02_Multi_Agent_Systems.Models;

public sealed class MultiAgentOptions
{
    public List<AgentTeamDefinition> Teams { get; set; } = new();
    public List<CommunicationChannelDefinition> Channels { get; set; } = new();
    public List<PlaybookDefinition> Playbooks { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int CoordinationIntervalMilliseconds { get; set; } = 250;
    public int MaxConcurrentTasks { get; set; } = 8;
    public double EscalationProbability { get; set; } = 0.15;
    public double ConsensusFailureRate { get; set; } = 0.05;
}

public sealed class AgentTeamDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Mission { get; set; } = string.Empty;
    public List<AgentRoleDefinition> Roles { get; set; } = new();
    public List<string> PreferredChannels { get; set; } = new();
    public List<string> Playbooks { get; set; } = new();
}

public sealed class AgentRoleDefinition
{
    public string RoleId { get; set; } = string.Empty;
    public string Persona { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public int Capacity { get; set; } = 1;
    public List<string> DecisionRights { get; set; } = new();
}

public sealed class CommunicationChannelDefinition
{
    public string ChannelId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> SupportedMessageTypes { get; set; } = new();
    public bool Persistent { get; set; }
}

public sealed class PlaybookDefinition
{
    public string PlaybookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<CoordinationStep> Steps { get; set; } = new();
}

public sealed class CoordinationStep
{
    public string StepId { get; set; } = string.Empty;
    public string OwnerRoleId { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public string HandOff { get; set; } = string.Empty;
}

public enum AgentTaskState
{
    Queued,
    Assigned,
    InProgress,
    AwaitingConsensus,
    Completed,
    Escalated,
    Failed
}

public sealed class MultiAgentTask
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public string TeamName { get; set; } = string.Empty;
    public string PlaybookId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AgentTaskState State { get; set; } = AgentTaskState.Queued;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public List<CoordinationLog> Timeline { get; set; } = new();
    public List<ConsensusVote> Votes { get; set; } = new();
    public Dictionary<string, string> Outputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class CoordinationLog
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string RoleId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
}

public sealed class ConsensusVote
{
    public string RoleId { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public string Rationale { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ChannelMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString("N");
    public string ChannelId { get; set; } = string.Empty;
    public string SenderRoleId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class TaskGraphDefinition
{
    public string GraphId { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public List<TaskGraphNode> Nodes { get; set; } = new();
    public List<TaskGraphEdge> Edges { get; set; } = new();
}

public sealed class TaskGraphNode
{
    public string NodeId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class TaskGraphEdge
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
}

public sealed class TaskSubmissionRequest
{
    public string TeamName { get; set; } = string.Empty;
    public string PlaybookId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class ConsensusSubmission
{
    public bool Approved { get; set; }
    public string Rationale { get; set; } = string.Empty;
}
