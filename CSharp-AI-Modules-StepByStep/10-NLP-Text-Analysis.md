# Module 10: NLP & Text Analysis

Process and analyze text data for classification and entity recognition.

## Learning Objectives
- Text preprocessing
- Named entity recognition
- Text classification
- Sentiment analysis

## Example: Text Classification

```csharp
var pipeline = mlContext.Transforms.Text.NormalizeText("Text")
    .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text"))
    .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens"))
    .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Tokens"))
    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy());
```

---
**Previous:** [Module 09](./09-Computer-Vision.md) | **Next:** [Module 11](./11-Model-Deployment-&-API.md)
