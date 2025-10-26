# Module 02: Data Loading & Preprocessing

Master data loading, cleaning, and preprocessing techniques for ML.NET projects.

## Learning Objectives

- Load data from multiple sources (CSV, databases, APIs)
- Handle missing and inconsistent data
- Perform feature engineering and selection
- Implement data transformations
- Create reusable data pipelines

## Part 1: Data Loading

### Loading from CSV Files

```csharp
var dataView = mlContext.Data.LoadFromTextFile<DataModel>(
    path: "data.csv",
    hasHeader: true,
    separatorChar: ',',
    allowQuoting: true);
```

### Loading from Databases

```csharp
using Microsoft.Data.SqlClient;

var connectionString = "Server=.;Database=ML;Trusted_Connection=True;";
var loaderColumns = new[]
{
    new DatabaseLoader.Column { Name = "Feature1", Type = DbType.Single },
    new DatabaseLoader.Column { Name = "Label", Type = DbType.Boolean }
};

var loader = mlContext.Data.CreateDatabaseLoader(loaderColumns);
var dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, 
    "SELECT Feature1, Label FROM TrainingData");
var data = loader.Load(dbSource);
```

## Part 2: Data Cleaning

### Handling Missing Values

```csharp
var pipeline = mlContext.Transforms.ReplaceMissingValues(
    new[] {
        new InputOutputColumnPair("Feature1", "Feature1"),
        new InputOutputColumnPair("Feature2", "Feature2")
    },
    replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);
```

### Filtering Data

```csharp
var filteredData = mlContext.Data.FilterRowsByColumn(
    data, 
    columnName: "Age", 
    lowerBound: 18, 
    upperBound: 100);
```

## Part 3: Feature Engineering

### Creating Derived Features

```csharp
var pipeline = mlContext.Transforms.CustomMapping<InputData, OutputData>(
    (input, output) => {
        output.BMI = input.Weight / (input.Height * input.Height);
    }, 
    contractName: "BMICalculation");
```

### Feature Selection

```csharp
var selector = mlContext.Transforms.FeatureSelection.SelectFeaturesBasedOnCount(
    "Features", 
    count: 1000);
```

## Part 4: Data Transformations

### Categorical Encoding

```csharp
var oneHotPipeline = mlContext.Transforms.Categorical.OneHotEncoding(
    "CategoryEncoded", 
    "Category");
```

### Text Featurization

```csharp
var textPipeline = mlContext.Transforms.Text.FeaturizeText(
    "Features", 
    "TextColumn");
```

### Normalization

```csharp
var normalizer = mlContext.Transforms.NormalizeMinMax("Features");
```

## Summary

You learned:
- ✅ Multiple data loading techniques
- ✅ Data cleaning strategies
- ✅ Feature engineering methods
- ✅ Data transformation pipelines

---

**Previous:** [Module 01: ML.NET Basics](./01-ML.NET-Basics-&-Architecture.md) | **Next:** [Module 03: Regression Models](./03-Regression-Models.md)
