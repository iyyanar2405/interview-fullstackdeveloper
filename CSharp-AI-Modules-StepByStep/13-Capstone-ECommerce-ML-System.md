# Module 13: Capstone - E-Commerce ML System

Build a complete, production-ready e-commerce intelligence system integrating multiple ML.NET models and AI.NET services for a real-time application.

## Project Overview

### E-Commerce Intelligence Platform

A comprehensive ML-powered e-commerce system featuring:
- **Product Recommendations** (Collaborative Filtering)
- **Dynamic Pricing** (Regression)
- **Sentiment Analysis** (Text Classification)
- **Fraud Detection** (Anomaly Detection)
- **Sales Forecasting** (Time Series)
- **Customer Segmentation** (Clustering)
- **Image Classification** (Computer Vision)
- **Demand Prediction** (Forecasting)

### Architecture

```
┌─────────────────────────────────────────────────────┐
│              ASP.NET Core Web API                    │
├─────────────────────────────────────────────────────┤
│  Controllers: Products, Orders, Analytics, ML       │
├─────────────────────────────────────────────────────┤
│              ML Service Layer                        │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐           │
│  │ Recommend│ │ Pricing  │ │Sentiment │           │
│  │  Engine  │ │Optimizer │ │ Analyzer │           │
│  └──────────┘ └──────────┘ └──────────┘           │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐           │
│  │  Fraud   │ │  Sales   │ │Customer  │           │
│  │ Detector │ │Forecaster│ │Segmenter │           │
│  └──────────┘ └──────────┘ └──────────┘           │
├─────────────────────────────────────────────────────┤
│            Data Access Layer (EF Core)               │
├─────────────────────────────────────────────────────┤
│         SQL Server / PostgreSQL Database             │
└─────────────────────────────────────────────────────┘
```

## Part 1: Project Setup

### Step 1: Create Solution Structure

```bash
# Create solution
dotnet new sln -n ECommerceMLPlatform

# Create projects
dotnet new webapi -n ECommerceML.API
dotnet new classlib -n ECommerceML.Core
dotnet new classlib -n ECommerceML.ML
dotnet new classlib -n ECommerceML.Data
dotnet new xunit -n ECommerceML.Tests

# Add projects to solution
dotnet sln add ECommerceML.API/ECommerceML.API.csproj
dotnet sln add ECommerceML.Core/ECommerceML.Core.csproj
dotnet sln add ECommerceML.ML/ECommerceML.ML.csproj
dotnet sln add ECommerceML.Data/ECommerceML.Data.csproj
dotnet sln add ECommerceML.Tests/ECommerceML.Tests.csproj

# Add project references
cd ECommerceML.API
dotnet add reference ../ECommerceML.Core/ECommerceML.Core.csproj
dotnet add reference ../ECommerceML.ML/ECommerceML.ML.csproj
dotnet add reference ../ECommerceML.Data/ECommerceML.Data.csproj

cd ../ECommerceML.ML
dotnet add reference ../ECommerceML.Core/ECommerceML.Core.csproj
dotnet add reference ../ECommerceML.Data/ECommerceML.Data.csproj
```

### Step 2: Install Required Packages

```bash
# ML.NET packages
cd ECommerceML.ML
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.Recommender
dotnet add package Microsoft.ML.TimeSeries
dotnet add package Microsoft.ML.FastTree
dotnet add package Microsoft.ML.LightGbm
dotnet add package Microsoft.ML.ImageAnalytics
dotnet add package Microsoft.ML.Vision

# Azure AI packages
dotnet add package Azure.AI.TextAnalytics
dotnet add package Azure.AI.Vision.ImageAnalysis

# API packages
cd ../ECommerceML.API
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Serilog.AspNetCore

# Data packages
cd ../ECommerceML.Data
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

## Part 2: Core Domain Models

### ECommerceML.Core/Models/Product.cs

```csharp
namespace ECommerceML.Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class Review
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public Product Product { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime JoinDate { get; set; }
        public string CustomerSegment { get; set; }
        
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public bool IsFraudulent { get; set; }
        
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
```

## Part 3: ML Models Implementation

### 1. Product Recommendation Engine

**ECommerceML.ML/Services/RecommendationService.cs**

```csharp
using Microsoft.ML;
using Microsoft.ML.Trainers;
using ECommerceML.Core.Models;

namespace ECommerceML.ML.Services
{
    public class ProductRating
    {
        public float UserId { get; set; }
        public float ProductId { get; set; }
        public float Label { get; set; }
    }

    public class ProductPrediction
    {
        public float Score { get; set; }
    }

    public class RecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly string _modelPath = "models/recommendation_model.zip";

        public RecommendationService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<(int UserId, int ProductId, float Rating)> ratings)
        {
            var trainingData = ratings.Select(r => new ProductRating
            {
                UserId = r.UserId,
                ProductId = r.ProductId,
                Label = r.Rating
            });

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(ProductRating.UserId),
                MatrixRowIndexColumnName = nameof(ProductRating.ProductId),
                LabelColumnName = nameof(ProductRating.Label),
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var pipeline = _mlContext.Recommendation().Trainers
                .MatrixFactorization(options);

            _model = pipeline.Fit(dataView);
            
            // Save model
            _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
        }

        public List<(int ProductId, float Score)> GetRecommendations(
            int userId, 
            List<int> allProductIds, 
            int topN = 10)
        {
            if (_model == null)
            {
                LoadModel();
            }

            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<ProductRating, ProductPrediction>(_model);

            var recommendations = new List<(int ProductId, float Score)>();

            foreach (var productId in allProductIds)
            {
                var prediction = predictionEngine.Predict(new ProductRating
                {
                    UserId = userId,
                    ProductId = productId
                });

                recommendations.Add((productId, prediction.Score));
            }

            return recommendations
                .OrderByDescending(r => r.Score)
                .Take(topN)
                .ToList();
        }

        private void LoadModel()
        {
            using var stream = File.OpenRead(_modelPath);
            _model = _mlContext.Model.Load(stream, out _);
        }
    }
}
```

### 2. Dynamic Pricing Optimizer

**ECommerceML.ML/Services/PricingService.cs**

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ECommerceML.ML.Services
{
    public class PricingData
    {
        public float CompetitorPrice { get; set; }
        public float HistoricalDemand { get; set; }
        public float SeasonalityIndex { get; set; }
        public float StockLevel { get; set; }
        public float CategoryAverage { get; set; }
        [ColumnName("Label")]
        public float OptimalPrice { get; set; }
    }

    public class PricePrediction
    {
        [ColumnName("Score")]
        public float OptimalPrice { get; set; }
    }

    public class PricingService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public PricingService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<PricingData> historicalData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(historicalData);

            var pipeline = _mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(PricingData.CompetitorPrice),
                    nameof(PricingData.HistoricalDemand),
                    nameof(PricingData.SeasonalityIndex),
                    nameof(PricingData.StockLevel),
                    nameof(PricingData.CategoryAverage))
                .Append(_mlContext.Regression.Trainers.FastTree());

            _model = pipeline.Fit(dataView);
        }

        public decimal CalculateOptimalPrice(
            decimal competitorPrice,
            int historicalDemand,
            float seasonalityIndex,
            int stockLevel,
            decimal categoryAverage)
        {
            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<PricingData, PricePrediction>(_model);

            var prediction = predictionEngine.Predict(new PricingData
            {
                CompetitorPrice = (float)competitorPrice,
                HistoricalDemand = historicalDemand,
                SeasonalityIndex = seasonalityIndex,
                StockLevel = stockLevel,
                CategoryAverage = (float)categoryAverage
            });

            return (decimal)prediction.OptimalPrice;
        }
    }
}
```

### 3. Sentiment Analysis Service

**ECommerceML.ML/Services/SentimentService.cs**

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ECommerceML.ML.Services
{
    public class SentimentData
    {
        public string ReviewText { get; set; }
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsPositive { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly string _modelPath = "models/sentiment_model.zip";

        public SentimentService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<(string Text, bool IsPositive)> reviews)
        {
            var trainingData = reviews.Select(r => new SentimentData
            {
                ReviewText = r.Text,
                Sentiment = r.IsPositive
            });

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(SentimentData.ReviewText))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SentimentData.Sentiment),
                    featureColumnName: "Features"));

            _model = pipeline.Fit(dataView);
            _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
        }

        public (bool IsPositive, float Confidence) AnalyzeSentiment(string reviewText)
        {
            if (_model == null)
            {
                LoadModel();
            }

            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);

            var prediction = predictionEngine.Predict(new SentimentData
            {
                ReviewText = reviewText
            });

            return (prediction.IsPositive, prediction.Probability);
        }

        private void LoadModel()
        {
            using var stream = File.OpenRead(_modelPath);
            _model = _mlContext.Model.Load(stream, out _);
        }
    }
}
```

### 4. Fraud Detection Service

**ECommerceML.ML/Services/FraudDetectionService.cs**

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ECommerceML.ML.Services
{
    public class TransactionData
    {
        public float Amount { get; set; }
        public float TimeOfDay { get; set; }
        public float LocationDistance { get; set; }
        public float TransactionFrequency { get; set; }
        public float AccountAge { get; set; }
    }

    public class FraudPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsFraud { get; set; }
        
        [ColumnName("Score")]
        public float Score { get; set; }
    }

    public class FraudDetectionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public FraudDetectionService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<TransactionData> transactions)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(transactions);

            var pipeline = _mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(TransactionData.Amount),
                    nameof(TransactionData.TimeOfDay),
                    nameof(TransactionData.LocationDistance),
                    nameof(TransactionData.TransactionFrequency),
                    nameof(TransactionData.AccountAge))
                .Append(_mlContext.AnomalyDetection.Trainers.RandomizedPca(
                    featureColumnName: "Features",
                    rank: 5,
                    ensureZeroMean: true));

            _model = pipeline.Fit(dataView);
        }

        public (bool IsFraud, float RiskScore) DetectFraud(
            decimal amount,
            int timeOfDay,
            double locationDistance,
            int transactionFrequency,
            int accountAge)
        {
            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<TransactionData, FraudPrediction>(_model);

            var prediction = predictionEngine.Predict(new TransactionData
            {
                Amount = (float)amount,
                TimeOfDay = timeOfDay,
                LocationDistance = (float)locationDistance,
                TransactionFrequency = transactionFrequency,
                AccountAge = accountAge
            });

            return (prediction.IsFraud, prediction.Score);
        }
    }
}
```

## Part 4: Web API Implementation

### ECommerceML.API/Controllers/MLController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using ECommerceML.ML.Services;

namespace ECommerceML.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MLController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;
        private readonly PricingService _pricingService;
        private readonly SentimentService _sentimentService;
        private readonly FraudDetectionService _fraudService;

        public MLController(
            RecommendationService recommendationService,
            PricingService pricingService,
            SentimentService sentimentService,
            FraudDetectionService fraudService)
        {
            _recommendationService = recommendationService;
            _pricingService = pricingService;
            _sentimentService = sentimentService;
            _fraudService = fraudService;
        }

        [HttpGet("recommendations/{userId}")]
        public IActionResult GetRecommendations(int userId, [FromQuery] int count = 10)
        {
            // Get all product IDs from database
            var allProductIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            
            var recommendations = _recommendationService.GetRecommendations(
                userId, 
                allProductIds, 
                count);

            return Ok(new
            {
                UserId = userId,
                Recommendations = recommendations.Select(r => new
                {
                    ProductId = r.ProductId,
                    Score = r.Score
                })
            });
        }

        [HttpPost("pricing/optimize")]
        public IActionResult OptimizePrice([FromBody] PriceOptimizationRequest request)
        {
            var optimalPrice = _pricingService.CalculateOptimalPrice(
                request.CompetitorPrice,
                request.HistoricalDemand,
                request.SeasonalityIndex,
                request.StockLevel,
                request.CategoryAverage);

            return Ok(new
            {
                OptimalPrice = optimalPrice,
                CurrentPrice = request.CompetitorPrice,
                PriceChange = optimalPrice - request.CompetitorPrice
            });
        }

        [HttpPost("sentiment/analyze")]
        public IActionResult AnalyzeSentiment([FromBody] SentimentRequest request)
        {
            var (isPositive, confidence) = _sentimentService.AnalyzeSentiment(request.Text);

            return Ok(new
            {
                Text = request.Text,
                IsPositive = isPositive,
                Sentiment = isPositive ? "Positive" : "Negative",
                Confidence = confidence
            });
        }

        [HttpPost("fraud/detect")]
        public IActionResult DetectFraud([FromBody] FraudDetectionRequest request)
        {
            var (isFraud, riskScore) = _fraudService.DetectFraud(
                request.Amount,
                request.TimeOfDay,
                request.LocationDistance,
                request.TransactionFrequency,
                request.AccountAge);

            return Ok(new
            {
                IsFraud = isFraud,
                RiskScore = riskScore,
                RiskLevel = riskScore > 0.8f ? "High" : riskScore > 0.5f ? "Medium" : "Low"
            });
        }
    }

    public class PriceOptimizationRequest
    {
        public decimal CompetitorPrice { get; set; }
        public int HistoricalDemand { get; set; }
        public float SeasonalityIndex { get; set; }
        public int StockLevel { get; set; }
        public decimal CategoryAverage { get; set; }
    }

    public class SentimentRequest
    {
        public string Text { get; set; }
    }

    public class FraudDetectionRequest
    {
        public decimal Amount { get; set; }
        public int TimeOfDay { get; set; }
        public double LocationDistance { get; set; }
        public int TransactionFrequency { get; set; }
        public int AccountAge { get; set; }
    }
}
```

### Program.cs Configuration

```csharp
using ECommerceML.ML.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ML services as singletons
builder.Services.AddSingleton<RecommendationService>();
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<SentimentService>();
builder.Services.AddSingleton<FraudDetectionService>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Part 5: Testing & Deployment

### Sample Test Data Generator

```csharp
public class TestDataGenerator
{
    public static List<(int UserId, int ProductId, float Rating)> GenerateRatings()
    {
        var ratings = new List<(int, int, float)>();
        var random = new Random(42);

        for (int userId = 1; userId <= 100; userId++)
        {
            int numRatings = random.Next(5, 20);
            for (int i = 0; i < numRatings; i++)
            {
                int productId = random.Next(1, 50);
                float rating = (float)(random.Next(1, 6));
                ratings.Add((userId, productId, rating));
            }
        }

        return ratings;
    }
}
```

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ECommerceML.API/ECommerceML.API.csproj", "ECommerceML.API/"]
COPY ["ECommerceML.ML/ECommerceML.ML.csproj", "ECommerceML.ML/"]
COPY ["ECommerceML.Core/ECommerceML.Core.csproj", "ECommerceML.Core/"]
RUN dotnet restore "ECommerceML.API/ECommerceML.API.csproj"
COPY . .
WORKDIR "/src/ECommerceML.API"
RUN dotnet build "ECommerceML.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ECommerceML.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerceML.API.dll"]
```

## Part 6: Running the Application

```bash
# Build solution
dotnet build

# Run API
cd ECommerceML.API
dotnet run

# Access Swagger UI
# http://localhost:5000/swagger

# Test recommendations endpoint
curl http://localhost:5000/api/ml/recommendations/1?count=5

# Test sentiment analysis
curl -X POST http://localhost:5000/api/ml/sentiment/analyze \
  -H "Content-Type: application/json" \
  -d '{"text": "This product is amazing!"}'
```

## Summary

You built:
- ✅ Complete e-commerce ML system
- ✅ 4 integrated ML models
- ✅ RESTful API with ML endpoints
- ✅ Production-ready architecture
- ✅ Docker containerization
- ✅ Real-time predictions

## Next Steps

1. Add more ML models (Time Series, Clustering)
2. Implement model monitoring
3. Add Azure AI.NET integration
4. Deploy to Azure/AWS
5. Add CI/CD pipeline
6. Implement A/B testing

**Congratulations!** You've completed all modules and built a production-ready ML system.

---

**Previous:** [Module 12: AI.NET Integration](./12-AI.NET-Azure-Integration.md)
