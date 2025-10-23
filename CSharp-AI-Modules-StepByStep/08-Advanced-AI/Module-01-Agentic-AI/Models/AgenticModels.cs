namespace Module_01_Agentic_AI.Models;

public sealed class AgenticOptions
{
    public List<AgentDefinition> Agents { get; set; } = new();
    public List<ToolDefinition> Tools { get; set; } = new();
    public List<WorkflowTemplate> Workflows { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int PlanningDelayMilliseconds { get; set; } = 150;
    public int ToolDelayMilliseconds { get; set; } = 120;
    public List<string> DefaultSignals { get; set; } = new() { "goal", "context", "feedback" };
    public double ReflectionProbability { get; set; } = 0.25;
    public int MaxConcurrentTasks { get; set; } = 4;
}

public sealed class AgentDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Persona { get; set; } = string.Empty;
    public List<string> Goals { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public List<string> AllowedTools { get; set; } = new();
    public List<string> MemoryChannels { get; set; } = new();
}

public sealed class ToolDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InputSchema { get; set; } = string.Empty;
    public string OutputSchema { get; set; } = string.Empty;
    public List<string> SafetyGuidelines { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public sealed class WorkflowTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkflowStage> Stages { get; set; } = new();
}

public sealed class WorkflowStage
{
    public string StageId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string PrimaryAgentId { get; set; } = string.Empty;
    public List<string> SupportingTools { get; set; } = new();
    public string ExitCriteria { get; set; } = string.Empty;
}

public enum AgentTaskStatus
{
    Pending,
    Planning,
    Executing,
    Reflecting,
    Completed,
    Failed
}

public sealed class AgentTask
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public string AgentId { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public AgentTaskStatus Status { get; set; } = AgentTaskStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public List<TaskPlanStep> Plan { get; set; } = new();
    public List<TaskStepRecord> Execution { get; set; } = new();
    public List<AgentMemoryEntry> CapturedMemories { get; set; } = new();
    public AgentReflection? Reflection { get; set; }
    public string OutcomeSummary { get; set; } = string.Empty;
}

public sealed class TaskPlanStep
{
    public int Order { get; set; }
    public string Thought { get; set; } = string.Empty;
    public string Tool { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
}

public sealed class TaskStepRecord
{
    public int Order { get; set; }
    public string Tool { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public List<ExecutionArtifact> Artifacts { get; set; } = new();
}

public sealed class ExecutionArtifact
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public sealed class AgentMemoryEntry
{
    public string AgentId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Salience { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class AgentReflection
{
    public string Summary { get; set; } = string.Empty;
    public List<string> FollowUpQuestions { get; set; } = new();
    public List<string> ImprovementIdeas { get; set; } = new();
}

public sealed class AgentSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    public string AgentId { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndedAt { get; set; }
    public List<AgentTask> Tasks { get; set; } = new();
    public List<AgentMemoryEntry> Memories { get; set; } = new();
}

public sealed class ToolInvocationResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public List<ExecutionArtifact> Artifacts { get; set; } = new();
    public string Reasoning { get; set; } = string.Empty;
}

public sealed class TaskCreationRequest
{
    public string AgentId { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}

public sealed class TaskStatusResponse
{
    public AgentTaskStatus Status { get; set; }
    public string OutcomeSummary { get; set; } = string.Empty;
    public List<TaskStepRecord> Execution { get; set; } = new();
    public AgentReflection? Reflection { get; set; }
}

public sealed class SessionCreationRequest
{
    public string AgentId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
}

public sealed class SessionSummary
{
    public string SessionId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public int TaskCount { get; set; }
    public int MemoryCount { get; set; }
}
