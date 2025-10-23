# Module-02-ETL-Pipelines

Comprehensive ETL (Extract, Transform, Load) pipeline service for AI data processing with support for multiple data sources, transformations, validations, and data quality monitoring.

## Features

### Core Capabilities

- **Extract**: SQL Server, PostgreSQL, MongoDB, CSV, JSON
- **Transform**: Map, Filter, Cleanse, Validate, Aggregate, Enrich
- **Load**: SQL Server, PostgreSQL, MongoDB, CSV, JSON
- **Validation**: Required fields, ranges, patterns, data types
- **Data Quality**: Completeness, uniqueness, quality scoring
- **Error Handling**: Retry logic, continue on error, error logging
- **Performance**: Batch processing, parallel execution, caching

### Architecture

```
AI.ETL/
├── Models/
│   └── ETLModels.cs               # 50+ model classes
├── Services/
│   ├── ExtractionService.cs       # Extract from multiple sources
│   ├── TransformationService.cs   # Transform and map data
│   ├── LoadService.cs             # Load to destinations
│   ├── ValidationService.cs       # Data validation & quality
│   └── ETLPipelineService.cs      # Pipeline orchestration
├── Controllers/
│   └── ETLController.cs           # REST API endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server or PostgreSQL (optional)
- MongoDB (optional)

### Configuration

Update `appsettings.json`:

```json
{
  "DataSources": {
    "SqlServer": {
      "ConnectionString": "Server=localhost;Database=ETL_Source;..."
    }
  }
}
```

### Run the Application

```bash
cd Module-02-ETL-Pipelines
dotnet restore
dotnet build
dotnet run
```

API available at `https://localhost:5001` (Swagger at root).

## API Examples

### 1. Create ETL Job

```bash
POST /api/etl/jobs
Content-Type: application/json

{
  "name": "Customer Data Migration",
  "description": "Migrate customers from SQL Server to PostgreSQL",
  "configuration": {
    "source": {
      "type": "SqlServer",
      "connectionString": "Server=localhost;Database=Source;...",
      "query": "SELECT * FROM Customers WHERE Active = 1",
      "timeoutSeconds": 30
    },
    "destination": {
      "type": "PostgreSQL",
      "connectionString": "Host=localhost;Database=destination;...",
      "tableName": "customers"
    },
    "transformations": [
      {
        "name": "Map Fields",
        "type": "Map",
        "order": 1,
        "parameters": {
          "CustomerID": "customer_id",
          "CustomerName": "full_name",
          "Email": "email_address"
        }
      },
      {
        "name": "Cleanse Data",
        "type": "Cleanse",
        "order": 2,
        "parameters": {
          "ToUpper": "false",
          "RemoveSpecialChars": "true"
        }
      }
    ],
    "validation": {
      "enabled": true,
      "failOnError": true,
      "rules": [
        {
          "name": "Email Required",
          "fieldName": "Email",
          "ruleType": "Required",
          "severity": "Error"
        },
        {
          "name": "Email Format",
          "fieldName": "Email",
          "ruleType": "Email",
          "severity": "Warning"
        }
      ]
    },
    "performance": {
      "batchSize": 1000,
      "maxDegreeOfParallelism": 4,
      "useParallelProcessing": true
    }
  }
}
```

Response:
```json
{
  "success": true,
  "data": {
    "jobId": "abc123-def456-...",
    "status": "Pending",
    "message": "ETL job created successfully",
    "createdAt": "2025-10-22T10:00:00Z"
  }
}
```

### 2. Execute ETL Pipeline

```bash
POST /api/etl/jobs/{jobId}/execute
Content-Type: application/json

{
  "asyncExecution": true
}
```

Response (Async):
```json
{
  "success": true,
  "data": {
    "executionId": "exec-123",
    "status": "Running",
    "message": "ETL pipeline execution started asynchronously"
  }
}
```

Response (Sync):
```json
{
  "success": true,
  "data": {
    "executionId": "exec-123",
    "status": "Completed",
    "message": "ETL pipeline completed successfully",
    "metrics": {
      "totalRecordsExtracted": 10000,
      "totalRecordsTransformed": 9950,
      "totalRecordsLoaded": 9950,
      "totalRecordsFailed": 50,
      "recordsPerSecond": 500.5,
      "extractionTime": "00:00:05",
      "transformationTime": "00:00:10",
      "loadTime": "00:00:05",
      "memoryUsedMB": 256
    }
  }
}
```

### 3. Get Job Status

```bash
GET /api/etl/jobs/{jobId}
```

Response:
```json
{
  "success": true,
  "data": {
    "jobId": "abc123-def456-...",
    "name": "Customer Data Migration",
    "status": "Completed",
    "createdAt": "2025-10-22T10:00:00Z",
    "startedAt": "2025-10-22T10:01:00Z",
    "completedAt": "2025-10-22T10:01:30Z",
    "totalRecords": 10000,
    "processedRecords": 9950,
    "failedRecords": 50
  }
}
```

### 4. Get All Jobs

```bash
GET /api/etl/jobs
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "jobId": "job-1",
      "name": "Customer Migration",
      "status": "Completed",
      "totalRecords": 10000
    },
    {
      "jobId": "job-2",
      "name": "Product Import",
      "status": "Running",
      "processedRecords": 5000
    }
  ]
}
```

### 5. Validate Data

```bash
POST /api/etl/validate
Content-Type: application/json

{
  "data": [
    {
      "Name": "John Doe",
      "Email": "john@example.com",
      "Age": 30
    },
    {
      "Name": "Jane Smith",
      "Email": "invalid-email",
      "Age": -5
    }
  ],
  "rules": [
    {
      "name": "Email Format",
      "fieldName": "Email",
      "ruleType": "Email",
      "severity": "Error"
    },
    {
      "name": "Age Range",
      "fieldName": "Age",
      "ruleType": "Range",
      "parameters": {
        "Min": 0,
        "Max": 120
      },
      "severity": "Error"
    }
  ]
}
```

Response:
```json
{
  "success": true,
  "data": {
    "isValid": false,
    "errors": [
      {
        "recordIndex": 1,
        "fieldName": "Email",
        "ruleName": "Email Format",
        "message": "Invalid email format",
        "severity": "Error",
        "actualValue": "invalid-email"
      },
      {
        "recordIndex": 1,
        "fieldName": "Age",
        "ruleName": "Age Range",
        "message": "Value must be between 0 and 120",
        "severity": "Error",
        "actualValue": -5
      }
    ],
    "totalRecords": 2,
    "validRecords": 1,
    "invalidRecords": 1
  }
}
```

### 6. Generate Data Quality Report

```bash
POST /api/etl/jobs/{jobId}/quality-report
Content-Type: application/json

[
  { "Name": "John", "Email": "john@example.com", "Phone": "123-456-7890" },
  { "Name": "Jane", "Email": "", "Phone": "987-654-3210" },
  { "Name": "Bob", "Email": "bob@example.com", "Phone": "" }
]
```

Response:
```json
{
  "success": true,
  "data": {
    "reportId": "report-123",
    "jobId": "job-abc",
    "totalRecords": 3,
    "validRecords": 2,
    "invalidRecords": 1,
    "qualityScore": 88.89,
    "fieldMetrics": {
      "Name": {
        "totalValues": 3,
        "nullValues": 0,
        "uniqueValues": 3,
        "completenessPercentage": 100,
        "uniquenessPercentage": 100
      },
      "Email": {
        "totalValues": 3,
        "nullValues": 1,
        "uniqueValues": 2,
        "completenessPercentage": 66.67,
        "uniquenessPercentage": 100
      },
      "Phone": {
        "totalValues": 3,
        "nullValues": 1,
        "uniqueValues": 2,
        "completenessPercentage": 66.67,
        "uniquenessPercentage": 100
      }
    }
  }
}
```

### 7. Cancel Job

```bash
POST /api/etl/jobs/{jobId}/cancel
```

## Transformation Types

### 1. Map (Field Mapping)
```json
{
  "type": "Map",
  "parameters": {
    "SourceField1": "DestField1",
    "SourceField2": "DestField2"
  }
}
```

### 2. Filter (Data Filtering)
```json
{
  "type": "Filter",
  "parameters": {
    "Status": "Active",
    "Country": "USA"
  }
}
```

### 3. Cleanse (Data Cleaning)
```json
{
  "type": "Cleanse",
  "parameters": {
    "RemoveSpecialChars": "true",
    "ToUpper": "false",
    "ToLower": "false"
  }
}
```

### 4. Validate (Data Validation)
```json
{
  "type": "Validate",
  "parameters": {
    "Email": "email",
    "Phone": "required",
    "Age": "numeric"
  }
}
```

### 5. Aggregate (Data Aggregation)
```json
{
  "type": "Aggregate",
  "parameters": {
    "GroupBy": "Category",
    "Sum": "Amount",
    "Avg": "Price",
    "Count": "*"
  }
}
```

### 6. Enrich (Add Fields)
```json
{
  "type": "Enrich",
  "parameters": {
    "ProcessedDate": "2025-10-22",
    "DataSource": "API"
  }
}
```

## Validation Rules

### Required Field
```json
{
  "fieldName": "Email",
  "ruleType": "Required",
  "severity": "Error"
}
```

### Range Validation
```json
{
  "fieldName": "Age",
  "ruleType": "Range",
  "parameters": {
    "Min": 18,
    "Max": 65
  }
}
```

### Pattern (Regex)
```json
{
  "fieldName": "Phone",
  "ruleType": "Pattern",
  "parameters": {
    "Regex": "^\\d{3}-\\d{3}-\\d{4}$"
  }
}
```

### Length Validation
```json
{
  "fieldName": "Name",
  "ruleType": "Length",
  "parameters": {
    "MinLength": 2,
    "MaxLength": 50
  }
}
```

### Email Validation
```json
{
  "fieldName": "Email",
  "ruleType": "Email"
}
```

### Numeric Validation
```json
{
  "fieldName": "Salary",
  "ruleType": "Numeric"
}
```

### Date Validation
```json
{
  "fieldName": "BirthDate",
  "ruleType": "Date"
}
```

## Performance Optimization

### Batch Processing
```json
{
  "performance": {
    "batchSize": 5000,
    "useParallelProcessing": true,
    "maxDegreeOfParallelism": 8
  }
}
```

### Memory Management
```json
{
  "performance": {
    "memoryLimitMB": 2048,
    "enableCaching": true,
    "cacheDurationMinutes": 60
  }
}
```

## Error Handling

### Continue on Error
```json
{
  "errorHandling": {
    "continueOnError": true,
    "maxRetries": 3,
    "retryDelaySeconds": 5,
    "logErrors": true,
    "createErrorFile": true
  }
}
```

## Complete Example: CSV to SQL Server

```bash
POST /api/etl/jobs
{
  "name": "Import Products from CSV",
  "configuration": {
    "source": {
      "type": "CsvFile",
      "filePath": "C:\\Data\\products.csv"
    },
    "destination": {
      "type": "SqlServer",
      "connectionString": "Server=localhost;Database=Store;...",
      "tableName": "Products"
    },
    "transformations": [
      {
        "name": "Clean Product Names",
        "type": "Cleanse",
        "order": 1,
        "parameters": {
          "RemoveSpecialChars": "false"
        }
      },
      {
        "name": "Validate Prices",
        "type": "Validate",
        "order": 2,
        "parameters": {
          "Price": "numeric"
        }
      }
    ],
    "validation": {
      "enabled": true,
      "rules": [
        {
          "fieldName": "ProductName",
          "ruleType": "Required"
        },
        {
          "fieldName": "Price",
          "ruleType": "Range",
          "parameters": { "Min": 0, "Max": 999999 }
        }
      ]
    }
  }
}
```

## Data Sources Supported

- **SQL Server**: Full CRUD with bulk insert
- **PostgreSQL**: Batch operations with transactions
- **MongoDB**: Document-based operations
- **CSV Files**: Read/Write with CsvHelper
- **JSON Files**: Structured data processing

## Key Features

### Pipeline Stages
1. **Extract**: Pull data from source
2. **Validate**: Check data quality (optional)
3. **Transform**: Apply transformations
4. **Load**: Write to destination

### Data Quality
- Completeness scoring
- Uniqueness analysis
- Field-level metrics
- Quality reports

### Monitoring
- Real-time status tracking
- Stage-level metrics
- Performance statistics
- Error tracking

## Performance Metrics

- Records extracted/transformed/loaded
- Records per second
- Execution time per stage
- Memory usage
- Error counts

## Logging

Comprehensive logging with Serilog:
- Console output
- File logs: `logs/etl-YYYYMMDD.txt`
- Structured logging
- Performance timing

## Best Practices

1. **Batch Size**: 1000-5000 for optimal performance
2. **Validation**: Enable for data quality
3. **Error Handling**: Use continue-on-error for resilience
4. **Parallel Processing**: Enable for large datasets
5. **Monitoring**: Track metrics and logs

## Dependencies

- Microsoft.Data.SqlClient 5.1.2
- Npgsql 8.0.1
- MongoDB.Driver 2.23.1
- CsvHelper 30.0.1
- FluentValidation 11.9.0
- Hangfire 1.8.6
- Serilog 8.0.0

## Testing

```bash
dotnet test
```

## Troubleshooting

### Connection Issues
- Verify connection strings
- Check firewall settings
- Ensure services are running

### Performance Issues
- Adjust batch size
- Enable parallel processing
- Increase memory limits

### Validation Errors
- Review validation rules
- Check data quality report
- Enable continue-on-error

## Next Steps

1. Add Redis caching
2. Implement Kafka streaming
3. Add workflow orchestration with Elsa
4. Implement job scheduling with Quartz
5. Add distributed tracing
6. Implement data lineage tracking

## License

MIT License

## Support

For questions: etl@example.com
