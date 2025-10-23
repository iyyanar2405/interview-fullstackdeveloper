# AI Knowledge Management Module

A comprehensive .NET 8.0 knowledge management system with versioning, access control, analytics, and synchronization capabilities for RAG (Retrieval Augmented Generation) applications.

## Features

### 1. Knowledge Base Management
- Create and manage multiple knowledge bases
- Public/private visibility control
- Owner-based permissions
- Tags and metadata support
- Settings customization

### 2. Document Management
- Full document lifecycle (Draft → Published → Archived)
- Rich metadata and tagging
- Parent-child document relationships
- Related documents linking
- Vector embeddings integration
- Status tracking

### 3. Versioning System
- Automatic version creation on updates
- Version history tracking
- Version comparison with similarity metrics
- Point-in-time restoration
- Automatic backup before restore
- Version cleanup (retain N recent versions)

### 4. Access Control
- Granular permission system (Read/Write/Delete/Admin/Owner)
- Resource-level permissions
- User and owner-based access
- Permission expiration
- Public resource support

### 5. Search & Discovery
- Multi-criteria search (text, tags, status)
- Knowledge base scoping
- Tag-based discovery
- Search analytics tracking
- Response time monitoring

### 6. Analytics
- Document view tracking
- Search query analytics
- Click-through rate (CTR) calculation
- User activity monitoring
- Knowledge base metrics
- Top queries tracking
- Trend analysis

### 7. Synchronization
- Background sync jobs
- Full and partial sync support
- Progress tracking
- Error handling and reporting
- Batch document processing
- Embedding regeneration

### 8. Change Tracking
- Complete audit trail
- Field-level change tracking
- User activity history
- Timestamp tracking
- Old/new value comparison

## Architecture

```
Module-05-Knowledge-Management/
├── Controllers/
│   └── KnowledgeManagementController.cs   # REST API endpoints
├── Data/
│   └── KnowledgeDbContext.cs              # EF Core database context
├── Extensions/
│   └── ServiceCollectionExtensions.cs     # DI configuration
├── Models/
│   └── KnowledgeManagementModels.cs       # 50+ model classes
├── Services/
│   ├── KnowledgeBaseServices.cs           # KB, Document, Search services
│   ├── VersioningServices.cs              # Versioning, Sync, Change Tracking
│   └── AccessControlAndAnalyticsServices.cs # Access & Analytics
├── Program.cs                              # Application startup
├── appsettings.json                        # Production configuration
└── appsettings.Development.json            # Development configuration
```

## Technology Stack

- **.NET 8.0** - Core framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core** - ORM
- **SQL Server** - Production database
- **SQLite** - Development database
- **ASP.NET Identity** - Authentication
- **JWT Bearer** - Token-based auth
- **Hangfire** - Background job processing
- **Azure Blob Storage** - Document storage
- **AutoMapper** - Object mapping
- **Polly** - Resilience and retry
- **Redis** - Caching
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (or SQLite for development)
- Redis (optional, for caching)
- Qdrant (optional, for vector storage)
- OpenAI API key (optional, for embeddings)

### Installation

1. **Restore packages**:
```bash
dotnet restore
```

2. **Update connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "KnowledgeDb": "Server=YOUR_SERVER;Database=KnowledgeManagement;..."
}
```

3. **Apply database migrations**:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Run the application**:
```bash
dotnet run
```

5. **Access Swagger UI**:
```
https://localhost:5001/swagger
```

## Configuration

### Database Providers

**SQL Server** (Production):
```json
{
  "ConnectionStrings": {
    "KnowledgeDb": "Server=(localdb)\\mssqllocaldb;Database=KnowledgeManagement;Trusted_Connection=true"
  },
  "KnowledgeManagement": {
    "DatabaseProvider": "SqlServer"
  }
}
```

**SQLite** (Development):
```json
{
  "ConnectionStrings": {
    "KnowledgeDb": "Data Source=knowledge.db"
  },
  "KnowledgeManagement": {
    "DatabaseProvider": "Sqlite"
  }
}
```

### Feature Flags

```json
{
  "KnowledgeManagement": {
    "EnableVersioning": true,
    "MaxVersionsPerDocument": 50,
    "EnableAccessControl": true,
    "EnableAnalytics": true,
    "EnableBackgroundJobs": true,
    "AutoArchiveAfterDays": 365
  }
}
```

## API Endpoints

### Knowledge Base Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/knowledgemanagement/knowledge-bases` | Create knowledge base |
| GET | `/api/knowledgemanagement/knowledge-bases/{id}` | Get knowledge base |
| GET | `/api/knowledgemanagement/knowledge-bases` | List knowledge bases |
| PUT | `/api/knowledgemanagement/knowledge-bases/{id}` | Update knowledge base |
| DELETE | `/api/knowledgemanagement/knowledge-bases/{id}` | Delete knowledge base |

### Document Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/knowledgemanagement/documents` | Create document |
| GET | `/api/knowledgemanagement/documents/{id}` | Get document |
| GET | `/api/knowledgemanagement/knowledge-bases/{id}/documents` | List documents |
| PUT | `/api/knowledgemanagement/documents/{id}` | Update document |
| POST | `/api/knowledgemanagement/documents/{id}/publish` | Publish document |
| POST | `/api/knowledgemanagement/documents/{id}/archive` | Archive document |
| DELETE | `/api/knowledgemanagement/documents/{id}` | Delete document |

### Versioning

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/knowledgemanagement/documents/{id}/versions` | Get document versions |
| GET | `/api/knowledgemanagement/versions/{id}` | Get specific version |
| POST | `/api/knowledgemanagement/documents/{id}/versions/{versionId}/restore` | Restore version |
| GET | `/api/knowledgemanagement/versions/compare` | Compare versions |

### Access Control

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/knowledgemanagement/access/grant` | Grant access |
| DELETE | `/api/knowledgemanagement/access/{id}` | Revoke access |
| GET | `/api/knowledgemanagement/access/check` | Check access |
| GET | `/api/knowledgemanagement/users/{id}/permissions` | Get user permissions |
| GET | `/api/knowledgemanagement/resources/{id}/permissions` | Get resource permissions |

### Search

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/knowledgemanagement/search` | Search documents |
| GET | `/api/knowledgemanagement/knowledge-bases/{id}/search/tags` | Search by tags |

### Analytics

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/knowledgemanagement/analytics/documents/{id}` | Get document analytics |
| POST | `/api/knowledgemanagement/analytics/knowledge-bases` | Get KB analytics |

### Synchronization

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/knowledgemanagement/sync` | Start sync job |
| GET | `/api/knowledgemanagement/sync/{id}` | Get sync job status |
| GET | `/api/knowledgemanagement/knowledge-bases/{id}/sync` | Get sync jobs |

### Change Tracking

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/knowledgemanagement/changes/{id}` | Get change history |
| GET | `/api/knowledgemanagement/users/{id}/activity` | Get user activity |

## Usage Examples

### Create Knowledge Base

```bash
curl -X POST https://localhost:5001/api/knowledgemanagement/knowledge-bases \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Technical Documentation",
    "Description": "Internal technical docs",
    "OwnerId": "user123",
    "IsPublic": false,
    "Tags": ["technical", "internal"]
  }'
```

### Create Document

```bash
curl -X POST https://localhost:5001/api/knowledgemanagement/documents \
  -H "Content-Type: application/json" \
  -d '{
    "KnowledgeBaseId": "kb123",
    "Title": "API Design Guide",
    "Content": "Best practices for API design...",
    "AuthorId": "user123",
    "Tags": ["api", "best-practices"],
    "AutoPublish": true
  }'
```

### Update Document (with versioning)

```bash
curl -X PUT https://localhost:5001/api/knowledgemanagement/documents/doc123 \
  -H "Content-Type: application/json" \
  -d '{
    "Title": "API Design Guide v2",
    "Content": "Updated best practices...",
    "CreateNewVersion": true,
    "VersionNotes": "Added GraphQL section"
  }'
```

### Grant Access

```bash
curl -X POST https://localhost:5001/api/knowledgemanagement/access/grant?grantedBy=user123 \
  -H "Content-Type: application/json" \
  -d '{
    "ResourceId": "doc123",
    "ResourceType": "Document",
    "UserId": "user456",
    "AccessLevel": 3
  }'
```

### Search Documents

```bash
curl -X POST https://localhost:5001/api/knowledgemanagement/search \
  -H "Content-Type: application/json" \
  -d '{
    "KnowledgeBaseId": "kb123",
    "Query": "API design patterns",
    "Tags": ["api"],
    "Status": 1,
    "UserId": "user123",
    "IncludeAnalytics": true
  }'
```

### Compare Versions

```bash
curl -X GET "https://localhost:5001/api/knowledgemanagement/versions/compare?version1Id=v1&version2Id=v2"
```

### Get Analytics

```bash
curl -X POST https://localhost:5001/api/knowledgemanagement/analytics/knowledge-bases \
  -H "Content-Type: application/json" \
  -d '{
    "KnowledgeBaseId": "kb123",
    "StartDate": "2024-01-01T00:00:00Z",
    "EndDate": "2024-12-31T23:59:59Z"
  }'
```

## Integration with Other Modules

### Module-02-Embeddings Integration

```csharp
// Generate embeddings for documents
var embeddingService = new OpenAIEmbeddingService(config, httpClient, logger);
var embedding = await embeddingService.GenerateEmbeddingAsync(document.Content);
document.VectorId = embedding.Id;
document.Embedding = embedding.Vector;
```

### Module-03-Vector-Databases Integration

```csharp
// Store document vectors in Qdrant
var vectorService = new QdrantService(config, logger);
await vectorService.UpsertVectorAsync(new VectorUpsertRequest
{
    CollectionName = "knowledge_documents",
    Id = document.Id,
    Vector = document.Embedding,
    Metadata = new Dictionary<string, object>
    {
        { "documentId", document.Id },
        { "title", document.Title },
        { "tags", document.Tags }
    }
});
```

### Module-04-Retrieval-Strategies Integration

```csharp
// Use retrieval strategies for enhanced search
var retrievalService = new SemanticSearchService(vectorDb, embeddingService, logger);
var results = await retrievalService.SearchAsync(new SemanticSearchRequest
{
    Query = searchRequest.Query,
    TopK = 10,
    Filters = new Dictionary<string, object>
    {
        { "knowledgeBaseId", searchRequest.KnowledgeBaseId }
    }
});
```

## Models Overview

### Core Models
- `KnowledgeBase` - Knowledge base container
- `Document` - Document entity with full metadata
- `DocumentVersion` - Version snapshot
- `AccessPermission` - Permission grant
- `SyncJob` - Background sync job
- `ChangeLog` - Audit trail entry
- `SearchAnalytics` - Search query tracking
- `DocumentAnalytics` - Document metrics
- `KnowledgeBaseAnalytics` - KB-level metrics

### Enums
- `DocumentStatus` - Draft, Published, Archived, Deleted, UnderReview
- `VersionStatus` - Active, Archived, Deleted
- `AccessLevel` - None, Read, Write, Delete, Admin, Owner (flags)
- `SyncStatus` - Pending, InProgress, Completed, Failed
- `ChangeType` - Created, Updated, Deleted, Published, Archived

## Database Schema

### Tables
- `KnowledgeBases` - Knowledge base records
- `Documents` - Document records with versioning
- `DocumentVersions` - Version history
- `AccessPermissions` - Permission grants
- `SyncJobs` - Sync job tracking
- `ChangeLogs` - Audit trail
- `SearchAnalytics` - Search analytics

### Indexes
- `KnowledgeBases`: OwnerId, CreatedAt
- `Documents`: KnowledgeBaseId, AuthorId, Status, CreatedAt
- `DocumentVersions`: (DocumentId, VersionNumber) UNIQUE, CreatedAt
- `AccessPermissions`: (ResourceId, UserId), UserId
- `SyncJobs`: KnowledgeBaseId, StartedAt
- `ChangeLogs`: ResourceId, ChangedBy, ChangedAt
- `SearchAnalytics`: KnowledgeBaseId, UserId, Timestamp

## Best Practices

1. **Versioning**: Always create versions for important updates
2. **Access Control**: Use granular permissions, not just owner checks
3. **Analytics**: Enable analytics to track usage patterns
4. **Search**: Use tags and metadata for better discoverability
5. **Sync Jobs**: Monitor sync job status for failures
6. **Change Tracking**: Review audit logs for compliance
7. **Performance**: Use pagination for large result sets
8. **Caching**: Cache frequently accessed documents
9. **Background Jobs**: Use Hangfire for long-running tasks
10. **Error Handling**: Implement retry logic with Polly

## Performance Considerations

- **Pagination**: All list endpoints support paging
- **Indexing**: Proper database indexes on foreign keys and common queries
- **Async/Await**: All I/O operations are asynchronous
- **Caching**: Redis integration for frequently accessed data
- **Batch Processing**: Sync service processes documents in batches
- **Connection Pooling**: EF Core manages connection pooling

## Security

- **Authentication**: JWT Bearer token authentication
- **Authorization**: Role-based and resource-level access control
- **Permission Expiration**: Time-limited access grants
- **Audit Trail**: Complete change tracking
- **Data Validation**: Model validation with FluentValidation
- **SQL Injection**: Parameterized queries via EF Core

## Monitoring & Logging

- **Structured Logging**: Serilog with Console and File sinks
- **Log Levels**: Debug (dev), Information (prod)
- **Performance Metrics**: Response time tracking
- **Error Tracking**: Exception logging with context
- **Activity Monitoring**: User activity logs

## Future Enhancements

- [ ] Full-text search with Elasticsearch
- [ ] Real-time collaboration
- [ ] Document templates
- [ ] Workflow automation
- [ ] AI-powered recommendations
- [ ] Multi-language support
- [ ] Document encryption
- [ ] Bulk operations API
- [ ] WebSocket notifications
- [ ] Advanced analytics dashboard

## Contributing

This module is part of the RAG Components learning series. Follow the established patterns from Modules 01-04.

## License

MIT License - See LICENSE file for details

## Support

For issues and questions:
- Check Swagger documentation at `/swagger`
- Review logs in `logs/` directory
- Verify configuration in `appsettings.json`
- Ensure database migrations are applied

## Related Modules

- **Module-01-Document-Processing**: Document extraction and chunking
- **Module-02-Embeddings**: Vector embedding generation
- **Module-03-Vector-Databases**: Vector storage and retrieval
- **Module-04-Retrieval-Strategies**: Advanced search and re-ranking
- **Module-05-Knowledge-Management**: This module
- **Module-06-RAG-Pipeline**: Complete RAG workflow integration (coming next)
