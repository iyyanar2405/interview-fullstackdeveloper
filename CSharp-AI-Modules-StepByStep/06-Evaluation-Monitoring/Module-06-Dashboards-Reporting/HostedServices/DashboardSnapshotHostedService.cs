using Microsoft.Extensions.Hosting;
using Module_06_Dashboards_Reporting.Models;
using Module_06_Dashboards_Reporting.Services;
using Microsoft.Extensions.Options;

namespace Module_06_Dashboards_Reporting.HostedServices;

public sealed class DashboardSnapshotHostedService : BackgroundService
{
	private readonly DashboardDefinitionService _definitions;
	private readonly DashboardComposerService _composer;
	private readonly DashboardSnapshotService _snapshots;
	private readonly ILogger<DashboardSnapshotHostedService> _logger;
	private readonly TimeSpan _interval;

	public DashboardSnapshotHostedService(
		DashboardDefinitionService definitions,
		DashboardComposerService composer,
		DashboardSnapshotService snapshots,
		IOptions<DashboardOptions> options,
		ILogger<DashboardSnapshotHostedService> logger)
	{
		_definitions = definitions;
		_composer = composer;
		_snapshots = snapshots;
		_logger = logger;
		_interval = TimeSpan.FromSeconds(30);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Dashboard snapshot worker started");

		while (!stoppingToken.IsCancellationRequested)
		{
			var dashboards = _definitions.GetAll();
			foreach (var dashboard in dashboards)
			{
				var snapshot = _composer.Compose(dashboard.DashboardId);
				if (snapshot is not null)
				{
					_snapshots.RecordSnapshot(snapshot);
				}
			}

			try
			{
				await Task.Delay(_interval, stoppingToken);
			}
			catch (TaskCanceledException)
			{
			}
		}

		_logger.LogInformation("Dashboard snapshot worker stopping");
	}
}
