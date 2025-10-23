# Module 05 - Model Management

Comprehensive model management system with intelligent selection, fallback mechanisms, cost optimization, and performance monitoring.

## Overview

This module provides a production-ready system for managing multiple AI models with:
- **Intelligent Model Selection**: Choose the best model based on requirements, cost, and performance
- **Fallback Mechanisms**: Automatic failover when models are unavailable or degraded
- **Cost Optimization**: Track spending, enforce budgets, and optimize costs
- **Performance Monitoring**: Real-time health checks and performance analytics

## Features

### 1. Model Selection Strategies

#### Cost Optimized
Selects the cheapest model that meets requirements:
```csharp
var request = new ModelSelectionRequest
{
    RequiredCapabilities = new List<ModelCapability> 
    { 
        ModelCapability.ChatCompletion 
    },
    Strategy = SelectionStrategy.CostOptimized
};

var response = await modelSelection.SelectModelAsync(request);
// Returns: gpt-3.5-turbo (lowest cost)
```

#### Performance Optimized
Prioritizes speed and quality over cost:
```csharp
var request = new ModelSelectionRequest
{
    RequiredCapabilities = new List<ModelCapability> 
    { 
        ModelCapability.Reasoning 
    },
    Strategy = SelectionStrategy.PerformanceOptimized
};

var response = await modelSelection.SelectModelAsync(request);
// Returns: claude-3-opus (best reasoning)
```

#### Balanced
Optimizes for both cost and performance:
```csharp
var request = new ModelSelectionRequest
{
    RequiredCapabilities = new List<ModelCapability> 
    { 
        ModelCapability.CodeGeneration 
    },
    Strategy = SelectionStrategy.Balanced,
    Constraints = new ModelConstraints
    {
        MaxCostPerRequest = 0.01m,
        MaxLatencyMs = 2000
    }
};

var response = await modelSelection.SelectModelAsync(request);
```

#### Weighted
Custom scoring based on specific factors:
```csharp
var request = new ModelSelectionRequest
{
    Strategy = SelectionStrategy.Weighted,
    WeightingFactors = new Dictionary<string, double>
    {
        { "cost", 0.3 },
        { "performance", 0.5 },
        { "context", 0.2 }
    }
};

var response = await modelSelection.SelectModelAsync(request);
```

### 2. Fallback Mechanisms

#### Sequential Fallback
Try fallback models in priority order:
```csharp
var fallbackRequest = new FallbackRequest
{
    OriginalModelId = "gpt-4-turbo",
    FailureReason = "RateLimitExceeded",
    RequiredCapabilities = new List<ModelCapability> 
    { 
        ModelCapability.ChatCompletion 
    }
};

var fallbackResponse = await fallback.GetFallbackModelAsync(fallbackRequest);
// Returns: gpt-3.5-turbo (next available model)
```

#### Cost-Based Fallback
Switch to cheaper alternatives:
```csharp
// Configure fallback strategy
var config = new FallbackConfiguration
{
    Strategy = FallbackStrategy.CostBased,
    MaxRetries = 3,
    EnableCircuitBreaker = true
};
```

#### Record Failures for Automatic Fallback
```csharp
// Record failure
await fallback.RecordFailureAsync("gpt-4-turbo", "Timeout");

// Check if fallback should trigger
var shouldTrigger = await fallback.ShouldTriggerFallbackAsync(
    "gpt-4-turbo", 
    "Timeout"
);

if (shouldTrigger)
{
    var fallbackResponse = await fallback.GetFallbackModelAsync(fallbackRequest);
    // Use fallback model
}
```

### 3. Cost Optimization

#### Track Costs
```csharp
var trackingRequest = new CostTrackingRequest
{
    ModelId = "gpt-4-turbo",
    InputTokens = 500,
    OutputTokens = 200,
    UserId = "user123",
    Tags = new Dictionary<string, string>
    {
        { "department", "engineering" },
        { "project", "chatbot" }
    }
};

var costEntry = await costOptimization.TrackCostAsync(trackingRequest);
// Cost: $0.011 (500 * $0.01/1K + 200 * $0.03/1K)
```

#### Budget Management
```csharp
// Check if within budget
var estimatedCost = 0.05m;
var withinBudget = await costOptimization.CheckBudgetAsync(estimatedCost);

if (!withinBudget)
{
    // Use cheaper model or throttle requests
}

// Get remaining budget
var dailyRemaining = await costOptimization.GetRemainingBudgetAsync("daily");
var monthlyRemaining = await costOptimization.GetRemainingBudgetAsync("monthly");
```

#### Cost Reports
```csharp
var report = await costOptimization.GetCostReportAsync(
    DateTime.UtcNow.AddDays(-7),
    DateTime.UtcNow
);

Console.WriteLine($"Total Cost: ${report.TotalCost}");
Console.WriteLine($"Average Daily: ${report.Trend.AverageDailyCost}");
Console.WriteLine($"Projected Monthly: ${report.Trend.ProjectedMonthlyCost}");

// Cost by provider
foreach (var (provider, cost) in report.CostByProvider)
{
    Console.WriteLine($"{provider}: ${cost}");
}
```

#### Cost Alerts
```csharp
var alerts = await costOptimization.GetActiveAlertsAsync();

foreach (var alert in alerts)
{
    Console.WriteLine($"⚠️ {alert.Message}");
    Console.WriteLine($"   Current: ${alert.CurrentAmount}");
    Console.WriteLine($"   Threshold: ${alert.ThresholdAmount}");
}
```

### 4. Performance Monitoring

#### Record Metrics
```csharp
var metrics = new PerformanceMetrics
{
    ModelId = "gpt-4-turbo",
    LatencyMs = 1250,
    TokensPerSecond = 45,
    Success = true,
    QualityScore = 0.95
};

await performanceMonitoring.RecordMetricsAsync(metrics);
```

#### Health Checks
```csharp
// Check specific model
var health = await performanceMonitoring.CheckModelHealthAsync("gpt-4-turbo");

Console.WriteLine($"Status: {health.Status}");
Console.WriteLine($"Success Rate: {health.SuccessRate:P1}");
Console.WriteLine($"Avg Latency: {health.AverageLatencyMs}ms");

if (health.Issues.Any())
{
    Console.WriteLine("Issues:");
    foreach (var issue in health.Issues)
    {
        Console.WriteLine($"  - {issue}");
    }
}

// Check all models
var allHealth = await performanceMonitoring.GetAllModelsHealthAsync();
foreach (var (modelId, status) in allHealth)
{
    Console.WriteLine($"{modelId}: {status}");
}
```

#### Performance Reports
```csharp
var report = await performanceMonitoring.GetPerformanceReportAsync(
    DateTime.UtcNow.AddDays(-7),
    DateTime.UtcNow
);

// Overall summary
Console.WriteLine($"Total Requests: {report.Summary.TotalRequests}");
Console.WriteLine($"Success Rate: {report.Summary.OverallSuccessRate:P1}");
Console.WriteLine($"Avg Latency: {report.Summary.AverageLatencyMs}ms");

// Per-model performance
foreach (var (modelId, perf) in report.ModelPerformance)
{
    Console.WriteLine($"\n{modelId}:");
    Console.WriteLine($"  Requests: {perf.TotalRequests}");
    Console.WriteLine($"  Success: {perf.SuccessRate:P1}");
    Console.WriteLine($"  Avg Latency: {perf.AverageLatencyMs}ms");
    Console.WriteLine($"  P95 Latency: {perf.P95LatencyMs}ms");
    Console.WriteLine($"  P99 Latency: {perf.P99LatencyMs}ms");
}
```

#### Recommendations
```csharp
var recommendations = await performanceMonitoring.GetRecommendationsAsync();

foreach (var recommendation in recommendations)
{
    Console.WriteLine($"💡 {recommendation}");
}

// Example output:
// 💡 Most reliable model: gpt-3.5-turbo with 99.5% success rate
// 💡 Fastest model: gpt-3.5-turbo with 850ms average latency
// 💡 Consider replacing gpt-4-turbo - low success rate (85.0%)
```

## API Endpoints

### Model Selection

```http
POST /api/modelmanagement/select
Content-Type: application/json

{
  "taskDescription": "Generate Python code",
  "requiredCapabilities": ["CodeGeneration"],
  "strategy": "Balanced",
  "constraints": {
    "maxCostPerRequest": 0.01,
    "maxLatencyMs": 2000
  }
}
```

```http
GET /api/modelmanagement/models
GET /api/modelmanagement/models/{modelId}
PUT /api/modelmanagement/models/{modelId}/status
```

### Fallback

```http
POST /api/modelmanagement/fallback
POST /api/modelmanagement/fallback/record-failure?modelId=gpt-4&errorType=Timeout
GET /api/modelmanagement/fallback/should-trigger?modelId=gpt-4&errorType=RateLimit
```

### Cost Optimization

```http
POST /api/modelmanagement/cost/track
GET /api/modelmanagement/cost/report?startDate=2024-01-01&endDate=2024-01-31
GET /api/modelmanagement/cost/check-budget?estimatedCost=0.05
GET /api/modelmanagement/cost/alerts
GET /api/modelmanagement/cost/remaining-budget?period=daily
```

### Performance Monitoring

```http
POST /api/modelmanagement/performance/record
GET /api/modelmanagement/performance/health/{modelId}
GET /api/modelmanagement/performance/health
GET /api/modelmanagement/performance/report?startDate=2024-01-01&endDate=2024-01-31
GET /api/modelmanagement/performance/recommendations
```

### Dashboard

```http
GET /api/modelmanagement/dashboard
GET /api/modelmanagement/stats
```

## Configuration

### appsettings.json

```json
{
  "ModelManagement": {
    "Models": [
      {
        "ModelId": "gpt-4-turbo",
        "Name": "GPT-4 Turbo",
        "Provider": "OpenAI",
        "Capabilities": ["TextGeneration", "ChatCompletion", "CodeGeneration"],
        "Pricing": {
          "InputTokenCost": 0.01,
          "OutputTokenCost": 0.03,
          "RequestCost": 0.0
        },
        "Limits": {
          "MaxTokens": 4096,
          "MaxRequestsPerMinute": 3500,
          "ContextWindow": 128000
        },
        "Status": "Available",
        "Priority": 1
      }
    ],
    "FallbackConfig": {
      "Strategy": "Sequential",
      "MaxRetries": 3,
      "EnableCircuitBreaker": true
    },
    "CostConfig": {
      "DailyBudget": 100.0,
      "MonthlyBudget": 2000.0,
      "EnableCostAlerts": true,
      "AlertThresholdPercentage": 80.0
    },
    "MonitoringSettings": {
      "EnablePerformanceTracking": true,
      "EnableCostTracking": true,
      "EnableHealthChecks": true,
      "MetricsRetentionDays": 30
    }
  }
}
```

## Usage Patterns

### Pattern 1: Intelligent Model Selection with Fallback

```csharp
public async Task<string> GenerateResponseWithFallback(string prompt)
{
    // Select best model
    var selectionRequest = new ModelSelectionRequest
    {
        RequiredCapabilities = new List<ModelCapability> { ModelCapability.ChatCompletion },
        Strategy = SelectionStrategy.Balanced,
        Constraints = new ModelConstraints
        {
            MaxCostPerRequest = 0.02m,
            MaxLatencyMs = 3000
        }
    };

    var selection = await _modelSelection.SelectModelAsync(selectionRequest);
    var selectedModel = selection.SelectedModel;

    try
    {
        var sw = Stopwatch.StartNew();
        var response = await CallModelAPI(selectedModel.ModelId, prompt);
        sw.Stop();

        // Record success metrics
        await _performanceMonitoring.RecordMetricsAsync(new PerformanceMetrics
        {
            ModelId = selectedModel.ModelId,
            LatencyMs = (int)sw.ElapsedMilliseconds,
            Success = true,
            TokensPerSecond = CalculateTokensPerSecond(response, sw.ElapsedMilliseconds)
        });

        // Track cost
        await _costOptimization.TrackCostAsync(new CostTrackingRequest
        {
            ModelId = selectedModel.ModelId,
            InputTokens = CountTokens(prompt),
            OutputTokens = CountTokens(response)
        });

        return response;
    }
    catch (Exception ex)
    {
        // Record failure
        await _performanceMonitoring.RecordMetricsAsync(new PerformanceMetrics
        {
            ModelId = selectedModel.ModelId,
            Success = false,
            ErrorType = ex.GetType().Name
        });

        await _fallback.RecordFailureAsync(selectedModel.ModelId, ex.GetType().Name);

        // Try fallback
        var fallbackRequest = new FallbackRequest
        {
            OriginalModelId = selectedModel.ModelId,
            FailureReason = ex.Message,
            RequiredCapabilities = selectionRequest.RequiredCapabilities
        };

        var fallbackResponse = await _fallback.GetFallbackModelAsync(fallbackRequest);
        
        if (fallbackResponse.Success && fallbackResponse.FallbackModel != null)
        {
            return await CallModelAPI(fallbackResponse.FallbackModel.ModelId, prompt);
        }

        throw new Exception("All models failed", ex);
    }
}
```

### Pattern 2: Cost-Aware Processing

```csharp
public async Task<string> ProcessWithBudgetControl(string prompt)
{
    // Check budget before processing
    var estimatedCost = EstimateCost(prompt);
    var withinBudget = await _costOptimization.CheckBudgetAsync(estimatedCost);

    if (!withinBudget)
    {
        // Switch to cheaper model
        var selectionRequest = new ModelSelectionRequest
        {
            Strategy = SelectionStrategy.CostOptimized,
            RequiredCapabilities = new List<ModelCapability> 
            { 
                ModelCapability.TextGeneration 
            }
        };

        var selection = await _modelSelection.SelectModelAsync(selectionRequest);
        return await CallModelAPI(selection.SelectedModel.ModelId, prompt);
    }

    // Use preferred model
    return await CallModelAPI("gpt-4-turbo", prompt);
}
```

### Pattern 3: Performance-Based Routing

```csharp
public async Task<string> RouteBasedOnHealth(string prompt, bool requireHighQuality)
{
    var allHealth = await _performanceMonitoring.GetAllModelsHealthAsync();

    var healthyModels = allHealth
        .Where(h => h.Value == HealthStatus.Healthy)
        .Select(h => h.Key)
        .ToList();

    if (!healthyModels.Any())
    {
        throw new Exception("No healthy models available");
    }

    var selectionRequest = new ModelSelectionRequest
    {
        Strategy = requireHighQuality 
            ? SelectionStrategy.PerformanceOptimized 
            : SelectionStrategy.Balanced,
        Constraints = new ModelConstraints
        {
            ExcludedModels = allHealth
                .Where(h => h.Value != HealthStatus.Healthy)
                .Select(h => h.Key)
                .ToList()
        }
    };

    var selection = await _modelSelection.SelectModelAsync(selectionRequest);
    return await CallModelAPI(selection.SelectedModel.ModelId, prompt);
}
```

## Best Practices

### 1. Model Selection
- **Use constraints** to filter models based on requirements
- **Prefer balanced strategy** for general use cases
- **Cache selection results** for similar requests
- **Consider context window** for long prompts

### 2. Fallback Configuration
- **Define fallback chains** for critical services
- **Enable circuit breakers** to prevent cascading failures
- **Monitor fallback frequency** to identify issues
- **Maintain capability requirements** in fallbacks

### 3. Cost Optimization
- **Set realistic budgets** based on usage patterns
- **Enable alerts** at 80% threshold
- **Track costs by user/project** for chargeback
- **Review cost reports** weekly

### 4. Performance Monitoring
- **Record all metrics** for comprehensive analysis
- **Set up health checks** with appropriate intervals
- **Monitor P95/P99 latencies** for SLA compliance
- **Act on recommendations** promptly

### 5. Production Deployment
- **Use Redis cache** for distributed scenarios
- **Enable correlation IDs** for tracing
- **Configure retry policies** appropriately
- **Set up alerting** for critical metrics

## Troubleshooting

### High Costs
```csharp
// Check cost breakdown
var report = await _costOptimization.GetCostReportAsync(
    DateTime.UtcNow.AddDays(-1),
    DateTime.UtcNow
);

// Identify expensive models
var topExpensive = report.CostByModel
    .OrderByDescending(kv => kv.Value)
    .Take(3);

// Switch to cost-optimized strategy
var request = new ModelSelectionRequest
{
    Strategy = SelectionStrategy.CostOptimized
};
```

### Low Success Rates
```csharp
// Get recommendations
var recommendations = await _performanceMonitoring.GetRecommendationsAsync();

// Check model health
var health = await _performanceMonitoring.GetAllModelsHealthAsync();

// Exclude unhealthy models
var constraints = new ModelConstraints
{
    ExcludedModels = health
        .Where(h => h.Value == HealthStatus.Unhealthy)
        .Select(h => h.Key)
        .ToList()
};
```

### High Latency
```csharp
// Get performance report
var report = await _performanceMonitoring.GetPerformanceReportAsync(
    DateTime.UtcNow.AddHours(-1),
    DateTime.UtcNow
);

// Find fastest model
var fastest = report.ModelPerformance
    .OrderBy(kv => kv.Value.AverageLatencyMs)
    .First();

// Use performance-optimized strategy
var request = new ModelSelectionRequest
{
    Strategy = SelectionStrategy.PerformanceOptimized
};
```

## Running the Application

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Access Swagger UI
# https://localhost:5001/swagger
```

## Testing

```bash
# Test model selection
curl -X POST https://localhost:5001/api/modelmanagement/select \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": "Balanced",
    "requiredCapabilities": ["ChatCompletion"]
  }'

# Get dashboard
curl https://localhost:5001/api/modelmanagement/dashboard

# Check health
curl https://localhost:5001/api/modelmanagement/performance/health
```

## Integration Example

```csharp
// Startup.cs or Program.cs
builder.Services.AddModelManagement(builder.Configuration);

// In your service
public class AIService
{
    private readonly IModelSelectionService _modelSelection;
    private readonly IFallbackService _fallback;
    private readonly ICostOptimizationService _costOptimization;
    private readonly IPerformanceMonitoringService _performanceMonitoring;

    public AIService(
        IModelSelectionService modelSelection,
        IFallbackService fallback,
        ICostOptimizationService costOptimization,
        IPerformanceMonitoringService performanceMonitoring)
    {
        _modelSelection = modelSelection;
        _fallback = fallback;
        _costOptimization = costOptimization;
        _performanceMonitoring = performanceMonitoring;
    }

    public async Task<string> ProcessRequest(string prompt)
    {
        // Your implementation using the services
    }
}
```

## Summary

Module 05 provides comprehensive model management capabilities:

✅ **6 Selection Strategies** - Cost, performance, balanced, capability, load-balanced, weighted  
✅ **5 Fallback Strategies** - Sequential, cost-based, quality-based, load-based, random  
✅ **Complete Cost Tracking** - Budgets, alerts, reports, projections  
✅ **Performance Monitoring** - Health checks, metrics, recommendations, reports  
✅ **20+ API Endpoints** - Full REST API for all features  
✅ **Production Ready** - Caching, retry policies, circuit breakers, logging  

This module enables intelligent, cost-effective, and reliable AI model management for production applications.
