namespace Module_06_Dashboards_Reporting.Models;

public enum WidgetType
{
	Timeseries,
	Number,
	Distribution,
	Table,
	Markdown
}

public sealed class DashboardWidget
{
	public string WidgetId { get; set; } = Guid.NewGuid().ToString("N");
	public WidgetType Type { get; set; } = WidgetType.Number;
	public string Title { get; set; } = string.Empty;
	public string Metric { get; set; } = string.Empty;
	public string Visualization { get; set; } = "line";
	public string TimeRange { get; set; } = "1h";
	public Dictionary<string, string> Filters { get; set; } = new();
	public Dictionary<string, double> Thresholds { get; set; } = new();
	public bool Highlight { get; set; }
		= false;
}

public sealed class DashboardDefinition
{
	public string DashboardId { get; set; } = Guid.NewGuid().ToString("N");
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<string> Tags { get; set; } = new();
	public List<DashboardWidget> Widgets { get; set; } = new();
	public int RefreshSeconds { get; set; } = 30;
	public bool IsDefault { get; set; } = false;
}

public sealed class WidgetDataPoint
{
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
	public double Value { get; set; }
		= 0;
}

public sealed class WidgetTabularRow
{
	public Dictionary<string, object> Columns { get; set; } = new();
}

public sealed class WidgetSnapshot
{
	public string WidgetId { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public WidgetType Type { get; set; } = WidgetType.Number;
	public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
	public IReadOnlyCollection<WidgetDataPoint> Series { get; set; } = Array.Empty<WidgetDataPoint>();
	public double? CurrentValue { get; set; }
		= null;
	public double? Delta { get; set; }
		= null;
	public IReadOnlyCollection<WidgetTabularRow> Table { get; set; } = Array.Empty<WidgetTabularRow>();
	public string Narrative { get; set; } = string.Empty;
}

public sealed class KpiSnapshot
{
	public string Name { get; set; } = string.Empty;
	public double Value { get; set; }
		= 0;
	public double Delta { get; set; }
		= 0;
	public string Trend { get; set; } = "flat";
}

public sealed class DashboardSnapshot
{
	public string DashboardId { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
	public IReadOnlyCollection<KpiSnapshot> Kpis { get; set; } = Array.Empty<KpiSnapshot>();
	public IReadOnlyCollection<WidgetSnapshot> Widgets { get; set; } = Array.Empty<WidgetSnapshot>();
}

public sealed class MetricTrendPoint
{
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
	public double Value { get; set; }
		= 0;
	public Dictionary<string, string> Dimensions { get; set; } = new();
}

public sealed class MetricAggregate
{
	public string Metric { get; set; } = string.Empty;
	public double Average { get; set; }
		= 0;
	public double Minimum { get; set; }
		= 0;
	public double Maximum { get; set; }
		= 0;
	public double Change { get; set; }
		= 0;
	public int Samples { get; set; }
		= 0;
}

public sealed class ReportRequest
{
	public DateTimeOffset? RangeStart { get; set; }
		= null;
	public DateTimeOffset? RangeEnd { get; set; }
		= null;
	public List<string> Metrics { get; set; } = new();
	public List<string> Dimensions { get; set; } = new();
	public string GroupBy { get; set; } = "hour";
	public string Format { get; set; } = "json";
}

public sealed class ReportSection
{
	public string Title { get; set; } = string.Empty;
	public string Summary { get; set; } = string.Empty;
	public List<MetricTrendPoint> Points { get; set; } = new();
	public List<WidgetTabularRow> Table { get; set; } = new();
}

public sealed class ReportDocument
{
	public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
	public string Title { get; set; } = string.Empty;
	public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
	public List<KpiSnapshot> Highlights { get; set; } = new();
	public List<ReportSection> Sections { get; set; } = new();
	public Dictionary<string, string> Metadata { get; set; } = new();
}

public sealed class ExecutiveSummary
{
	public string Overview { get; set; } = string.Empty;
	public List<string> Strengths { get; set; } = new();
	public List<string> Risks { get; set; } = new();
	public List<string> Opportunities { get; set; } = new();
	public string NextActions { get; set; } = string.Empty;
}

public sealed class DashboardOptions
{
	public List<DashboardDefinition> SeedDashboards { get; set; } = new();
	public List<KpiSnapshot> SeedKpis { get; set; } = new();
	public int SnapshotHistory { get; set; }
		= 240;
	public int TrendHistoryMinutes { get; set; } = 720;
}

public sealed class ReportingOptions
{
	public List<string> AllowedFormats { get; set; } = new() { "json", "pdf", "csv" };
	public string DefaultGroupBy { get; set; } = "hour";
	public int MaxRangeDays { get; set; } = 30;
}

public sealed class MetricSimulationOptions
{
	public List<string> Metrics { get; set; } = new();
	public Dictionary<string, double> Baseline { get; set; } = new();
	public Dictionary<string, double> Volatility { get; set; } = new();
	public int IntervalSeconds { get; set; } = 15;
}