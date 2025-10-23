using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_05_Memory_Systems.Models;

namespace Module_05_Memory_Systems.Services;

public sealed class MemoryProfileCatalogService
{
    private readonly ConcurrentDictionary<string, MemoryProfileDefinition> _profiles = new(StringComparer.OrdinalIgnoreCase);

    public MemoryProfileCatalogService(IOptions<MemorySystemsOptions> options)
    {
        foreach (var profile in options.Value.Profiles)
        {
            _profiles[profile.ProfileId] = Clone(profile);
        }
    }

    public IReadOnlyCollection<MemoryProfileDefinition> GetProfiles()
    {
        return _profiles.Values.Select(Clone).ToArray();
    }

    public MemoryProfileDefinition? GetProfile(string profileId)
    {
        return _profiles.TryGetValue(profileId, out var profile) ? Clone(profile) : null;
    }

    public bool TryGetStoreDefinition(string profileId, string storeId, out MemoryStoreDefinition store)
    {
        store = null!;
        if (!_profiles.TryGetValue(profileId, out var profile))
        {
            return false;
        }

        var match = profile.Stores.FirstOrDefault(s => string.Equals(s.StoreId, storeId, StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            return false;
        }

        store = Clone(match);
        return true;
    }

    public IReadOnlyCollection<MemoryStoreDefinition> GetStores(string profileId)
    {
        if (!_profiles.TryGetValue(profileId, out var profile))
        {
            return Array.Empty<MemoryStoreDefinition>();
        }

        return profile.Stores.Select(Clone).ToArray();
    }

    private static MemoryProfileDefinition Clone(MemoryProfileDefinition profile)
    {
        return new MemoryProfileDefinition
        {
            ProfileId = profile.ProfileId,
            Name = profile.Name,
            Description = profile.Description,
            DefaultTags = profile.DefaultTags.ToList(),
            RetrievalStrategies = profile.RetrievalStrategies.ToList(),
            Stores = profile.Stores.Select(Clone).ToList(),
            Retention = new MemoryRetentionPolicy
            {
                ShortTermMinutes = profile.Retention.ShortTermMinutes,
                LongTermDays = profile.Retention.LongTermDays,
                EpisodicDays = profile.Retention.EpisodicDays
            }
        };
    }

    private static MemoryStoreDefinition Clone(MemoryStoreDefinition store)
    {
        return new MemoryStoreDefinition
        {
            StoreId = store.StoreId,
            MemoryType = store.MemoryType,
            Description = store.Description,
            Capacity = store.Capacity,
            ImportanceFloor = store.ImportanceFloor,
            EnableAutoSummaries = store.EnableAutoSummaries,
            PromoteToStoreId = store.PromoteToStoreId,
            DefaultTags = store.DefaultTags.ToList()
        };
    }
}
