# C# AI & ML.NET Modules — Step by Step (00–13)

A comprehensive, hands-on journey through ML.NET and AI.NET to build production-ready machine learning solutions with real-time project examples.

## Outcomes
- **Beginner**: Set up ML.NET environment, understand ML fundamentals, build basic models
- **Advanced**: Implement various ML algorithms (regression, classification, clustering), deploy models
- **Expert**: Build complete real-time ML systems with AI.NET integration, production deployment

## Quickstart
- Install .NET 8 SDK or later
- Install Visual Studio 2022 or VS Code with C# extension
- Install NuGet packages: `Microsoft.ML`, `Microsoft.ML.Vision`, `Microsoft.ML.TimeSeries`
- For AI.NET features: Azure subscription (free tier available)

## Structure
- **examples/src** — Runnable .NET console and web apps with ML.NET implementations
- **00–13** — Progressive modules from foundations to capstone project
- **TUTORIAL.md** — Guided learning path with hands-on exercises
- **Templates-&-Checklists.md** — Project templates, model evaluation checklists
- **Tracking-Workbook.md** — Track your progress through each module

## Contents

### Foundation & Setup
- [00 — Foundations & ML.NET Setup](./00-Foundations-&-ML.NET-Setup.md)

### Core ML.NET Concepts
- [01 — ML.NET Basics & Architecture](./01-ML.NET-Basics-&-Architecture.md)
- [02 — Data Loading & Preprocessing](./02-Data-Loading-&-Preprocessing.md)

### Supervised Learning
- [03 — Regression Models (Price Prediction)](./03-Regression-Models.md)
- [04 — Classification Models (Sentiment Analysis)](./04-Classification-Models.md)

### Unsupervised Learning
- [05 — Clustering (Customer Segmentation)](./05-Clustering-Customer-Segmentation.md)
- [06 — Anomaly Detection (Fraud Detection)](./06-Anomaly-Detection.md)

### Advanced ML Scenarios
- [07 — Recommendation Systems](./07-Recommendation-Systems.md)
- [08 — Time Series Forecasting](./08-Time-Series-Forecasting.md)
- [09 — Computer Vision (Image Classification)](./09-Computer-Vision.md)
- [10 — NLP & Text Analysis](./10-NLP-Text-Analysis.md)

### Production & Integration
- [11 — Model Deployment & API Integration](./11-Model-Deployment-&-API.md)
- [12 — AI.NET & Azure AI Services](./12-AI.NET-Azure-Integration.md)

### Real-Time Project
- [13 — Capstone: E-Commerce ML System](./13-Capstone-ECommerce-ML-System.md)

## Real-Time Project Overview

The capstone project (Module 13) integrates all learned concepts into a complete **E-Commerce Intelligence System** featuring:

### Key Features
1. **Product Recommendation Engine** (Collaborative Filtering)
2. **Dynamic Price Optimization** (Regression Models)
3. **Customer Sentiment Analysis** (Text Classification)
4. **Fraud Detection System** (Anomaly Detection)
5. **Sales Forecasting** (Time Series Analysis)
6. **Customer Segmentation** (Clustering)
7. **Product Image Classification** (Computer Vision)
8. **Inventory Demand Prediction** (Forecasting)

### Tech Stack
- **ML.NET** — Core machine learning framework
- **AI.NET** — Azure Cognitive Services integration
- **ASP.NET Core** — Web API and services
- **Entity Framework Core** — Data access
- **Azure ML** — Model deployment and monitoring
- **Docker** — Containerization
- **SignalR** — Real-time updates

## Learning Path

### Phase 1: Foundations (Modules 00-02)
**Duration**: 1-2 weeks
- Environment setup and ML.NET installation
- Understanding ML.NET architecture and pipeline
- Data loading, transformation, and feature engineering

### Phase 2: Core ML Algorithms (Modules 03-06)
**Duration**: 3-4 weeks
- Regression: Price prediction, demand forecasting
- Classification: Sentiment analysis, category prediction
- Clustering: Customer segmentation
- Anomaly Detection: Fraud detection, outlier identification

### Phase 3: Advanced Scenarios (Modules 07-10)
**Duration**: 3-4 weeks
- Recommendation systems
- Time series forecasting
- Computer vision with ML.NET
- Natural language processing

### Phase 4: Production Deployment (Modules 11-12)
**Duration**: 2-3 weeks
- Model deployment strategies
- REST API integration
- Azure AI Services
- Model monitoring and retraining

### Phase 5: Capstone Project (Module 13)
**Duration**: 2-3 weeks
- Complete e-commerce ML system
- Integration of multiple ML models
- Production-ready deployment
- Documentation and portfolio piece

## Prerequisites

### Required Knowledge
- C# programming fundamentals
- .NET Core/ASP.NET Core basics
- Basic understanding of databases and SQL
- Git version control

### Recommended (Not Required)
- Statistics fundamentals
- Linear algebra basics
- Previous exposure to machine learning concepts
- Cloud platform experience (Azure)

## Sample Projects in Each Module

1. **House Price Predictor** (Regression)
2. **Email Spam Classifier** (Binary Classification)
3. **Product Category Classifier** (Multi-class Classification)
4. **Customer Segmentation Tool** (Clustering)
5. **Credit Card Fraud Detector** (Anomaly Detection)
6. **Movie Recommendation Engine** (Recommendation)
7. **Sales Forecaster** (Time Series)
8. **Product Image Classifier** (Computer Vision)
9. **Sentiment Analyzer** (NLP)
10. **Complete E-Commerce Platform** (Capstone)

## Tools & Technologies

### Core Framework
- **ML.NET** 3.0+ — Microsoft's open-source ML framework
- **.NET 8.0** — Latest .NET runtime

### ML.NET Packages
```
Microsoft.ML
Microsoft.ML.Vision
Microsoft.ML.ImageAnalytics
Microsoft.ML.TimeSeries
Microsoft.ML.Recommender
Microsoft.ML.FastTree
Microsoft.ML.LightGbm
```

### AI.NET & Azure
```
Azure.AI.TextAnalytics
Azure.AI.Vision.ImageAnalysis
Azure.AI.OpenAI
Microsoft.Azure.CognitiveServices.Vision.ComputerVision
```

### Development Tools
- **Visual Studio 2022** or **VS Code**
- **ML.NET Model Builder** (VS extension)
- **Azure Machine Learning Studio**
- **Docker Desktop**
- **Postman** or **REST Client**

### Data Tools
- **ML.NET CLI** — Command-line model training
- **Jupyter Notebooks** (.NET Interactive)
- **TensorBoard** — Model visualization
- **Azure Storage Explorer** — Data management

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/iyyanar2405/interview-fullstackdeveloper.git
   cd interview-fullstackdeveloper/CSharp-AI-Modules-StepByStep
   ```

2. **Install .NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download
   - Verify: `dotnet --version`

3. **Install ML.NET packages**
   ```bash
   cd examples/src
   dotnet new console -n MLNetDemo
   cd MLNetDemo
   dotnet add package Microsoft.ML
   dotnet add package Microsoft.ML.Vision
   ```

4. **Start with Module 00**
   - Read: [00-Foundations-&-ML.NET-Setup.md](./00-Foundations-&-ML.NET-Setup.md)
   - Follow setup instructions
   - Complete hands-on exercises

5. **Track your progress**
   - Use [Tracking-Workbook.md](./Tracking-Workbook.md)
   - Complete exercises in each module
   - Build portfolio projects

## Project Structure

```
CSharp-AI-Modules-StepByStep/
├── README.md (this file)
├── TUTORIAL.md
├── Templates-&-Checklists.md
├── Tracking-Workbook.md
├── 00-Foundations-&-ML.NET-Setup.md
├── 01-ML.NET-Basics-&-Architecture.md
├── 02-Data-Loading-&-Preprocessing.md
├── 03-Regression-Models.md
├── 04-Classification-Models.md
├── 05-Clustering-Customer-Segmentation.md
├── 06-Anomaly-Detection.md
├── 07-Recommendation-Systems.md
├── 08-Time-Series-Forecasting.md
├── 09-Computer-Vision.md
├── 10-NLP-Text-Analysis.md
├── 11-Model-Deployment-&-API.md
├── 12-AI.NET-Azure-Integration.md
├── 13-Capstone-ECommerce-ML-System.md
└── examples/
    ├── data/                    # Sample datasets
    ├── models/                  # Trained model files
    └── src/
        ├── 01-Regression/       # Regression examples
        ├── 02-Classification/   # Classification examples
        ├── 03-Clustering/       # Clustering examples
        ├── 04-AnomalyDetection/ # Anomaly detection examples
        ├── 05-Recommendation/   # Recommendation examples
        ├── 06-TimeSeries/       # Time series examples
        ├── 07-ComputerVision/   # Computer vision examples
        ├── 08-NLP/              # NLP examples
        ├── 09-WebAPI/           # API deployment examples
        └── 10-Capstone/         # E-commerce capstone project
```

## Resources

### Official Documentation
- [ML.NET Documentation](https://docs.microsoft.com/dotnet/machine-learning/)
- [ML.NET Samples](https://github.com/dotnet/machinelearning-samples)
- [Azure AI Services](https://azure.microsoft.com/services/cognitive-services/)

### Community
- [ML.NET GitHub](https://github.com/dotnet/machinelearning)
- [.NET Community](https://dotnet.microsoft.com/platform/community)

### Additional Learning
- [Microsoft Learn - ML.NET](https://docs.microsoft.com/learn/paths/get-started-with-ml-dotnet/)
- [Machine Learning Crash Course](https://developers.google.com/machine-learning/crash-course)

## Contributing

This is a learning repository. Feel free to:
- Submit issues for clarifications
- Suggest improvements
- Share your completed projects
- Add additional examples

## License

Educational use - Part of the interview-fullstackdeveloper repository.

---

**Ready to start?** Begin with [Module 00: Foundations & ML.NET Setup](./00-Foundations-&-ML.NET-Setup.md)

**Questions?** Check [TUTORIAL.md](./TUTORIAL.md) for detailed guidance.

**Track Progress:** Use [Tracking-Workbook.md](./Tracking-Workbook.md) to monitor your learning journey.
