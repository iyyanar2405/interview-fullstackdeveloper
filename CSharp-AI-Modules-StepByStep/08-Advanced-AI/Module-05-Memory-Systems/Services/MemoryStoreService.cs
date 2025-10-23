using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_05_Memory_Systems.Models;

namespace Module_05_Memory_Systems.Services;

public sealed class MemoryStoreService
{
    private sealed class StoreBucket
    {
        public StoreBucket()
        {
            Entries = new ConcurrentDictionary<string, MemoryEntry>(StringComparer.OrdinalIgnoreCase);
        }

        public ConcurrentDictionary<string, MemoryEntry> Entries { get; }
        public object Gate { get; } = new();
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.MinValue;
    }

    private readonly MemoryProfileCatalogService _catalog;
    private readonly MemorySystemsOptions _options;
    private readonly ConcurrentDictionary<string, StoreBucket> _stores = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _entryIndex = new(StringComparer.OrdinalIgnoreCase);

    public MemoryStoreService(MemoryProfileCatalogService catalog, IOptions<MemorySystemsOptions> options)
    {
        _catalog = catalog;
        _options = options.Value;

        foreach (var profile in options.Value.Profiles)
        {
            foreach (var store in profile.Stores)
            {
                var key = BuildStoreKey(profile.ProfileId, store.StoreId);
                _stores.TryAdd(key, new StoreBucket());
            }
        }
    }

    public MemoryEntry IngestMemory(MemoryIngestionRequest request)
    {
        if (!_catalog.TryGetStoreDefinition(request.ProfileId, request.StoreId, out var storeDefinition))
        {
            throw new InvalidOperationException($"Unknown profile/store combination '{request.ProfileId}/{request.StoreId}'.");
        }

        var key = BuildStoreKey(request.ProfileId, request.StoreId);
        var bucket = _stores.GetOrAdd(key, _ => new StoreBucket());
        var now = DateTimeOffset.UtcNow;

        var content = request.Content ?? string.Empty;
        var requestedTags = request.Tags ?? new List<string>();
        var requestedMetadata = request.Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var tags = MergeTags(storeDefinition.DefaultTags, requestedTags);
        var importance = Math.Max(storeDefinition.ImportanceFloor, request.Importance);
        var title = string.IsNullOrWhiteSpace(request.Title)
            ? content[..Math.Min(content.Length, 60)]
            : request.Title;
        var entry = new MemoryEntry
        {
            ProfileId = request.ProfileId,
            StoreId = request.StoreId,
            MemoryType = storeDefinition.MemoryType,
            Title = title,
            Content = content,
            Importance = importance,
            Tags = tags,
            Metadata = new Dictionary<string, string>(requestedMetadata, StringComparer.OrdinalIgnoreCase),
            CreatedAt = now,
            LastAccessedAt = now,
            Traces =
            {
                new MemoryTrace
                {
                    Action = "ingest",
                    Notes = "Ingested via API",
                    Timestamp = now
                }
            }
        };

        lock (bucket.Gate)
        {
            bucket.Entries[entry.EntryId] = entry;
            bucket.LastUpdated = now;
            _entryIndex[entry.EntryId] = key;
            EnforceCapacity(bucket, storeDefinition);
        }

        return entry.Clone();
    }

    public IReadOnlyCollection<MemoryEntry> IngestBatch(MemoryBatchIngestionRequest request)
    {
        var responses = new List<MemoryEntry>();
        foreach (var item in request.Items)
        {
            var ingestion = new MemoryIngestionRequest
            {
                ProfileId = request.ProfileId,
                StoreId = request.StoreId,
                Title = item.Title,
                Content = item.Content,
                Importance = item.Importance,
                Tags = (item.Tags ?? new List<string>()).ToList(),
                Metadata = new Dictionary<string, string>(item.Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase)
            };

            responses.Add(IngestMemory(ingestion));
        }

        return responses;
    }

    public IReadOnlyCollection<MemoryEntry> QueryMemories(MemoryRetrievalRequest request)
    {
        var targetStoreIds = request.StoreIds.Count > 0
            ? request.StoreIds
            : _catalog.GetStores(request.ProfileId).Select(store => store.StoreId).ToList();

        var typeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in request.MemoryTypes)
        {
            if (MemoryTypes.TryNormalize(type, out var normalized))
            {
                typeFilter.Add(normalized);
            }
        }

        var searchText = request.SearchText?.Trim();
        var hasSearch = !string.IsNullOrWhiteSpace(searchText);
        var tagFilter = request.Tags.Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var results = new List<MemoryEntry>();

        foreach (var storeId in targetStoreIds)
        {
            var key = BuildStoreKey(request.ProfileId, storeId);
            if (!_stores.TryGetValue(key, out var bucket))
            {
                continue;
            }

            foreach (var entry in bucket.Entries.Values)
            {
                if (entry.ProfileId != request.ProfileId)
                {
                    continue;
                }

                if (typeFilter.Count > 0 && !typeFilter.Contains(entry.MemoryType))
                {
                    continue;
                }

                if (entry.Importance < request.MinImportance)
                {
                    continue;
                }

                if (tagFilter.Count > 0 && !entry.Tags.Any(tagFilter.Contains))
                {
                    continue;
                }

                if (hasSearch && !entry.Content.Contains(searchText!, StringComparison.OrdinalIgnoreCase) && !entry.Title.Contains(searchText!, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var clone = entry.Clone();
                if (!request.IncludeTraces)
                {
                    clone.Traces.Clear();
                }

                results.Add(clone);
            }
        }

        return results;
    }

    public void RegisterRecall(string entryId)
    {
        if (!_entryIndex.TryGetValue(entryId, out var key))
        {
            return;
        }

        if (!_stores.TryGetValue(key, out var bucket))
        {
            return;
        }

        if (!bucket.Entries.TryGetValue(entryId, out var entry))
        {
            return;
        }

        lock (bucket.Gate)
        {
            entry.RecallCount += 1;
            entry.LastAccessedAt = DateTimeOffset.UtcNow;
            entry.Traces.Add(new MemoryTrace
            {
                Action = "recall",
                Notes = "Retrieved via API",
                Timestamp = entry.LastAccessedAt
            });
        }
    }

    public MemoryStoreSnapshot GetStoreSnapshot(string profileId, string storeId)
    {
        var snapshot = new MemoryStoreSnapshot
        {
            ProfileId = profileId,
            StoreId = storeId
        };

        var key = BuildStoreKey(profileId, storeId);
        if (!_stores.TryGetValue(key, out var bucket))
        {
            return snapshot;
        }

        var entries = bucket.Entries.Values.ToList();
        if (entries.Count == 0)
        {
            return snapshot;
        }

        snapshot.MemoryType = entries[0].MemoryType;
        snapshot.TotalEntries = entries.Count;
        snapshot.AverageImportance = Math.Round(entries.Average(entry => entry.Importance), 3);
        snapshot.LastUpdated = bucket.LastUpdated;
        snapshot.RecentEntryCount = entries.Count(entry => entry.CreatedAt >= DateTimeOffset.UtcNow.AddMinutes(-30));

        foreach (var tag in entries.SelectMany(entry => entry.Tags))
        {
            snapshot.TagFrequency.TryGetValue(tag, out var current);
            snapshot.TagFrequency[tag] = current + 1;
        }

        return snapshot;
    }

    public MemoryProfileSummary GetProfileSummary(string profileId)
    {
        var storeDefinitions = _catalog.GetStores(profileId);
        var summary = new MemoryProfileSummary
        {
            ProfileId = profileId
        };

        foreach (var store in storeDefinitions)
        {
            var snapshot = GetStoreSnapshot(profileId, store.StoreId);
            summary.Stores.Add(snapshot);
        }

        if (summary.Stores.Count > 0)
        {
            summary.TotalEntries = summary.Stores.Sum(store => store.TotalEntries);
            var weighted = summary.Stores.Where(store => store.TotalEntries > 0)
                .Select(store => store.AverageImportance * store.TotalEntries)
                .Sum();
            var divisor = summary.Stores.Where(store => store.TotalEntries > 0).Sum(store => store.TotalEntries);
            summary.AverageImportance = divisor > 0 ? Math.Round(weighted / divisor, 3) : 0.0;
        }

        return summary;
    }

    public IReadOnlyCollection<MemoryEntry> GetEntriesForStore(string profileId, string storeId, Func<MemoryEntry, bool> predicate)
    {
        var key = BuildStoreKey(profileId, storeId);
        if (!_stores.TryGetValue(key, out var bucket))
        {
            return Array.Empty<MemoryEntry>();
        }

        return bucket.Entries.Values.Where(predicate).Select(entry => entry.Clone()).ToArray();
    }

    public MemoryEntry CreateSummaryEntry(string profileId, string storeId, string title, string content, double importance, IEnumerable<string> tags, IDictionary<string, string>? metadata = null)
    {
        var tagList = tags?.ToList() ?? new List<string>();
        var request = new MemoryIngestionRequest
        {
            ProfileId = profileId,
            StoreId = storeId,
            Title = title,
            Content = content,
            Importance = importance,
            Tags = tagList,
            Metadata = metadata is null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase)
        };

        return IngestMemory(request);
    }

    public void TagEntries(string profileId, IEnumerable<string> entryIds, string tag)
    {
        foreach (var entryId in entryIds)
        {
            if (!_entryIndex.TryGetValue(entryId, out var key))
            {
                continue;
            }

            if (!_stores.TryGetValue(key, out var bucket))
            {
                continue;
            }

            if (!bucket.Entries.TryGetValue(entryId, out var entry))
            {
                continue;
            }

            lock (bucket.Gate)
            {
                if (!entry.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    entry.Tags.Add(tag);
                }
            }
        }
    }

    public void AppendTrace(IEnumerable<string> entryIds, MemoryTrace trace)
    {
        foreach (var entryId in entryIds)
        {
            if (!_entryIndex.TryGetValue(entryId, out var key))
            {
                continue;
            }

            if (!_stores.TryGetValue(key, out var bucket))
            {
                continue;
            }

            if (!bucket.Entries.TryGetValue(entryId, out var entry))
            {
                continue;
            }

            lock (bucket.Gate)
            {
                entry.Traces.Add(new MemoryTrace
                {
                    Action = trace.Action,
                    Actor = trace.Actor,
                    Notes = trace.Notes,
                    Timestamp = trace.Timestamp
                });
            }
        }
    }

    private void EnforceCapacity(StoreBucket bucket, MemoryStoreDefinition definition)
    {
        var globalCap = _options.Simulation.MaxEntriesPerStore > 0 ? _options.Simulation.MaxEntriesPerStore : int.MaxValue;
        var maxEntries = Math.Min(
            definition.Capacity > 0 ? definition.Capacity : int.MaxValue,
            globalCap);

        if (bucket.Entries.Count <= maxEntries)
        {
            return;
        }

        var excess = bucket.Entries.Count - maxEntries;
        var ordered = bucket.Entries.Values
            .OrderBy(entry => entry.IsPinned)
            .ThenBy(entry => entry.Importance)
            .ThenBy(entry => entry.CreatedAt)
            .Take(excess)
            .ToList();

        foreach (var entry in ordered)
        {
            bucket.Entries.TryRemove(entry.EntryId, out _);
            _entryIndex.TryRemove(entry.EntryId, out _);
        }
    }

    private static string BuildStoreKey(string profileId, string storeId)
    {
        return $"{profileId.Trim().ToLowerInvariant()}::{storeId.Trim().ToLowerInvariant()}";
    }

    private static List<string> MergeTags(IEnumerable<string> defaults, IEnumerable<string> additional)
    {
        return defaults.Concat(additional)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
