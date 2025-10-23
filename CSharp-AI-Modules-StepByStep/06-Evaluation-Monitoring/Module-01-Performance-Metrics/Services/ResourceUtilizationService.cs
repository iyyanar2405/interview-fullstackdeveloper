using System.Collections.Concurrent;
using Module_01_Performance_Metrics.Models;

namespace Module_01_Performance_Metrics.Services;

public sealed class ResourceUtilizationService
{
    private readonly ConcurrentQueue<ResourceSnapshot> _samples = new();
    private readonly int _maxSamples;

    public ResourceUtilizationService(ResourceSamplingOptions options)
    {
        _maxSamples = Math.Max(10, options.HistorySize);
    }

    public void RecordSample(ResourceSnapshot snapshot)
    {
        _samples.Enqueue(snapshot);
        while (_samples.Count > _maxSamples && _samples.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyCollection<ResourceSnapshot> GetSamples(TimeSpan window)
    {
        var cutoff = DateTimeOffset.UtcNow - window;
        return _samples.Where(s => s.Timestamp >= cutoff).ToArray();
    }

    public ResourceSummary GetSummary(TimeSpan window)
    {
        var samples = GetSamples(window);
        if (samples.Count == 0)
        {
            return new ResourceSummary
            {
                Window = window,
                AverageCpuUsage = 0,
                PeakCpuUsage = 0,
                AverageMemoryUsageMb = 0,
                PeakMemoryUsageMb = 0,
                AverageManagedMemoryMb = 0,
                PeakActiveRequests = 0,
                AverageActiveRequests = 0,
                AverageThreads = 0,
                PeakThreads = 0
            };
        }

        return new ResourceSummary
        {
            Window = window,
            AverageCpuUsage = samples.Average(s => s.CpuUsagePercentage),
            PeakCpuUsage = samples.Max(s => s.CpuUsagePercentage),
            AverageMemoryUsageMb = samples.Average(s => s.MemoryUsageMb),
            PeakMemoryUsageMb = samples.Max(s => s.MemoryUsageMb),
            AverageManagedMemoryMb = samples.Average(s => s.ManagedMemoryMb),
            PeakActiveRequests = samples.Max(s => s.ActiveRequests),
            AverageActiveRequests = samples.Average(s => s.ActiveRequests),
            AverageThreads = samples.Average(s => s.ActiveThreads),
            PeakThreads = samples.Max(s => s.ActiveThreads)
        };
    }
}
