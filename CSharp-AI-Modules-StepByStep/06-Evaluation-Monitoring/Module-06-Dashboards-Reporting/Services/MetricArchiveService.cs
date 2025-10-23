using System.Collections.Concurrent;
using Module_06_Dashboards_Reporting.Models;
using Microsoft.Extensions.Options;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class MetricArchiveService
{
	private readonly ConcurrentDictionary<string, LinkedList<MetricTrendPoint>> _series = new(StringComparer.OrdinalIgnoreCase);
	private readonly int _maxPoints;
	private readonly TimeSpan _retention;
	private readonly ILogger<MetricArchiveService> _logger;

	public MetricArchiveService(IOptions<DashboardOptions> options, ILogger<MetricArchiveService> logger)
	{
		_maxPoints = Math.Max(120, options.Value.SnapshotHistory);
		_retention = TimeSpan.FromMinutes(Math.Max(60, options.Value.TrendHistoryMinutes));
		_logger = logger;
	}

	public void Record(string metric, double value, IDictionary<string, string>? dimensions = null)
	{
		if (string.IsNullOrWhiteSpace(metric))
		{
			return;
		}

		var point = new MetricTrendPoint
		{
			Timestamp = DateTimeOffset.UtcNow,
			Value = value,
			Dimensions = dimensions is null ? new Dictionary<string, string>() : new Dictionary<string, string>(dimensions)
		};

		var series = _series.GetOrAdd(metric, _ => new LinkedList<MetricTrendPoint>());

		lock (series)
		{
			series.AddLast(point);
			TrimSeries(series);
		}
	}

	public IReadOnlyList<MetricTrendPoint> GetSeries(string metric, TimeSpan range)
	{
		if (!_series.TryGetValue(metric, out var series))
		{
			return Array.Empty<MetricTrendPoint>();
		}

		var cutoff = DateTimeOffset.UtcNow - range;
		lock (series)
		{
			return series.Where(p => p.Timestamp >= cutoff).ToList();
		}
	}

	public IReadOnlyList<MetricTrendPoint> GetRange(string metric, DateTimeOffset? start, DateTimeOffset? end)
	{
		if (!_series.TryGetValue(metric, out var series))
		{
			return Array.Empty<MetricTrendPoint>();
		}

		var effectiveStart = start ?? DateTimeOffset.UtcNow - _retention;
		var effectiveEnd = end ?? DateTimeOffset.UtcNow;

		lock (series)
		{
			return series.Where(p => p.Timestamp >= effectiveStart && p.Timestamp <= effectiveEnd).ToList();
		}
	}

	public MetricAggregate GetAggregate(string metric, TimeSpan range)
	{
		var points = GetSeries(metric, range);
		if (points.Count == 0)
		{
			return new MetricAggregate { Metric = metric };
		}

		var average = points.Average(p => p.Value);
		var min = points.Min(p => p.Value);
		var max = points.Max(p => p.Value);
		var change = points[^1].Value - points[0].Value;

		return new MetricAggregate
		{
			Metric = metric,
			Average = Math.Round(average, 2),
			Minimum = Math.Round(min, 2),
			Maximum = Math.Round(max, 2),
			Change = Math.Round(change, 2),
			Samples = points.Count
		};
	}

	public MetricTrendPoint? GetLatest(string metric)
	{
		if (!_series.TryGetValue(metric, out var series))
		{
			return null;
		}

		lock (series)
		{
			return series.LastOrDefault();
		}
	}

	private void TrimSeries(LinkedList<MetricTrendPoint> series)
	{
		while (series.Count > _maxPoints)
		{
			series.RemoveFirst();
		}

		var cutoff = DateTimeOffset.UtcNow - _retention;
		while (series.First is { Value: { } head } && head.Timestamp < cutoff)
		{
			series.RemoveFirst();
		}
	}
}
