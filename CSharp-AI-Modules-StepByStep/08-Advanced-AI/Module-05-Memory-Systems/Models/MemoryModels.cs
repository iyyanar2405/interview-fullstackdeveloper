namespace Module_05_Memory_Systems.Models;

public static class MemoryTypes
{
    public const string ShortTerm = "short-term";
    public const string LongTerm = "long-term";
    public const string Episodic = "episodic";
    public const string Semantic = "semantic";
    public const string Working = "working";

    public static bool TryNormalize(string? value, out string normalized)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = value.Trim().ToLowerInvariant();
        return normalized is ShortTerm or LongTerm or Episodic or Semantic or Working;
    }
}

public sealed class MemorySystemsOptions
{
    public List<MemoryProfileDefinition> Profiles { get; set; } = new();
    public MemorySimulationOptions Simulation { get; set; } = new();
}

public sealed class MemorySimulationOptions
{
    public int ConsolidationIntervalMilliseconds { get; set; } = 1500;
    public double AutoConsolidationImportanceThreshold { get; set; } = 0.7;
    public int MaxEntriesPerStore { get; set; } = 2048;
    public int ShortTermRetentionMinutes { get; set; } = 90;
    public int MaxConsolidationEvents { get; set; } = 250;
}

public sealed class MemoryProfileDefinition
{
    public string ProfileId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> DefaultTags { get; set; } = new();
    public List<string> RetrievalStrategies { get; set; } = new();
    public List<MemoryStoreDefinition> Stores { get; set; } = new();
    public MemoryRetentionPolicy Retention { get; set; } = new();
}

public sealed class MemoryStoreDefinition
{
    public string StoreId { get; set; } = string.Empty;
    public string MemoryType { get; set; } = MemoryTypes.ShortTerm;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; } = 256;
    public double ImportanceFloor { get; set; } = 0.25;
    public bool EnableAutoSummaries { get; set; }
        = false;
    public string? PromoteToStoreId { get; set; }
        = null;
    public List<string> DefaultTags { get; set; } = new();
}

public sealed class MemoryRetentionPolicy
{
    public int ShortTermMinutes { get; set; } = 120;
    public int LongTermDays { get; set; } = 180;
    public int EpisodicDays { get; set; } = 45;
}

public sealed class MemoryEntry
{
    public string EntryId { get; set; } = Guid.NewGuid().ToString("N");
    public string ProfileId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string MemoryType { get; set; } = MemoryTypes.ShortTerm;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Importance { get; set; } = 0.5;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastAccessedAt { get; set; } = DateTimeOffset.UtcNow;
    public int RecallCount { get; set; }
        = 0;
    public bool IsPinned { get; set; }
        = false;
    public List<MemoryTrace> Traces { get; set; } = new();

    public MemoryEntry Clone()
    {
        return new MemoryEntry
        {
            EntryId = EntryId,
            ProfileId = ProfileId,
            StoreId = StoreId,
            MemoryType = MemoryType,
            Title = Title,
            Content = Content,
            Importance = Importance,
            Tags = Tags.ToList(),
            Metadata = new Dictionary<string, string>(Metadata, StringComparer.OrdinalIgnoreCase),
            CreatedAt = CreatedAt,
            LastAccessedAt = LastAccessedAt,
            RecallCount = RecallCount,
            IsPinned = IsPinned,
            Traces = Traces.Select(trace => trace.Clone()).ToList()
        };
    }
}

public sealed class MemoryTrace
{
    public string TraceId { get; set; } = Guid.NewGuid().ToString("N");
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = "system";
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public MemoryTrace Clone()
    {
        return new MemoryTrace
        {
            TraceId = TraceId,
            Action = Action,
            Actor = Actor,
            Notes = Notes,
            Timestamp = Timestamp
        };
    }
}

public sealed class MemoryIngestionRequest
{
    public string ProfileId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Importance { get; set; } = 0.5;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class MemoryBatchIngestionRequest
{
    public string ProfileId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public List<MemoryIngestionItem> Items { get; set; } = new();
}

public sealed class MemoryIngestionItem
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Importance { get; set; } = 0.5;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class MemoryRetrievalRequest
{
    public string ProfileId { get; set; } = string.Empty;
    public List<string> StoreIds { get; set; } = new();
    public List<string> MemoryTypes { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string SearchText { get; set; } = string.Empty;
    public double MinImportance { get; set; } = 0.0;
    public int Limit { get; set; } = 10;
    public bool IncludeTraces { get; set; }
        = false;
}

public sealed class MemoryRetrievalResponse
{
    public string ProfileId { get; set; } = string.Empty;
    public List<MemoryEntry> Entries { get; set; } = new();
    public List<MemoryInsight> Insights { get; set; } = new();
}

public sealed class MemoryInsight
{
    public string InsightId { get; set; } = Guid.NewGuid().ToString("N");
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Score { get; set; } = 0.0;
}

public sealed class MemoryStoreSnapshot
{
    public string ProfileId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string MemoryType { get; set; } = string.Empty;
    public int TotalEntries { get; set; }
        = 0;
    public double AverageImportance { get; set; } = 0.0;
    public int RecentEntryCount { get; set; } = 0;
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.MinValue;
    public Dictionary<string, int> TagFrequency { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class MemoryProfileSummary
{
    public string ProfileId { get; set; } = string.Empty;
    public int TotalEntries { get; set; }
        = 0;
    public double AverageImportance { get; set; } = 0.0;
    public List<MemoryStoreSnapshot> Stores { get; set; } = new();
}

public sealed class MemoryAnalyticsSnapshot
{
    public MemoryProfileSummary Summary { get; set; } = new();
    public List<MemoryInsight> Insights { get; set; } = new();
}

public sealed class MemoryConsolidationEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString("N");
    public string ProfileId { get; set; } = string.Empty;
    public string SourceStoreId { get; set; } = string.Empty;
    public string TargetStoreId { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int EntriesMerged { get; set; }
        = 0;
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public List<string> SourceEntryIds { get; set; } = new();
    public string TargetEntryId { get; set; } = string.Empty;

    public MemoryConsolidationEvent Clone()
    {
        return new MemoryConsolidationEvent
        {
            EventId = EventId,
            ProfileId = ProfileId,
            SourceStoreId = SourceStoreId,
            TargetStoreId = TargetStoreId,
            Strategy = Strategy,
            Summary = Summary,
            EntriesMerged = EntriesMerged,
            OccurredAt = OccurredAt,
            SourceEntryIds = SourceEntryIds.ToList(),
            TargetEntryId = TargetEntryId
        };
    }
}
