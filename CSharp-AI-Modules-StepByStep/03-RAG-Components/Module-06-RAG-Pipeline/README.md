# AI RAG Pipeline - Complete End-to-End Implementation

A comprehensive .NET 8.0 Retrieval Augmented Generation (RAG) pipeline that integrates all previous modules into a production-ready system for intelligent document processing and question answering.

## Overview

This module provides a complete RAG implementation with:
- **Document Ingestion**: Multi-format document processing with chunking strategies
- **Query Processing**: Query rewriting, expansion, and optimization
- **Context Retrieval**: Multiple retrieval strategies with re-ranking
- **Response Generation**: OpenAI/Azure OpenAI integration with streaming support
- **Quality Evaluation**: Automated evaluation of faithfulness, relevance, and quality
- **Pipeline Orchestration**: End-to-end workflow management with metrics

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        RAG Pipeline                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │   Document   │───▶│   Chunking   │───▶│  Embeddings  │      │
│  │  Extraction  │    │              │    │              │      │
│  └──────────────┘    └──────────────┘    └──────┬───────┘      │
│                                                   │              │
│                                                   ▼              │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │   Response   │◀───│   Context    │◀───│    Vector    │      │
│  │  Generation  │    │   Retrieval  │    │   Database   │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│         │                                                        │
│         ▼                                                        │
│  ┌──────────────┐                                               │
│  │  Evaluation  │                                               │
│  └──────────────┘                                               │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

## Features

### 1. Document Ingestion Pipeline
- **Multi-Format Support**: PDF, DOCX, TXT, HTML, Markdown
- **Chunking Strategies**: Fixed size, Sentence, Paragraph, Recursive, Semantic, Sliding window
- **Embedding Generation**: Automatic vector embedding with OpenAI/Azure OpenAI
- **Vector Storage**: Seamless integration with vector databases
- **Batch Processing**: Process multiple documents concurrently
- **Metadata Extraction**: Preserve document metadata throughout pipeline

### 2. Query Processing
- **Query Rewriting**: Context-aware query reformulation
- **Query Expansion**: Generate alternative phrasings for better retrieval
- **Keyword Extraction**: Automatic keyword identification
- **Conversation History**: Multi-turn conversation support
- **Query Embedding**: Vector representation generation

### 3. Context Retrieval
- **Multiple Strategies**:
  - Simple Semantic Search
  - Hybrid Search (Semantic + Keyword)
  - Multi-Query Retrieval
  - HyDE (Hypothetical Document Embeddings)
  - Parent Document Retrieval
  - Self-Query with metadata filtering
- **Re-ranking Algorithms**:
  - Reciprocal Rank Fusion (RRF)
  - Maximal Marginal Relevance (MMR)
  - BM25 scoring
  - Diversity re-ranking
- **Filtering**: Metadata-based filtering
- **Score Thresholds**: Configurable minimum relevance scores

### 4. Response Generation
- **OpenAI Integration**: GPT-4, GPT-3.5-turbo support
- **Azure OpenAI**: Enterprise-grade deployment option
- **Streaming Responses**: Real-time token streaming
- **Configurable Parameters**: Temperature, max tokens, penalties
- **Context Assembly**: Intelligent context organization
- **Source Attribution**: Track which sources were used

### 5. Quality Evaluation
- **Faithfulness**: Measure answer grounding in context
- **Relevance**: Assess answer relevance to query
- **Answer Quality**: Evaluate response completeness
- **Context Precision**: Measure retrieval accuracy
- **Context Recall**: Assess retrieval completeness
- **Overall Scoring**: Weighted composite score
- **Quality Ratings**: Poor, Fair, Good, Excellent
- **Actionable Feedback**: Suggestions for improvement

### 6. Pipeline Orchestration
- **Stage-by-Stage Execution**: Query Processing → Retrieval → Generation → Evaluation
- **Error Handling**: Graceful error recovery per stage
- **Execution History**: Track all pipeline runs
- **Batch Processing**: Execute multiple queries concurrently
- **Metrics Collection**: Comprehensive performance metrics
- **Health Monitoring**: Service health checks

## Technology Stack

### Core Technologies
- **.NET 8.0** - Application framework
- **ASP.NET Core Web API** - REST API
- **Azure.AI.OpenAI** - OpenAI/Azure OpenAI integration
- **C# 12** - Programming language

### Document Processing (Module-01)
- **iText7** - PDF processing
- **NPOI** - Word document processing
- **DocumentFormat.OpenXml** - Office document support
- **HtmlAgilityPack** - HTML parsing
- **Markdig** - Markdown processing

### Vector & Search (Modules 03-04)
- **Qdrant.Client** - Vector database integration
- **MathNet.Numerics** - Mathematical operations
- **Lucene.NET** - Full-text search

### Infrastructure
- **Serilog** - Structured logging
- **Polly** - Resilience and retry
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- OpenAI API key or Azure OpenAI credentials
- Qdrant (optional, for production vector storage)
- Redis (optional, for caching)

### Installation

1. **Clone and navigate**:
```bash
cd Module-06-RAG-Pipeline
```

2. **Restore packages**:
```bash
dotnet restore
```

3. **Configure API keys** in `appsettings.json`:
```json
{
  "RAGPipeline": {
    "OpenAIApiKey": "your-openai-api-key",
    "EmbeddingModel": "text-embedding-3-small",
    "ChatModel": "gpt-4"
  }
}
```

4. **Run the application**:
```bash
dotnet run
```

5. **Access Swagger UI**:
```
https://localhost:5001/swagger
```

## Usage Examples

### 1. Ingest a Document

**Upload a file**:
```bash
curl -X POST https://localhost:5001/api/rag/ingest/file \
  -F "file=@document.pdf" \
  -F "strategy=Recursive" \
  -F "chunkSize=1000" \
  -F "chunkOverlap=200"
```

**Response**:
```json
{
  "success": true,
  "data": {
    "documentId": "doc-123",
    "chunksCreated": 15,
    "embeddingsGenerated": 15,
    "vectorsStored": 15,
    "processingTime": "00:00:03.245",
    "chunkIds": ["chunk-1", "chunk-2", "..."]
  },
  "message": "File ingested successfully"
}
```

### 2. Execute a Simple Query

```bash
curl -X POST https://localhost:5001/api/rag/query/simple \
  -H "Content-Type: application/json" \
  -d '{
    "query": "What are the key features of RAG systems?",
    "strategy": "HybridSearch",
    "topK": 5,
    "temperature": 0.7
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "queryId": "query-456",
    "query": "What are the key features of RAG systems?",
    "answer": "RAG systems combine retrieval and generation...",
    "sources": [
      {
        "id": "chunk-1",
        "content": "...",
        "score": 0.92
      }
    ],
    "tokensUsed": 450,
    "totalTime": "00:00:02.150",
    "evaluation": {
      "overallScore": 0.87,
      "quality": "Excellent",
      "faithfulnessScore": 0.95,
      "relevanceScore": 0.89
    }
  }
}
```

### 3. Advanced Query with Full Configuration

```bash
curl -X POST https://localhost:5001/api/rag/query \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Explain the benefits of hybrid search",
    "knowledgeBaseId": "kb-789",
    "retrievalConfig": {
      "strategy": "HybridSearch",
      "topK": 10,
      "minimumScore": 0.7,
      "enableReranking": true,
      "rerankingAlgorithm": "MMR",
      "filters": {
        "documentType": "technical"
      }
    },
    "generationConfig": {
      "model": "gpt-4",
      "temperature": 0.7,
      "maxTokens": 1000,
      "systemPrompt": "You are a technical expert..."
    },
    "conversationHistory": [
      {
        "role": "user",
        "content": "What is hybrid search?",
        "timestamp": "2024-01-01T10:00:00Z"
      }
    ]
  }'
```

### 4. Streaming Query

```bash
curl -X POST https://localhost:5001/api/rag/query/stream \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Explain RAG architecture",
    "retrievalConfig": { "topK": 5 },
    "generationConfig": { "stream": true }
  }'
```

**Server-Sent Events Output**:
```
data: {"queryId":"query-789","sources":[...]}

data: {"content":"RAG"}

data: {"content":" architecture"}

data: {"content":" combines"}

data: [DONE]
```

### 5. Batch Query Processing

```bash
curl -X POST https://localhost:5001/api/rag/query/batch \
  -H "Content-Type: application/json" \
  -d '{
    "queries": [
      { "query": "What is RAG?" },
      { "query": "How does vector search work?" },
      { "query": "Explain embeddings" }
    ],
    "maxConcurrency": 3,
    "continueOnError": true
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "totalQueries": 3,
    "successfulQueries": 3,
    "failedQueries": 0,
    "responses": [...],
    "totalTime": "00:00:05.450"
  }
}
```

### 6. Get Pipeline Metrics

```bash
curl https://localhost:5001/api/rag/metrics
```

**Response**:
```json
{
  "success": true,
  "data": {
    "totalQueries": 150,
    "successfulQueries": 147,
    "failedQueries": 3,
    "averageResponseTime": 2340.5,
    "averageRetrievalTime": 450.2,
    "averageGenerationTime": 1890.3,
    "averageRelevanceScore": 0.85,
    "totalTokensUsed": 67500,
    "errorsByStage": {
      "Retrieval": 2,
      "Generation": 1
    }
  }
}
```

## Configuration

### RAG Pipeline Settings

```json
{
  "RAGPipeline": {
    "OpenAIApiKey": "sk-...",
    "EmbeddingModel": "text-embedding-3-small",
    "ChatModel": "gpt-4",
    "EmbeddingDimensions": 1536,
    "VectorDatabaseUrl": "http://localhost:6333",
    "CollectionName": "rag_documents",
    "DefaultStrategy": "HybridSearch",
    "DefaultTopK": 5,
    "DefaultTemperature": 0.7,
    "MaxTokens": 2000,
    "EnableCaching": true,
    "EnableEvaluation": true,
    "RedisConnectionString": "localhost:6379"
  }
}
```

### Retrieval Strategies

| Strategy | Description | Best For |
|----------|-------------|----------|
| **Simple** | Semantic search only | Quick lookups, single concept queries |
| **HybridSearch** | Semantic + Keyword | Most queries, balanced approach |
| **MultiQuery** | Multiple query variations | Ambiguous queries |
| **HyDE** | Hypothetical answer generation | Complex reasoning |
| **ParentDocument** | Retrieve parent chunks | Long documents |
| **SelfQuery** | Metadata-aware filtering | Structured data |

### Chunking Strategies

| Strategy | Description | Best For |
|----------|-------------|----------|
| **FixedSize** | Fixed character count | Uniform processing |
| **Sentence** | Split by sentences | Preserving complete thoughts |
| **Paragraph** | Split by paragraphs | Maintaining context |
| **Recursive** | Hierarchical splitting | General purpose |
| **Semantic** | Meaning-based splits | High-quality splits |
| **Sliding** | Overlapping windows | Maximum coverage |

## Integration with Previous Modules

### Module-01: Document Processing
```csharp
var ingestionService = serviceProvider.GetRequiredService<IDocumentIngestionService>();

var request = new DocumentIngestionRequest
{
    FilePath = "document.pdf",
    ContentType = "application/pdf",
    Config = new IngestionConfig
    {
        Strategy = ChunkingStrategy.Recursive,
        ChunkSize = 1000,
        ChunkOverlap = 200
    }
};

var result = await ingestionService.IngestDocumentAsync(request);
```

### Module-02: Embeddings
```csharp
var embeddingService = serviceProvider.GetRequiredService<IEmbeddingGenerationService>();

// Single embedding
var embedding = await embeddingService.GenerateEmbeddingAsync("sample text");

// Batch embeddings
var texts = new List<string> { "text1", "text2", "text3" };
var embeddings = await embeddingService.GenerateBatchEmbeddingsAsync(texts);
```

### Module-03: Vector Databases
```csharp
var vectorService = serviceProvider.GetRequiredService<IVectorStorageService>();

// Store vectors
await vectorService.UpsertVectorsAsync(documentChunks);

// Search vectors
var results = await vectorService.SearchVectorsAsync(
    queryEmbedding, 
    topK: 5, 
    filters: new Dictionary<string, object> { { "category", "technical" } });
```

### Module-04: Retrieval Strategies
```csharp
var retrievalService = serviceProvider.GetRequiredService<IContextRetrievalService>();

var config = new RetrievalConfig
{
    Strategy = RAGStrategy.HybridSearch,
    TopK = 10,
    EnableReranking = true,
    RerankingAlgorithm = "MMR"
};

var context = await retrievalService.RetrieveContextAsync(processedQuery, config);
```

### Module-05: Knowledge Management
```csharp
// Query specific knowledge base
var query = new RAGQuery
{
    Query = "What is machine learning?",
    KnowledgeBaseId = "ml-docs-kb",
    RetrievalConfig = new RetrievalConfig
    {
        Filters = new Dictionary<string, object>
        {
            { "knowledgeBaseId", "ml-docs-kb" }
        }
    }
};

var response = await orchestrator.ExecuteQueryAsync(query);
```

## API Endpoints

### Document Ingestion

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/rag/ingest` | Ingest document (JSON) |
| POST | `/api/rag/ingest/file` | Ingest document (file upload) |

### Query Execution

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/rag/query` | Execute full RAG query |
| POST | `/api/rag/query/simple` | Execute simplified query |
| POST | `/api/rag/query/stream` | Execute streaming query |
| POST | `/api/rag/query/batch` | Execute batch queries |

### Monitoring

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/rag/metrics` | Get pipeline metrics |
| GET | `/api/rag/health` | Health check |

## Pipeline Stages

### Stage 1: Query Processing
- Query rewriting with conversation context
- Query expansion for better coverage
- Keyword extraction
- Embedding generation

### Stage 2: Context Retrieval
- Execute configured retrieval strategy
- Apply filters and score thresholds
- Re-rank results (if enabled)
- Assemble context chunks

### Stage 3: Response Generation
- Build prompt with context
- Generate response with OpenAI
- Track token usage
- Return structured response

### Stage 4: Evaluation (Optional)
- Calculate faithfulness score
- Calculate relevance score
- Assess answer quality
- Provide improvement suggestions

## Evaluation Metrics

### Faithfulness Score (0-1)
Measures how well the answer is grounded in the retrieved context.
- 1.0 = Completely faithful
- 0.0 = Not grounded

### Relevance Score (0-1)
Measures how relevant the answer is to the query.
- 1.0 = Highly relevant
- 0.0 = Not relevant

### Answer Quality Score (0-1)
Assesses the completeness and structure of the answer.
- Checks length, structure, confidence

### Context Precision (0-1)
Measures the quality of retrieved chunks.
- Based on top-ranked scores

### Context Recall (0-1)
Measures how much of the context was used.
- Based on word overlap analysis

### Overall Score
Weighted combination:
- 30% Faithfulness
- 30% Relevance
- 20% Answer Quality
- 10% Context Precision
- 10% Context Recall

## Performance Optimization

### 1. Caching
- Enable Redis caching for embeddings
- Cache frequent queries
- TTL configuration

### 2. Batch Processing
- Process multiple documents concurrently
- Batch embedding generation
- Parallel query execution

### 3. Retrieval Optimization
- Adjust `topK` based on needs
- Use filters to reduce search space
- Enable re-ranking only when needed

### 4. Generation Optimization
- Adjust `maxTokens` appropriately
- Use streaming for long responses
- Cache common responses

## Error Handling

### Pipeline Errors
Each stage tracks errors independently:
```json
{
  "errors": [
    {
      "stage": "Retrieval",
      "message": "Vector search timeout",
      "timestamp": "2024-01-01T10:00:00Z"
    }
  ]
}
```

### Retry Logic
Automatic retry with exponential backoff for:
- OpenAI API calls
- Vector database operations
- Network errors

## Monitoring & Logging

### Structured Logging
- Serilog with Console and File sinks
- JSON log formatting
- Contextual logging with execution IDs

### Metrics
- Query success/failure rates
- Response times per stage
- Token usage tracking
- Error rates by stage

### Health Checks
```bash
curl https://localhost:5001/api/rag/health
```

## Best Practices

### 1. Document Ingestion
- Choose appropriate chunking strategy
- Set overlap for context continuity
- Extract and preserve metadata
- Process in batches for efficiency

### 2. Query Processing
- Use conversation history for multi-turn
- Enable query expansion for ambiguous queries
- Rewrite queries for clarity

### 3. Retrieval
- Start with HybridSearch for best results
- Use filters to narrow search space
- Enable re-ranking for better precision
- Adjust `topK` based on document length

### 4. Generation
- Use appropriate temperature (0.7 for balanced)
- Set reasonable `maxTokens`
- Customize system prompts for domain
- Enable streaming for better UX

### 5. Evaluation
- Enable in production for quality monitoring
- Track metrics over time
- Act on suggestions for improvement

## Troubleshooting

### Common Issues

**1. No Results Returned**
- Check if documents are ingested
- Verify embedding generation
- Lower `minimumScore` threshold

**2. Low Quality Answers**
- Increase `topK` for more context
- Adjust chunking strategy
- Enable re-ranking
- Refine system prompt

**3. High Latency**
- Reduce `topK`
- Disable evaluation in development
- Use caching
- Optimize chunk size

**4. API Key Errors**
- Verify OpenAI API key in config
- Check API quota and limits
- Ensure proper authentication

## Production Deployment

### Requirements
- .NET 8.0 Runtime
- Redis (recommended)
- Qdrant or other vector DB (recommended)
- Load balancer for scaling
- Monitoring solution

### Configuration
```json
{
  "RAGPipeline": {
    "OpenAIApiKey": "${OPENAI_API_KEY}",
    "EnableCaching": true,
    "EnableEvaluation": true,
    "VectorDatabaseUrl": "https://qdrant.production.com"
  }
}
```

### Scaling
- Horizontal scaling with multiple instances
- Redis for distributed caching
- Load balancing across instances
- Vector DB replication

## Future Enhancements

- [ ] Multi-modal support (images, audio)
- [ ] Advanced caching strategies
- [ ] Custom re-ranking models
- [ ] Real-time index updates
- [ ] A/B testing framework
- [ ] Advanced analytics dashboard
- [ ] Multi-language support
- [ ] Fine-tuned embedding models
- [ ] Hybrid vector databases
- [ ] GraphRAG implementation

## License

MIT License - See LICENSE file for details

## Support

For issues and questions:
- Check Swagger documentation at `/swagger`
- Review logs in `logs/` directory
- Verify configuration in `appsettings.json`
- Test with `/health` endpoint

## Related Modules

This module integrates:
- **Module-01-Document-Processing**: Document extraction and chunking
- **Module-02-Embeddings**: Vector embedding generation
- **Module-03-Vector-Databases**: Vector storage and search
- **Module-04-Retrieval-Strategies**: Advanced retrieval methods
- **Module-05-Knowledge-Management**: Knowledge base organization

## Complete RAG Pipeline Architecture

```
User Query
    ↓
┌─────────────────────────────────────┐
│  1. Query Processing                │
│  - Rewrite with history             │
│  - Expand variations                │
│  - Extract keywords                 │
│  - Generate embedding               │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│  2. Context Retrieval               │
│  - Semantic search (vectors)        │
│  - Keyword search (BM25)            │
│  - Hybrid fusion (RRF)              │
│  - Re-ranking (MMR/Diversity)       │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│  3. Context Assembly                │
│  - Select top-K chunks              │
│  - Format with sources              │
│  - Build prompt                     │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│  4. Response Generation             │
│  - Call OpenAI/Azure                │
│  - Stream or complete               │
│  - Track tokens                     │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│  5. Evaluation (Optional)           │
│  - Faithfulness                     │
│  - Relevance                        │
│  - Quality                          │
│  - Overall score                    │
└──────────────┬──────────────────────┘
               ↓
    Final Response with Sources
```

## Conclusion

This RAG Pipeline module provides a production-ready, end-to-end solution for building intelligent question-answering systems. It integrates all previous modules and adds sophisticated orchestration, evaluation, and monitoring capabilities for enterprise applications.
