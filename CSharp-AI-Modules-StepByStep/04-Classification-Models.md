# Module 04: Classification Models (Sentiment Analysis)

Master classification algorithms for categorizing data into discrete classes.

## Learning Objectives
- Binary classification
- Multi-class classification
- Sentiment analysis
- Model evaluation metrics
- Handle imbalanced datasets

## Topics Covered
- Logistic regression
- Decision trees
- SVM
- Evaluation metrics (Accuracy, Precision, Recall, F1)

## Example: Sentiment Classification

```csharp
var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "ReviewText")
    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

var metrics = mlContext.BinaryClassification.Evaluate(predictions);
Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
```

---
**Previous:** [Module 03](./03-Regression-Models.md) | **Next:** [Module 05](./05-Clustering-Customer-Segmentation.md)
