# Data Formats Processing Module

## Overview

The **Data Formats Processing Module** provides comprehensive functionality for processing, validating, querying, transforming, and converting various data formats commonly used in AI and data engineering pipelines.

### Supported Formats

- **JSON**: Parse, validate (JSON Schema), query (JSONPath), transform, serialize/deserialize
- **CSV**: Parse, write, validate, transform, delimiter auto-detection, metadata extraction
- **Excel**: Read (.xlsx/.xls), write, validate, metadata, transform, CSV export
- **Parquet**: Read, write, schema management, validate, columnar queries
- **XML**: Parse, validate (XSD), query (XPath), transform (XSLT), serialize/deserialize
- **HTML**: Parse, query (CSS selectors), extract data, validate, sanitize
- **Conversions**: 15+ conversion paths between formats with metadata preservation

---

## Architecture

```
AI.DataFormats/
├── Models/
│   └── DataFormatModels.cs          # Comprehensive models for all formats
├── Services/
│   ├── JsonService.cs               # JSON processing
│   ├── CsvService.cs                # CSV processing
│   ├── ExcelService.cs              # Excel processing
│   ├── ParquetService.cs            # Parquet processing
│   ├── XmlHtmlService.cs            # XML and HTML processing
│   └── FormatConversionService.cs   # Universal format conversion
├── Controllers/
│   └── DataFormatController.cs      # REST API endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

---

## Quick Start

### 1. Installation

```bash
dotnet add package Newtonsoft.Json
dotnet add package CsvHelper
dotnet add package ClosedXML
dotnet add package ExcelDataReader
dotnet add package Parquet.NET
dotnet add package HtmlAgilityPack
dotnet add package MessagePack
dotnet add package protobuf-net
dotnet add package Serilog.AspNetCore
```

### 2. Configuration

Add to `appsettings.json`:

```json
{
  "FormatSettings": {
    "MaxFileSizeBytes": 104857600,
    "DefaultEncoding": "UTF-8"
  },
  "JsonSettings": {
    "MaxDepth": 100,
    "SchemaValidationEnabled": true
  },
  "CsvSettings": {
    "DefaultDelimiter": ",",
    "AutoDetectDelimiter": true
  }
}
```

### 3. Register Services

```csharp
builder.Services.AddDataFormatServices();
```

### 4. Run the Application

```bash
dotnet run
```

Access Swagger UI: `https://localhost:5001/swagger`

---

## JSON Processing

### Parse JSON

**Endpoint**: `POST /api/dataformat/json/parse`

```csharp
var json = @"{
    ""name"": ""John Doe"",
    ""age"": 30,
    ""items"": [1, 2, 3]
}";

var result = await _jsonService.ParseJsonAsync(json);
// result.Metadata: { TokenCount: 10, MaxDepth: 2, ObjectCount: 1, ArrayCount: 1 }
```

### Validate JSON with Schema

**Endpoint**: `POST /api/dataformat/json/validate`

```csharp
var jsonSchema = @"{
    ""type"": ""object"",
    ""properties"": {
        ""name"": { ""type"": ""string"" },
        ""age"": { ""type"": ""number"", ""minimum"": 0 }
    },
    ""required"": [""name"", ""age""]
}";

var validationResult = await _jsonService.ValidateJsonAsync(json, jsonSchema);
// validationResult.IsValid: true/false
// validationResult.Errors: List of validation errors with line numbers
```

### Query JSON with JSONPath

```csharp
var query = "$.items[?(@.price > 10)]";
var results = await _jsonService.QueryJsonAsync(json, query);
```

### Transform JSON

```csharp
var template = @"{
    ""fullName"": ""{{name}}"",
    ""yearOfBirth"": {{currentYear - age}}
}";

var transformed = await _jsonService.TransformJsonAsync(json, template);
```

---

## CSV Processing

### Parse CSV

**Endpoint**: `POST /api/dataformat/csv/parse`

```csharp
var csvContent = @"Name,Age,City
John Doe,30,New York
Jane Smith,25,Los Angeles";

var options = new CsvOptions
{
    Delimiter = ",",
    HasHeaderRow = true,
    Encoding = Encoding.UTF8
};

var result = await _csvService.ParseCsvAsync(csvContent, options);
// result.Data: List<Dictionary<string, string>>
```

### Auto-Detect Delimiter

```csharp
var delimiter = await _csvService.DetectDelimiterAsync(csvContent);
// Automatically detects comma, semicolon, tab, or pipe
```

### Write CSV

**Endpoint**: `POST /api/dataformat/csv/write`

```csharp
var data = new List<Dictionary<string, object>>
{
    new() { ["Name"] = "John", ["Age"] = 30 },
    new() { ["Name"] = "Jane", ["Age"] = 25 }
};

var csvOutput = await _csvService.WriteCsvAsync(data, options);
```

### Validate CSV

```csharp
var validation = await _csvService.ValidateCsvAsync(csvContent, new List<string> { "Name", "Age", "City" });
// validation.IsValid, validation.RowConsistency, validation.DataTypes
```

---

## Excel Processing

### Read Excel File

**Endpoint**: `POST /api/dataformat/excel/read` (multipart/form-data)

```csharp
var options = new ExcelOptions
{
    SheetNames = new List<string> { "Sheet1", "Sheet2" },
    HasHeaderRow = true
};

var result = await _excelService.ReadExcelFileAsync("data.xlsx", options);
// result.Worksheets: List<ExcelWorksheetData>
// Each worksheet contains: Name, Rows (as Dictionary), Columns, RowCount, ColumnCount
```

### Write Excel File

**Endpoint**: `POST /api/dataformat/excel/write`

```csharp
var sheets = new Dictionary<string, List<Dictionary<string, object>>>
{
    ["Employees"] = new()
    {
        new() { ["Name"] = "John", ["Salary"] = 50000, ["HireDate"] = DateTime.Now }
    }
};

var options = new ExcelWriteOptions
{
    AutoFitColumns = true,
    DefaultDateFormat = "yyyy-MM-dd"
};

await _excelService.WriteExcelFileAsync("output.xlsx", sheets, options);
```

### Get Excel Metadata

```csharp
var metadata = await _excelService.GetExcelMetadataAsync("data.xlsx");
// metadata: WorksheetNames, RowCounts, ColumnCounts, FileSize, CellTypes, FormulaCount
```

---

## Parquet Processing

### Read Parquet File

**Endpoint**: `POST /api/dataformat/parquet/read` (multipart/form-data)

```csharp
var result = await _parquetService.ReadParquetFileAsync("data.parquet");
// result.Data: List<Dictionary<string, object>>
// result.Metadata: Schema, RowCount, ColumnCount, Compression
```

### Write Parquet File

**Endpoint**: `POST /api/dataformat/parquet/write`

```csharp
var schema = new List<ParquetSchemaField>
{
    new() { Name = "Id", DataType = "Int32", Nullable = false },
    new() { Name = "Name", DataType = "String", Nullable = true },
    new() { Name = "CreatedAt", DataType = "DateTime", Nullable = false }
};

var data = new List<Dictionary<string, object>>
{
    new() { ["Id"] = 1, ["Name"] = "John", ["CreatedAt"] = DateTime.Now }
};

var options = new ParquetOptions
{
    Schema = schema,
    Compression = "Snappy"
};

await _parquetService.WriteParquetFileAsync("output.parquet", data, options);
```

### Query Parquet

```csharp
var filtered = await _parquetService.QueryParquetAsync("data.parquet", "Age > 25");
```

---

## XML Processing

### Parse XML

**Endpoint**: `POST /api/dataformat/xml/parse`

```csharp
var xmlContent = @"<?xml version=""1.0""?>
<catalog>
    <book id=""1"">
        <title>XML Fundamentals</title>
        <price>29.99</price>
    </book>
</catalog>";

var result = await _xmlService.ParseXmlAsync(xmlContent);
```

### Validate XML with XSD Schema

```csharp
var xsdSchema = @"<?xml version=""1.0""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
    <xs:element name=""catalog"">
        <xs:complexType>
            <xs:sequence>
                <xs:element name=""book"" maxOccurs=""unbounded"">
                    <xs:complexType>
                        <xs:sequence>
                            <xs:element name=""title"" type=""xs:string""/>
                            <xs:element name=""price"" type=""xs:decimal""/>
                        </xs:sequence>
                    </xs:complexType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>";

var validation = await _xmlService.ValidateXmlAsync(xmlContent, xsdSchema);
```

### Query XML with XPath

```csharp
var query = "//book[@id='1']/title";
var results = await _xmlService.QueryXmlAsync(xmlContent, query);
```

### Transform XML with XSLT

```csharp
var xslt = @"<?xml version=""1.0""?>
<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
    <xsl:template match=""/"">
        <html>
            <body>
                <xsl:for-each select=""catalog/book"">
                    <p><xsl:value-of select=""title""/></p>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>";

var transformed = await _xmlService.TransformXmlAsync(xmlContent, xslt);
```

---

## HTML Processing

### Parse HTML

**Endpoint**: `POST /api/dataformat/html/parse`

```csharp
var htmlContent = @"<html>
    <body>
        <div class=""product"">
            <h2 class=""title"">Product Name</h2>
            <span class=""price"">$29.99</span>
        </div>
    </body>
</html>";

var result = await _htmlService.ParseHtmlAsync(htmlContent);
```

### Query HTML with CSS Selectors

```csharp
var selector = "div.product > h2.title";
var results = await _htmlService.QueryHtmlAsync(htmlContent, selector);
```

### Extract Tables from HTML

**Endpoint**: `POST /api/dataformat/html/extract-tables`

```csharp
var tables = await _htmlService.ExtractTablesAsync(htmlContent);
// Returns structured data from HTML tables as List<Dictionary<string, string>>
```

### Sanitize HTML

```csharp
var sanitized = await _htmlService.SanitizeHtmlAsync(htmlContent);
// Removes dangerous tags (script, iframe) and attributes (onclick, onerror)
```

---

## Format Conversion

### Universal Conversion

**Endpoint**: `POST /api/dataformat/convert`

```csharp
var request = new FormatConversionRequest
{
    SourceContent = jsonContent,
    SourceFormat = DataFormat.Json,
    TargetFormat = DataFormat.Csv,
    Options = new Dictionary<string, object>
    {
        ["PreserveMetadata"] = true,
        ["Validate"] = true
    }
};

var result = await _conversionService.ConvertAsync(request);
// result.ConvertedContent, result.Metadata, result.Warnings
```

### Supported Conversion Paths

| From    | To      | Example Use Case                          |
|---------|---------|-------------------------------------------|
| JSON    | CSV     | Export API responses to spreadsheet       |
| JSON    | Excel   | Create reports from JSON data             |
| JSON    | XML     | Legacy system integration                 |
| CSV     | JSON    | Import spreadsheet data to API            |
| CSV     | Excel   | Enhance CSV with formatting               |
| Excel   | CSV     | Export worksheet for processing           |
| Excel   | JSON    | Import Excel data to web application      |
| XML     | JSON    | Modernize XML APIs                        |
| XML     | CSV     | Export XML data to spreadsheet            |
| Parquet | JSON    | Query analytics data in web apps          |
| JSON    | Parquet | Store JSON data in columnar format        |
| HTML    | Text    | Extract plain text from web pages         |

### JSON to CSV Conversion

```csharp
var jsonArray = @"[
    { ""Name"": ""John"", ""Age"": 30, ""City"": ""New York"" },
    { ""Name"": ""Jane"", ""Age"": 25, ""City"": ""Los Angeles"" }
]";

var csvResult = await _conversionService.ConvertAsync(new FormatConversionRequest
{
    SourceContent = jsonArray,
    SourceFormat = DataFormat.Json,
    TargetFormat = DataFormat.Csv
});

// Output:
// Name,Age,City
// John,30,New York
// Jane,25,Los Angeles
```

### CSV to Excel Conversion

```csharp
var excelResult = await _conversionService.ConvertAsync(new FormatConversionRequest
{
    SourceContent = csvContent,
    SourceFormat = DataFormat.Csv,
    TargetFormat = DataFormat.Excel
});
```

### JSON to Parquet Conversion

```csharp
var parquetResult = await _conversionService.ConvertAsync(new FormatConversionRequest
{
    SourceContent = jsonArray,
    SourceFormat = DataFormat.Json,
    TargetFormat = DataFormat.Parquet
});
// Automatically infers schema and creates columnar storage
```

---

## API Endpoints Summary

### JSON Operations
- `POST /api/dataformat/json/parse` - Parse JSON
- `POST /api/dataformat/json/validate` - Validate with JSON Schema

### CSV Operations
- `POST /api/dataformat/csv/parse` - Parse CSV
- `POST /api/dataformat/csv/write` - Write CSV

### Excel Operations
- `POST /api/dataformat/excel/read` - Read Excel file
- `POST /api/dataformat/excel/write` - Write Excel file

### Parquet Operations
- `POST /api/dataformat/parquet/read` - Read Parquet file
- `POST /api/dataformat/parquet/write` - Write Parquet file

### XML Operations
- `POST /api/dataformat/xml/parse` - Parse XML

### HTML Operations
- `POST /api/dataformat/html/parse` - Parse HTML
- `POST /api/dataformat/html/extract-tables` - Extract tables

### Conversion Operations
- `POST /api/dataformat/convert` - Universal format conversion
- `POST /api/dataformat/detect-schema` - Detect schema from file

### Health Check
- `GET /api/dataformat/health` - Service health status

---

## Performance Tips

### JSON Processing
- Use `MinifyJsonAsync` to reduce payload size before transmission
- Enable caching for frequently accessed JSON schemas
- Use `System.Text.Json` for simple serialization (faster than Newtonsoft.Json)

### CSV Processing
- Enable `AutoDetectDelimiter` for unknown CSV formats
- Use streaming for large files (>100MB)
- Set `MaxRowsForDelimiterDetection` to balance accuracy vs performance

### Excel Processing
- Read only required worksheets using `SheetNames` option
- Enable `AutoFitColumns` only when creating user-facing reports
- Use ExcelDataReader for read-only operations (faster than ClosedXML)

### Parquet Processing
- Use Snappy compression for balanced performance
- Set appropriate `MaxRowGroupSize` (default: 5000 rows)
- Enable statistics for better query performance

### Format Conversion
- Set `Validate = false` for trusted data sources
- Use `PreserveMetadata = false` when metadata is not needed
- Cache conversion results for frequently converted data

---

## Error Handling

All services return `ApiResponseModel<T>` with consistent error handling:

```csharp
public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public List<string> Warnings { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Common Error Scenarios

**Invalid Format**
```json
{
  "success": false,
  "error": "Invalid JSON syntax at line 5, position 12",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Schema Validation Failure**
```json
{
  "success": false,
  "error": "Schema validation failed: Required property 'name' is missing",
  "warnings": ["Property 'age' has unexpected type"],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## Testing

### Unit Tests

```csharp
[Fact]
public async Task ParseJson_ValidJson_ReturnsSuccess()
{
    // Arrange
    var jsonService = new JsonService(logger);
    var json = @"{ ""name"": ""test"" }";

    // Act
    var result = await jsonService.ParseJsonAsync(json);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.Success);
}
```

### Integration Tests

```csharp
[Fact]
public async Task ConvertJsonToCsv_ValidData_ReturnsValidCsv()
{
    // Arrange
    var request = new FormatConversionRequest
    {
        SourceContent = "[{\"name\":\"John\",\"age\":30}]",
        SourceFormat = DataFormat.Json,
        TargetFormat = DataFormat.Csv
    };

    // Act
    var result = await _conversionService.ConvertAsync(request);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.Success);
    Assert.Contains("name,age", result.ConvertedContent);
}
```

---

## Troubleshooting

### Issue: "Maximum file size exceeded"
**Solution**: Increase `MaxFileSizeBytes` in `appsettings.json` or process file in chunks

### Issue: "Delimiter detection failed"
**Solution**: Manually specify delimiter in `CsvOptions` or increase `MaxRowsForDelimiterDetection`

### Issue: "Excel file is corrupt"
**Solution**: Ensure file is saved in .xlsx format (not .xls) or use ExcelDataReader for legacy formats

### Issue: "Parquet schema mismatch"
**Solution**: Verify data types match schema definition or enable auto-schema detection

### Issue: "XML validation fails with XSD"
**Solution**: Check namespace declarations and ensure XSD schema is well-formed

### Issue: "JSON Schema validation errors"
**Solution**: Use online JSON Schema validators to verify schema correctness

---

## Dependencies

- **Newtonsoft.Json** (13.0.3): JSON processing with advanced features
- **CsvHelper** (30.0.1): Robust CSV parsing and writing
- **ClosedXML** (0.102.1): Excel file creation
- **ExcelDataReader** (3.6.0): Excel file reading
- **Parquet.NET** (4.13.1): Apache Parquet support
- **HtmlAgilityPack** (1.11.54): HTML parsing and manipulation
- **MessagePack** (2.5.140): Binary serialization
- **protobuf-net** (3.2.26): Protocol Buffers support
- **Serilog** (8.0.0): Structured logging

---

## License

This module is part of the AI Learning Modules project.

---

## Support

For issues, questions, or contributions, please refer to the main project repository.
