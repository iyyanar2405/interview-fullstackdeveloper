# Module 02: Input Validation

Comprehensive input validation, sanitization, and security module for .NET 8.0 applications with protection against SQL injection, XSS, command injection, and other security threats.

## Features

### 🛡️ Input Validation
- **FluentValidation**: Declarative validation rules for complex objects
- **Data Annotations**: Attribute-based validation
- **Custom Validators**: User registration, comments, search, file uploads
- **Pattern Matching**: Email, phone, URL, password strength validation
- **Schema Validation**: JSON schema validation with NJsonSchema

### 🧹 Input Sanitization
- **HTML Sanitization**: Remove dangerous tags, scripts, and event handlers
- **SQL Sanitization**: Escape quotes, remove SQL keywords
- **XML/JSON Sanitization**: Encode special characters
- **Filename Sanitization**: Remove path traversal attempts
- **URL Sanitization**: Validate and normalize URLs

### 🔒 Security Protection
- **SQL Injection Detection**: Pattern-based detection with risk scoring
- **XSS Protection**: Script tag, event handler, and protocol detection
- **Command Injection Detection**: Shell command pattern detection
- **LDAP Injection Detection**: LDAP filter syntax validation
- **Path Traversal Detection**: Directory traversal attempt detection

### 📄 File Validation
- **Extension Validation**: Whitelist/blacklist file extensions
- **MIME Type Detection**: Detect actual MIME type from file content
- **File Signature Validation**: Magic number verification
- **Size Validation**: Maximum file size enforcement
- **Executable Detection**: Block executable files

## Project Structure

```
Module-02-Input-Validation/
├── Models/
│   └── ValidationModels.cs              # All validation models
├── Validators/
│   └── FluentValidators.cs              # FluentValidation validators
├── Services/
│   ├── SanitizationService.cs           # HTML, SQL, XML sanitization
│   ├── InjectionDetectionService.cs     # Multi-injection detection
│   ├── XssProtectionService.cs          # XSS detection and protection
│   ├── SchemaValidationService.cs       # JSON schema validation
│   └── FileValidationService.cs         # File validation and security
├── Controllers/
│   └── ValidationController.cs          # REST API endpoints
├── appsettings.json                     # Configuration
├── Program.cs                           # Application entry point
└── Module-02-Input-Validation.csproj
```

## Dependencies

- FluentValidation 11.9.0
- FluentValidation.AspNetCore 11.3.0
- HtmlSanitizer 8.0.865
- Ganss.Xss 5.2.0
- Newtonsoft.Json.Schema 3.0.15
- NJsonSchema 11.0.0
- MimeDetective 24.7.1
- MimeMapping 2.0.0
- Microsoft.Data.SqlClient 5.1.5
- Serilog.AspNetCore 8.0.0

## Installation

1. **Install NuGet packages:**
```bash
dotnet restore
```

2. **Configure settings in appsettings.json:**
```json
{
  "Validation": {
    "MaxInputLength": 10000,
    "MaxCollectionSize": 1000
  },
  "FileValidation": {
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".jpg", ".png", ".pdf"]
  }
}
```

3. **Run the application:**
```bash
dotnet run
```

## API Endpoints

### User Registration Validation

```http
POST /api/validation/user/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-01-01",
  "website": "https://example.com",
  "bio": "Software developer"
}
```

**Response:**
```json
{
  "isValid": true,
  "errors": [],
  "message": "Validation passed",
  "metadata": {}
}
```

### Comment Validation

```http
POST /api/validation/comment/validate
Content-Type: application/json

{
  "content": "Great article!",
  "authorName": "John Doe",
  "email": "john@example.com",
  "website": "https://johndoe.com"
}
```

### HTML Sanitization

```http
POST /api/validation/sanitize/html
Content-Type: application/json

{
  "content": "<p>Hello <script>alert('xss')</script></p>",
  "allowScripts": false,
  "allowStyles": true,
  "allowedTags": ["p", "strong", "em"]
}
```

**Response:**
```json
{
  "originalValue": "<p>Hello <script>alert('xss')</script></p>",
  "sanitizedValue": "<p>Hello </p>",
  "wasModified": true,
  "removedElements": ["1 script tag(s)"],
  "warnings": []
}
```

### SQL Injection Detection

```http
POST /api/validation/detect/sql-injection?input=SELECT * FROM users WHERE id=1 OR 1=1
```

**Response:**
```json
{
  "isSuspicious": true,
  "type": "SqlInjection",
  "detectedPatterns": [
    "\\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|DECLARE)\\b",
    "(\\bOR\\b|\\bAND\\b).*=.*"
  ],
  "riskScore": 7.0,
  "message": "Potential SQL injection detected",
  "sanitizedInput": "* FROM users WHERE id=1  1=1"
}
```

### XSS Detection

```http
POST /api/validation/detect/xss?input=<script>alert('xss')</script>
```

**Response:**
```json
{
  "isSuspicious": true,
  "type": "XssAttack",
  "detectedPatterns": [
    "<script[^>]*>.*?</script>"
  ],
  "riskScore": 8.5,
  "message": "Potential XSS attack detected",
  "sanitizedInput": "&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;"
}
```

### XSS Content Analysis

```http
POST /api/validation/xss/detect?input=<img src=x onerror=alert('xss')>
```

**Response:**
```json
{
  "containsXss": true,
  "detectedPatterns": [
    "OnError handler"
  ],
  "sanitizedContent": "",
  "riskLevel": 8,
  "vectors": [
    {
      "pattern": "OnError handler",
      "position": 11,
      "context": "...<img src=x onerror=alert('xss')>..."
    }
  ]
}
```

### Command Injection Detection

```http
POST /api/validation/detect/command-injection?input=test; rm -rf /
```

**Response:**
```json
{
  "isSuspicious": true,
  "type": "CommandInjection",
  "detectedPatterns": [
    "[;&|]\\s*(ls|cat|rm|mv|cp|wget|curl|nc|bash|sh|cmd|powershell)"
  ],
  "riskScore": 9.0,
  "message": "Potential command injection detected",
  "sanitizedInput": "test"
}
```

### Path Traversal Detection

```http
POST /api/validation/detect/path-traversal?input=../../etc/passwd
```

**Response:**
```json
{
  "isSuspicious": true,
  "type": "PathTraversal",
  "detectedPatterns": [
    "\\.\\.\/"
  ],
  "riskScore": 6.0,
  "message": "Potential path traversal detected",
  "sanitizedInput": "etcpasswd"
}
```

### JSON Schema Validation

```http
POST /api/validation/schema/validate
Content-Type: application/json

{
  "jsonData": "{\"name\":\"John\",\"age\":30}",
  "schema": "{\"type\":\"object\",\"properties\":{\"name\":{\"type\":\"string\"},\"age\":{\"type\":\"number\"}},\"required\":[\"name\"]}"
}
```

**Response:**
```json
{
  "isValid": true,
  "errors": [],
  "warnings": [],
  "schemaVersion": "draft-07"
}
```

### Generate JSON Schema

```http
POST /api/validation/schema/generate/userregistration
```

**Response:**
```json
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "object",
  "properties": {
    "username": { "type": "string" },
    "email": { "type": "string" },
    "password": { "type": "string" }
  },
  "required": ["username", "email", "password"]
}
```

### File Upload Validation

```http
POST /api/validation/file/validate
Content-Type: application/json

{
  "fileName": "document.pdf",
  "contentType": "application/pdf",
  "fileSize": 1024000,
  "fileContent": "base64_encoded_content",
  "description": "Important document"
}
```

**Response:**
```json
{
  "isValid": true,
  "fileName": "document.pdf",
  "detectedMimeType": "application/pdf",
  "declaredMimeType": "application/pdf",
  "mimeTypeMismatch": false,
  "fileSize": 1024000,
  "validationErrors": [],
  "fileProperties": {
    "SHA256": "abc123...",
    "Extension": ".pdf"
  }
}
```

### Detect File MIME Type

```http
POST /api/validation/file/detect-mime-type
Content-Type: application/json

{
  "fileName": "image.jpg",
  "fileContent": "base64_encoded_content"
}
```

### Calculate File Hash

```http
POST /api/validation/file/calculate-hash
Content-Type: application/json

{
  "fileName": "document.pdf",
  "fileContent": "base64_encoded_content"
}
```

**Response:**
```json
{
  "fileName": "document.pdf",
  "sha256Hash": "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
}
```

### Check if File is Executable

```http
GET /api/validation/file/is-executable?fileName=script.exe
```

**Response:**
```json
{
  "fileName": "script.exe",
  "isExecutable": true
}
```

### Get Common Validation Patterns

```http
GET /api/validation/patterns/common
```

**Response:**
```json
{
  "email": "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$",
  "phoneUS": "^\\+?1?\\d{10}$",
  "passwordStrong": "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
  "username": "^[a-zA-Z0-9_-]{3,16}$",
  "url": "^https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b"
}
```

### Validate Email Format

```http
POST /api/validation/validate/email?email=john@example.com
```

### Validate URL Format

```http
POST /api/validation/validate/url?url=https://example.com
```

## Usage Examples

### C# Client Example

```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://localhost:5001") };

// Validate user registration
var registrationRequest = new
{
    username = "john_doe",
    email = "john@example.com",
    password = "SecurePass123!",
    confirmPassword = "SecurePass123!",
    firstName = "John",
    lastName = "Doe"
};

var response = await client.PostAsJsonAsync("/api/validation/user/register", registrationRequest);
var result = await response.Content.ReadFromJsonAsync<ValidationResult>();

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}

// Detect SQL injection
var sqlInput = "SELECT * FROM users WHERE id=1 OR 1=1";
var sqlResponse = await client.PostAsync($"/api/validation/detect/sql-injection?input={sqlInput}", null);
var sqlResult = await sqlResponse.Content.ReadFromJsonAsync<InjectionDetectionResult>();

if (sqlResult.IsSuspicious)
{
    Console.WriteLine($"SQL Injection detected! Risk score: {sqlResult.RiskScore}");
    Console.WriteLine($"Sanitized: {sqlResult.SanitizedInput}");
}

// Sanitize HTML
var htmlRequest = new
{
    content = "<p>Hello <script>alert('xss')</script></p>",
    allowScripts = false
};

var htmlResponse = await client.PostAsJsonAsync("/api/validation/sanitize/html", htmlRequest);
var htmlResult = await htmlResponse.Content.ReadFromJsonAsync<SanitizationResult>();

Console.WriteLine($"Original: {htmlResult.OriginalValue}");
Console.WriteLine($"Sanitized: {htmlResult.SanitizedValue}");
```

### JavaScript/TypeScript Example

```typescript
// Validate user registration
const registrationData = {
  username: 'john_doe',
  email: 'john@example.com',
  password: 'SecurePass123!',
  confirmPassword: 'SecurePass123!',
  firstName: 'John',
  lastName: 'Doe'
};

const response = await fetch('https://localhost:5001/api/validation/user/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(registrationData)
});

const result = await response.json();

if (!result.isValid) {
  result.errors.forEach(error => {
    console.log(`${error.propertyName}: ${error.errorMessage}`);
  });
}

// Detect XSS
const xssInput = '<script>alert("xss")</script>';
const xssResponse = await fetch(
  `https://localhost:5001/api/validation/detect/xss?input=${encodeURIComponent(xssInput)}`,
  { method: 'POST' }
);

const xssResult = await xssResponse.json();

if (xssResult.isSuspicious) {
  console.log(`XSS detected! Risk score: ${xssResult.riskScore}`);
}

// Sanitize HTML
const htmlData = {
  content: '<p>Hello <script>alert("xss")</script></p>',
  allowScripts: false
};

const htmlResponse = await fetch('https://localhost:5001/api/validation/sanitize/html', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(htmlData)
});

const htmlResult = await htmlResponse.json();
console.log(`Sanitized: ${htmlResult.sanitizedValue}`);
```

## Security Best Practices

### 1. Always Validate and Sanitize
```csharp
// Bad - No validation
var userInput = Request.Query["search"];
var sql = $"SELECT * FROM products WHERE name LIKE '%{userInput}%'";

// Good - Validate and use parameterized queries
if (_injectionDetectionService.DetectSqlInjection(userInput).IsSuspicious)
{
    return BadRequest("Invalid input detected");
}
var sanitized = _sanitizationService.SanitizeForSql(userInput);
var sql = "SELECT * FROM products WHERE name LIKE @search";
```

### 2. Layer Security Defenses
```csharp
// Multiple layers of protection
var input = Request.Form["comment"];

// 1. Validate
var validationResult = await _commentValidator.ValidateAsync(new CommentRequest { Content = input });
if (!validationResult.IsValid) return BadRequest();

// 2. Detect injections
var injectionResult = _injectionDetectionService.DetectAllInjections(input);
if (injectionResult.IsSuspicious) return BadRequest();

// 3. Sanitize
var sanitized = _sanitizationService.SanitizeHtml(input);

// 4. Save sanitized content
await SaveComment(sanitized.SanitizedValue);
```

### 3. File Upload Security
```csharp
// Comprehensive file validation
var rules = new FileValidationRules
{
    AllowedExtensions = new List<string> { ".jpg", ".png", ".pdf" },
    MaxFileSizeBytes = 5 * 1024 * 1024, // 5 MB
    CheckMimeType = true,
    CheckFileSignature = true,
    AllowExecutables = false
};

var result = _fileValidationService.ValidateFile(
    fileBytes, 
    fileName, 
    contentType, 
    rules
);

if (!result.IsValid)
{
    return BadRequest(result.ValidationErrors);
}

// Store file with validated properties
await StoreFile(fileBytes, result.DetectedMimeType);
```

### 4. Context-Specific Encoding
```csharp
// HTML context
var htmlSafe = _sanitizationService.EncodeHtml(userInput);

// JavaScript context
var jsSafe = _sanitizationService.EncodeJavaScript(userInput);

// CSS context
var cssSafe = _sanitizationService.EncodeCss(userInput);

// URL context
var urlSafe = _sanitizationService.SanitizeUrl(userInput);
```

## Configuration Guide

### Validation Configuration

```json
{
  "Validation": {
    "EnableAutoValidation": true,
    "StrictMode": false,
    "LogValidationErrors": true,
    "MaxInputLength": 10000,
    "MaxCollectionSize": 1000
  }
}
```

### Sanitization Configuration

```json
{
  "Sanitization": {
    "Html": {
      "AllowedTags": ["p", "br", "strong"],
      "RemoveScripts": true,
      "RemoveStyles": false
    },
    "Sql": {
      "RemoveSqlKeywords": true,
      "EscapeQuotes": true
    }
  }
}
```

### File Validation Configuration

```json
{
  "FileValidation": {
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".jpg", ".png", ".pdf"],
    "CheckMimeType": true,
    "CheckFileSignature": true,
    "AllowExecutables": false
  }
}
```

## Common Validation Patterns

### Email Validation
```regex
^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$
```

### Strong Password
```regex
^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$
```

### Username
```regex
^[a-zA-Z0-9_-]{3,16}$
```

### URL
```regex
^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b
```

### Phone Number (International)
```regex
^\+?[1-9]\d{1,14}$
```

## Troubleshooting

### Common Issues

1. **Validation always fails**
   - Check input length against MaxInputLength
   - Verify pattern matches in FluentValidators
   - Review custom validation logic

2. **False positives in injection detection**
   - Adjust risk score thresholds
   - Whitelist known safe patterns
   - Use context-specific validators

3. **File validation fails**
   - Verify MIME type matches file extension
   - Check file signature (magic numbers)
   - Ensure file size is within limits

4. **HTML sanitization too aggressive**
   - Configure AllowedTags in SanitizationOptions
   - Add specific tags/attributes to whitelist
   - Use context-specific sanitization

## Testing

### Unit Test Example

```csharp
[Fact]
public async Task ValidateUserRegistration_WithValidData_ReturnsSuccess()
{
    // Arrange
    var validator = new UserRegistrationValidator();
    var request = new UserRegistrationRequest
    {
        Username = "john_doe",
        Email = "john@example.com",
        Password = "SecurePass123!",
        ConfirmPassword = "SecurePass123!",
        FirstName = "John",
        LastName = "Doe"
    };

    // Act
    var result = await validator.ValidateAsync(request);

    // Assert
    Assert.True(result.IsValid);
}

[Fact]
public void DetectSqlInjection_WithMaliciousInput_ReturnsTrue()
{
    // Arrange
    var service = new InjectionDetectionService(logger, sanitizationService);
    var input = "SELECT * FROM users WHERE id=1 OR 1=1";

    // Act
    var result = service.DetectSqlInjection(input);

    // Assert
    Assert.True(result.IsSuspicious);
    Assert.True(result.RiskScore > 5.0);
}
```

## Performance Considerations

- Use caching for compiled regex patterns
- Implement rate limiting for validation endpoints
- Stream large file uploads instead of loading into memory
- Use async validation for I/O operations
- Configure validation timeout thresholds

## License

This module is provided as-is for educational and commercial use.

## Version History

- **1.0.0** - Initial release
  - FluentValidation for complex objects
  - HTML/SQL/XML sanitization
  - SQL injection detection
  - XSS protection
  - Command injection detection
  - LDAP injection detection
  - Path traversal detection
  - JSON schema validation
  - File validation with MIME type detection
  - 40+ REST API endpoints

## Related Modules

- Module 01: Authentication & Authorization
- Module 03: Content Safety
- Module 04: Data Protection
- Module 05: Rate Limiting
