# Module 07: Recommendation Systems

Build collaborative filtering systems for product and content recommendations.

## Learning Objectives
- Matrix factorization
- Collaborative filtering
- Content-based recommendations
- Hybrid systems

## Example: Product Recommendations

```csharp
var options = new MatrixFactorizationTrainer.Options
{
    MatrixColumnIndexColumnName = "UserId",
    MatrixRowIndexColumnName = "ProductId",
    LabelColumnName = "Rating",
    NumberOfIterations = 20
};

var trainer = mlContext.Recommendation().Trainers.MatrixFactorization(options);
```

---
**Previous:** [Module 06](./06-Anomaly-Detection.md) | **Next:** [Module 08](./08-Time-Series-Forecasting.md)
