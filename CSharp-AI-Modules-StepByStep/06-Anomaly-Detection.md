# Module 06: Anomaly Detection (Fraud Detection)

Detect outliers and anomalies in data for fraud detection and quality control.

## Learning Objectives
- Anomaly detection algorithms
- Fraud detection systems
- Outlier identification
- Time-based anomalies

## Example: Fraud Detection

```csharp
var pipeline = mlContext.Transforms.Concatenate("Features", "Amount", "Location", "Time")
    .Append(mlContext.AnomalyDetection.Trainers.RandomizedPca());
```

---
**Previous:** [Module 05](./05-Clustering-Customer-Segmentation.md) | **Next:** [Module 07](./07-Recommendation-Systems.md)
