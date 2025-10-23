# Module 02: Embeddings

## Overview
This module provides comprehensive embedding generation capabilities for RAG systems, including OpenAI embeddings, Azure OpenAI, similarity calculations, batch processing, caching, and semantic search.

## Features

### 🔢 Embedding Generation
- **OpenAI Models**: text-embedding-3-small, text-embedding-3-large, ada-002
- **Azure OpenAI**: Azure-hosted embedding models
- **Custom Dimensions**: Support for dimension reduction
- **Batch Processing**: Efficient bulk embedding generation
- **Caching**: Memory and Redis caching strategies

### 📊 Similarity Metrics
1. **Cosine Similarity**: Measures angle between vectors
2. **Euclidean Distance**: L2 distance between points
3. **Dot Product**: Inner product similarity
4. **Manhattan Distance**: L1 distance (taxicab geometry)
5. **Jaccard Similarity**: Set-based similarity (planned)

### 🔍 Semantic Search
- Query-based document search
- Top-K retrieval with scoring
- Threshold-based filtering
- Metadata preservation

### 💾 Caching Strategies
- **Memory Cache**: Fast in-memory caching
- **Redis Cache**: Distributed caching
- **Hybrid Cache**: Combined memory + Redis
- **No Cache**: Direct API calls

### 📈 Analytics & Monitoring
- Token usage tracking
- Cost estimation
- Cache hit/miss rates
- Model usage statistics
- Provider analytics

## Installation

### Prerequisites
```powershell
# Install .NET 8.0 SDK
dotnet --version  # Should be 8.0 or higher
```

### Restore Dependencies
```powershell
cd Module-02-Embeddings
dotnet restore
```

### Configuration
Set your OpenAI API key in `appsettings.json`:
```json
{
  "EmbeddingSettings": {
    "OpenAIApiKey": "sk-your-actual-api-key-here"
  }
}
```

Or use environment variables:
```powershell
$env:OPENAI_API_KEY = "sk-your-actual-api-key-here"
```

## Configuration

### appsettings.json
```json
{
  "EmbeddingSettings": {
    "OpenAIApiKey": "your-openai-api-key",
    "AzureEndpoint": "https://your-resource.openai.azure.com/",
    "AzureApiKey": "your-azure-key",
    "DefaultModel": "TextEmbedding3Small",
    "DefaultProvider": "OpenAI",
    "CacheStrategy": "Memory",
    "CacheDurationMinutes": 60,
    "MaxCacheEntries": 10000,
    "DefaultBatchSize": 100,
    "MaxConcurrency": 5,
    "EnableCaching": true,
    "EnableMetrics": true
  }
}
```

## Usage Examples

### 1. Generate Embeddings

#### Single Text Embedding
```csharp
var embeddingService = serviceProvider.GetRequiredService<IEmbeddingServiceFactory>()
    .GetService(EmbeddingProvider.OpenAI);

var result = await embeddingService.GenerateSingleEmbeddingAsync(
    "This is a sample text",
    EmbeddingModel.TextEmbedding3Small,
    EmbeddingProvider.OpenAI);

Console.WriteLine($"Dimensions: {result.Dimensions}");
Console.WriteLine($"First 5 values: {string.Join(", ", result.Embedding.Take(5))}");
```

#### Multiple Texts
```csharp
var request = new EmbeddingRequest
{
    Texts = new List<string>
    {
        "First document about machine learning",
        "Second document about deep learning",
        "Third document about neural networks"
    },
    Model = EmbeddingModel.TextEmbedding3Small,
    Provider = EmbeddingProvider.OpenAI,
    Options = new EmbeddingOptions
    {
        Normalize = true,
        UseCache = true
    }
};

var response = await embeddingService.GenerateEmbeddingsAsync(request);

Console.WriteLine($"Generated {response.Results.Count} embeddings");
Console.WriteLine($"Total tokens: {response.Usage.TotalTokens}");
Console.WriteLine($"Estimated cost: ${response.Usage.EstimatedCost:F6}");
Console.WriteLine($"Cache hits: {response.Usage.CacheHits}");
```

#### Custom Dimensions
```csharp
var request = new EmbeddingRequest
{
    Texts = new List<string> { "Sample text" },
    Model = EmbeddingModel.TextEmbedding3Large,
    Options = new EmbeddingOptions
    {
        Dimensions = 1024  // Reduce from 3072 to 1024
    }
};

var response = await embeddingService.GenerateEmbeddingsAsync(request);
Console.WriteLine($"Dimensions: {response.Results[0].Dimensions}");
```

### 2. Similarity Calculations

#### Cosine Similarity
```csharp
var similarityService = serviceProvider.GetRequiredService<ISimilarityService>();

float[] embedding1 = /* ... */;
float[] embedding2 = /* ... */;

var similarity = similarityService.CosineSimilarity(embedding1, embedding2);
Console.WriteLine($"Cosine similarity: {similarity:F4}");  // 0.0 to 1.0
```

#### Calculate Similarity Between Texts
```csharp
var embeddingService = serviceProvider.GetRequiredService<IEmbeddingServiceFactory>()
    .GetService(EmbeddingProvider.OpenAI);

var text1 = "Machine learning is fascinating";
var text2 = "Deep learning is a subset of ML";

var emb1 = await embeddingService.GenerateSingleEmbeddingAsync(text1, EmbeddingModel.TextEmbedding3Small, EmbeddingProvider.OpenAI);
var emb2 = await embeddingService.GenerateSingleEmbeddingAsync(text2, EmbeddingModel.TextEmbedding3Small, EmbeddingProvider.OpenAI);

var similarity = similarityService.CosineSimilarity(emb1.Embedding, emb2.Embedding);
Console.WriteLine($"Texts similarity: {similarity:F4}");
```

#### Find Similar Embeddings
```csharp
var request = new BatchSimilarityRequest
{
    QueryEmbedding = queryEmbedding,
    TargetEmbeddings = documentEmbeddings,
    Metric = SimilarityMetric.Cosine,
    TopK = 5,
    Threshold = 0.7f
};

var result = await similarityService.FindSimilarAsync(request);

foreach (var match in result.Matches)
{
    Console.WriteLine($"Index: {match.Index}, Score: {match.Score:F4}");
}
```

### 3. Batch Processing

#### Process Large Number of Texts
```csharp
var batchService = serviceProvider.GetRequiredService<IBatchEmbeddingService>();

var request = new BatchEmbeddingRequest
{
    Texts = largeListOfTexts,  // e.g., 10,000 texts
    Model = EmbeddingModel.TextEmbedding3Small,
    Provider = EmbeddingProvider.OpenAI,
    Options = new BatchOptions
    {
        BatchSize = 100,
        MaxConcurrency = 5,
        ContinueOnError = true,
        UseCache = true,
        RetryCount = 3
    }
};

var response = await batchService.ProcessBatchAsync(request);

Console.WriteLine($"Total: {response.Statistics.TotalTexts}");
Console.WriteLine($"Success: {response.Statistics.SuccessCount}");
Console.WriteLine($"Failed: {response.Statistics.FailureCount}");
Console.WriteLine($"Duration: {response.Statistics.TotalDuration.TotalSeconds:F2}s");
Console.WriteLine($"Total cost: ${response.Statistics.TotalCost:F6}");
Console.WriteLine($"Cache hits: {response.Statistics.CacheHits}");
```

### 4. Semantic Search

#### Search Documents
```csharp
var searchService = serviceProvider.GetRequiredService<ISemanticSearchService>();

var documents = new List<SemanticDocument>
{
    new SemanticDocument { Id = "1", Content = "Python is a programming language" },
    new SemanticDocument { Id = "2", Content = "JavaScript is used for web development" },
    new SemanticDocument { Id = "3", Content = "Machine learning uses Python extensively" },
    new SemanticDocument { Id = "4", Content = "TypeScript is a superset of JavaScript" }
};

var request = new SemanticSearchRequest
{
    Query = "What is Python?",
    Documents = documents,
    TopK = 2,
    Threshold = 0.5f
};

var response = await searchService.SearchAsync(request);

foreach (var result in response.Results)
{
    Console.WriteLine($"Rank {result.Rank}: {result.Content}");
    Console.WriteLine($"Score: {result.Score:F4}");
    Console.WriteLine();
}
```

#### Pre-computed Embeddings
```csharp
// If you already have embeddings, provide them to skip re-generation
var documents = new List<SemanticDocument>
{
    new SemanticDocument 
    { 
        Id = "1", 
        Content = "Document 1",
        Embedding = precomputedEmbedding1 
    },
    new SemanticDocument 
    { 
        Id = "2", 
        Content = "Document 2",
        Embedding = precomputedEmbedding2 
    }
};

var request = new SemanticSearchRequest
{
    Query = "search query",
    Documents = documents,
    TopK = 5
};

var response = await searchService.SearchAsync(request);
```

### 5. Caching

#### Cache Configuration
```csharp
// In appsettings.json
{
  "EmbeddingSettings": {
    "CacheStrategy": "Memory",  // or "Redis", "Hybrid", "None"
    "CacheDurationMinutes": 60,
    "MaxCacheEntries": 10000
  }
}
```

#### Check Cache Statistics
```csharp
var cacheService = serviceProvider.GetRequiredService<IEmbeddingCacheService>();

var stats = await cacheService.GetStatisticsAsync();

Console.WriteLine($"Total entries: {stats.TotalEntries}");
Console.WriteLine($"Cache hits: {stats.Hits}");
Console.WriteLine($"Cache misses: {stats.Misses}");
Console.WriteLine($"Hit rate: {stats.HitRate:P2}");
```

#### Clear Cache
```csharp
await cacheService.ClearAsync();
```

### 6. Model Information

#### Get Model Details
```csharp
var embeddingService = serviceProvider.GetRequiredService<IEmbeddingServiceFactory>()
    .GetService(EmbeddingProvider.OpenAI);

var modelInfo = embeddingService.GetModelInfo(EmbeddingModel.TextEmbedding3Small);

Console.WriteLine($"Model: {modelInfo.DisplayName}");
Console.WriteLine($"Dimensions: {modelInfo.DefaultDimensions}");
Console.WriteLine($"Max tokens: {modelInfo.MaxTokens}");
Console.WriteLine($"Cost per 1K tokens: ${modelInfo.CostPer1KTokens}");
Console.WriteLine($"Supports custom dimensions: {modelInfo.SupportsCustomDimensions}");
```

#### List Available Models
```csharp
var models = embeddingService.GetAvailableModels();

foreach (var model in models)
{
    Console.WriteLine($"{model.DisplayName} - {model.DefaultDimensions}D - ${model.CostPer1KTokens}/1K tokens");
}
```

## API Endpoints

### Embedding Generation
- `POST /api/embeddings/generate` - Generate embeddings for multiple texts
- `POST /api/embeddings/generate/single` - Generate single embedding
- `POST /api/embeddings/batch` - Batch processing with concurrency

### Similarity
- `POST /api/embeddings/similarity` - Calculate similarity between embeddings
- `POST /api/embeddings/similarity/text` - Calculate similarity between texts
- `POST /api/embeddings/similarity/find` - Find similar embeddings
- `POST /api/embeddings/similarity/cosine` - Cosine similarity
- `POST /api/embeddings/similarity/check` - Check if texts are similar

### Semantic Search
- `POST /api/embeddings/search` - Semantic search
- `POST /api/embeddings/search/simple` - Simple search with query string

### Models
- `GET /api/embeddings/models` - Get available models
- `GET /api/embeddings/models/{model}` - Get model information

### Cache
- `GET /api/embeddings/cache/stats` - Cache statistics
- `DELETE /api/embeddings/cache` - Clear cache

### Analytics
- `GET /api/embeddings/analytics` - Get usage analytics

### Utilities
- `POST /api/embeddings/compare` - Compare multiple texts
- `POST /api/embeddings/similarity/check` - Check similarity with threshold

## API Examples (cURL)

### Generate Embeddings
```bash
curl -X POST "http://localhost:5000/api/embeddings/generate" \
  -H "Content-Type: application/json" \
  -d '{
    "texts": ["First text", "Second text"],
    "model": "TextEmbedding3Small",
    "provider": "OpenAI"
  }'
```

### Calculate Text Similarity
```bash
curl -X POST "http://localhost:5000/api/embeddings/similarity/text" \
  -H "Content-Type: application/json" \
  -d '{
    "text1": "Machine learning is great",
    "text2": "Deep learning is powerful",
    "metric": "Cosine"
  }'
```

### Semantic Search
```bash
curl -X POST "http://localhost:5000/api/embeddings/search/simple?query=Python programming&topK=3" \
  -H "Content-Type: application/json" \
  -d '[
    {"id": "1", "content": "Python is a programming language"},
    {"id": "2", "content": "JavaScript for web development"}
  ]'
```

### Batch Processing
```bash
curl -X POST "http://localhost:5000/api/embeddings/batch" \
  -H "Content-Type: application/json" \
  -d '{
    "texts": ["Text 1", "Text 2", "..."],
    "model": "TextEmbedding3Small",
    "provider": "OpenAI",
    "options": {
      "batchSize": 100,
      "maxConcurrency": 5
    }
  }'
```

## Running the Application

### Development Mode
```powershell
dotnet run
```

### Production Mode
```powershell
dotnet run --configuration Release
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` (root URL)

## Project Structure
```
Module-02-Embeddings/
├── Controllers/
│   └── EmbeddingsController.cs        # REST API endpoints
├── Models/
│   └── EmbeddingModels.cs             # All data models
├── Services/
│   ├── EmbeddingService.cs            # OpenAI & Azure embedding services
│   └── SimilarityService.cs           # Similarity, batch, cache, search
├── Extensions/
│   └── ServiceCollectionExtensions.cs # Dependency injection
├── AI.Embeddings.csproj               # Project configuration
├── Program.cs                         # Application entry point
├── appsettings.json                   # Configuration
├── appsettings.Development.json       # Dev configuration
└── README.md                          # This file
```

## Dependencies
- **Azure.AI.OpenAI** (1.0.0-beta.17) - OpenAI API client
- **MathNet.Numerics** (5.0.0) - Mathematical operations
- **Microsoft.ML** (3.0.1) - Machine learning framework
- **StackExchangeRedis** (8.0.0) - Redis caching
- **Serilog** - Logging
- **Swashbuckle** - API documentation

## Supported Embedding Models

### OpenAI Models
| Model | Dimensions | Max Tokens | Cost/1K Tokens | Custom Dimensions |
|-------|------------|------------|----------------|-------------------|
| text-embedding-3-small | 1536 | 8191 | $0.00002 | ✅ (512, 1536) |
| text-embedding-3-large | 3072 | 8191 | $0.00013 | ✅ (256, 1024, 3072) |
| text-embedding-ada-002 | 1536 | 8191 | $0.0001 | ❌ |

### Similarity Metrics Comparison

| Metric | Range | Best For | Formula |
|--------|-------|----------|---------|
| Cosine | -1 to 1 | Text similarity | cos(θ) = A·B / (‖A‖‖B‖) |
| Euclidean | 0 to ∞ | Distance measurement | √Σ(ai - bi)² |
| Dot Product | -∞ to ∞ | Magnitude + direction | Σ(ai × bi) |
| Manhattan | 0 to ∞ | Grid-based distance | Σ\|ai - bi\| |

## Best Practices

### Embedding Generation
- **Use caching**: Dramatically reduces API calls and costs
- **Batch processing**: Process multiple texts together
- **Choose right model**: text-embedding-3-small for most cases
- **Normalize vectors**: Enable normalization for consistent similarity scores

### Similarity Calculations
- **Cosine similarity**: Best for text semantic similarity (most common)
- **Pre-compute embeddings**: Cache document embeddings
- **Use thresholds**: Filter low-similarity results

### Cost Optimization
- **Enable caching**: Reduce redundant API calls
- **Use smaller models**: text-embedding-3-small vs large
- **Custom dimensions**: Reduce dimensions if possible
- **Batch processing**: More efficient than individual calls

### Performance
- **Concurrent processing**: Use MaxConcurrency in batch options
- **Memory cache**: Faster than Redis for small datasets
- **Redis cache**: Better for distributed/large-scale systems

## Troubleshooting

### API Key Issues
```
Error: Incorrect API key provided
Solution: Check OPENAI_API_KEY in appsettings.json or environment variables
```

### Rate Limiting
```
Error: Rate limit exceeded
Solution: Reduce MaxConcurrency or add retry logic with delays
```

### Dimension Mismatch
```
Error: Embeddings must have same dimensions
Solution: Ensure both embeddings use the same model and dimension settings
```

### Cache Issues
```
Error: Redis connection failed
Solution: Check RedisConnectionString or use Memory cache instead
```

## Learning Path
1. ✅ Embedding Generation (OpenAI, Azure)
2. ✅ Similarity Calculations (5 metrics)
3. ✅ Batch Processing (concurrent, efficient)
4. ✅ Caching Strategies (Memory, Redis, Hybrid)
5. ✅ Semantic Search (query-based retrieval)
6. ✅ Analytics & Monitoring (usage tracking)
7. ✅ REST API Development
8. ✅ Configuration Management

## Next Steps
- Implement local embedding models (Sentence Transformers)
- Add Cohere and HuggingFace providers
- Implement clustering algorithms
- Add dimension reduction techniques
- Implement fine-tuning support
- Add embedding visualization

## License
This module is part of the CSharp-AI-Modules-StepByStep learning series.
