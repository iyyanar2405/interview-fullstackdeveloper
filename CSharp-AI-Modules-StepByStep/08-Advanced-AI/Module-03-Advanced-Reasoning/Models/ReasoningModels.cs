namespace Module_03_Advanced_Reasoning.Models;

public sealed class AdvancedReasoningOptions
{
    public List<StrategyDefinition> Strategies { get; set; } = new();
    public List<PromptTemplate> Prompts { get; set; } = new();
    public List<EvaluationMetric> Metrics { get; set; } = new();
    public SimulationSettings Simulation { get; set; } = new();
}

public sealed class SimulationSettings
{
    public int ThoughtDelayMilliseconds { get; set; } = 120;
    public int MaxBreadth { get; set; } = 3;
    public int MaxDepth { get; set; } = 4;
    public double ReflectionProbability { get; set; } = 0.4;
    public double FailureProbability { get; set; } = 0.08;
}

public sealed class StrategyDefinition
{
    public string StrategyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mode { get; set; } = "chain";
    public List<string> DefaultPrompts { get; set; } = new();
    public List<string> Metrics { get; set; } = new();
}

public sealed class PromptTemplate
{
    public string PromptId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public sealed class EvaluationMetric
{
    public string MetricId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Threshold { get; set; }
}

public enum ReasoningTaskStatus
{
    Pending,
    Generating,
    Exploring,
    Evaluating,
    Reflecting,
    Completed,
    Failed
}

public sealed class ReasoningTask
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public string StrategyId { get; set; } = string.Empty;
    public string PromptId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public ReasoningTaskStatus Status { get; set; } = ReasoningTaskStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public List<ThoughtStep> ChainOfThought { get; set; } = new();
    public ReasoningGraph Graph { get; set; } = new();
    public List<EvaluationScore> Scores { get; set; } = new();
    public ReflectionSummary? Reflection { get; set; }
    public string FinalAnswer { get; set; } = string.Empty;
}

public sealed class ThoughtStep
{
    public int Order { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ReasoningGraph
{
    public string GraphId { get; set; } = Guid.NewGuid().ToString("N");
    public List<ReasoningNode> Nodes { get; set; } = new();
    public List<ReasoningEdge> Edges { get; set; } = new();
}

public sealed class ReasoningNode
{
    public string NodeId { get; set; } = Guid.NewGuid().ToString("N");
    public string ParentId { get; set; } = string.Empty;
    public string Thought { get; set; } = string.Empty;
    public double Score { get; set; }
    public int Depth { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ReasoningEdge
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
}

public sealed class EvaluationScore
{
    public string MetricId { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public sealed class ReflectionSummary
{
    public string Insight { get; set; } = string.Empty;
    public List<string> FollowUps { get; set; } = new();
    public List<string> Corrections { get; set; } = new();
}

public sealed class ReasoningTaskRequest
{
    public string Question { get; set; } = string.Empty;
    public string StrategyId { get; set; } = string.Empty;
    public string? PromptId { get; set; }
}

public sealed class ThoughtExpansion
{
    public string NodeId { get; set; } = string.Empty;
    public string Thought { get; set; } = string.Empty;
    public double Score { get; set; }
}
