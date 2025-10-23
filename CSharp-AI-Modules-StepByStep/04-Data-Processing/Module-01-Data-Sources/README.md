# Module-01-Data-Sources

Multi-source data integration API for AI data processing pipelines. Supports SQL databases, NoSQL databases, cloud storage (Azure/AWS), REST/GraphQL APIs, and file systems.

## Features

### Data Sources

- **SQL Server**: Connection testing, query execution, bulk insert operations
- **PostgreSQL**: Connection testing, query execution, batch insert with transactions
- **MongoDB**: CRUD operations, JSON-based queries, BSON conversion
- **Azure Blob Storage**: Upload, download, list, delete with metadata support
- **AWS S3**: Object operations with regions and credentials
- **REST API**: HTTP methods (GET/POST/PUT/DELETE) with RestSharp
- **GraphQL**: Query/mutation execution with GraphQL.Client
- **File System**: Local file CRUD, directory management

### Architecture

```
AI.DataSources/
├── Models/
│   └── DataSourceModels.cs         # 150+ model classes for all sources
├── Services/
│   ├── RelationalDatabaseServices.cs   # SQL Server & PostgreSQL
│   ├── MongoDbService.cs               # MongoDB operations
│   ├── CloudStorageServices.cs         # Azure Blob & AWS S3
│   └── ApiAndFileSystemServices.cs     # REST/GraphQL/File System
├── Controllers/
│   └── DataSourceController.cs     # REST API with ~35 endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # DI registration
└── Program.cs                      # Application startup
```

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server (optional)
- PostgreSQL (optional)
- MongoDB (optional)
- Azure Storage Account (optional)
- AWS Account (optional)

### Configuration

Update `appsettings.json` with your connection strings:

```json
{
  "DataSources": {
    "SqlServer": {
      "Server": "localhost",
      "Database": "AIDataSource",
      "UserId": "sa",
      "Password": "YourPassword123!"
    },
    "PostgreSQL": {
      "Host": "localhost",
      "Port": 5432,
      "Database": "ai_datasource",
      "Username": "postgres",
      "Password": "postgres"
    },
    "MongoDB": {
      "ConnectionString": "mongodb://localhost:27017",
      "Database": "ai_datasource"
    }
  }
}
```

### Run the Application

```bash
cd Module-01-Data-Sources
dotnet restore
dotnet build
dotnet run
```

API will be available at `https://localhost:5001` (Swagger UI at root).

## API Examples

### SQL Server

#### Test Connection
```bash
POST /api/datasource/sqlserver/test
Content-Type: application/json

{
  "server": "localhost",
  "database": "AIDataSource",
  "userId": "sa",
  "password": "YourPassword123!",
  "timeout": 30
}
```

#### Execute Query
```bash
POST /api/datasource/sqlserver/query
Content-Type: application/json

{
  "connectionConfig": {
    "Server": "localhost",
    "Database": "AIDataSource",
    "UserId": "sa",
    "Password": "YourPassword123!"
  },
  "query": "SELECT * FROM Users WHERE Age > @Age",
  "parameters": {
    "Age": 25
  }
}
```

#### Bulk Insert
```bash
POST /api/datasource/sqlserver/bulk-insert
Content-Type: application/json

{
  "connectionConfig": {
    "Server": "localhost",
    "Database": "AIDataSource",
    "UserId": "sa",
    "Password": "YourPassword123!"
  },
  "tableName": "Users",
  "data": [
    { "Name": "John", "Age": 30, "Email": "john@example.com" },
    { "Name": "Jane", "Age": 28, "Email": "jane@example.com" }
  ]
}
```

### PostgreSQL

#### Execute Query
```bash
POST /api/datasource/postgresql/query
Content-Type: application/json

{
  "connectionConfig": {
    "Host": "localhost",
    "Port": "5432",
    "Database": "ai_datasource",
    "Username": "postgres",
    "Password": "postgres"
  },
  "query": "SELECT * FROM products WHERE price > $1",
  "parameters": {
    "1": 100
  }
}
```

### MongoDB

#### Find Documents
```bash
POST /api/datasource/mongodb/find
Content-Type: application/json

{
  "connectionString": "mongodb://localhost:27017",
  "database": "ai_datasource",
  "collection": "users",
  "filterJson": "{ \"age\": { \"$gt\": 25 } }",
  "projectionJson": "{ \"name\": 1, \"email\": 1 }",
  "sortJson": "{ \"name\": 1 }",
  "limit": 10,
  "skip": 0
}
```

#### Insert Documents
```bash
POST /api/datasource/mongodb/insert
Content-Type: application/json

{
  "connectionString": "mongodb://localhost:27017",
  "database": "ai_datasource",
  "collection": "users",
  "documents": [
    { "name": "John", "age": 30, "email": "john@example.com" },
    { "name": "Jane", "age": 28, "email": "jane@example.com" }
  ]
}
```

#### Update Documents
```bash
POST /api/datasource/mongodb/update
Content-Type: application/json

{
  "connectionString": "mongodb://localhost:27017",
  "database": "ai_datasource",
  "collection": "users",
  "filterJson": "{ \"name\": \"John\" }",
  "updateJson": "{ \"$set\": { \"age\": 31 } }",
  "updateMany": false,
  "upsert": false
}
```

### Azure Blob Storage

#### Upload Blob
```bash
POST /api/datasource/azure/upload
Content-Type: application/json

{
  "connectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;...",
  "containerName": "datasources",
  "blobName": "data/file.txt",
  "content": "SGVsbG8gV29ybGQh",  // Base64 encoded
  "contentType": "text/plain",
  "metadata": {
    "source": "api",
    "category": "test"
  }
}
```

#### Download Blob
```bash
POST /api/datasource/azure/download
Content-Type: application/json

{
  "connectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;...",
  "containerName": "datasources",
  "blobName": "data/file.txt"
}
```

#### List Blobs
```bash
POST /api/datasource/azure/list
Content-Type: application/json

{
  "connectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;...",
  "containerName": "datasources",
  "prefix": "data/",
  "maxResults": 100,
  "continuationToken": null
}
```

### AWS S3

#### Upload Object
```bash
POST /api/datasource/aws/upload
Content-Type: application/json

{
  "connectionString": "us-east-1;YOUR_ACCESS_KEY;YOUR_SECRET_KEY",
  "containerName": "ai-datasources",
  "blobName": "data/file.txt",
  "content": "SGVsbG8gV29ybGQh",  // Base64 encoded
  "contentType": "text/plain",
  "metadata": {
    "source": "api"
  }
}
```

#### List Objects
```bash
POST /api/datasource/aws/list
Content-Type: application/json

{
  "connectionString": "us-east-1;YOUR_ACCESS_KEY;YOUR_SECRET_KEY",
  "containerName": "ai-datasources",
  "prefix": "data/",
  "maxResults": 100
}
```

### REST API Client

#### Send Request
```bash
POST /api/datasource/api/request
Content-Type: application/json

{
  "method": "GET",
  "endpoint": "/users/123",
  "headers": {
    "BaseUrl": "https://api.example.com",
    "Authorization": "Bearer token123"
  },
  "queryParameters": {
    "include": "profile"
  }
}
```

### GraphQL

#### Send Query
```bash
POST /api/datasource/api/graphql
Content-Type: application/json

{
  "query": "query GetUser($id: ID!) { user(id: $id) { name email } }",
  "variables": {
    "endpoint": "https://api.example.com/graphql",
    "id": "123"
  },
  "operationName": "GetUser"
}
```

### File System

#### Read File
```bash
POST /api/datasource/file/read
Content-Type: application/json

{
  "filePath": "C:\\AIData\\data.txt",
  "readAsBinary": false,
  "encoding": "utf-8"
}
```

#### Write File
```bash
POST /api/datasource/file/write
Content-Type: application/json

{
  "filePath": "C:\\AIData\\output.txt",
  "textContent": "Hello World!",
  "encoding": "utf-8",
  "overwrite": true
}
```

#### List Files
```bash
GET /api/datasource/file/list?directory=C:\AIData&pattern=*.txt
```

## Key Features

### SQL Server Service
- Connection validation with metadata retrieval
- Parameterized queries for SQL injection prevention
- Bulk insert with SqlBulkCopy (high performance)
- Connection pooling
- Configurable timeouts

### PostgreSQL Service
- Npgsql-based connection management
- Dapper for query execution
- Batch insert with transactions
- Chunked processing for large datasets

### MongoDB Service
- BSON document operations
- JSON-based query syntax
- Comprehensive type conversion (12+ BSON types)
- Projection, sorting, pagination support
- Upsert operations

### Azure Blob Storage Service
- BlobServiceClient pattern
- Metadata preservation
- Pagination with continuation tokens
- Content type handling
- ETag support

### AWS S3 Service
- Region-based configuration
- IAM credential authentication
- ListObjectsV2 pagination
- Custom metadata support
- Pre-signed URL generation ready

### API Client Service
- RestSharp for HTTP operations
- GraphQL.Client for GraphQL queries
- Retry logic with Polly (ready)
- Custom header support
- Query parameter handling

### File System Service
- Binary and text file support
- Encoding configuration
- Directory operations
- File existence checking
- Overwrite protection

## Performance

- **SQL Server Bulk Insert**: ~50,000 rows/second with SqlBulkCopy
- **PostgreSQL Batch Insert**: ~20,000 rows/second with chunked transactions
- **MongoDB Insert**: ~10,000 documents/second
- **Azure Blob Upload**: ~10 MB/second (depends on network)
- **AWS S3 Upload**: ~10 MB/second (depends on network)

## Error Handling

All endpoints return standardized responses:

```json
{
  "success": true,
  "data": { ... },
  "error": null
}
```

On error:
```json
{
  "success": false,
  "data": null,
  "error": "Error message details"
}
```

## Logging

Comprehensive logging with Serilog:
- Console output for development
- File logs: `logs/datasources-YYYYMMDD.txt`
- Structured logging with correlation IDs
- Performance timing for all operations

## Security Best Practices

1. **Connection Strings**: Store in environment variables or Azure Key Vault
2. **SQL Injection**: Always use parameterized queries
3. **Authentication**: Use managed identities for Azure/AWS when possible
4. **HTTPS**: Enforce HTTPS in production
5. **Input Validation**: Validate all user inputs
6. **Secrets**: Never commit credentials to source control

## Dependencies

- Microsoft.Data.SqlClient 5.1.2
- Npgsql 8.0.1
- Dapper 2.1.24
- MongoDB.Driver 2.23.1
- Azure.Storage.Blobs 12.19.1
- AWSSDK.S3 3.7.307
- RestSharp 110.2.0
- GraphQL.Client 6.0.3
- Polly 8.2.0
- Serilog 8.0.0

## Testing

Run tests with:
```bash
dotnet test
```

## Troubleshooting

### SQL Server Connection Issues
- Verify SQL Server is running
- Check firewall settings
- Ensure SQL Server authentication is enabled
- Verify connection string format

### PostgreSQL Connection Issues
- Verify PostgreSQL service is running
- Check pg_hba.conf for authentication settings
- Ensure port 5432 is accessible

### MongoDB Connection Issues
- Verify MongoDB service is running
- Check authentication database settings
- Verify connection string format

### Azure Blob Storage Issues
- Verify storage account credentials
- Ensure container exists
- Check firewall/network rules

### AWS S3 Issues
- Verify IAM credentials
- Ensure bucket exists in correct region
- Check bucket policies

## Next Steps

1. Implement Redis caching service
2. Add Kafka/Event Hub streaming support
3. Implement data transfer between sources
4. Add connection pooling and health checks
5. Implement retry policies with Polly
6. Add distributed tracing
7. Implement rate limiting

## License

MIT License - See LICENSE file for details

## Support

For questions or issues, contact datasources@example.com
