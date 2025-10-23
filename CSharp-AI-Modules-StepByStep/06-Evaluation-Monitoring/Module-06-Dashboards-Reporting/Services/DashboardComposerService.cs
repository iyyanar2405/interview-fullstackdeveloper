using System.Globalization;
using Microsoft.Extensions.Options;
using Module_06_Dashboards_Reporting.Models;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class DashboardComposerService
{
	private readonly DashboardDefinitionService _definitions;
	private readonly MetricArchiveService _metrics;
	private readonly ILogger<DashboardComposerService> _logger;
 	private readonly DashboardOptions _options;

	public DashboardComposerService(
		DashboardDefinitionService definitions,
		MetricArchiveService metrics,
		IOptions<DashboardOptions> options,
		ILogger<DashboardComposerService> logger)
	{
		_definitions = definitions;
		_metrics = metrics;
		_options = options.Value;
		_logger = logger;
	}

	public DashboardSnapshot? Compose(string dashboardId)
	{
		var definition = _definitions.Get(dashboardId);
		if (definition is null)
		{
			_logger.LogWarning("Dashboard {DashboardId} not found", dashboardId);
			return null;
		}

		var snapshot = new DashboardSnapshot
		{
			DashboardId = definition.DashboardId,
			Name = definition.Name,
			GeneratedAt = DateTimeOffset.UtcNow,
			Kpis = ComposeKpis(definition).ToArray(),
			Widgets = definition.Widgets.Select(ComposeWidget).ToArray()
		};

		return snapshot;
	}

	private IEnumerable<KpiSnapshot> ComposeKpis(DashboardDefinition definition)
	{
		var candidates = definition.Widgets.Where(w => w.Highlight).ToList();
		if (candidates.Count == 0)
		{
			candidates = definition.Widgets.Take(3).ToList();
		}

		if (candidates.Count == 0 && _options.SeedKpis.Count > 0)
		{
			foreach (var seed in _options.SeedKpis)
			{
				yield return new KpiSnapshot
				{
					Name = seed.Name,
					Value = seed.Value,
					Delta = seed.Delta,
					Trend = seed.Trend
				};
			}
			yield break;
		}

		foreach (var widget in candidates)
		{
			var window = GetWindow(widget.TimeRange);
			var aggregate = _metrics.GetAggregate(widget.Metric, window);
			if (aggregate.Samples == 0)
			{
				continue;
			}

			yield return new KpiSnapshot
			{
				Name = widget.Title,
				Value = aggregate.Average,
				Delta = aggregate.Change,
				Trend = aggregate.Change switch
				{
					> 1 => "up",
					< -1 => "down",
					_ => "flat"
				}
			};
		}
	}

	private WidgetSnapshot ComposeWidget(DashboardWidget widget)
	{
		var window = GetWindow(widget.TimeRange);
		var series = _metrics.GetSeries(widget.Metric, window)
			.Select(p => new WidgetDataPoint { Timestamp = p.Timestamp, Value = Math.Round(p.Value, 2) })
			.ToList();

		var aggregate = _metrics.GetAggregate(widget.Metric, window);
		var latest = series.LastOrDefault();

		return new WidgetSnapshot
		{
			WidgetId = widget.WidgetId,
			Title = widget.Title,
			Type = widget.Type,
			LastUpdated = latest?.Timestamp ?? DateTimeOffset.UtcNow,
			Series = series,
			CurrentValue = latest?.Value,
			Delta = series.Count > 1 ? Math.Round(series[^1].Value - series[0].Value, 2) : null,
			Table = widget.Type == WidgetType.Table ? BuildTable(widget, window) : Array.Empty<WidgetTabularRow>(),
			Narrative = BuildNarrative(widget, aggregate)
		};
	}

	private IReadOnlyCollection<WidgetTabularRow> BuildTable(DashboardWidget widget, TimeSpan window)
	{
		var points = _metrics.GetSeries(widget.Metric, window);
		if (points.Count == 0)
		{
			return Array.Empty<WidgetTabularRow>();
		}

		var grouped = points
			.Where(p => p.Dimensions.Count > 0)
			.GroupBy(p => string.Join(';', p.Dimensions.OrderBy(k => k.Key).Select(kv => $"{kv.Key}:{kv.Value}")))
			.Select(g => new WidgetTabularRow
			{
				Columns = new Dictionary<string, object>
				{
					["segment"] = g.Key,
					["avg"] = Math.Round(g.Average(p => p.Value), 2),
					["max"] = Math.Round(g.Max(p => p.Value), 2),
					["samples"] = g.Count()
				}
			})
			.ToList();

		return grouped.Count == 0 ?
			new List<WidgetTabularRow>
			{
				new()
				{
					Columns = new Dictionary<string, object>
					{
						["segment"] = "overall",
						["avg"] = Math.Round(points.Average(p => p.Value), 2),
						["max"] = Math.Round(points.Max(p => p.Value), 2),
						["samples"] = points.Count
					}
				}
			}
			: grouped;
	}

	private static string BuildNarrative(DashboardWidget widget, MetricAggregate aggregate)
	{
		if (aggregate.Samples == 0)
		{
			return "No data collected for the selected window.";
		}

		var comparison = aggregate.Change switch
		{
			> 5 => "rising sharply",
			> 1 => "trending upward",
			< -5 => "dropping sharply",
			< -1 => "trending downward",
			_ => "stable"
		};

		if (widget.Thresholds.TryGetValue("warning", out var warning) && aggregate.Maximum > warning)
		{
			comparison = "breaching warning threshold";
		}

		return $"Average {widget.Metric} is {aggregate.Average:F2} ({comparison}).";
	}

	private static TimeSpan GetWindow(string? window)
	{
		if (string.IsNullOrWhiteSpace(window))
		{
			return TimeSpan.FromHours(1);
		}

		if (TryParseWindow(window, out var parsed))
		{
			return parsed;
		}

		return TimeSpan.FromHours(1);
	}

	private static bool TryParseWindow(string input, out TimeSpan window)
	{
		window = TimeSpan.Zero;
		if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
		{
			return false;
		}
		var suffix = input[^1];
		if (!double.TryParse(input[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
		{
			return false;
		}

		window = suffix switch
		{
			'h' or 'H' => TimeSpan.FromHours(value),
			'm' or 'M' => TimeSpan.FromMinutes(value),
			'd' or 'D' => TimeSpan.FromDays(value),
			_ => TimeSpan.FromHours(value)
		};

		return true;
	}
}
