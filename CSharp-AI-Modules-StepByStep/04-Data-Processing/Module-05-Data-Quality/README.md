# Data Quality Module

## Overview

The **Data Quality Module** provides comprehensive data quality management capabilities including validation, profiling, anomaly detection, data cleansing, and governance for AI and data engineering pipelines.

### Key Features

- **Data Validation**: Rule-based validation with FluentValidation
- **Data Profiling**: Statistical analysis and metadata extraction
- **Anomaly Detection**: ML-powered outlier and pattern detection
- **Data Cleansing**: Automated data cleaning and standardization
- **Data Governance**: Policy management and compliance reporting

---

## Architecture

```
AI.DataQuality/
├── Models/
│   └── DataQualityModels.cs           # Comprehensive quality models
├── Services/
│   ├── DataValidationService.cs       # Validation and schema checking
│   ├── DataProfilingService.cs        # Statistical profiling
│   ├── AnomalyDetectionService.cs     # ML-based anomaly detection
│   └── DataCleansingService.cs        # Data cleansing and governance
├── Controllers/
│   └── DataQualityController.cs       # REST API endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

---

## Quick Start

### 1. Installation

```bash
dotnet add package FluentValidation
dotnet add package Microsoft.ML
dotnet add package MathNet.Numerics
dotnet add package RulesEngine
dotnet add package Serilog.AspNetCore
```

### 2. Configuration

```json
{
  "DataQuality": {
    "ValidationSettings": {
      "MaxRecordsPerBatch": 10000,
      "EnableParallelValidation": true
    },
    "AnomalyDetection": {
      "DefaultSensitivity": 0.95,
      "OutlierThreshold": 3.0
    }
  }
}
```

### 3. Register Services

```csharp
builder.Services.AddDataQualityServices();
```

### 4. Run the Application

```bash
dotnet run
```

Access Swagger UI: `https://localhost:5001/swagger`

---

## Data Validation

### Validate Data with Rules

**Endpoint**: `POST /api/dataquality/validate`

```csharp
var rules = new List<DataQualityRule>
{
    new()
    {
        RuleName = "EmailValidation",
        Description = "Validate email format",
        Dimension = DataQualityDimension.Accuracy,
        Severity = ValidationSeverity.Error,
        Expression = "email_pattern",
        ErrorMessage = "Invalid email format"
    },
    new()
    {
        RuleName = "AgeRange",
        Description = "Age must be between 0 and 120",
        Dimension = DataQualityDimension.Validity,
        Severity = ValidationSeverity.Error,
        Expression = "age >= 0 AND age <= 120",
        ErrorMessage = "Age must be between 0 and 120"
    }
};

var data = new List<Dictionary<string, object>>
{
    new() { ["email"] = "user@example.com", ["age"] = 25 },
    new() { ["email"] = "invalid-email", ["age"] = 150 }
};

var result = await _validationService.ValidateDataAsync(data, rules);
// result.IsValid: false
// result.Errors: List of validation errors with row numbers
```

**cURL Example**:
```bash
curl -X POST "https://localhost:5001/api/dataquality/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "data": [
      {"email": "user@example.com", "age": 25}
    ],
    "rules": [
      {
        "ruleName": "EmailValidation",
        "dimension": "Accuracy",
        "severity": "Error",
        "expression": "email_pattern",
        "errorMessage": "Invalid email format"
      }
    ]
  }'
```

### Schema Validation

**Endpoint**: `POST /api/dataquality/validate/schema`

```csharp
var schema = new SchemaDefinition
{
    SchemaName = "UserSchema",
    Version = "1.0",
    Fields = new List<FieldDefinition>
    {
        new()
        {
            FieldName = "userId",
            DataType = DataType.Integer,
            IsRequired = true,
            IsUnique = true
        },
        new()
        {
            FieldName = "email",
            DataType = DataType.String,
            IsRequired = true,
            Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            MaxLength = 100
        },
        new()
        {
            FieldName = "age",
            DataType = DataType.Integer,
            MinValue = 0,
            MaxValue = 120
        },
        new()
        {
            FieldName = "status",
            DataType = DataType.String,
            AllowedValues = new List<object> { "active", "inactive", "pending" }
        }
    },
    RequiredFields = new List<string> { "userId", "email" }
};

var result = await _validationService.ValidateSchemaAsync(data, schema);
// result.IsValid: true/false
// result.Violations: List of schema violations
```

### Get Standard Rules

**Endpoint**: `GET /api/dataquality/validate/rules/{datasetType}`

```csharp
var rules = await _validationService.GetStandardRulesAsync("user-data");
// Returns pre-defined rules for common validation scenarios
```

---

## Data Profiling

### Profile Dataset

**Endpoint**: `POST /api/dataquality/profile`

```csharp
var data = new List<Dictionary<string, object>>
{
    new() { ["name"] = "John", ["age"] = 30, ["salary"] = 50000 },
    new() { ["name"] = "Jane", ["age"] = 25, ["salary"] = 55000 },
    new() { ["name"] = "Bob", ["age"] = 35, ["salary"] = 60000 }
};

var profile = await _profilingService.ProfileDatasetAsync(data, "Employees");

// Profile contains:
// - TotalRecords: 3
// - TotalColumns: 3
// - Columns: List of ColumnProfile with detailed statistics
```

**Column Profile Example**:
```csharp
// For numeric column "age":
{
  "columnName": "age",
  "dataType": "Integer",
  "totalValues": 3,
  "nullCount": 0,
  "distinctCount": 3,
  "nullPercentage": 0,
  "uniquenessRatio": 1.0,
  "minValue": 25,
  "maxValue": 35,
  "mean": 30,
  "median": 30,
  "standardDeviation": 5.0,
  "sampleValues": ["30", "25", "35"]
}
```

### Calculate Quality Metrics

**Endpoint**: `POST /api/dataquality/metrics`

```csharp
var metrics = await _profilingService.CalculateQualityMetricsAsync(data, "Employees");

Console.WriteLine($"Overall Score: {metrics.OverallScore}%");
Console.WriteLine($"Completeness: {metrics.CompletenessPercentage}%");
Console.WriteLine($"Accuracy: {metrics.AccuracyPercentage}%");
Console.WriteLine($"Uniqueness: {metrics.UniquenessPercentage}%");

foreach (var issue in metrics.Issues)
{
    Console.WriteLine($"Issue: {issue}");
}
```

**Response Example**:
```json
{
  "datasetName": "Employees",
  "overallScore": 95.5,
  "dimensionScores": {
    "Completeness": 100,
    "Accuracy": 98,
    "Consistency": 100,
    "Uniqueness": 100,
    "Validity": 98
  },
  "totalRecords": 1000,
  "validRecords": 980,
  "invalidRecords": 20,
  "issues": [
    "Accuracy: 20 records have invalid data"
  ]
}
```

---

## Anomaly Detection

### Detect All Anomalies

**Endpoint**: `POST /api/dataquality/anomalies/detect`

```csharp
var request = new AnomalyDetectionRequest
{
    DatasetName = "SensorData",
    Data = sensorReadings,
    NumericColumns = new List<string> { "temperature", "humidity", "pressure" },
    Sensitivity = 0.95,
    WindowSize = 100
};

var result = await _anomalyService.DetectAnomaliesAsync(request);

Console.WriteLine($"Anomalies Detected: {result.AnomaliesDetected}");
Console.WriteLine($"Anomaly Percentage: {result.AnomalyPercentage}%");

foreach (var anomaly in result.Anomalies)
{
    Console.WriteLine($"Type: {anomaly.Type}");
    Console.WriteLine($"Row: {anomaly.RowNumber}");
    Console.WriteLine($"Column: {anomaly.ColumnName}");
    Console.WriteLine($"Value: {anomaly.Value}");
    Console.WriteLine($"Score: {anomaly.AnomalyScore}");
    Console.WriteLine($"Description: {anomaly.Description}");
}
```

### Detect Outliers

**Endpoint**: `POST /api/dataquality/anomalies/outliers`

```csharp
var values = new List<double> { 10, 12, 11, 13, 10, 12, 100, 11, 12 };

var outliers = await _anomalyService.DetectOutliersAsync(
    values,
    columnName: "temperature",
    threshold: 3.0); // Z-score threshold

// Outliers detected: [100] at index 6
```

**Anomaly Types**:
- **Outlier**: Statistical outliers using Z-score
- **Missing**: Patterns of missing data
- **Duplicate**: Duplicate records
- **Format**: Invalid formats
- **Range**: Values outside expected range
- **Statistical**: ML.NET time series anomalies
- **Pattern**: Unusual patterns in data

### Detect Duplicates

**Endpoint**: `POST /api/dataquality/anomalies/duplicates`

```csharp
var duplicates = await _anomalyService.DetectDuplicatesAsync(data);

foreach (var duplicate in duplicates)
{
    Console.WriteLine($"Duplicate found at row {duplicate.RowNumber}");
    Console.WriteLine($"Occurs {duplicate.AnomalyScore} times");
}
```

---

## Data Cleansing

### Comprehensive Data Cleansing

**Endpoint**: `POST /api/dataquality/cleanse`

```csharp
var cleansingRules = new List<CleansingRule>
{
    new()
    {
        ColumnName = "email",
        Operation = CleansingOperation.LowerCase
    },
    new()
    {
        ColumnName = "name",
        Operation = CleansingOperation.Trim
    },
    new()
    {
        ColumnName = "phone",
        Operation = CleansingOperation.RemoveSpecialCharacters
    },
    new()
    {
        ColumnName = "price",
        Operation = CleansingOperation.Round,
        Parameter = 2 // 2 decimal places
    }
};

var request = new DataCleansingRequest
{
    Data = dirtyData,
    Rules = cleansingRules,
    RemoveDuplicates = true,
    FillMissingValues = true,
    DefaultValues = new Dictionary<string, object>
    {
        ["status"] = "pending",
        ["country"] = "Unknown"
    }
};

var result = await _cleansingService.CleanseDataAsync(request);

Console.WriteLine($"Original: {result.OriginalRecordCount} records");
Console.WriteLine($"Cleaned: {result.CleanedRecordCount} records");
Console.WriteLine($"Removed: {result.RecordsRemoved} records");
Console.WriteLine($"Modified: {result.RecordsModified} records");

foreach (var action in result.ActionsApplied)
{
    Console.WriteLine($"Applied {action.Operation} to {action.ColumnName}: {action.RecordsAffected} records affected");
}
```

### Cleansing Operations

| Operation | Description | Example |
|-----------|-------------|---------|
| `Trim` | Remove leading/trailing whitespace | `" hello "` → `"hello"` |
| `UpperCase` | Convert to uppercase | `"hello"` → `"HELLO"` |
| `LowerCase` | Convert to lowercase | `"HELLO"` → `"hello"` |
| `RemoveSpecialCharacters` | Remove non-alphanumeric | `"hello@123"` → `"hello123"` |
| `ReplaceValue` | Replace with parameter | `null` → `"default"` |
| `FillNull` | Fill null with default | `null` → `0` |
| `FormatDate` | Standardize date format | Any date → `"2024-01-15"` |
| `Round` | Round numeric values | `3.14159` → `3.14` |
| `Standardize` | Standardize string format | `"  john DOE  "` → `"John doe"` |

### Remove Duplicates Only

**Endpoint**: `POST /api/dataquality/cleanse/duplicates`

```csharp
var uniqueData = await _cleansingService.RemoveDuplicatesAsync(data);
Console.WriteLine($"Removed {data.Count - uniqueData.Count} duplicates");
```

---

## Data Governance

### Check Compliance

**Endpoint**: `POST /api/dataquality/governance/check-compliance`

```csharp
var policy = new DataGovernancePolicy
{
    PolicyName = "GDPR Compliance",
    Description = "Ensure data meets GDPR requirements",
    Owner = "Data Governance Team",
    Rules = new List<DataQualityRule>
    {
        new()
        {
            RuleName = "PersonalDataEncryption",
            Description = "Personal data must be encrypted",
            Dimension = DataQualityDimension.Integrity,
            Severity = ValidationSeverity.Critical
        },
        new()
        {
            RuleName = "DataRetention",
            Description = "Data must not exceed retention period",
            Dimension = DataQualityDimension.Timeliness,
            Severity = ValidationSeverity.Error
        }
    }
};

var report = await _governanceService.CheckComplianceAsync(data, policy);

Console.WriteLine($"Compliance Score: {report.ComplianceScore}%");
Console.WriteLine($"Is Compliant: {report.IsCompliant}");

foreach (var violation in report.Violations)
{
    Console.WriteLine($"Violation: {violation.RuleName}");
    Console.WriteLine($"Severity: {violation.Severity}");
    Console.WriteLine($"Affected Records: {violation.AffectedRecords}");
}
```

### Create Policy

**Endpoint**: `POST /api/dataquality/governance/policies`

```csharp
var rules = new List<DataQualityRule>
{
    // Define rules
};

var policy = await _governanceService.CreatePolicyAsync(
    policyName: "Data Quality Standard",
    description: "Standard quality rules for all datasets",
    rules: rules);
```

### Get Active Policies

**Endpoint**: `GET /api/dataquality/governance/policies`

```csharp
var policies = await _governanceService.GetActivePoliciesAsync();

foreach (var policy in policies)
{
    Console.WriteLine($"Policy: {policy.PolicyName}");
    Console.WriteLine($"Rules: {policy.Rules.Count}");
    Console.WriteLine($"Effective Date: {policy.EffectiveDate}");
}
```

---

## Data Quality Dimensions

The module tracks **7 key dimensions** of data quality:

1. **Accuracy**: Correctness of data values
2. **Completeness**: Presence of required data
3. **Consistency**: Data uniformity across systems
4. **Timeliness**: Data currency and freshness
5. **Validity**: Data conformance to rules
6. **Uniqueness**: Absence of duplicates
7. **Integrity**: Referential and structural integrity

---

## Performance Tips

### Validation
- Batch validate large datasets (10,000 records per batch)
- Use parallel validation for independent rules
- Cache validation rules for reuse

### Profiling
- Use sampling for very large datasets
- Profile incrementally during data ingestion
- Cache profile results with expiration

### Anomaly Detection
- Adjust sensitivity based on data characteristics
- Use appropriate window sizes for time series
- Pre-filter data to reduce noise

### Cleansing
- Apply cleansing rules in optimal order
- Use batch operations for large datasets
- Test rules on sample data first

---

## Testing

### Unit Tests

```csharp
[Fact]
public async Task ValidateData_WithValidData_ReturnsSuccess()
{
    // Arrange
    var service = new DataValidationService(logger);
    var data = new List<Dictionary<string, object>>
    {
        new() { ["email"] = "test@example.com", ["age"] = 25 }
    };
    var rules = new List<DataQualityRule> { /* rules */ };

    // Act
    var result = await service.ValidateDataAsync(data, rules);

    // Assert
    Assert.True(result.IsValid);
}
```

### Integration Tests

```csharp
[Fact]
public async Task EndToEnd_DataQualityPipeline_ProcessesData()
{
    // Profile → Validate → Detect Anomalies → Cleanse
    var profile = await _profilingService.ProfileDatasetAsync(data, "Test");
    var validation = await _validationService.ValidateDataAsync(data, rules);
    var anomalies = await _anomalyService.DetectAnomaliesAsync(request);
    var cleaned = await _cleansingService.CleanseDataAsync(cleansingRequest);

    Assert.NotNull(cleaned.CleanedData);
}
```

---

## Troubleshooting

### Issue: "Validation timeout"
**Solution**: Reduce `MaxRecordsPerBatch` or enable parallel validation

### Issue: "Outlier detection too sensitive"
**Solution**: Increase `OutlierThreshold` from default 3.0 to 4.0 or higher

### Issue: "Profiling uses too much memory"
**Solution**: Use sampling by setting `SampleSize` in configuration

### Issue: "ML.NET anomaly detection errors"
**Solution**: Ensure minimum data points (10+) and valid numeric values

---

## Dependencies

- **FluentValidation** (11.9.0): Rule-based validation
- **Microsoft.ML** (3.0.1): Machine learning for anomaly detection
- **MathNet.Numerics** (5.0.0): Statistical calculations
- **RulesEngine** (5.0.3): Business rules engine
- **Dapper** (2.1.24): Database access
- **Serilog** (8.0.0): Structured logging
- **prometheus-net** (8.2.1): Metrics collection

---

## License

This module is part of the AI Learning Modules project.

---

## Support

For issues, questions, or contributions, please refer to the main project repository.
