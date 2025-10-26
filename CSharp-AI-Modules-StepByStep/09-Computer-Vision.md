# Module 09: Computer Vision (Image Classification)

Implement image classification and computer vision tasks with ML.NET.

## Learning Objectives
- Image classification
- Transfer learning
- ML.NET Vision API
- Object detection basics

## Example: Image Classification

```csharp
var pipeline = mlContext.Transforms.LoadImages("Image", imageFolder, "ImagePath")
    .Append(mlContext.Transforms.ResizeImages("Image", 224, 224))
    .Append(mlContext.Transforms.ExtractPixels("Image"))
    .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(
        featureColumnName: "Image",
        labelColumnName: "Label"));
```

---
**Previous:** [Module 08](./08-Time-Series-Forecasting.md) | **Next:** [Module 10](./10-NLP-Text-Analysis.md)
