using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Module_06_Dashboards_Reporting.Models;
using Module_06_Dashboards_Reporting.Services;

namespace Module_06_Dashboards_Reporting.HostedServices;

public sealed class MetricSimulationHostedService : BackgroundService
{
	private readonly MetricArchiveService _metrics;
	private readonly IReadOnlyList<string> _metricIds;
	private readonly Dictionary<string, double> _baselines;
	private readonly Dictionary<string, double> _volatility;
	private readonly TimeSpan _interval;
	private readonly ILogger<MetricSimulationHostedService> _logger;

	public MetricSimulationHostedService(
		MetricArchiveService metrics,
		IOptions<MetricSimulationOptions> options,
		ILogger<MetricSimulationHostedService> logger)
	{
		_metrics = metrics;
		_logger = logger;
		_metricIds = options.Value.Metrics.Count > 0
			? options.Value.Metrics
			: new List<string> { "latency.p95", "throughput.rps", "errors.rate", "cost.per.request" };
		_baselines = options.Value.Baseline;
		_volatility = options.Value.Volatility;
		_interval = TimeSpan.FromSeconds(Math.Max(5, options.Value.IntervalSeconds));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting metric simulation for {MetricCount} metrics", _metricIds.Count);

		while (!stoppingToken.IsCancellationRequested)
		{
			foreach (var metric in _metricIds)
			{
				var value = GenerateValue(metric);
				_metrics.Record(metric, value, BuildDimensions(metric));
			}

			try
			{
				await Task.Delay(_interval, stoppingToken);
			}
			catch (TaskCanceledException)
			{
			}
		}

		_logger.LogInformation("Metric simulation stopped");
	}

	private double GenerateValue(string metric)
	{
		var baseline = _baselines.TryGetValue(metric, out var b) ? b : DefaultBaseline(metric);
		var volatility = _volatility.TryGetValue(metric, out var v) ? v : DefaultVolatility(metric);
		var noise = NextGaussian() * volatility;
		return Math.Max(0, baseline + noise);
	}

	private static double DefaultBaseline(string metric)
	{
		return metric switch
		{
			"latency.p95" => 220,
			"throughput.rps" => 85,
			"errors.rate" => 2.5,
			"cost.per.request" => 0.018,
			"uptime.percent" => 99.5,
			_ => 50
		};
	}

	private static double DefaultVolatility(string metric)
	{
		return metric switch
		{
			"latency.p95" => 12,
			"throughput.rps" => 6,
			"errors.rate" => 0.8,
			"cost.per.request" => 0.002,
			"uptime.percent" => 0.1,
			_ => 5
		};
	}

	private static double NextGaussian()
	{
		var u1 = 1.0 - Random.Shared.NextDouble();
		var u2 = 1.0 - Random.Shared.NextDouble();
		return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
	}

	private static IDictionary<string, string> BuildDimensions(string metric)
	{
		var regions = new[] { "use", "usw", "euw", "apse" };
		var environments = new[] { "prod", "staging" };
		return new Dictionary<string, string>
		{
			["region"] = regions[Random.Shared.Next(regions.Length)],
			["environment"] = environments[Random.Shared.Next(environments.Length)],
			["metric"] = metric
		};
	}
}
