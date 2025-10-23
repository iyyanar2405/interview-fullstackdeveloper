# Module 03 - Vector Databases

## Overview
Production-ready ASP.NET Core Web API for Vector Database operations with support for **Qdrant** and **In-Memory** storage. This module provides vector storage, similarity search, filtering, batch operations, and index management for RAG (Retrieval-Augmented Generation) systems.

## Features

### Vector Database Providers
- **Qdrant** - High-performance vector database with gRPC support
- **In-Memory** - Fast local storage for development and testing

### Core Capabilities
✅ Collection/Index Management (Create, Delete, List, Info)  
✅ Vector CRUD Operations (Upsert, Retrieve, Delete, Update)  
✅ Similarity Search (Cosine, Euclidean, Dot Product, Manhattan)  
✅ Advanced Filtering (Must/Should/MustNot conditions)  
✅ Batch Operations (Batch Search, Multi-Vector Search)  
✅ Hybrid Search (Vector + Keyword combination)  
✅ Recommendations (Based on positive/negative examples)  
✅ Pagination (Scroll API)  
✅ Analytics & Statistics  
✅ Index Optimization  

## Technology Stack

### NuGet Packages
```xml
<PackageReference Include="Qdrant.Client" Version="1.9.0" />
<PackageReference Include="MathNet.Numerics" Version="5.0.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.0" />
<PackageReference Include="Polly" Version="8.2.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
```

## Getting Started

### 1. Prerequisites
- .NET 8.0 SDK
- Qdrant (optional - runs on Docker or cloud)

### 2. Run Qdrant (Optional)
```bash
# Using Docker
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant

# Qdrant will be available at:
# REST API: http://localhost:6333
# gRPC API: http://localhost:6334
```

### 3. Configure Settings
Update `appsettings.json`:
```json
{
  "VectorDatabase": {
    "DefaultProvider": "InMemory",  // or "Qdrant"
    "Qdrant": {
      "Host": "localhost",
      "Port": 6334,
      "ApiKey": "",
      "UseTls": false
    },
    "InMemory": {
      "MaxCollections": 10,
      "MaxPointsPerCollection": 100000
    }
  }
}
```

### 4. Run the Application
```bash
cd Module-03-Vector-Databases
dotnet restore
dotnet run
```

API available at: **http://localhost:5003**  
Swagger UI: **http://localhost:5003**

## API Documentation

### Collection Management

#### Create Collection
```http
POST /api/VectorDatabase/collections
Content-Type: application/json

{
  "name": "product_embeddings",
  "vectorSize": 1536,
  "distance": "Cosine"
}
```

#### List Collections
```http
GET /api/VectorDatabase/collections?provider=Qdrant
```

#### Get Collection Info
```http
GET /api/VectorDatabase/collections/product_embeddings
```

#### Delete Collection
```http
DELETE /api/VectorDatabase/collections/product_embeddings
```

### Vector Operations

#### Upsert Vectors
```http
POST /api/VectorDatabase/vectors/upsert
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "points": [
    {
      "id": "prod-001",
      "vector": [0.1, 0.2, 0.3, ...],  // 1536 dimensions
      "payload": {
        "title": "Wireless Mouse",
        "category": "Electronics",
        "price": 29.99
      }
    }
  ],
  "wait": true
}
```

#### Retrieve Vectors
```http
POST /api/VectorDatabase/vectors/retrieve
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "pointIds": ["prod-001", "prod-002"],
  "withPayload": true,
  "withVector": true
}
```

#### Delete Vectors
```http
POST /api/VectorDatabase/vectors/delete
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "pointIds": ["prod-001", "prod-002"]
}
```

#### Update Payload
```http
POST /api/VectorDatabase/vectors/update-payload
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "pointIds": ["prod-001"],
  "payload": {
    "price": 24.99,
    "inStock": true
  },
  "overwrite": false
}
```

### Search Operations

#### Vector Search
```http
POST /api/VectorDatabase/search
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "queryVector": [0.1, 0.2, 0.3, ...],
  "limit": 10,
  "scoreThreshold": 0.7,
  "withPayload": true,
  "withVector": false
}
```

#### Filtered Search
```http
POST /api/VectorDatabase/search
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "queryVector": [0.1, 0.2, 0.3, ...],
  "limit": 10,
  "filter": {
    "must": [
      {
        "key": "category",
        "operator": "Equals",
        "value": "Electronics"
      },
      {
        "key": "price",
        "operator": "LessThan",
        "value": "50"
      }
    ]
  }
}
```

#### Batch Search
```http
POST /api/VectorDatabase/search/batch
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "queryVectors": [
    [0.1, 0.2, 0.3, ...],
    [0.4, 0.5, 0.6, ...],
    [0.7, 0.8, 0.9, ...]
  ],
  "limit": 5
}
```

#### Hybrid Search (Vector + Keywords)
```http
POST /api/VectorDatabase/search/hybrid
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "queryVector": [0.1, 0.2, 0.3, ...],
  "keywordQuery": "wireless bluetooth",
  "keywordFields": ["title", "description"],
  "limit": 10,
  "vectorWeight": 0.7,
  "keywordWeight": 0.3
}
```

#### Recommendations
```http
POST /api/VectorDatabase/recommend
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "positiveIds": ["prod-001", "prod-005"],
  "negativeIds": ["prod-010"],
  "limit": 10
}
```

### Pagination

#### Scroll API
```http
POST /api/VectorDatabase/vectors/scroll
Content-Type: application/json

{
  "collectionName": "product_embeddings",
  "limit": 100,
  "offset": null,  // Use nextOffset from previous response
  "withPayload": true,
  "withVector": false
}
```

### Analytics

#### Get Collection Statistics
```http
GET /api/VectorDatabase/collections/product_embeddings/statistics
```

#### Get Index Size
```http
GET /api/VectorDatabase/index/product_embeddings/size
```

## Code Examples

### C# Client Usage

#### 1. Create Collection
```csharp
using var httpClient = new HttpClient();
var request = new CreateCollectionRequest
{
    Name = "product_embeddings",
    VectorSize = 1536,
    Distance = DistanceMetric.Cosine
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5003/api/VectorDatabase/collections", 
    request);
```

#### 2. Upsert Vectors
```csharp
var upsertRequest = new UpsertRequest
{
    CollectionName = "product_embeddings",
    Points = new List<VectorPoint>
    {
        new VectorPoint
        {
            Id = Guid.NewGuid().ToString(),
            Vector = embeddings, // float[] from OpenAI
            Payload = new Dictionary<string, object>
            {
                { "title", "Wireless Mouse" },
                { "category", "Electronics" },
                { "price", 29.99 }
            }
        }
    },
    Wait = true
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5003/api/VectorDatabase/vectors/upsert", 
    upsertRequest);
```

#### 3. Search Similar Vectors
```csharp
var searchRequest = new SearchRequest
{
    CollectionName = "product_embeddings",
    QueryVector = queryEmbedding, // float[] from user query
    Limit = 10,
    ScoreThreshold = 0.7f,
    Filter = new SearchFilter
    {
        Must = new List<FilterCondition>
        {
            new FilterCondition 
            { 
                Key = "category", 
                Operator = FilterOperator.Equals, 
                Value = "Electronics" 
            }
        }
    },
    WithPayload = true
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5003/api/VectorDatabase/search", 
    searchRequest);
var results = await response.Content.ReadFromJsonAsync<SearchResponse>();

foreach (var result in results.Results)
{
    Console.WriteLine($"ID: {result.Id}, Score: {result.Score:F4}");
    Console.WriteLine($"Title: {result.Payload["title"]}");
}
```

### Python Client Example

```python
import requests
import numpy as np

# Create collection
response = requests.post(
    "http://localhost:5003/api/VectorDatabase/collections",
    json={
        "name": "products",
        "vectorSize": 1536,
        "distance": "Cosine"
    }
)

# Upsert vectors
embedding = np.random.rand(1536).tolist()
response = requests.post(
    "http://localhost:5003/api/VectorDatabase/vectors/upsert",
    json={
        "collectionName": "products",
        "points": [
            {
                "id": "prod-001",
                "vector": embedding,
                "payload": {
                    "title": "Laptop",
                    "price": 999.99
                }
            }
        ],
        "wait": True
    }
)

# Search
query_embedding = np.random.rand(1536).tolist()
response = requests.post(
    "http://localhost:5003/api/VectorDatabase/search",
    json={
        "collectionName": "products",
        "queryVector": query_embedding,
        "limit": 5,
        "withPayload": True
    }
)
results = response.json()
```

## Distance Metrics

### Cosine Similarity (Default)
- Range: [-1, 1] (normalized to [0, 1])
- Best for: Text embeddings, normalized vectors
- Formula: `(A · B) / (||A|| × ||B||)`

### Euclidean Distance
- Range: [0, ∞)
- Best for: Spatial data, unnormalized vectors
- Formula: `√Σ(A[i] - B[i])²`

### Dot Product
- Range: [-∞, ∞]
- Best for: Magnitude-aware similarity
- Formula: `Σ(A[i] × B[i])`

### Manhattan Distance
- Range: [0, ∞)
- Best for: Grid-based data
- Formula: `Σ|A[i] - B[i]|`

## Filter Operations

### Supported Operators
- `Equals` - Exact match
- `NotEquals` - Not equal
- `Contains` - String contains
- `GreaterThan` - Numeric comparison
- `LessThan` - Numeric comparison
- `In` - Value in comma-separated list

### Filter Example
```json
{
  "filter": {
    "must": [
      { "key": "category", "operator": "Equals", "value": "Electronics" },
      { "key": "price", "operator": "LessThan", "value": "100" }
    ],
    "should": [
      { "key": "brand", "operator": "Equals", "value": "Apple" },
      { "key": "brand", "operator": "Equals", "value": "Samsung" }
    ],
    "mustNot": [
      { "key": "status", "operator": "Equals", "value": "discontinued" }
    ]
  }
}
```

## Performance Optimization

### Batch Operations
```csharp
// Batch upsert (up to 1000 points)
var points = new List<VectorPoint>();
for (int i = 0; i < 1000; i++)
{
    points.Add(new VectorPoint 
    { 
        Id = Guid.NewGuid().ToString(), 
        Vector = GenerateEmbedding(), 
        Payload = metadata 
    });
}

await httpClient.PostAsJsonAsync("/api/VectorDatabase/vectors/upsert", 
    new UpsertRequest { CollectionName = "products", Points = points });
```

### Pagination for Large Datasets
```csharp
string? nextOffset = null;
do
{
    var scrollRequest = new ScrollRequest
    {
        CollectionName = "products",
        Limit = 1000,
        Offset = nextOffset
    };
    
    var response = await httpClient.PostAsJsonAsync(
        "/api/VectorDatabase/vectors/scroll", scrollRequest);
    var result = await response.Content.ReadFromJsonAsync<ScrollResponse>();
    
    // Process result.Points
    ProcessPoints(result.Points);
    
    nextOffset = result.NextOffset;
} while (nextOffset != null);
```

## Architecture

### Project Structure
```
Module-03-Vector-Databases/
├── Controllers/
│   └── VectorDatabaseController.cs      # REST API endpoints
├── Services/
│   ├── QdrantService.cs                 # Qdrant implementation
│   ├── InMemoryService.cs               # In-memory implementation
│   └── SearchAndIndexServices.cs        # Search & index management
├── Models/
│   └── VectorDatabaseModels.cs          # Data models
├── Extensions/
│   └── ServiceCollectionExtensions.cs   # DI configuration
├── Program.cs                           # Application entry point
└── appsettings.json                     # Configuration
```

### Design Patterns
- **Factory Pattern** - Provider selection (Qdrant/InMemory)
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Service lifetime management
- **Options Pattern** - Configuration management

## Best Practices

### 1. Vector Normalization
```csharp
// Normalize vectors for cosine similarity
float[] NormalizeVector(float[] vector)
{
    var magnitude = Math.Sqrt(vector.Sum(v => v * v));
    return vector.Select(v => v / (float)magnitude).ToArray();
}
```

### 2. Efficient Filtering
```csharp
// Use indexed fields for filtering
var filter = new SearchFilter
{
    Must = new List<FilterCondition>
    {
        new() { Key = "category", Operator = FilterOperator.Equals, Value = "Books" }
    }
};
```

### 3. Batch Processing
```csharp
// Process in batches of 100-1000 points
const int batchSize = 500;
for (int i = 0; i < allPoints.Count; i += batchSize)
{
    var batch = allPoints.Skip(i).Take(batchSize).ToList();
    await UpsertBatch(batch);
}
```

## Common Use Cases

### 1. Semantic Search
```csharp
// Convert user query to embedding
var queryEmbedding = await openAIService.GetEmbeddingAsync(userQuery);

// Search similar documents
var results = await vectorDb.SearchAsync(new SearchRequest
{
    CollectionName = "documents",
    QueryVector = queryEmbedding,
    Limit = 10
});
```

### 2. Product Recommendations
```csharp
// Get recommendations based on viewed products
var recommendations = await vectorDb.RecommendAsync(new RecommendRequest
{
    CollectionName = "products",
    PositiveIds = viewedProductIds,
    NegativeIds = dislikedProductIds,
    Limit = 20
});
```

### 3. Duplicate Detection
```csharp
// Find near-duplicates
var results = await vectorDb.SearchAsync(new SearchRequest
{
    CollectionName = "documents",
    QueryVector = documentEmbedding,
    Limit = 5,
    ScoreThreshold = 0.95f  // Very high similarity
});
```

## Troubleshooting

### Qdrant Connection Issues
```bash
# Check if Qdrant is running
curl http://localhost:6333/collections

# Check Docker container
docker ps | grep qdrant
docker logs <container-id>
```

### Memory Issues (In-Memory Provider)
- Reduce `MaxCollections` and `MaxPointsPerCollection` in settings
- Switch to Qdrant for large datasets (>100K vectors)

### Slow Search Performance
- Reduce vector dimensions (use PCA/dimensionality reduction)
- Use appropriate distance metric for your data
- Add filters to narrow search space
- Consider using score threshold to limit results

## License
MIT License - Free for commercial and personal use

## Related Modules
- **Module-01-Document-Processing** - Extract and chunk documents
- **Module-02-Embeddings** - Generate vector embeddings
- **Module-04-Query-Processing** - Process and optimize queries
- **Module-05-Context-Retrieval** - Retrieve relevant context

## Support
For issues, questions, or contributions, please refer to the main repository documentation.

---

**Built with ❤️ for AI/RAG Applications**
