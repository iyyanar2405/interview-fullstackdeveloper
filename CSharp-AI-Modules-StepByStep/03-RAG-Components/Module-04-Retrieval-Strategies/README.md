# Module 04 - Retrieval Strategies

## Overview
Production-ready ASP.NET Core Web API for advanced retrieval strategies in RAG (Retrieval-Augmented Generation) systems. Includes **semantic search**, **hybrid search**, **multi-query retrieval**, **HyDE**, **re-ranking algorithms**, **query expansion**, and **context window optimization**.

## Features

### Retrieval Strategies
✅ **Semantic Search** - Dense vector similarity search  
✅ **Hybrid Search** - Combine semantic + keyword (BM25)  
✅ **Multi-Query Retrieval** - Parallel queries with result fusion  
✅ **HyDE (Hypothetical Document Embeddings)** - Generate hypothetical answers  
✅ **Parent Document Retrieval** - Retrieve child chunks, return parent docs  
✅ **Self-Query** - Natural language to structured query parsing  

### Re-ranking Algorithms
✅ **Reciprocal Rank Fusion (RRF)** - Merge multiple ranked lists  
✅ **Maximal Marginal Relevance (MMR)** - Balance relevance and diversity  
✅ **BM25** - Statistical keyword-based ranking  
✅ **Diversity Re-ranking** - Reduce redundancy in results  

### Query Optimization
✅ **Query Expansion** - Synonyms, paraphrasing, multi-perspective  
✅ **Context Window Optimization** - Fit documents within token limits  
✅ **Token Estimation** - Calculate context size  

## Technology Stack

### NuGet Packages
```xml
<PackageReference Include="Azure.AI.OpenAI" Version="1.0.0-beta.12" />
<PackageReference Include="MathNet.Numerics" Version="5.0.0" />
<PackageReference Include="Microsoft.ML" Version="3.0.1" />
<PackageReference Include="Lucene.Net" Version="4.8.0-beta00016" />
<PackageReference Include="Polly" Version="8.2.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
```

## Getting Started

### 1. Prerequisites
- .NET 8.0 SDK
- OpenAI API Key or Azure OpenAI credentials
- Vector Database (Module-03) running (optional)

### 2. Configure Settings
Update `appsettings.json`:
```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key",
    "EmbeddingModel": "text-embedding-3-small",
    "ChatModel": "gpt-4"
  },
  "VectorDatabase": {
    "Endpoint": "http://localhost:5003"
  },
  "Retrieval": {
    "DefaultTopK": 10,
    "HybridSemanticWeight": 0.7,
    "HybridKeywordWeight": 0.3,
    "MaxContextTokens": 4000
  }
}
```

### 3. Run the Application
```bash
cd Module-04-Retrieval-Strategies
dotnet restore
dotnet run
```

API available at: **http://localhost:5004**  
Swagger UI: **http://localhost:5004**

## API Documentation

### Semantic Search

#### Basic Semantic Search
```http
POST /api/Retrieval/semantic-search
Content-Type: application/json

{
  "queryText": "What is machine learning?",
  "strategy": "Semantic",
  "topK": 10,
  "scoreThreshold": 0.7,
  "filters": {}
}
```

**Response:**
```json
{
  "documents": [
    {
      "id": "doc-001",
      "content": "Machine learning is a subset of AI...",
      "score": 0.89,
      "metadata": { "category": "AI", "author": "John Doe" },
      "source": "ml-textbook.pdf"
    }
  ],
  "query": "What is machine learning?",
  "strategy": "Semantic",
  "totalFound": 10,
  "duration": "00:00:00.1234567",
  "success": true
}
```

### Hybrid Search

#### Semantic + Keyword Search
```http
POST /api/Retrieval/hybrid-search
Content-Type: application/json

{
  "queryText": "neural networks deep learning",
  "collectionName": "ml_docs",
  "topK": 10,
  "semanticWeight": 0.7,
  "keywordWeight": 0.3,
  "filters": { "category": "AI" }
}
```

**Use Cases:**
- Balance semantic understanding with exact keyword matching
- Improve recall for technical terms and proper nouns
- Combine best of both retrieval approaches

### Multi-Query Retrieval

#### Parallel Query Execution
```http
POST /api/Retrieval/multi-query
Content-Type: application/json

{
  "originalQuery": "How does deep learning work?",
  "expandedQueries": [
    "Explain the principles of deep learning",
    "What are the fundamentals of neural networks?",
    "How do deep neural networks function?"
  ],
  "topK": 10,
  "deduplicateResults": true
}
```

**Benefits:**
- Capture different aspects of the query
- Improve recall by exploring query variations
- Reduce sensitivity to query phrasing

### HyDE (Hypothetical Document Embeddings)

#### Generate Hypothetical Answers
```http
POST /api/Retrieval/hyde
Content-Type: application/json

{
  "query": "What are the benefits of transformer models?",
  "numHypotheticalDocs": 3,
  "topK": 10,
  "collectionName": "ai_research"
}
```

**How it works:**
1. Generate hypothetical documents that would answer the query
2. Create embeddings for these hypothetical docs
3. Use averaged embedding to search real documents
4. Better matches for answer-like content

### Re-ranking

#### Reciprocal Rank Fusion (RRF)
```http
POST /api/Retrieval/rerank/rrf?topK=10
Content-Type: application/json

[
  [
    { "id": "doc-1", "content": "...", "score": 0.9 },
    { "id": "doc-2", "content": "...", "score": 0.8 }
  ],
  [
    { "id": "doc-2", "content": "...", "score": 0.85 },
    { "id": "doc-3", "content": "...", "score": 0.75 }
  ]
]
```

**Formula:**
```
RRF_score = Σ (1 / (k + rank))
```
Where k = 60 (default constant)

#### Maximal Marginal Relevance (MMR)
```http
POST /api/Retrieval/rerank/mmr
Content-Type: application/json

{
  "documents": [ /* retrieved documents */ ],
  "topK": 10,
  "lambdaParameter": 0.5,
  "queryText": "transformer architecture"
}
```

**Formula:**
```
MMR = λ × Relevance - (1-λ) × MaxSimilarity
```

**Use Cases:**
- Reduce redundancy in search results
- Ensure diverse perspectives
- Balance relevance and novelty

#### BM25 Re-ranking
```http
POST /api/Retrieval/rerank/bm25?query=machine+learning&topK=10
Content-Type: application/json

[ /* documents to re-rank */ ]
```

**Parameters:**
- `k1` = 1.5 (term frequency saturation)
- `b` = 0.75 (document length normalization)

### Query Expansion

#### Generate Synonyms
```http
POST /api/Retrieval/query/synonyms?query=artificial+intelligence&maxExpansions=5
```

**Response:**
```json
{
  "query": "artificial intelligence",
  "synonyms": [
    "machine intelligence",
    "AI systems",
    "cognitive computing",
    "intelligent systems",
    "automated reasoning"
  ]
}
```

#### Generate Paraphrases
```http
POST /api/Retrieval/query/paraphrases?query=How+does+AI+work&maxExpansions=5
```

#### Multi-Perspective Queries
```http
POST /api/Retrieval/query/perspectives?query=climate+change&maxExpansions=5
```

**Response:**
```json
{
  "query": "climate change",
  "perspectives": [
    "What causes global warming?",
    "Environmental impact of carbon emissions",
    "How is climate change affecting ecosystems?",
    "Solutions to reduce greenhouse gases",
    "Economic implications of climate policy"
  ]
}
```

### Context Optimization

#### Optimize for Token Limits
```http
POST /api/Retrieval/context/optimize
Content-Type: application/json

{
  "documents": [ /* retrieved documents */ ],
  "maxTokens": 4000,
  "strategy": "Hybrid",
  "preserveMetadata": true,
  "queryContext": "machine learning algorithms"
}
```

**Strategies:**
- `MaxTokens` - Fill up to max tokens by relevance
- `Relevance` - Select highest-scoring documents
- `Diversity` - Balance relevance and diversity
- `Recency` - Prioritize recent documents
- `Hybrid` - Combine multiple factors (60% relevance, 30% diversity, 10% recency)

**Response:**
```json
{
  "optimizedContext": {
    "documents": [ /* optimized subset */ ],
    "totalTokens": 3950,
    "maxTokens": 4000,
    "strategy": "Hybrid",
    "isTruncated": true
  },
  "originalTokenCount": 8500,
  "optimizedTokenCount": 3950,
  "compressionRatio": 0.46,
  "success": true
}
```

### Combined Workflows

#### Retrieve and Re-rank
```http
POST /api/Retrieval/retrieve-and-rerank?algorithm=MaximalMarginalRelevance
Content-Type: application/json

{
  "queryText": "deep learning architectures",
  "topK": 10,
  "scoreThreshold": 0.7
}
```

#### Expand and Retrieve
```http
POST /api/Retrieval/expand-and-retrieve?query=neural+networks&method=Paraphrasing&maxExpansions=3&topK=10
```

#### Full Pipeline
```http
POST /api/Retrieval/full-pipeline?query=transformer+models&maxTokens=4000&topK=10
```

**Pipeline Steps:**
1. **Query Expansion** - Generate paraphrases
2. **Multi-Query Retrieval** - Search with all variations
3. **Re-ranking** - Apply MMR for diversity
4. **Context Optimization** - Fit within token limit

## Code Examples

### C# Client Usage

#### 1. Semantic Search
```csharp
using var httpClient = new HttpClient();

var query = new RetrievalQuery
{
    QueryText = "What is reinforcement learning?",
    Strategy = RetrievalStrategy.Semantic,
    TopK = 10,
    ScoreThreshold = 0.75f
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5004/api/Retrieval/semantic-search",
    query);

var result = await response.Content.ReadFromJsonAsync<RetrievalResponse>();

foreach (var doc in result.Documents)
{
    Console.WriteLine($"Score: {doc.Score:F4} - {doc.Content.Substring(0, 100)}...");
}
```

#### 2. Hybrid Search
```csharp
var hybridRequest = new HybridSearchRequest
{
    QueryText = "transformer attention mechanism",
    CollectionName = "ml_papers",
    TopK = 10,
    SemanticWeight = 0.7f,
    KeywordWeight = 0.3f
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5004/api/Retrieval/hybrid-search",
    hybridRequest);

var results = await response.Content.ReadFromJsonAsync<RetrievalResponse>();
```

#### 3. Multi-Query with Expansion
```csharp
// Step 1: Expand query
var expansionResponse = await httpClient.PostAsync(
    "http://localhost:5004/api/Retrieval/query/paraphrases?query=machine+learning&maxExpansions=3",
    null);

var expansion = await expansionResponse.Content.ReadFromJsonAsync<dynamic>();

// Step 2: Multi-query retrieval
var multiQuery = new MultiQueryRequest
{
    OriginalQuery = "machine learning",
    ExpandedQueries = expansion.paraphrases.ToObject<List<string>>(),
    TopK = 10
};

var retrievalResponse = await httpClient.PostAsJsonAsync(
    "http://localhost:5004/api/Retrieval/multi-query",
    multiQuery);
```

#### 4. Full Pipeline
```csharp
var response = await httpClient.PostAsync(
    "http://localhost:5004/api/Retrieval/full-pipeline?query=deep+learning&maxTokens=4000&topK=10",
    null);

var result = await response.Content.ReadFromJsonAsync<dynamic>();

Console.WriteLine($"Final context: {result.finalContext.documents.Count} documents");
Console.WriteLine($"Total tokens: {result.context.tokens}");
Console.WriteLine($"Compression: {result.context.compressionRatio:P0}");
```

### Python Client Example

```python
import requests

# Semantic search
response = requests.post(
    "http://localhost:5004/api/Retrieval/semantic-search",
    json={
        "queryText": "What is deep learning?",
        "topK": 10,
        "scoreThreshold": 0.7
    }
)
results = response.json()

# Hybrid search
response = requests.post(
    "http://localhost:5004/api/Retrieval/hybrid-search",
    json={
        "queryText": "neural networks",
        "collectionName": "ml_docs",
        "topK": 10,
        "semanticWeight": 0.7,
        "keywordWeight": 0.3
    }
)

# Full pipeline
response = requests.post(
    "http://localhost:5004/api/Retrieval/full-pipeline",
    params={
        "query": "transformer architecture",
        "maxTokens": 4000,
        "topK": 10
    }
)
pipeline_result = response.json()
```

## Retrieval Strategies Comparison

| Strategy | Pros | Cons | Best For |
|----------|------|------|----------|
| **Semantic** | Understands context, handles synonyms | May miss exact matches | Conceptual queries |
| **Hybrid** | Combines semantic + keyword | More complex | Balanced retrieval |
| **Multi-Query** | Better coverage, robust | Higher latency | Critical queries |
| **HyDE** | Finds answer-like docs | Requires generation | Question answering |
| **Parent Document** | Full context available | More tokens | Long documents |

## Re-ranking Algorithms Comparison

| Algorithm | Time Complexity | Best For | Trade-offs |
|-----------|----------------|----------|------------|
| **RRF** | O(n) | Merging multiple rankings | Simple, effective |
| **MMR** | O(n²) | Diversity | Slower for large sets |
| **BM25** | O(n × m) | Keyword relevance | Needs tokenization |
| **Cross-Encoder** | O(n) | Highest accuracy | Requires model |

## Performance Optimization

### Caching
```csharp
// Enable caching in appsettings.json
{
  "Retrieval": {
    "EnableCaching": true,
    "CacheDurationMinutes": 30
  }
}
```

### Batch Processing
```csharp
// Process multiple queries in parallel
var queries = new[] { "query1", "query2", "query3" };
var tasks = queries.Select(q => SearchAsync(q));
var results = await Task.WhenAll(tasks);
```

### Token Optimization
```csharp
// Estimate tokens before retrieval
var estimatedTokens = contextOptimization.EstimateTokenCount(document.Content);

// Adjust topK based on token budget
var adjustedTopK = CalculateOptimalTopK(maxTokens, avgDocLength);
```

## Best Practices

### 1. Choose the Right Strategy
- **Semantic**: Conceptual queries, natural language
- **Hybrid**: Technical documents with specific terms
- **Multi-Query**: Critical applications requiring high recall
- **HyDE**: Question-answering scenarios

### 2. Re-rank for Quality
```csharp
// Always re-rank for top results
var retrieved = await RetrieveAsync(query, topK: 50);
var reRanked = await ReRankAsync(retrieved, algorithm: MMR, topK: 10);
```

### 3. Optimize Context Window
```csharp
// Fit documents within model limits
var optimized = await OptimizeContextAsync(documents, 
    maxTokens: 4000, 
    strategy: ContextOptimizationStrategy.Hybrid);
```

### 4. Monitor Performance
- Track retrieval latency
- Measure relevance scores
- Monitor cache hit rates
- Analyze query patterns

## Common Use Cases

### 1. Document QA System
```csharp
// Full pipeline: expand → retrieve → re-rank → optimize
var result = await httpClient.PostAsync(
    "/api/Retrieval/full-pipeline?query=What+is+RAG&maxTokens=4000",
    null);
```

### 2. Research Paper Search
```csharp
// Hybrid search with BM25 re-ranking
var hybrid = await HybridSearchAsync("transformer attention");
var reRanked = await BM25ReRankAsync("transformer attention", hybrid.Documents);
```

### 3. Customer Support Bot
```csharp
// Parent document retrieval for full context
var parent = await ParentDocumentRetrievalAsync(new ParentDocumentRequest
{
    Query = "How do I reset my password?",
    TopK = 5,
    RetrieveFullParent = true
});
```

## Architecture

### Project Structure
```
Module-04-Retrieval-Strategies/
├── Controllers/
│   └── RetrievalController.cs           # REST API endpoints
├── Services/
│   ├── RetrievalService.cs              # Core retrieval strategies
│   ├── ReRankingService.cs              # Re-ranking algorithms
│   └── QueryOptimizationServices.cs     # Query expansion, context optimization
├── Models/
│   └── RetrievalModels.cs               # Data models
├── Extensions/
│   └── ServiceCollectionExtensions.cs   # DI configuration
├── Program.cs                           # Application entry point
└── appsettings.json                     # Configuration
```

## Troubleshooting

### High Latency
- Enable caching for repeated queries
- Reduce `topK` for initial retrieval
- Use parallel processing for multi-query

### Poor Relevance
- Adjust `semanticWeight` and `keywordWeight` in hybrid search
- Try HyDE for question-answering
- Re-rank with MMR or BM25

### Token Limit Exceeded
- Use context optimization with `MaxTokens` strategy
- Reduce `topK` value
- Implement chunking for large documents

## Related Modules
- **Module-01-Document-Processing** - Extract and chunk documents
- **Module-02-Embeddings** - Generate vector embeddings
- **Module-03-Vector-Databases** - Store and search vectors
- **Module-05-Knowledge-Management** - Manage knowledge bases
- **Module-06-RAG-Pipeline** - Complete RAG implementation

## License
MIT License - Free for commercial and personal use

---

**Built with ❤️ for Advanced RAG Systems**
