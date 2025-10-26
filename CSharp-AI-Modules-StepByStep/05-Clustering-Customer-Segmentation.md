# Module 05: Clustering (Customer Segmentation)

Implement unsupervised learning for grouping similar data points.

## Learning Objectives
- K-means clustering
- Customer segmentation
- Cluster analysis
- Optimal cluster selection

## Example: Customer Segmentation

```csharp
var pipeline = mlContext.Transforms.Concatenate("Features", "Age", "Income", "Spending")
    .Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));

var predictions = model.Transform(data);
```

---
**Previous:** [Module 04](./04-Classification-Models.md) | **Next:** [Module 06](./06-Anomaly-Detection.md)
