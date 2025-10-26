# Templates & Checklists

Ready-to-use templates and checklists for ML.NET projects.

## Model Development Checklist

### Phase 1: Problem Definition
- [ ] Define business problem clearly
- [ ] Identify ML problem type (classification, regression, etc.)
- [ ] Define success metrics
- [ ] Gather stakeholder requirements
- [ ] Document constraints and limitations

### Phase 2: Data Preparation
- [ ] Collect sufficient training data
- [ ] Verify data quality
- [ ] Handle missing values
- [ ] Remove duplicates
- [ ] Split data (train/validation/test)
- [ ] Document data schema
- [ ] Perform exploratory data analysis

### Phase 3: Feature Engineering
- [ ] Select relevant features
- [ ] Create derived features
- [ ] Encode categorical variables
- [ ] Normalize/standardize numeric features
- [ ] Handle imbalanced data (if applicable)
- [ ] Document feature transformations

### Phase 4: Model Training
- [ ] Choose appropriate algorithm
- [ ] Configure hyperparameters
- [ ] Train model on training set
- [ ] Implement cross-validation
- [ ] Monitor training progress
- [ ] Save trained model

### Phase 5: Model Evaluation
- [ ] Evaluate on test set
- [ ] Calculate relevant metrics
- [ ] Compare with baseline
- [ ] Perform error analysis
- [ ] Test edge cases
- [ ] Validate business requirements met

### Phase 6: Deployment
- [ ] Create prediction API
- [ ] Implement error handling
- [ ] Add logging and monitoring
- [ ] Set up model versioning
- [ ] Deploy to production environment
- [ ] Create rollback plan

### Phase 7: Monitoring
- [ ] Track prediction accuracy
- [ ] Monitor data drift
- [ ] Log prediction requests
- [ ] Set up alerts
- [ ] Plan model retraining schedule
- [ ] Document performance

## Project Template

### ML.NET Console Application

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;

namespace MLProjectTemplate
{
    // 1. Define Data Models
    public class InputData
    {
        [LoadColumn(0)]
        public float Feature1 { get; set; }
        
        [LoadColumn(1)]
        public float Feature2 { get; set; }
        
        [LoadColumn(2)]
        [ColumnName("Label")]
        public float Target { get; set; }
    }

    public class OutputPrediction
    {
        [ColumnName("Score")]
        public float PredictedValue { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 2. Create ML Context
            var mlContext = new MLContext(seed: 0);

            // 3. Load Data
            IDataView data = LoadData(mlContext);

            // 4. Split Data
            var splitData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // 5. Build Pipeline
            var pipeline = BuildPipeline(mlContext);

            // 6. Train Model
            var model = TrainModel(mlContext, pipeline, splitData.TrainSet);

            // 7. Evaluate Model
            EvaluateModel(mlContext, model, splitData.TestSet);

            // 8. Save Model
            SaveModel(mlContext, model, data.Schema);

            // 9. Make Predictions
            MakePredictions(mlContext, model);
        }

        static IDataView LoadData(MLContext mlContext)
        {
            // Load your data here
            var data = new[]
            {
                new InputData { Feature1 = 1.0f, Feature2 = 2.0f, Target = 3.0f }
            };
            return mlContext.Data.LoadFromEnumerable(data);
        }

        static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            return mlContext.Transforms.Concatenate("Features", "Feature1", "Feature2")
                .Append(mlContext.Regression.Trainers.Sdca());
        }

        static ITransformer TrainModel(
            MLContext mlContext, 
            IEstimator<ITransformer> pipeline, 
            IDataView trainData)
        {
            Console.WriteLine("Training model...");
            return pipeline.Fit(trainData);
        }

        static void EvaluateModel(
            MLContext mlContext, 
            ITransformer model, 
            IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.Regression.Evaluate(predictions);
            
            Console.WriteLine($"R-Squared: {metrics.RSquared:F4}");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F4}");
        }

        static void SaveModel(
            MLContext mlContext, 
            ITransformer model, 
            DataViewSchema schema)
        {
            mlContext.Model.Save(model, schema, "model.zip");
            Console.WriteLine("Model saved!");
        }

        static void MakePredictions(MLContext mlContext, ITransformer model)
        {
            var predictionEngine = mlContext.Model
                .CreatePredictionEngine<InputData, OutputPrediction>(model);

            var testInput = new InputData { Feature1 = 5.0f, Feature2 = 10.0f };
            var prediction = predictionEngine.Predict(testInput);
            
            Console.WriteLine($"Prediction: {prediction.PredictedValue:F2}");
        }
    }
}
```

## Model Documentation Template

```markdown
# Model: [Model Name]

## Overview
- **Purpose**: [What problem does this model solve?]
- **Type**: [Classification/Regression/Clustering/etc.]
- **Version**: [1.0.0]
- **Created**: [Date]
- **Updated**: [Date]

## Data

### Input Features
| Feature | Type | Description | Example |
|---------|------|-------------|---------|
| Feature1 | float | [Description] | 42.5 |
| Feature2 | string | [Description] | "value" |

### Output
| Field | Type | Description |
|-------|------|-------------|
| Prediction | [type] | [Description] |
| Confidence | float | Prediction confidence (0-1) |

### Training Data
- **Source**: [Where data comes from]
- **Size**: [Number of samples]
- **Date Range**: [Time period covered]
- **Preprocessing**: [Steps applied]

## Model Architecture

### Algorithm
- **Trainer**: [e.g., SdcaLogisticRegression]
- **Hyperparameters**:
  - Parameter1: value
  - Parameter2: value

### Pipeline
```
1. Load Data
2. [Transform 1]
3. [Transform 2]
4. Train with [Algorithm]
5. Output Model
```

## Performance Metrics

### Training Metrics
- Accuracy: [value]%
- Precision: [value]%
- Recall: [value]%
- F1 Score: [value]

### Test Metrics
- Accuracy: [value]%
- AUC-ROC: [value]
- Confusion Matrix:
  ```
  [[TP, FP],
   [FN, TN]]
  ```

### Cross-Validation
- Folds: [number]
- Average Accuracy: [value]%

## Usage

### Training
```bash
dotnet run --train --data path/to/data.csv
```

### Prediction
```csharp
var input = new InputData { Feature1 = 10.0f };
var prediction = engine.Predict(input);
```

### API Endpoint
```
POST /api/predict
{
  "feature1": 10.0,
  "feature2": "value"
}
```

## Deployment

- **Environment**: [Production/Staging]
- **Location**: [Azure/AWS/On-premise]
- **Container**: [Docker image]
- **Scaling**: [Strategy]

## Monitoring

- **Metrics Tracked**: [List metrics]
- **Alert Thresholds**: [When to alert]
- **Retraining Schedule**: [Frequency]

## Limitations

- [Limitation 1]
- [Limitation 2]
- [Edge cases to watch]

## Changelog

### Version 1.0.0 (YYYY-MM-DD)
- Initial model release
- Features: [list]
- Performance: [metrics]
```

## Evaluation Metrics Reference

### Classification Metrics

```csharp
// Binary Classification
var metrics = mlContext.BinaryClassification.Evaluate(predictions);
Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
Console.WriteLine($"Recall: {metrics.PositiveRecall:P2}");

// Multiclass Classification
var metrics = mlContext.MulticlassClassification.Evaluate(predictions);
Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy:P2}");
Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy:P2}");
Console.WriteLine($"Log Loss: {metrics.LogLoss:F4}");
```

### Regression Metrics

```csharp
var metrics = mlContext.Regression.Evaluate(predictions);
Console.WriteLine($"R-Squared: {metrics.RSquared:F4}");
Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F4}");
Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F4}");
Console.WriteLine($"MSE: {metrics.MeanSquaredError:F4}");
```

### Clustering Metrics

```csharp
var metrics = mlContext.Clustering.Evaluate(predictions);
Console.WriteLine($"Average Distance: {metrics.AverageDistance:F4}");
Console.WriteLine($"Davies Bouldin Index: {metrics.DaviesBouldinIndex:F4}");
```

## Data Preprocessing Snippets

### Handle Missing Values

```csharp
// Replace missing values with default
var pipeline = mlContext.Transforms.ReplaceMissingValues(
    outputColumnName: "Feature",
    inputColumnName: "Feature",
    replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);
```

### Feature Scaling

```csharp
// Min-Max normalization
var normalizer = mlContext.Transforms.NormalizeMinMax("Features");

// Z-score normalization
var normalizer = mlContext.Transforms.NormalizeMeanVariance("Features");
```

### Text Processing

```csharp
var textPipeline = mlContext.Transforms.Text
    .NormalizeText("NormalizedText", "Text")
    .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormalizedText"))
    .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens"))
    .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Tokens"));
```

## API Response Template

```json
{
  "success": true,
  "prediction": {
    "value": 42.5,
    "confidence": 0.95,
    "label": "positive"
  },
  "metadata": {
    "modelVersion": "1.0.0",
    "timestamp": "2025-10-26T18:00:00Z",
    "processingTimeMs": 15
  }
}
```

## Error Handling Template

```csharp
public class PredictionResult<T>
{
    public bool Success { get; set; }
    public T Prediction { get; set; }
    public string ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; }

    public static PredictionResult<T> Ok(T prediction)
    {
        return new PredictionResult<T>
        {
            Success = true,
            Prediction = prediction,
            Metadata = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["modelVersion"] = "1.0.0"
            }
        };
    }

    public static PredictionResult<T> Error(string message)
    {
        return new PredictionResult<T>
        {
            Success = false,
            ErrorMessage = message
        };
    }
}
```

## Testing Checklist

### Unit Tests
- [ ] Test data loading
- [ ] Test feature transformations
- [ ] Test model training
- [ ] Test prediction logic
- [ ] Test error handling

### Integration Tests
- [ ] Test API endpoints
- [ ] Test database operations
- [ ] Test model loading
- [ ] Test batch predictions

### Performance Tests
- [ ] Test prediction latency
- [ ] Test throughput
- [ ] Test memory usage
- [ ] Test concurrent requests

## Deployment Checklist

- [ ] Model trained and validated
- [ ] API endpoints tested
- [ ] Documentation complete
- [ ] Monitoring configured
- [ ] Security review passed
- [ ] Performance benchmarks met
- [ ] Rollback plan ready
- [ ] Stakeholders notified

---

Use these templates and checklists to maintain consistency across your ML.NET projects!
