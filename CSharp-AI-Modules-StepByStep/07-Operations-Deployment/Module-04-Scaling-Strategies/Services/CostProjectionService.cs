using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class CostProjectionService
{
    public CostProjection ProjectCosts(ScalingProfile profile, WorkloadSnapshot snapshot, CapacityForecast forecast)
    {
        var currentInstances = Math.Max(profile.MinInstances, snapshot.CurrentInstances);
        var forecastInstances = Math.Max(forecast.RequiredInstances, currentInstances);
        var hourly = profile.CostPerInstance;

        var hourlyCurrent = hourly * currentInstances;
        var hourlyForecast = hourly * forecastInstances;
        var monthlyCurrent = hourlyCurrent * 24 * 30;
        var monthlyForecast = hourlyForecast * 24 * 30;
        var savings = Math.Max(0, monthlyCurrent - monthlyForecast);

        return new CostProjection
        {
            HourlyCostCurrent = Math.Round(hourlyCurrent, 2),
            HourlyCostForecast = Math.Round(hourlyForecast, 2),
            MonthlyCostCurrent = Math.Round(monthlyCurrent, 2),
            MonthlyCostForecast = Math.Round(monthlyForecast, 2),
            SavingsOpportunity = Math.Round(savings, 2)
        };
    }
}
