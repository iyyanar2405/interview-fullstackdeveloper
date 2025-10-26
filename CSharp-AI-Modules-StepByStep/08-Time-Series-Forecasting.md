# Module 08: Time Series Forecasting

Predict future values based on historical time-series data.

## Learning Objectives
- Time series concepts
- Sales forecasting
- Trend analysis
- Seasonal patterns

## Example: Sales Forecasting

```csharp
var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
    outputColumnName: "ForecastedSales",
    inputColumnName: "Sales",
    windowSize: 7,
    seriesLength: 30,
    trainSize: 100,
    horizon: 7);
```

---
**Previous:** [Module 07](./07-Recommendation-Systems.md) | **Next:** [Module 09](./09-Computer-Vision.md)
