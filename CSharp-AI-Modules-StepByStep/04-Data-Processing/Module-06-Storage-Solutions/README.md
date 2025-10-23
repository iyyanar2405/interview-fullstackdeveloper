# Module 06 - Storage Solutions

Comprehensive storage solutions module providing unified access to relational databases (SQL Server, PostgreSQL), NoSQL databases (MongoDB, Redis), cloud storage (Azure Blob/Data Lake, AWS S3/DynamoDB), and distributed caching strategies.

## 📋 Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Storage Types](#storage-types)
  - [SQL Server](#sql-server)
  - [PostgreSQL](#postgresql)
  - [MongoDB](#mongodb)
  - [Redis](#redis)
  - [Azure Storage](#azure-storage)
  - [AWS Storage](#aws-storage)
  - [Caching](#caching)
- [Performance Optimization](#performance-optimization)
- [Best Practices](#best-practices)
- [API Reference](#api-reference)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)

## Features

### Relational Databases
- **SQL Server**: Full support with Dapper ORM, bulk operations, transactions, stored procedures
- **PostgreSQL**: Native driver with prepared statements, COPY bulk insert, connection pooling
- **Query Optimization**: Index suggestions, execution plan analysis, performance statistics

### NoSQL Databases
- **MongoDB**: Document operations, aggregation pipelines, indexing, BSON support
- **Redis**: Key-value operations, hashes, lists, sets, pub/sub messaging
- **Elasticsearch**: Full-text search, document indexing (configured, not implemented in services)

### Cloud Storage
- **Azure Blob Storage**: Binary large object storage with metadata
- **Azure Data Lake**: Hierarchical file system for data analytics
- **AWS S3**: Object storage with multipart upload support
- **AWS DynamoDB**: NoSQL key-value and document database

### Caching Strategies
- **L1 Cache**: In-memory caching with Microsoft.Extensions.Caching.Memory
- **L2 Cache**: Distributed caching with Redis
- **Cache-Aside Pattern**: GetOrSet with factory functions
- **Cache Statistics**: Hit/miss rates, performance metrics

### Storage Management
- **Unified Interface**: Single service to manage all storage types
- **Metrics & Monitoring**: Operation counts, response times, success rates
- **Connection Testing**: Health checks for all storage types
- **Backup & Restore**: Database backup operations with configurable retention

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      StorageController                          │
│  REST API Layer - HTTP Endpoints for all storage operations    │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                  StorageManagerService                          │
│  Orchestration Layer - Unified access to all storage types     │
└────┬─────────┬──────────┬──────────┬──────────┬────────────────┘
     │         │          │          │          │
     ▼         ▼          ▼          ▼          ▼
┌─────────┐ ┌──────┐ ┌────────┐ ┌───────┐ ┌──────────┐
│SQL      │ │Post- │ │MongoDB │ │Redis  │ │Cloud     │
│Server   │ │greSQL│ │Service │ │Service│ │Storage   │
│Service  │ │Service│         │        │ │Services  │
└─────────┘ └──────┘ └────────┘ └───────┘ └──────────┘
     │         │          │          │          │
     ▼         ▼          ▼          ▼          ▼
┌─────────┐ ┌──────┐ ┌────────┐ ┌───────┐ ┌──────────┐
│SQL      │ │Npgsql│ │MongoDB │ │Stack  │ │Azure/AWS │
│Client   │ │      │ │.Driver │ │Exchange│ │SDKs     │
│+ Dapper │ │      │ │        │ │.Redis │ │         │
└─────────┘ └──────┘ └────────┘ └───────┘ └──────────┘
```

### Service Layer Components

1. **ISqlServerService**: SQL Server operations with Dapper ORM
2. **IPostgreSqlService**: PostgreSQL operations with Npgsql
3. **IMongoDbService**: MongoDB document operations
4. **IRedisService**: Redis caching and data structures
5. **IAzureStorageService**: Azure Blob and Data Lake operations
6. **IAwsStorageService**: AWS S3 and DynamoDB operations
7. **ICachingService**: Multi-level caching with L1/L2 strategy
8. **IStorageManagerService**: Unified storage orchestration

## Installation

### Prerequisites

- .NET 8.0 SDK or later
- At least one database/storage system:
  - SQL Server 2019+ or Azure SQL Database
  - PostgreSQL 13+
  - MongoDB 5.0+
  - Redis 6.0+
  - Azure Storage Account (for Azure services)
  - AWS Account (for AWS services)

### NuGet Packages

```bash
# Relational Databases
dotnet add package Microsoft.Data.SqlClient --version 5.1.5
dotnet add package Dapper --version 2.1.24
dotnet add package Npgsql --version 8.0.1

# NoSQL Databases
dotnet add package MongoDB.Driver --version 2.23.1
dotnet add package StackExchange.Redis --version 2.7.10

# Cloud Storage
dotnet add package Azure.Storage.Blobs --version 12.19.1
dotnet add package Azure.Storage.Files.DataLake --version 12.16.0
dotnet add package AWSSDK.S3 --version 3.7.305.9
dotnet add package AWSSDK.DynamoDBv2 --version 3.7.304.8

# Caching
dotnet add package Microsoft.Extensions.Caching.Memory --version 8.0.0
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis --version 8.0.0

# Monitoring
dotnet add package prometheus-net --version 8.2.1
dotnet add package Serilog.AspNetCore --version 8.0.0
```

### Configuration

Update `appsettings.json` with your connection strings:

```json
{
  "SqlServer": {
    "Server": "localhost",
    "Database": "YourDatabase",
    "UserId": "sa",
    "Password": "YourPassword",
    "MaxPoolSize": 100
  },
  "PostgreSQL": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "yourdb",
    "Username": "postgres",
    "Password": "yourpassword"
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "yourdb"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "Database": 0
  }
}
```

## Quick Start

### 1. Register Services

```csharp
// Program.cs
builder.Services.AddStorageSolutions(builder.Configuration);
```

### 2. Inject and Use Services

```csharp
public class DataService
{
    private readonly ISqlServerService _sqlServer;
    private readonly IMongoDbService _mongo;
    private readonly ICachingService _cache;

    public DataService(
        ISqlServerService sqlServer,
        IMongoDbService mongo,
        ICachingService cache)
    {
        _sqlServer = sqlServer;
        _mongo = mongo;
        _cache = cache;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        // Try cache first
        var cacheKey = "users:all";
        var cached = await _cache.GetAsync<List<User>>(cacheKey);
        if (cached != null) return cached;

        // Query database
        var query = new SqlQuery
        {
            Query = "SELECT * FROM Users WHERE Active = @Active",
            Parameters = new() { ["Active"] = true }
        };
        
        var users = await _sqlServer.QueryAsync<User>(query);

        // Cache results
        var entry = new CacheEntry
        {
            Key = cacheKey,
            Value = users,
            Expiration = TimeSpan.FromMinutes(10)
        };
        await _cache.SetAsync(cacheKey, users, entry);

        return users;
    }
}
```

## Storage Types

### SQL Server

#### Basic Query

```csharp
var query = new SqlQuery
{
    Query = "SELECT * FROM Products WHERE CategoryId = @CategoryId",
    Parameters = new Dictionary<string, object?>
    {
        ["CategoryId"] = 5
    }
};

var result = await _sqlServerService.ExecuteQueryAsync(query);
```

#### Bulk Insert

```csharp
var products = new List<Product>
{
    new Product { Name = "Product 1", Price = 19.99m },
    new Product { Name = "Product 2", Price = 29.99m }
};

var options = new BulkInsertOptions
{
    TableName = "Products",
    BatchSize = 1000,
    Timeout = 300,
    EnableStreaming = true
};

var success = await _sqlServerService.BulkInsertAsync(products, options);
```

#### Transaction

```csharp
var queries = new List<SqlQuery>
{
    new SqlQuery
    {
        Query = "INSERT INTO Orders (CustomerId, Total) VALUES (@CustomerId, @Total)",
        Parameters = new() { ["CustomerId"] = 1, ["Total"] = 99.99 }
    },
    new SqlQuery
    {
        Query = "UPDATE Customers SET LastOrderDate = @Date WHERE Id = @Id",
        Parameters = new() { ["Date"] = DateTime.UtcNow, ["Id"] = 1 }
    }
};

var success = await _sqlServerService.ExecuteTransactionAsync(queries);
```

#### Stored Procedure

```csharp
var parameters = new Dictionary<string, object?>
{
    ["CustomerId"] = 1,
    ["StartDate"] = DateTime.UtcNow.AddMonths(-1),
    ["EndDate"] = DateTime.UtcNow
};

var result = await _sqlServerService.ExecuteStoredProcedureAsync(
    "sp_GetCustomerOrders", 
    parameters);
```

#### Performance Analysis

```csharp
var stats = await _sqlServerService.AnalyzeQueryPerformanceAsync(
    "SELECT * FROM Orders WHERE CustomerId = 1");

Console.WriteLine($"Execution Time: {stats.ExecutionTime}");
Console.WriteLine($"Rows: {stats.RowsAffected}");
Console.WriteLine($"Logical Reads: {stats.LogicalReads}");
```

#### Index Suggestions

```csharp
var suggestions = await _sqlServerService.GetIndexSuggestionsAsync("Orders");

foreach (var suggestion in suggestions)
{
    Console.WriteLine($"Table: {suggestion.TableName}");
    Console.WriteLine($"Columns: {string.Join(", ", suggestion.Columns)}");
    Console.WriteLine($"Improvement: {suggestion.EstimatedImprovement}%");
}
```

### PostgreSQL

#### Query with Prepared Statement

```csharp
var query = new PostgreSqlQuery
{
    Query = "SELECT * FROM products WHERE price > @MinPrice",
    Parameters = new() { ["MinPrice"] = 10.00 },
    UsePreparedStatement = true
};

var products = await _postgreSqlService.QueryAsync<Product>(query);
```

#### Bulk Insert with COPY

```csharp
var products = Enumerable.Range(1, 10000)
    .Select(i => new Product 
    { 
        Name = $"Product {i}", 
        Price = i * 10.0m 
    })
    .ToList();

var success = await _postgreSqlService.BulkInsertAsync(products, "products");
```

#### Create Index

```csharp
var success = await _postgreSqlService.CreateIndexAsync(
    tableName: "products",
    columns: new List<string> { "category_id", "price" },
    indexName: "idx_products_category_price");
```

#### EXPLAIN Query

```csharp
var stats = await _postgreSqlService.ExplainQueryAsync(
    "SELECT * FROM products WHERE category_id = 5 ORDER BY price DESC");

Console.WriteLine($"Execution Time: {stats.ExecutionTime}");
Console.WriteLine($"Rows: {stats.RowsAffected}");
```

### MongoDB

#### Query Documents

```csharp
var query = new MongoQuery
{
    Collection = "products",
    Filter = "{ category: 'Electronics' }",
    Projection = "{ name: 1, price: 1, _id: 0 }",
    Sort = "{ price: -1 }",
    Limit = 10
};

var result = await _mongoDbService.QueryAsync(query);

foreach (var doc in result.Documents)
{
    Console.WriteLine(doc.ToJson());
}
```

#### Insert Document

```csharp
var document = new BsonDocument
{
    { "name", "New Product" },
    { "price", 29.99 },
    { "category", "Electronics" },
    { "tags", new BsonArray { "new", "featured" } },
    { "createdAt", DateTime.UtcNow }
};

var success = await _mongoDbService.InsertAsync("products", document);
```

#### Update Documents

```csharp
var filter = "{ category: 'Electronics' }";
var update = new BsonDocument
{
    { "$set", new BsonDocument { { "onSale", true } } },
    { "$inc", new BsonDocument { { "viewCount", 1 } } }
};

var success = await _mongoDbService.UpdateAsync("products", filter, update);
```

#### Aggregation Pipeline

```csharp
var pipeline = new List<BsonDocument>
{
    BsonDocument.Parse("{ $match: { category: 'Electronics' } }"),
    BsonDocument.Parse("{ $group: { _id: '$brand', avgPrice: { $avg: '$price' }, count: { $sum: 1 } } }"),
    BsonDocument.Parse("{ $sort: { avgPrice: -1 } }")
};

var results = await _mongoDbService.AggregateAsync("products", pipeline);
```

#### Create Index

```csharp
var success = await _mongoDbService.CreateIndexAsync(
    collection: "products",
    field: "category",
    ascending: true);
```

### Redis

#### String Operations

```csharp
// Set value
await _redisService.SetAsync("user:1:name", "John Doe", TimeSpan.FromMinutes(30));

// Get value
var name = await _redisService.GetAsync<string>("user:1:name");

// Check existence
var exists = await _redisService.ExistsAsync("user:1:name");

// Delete
await _redisService.DeleteAsync("user:1:name");
```

#### Hash Operations

```csharp
// Set hash
var userHash = new Dictionary<string, string>
{
    ["id"] = "1",
    ["name"] = "John Doe",
    ["email"] = "john@example.com",
    ["created"] = DateTime.UtcNow.ToString()
};

await _redisService.SetHashAsync("user:1", userHash);

// Get hash
var user = await _redisService.GetHashAsync("user:1");
```

#### List Operations

```csharp
// Add to list
await _redisService.AddToListAsync("recent:views", "product:123");
await _redisService.AddToListAsync("recent:views", "product:456");

// Get list
var recentViews = await _redisService.GetListAsync("recent:views");
```

#### Set Operations

```csharp
// Add to set
await _redisService.AddToSetAsync("user:1:tags", "premium");
await _redisService.AddToSetAsync("user:1:tags", "verified");

// Get set members
var tags = await _redisService.GetSetMembersAsync("user:1:tags");
```

#### Pub/Sub

```csharp
// Subscribe to channel
await _redisService.SubscribeAsync("notifications", message =>
{
    Console.WriteLine($"Received: {message}");
});

// Publish message
await _redisService.PublishAsync("notifications", "New order received");
```

#### Counter Operations

```csharp
// Increment counter
var views = await _redisService.IncrementAsync("page:views", 1);

// Decrement counter
var stock = await _redisService.DecrementAsync("product:123:stock", 1);
```

### Azure Storage

#### Blob Storage

```csharp
// Upload blob
using var fileStream = File.OpenRead("document.pdf");
var options = new BlobUploadOptions
{
    BlobName = "documents/document.pdf",
    ContentType = "application/pdf",
    Metadata = new Dictionary<string, string>
    {
        ["uploadedBy"] = "user123",
        ["uploadedAt"] = DateTime.UtcNow.ToString()
    }
};

await _azureStorageService.UploadBlobAsync(fileStream, options);

// Download blob
var downloadOptions = new BlobDownloadOptions 
{ 
    BlobName = "documents/document.pdf" 
};
var stream = await _azureStorageService.DownloadBlobAsync(downloadOptions);

// List blobs
var blobs = await _azureStorageService.ListBlobsAsync("documents/");

// Check existence
var exists = await _azureStorageService.BlobExistsAsync("documents/document.pdf");

// Get properties
var properties = await _azureStorageService.GetBlobPropertiesAsync("documents/document.pdf");

// Delete blob
await _azureStorageService.DeleteBlobAsync("documents/document.pdf");
```

#### Data Lake Storage

```csharp
// Upload file
using var fileStream = File.OpenRead("data.csv");
var metadata = new Dictionary<string, string>
{
    ["dataType"] = "sales",
    ["region"] = "us-west"
};

await _azureStorageService.UploadToDataLakeAsync(
    "raw/sales/2024/data.csv", 
    fileStream, 
    metadata);

// Download file
var stream = await _azureStorageService.DownloadFromDataLakeAsync(
    "raw/sales/2024/data.csv");

// List files
var files = await _azureStorageService.ListDataLakeFilesAsync("raw/sales/2024/");

foreach (var file in files)
{
    Console.WriteLine($"{file.Path} - {file.Size} bytes - {file.LastModified}");
}

// Delete file
await _azureStorageService.DeleteDataLakeFileAsync("raw/sales/2024/data.csv");
```

### AWS Storage

#### S3 Storage

```csharp
// Upload to S3
using var fileStream = File.OpenRead("image.jpg");
var options = new S3UploadOptions
{
    Key = "images/product-123.jpg",
    ContentType = "image/jpeg",
    Metadata = new Dictionary<string, string>
    {
        ["product-id"] = "123",
        ["uploaded-by"] = "user456"
    },
    ServerSideEncryption = "AES256",
    UseMultipartUpload = true,
    MultipartThresholdMB = 100
};

await _awsStorageService.UploadToS3Async(fileStream, options);

// Download from S3
var stream = await _awsStorageService.DownloadFromS3Async("images/product-123.jpg");

// List objects
var objects = await _awsStorageService.ListS3ObjectsAsync("images/");

// Check existence
var exists = await _awsStorageService.S3ObjectExistsAsync("images/product-123.jpg");

// Delete object
await _awsStorageService.DeleteFromS3Async("images/product-123.jpg");
```

#### DynamoDB

```csharp
// Put item
var item = new Dictionary<string, object>
{
    ["UserId"] = "user123",
    ["OrderId"] = "order456",
    ["Total"] = 99.99,
    ["Items"] = 3,
    ["OrderDate"] = DateTime.UtcNow.ToString()
};

await _awsStorageService.PutDynamoDbItemAsync("Orders", item);

// Get item
var key = new Dictionary<string, object>
{
    ["UserId"] = "user123",
    ["OrderId"] = "order456"
};

var order = await _awsStorageService.GetDynamoDbItemAsync("Orders", key);

// Query items
var query = new DynamoDbQuery
{
    TableName = "Orders",
    KeyConditions = new Dictionary<string, object>
    {
        ["UserId"] = "user123"
    },
    Limit = 10,
    ConsistentRead = true
};

var orders = await _awsStorageService.QueryDynamoDbAsync(query);

// Delete item
await _awsStorageService.DeleteDynamoDbItemAsync("Orders", key);
```

### Caching

#### Basic Caching

```csharp
// Set cache value
var entry = new CacheEntry
{
    Key = "product:123",
    Value = productData,
    Expiration = TimeSpan.FromMinutes(30),
    ExpirationMode = CacheExpirationMode.Absolute
};

await _cachingService.SetAsync("product:123", productData, entry);

// Get cache value
var cached = await _cachingService.GetAsync<Product>("product:123");

// Remove from cache
await _cachingService.RemoveAsync("product:123");

// Check existence
var exists = await _cachingService.ExistsAsync("product:123");
```

#### Cache-Aside Pattern

```csharp
var product = await _cachingService.GetOrSetAsync(
    key: "product:123",
    factory: async () =>
    {
        // This executes only on cache miss
        var query = new SqlQuery
        {
            Query = "SELECT * FROM Products WHERE Id = @Id",
            Parameters = new() { ["Id"] = 123 }
        };
        return await _sqlServerService.QueryAsync<Product>(query);
    },
    expiration: TimeSpan.FromMinutes(15));
```

#### Cache Statistics

```csharp
var stats = await _cachingService.GetCacheStatsAsync();

Console.WriteLine($"Hits: {stats["hits"]}");
Console.WriteLine($"Misses: {stats["misses"]}");
Console.WriteLine($"Hit Rate: {stats["hit_rate"]}%");
Console.WriteLine($"Sets: {stats["sets"]}");
Console.WriteLine($"Removes: {stats["removes"]}");
```

#### Cache Key Builder

```csharp
// Build structured cache keys
var cacheKey = CacheKeyBuilder.Build("product", "123", "details");
// Result: "product:123:details"

var cacheKey2 = new CacheKeyBuilder()
    .WithPrefix("user")
    .WithId("456")
    .WithSuffix("profile")
    .Build();
// Result: "user:456:profile"
```

## Performance Optimization

### Connection Pooling

#### SQL Server

```json
{
  "SqlServer": {
    "MaxPoolSize": 100,
    "MinPoolSize": 10,
    "ConnectionTimeout": 30
  }
}
```

#### PostgreSQL

```json
{
  "PostgreSQL": {
    "Pooling": true,
    "MaxPoolSize": 100,
    "MinPoolSize": 10
  }
}
```

#### MongoDB

```json
{
  "MongoDB": {
    "MaxConnectionPoolSize": 100,
    "MinConnectionPoolSize": 10,
    "MaxConnectionIdleTime": "00:10:00"
  }
}
```

### Bulk Operations

```csharp
// SQL Server Bulk Insert - 1000x faster than row-by-row
var options = new BulkInsertOptions
{
    TableName = "Products",
    BatchSize = 5000,  // Optimal batch size
    EnableStreaming = true,  // Memory efficient
    Timeout = 300
};

await _sqlServerService.BulkInsertAsync(products, options);
```

### Query Optimization

```csharp
// Use parameterized queries (prevents SQL injection + plan caching)
var query = new SqlQuery
{
    Query = "SELECT * FROM Products WHERE CategoryId = @CategoryId AND Price > @MinPrice",
    Parameters = new()
    {
        ["CategoryId"] = 5,
        ["MinPrice"] = 10.00
    }
};

// Use index hints for complex queries
var optimizedQuery = new SqlQuery
{
    Query = "SELECT * FROM Products WITH (INDEX(IX_Products_Category)) WHERE CategoryId = @CategoryId"
};
```

### Caching Strategies

#### Multi-Level Cache

```csharp
// L1 (Memory) + L2 (Redis) caching
// Fast in-memory lookup, distributed consistency
var product = await _cachingService.GetAsync<Product>("product:123");
// Checks memory cache first, then Redis, returns null on miss
```

#### Cache Warming

```csharp
// Pre-populate frequently accessed data
public async Task WarmCacheAsync()
{
    var popularProducts = await _sqlServerService.QueryAsync<Product>(
        new SqlQuery { Query = "SELECT TOP 100 * FROM Products ORDER BY ViewCount DESC" });

    foreach (var product in popularProducts)
    {
        var entry = new CacheEntry
        {
            Key = $"product:{product.Id}",
            Value = product,
            Expiration = TimeSpan.FromHours(1)
        };
        await _cachingService.SetAsync(entry.Key, product, entry);
    }
}
```

#### Cache Expiration Strategies

```csharp
// Absolute expiration - expires at specific time
var absoluteEntry = new CacheEntry
{
    Key = "daily-report",
    Value = report,
    Expiration = TimeSpan.FromHours(24),
    ExpirationMode = CacheExpirationMode.Absolute
};

// Sliding expiration - resets on access
var slidingEntry = new CacheEntry
{
    Key = "user-session",
    Value = session,
    Expiration = TimeSpan.FromMinutes(30),
    ExpirationMode = CacheExpirationMode.Sliding
};
```

### Database Indexing

```csharp
// Get index suggestions from SQL Server
var suggestions = await _sqlServerService.GetIndexSuggestionsAsync("Products");

foreach (var suggestion in suggestions)
{
    if (suggestion.EstimatedImprovement > 50)
    {
        Console.WriteLine($"Create index on {suggestion.TableName}({string.Join(", ", suggestion.Columns)})");
        Console.WriteLine($"Expected improvement: {suggestion.EstimatedImprovement}%");
    }
}
```

## Best Practices

### 1. Always Use Connection Pooling

```csharp
// ✅ GOOD - Connection pooling enabled (default)
var config = new SqlServerConfig
{
    MaxPoolSize = 100,
    MinPoolSize = 10
};

// ❌ BAD - Creating new connections each time
// Don't manually create connections without pooling
```

### 2. Use Transactions for Multiple Operations

```csharp
// ✅ GOOD - Atomic operation
var queries = new List<SqlQuery>
{
    new SqlQuery { Query = "INSERT INTO Orders ..." },
    new SqlQuery { Query = "UPDATE Inventory ..." },
    new SqlQuery { Query = "INSERT INTO OrderItems ..." }
};

await _sqlServerService.ExecuteTransactionAsync(queries);

// ❌ BAD - Partial failure possible
await _sqlServerService.ExecuteAsync(query1);
await _sqlServerService.ExecuteAsync(query2);  // Could fail, leaving inconsistent state
await _sqlServerService.ExecuteAsync(query3);
```

### 3. Always Parameterize Queries

```csharp
// ✅ GOOD - Prevents SQL injection
var query = new SqlQuery
{
    Query = "SELECT * FROM Users WHERE Email = @Email",
    Parameters = new() { ["Email"] = userEmail }
};

// ❌ BAD - SQL injection vulnerability
var badQuery = new SqlQuery
{
    Query = $"SELECT * FROM Users WHERE Email = '{userEmail}'"
};
```

### 4. Use Bulk Operations for Large Datasets

```csharp
// ✅ GOOD - Efficient bulk insert
await _sqlServerService.BulkInsertAsync(10000ProductsList, bulkOptions);

// ❌ BAD - Slow row-by-row insert
foreach (var product in products)
{
    await _sqlServerService.ExecuteAsync(insertQuery);  // 10,000 round trips!
}
```

### 5. Implement Caching for Read-Heavy Operations

```csharp
// ✅ GOOD - Cache frequently accessed data
public async Task<Product> GetProductAsync(int id)
{
    return await _cachingService.GetOrSetAsync(
        $"product:{id}",
        async () => await _sqlServerService.QueryAsync<Product>(query),
        TimeSpan.FromMinutes(15));
}

// ❌ BAD - Hit database every time
public async Task<Product> GetProductAsync(int id)
{
    return await _sqlServerService.QueryAsync<Product>(query);
}
```

### 6. Handle Exceptions Gracefully

```csharp
// ✅ GOOD - Proper error handling
try
{
    var result = await _sqlServerService.ExecuteQueryAsync(query);
    if (!result.Success)
    {
        _logger.LogError("Query failed: {Error}", result.ErrorMessage);
        return fallbackData;
    }
}
catch (SqlException ex) when (ex.Number == 1205)  // Deadlock
{
    _logger.LogWarning("Deadlock detected, retrying...");
    await Task.Delay(100);
    // Retry logic
}

// ❌ BAD - Silent failure
try
{
    var result = await _sqlServerService.ExecuteQueryAsync(query);
}
catch { }  // Swallows all errors
```

### 7. Use Async/Await Properly

```csharp
// ✅ GOOD - Properly async
public async Task<List<Product>> GetProductsAsync()
{
    var query = new SqlQuery { Query = "SELECT * FROM Products" };
    return await _sqlServerService.QueryAsync<Product>(query);
}

// ❌ BAD - Blocking async code
public List<Product> GetProducts()
{
    var query = new SqlQuery { Query = "SELECT * FROM Products" };
    return _sqlServerService.QueryAsync<Product>(query).Result;  // Deadlock risk!
}
```

### 8. Monitor and Log Performance

```csharp
// ✅ GOOD - Log slow queries
var stopwatch = Stopwatch.StartNew();
var result = await _sqlServerService.ExecuteQueryAsync(query);
stopwatch.Stop();

if (stopwatch.ElapsedMilliseconds > 1000)
{
    _logger.LogWarning("Slow query detected: {Query}, Time: {Time}ms", 
        query.Query, stopwatch.ElapsedMilliseconds);
}
```

### 9. Use Appropriate Storage for Data Type

```csharp
// ✅ GOOD - Use appropriate storage
// - Structured data → SQL Server/PostgreSQL
await _sqlServerService.ExecuteAsync(structuredQuery);

// - Documents/JSON → MongoDB
await _mongoDbService.InsertAsync("products", document);

// - Key-value cache → Redis
await _redisService.SetAsync("session:123", sessionData);

// - Large files → Azure Blob/AWS S3
await _azureStorageService.UploadBlobAsync(fileStream, blobOptions);

// - Analytics data → Azure Data Lake
await _azureStorageService.UploadToDataLakeAsync(path, stream);
```

### 10. Implement Health Checks

```csharp
// ✅ GOOD - Test connections on startup
var sqlHealthy = await _storageManagerService.TestConnectionAsync(StorageType.SqlServer);
var mongoHealthy = await _storageManagerService.TestConnectionAsync(StorageType.MongoDB);
var redisHealthy = await _storageManagerService.TestConnectionAsync(StorageType.Redis);

if (!sqlHealthy || !mongoHealthy || !redisHealthy)
{
    _logger.LogError("Storage health check failed");
    // Alert operations team
}
```

## API Reference

### REST Endpoints

#### SQL Server

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/sqlserver/query` | Execute SELECT query |
| POST | `/api/storage/sqlserver/execute` | Execute INSERT/UPDATE/DELETE |
| POST | `/api/storage/sqlserver/bulk-insert` | Bulk insert data |
| POST | `/api/storage/sqlserver/transaction` | Execute transaction |
| POST | `/api/storage/sqlserver/stored-procedure` | Execute stored procedure |
| GET | `/api/storage/sqlserver/index-suggestions/{table}` | Get index suggestions |

#### PostgreSQL

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/postgresql/query` | Execute query |
| POST | `/api/storage/postgresql/execute` | Execute command |
| GET | `/api/storage/postgresql/tables` | List tables |
| POST | `/api/storage/postgresql/create-index` | Create index |

#### MongoDB

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/mongodb/query` | Query documents |
| POST | `/api/storage/mongodb/insert` | Insert document |
| POST | `/api/storage/mongodb/update` | Update documents |
| POST | `/api/storage/mongodb/delete` | Delete documents |
| GET | `/api/storage/mongodb/collections` | List collections |
| POST | `/api/storage/mongodb/aggregate` | Run aggregation pipeline |

#### Redis

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/storage/redis/get/{key}` | Get value |
| POST | `/api/storage/redis/set` | Set value |
| DELETE | `/api/storage/redis/{key}` | Delete key |
| GET | `/api/storage/redis/exists/{key}` | Check existence |
| POST | `/api/storage/redis/hash/set` | Set hash |
| GET | `/api/storage/redis/hash/get/{key}` | Get hash |

#### Azure Storage

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/azure/blob/upload` | Upload blob |
| GET | `/api/storage/azure/blob/download/{name}` | Download blob |
| GET | `/api/storage/azure/blob/list` | List blobs |
| DELETE | `/api/storage/azure/blob/{name}` | Delete blob |
| POST | `/api/storage/azure/datalake/upload` | Upload to Data Lake |
| GET | `/api/storage/azure/datalake/list` | List Data Lake files |

#### AWS Storage

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/aws/s3/upload` | Upload to S3 |
| GET | `/api/storage/aws/s3/download/{key}` | Download from S3 |
| GET | `/api/storage/aws/s3/list` | List S3 objects |
| DELETE | `/api/storage/aws/s3/{key}` | Delete from S3 |
| POST | `/api/storage/aws/dynamodb/put` | Put DynamoDB item |
| POST | `/api/storage/aws/dynamodb/get` | Get DynamoDB item |

#### Caching

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/storage/cache/get/{key}` | Get cached value |
| POST | `/api/storage/cache/set` | Set cached value |
| DELETE | `/api/storage/cache/{key}` | Remove from cache |
| GET | `/api/storage/cache/stats` | Get cache statistics |

#### Storage Manager

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/storage/manager/execute` | Execute storage operation |
| GET | `/api/storage/manager/metrics/{type}` | Get storage metrics |
| GET | `/api/storage/manager/metrics` | Get all metrics |
| POST | `/api/storage/manager/test-connection/{type}` | Test connection |
| POST | `/api/storage/manager/backup` | Create backup |

## Testing

### Unit Testing Example

```csharp
using Xunit;
using Moq;

public class SqlServerServiceTests
{
    private readonly Mock<ILogger<SqlServerService>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;

    public SqlServerServiceTests()
    {
        _loggerMock = new Mock<ILogger<SqlServerService>>();
        _configMock = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task ExecuteQueryAsync_ValidQuery_ReturnsResults()
    {
        // Arrange
        var service = new SqlServerService(_loggerMock.Object, _configMock.Object);
        var query = new SqlQuery
        {
            Query = "SELECT * FROM Products WHERE Id = @Id",
            Parameters = new() { ["Id"] = 1 }
        };

        // Act
        var result = await service.ExecuteQueryAsync(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Data);
    }
}
```

### Integration Testing

```csharp
[Fact]
public async Task BulkInsert_LargeDataset_SuccessfullyInserts()
{
    // Arrange
    var products = Enumerable.Range(1, 10000)
        .Select(i => new Product { Name = $"Product {i}", Price = i * 10.0m })
        .ToList();

    var options = new BulkInsertOptions
    {
        TableName = "Products",
        BatchSize = 1000
    };

    // Act
    var success = await _sqlServerService.BulkInsertAsync(products, options);

    // Assert
    Assert.True(success);
    
    var count = await _sqlServerService.ExecuteScalarAsync<int>(
        new SqlQuery { Query = "SELECT COUNT(*) FROM Products" });
    Assert.Equal(10000, count);
}
```

### Performance Testing

```csharp
[Fact]
public async Task CachingService_MultipleRequests_ImprovesPerformance()
{
    var stopwatch = Stopwatch.StartNew();

    // First request - cache miss
    var product1 = await _cachingService.GetOrSetAsync(
        "product:1",
        async () => await GetProductFromDb(1),
        TimeSpan.FromMinutes(5));

    var firstRequestTime = stopwatch.ElapsedMilliseconds;

    // Second request - cache hit
    stopwatch.Restart();
    var product2 = await _cachingService.GetAsync<Product>("product:1");
    var secondRequestTime = stopwatch.ElapsedMilliseconds;

    // Cache hit should be significantly faster
    Assert.True(secondRequestTime < firstRequestTime / 10);
}
```

## Troubleshooting

### Connection Issues

#### SQL Server Connection Timeout

```
Error: Connection Timeout Expired
```

**Solution:**
```json
{
  "SqlServer": {
    "CommandTimeout": 60,  // Increase timeout
    "MaxPoolSize": 200,    // Increase pool size
    "ConnectTimeout": 30
  }
}
```

#### PostgreSQL Connection Refused

```
Error: Connection refused
```

**Solution:**
1. Check PostgreSQL is running: `sudo systemctl status postgresql`
2. Verify pg_hba.conf allows connections
3. Check firewall: `sudo ufw allow 5432/tcp`

#### MongoDB Authentication Failed

```
Error: Authentication failed
```

**Solution:**
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://username:password@localhost:27017/database?authSource=admin"
  }
}
```

### Performance Issues

#### Slow Query Performance

**Diagnosis:**
```csharp
var stats = await _sqlServerService.AnalyzeQueryPerformanceAsync(slowQuery);
Console.WriteLine($"Execution Time: {stats.ExecutionTime}");
Console.WriteLine($"Logical Reads: {stats.LogicalReads}");
```

**Solutions:**
1. Add indexes based on suggestions
2. Rewrite query with better joins
3. Use query hints
4. Consider partitioning large tables

#### Cache Hit Rate Too Low

**Diagnosis:**
```csharp
var stats = await _cachingService.GetCacheStatsAsync();
var hitRate = stats["hit_rate"];

if (hitRate < 80)
{
    Console.WriteLine("Cache hit rate too low, review caching strategy");
}
```

**Solutions:**
1. Increase cache expiration times
2. Pre-warm cache for frequently accessed data
3. Review cache key patterns
4. Consider larger Redis instance

### Memory Issues

#### Redis Out of Memory

```
Error: OOM command not allowed when used memory > 'maxmemory'
```

**Solution:**
```bash
# Set Redis maxmemory policy
redis-cli CONFIG SET maxmemory-policy allkeys-lru
redis-cli CONFIG SET maxmemory 2gb
```

#### MongoDB High Memory Usage

**Solution:**
1. Add indexes to reduce collection scans
2. Use projection to limit returned fields
3. Implement data retention policies
4. Consider sharding for large datasets

### Data Consistency Issues

#### Transaction Deadlock

```
Error: Transaction (Process ID) was deadlocked
```

**Solution:**
```csharp
// Implement retry logic with exponential backoff
public async Task<bool> ExecuteWithRetry(SqlQuery query, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            await _sqlServerService.ExecuteAsync(query);
            return true;
        }
        catch (SqlException ex) when (ex.Number == 1205)  // Deadlock
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(TimeSpan.FromMilliseconds(Math.Pow(2, i) * 100));
        }
    }
    return false;
}
```

## Dependencies

### Core Packages
- Microsoft.Data.SqlClient 5.1.5
- Npgsql 8.0.1
- MongoDB.Driver 2.23.1
- StackExchange.Redis 2.7.10
- Dapper 2.1.24

### Cloud Packages
- Azure.Storage.Blobs 12.19.1
- Azure.Storage.Files.DataLake 12.16.0
- AWSSDK.S3 3.7.305.9
- AWSSDK.DynamoDBv2 3.7.304.8

### Monitoring & Logging
- prometheus-net 8.2.1
- Serilog.AspNetCore 8.0.0

## License

This module is part of the CSharp-AI-Modules-StepByStep learning repository.

## Contributing

For issues, improvements, or questions, please refer to the main repository documentation.
