# Module 06 - Response Processing

Comprehensive response processing system with parsing, validation, formatting, error handling, and rate limiting capabilities.

## Overview

This module provides production-ready tools for processing AI model responses:
- **Response Parsing**: JSON, XML, YAML, Markdown, CSV parsing with code block extraction
- **Validation**: Schema validation, custom rules, content sanitization
- **Output Formatting**: Convert between formats with pretty printing
- **Error Handling**: Retry policies, circuit breakers, graceful degradation
- **Rate Limiting**: Multiple strategies to prevent API abuse

## Features

### 1. Response Parsing

#### Parse JSON
```csharp
var request = new ParsingRequest
{
    Content = "{\"name\": \"John\", \"age\": 30}",
    Format = ResponseFormat.Json,
    Options = new ParsingOptions
    {
        StrictMode = false,
        TrimWhitespace = true
    }
};

var result = await _parser.ParseJsonAsync(request.Content, request.Options);

if (result.Success)
{
    var name = result.Data["name"];
    var age = result.Data["age"];
}
```

#### Parse XML
```csharp
var xmlContent = "<person><name>John</name><age>30</age></person>";
var result = await _parser.ParseXmlAsync(xmlContent);

if (result.Success)
{
    var root = result.Data.Root;
    var name = root.Element("name")?.Value;
}
```

#### Parse Markdown with Extraction
```csharp
var markdownContent = @"
# Title

Here's some code:

```csharp
public void Method() { }
```

Visit [Google](https://google.com)

| Name | Age |
|------|-----|
| John | 30  |
";

var options = new ParsingOptions
{
    ExtractCodeBlocks = true,
    ExtractLinks = true,
    ExtractTables = true
};

var result = await _parser.ParseMarkdownAsync(markdownContent, options);

// Access extracted elements
var codeBlocks = result.ExtractedMetadata["codeBlocks"] as List<CodeBlock>;
var links = result.ExtractedMetadata["links"] as List<ExtractedLink>;
var tables = result.ExtractedMetadata["tables"] as List<ExtractedTable>;
```

#### Extract Code Blocks
```csharp
var content = @"
Here's Python code:
```python
def hello():
    print('Hello World')
```

And TypeScript:
```typescript
const greet = () => console.log('Hello');
```
";

var codeBlocks = await _parser.ExtractCodeBlocksAsync(content);

foreach (var block in codeBlocks)
{
    Console.WriteLine($"Language: {block.Language}");
    Console.WriteLine($"Code: {block.Code}");
}
```

#### Extract Structured Data
```csharp
var structuredData = await _parser.ExtractStructuredDataAsync(
    jsonContent,
    ResponseFormat.Json
);

// Access fields
foreach (var (key, value) in structuredData.Fields)
{
    Console.WriteLine($"{key}: {value}");
}
```

### 2. Validation

#### Validate with Rules
```csharp
var validationRequest = new ValidationRequest
{
    Data = new { Name = "John", Email = "john@example.com", Age = 25 },
    Rules = new List<ValidationRule>
    {
        new ValidationRule
        {
            FieldPath = "Name",
            RuleType = "Required",
            ErrorMessage = "Name is required"
        },
        new ValidationRule
        {
            FieldPath = "Name",
            RuleType = "MinLength",
            Value = 3,
            ErrorMessage = "Name must be at least 3 characters"
        },
        new ValidationRule
        {
            FieldPath = "Email",
            RuleType = "Email",
            ErrorMessage = "Invalid email format"
        },
        new ValidationRule
        {
            FieldPath = "Age",
            RuleType = "Range",
            Value = "18,100",
            ErrorMessage = "Age must be between 18 and 100"
        }
    },
    StopOnFirstError = false
};

var result = await _validation.ValidateAsync(validationRequest);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Field}: {error.Message}");
    }
}
```

#### Validate JSON Schema
```csharp
var jsonSchema = @"{
    ""required"": [""name"", ""email""],
    ""properties"": {
        ""name"": { ""type"": ""string"" },
        ""email"": { ""type"": ""string"" }
    }
}";

var json = @"{""name"": ""John"", ""email"": ""john@example.com""}";

var result = await _validation.ValidateJsonSchemaAsync(json, jsonSchema);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.Message);
    }
}
```

#### Sanitize Content
```csharp
var sanitizationRequest = new SanitizationRequest
{
    Content = "<script>alert('XSS')</script><p>Safe content</p>",
    Options = new SanitizationOptions
    {
        RemoveHtmlTags = true,
        RemoveScripts = true,
        RemoveXss = true,
        RemoveSqlInjection = true
    }
};

var result = await _validation.SanitizeAsync(sanitizationRequest);

Console.WriteLine($"Clean: {result.CleanContent}");
Console.WriteLine($"Removed: {string.Join(", ", result.RemovedElements)}");
Console.WriteLine($"Modified: {result.WasModified}");
```

### 3. Output Formatting

#### Format as JSON
```csharp
var data = new
{
    Name = "John",
    Age = 30,
    Skills = new[] { "C#", "Python", "TypeScript" }
};

var options = new FormattingOptions
{
    PrettyPrint = true,
    IndentSize = 2,
    DateFormat = "yyyy-MM-dd"
};

var json = await _formatting.FormatAsJsonAsync(data, options);
Console.WriteLine(json);
```

#### Format as XML
```csharp
var xml = await _formatting.FormatAsXmlAsync(data, new FormattingOptions
{
    PrettyPrint = true
});
Console.WriteLine(xml);
```

#### Format as YAML
```csharp
var yaml = await _formatting.FormatAsYamlAsync(data);
Console.WriteLine(yaml);
```

#### Format as CSV
```csharp
var people = new List<Person>
{
    new Person { Name = "John", Age = 30, City = "New York" },
    new Person { Name = "Jane", Age = 25, City = "London" }
};

var csv = await _formatting.FormatAsCsvAsync(people);
Console.WriteLine(csv);
```

#### Format as Markdown Table
```csharp
var markdown = await _formatting.FormatAsMarkdownTableAsync(people);
Console.WriteLine(markdown);

// Output:
// | Name | Age | City |
// | --- | --- | --- |
// | John | 30 | New York |
// | Jane | 25 | London |
```

#### Universal Format Conversion
```csharp
var formattingRequest = new FormattingRequest
{
    Data = data,
    TargetFormat = ResponseFormat.Yaml,
    Options = new FormattingOptions { PrettyPrint = true }
};

var result = await _formatting.FormatAsync(formattingRequest);

Console.WriteLine($"Format: {result.Format}");
Console.WriteLine($"Content-Type: {result.ContentType}");
Console.WriteLine($"Content:\n{result.Content}");
```

### 4. Error Handling

#### Execute with Retry
```csharp
var retryPolicy = new RetryPolicy
{
    MaxAttempts = 5,
    InitialDelay = TimeSpan.FromSeconds(1),
    BackoffMultiplier = 2.0,
    MaxDelay = TimeSpan.FromSeconds(30)
};

var result = await _errorHandling.ExecuteWithRetryAsync(async () =>
{
    // Your operation that might fail
    var response = await CallExternalApiAsync();
    return response;
}, retryPolicy);

if (result.Success)
{
    Console.WriteLine($"Succeeded after {result.AttemptsMade} attempts");
    Console.WriteLine($"Duration: {result.TotalDuration}");
}
else
{
    Console.WriteLine($"Failed after {result.AttemptsMade} attempts");
    Console.WriteLine($"Last error: {result.LastException?.Message}");
}
```

#### Execute with Error Handling Strategy
```csharp
var config = new ErrorHandlingConfig
{
    Strategy = ErrorHandlingStrategy.ReturnDefault,
    MaxRetries = 3,
    RetryDelayMs = 1000,
    DefaultValue = "Default Response",
    RetryableExceptions = new List<Type>
    {
        typeof(HttpRequestException),
        typeof(TimeoutException)
    }
};

var result = await _errorHandling.ExecuteWithErrorHandlingAsync(
    async () => await RiskyOperationAsync(),
    config,
    "ProcessUserRequest"
);

if (result.Success)
{
    Console.WriteLine($"Result: {result.Data}");
}
else
{
    Console.WriteLine($"Returned default after {result.AttemptsCount} attempts");
}
```

#### Log and Track Errors
```csharp
var errorContext = new ErrorContext
{
    Exception = new Exception("Something went wrong"),
    Operation = "ProcessPayment",
    UserId = "user123",
    CorrelationId = Guid.NewGuid().ToString(),
    AdditionalData = new Dictionary<string, object>
    {
        { "Amount", 99.99m },
        { "Currency", "USD" }
    }
};

await _errorHandling.LogErrorAsync(errorContext);

// Get recent errors
var recentErrors = await _errorHandling.GetRecentErrorsAsync(10);
foreach (var error in recentErrors)
{
    Console.WriteLine($"{error.Timestamp}: {error.Operation} - {error.Exception.Message}");
}
```

### 5. Rate Limiting

#### Check Rate Limit
```csharp
var rateLimitRequest = new RateLimitRequest
{
    UserId = "user123",
    Endpoint = "/api/process",
    Weight = 1
};

var result = await _rateLimiting.CheckRateLimitAsync(rateLimitRequest);

if (result.Allowed)
{
    Console.WriteLine($"Request allowed. Remaining: {result.RemainingRequests}");
    // Process request
}
else
{
    Console.WriteLine($"Rate limit exceeded. Retry after: {result.RetryAfter}");
    Console.WriteLine($"Reset time: {result.ResetTime}");
}
```

#### Get Rate Limit Info
```csharp
var info = await _rateLimiting.GetRateLimitInfoAsync("user_user123");

Console.WriteLine($"Limit: {info.Limit}");
Console.WriteLine($"Remaining: {info.Remaining}");
Console.WriteLine($"Reset Time: {info.ResetTime}");
Console.WriteLine($"Window: {info.WindowDuration}");
```

#### Reset Rate Limit
```csharp
await _rateLimiting.ResetRateLimitAsync("user_user123");
Console.WriteLine("Rate limit reset successfully");
```

## API Endpoints

### Parsing Endpoints

```http
POST /api/responseprocessing/parse/json
POST /api/responseprocessing/parse/xml
POST /api/responseprocessing/parse/markdown
POST /api/responseprocessing/parse/code-blocks
POST /api/responseprocessing/parse/links
POST /api/responseprocessing/parse/tables
POST /api/responseprocessing/parse/structured
```

**Example: Parse JSON**
```bash
curl -X POST https://localhost:5001/api/responseprocessing/parse/json \
  -H "Content-Type: application/json" \
  -d '{
    "content": "{\"name\":\"John\",\"age\":30}",
    "format": "Json",
    "options": {
      "strictMode": false,
      "trimWhitespace": true
    }
  }'
```

### Validation Endpoints

```http
POST /api/responseprocessing/validate
POST /api/responseprocessing/validate/json-schema
POST /api/responseprocessing/validate/sanitize
```

**Example: Validate Data**
```bash
curl -X POST https://localhost:5001/api/responseprocessing/validate \
  -H "Content-Type: application/json" \
  -d '{
    "data": {"name": "John", "email": "john@example.com"},
    "rules": [
      {
        "fieldPath": "email",
        "ruleType": "Email",
        "errorMessage": "Invalid email"
      }
    ]
  }'
```

### Formatting Endpoints

```http
POST /api/responseprocessing/format
POST /api/responseprocessing/format/json?prettyPrint=true
POST /api/responseprocessing/format/xml?prettyPrint=true
POST /api/responseprocessing/format/yaml
POST /api/responseprocessing/format/markdown-table
```

**Example: Format as YAML**
```bash
curl -X POST https://localhost:5001/api/responseprocessing/format/yaml \
  -H "Content-Type: application/json" \
  -d '{"name": "John", "age": 30}'
```

### Rate Limiting Endpoints

```http
POST /api/responseprocessing/ratelimit/check
GET /api/responseprocessing/ratelimit/info/{key}
POST /api/responseprocessing/ratelimit/reset/{key}
GET /api/responseprocessing/ratelimit/all
```

**Example: Check Rate Limit**
```bash
curl -X POST https://localhost:5001/api/responseprocessing/ratelimit/check \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "endpoint": "/api/process",
    "weight": 1
  }'
```

### Error Handling Endpoints

```http
GET /api/responseprocessing/errors/recent?count=10
POST /api/responseprocessing/errors/test-retry?failureCount=2
```

### Utility Endpoints

```http
GET /api/responseprocessing/health
GET /api/responseprocessing/stats
```

## Configuration

### appsettings.json

```json
{
  "ResponseProcessing": {
    "DefaultParsingOptions": {
      "StrictMode": false,
      "ExtractCodeBlocks": true,
      "ExtractLinks": true,
      "ExtractTables": true,
      "TrimWhitespace": true
    },
    "RateLimiting": {
      "Strategy": "SlidingWindow",
      "MaxRequests": 100,
      "TimeWindow": "00:01:00",
      "EnablePerUser": true
    },
    "ErrorHandling": {
      "Strategy": "Log",
      "MaxRetries": 3,
      "RetryDelayMs": 1000,
      "EnableCircuitBreaker": true
    },
    "Sanitization": {
      "RemoveHtmlTags": true,
      "RemoveScripts": true,
      "RemoveSqlInjection": true,
      "RemoveXss": true
    }
  }
}
```

## Usage Patterns

### Pattern 1: Complete Response Processing Pipeline

```csharp
public async Task<ProcessedResponse> ProcessAIResponse(string rawResponse)
{
    // 1. Parse the response
    var parseResult = await _parser.ParseJsonAsync(rawResponse);
    if (!parseResult.Success)
    {
        _logger.LogError("Parsing failed: {Errors}", parseResult.Errors);
        return null;
    }

    // 2. Validate the data
    var validationRequest = new ValidationRequest
    {
        Data = parseResult.Data,
        Rules = GetValidationRules()
    };

    var validationResult = await _validation.ValidateAsync(validationRequest);
    if (!validationResult.IsValid)
    {
        _logger.LogWarning("Validation failed: {Errors}", validationResult.Errors);
    }

    // 3. Sanitize content
    var sanitized = await _validation.SanitizeAsync(new SanitizationRequest
    {
        Content = rawResponse,
        Options = new SanitizationOptions
        {
            RemoveXss = true,
            RemoveScripts = true
        }
    });

    // 4. Format for output
    var formatted = await _formatting.FormatAsJsonAsync(
        parseResult.Data,
        new FormattingOptions { PrettyPrint = true }
    );

    return new ProcessedResponse
    {
        Data = parseResult.Data,
        Formatted = formatted,
        IsValid = validationResult.IsValid,
        Errors = validationResult.Errors
    };
}
```

### Pattern 2: Resilient API Call with Rate Limiting

```csharp
public async Task<T> CallAPIWithRateLimitAndRetry<T>(
    Func<Task<T>> apiCall,
    string userId)
{
    // Check rate limit
    var rateLimitResult = await _rateLimiting.CheckRateLimitAsync(
        new RateLimitRequest
        {
            UserId = userId,
            Endpoint = "api/process"
        }
    );

    if (!rateLimitResult.Allowed)
    {
        throw new RateLimitException(
            $"Rate limit exceeded. Retry after {rateLimitResult.RetryAfter}"
        );
    }

    // Execute with retry policy
    var retryPolicy = new RetryPolicy
    {
        MaxAttempts = 3,
        InitialDelay = TimeSpan.FromSeconds(1),
        BackoffMultiplier = 2.0
    };

    var result = await _errorHandling.ExecuteWithRetryAsync(apiCall, retryPolicy);

    if (!result.Success)
    {
        _logger.LogError("API call failed after {Attempts} attempts",
            result.AttemptsMade);
        throw result.LastException!;
    }

    return result.Result!;
}
```

### Pattern 3: Multi-Format Response Handler

```csharp
public async Task<string> FormatResponseForClient(
    object data,
    string acceptHeader)
{
    var targetFormat = acceptHeader switch
    {
        "application/json" => ResponseFormat.Json,
        "application/xml" => ResponseFormat.Xml,
        "application/x-yaml" => ResponseFormat.Yaml,
        "text/csv" => ResponseFormat.Csv,
        _ => ResponseFormat.Json
    };

    var formattingRequest = new FormattingRequest
    {
        Data = data,
        TargetFormat = targetFormat,
        Options = new FormattingOptions
        {
            PrettyPrint = true,
            IncludeMetadata = true
        }
    };

    var result = await _formatting.FormatAsync(formattingRequest);
    return result.Content;
}
```

## Best Practices

### 1. Parsing
- **Always validate format** before parsing
- **Use strict mode** for production environments
- **Extract metadata** for logging and analytics
- **Handle parsing errors** gracefully

### 2. Validation
- **Define clear validation rules** upfront
- **Use appropriate severity levels** (Error vs Warning)
- **Sanitize user input** to prevent injection attacks
- **Log validation failures** for monitoring

### 3. Formatting
- **Cache formatted responses** when possible
- **Set appropriate Content-Type headers**
- **Use pretty printing** for debugging only
- **Consider output size** for large datasets

### 4. Error Handling
- **Use exponential backoff** for retries
- **Set maximum retry limits** to prevent infinite loops
- **Log all errors** with correlation IDs
- **Return meaningful error messages** to clients

### 5. Rate Limiting
- **Choose appropriate strategy** for your use case:
  - **FixedWindow**: Simple, predictable
  - **SlidingWindow**: Smooth, fair
  - **TokenBucket**: Handles bursts well
- **Set reasonable limits** based on capacity
- **Include retry-after headers** in responses
- **Monitor rate limit metrics**

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
# Test parsing
curl -X POST https://localhost:5001/api/responseprocessing/parse/json \
  -H "Content-Type: application/json" \
  -d '{"content":"{\"test\":\"data\"}", "format":"Json"}'

# Test validation
curl -X POST https://localhost:5001/api/responseprocessing/validate \
  -H "Content-Type: application/json" \
  -d '{"data":{"email":"test@example.com"},"rules":[{"fieldPath":"email","ruleType":"Email"}]}'

# Test rate limiting
curl -X POST https://localhost:5001/api/responseprocessing/ratelimit/check \
  -H "Content-Type: application/json" \
  -d '{"userId":"testuser"}'

# Get health status
curl https://localhost:5001/api/responseprocessing/health
```

## Troubleshooting

### Parsing Failures
```csharp
// Enable debug logging
var options = new ParsingOptions
{
    StrictMode = true // Will throw detailed errors
};

try
{
    var result = await _parser.ParseJsonAsync(content, options);
}
catch (JsonException ex)
{
    Console.WriteLine($"Line: {ex.LineNumber}, Position: {ex.LinePosition}");
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Rate Limit Issues
```csharp
// Check current status
var info = await _rateLimiting.GetRateLimitInfoAsync("user_123");
Console.WriteLine($"Remaining: {info.Remaining}/{info.Limit}");

// Reset if needed
if (info.Remaining == 0)
{
    await _rateLimiting.ResetRateLimitAsync("user_123");
}
```

### Validation Errors
```csharp
var result = await _validation.ValidateAsync(request);

if (!result.IsValid)
{
    // Group errors by field
    var errorsByField = result.Errors.GroupBy(e => e.Field);
    
    foreach (var group in errorsByField)
    {
        Console.WriteLine($"{group.Key}:");
        foreach (var error in group)
        {
            Console.WriteLine($"  - {error.Message}");
        }
    }
}
```

## Summary

Module 06 provides comprehensive response processing:

✅ **5 Parsing Formats** - JSON, XML, YAML, Markdown, CSV  
✅ **Code/Link/Table Extraction** - Automatic element extraction  
✅ **8 Validation Rules** - Required, length, pattern, range, email, URL  
✅ **Content Sanitization** - XSS, SQL injection, HTML tag removal  
✅ **5 Output Formats** - JSON, XML, YAML, CSV, Markdown tables  
✅ **5 Rate Limit Strategies** - Fixed, sliding, token bucket, leaky bucket, adaptive  
✅ **Error Handling** - Retry policies, circuit breakers, exponential backoff  
✅ **25+ API Endpoints** - Complete REST API for all features  
✅ **Production Ready** - Logging, caching, correlation IDs, health checks  

This module enables robust processing of AI responses with parsing, validation, formatting, and protection mechanisms!
