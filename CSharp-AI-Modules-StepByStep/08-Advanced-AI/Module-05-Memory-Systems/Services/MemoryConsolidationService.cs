using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_05_Memory_Systems.Models;

namespace Module_05_Memory_Systems.Services;

public sealed class MemoryConsolidationService
{
    private readonly MemoryProfileCatalogService _catalog;
    private readonly MemoryStoreService _storeService;
    private readonly MemorySystemsOptions _options;
    private readonly ConcurrentQueue<MemoryConsolidationEvent> _events = new();

    public MemoryConsolidationService(
        MemoryProfileCatalogService catalog,
        MemoryStoreService storeService,
        IOptions<MemorySystemsOptions> options)
    {
        _catalog = catalog;
        _storeService = storeService;
        _options = options.Value;
    }

    public async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        foreach (var profile in _catalog.GetProfiles())
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var store in profile.Stores.Where(store => store.EnableAutoSummaries && !string.IsNullOrWhiteSpace(store.PromoteToStoreId)))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var candidates = _storeService.GetEntriesForStore(profile.ProfileId, store.StoreId, entry => ShouldConsolidate(entry, store));
                if (candidates.Count == 0)
                {
                    continue;
                }

                var summaryContent = BuildSummaryContent(profile, store, candidates);
                var importance = Math.Clamp(candidates.Average(entry => entry.Importance), store.ImportanceFloor, 1.0);
                var summaryEntry = _storeService.CreateSummaryEntry(
                    profile.ProfileId,
                    store.PromoteToStoreId!,
                    $"Consolidated {candidates.Count} {store.MemoryType} memories",
                    summaryContent,
                    importance,
                    new[] { "consolidated", store.MemoryType, store.PromoteToStoreId! },
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["sourceStoreId"] = store.StoreId,
                        ["sourceCount"] = candidates.Count.ToString()
                    });

                var trace = new MemoryTrace
                {
                    Action = "consolidated",
                    Actor = "consolidation-worker",
                    Notes = $"Promoted to {store.PromoteToStoreId}",
                    Timestamp = DateTimeOffset.UtcNow
                };

                _storeService.TagEntries(profile.ProfileId, candidates.Select(entry => entry.EntryId), "consolidated");
                _storeService.AppendTrace(candidates.Select(entry => entry.EntryId), trace);

                RecordEvent(profile.ProfileId, store.StoreId, store.PromoteToStoreId!, summaryEntry.EntryId, candidates.Select(entry => entry.EntryId).ToList(), summaryContent, candidates.Count);
            }
        }

        await Task.CompletedTask;
    }

    public IReadOnlyCollection<MemoryConsolidationEvent> GetEvents()
    {
        return _events.Select(evt => evt.Clone()).ToArray();
    }

    private bool ShouldConsolidate(MemoryEntry entry, MemoryStoreDefinition storeDefinition)
    {
        if (entry.IsPinned)
        {
            return false;
        }

        if (entry.Importance >= Math.Max(storeDefinition.ImportanceFloor, _options.Simulation.AutoConsolidationImportanceThreshold))
        {
            return true;
        }

        var ageMinutes = (DateTimeOffset.UtcNow - entry.CreatedAt).TotalMinutes;
        return ageMinutes >= Math.Max(15, _options.Simulation.ShortTermRetentionMinutes / 2.0);
    }

    private static string BuildSummaryContent(MemoryProfileDefinition profile, MemoryStoreDefinition store, IReadOnlyCollection<MemoryEntry> entries)
    {
        var highlights = entries
            .OrderByDescending(entry => entry.Importance)
            .ThenByDescending(entry => entry.CreatedAt)
            .Take(5)
            .Select(entry => $"- {entry.Title}: {entry.Content.Substring(0, Math.Min(entry.Content.Length, 120))}")
            .ToList();

        return $"Profile: {profile.Name}\nSource Store: {store.StoreId}\nEntries Merged: {entries.Count}\nHighlights:\n{string.Join("\n", highlights)}";
    }

    private void RecordEvent(
        string profileId,
        string sourceStoreId,
        string targetStoreId,
        string targetEntryId,
        List<string> sourceEntryIds,
        string summary,
        int mergedCount)
    {
        var evt = new MemoryConsolidationEvent
        {
            ProfileId = profileId,
            SourceStoreId = sourceStoreId,
            TargetStoreId = targetStoreId,
            TargetEntryId = targetEntryId,
            SourceEntryIds = sourceEntryIds,
            EntriesMerged = mergedCount,
            Summary = summary,
            Strategy = "auto-consolidation",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _events.Enqueue(evt);
        TrimEvents();
    }

    private void TrimEvents()
    {
        while (_events.Count > _options.Simulation.MaxConsolidationEvents && _events.TryDequeue(out _))
        {
        }
    }
}
