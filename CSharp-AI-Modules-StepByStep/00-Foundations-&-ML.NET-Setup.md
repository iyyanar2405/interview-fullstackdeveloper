# Module 00: Foundations & ML.NET Setup

Master the fundamentals of machine learning and set up your ML.NET development environment for building AI-powered applications.

## Learning Objectives

By the end of this module, you will be able to:
- Understand core machine learning concepts and terminology
- Set up a complete ML.NET development environment
- Create your first ML.NET project
- Understand the ML.NET pipeline architecture
- Run a simple "Hello World" ML.NET application

## Prerequisites

### Required Knowledge
- C# programming fundamentals (variables, loops, classes, methods)
- Basic .NET Core experience
- Command line/terminal usage
- Understanding of NuGet packages

### System Requirements
- **OS**: Windows 10/11, macOS, or Linux
- **RAM**: 8GB minimum (16GB recommended)
- **Disk**: 10GB free space
- **CPU**: Modern multi-core processor

## Part 1: Machine Learning Fundamentals

### What is Machine Learning?

Machine Learning (ML) is a subset of artificial intelligence that enables computers to learn from data and make predictions without being explicitly programmed for every scenario.

### Key ML Concepts

#### 1. **Types of Machine Learning**

**Supervised Learning**
- Learn from labeled data (input + correct output)
- Examples: Regression, Classification
- Use case: Price prediction, spam detection

**Unsupervised Learning**
- Find patterns in unlabeled data
- Examples: Clustering, Dimensionality reduction
- Use case: Customer segmentation, anomaly detection

**Reinforcement Learning**
- Learn through trial and error with rewards
- Examples: Game AI, robotics
- Not directly supported in ML.NET (use specialized frameworks)

#### 2. **Core ML Terminology**

| Term | Definition | Example |
|------|------------|---------|
| **Dataset** | Collection of data for training/testing | Customer purchase history |
| **Features** | Input variables used for prediction | Price, rating, category |
| **Label** | Target variable to predict | Sales amount, category |
| **Model** | Trained algorithm that makes predictions | Trained price predictor |
| **Training** | Process of teaching the model | Feeding historical data |
| **Prediction** | Output from the model | Predicted price: $49.99 |
| **Accuracy** | How often model is correct | 95% accuracy |

#### 3. **ML Problem Types in ML.NET**

| Problem | Description | ML.NET API | Example |
|---------|-------------|-----------|---------|
| **Binary Classification** | Two possible outcomes (Yes/No) | `BinaryClassification` | Spam detection |
| **Multiclass Classification** | Multiple categories | `MulticlassClassification` | Product categorization |
| **Regression** | Predict continuous values | `Regression` | Price prediction |
| **Clustering** | Group similar items | `Clustering` | Customer segments |
| **Anomaly Detection** | Identify outliers | `AnomalyDetection` | Fraud detection |
| **Ranking** | Order items by relevance | `Ranking` | Search results |
| **Recommendation** | Suggest items to users | `Recommendation` | Product suggestions |
| **Time Series** | Forecast future values | `Forecasting` | Sales prediction |

## Part 2: ML.NET Overview

### What is ML.NET?

ML.NET is a free, open-source, cross-platform machine learning framework for .NET developers. It enables you to:
- Build custom ML models using C# or F#
- Integrate ML into .NET applications without Python
- Deploy models to various platforms (web, desktop, mobile, IoT)
- Use pre-trained models or train custom models

### ML.NET Architecture

```
┌─────────────────────────────────────────────────┐
│           Your .NET Application                  │
├─────────────────────────────────────────────────┤
│              ML.NET API Layer                    │
├─────────────────────────────────────────────────┤
│  Data Loading → Transform → Train → Evaluate    │
├─────────────────────────────────────────────────┤
│          ML.NET Core Algorithms                  │
│  (FastTree, LightGBM, SDCA, etc.)               │
├─────────────────────────────────────────────────┤
│        .NET Runtime & Libraries                  │
└─────────────────────────────────────────────────┘
```

### Key Components

1. **MLContext**: Entry point for all ML.NET operations
2. **Data**: Loading and handling datasets
3. **Transforms**: Data preprocessing and feature engineering
4. **Trainers**: ML algorithms for training models
5. **Prediction Engine**: Making predictions with trained models

## Part 3: Environment Setup

### Step 1: Install .NET SDK

**Windows**
```bash
# Download from https://dotnet.microsoft.com/download
# Or use winget
winget install Microsoft.DotNet.SDK.8
```

**macOS**
```bash
# Using Homebrew
brew install --cask dotnet-sdk
```

**Linux (Ubuntu)**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

**Verify Installation**
```bash
dotnet --version
# Should show: 8.0.x or later
```

### Step 2: Install IDE

**Option A: Visual Studio 2022 (Windows/Mac)**
- Download: https://visualstudio.microsoft.com/
- Workloads: ".NET desktop development" or "ASP.NET and web development"
- Extensions: Install "ML.NET Model Builder" from Extensions menu

**Option B: Visual Studio Code (All Platforms)**
```bash
# Download from https://code.visualstudio.com/

# Install C# extension
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

### Step 3: Create Your First ML.NET Project

```bash
# Create project directory
mkdir MLNetGettingStarted
cd MLNetGettingStarted

# Create console application
dotnet new console -n HelloMLNet
cd HelloMLNet

# Add ML.NET package
dotnet add package Microsoft.ML --version 3.0.1

# Restore packages
dotnet restore
```

### Step 4: Verify Installation

Create a simple test file `Program.cs`:

```csharp
using Microsoft.ML;
using System;

namespace HelloMLNet
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create ML Context
            var mlContext = new MLContext(seed: 0);
            
            Console.WriteLine("ML.NET Environment Setup Successful!");
            Console.WriteLine($"ML.NET Version: {typeof(MLContext).Assembly.GetName().Version}");
            Console.WriteLine("Ready to build ML models!");
        }
    }
}
```

Run the project:
```bash
dotnet run
```

Expected output:
```
ML.NET Environment Setup Successful!
ML.NET Version: 3.0.1.0
Ready to build ML models!
```

## Part 4: ML.NET Pipeline Basics

### The ML.NET Workflow

```
1. Load Data         → IDataView
2. Transform Data    → ML Pipeline
3. Train Model       → ITransformer
4. Evaluate Model    → Metrics
5. Make Predictions  → Prediction Results
```

### Simple Example: Hello World ML

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace HelloMLNet
{
    // Step 1: Define data structure
    public class HouseData
    {
        [LoadColumn(0)]
        public float Size { get; set; }
        
        [LoadColumn(1)]
        public float Price { get; set; }
    }

    public class HousePrediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Step 2: Create ML Context
            var mlContext = new MLContext();

            // Step 3: Create sample data
            var data = new[]
            {
                new HouseData { Size = 1000, Price = 100000 },
                new HouseData { Size = 1500, Price = 150000 },
                new HouseData { Size = 2000, Price = 200000 },
                new HouseData { Size = 2500, Price = 250000 }
            };

            // Load data into IDataView
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            // Step 4: Define pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", "Size")
                .Append(mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "Price",
                    featureColumnName: "Features"));

            // Step 5: Train model
            Console.WriteLine("Training model...");
            var model = pipeline.Fit(dataView);

            // Step 6: Make predictions
            var predictionEngine = mlContext.Model
                .CreatePredictionEngine<HouseData, HousePrediction>(model);

            var testHouse = new HouseData { Size = 1800 };
            var prediction = predictionEngine.Predict(testHouse);

            Console.WriteLine($"\nPrediction:");
            Console.WriteLine($"House Size: {testHouse.Size} sqft");
            Console.WriteLine($"Predicted Price: ${prediction.Price:N2}");
        }
    }
}
```

Run this example:
```bash
dotnet run
```

Expected output:
```
Training model...

Prediction:
House Size: 1800 sqft
Predicted Price: $180,000.00
```

## Part 5: Essential ML.NET Packages

### Core Packages

```xml
<!-- Add to your .csproj file -->
<ItemGroup>
  <!-- Core ML.NET package -->
  <PackageReference Include="Microsoft.ML" Version="3.0.1" />
  
  <!-- For computer vision tasks -->
  <PackageReference Include="Microsoft.ML.Vision" Version="3.0.1" />
  
  <!-- For image analytics -->
  <PackageReference Include="Microsoft.ML.ImageAnalytics" Version="3.0.1" />
  
  <!-- For time series and forecasting -->
  <PackageReference Include="Microsoft.ML.TimeSeries" Version="3.0.1" />
  
  <!-- For recommendation systems -->
  <PackageReference Include="Microsoft.ML.Recommender" Version="0.21.1" />
  
  <!-- FastTree algorithm -->
  <PackageReference Include="Microsoft.ML.FastTree" Version="3.0.1" />
  
  <!-- LightGBM algorithm -->
  <PackageReference Include="Microsoft.ML.LightGbm" Version="3.0.1" />
  
  <!-- ONNX model support -->
  <PackageReference Include="Microsoft.ML.OnnxTransformer" Version="3.0.1" />
</ItemGroup>
```

### Install via CLI

```bash
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.Vision
dotnet add package Microsoft.ML.ImageAnalytics
dotnet add package Microsoft.ML.TimeSeries
dotnet add package Microsoft.ML.FastTree
dotnet add package Microsoft.ML.LightGbm
```

## Part 6: Development Tools

### ML.NET CLI

Install the ML.NET CLI for automated model generation:

```bash
dotnet tool install -g mlnet
```

Verify installation:
```bash
mlnet --version
```

### ML.NET Model Builder (Visual Studio Extension)

1. Open Visual Studio 2022
2. Go to Extensions → Manage Extensions
3. Search for "ML.NET Model Builder"
4. Install and restart Visual Studio

### Jupyter Notebooks with .NET Interactive

```bash
# Install .NET Interactive
dotnet tool install -g Microsoft.dotnet-interactive

# Install Jupyter
dotnet interactive jupyter install
```

## Part 7: Project Structure Best Practices

### Recommended Folder Structure

```
MyMLProject/
├── src/
│   ├── MyMLProject.Console/        # Console app
│   ├── MyMLProject.WebAPI/         # Web API
│   └── MyMLProject.ML/             # ML logic
│       ├── Models/                 # Data models
│       ├── DataModels/             # Input/Output classes
│       ├── Trainers/               # Training logic
│       └── Predictors/             # Prediction logic
├── data/
│   ├── raw/                        # Original datasets
│   ├── processed/                  # Cleaned data
│   └── models/                     # Trained models (.zip)
├── tests/
│   └── MyMLProject.Tests/          # Unit tests
└── docs/
    └── model-documentation.md      # Model docs
```

## Part 8: Common Issues and Troubleshooting

### Issue 1: Package Restore Fails

**Solution:**
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Issue 2: ML.NET Version Conflicts

**Solution:**
```bash
# Check installed versions
dotnet list package

# Update all packages
dotnet add package Microsoft.ML --version 3.0.1
```

### Issue 3: Out of Memory Errors

**Solution:**
- Reduce dataset size during development
- Use data sampling
- Increase available memory
- Use lazy loading for large datasets

### Issue 4: Model Not Loading

**Solution:**
```csharp
// Always use consistent ML Context seed
var mlContext = new MLContext(seed: 0);

// Save model with schema
mlContext.Model.Save(model, dataView.Schema, "model.zip");

// Load with proper schema
var loadedModel = mlContext.Model.Load("model.zip", out var schema);
```

## Part 9: Hands-On Exercise

### Exercise: Build a Temperature Converter Predictor

Create a simple ML model that learns to convert Celsius to Fahrenheit.

**Task Steps:**
1. Create a new console project
2. Define data classes for Celsius and Fahrenheit
3. Create sample training data
4. Build and train a regression model
5. Test predictions with new values
6. Calculate prediction accuracy

**Starter Code:**

```csharp
// Define your data classes
public class TemperatureData
{
    public float Celsius { get; set; }
    public float Fahrenheit { get; set; }
}

// TODO: Add prediction class
// TODO: Create MLContext
// TODO: Generate training data (use F = C * 1.8 + 32)
// TODO: Build pipeline
// TODO: Train model
// TODO: Test predictions
```

**Expected Results:**
- Input: 0°C → Output: ~32°F
- Input: 100°C → Output: ~212°F
- Input: 37°C → Output: ~98.6°F

## Part 10: Resources and Next Steps

### Official Resources
- [ML.NET Documentation](https://docs.microsoft.com/dotnet/machine-learning/)
- [ML.NET Samples GitHub](https://github.com/dotnet/machinelearning-samples)
- [ML.NET API Reference](https://docs.microsoft.com/dotnet/api/microsoft.ml)

### Community
- [ML.NET GitHub Issues](https://github.com/dotnet/machinelearning/issues)
- [Stack Overflow - ML.NET Tag](https://stackoverflow.com/questions/tagged/ml.net)

### Learning Path
- [Microsoft Learn - ML.NET](https://docs.microsoft.com/learn/paths/get-started-with-ml-dotnet/)

## Summary

In this module, you:
- ✅ Learned machine learning fundamentals
- ✅ Understood ML.NET architecture
- ✅ Set up your development environment
- ✅ Created your first ML.NET project
- ✅ Ran a simple regression model
- ✅ Learned best practices and troubleshooting

## Next Module

Continue to [Module 01: ML.NET Basics & Architecture](./01-ML.NET-Basics-&-Architecture.md) to dive deeper into ML.NET's pipeline architecture and data handling.

## Checklist

- [ ] .NET SDK 8.0+ installed and verified
- [ ] IDE (Visual Studio or VS Code) set up
- [ ] ML.NET packages installed
- [ ] First "Hello World" ML.NET app running
- [ ] Simple regression model tested
- [ ] Development environment working
- [ ] Exercise completed
- [ ] Ready for Module 01

---

**Need Help?** Check the [Troubleshooting section](#part-8-common-issues-and-troubleshooting) or refer to [TUTORIAL.md](./TUTORIAL.md) for additional guidance.
