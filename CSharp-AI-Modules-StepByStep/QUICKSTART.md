# Quick Start Guide

Get started with C# AI & ML.NET modules in 5 minutes.

## Prerequisites

- .NET 8.0 SDK installed
- Visual Studio 2022 or VS Code
- Basic C# knowledge

## 5-Minute Setup

### Step 1: Verify .NET Installation

```bash
dotnet --version
# Should show 8.0.x or later
```

### Step 2: Run the Hello World Example

```bash
# Navigate to examples
cd examples/src/HelloMLNet/HelloMLNet

# Run the example
dotnet run
```

Expected output:
```
=== ML.NET Hello World Example ===
✓ ML.NET Context Created
✓ Training Data Loaded

Training model...
✓ Model Trained Successfully

=== Making Predictions ===

House Size: 1,200 sqft → Predicted Price: $34,145.16
...
✓ ML.NET Hello World Complete!
```

### Step 3: Start Learning

Begin with [Module 00: Foundations](./00-Foundations-&-ML.NET-Setup.md)

## Quick Reference

### Learning Path

1. **Week 1-2**: Foundations (Modules 00-02)
2. **Week 3-6**: Core ML (Modules 03-06)
3. **Week 7-10**: Advanced (Modules 07-10)
4. **Week 11-12**: Production (Modules 11-12)
5. **Week 13-14**: Capstone (Module 13)

### Key Resources

- [Full README](./README.md) - Complete overview
- [TUTORIAL.md](./TUTORIAL.md) - Detailed learning guide
- [Tracking Workbook](./Tracking-Workbook.md) - Track your progress
- [Templates & Checklists](./Templates-&-Checklists.md) - Reusable templates

## Common First Steps

### Create a New ML.NET Project

```bash
# Create console app
dotnet new console -n MyMLProject
cd MyMLProject

# Add ML.NET
dotnet add package Microsoft.ML

# Run
dotnet run
```

### Basic ML.NET Code Template

```csharp
using Microsoft.ML;

var mlContext = new MLContext(seed: 0);

// 1. Load data
// 2. Build pipeline
// 3. Train model
// 4. Evaluate
// 5. Predict
```

## What's Included

- **14 Modules**: From basics to advanced
- **4 Support Files**: README, Tutorial, Templates, Tracking
- **Working Examples**: HelloMLNet and more
- **Real Project**: E-commerce ML system

## Module Overview

| Module | Topic | Duration |
|--------|-------|----------|
| 00 | Foundations | 3-4 days |
| 01 | ML.NET Basics | 3-4 days |
| 02 | Data Processing | 3-4 days |
| 03 | Regression | 4-5 days |
| 04 | Classification | 4-5 days |
| 05 | Clustering | 4-5 days |
| 06 | Anomaly Detection | 4-5 days |
| 07 | Recommendations | 5-6 days |
| 08 | Time Series | 5-6 days |
| 09 | Computer Vision | 5-6 days |
| 10 | NLP | 5-6 days |
| 11 | Deployment | 5-6 days |
| 12 | AI.NET | 5-6 days |
| 13 | Capstone | 10-12 days |

## Troubleshooting

### Can't find dotnet command
- Install .NET SDK from https://dotnet.microsoft.com/download

### Package restore fails
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Build errors
```bash
dotnet clean
dotnet build
```

## Next Steps

1. ✅ Complete the Hello World example
2. 📖 Read [Module 00](./00-Foundations-&-ML.NET-Setup.md)
3. 🎯 Set goals in [Tracking Workbook](./Tracking-Workbook.md)
4. 💻 Start coding!

## Questions?

- Check [TUTORIAL.md](./TUTORIAL.md) for detailed guidance
- Review [Templates & Checklists](./Templates-&-Checklists.md)
- Refer to [official ML.NET docs](https://docs.microsoft.com/dotnet/machine-learning/)

---

**Ready to master ML.NET?** Start with [Module 00](./00-Foundations-&-ML.NET-Setup.md)! 🚀
