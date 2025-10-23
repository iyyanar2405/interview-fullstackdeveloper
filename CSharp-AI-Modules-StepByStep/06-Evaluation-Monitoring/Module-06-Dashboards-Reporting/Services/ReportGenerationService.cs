using System.Globalization;
using Module_06_Dashboards_Reporting.Models;
using Microsoft.Extensions.Options;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class ReportGenerationService
{
	private readonly MetricArchiveService _metrics;
	private readonly DashboardDefinitionService _definitions;
	private readonly ReportingOptions _options;
	private readonly ILogger<ReportGenerationService> _logger;

	public ReportGenerationService(
		MetricArchiveService metrics,
		DashboardDefinitionService definitions,
		IOptions<ReportingOptions> options,
		ILogger<ReportGenerationService> logger)
	{
		_metrics = metrics;
		_definitions = definitions;
		_options = options.Value;
		_logger = logger;
	}

	public ReportDocument Generate(ReportRequest request)
	{
		var metrics = ResolveMetrics(request);
		var window = DetermineWindow(request);
		var (rangeStart, rangeEnd) = NormalizeRange(request);
		var format = NormalizeFormat(request.Format);
		var sections = new List<ReportSection>();
		var highlights = new List<KpiSnapshot>();

		foreach (var metric in metrics)
		{
			var series = request.RangeStart.HasValue || request.RangeEnd.HasValue
				? _metrics.GetRange(metric, rangeStart, rangeEnd)
				: _metrics.GetSeries(metric, window);

			if (series.Count == 0)
			{
				continue;
			}

			var aggregate = CalculateAggregate(metric, series);
			highlights.Add(new KpiSnapshot
			{
				Name = metric,
				Value = aggregate.Average,
				Delta = aggregate.Change,
				Trend = aggregate.Change switch
				{
					> 1 => "up",
					< -1 => "down",
					_ => "flat"
				}
			});

			sections.Add(new ReportSection
			{
				Title = $"Metric: {metric}",
				Summary = BuildSummary(metric, aggregate),
				Points = series.ToList(),
				Table = BuildTabular(series, request.GroupBy)
			});
		}

		var document = new ReportDocument
		{
			Title = $"AI Monitoring Report ({DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm} UTC)",
			GeneratedAt = DateTimeOffset.UtcNow,
			Highlights = highlights,
			Sections = sections,
			Metadata = new Dictionary<string, string>
			{
				["format"] = format,
				["groupBy"] = string.IsNullOrWhiteSpace(request.GroupBy) ? _options.DefaultGroupBy : request.GroupBy,
				["metrics"] = string.Join(',', metrics)
			}
		};

		_logger.LogInformation("Generated report with {SectionCount} sections", sections.Count);
		return document;
	}

	private IReadOnlyCollection<string> ResolveMetrics(ReportRequest request)
	{
		if (request.Metrics.Count > 0)
		{
			return request.Metrics;
		}

		return _definitions.GetAll()
			.SelectMany(d => d.Widgets)
			.Select(w => w.Metric)
			.Where(m => !string.IsNullOrWhiteSpace(m))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Take(12)
			.ToArray();
	}

	private TimeSpan DetermineWindow(ReportRequest request)
	{
		if (request.RangeStart.HasValue && request.RangeEnd.HasValue)
		{
			var delta = request.RangeEnd.Value - request.RangeStart.Value;
			var max = TimeSpan.FromDays(Math.Max(1, _options.MaxRangeDays));
			if (delta > max)
			{
				delta = max;
			}
			return delta > TimeSpan.Zero ? delta : TimeSpan.FromHours(6);
		}

		return TimeSpan.FromHours(6);
	}

	private (DateTimeOffset Start, DateTimeOffset End) NormalizeRange(ReportRequest request)
	{
		var end = request.RangeEnd ?? DateTimeOffset.UtcNow;
		var start = request.RangeStart ?? end - TimeSpan.FromHours(6);
		var max = TimeSpan.FromDays(Math.Max(1, _options.MaxRangeDays));
		if (end - start > max)
		{
			start = end - max;
		}

		return (start, end);
	}

	private string NormalizeFormat(string? format)
	{
		if (string.IsNullOrWhiteSpace(format))
		{
			return _options.AllowedFormats.First();
		}

		var normalized = format.Trim().ToLowerInvariant();
		return _options.AllowedFormats.Contains(normalized) ? normalized : _options.AllowedFormats.First();
	}

	private static MetricAggregate CalculateAggregate(string metric, IReadOnlyList<MetricTrendPoint> points)
	{
		var aggregate = new MetricAggregate
		{
			Metric = metric,
			Samples = points.Count,
			Average = Math.Round(points.Average(p => p.Value), 2),
			Minimum = Math.Round(points.Min(p => p.Value), 2),
			Maximum = Math.Round(points.Max(p => p.Value), 2),
			Change = Math.Round(points[^1].Value - points[0].Value, 2)
		};

		return aggregate;
	}

	private static string BuildSummary(string metric, MetricAggregate aggregate)
	{
		if (aggregate.Samples == 0)
		{
			return $"No data available for {metric}.";
		}

		var trend = aggregate.Change switch
		{
			> 5 => "significant increase",
			> 1 => "moderate increase",
			< -5 => "significant decrease",
			< -1 => "moderate decrease",
			_ => "stable"
		};

		return $"Average {metric} {aggregate.Average.ToString("F2", CultureInfo.InvariantCulture)} with {trend}.";
	}

	private static List<WidgetTabularRow> BuildTabular(IReadOnlyList<MetricTrendPoint> points, string? groupBy)
	{
		var grouping = NormalizeGroupBy(groupBy);
		return grouping switch
		{
			"hour" => Group(points, p => new DateTimeOffset(p.Timestamp.Year, p.Timestamp.Month, p.Timestamp.Day, p.Timestamp.Hour, 0, 0, TimeSpan.Zero)),
			"day" => Group(points, p => new DateTimeOffset(p.Timestamp.Year, p.Timestamp.Month, p.Timestamp.Day, 0, 0, 0, TimeSpan.Zero)),
			_ => Group(points, _ => DateTimeOffset.FromUnixTimeSeconds(0))
		};
	}

	private static string NormalizeGroupBy(string? groupBy)
	{
		return string.IsNullOrWhiteSpace(groupBy) ? "hour" : groupBy.Trim().ToLowerInvariant();
	}

	private static List<WidgetTabularRow> Group(IReadOnlyList<MetricTrendPoint> points, Func<MetricTrendPoint, DateTimeOffset> selector)
	{
		return points
			.GroupBy(selector)
			.Select(g => new WidgetTabularRow
			{
				Columns = new Dictionary<string, object>
				{
					["timestamp"] = g.Key.ToString("yyyy-MM-dd HH:mm"),
					["avg"] = Math.Round(g.Average(p => p.Value), 2),
					["min"] = Math.Round(g.Min(p => p.Value), 2),
					["max"] = Math.Round(g.Max(p => p.Value), 2),
					["samples"] = g.Count()
				}
			})
			.ToList();
	}
}
