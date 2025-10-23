using System.Collections.Concurrent;
using Module_06_Dashboards_Reporting.Models;
using Microsoft.Extensions.Options;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class DashboardSnapshotService
{
	private readonly ConcurrentDictionary<string, ConcurrentQueue<DashboardSnapshot>> _snapshots = new(StringComparer.OrdinalIgnoreCase);
	private readonly int _maxHistory;

	public DashboardSnapshotService(IOptions<DashboardOptions> options)
	{
		_maxHistory = Math.Max(20, options.Value.SnapshotHistory);
	}

	public void RecordSnapshot(DashboardSnapshot snapshot)
	{
		var history = _snapshots.GetOrAdd(snapshot.DashboardId, _ => new ConcurrentQueue<DashboardSnapshot>());
		history.Enqueue(snapshot);

		while (history.Count > _maxHistory && history.TryDequeue(out _))
		{
		}
	}

	public DashboardSnapshot? GetLatest(string dashboardId)
	{
		if (!_snapshots.TryGetValue(dashboardId, out var history))
		{
			return null;
		}

		return history.LastOrDefault();
	}

	public IReadOnlyCollection<DashboardSnapshot> GetHistory(string dashboardId, int count)
	{
		if (!_snapshots.TryGetValue(dashboardId, out var history))
		{
			return Array.Empty<DashboardSnapshot>();
		}

		count = Math.Clamp(count, 1, _maxHistory);
		return history.Reverse().Take(count).ToArray();
	}

	public IReadOnlyCollection<KpiSnapshot> GetLatestKpis(string dashboardId)
	{
		return GetLatest(dashboardId)?.Kpis ?? Array.Empty<KpiSnapshot>();
	}
}
