using System.Collections.Concurrent;
using Module_06_Dashboards_Reporting.Models;
using Microsoft.Extensions.Options;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class DashboardDefinitionService
{
	private readonly ConcurrentDictionary<string, DashboardDefinition> _dashboards = new(StringComparer.OrdinalIgnoreCase);
	private readonly ILogger<DashboardDefinitionService> _logger;

	public DashboardDefinitionService(IOptions<DashboardOptions> options, ILogger<DashboardDefinitionService> logger)
	{
		_logger = logger;
		foreach (var seed in options.Value.SeedDashboards)
		{
			var clone = Clone(seed);
			_dashboards[clone.DashboardId] = clone;
		}
	}

	public IReadOnlyCollection<DashboardDefinition> GetAll()
	{
		return _dashboards.Values
			.Select(Clone)
			.OrderByDescending(d => d.IsDefault)
			.ThenBy(d => d.Name)
			.ToArray();
	}

	public DashboardDefinition? Get(string dashboardId)
	{
		if (string.IsNullOrWhiteSpace(dashboardId))
		{
			return null;
		}

		return _dashboards.TryGetValue(dashboardId, out var definition) ? Clone(definition) : null;
	}

	public DashboardDefinition Add(DashboardDefinition definition)
	{
		var created = Clone(definition);
		created.DashboardId = string.IsNullOrWhiteSpace(created.DashboardId)
			? Guid.NewGuid().ToString("N")
			: created.DashboardId;

		_dashboards[created.DashboardId] = created;
		_logger.LogInformation("Dashboard {DashboardName} registered with {WidgetCount} widgets", created.Name, created.Widgets.Count);
		return Clone(created);
	}

	public DashboardDefinition? Update(string dashboardId, DashboardDefinition definition)
	{
		if (!_dashboards.ContainsKey(dashboardId))
		{
			return null;
		}

		var updated = Clone(definition);
		updated.DashboardId = dashboardId;
		_dashboards[dashboardId] = updated;
		_logger.LogInformation("Dashboard {DashboardId} updated", dashboardId);
		return Clone(updated);
	}

	public bool Delete(string dashboardId)
	{
		return _dashboards.TryRemove(dashboardId, out _);
	}

	private static DashboardDefinition Clone(DashboardDefinition source)
	{
		return new DashboardDefinition
		{
			DashboardId = string.IsNullOrWhiteSpace(source.DashboardId) ? Guid.NewGuid().ToString("N") : source.DashboardId,
			Name = source.Name,
			Description = source.Description,
			Tags = new List<string>(source.Tags),
			RefreshSeconds = source.RefreshSeconds,
			IsDefault = source.IsDefault,
			Widgets = source.Widgets.Select(CloneWidget).ToList()
		};
	}

	private static DashboardWidget CloneWidget(DashboardWidget source)
	{
		return new DashboardWidget
		{
			WidgetId = string.IsNullOrWhiteSpace(source.WidgetId) ? Guid.NewGuid().ToString("N") : source.WidgetId,
			Type = source.Type,
			Title = source.Title,
			Metric = source.Metric,
			Visualization = source.Visualization,
			TimeRange = source.TimeRange,
			Filters = new Dictionary<string, string>(source.Filters),
			Thresholds = new Dictionary<string, double>(source.Thresholds),
			Highlight = source.Highlight
		};
	}
}
