# Module 03: Regression Models (Price Prediction)

Build regression models for predicting continuous values like prices, sales, and forecasts.

## Learning Objectives
- Understand regression fundamentals
- Implement linear regression
- Use FastTree and other algorithms
- Evaluate regression models
- Build a price prediction system

## Topics Covered
- Linear regression basics
- Polynomial regression
- FastTree regression
- Model evaluation (R², RMSE, MAE)
- Real-world price prediction

## Example: House Price Prediction

```csharp
public class HouseData
{
    [LoadColumn(0)] public float Size { get; set; }
    [LoadColumn(1)] public float Bedrooms { get; set; }
    [LoadColumn(2)] public float Age { get; set; }
    [LoadColumn(3), ColumnName("Label")] public float Price { get; set; }
}

var pipeline = mlContext.Transforms.Concatenate("Features", "Size", "Bedrooms", "Age")
    .Append(mlContext.Regression.Trainers.FastTree());
```

---
**Previous:** [Module 02](./02-Data-Loading-&-Preprocessing.md) | **Next:** [Module 04](./04-Classification-Models.md)
