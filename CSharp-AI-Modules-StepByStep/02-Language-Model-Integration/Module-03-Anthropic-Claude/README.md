# Anthropic Claude Integration Module

## Overview

This module provides comprehensive integration with Anthropic's Claude language models, featuring Constitutional AI principles, streaming responses, long context processing, and advanced token management.

## Features

- **Complete Claude API Integration**: Support for all Claude models (Opus, Sonnet, Haiku)
- **Constitutional AI**: Built-in validation and improvement using Constitutional AI principles
- **Streaming Responses**: Real-time streaming chat completions
- **Long Context Processing**: Intelligent document chunking and synthesis for large documents
- **Token Management**: Accurate token counting, cost estimation, and context limit validation
- **Rate Limiting**: Configurable rate limiting for API protection
- **Comprehensive Validation**: FluentValidation for request validation
- **Error Handling**: Robust error handling with retry policies and circuit breakers
- **Swagger Documentation**: Complete API documentation with examples
- **Health Monitoring**: Health checks and application monitoring

## Project Structure

```
Module-03-Anthropic-Claude/
├── Controllers/
│   └── ClaudeController.cs          # Main API controller
├── Extensions/
│   ├── ServiceCollectionExtensions.cs # DI and service configuration
│   └── ValidatorExtensions.cs       # Request validators
├── Models/
│   └── ClaudeModels.cs             # Data models and DTOs
├── Services/
│   ├── ClaudeService.cs            # Main Claude API service
│   ├── ConstitutionalAIService.cs  # Constitutional AI validation
│   └── TokenService.cs             # Token counting and cost estimation
├── AI.Anthropic.Claude.csproj      # Project file
├── Program.cs                      # Application entry point
├── appsettings.json               # Configuration
├── appsettings.Development.json   # Development configuration
└── appsettings.Production.json    # Production configuration
```

## Getting Started

### Prerequisites

- .NET 8.0 or later
- Anthropic API key
- Visual Studio 2022 or VS Code

### Configuration

1. **API Key Setup**
   
   Update `appsettings.json` with your Anthropic API key:
   ```json
   {
     "Anthropic": {
       "ApiKey": "your-anthropic-api-key-here",
       "BaseUrl": "https://api.anthropic.com/v1",
       "Version": "2023-06-01"
     }
   }
   ```

2. **Environment Variables** (Recommended for production)
   ```bash
   export ANTHROPIC_API_KEY="your-api-key"
   ```

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access Swagger UI**:
   - Development: https://localhost:5001/swagger
   - API Documentation: https://localhost:5001

## API Endpoints

### Chat Completions

- **POST** `/api/claude/chat` - Generate chat completion
- **POST** `/api/claude/chat/stream` - Streaming chat completion

### Long Context Processing

- **POST** `/api/claude/long-context` - Process large documents with intelligent chunking

### Constitutional AI

- **POST** `/api/claude/constitutional/validate` - Validate content against Constitutional AI principles
- **POST** `/api/claude/constitutional/improve` - Apply Constitutional AI feedback
- **POST** `/api/claude/constitutional/critique` - Generate Constitutional AI critique

### Token Management

- **POST** `/api/claude/tokens/estimate` - Estimate token count and cost

### System Information

- **GET** `/api/claude/models` - List available models
- **GET** `/api/claude/health` - Health check
- **GET** `/health` - Application health status

## Usage Examples

### Basic Chat Completion

```csharp
var request = new ClaudeRequest
{
    Model = "claude-3-sonnet-20240229",
    Messages = new List<ClaudeMessage>
    {
        new() { Role = "user", Content = "Explain quantum computing in simple terms." }
    },
    MaxTokens = 1000,
    Temperature = 0.7
};

var response = await claudeService.ChatCompletionAsync(request);
```

### Streaming Chat

```csharp
var streamRequest = new ClaudeStreamRequest
{
    Model = "claude-3-sonnet-20240229",
    Messages = new List<ClaudeMessage>
    {
        new() { Role = "user", Content = "Write a story about AI." }
    },
    MaxTokens = 2000,
    Stream = true
};

await foreach (var chunk in claudeService.ChatCompletionStreamAsync(streamRequest))
{
    Console.Write(chunk.Delta?.Text);
}
```

### Constitutional AI Validation

```csharp
var principles = new List<string>
{
    "Be helpful and provide accurate information",
    "Be harmless and avoid content that could cause harm",
    "Be honest and acknowledge uncertainty when appropriate"
};

var validation = await constitutionalService.ValidateAsync(content, principles);
if (!validation.IsValid)
{
    var improvedContent = await constitutionalService.ApplyConstitutionalFeedbackAsync(content, principles);
}
```

### Long Context Processing

```csharp
var longContextRequest = new LongContextRequest
{
    Document = largeDocument,
    Query = "Summarize the key findings and recommendations",
    Model = "claude-3-sonnet-20240229",
    ChunkOverlap = 100
};

var response = await claudeService.ProcessLongContextAsync(longContextRequest);
```

## Configuration Options

### Anthropic Settings

```json
{
  "Anthropic": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.anthropic.com/v1",
    "Version": "2023-06-01",
    "TimeoutSeconds": 300,
    "DefaultModel": "claude-3-sonnet-20240229",
    "MaxRetries": 3,
    "RetryDelayMs": 1000
  }
}
```

### Rate Limiting

```json
{
  "RateLimit": {
    "Claude": {
      "PermitLimit": 100,
      "WindowMinutes": 1,
      "QueueLimit": 50
    },
    "ClaudeStream": {
      "PermitLimit": 20,
      "WindowMinutes": 1,
      "QueueLimit": 10
    }
  }
}
```

### Constitutional AI

```json
{
  "ConstitutionalAI": {
    "DefaultPrinciples": [
      "Be helpful and provide accurate information",
      "Be harmless and avoid content that could cause harm",
      "Be honest and acknowledge uncertainty when appropriate"
    ],
    "ValidationThreshold": 0.7,
    "AutoImprovement": true
  }
}
```

## Model Support

### Supported Models

- **Claude 3 Opus** (`claude-3-opus-20240229`) - Most capable, best for complex reasoning
- **Claude 3 Sonnet** (`claude-3-sonnet-20240229`) - Balanced performance and speed
- **Claude 3 Haiku** (`claude-3-haiku-20240307`) - Fastest, most cost-effective
- **Claude 2.1** (`claude-2.1`) - Previous generation, long context
- **Claude 2.0** (`claude-2.0`) - Previous generation
- **Claude Instant** (`claude-instant-1.2`) - Fast, cost-effective

### Model Capabilities

| Model | Context Length | Max Output | Input Cost (per 1K tokens) | Output Cost (per 1K tokens) |
|-------|----------------|------------|----------------------------|------------------------------|
| Claude 3 Opus | 200,000 | 4,096 | $0.015 | $0.075 |
| Claude 3 Sonnet | 200,000 | 4,096 | $0.003 | $0.015 |
| Claude 3 Haiku | 200,000 | 4,096 | $0.00025 | $0.00125 |

## Constitutional AI Principles

The module includes default Constitutional AI principles:

1. **Helpfulness**: Provide accurate and useful information
2. **Harmlessness**: Avoid content that could cause harm
3. **Honesty**: Acknowledge uncertainty and limitations
4. **Autonomy**: Respect human decision-making
5. **Transparency**: Provide clear reasoning
6. **Ethics**: Avoid illegal or unethical content
7. **Privacy**: Respect confidentiality
8. **Inclusion**: Use inclusive language
9. **Balance**: Present multiple perspectives
10. **Critical Thinking**: Encourage informed decisions

## Error Handling

The module includes comprehensive error handling:

- **Validation Errors**: Detailed validation messages using FluentValidation
- **Rate Limiting**: Automatic retry with exponential backoff
- **Circuit Breaker**: Prevents cascade failures
- **Timeout Handling**: Configurable request timeouts
- **Logging**: Structured logging with Serilog support

## Security Features

- **API Key Management**: Secure API key handling
- **Rate Limiting**: Protection against abuse
- **Input Validation**: Comprehensive request validation
- **Security Headers**: Standard security headers
- **CORS Configuration**: Configurable CORS policies
- **HTTPS Enforcement**: Secure transport

## Monitoring and Observability

- **Health Checks**: Application and dependency health monitoring
- **Logging**: Structured logging with correlation IDs
- **Metrics**: Performance and usage metrics
- **Tracing**: Request tracing support
- **Application Insights**: Azure Application Insights integration

## Testing

### Unit Tests

```bash
dotnet test
```

### Integration Tests

```bash
dotnet test --filter Category=Integration
```

### Performance Tests

```bash
dotnet test --filter Category=Performance
```

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "AI.Anthropic.Claude.dll"]
```

### Azure App Service

Configure the following application settings:
- `ANTHROPIC_API_KEY`: Your Anthropic API key
- `ASPNETCORE_ENVIRONMENT`: Production

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: claude-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: claude-api
  template:
    metadata:
      labels:
        app: claude-api
    spec:
      containers:
      - name: claude-api
        image: your-registry/claude-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ANTHROPIC_API_KEY
          valueFrom:
            secretKeyRef:
              name: claude-secrets
              key: api-key
```

## Best Practices

### Performance

1. **Caching**: Enable token estimate and validation result caching
2. **Connection Pooling**: Configure HTTP client connection pooling
3. **Async Operations**: Use async/await throughout
4. **Resource Management**: Proper disposal of resources

### Security

1. **API Key Rotation**: Regular API key rotation
2. **Rate Limiting**: Implement appropriate rate limits
3. **Input Validation**: Validate all inputs
4. **Logging**: Avoid logging sensitive data

### Reliability

1. **Retry Logic**: Implement exponential backoff
2. **Circuit Breaker**: Prevent cascade failures
3. **Health Checks**: Monitor dependencies
4. **Graceful Shutdown**: Handle shutdown signals

## Troubleshooting

### Common Issues

1. **API Key Issues**
   - Verify API key is correct
   - Check environment variable configuration
   - Ensure proper permissions

2. **Rate Limiting**
   - Review rate limit configuration
   - Implement proper backoff strategies
   - Monitor usage patterns

3. **Context Limits**
   - Use token estimation before requests
   - Implement long context processing
   - Split large inputs appropriately

4. **Performance Issues**
   - Enable caching
   - Optimize chunk sizes
   - Use appropriate models for tasks

### Debugging

Enable detailed logging in development:

```json
{
  "Logging": {
    "LogLevel": {
      "AI.Anthropic": "Trace"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:

- GitHub Issues: [Project Issues](https://github.com/your-org/claude-integration/issues)
- Documentation: [API Documentation](https://your-api-docs.com)
- Anthropic Support: [Anthropic Support](https://support.anthropic.com)

## Changelog

### Version 1.0.0
- Initial release
- Complete Claude API integration
- Constitutional AI support
- Streaming responses
- Long context processing
- Token management
- Comprehensive documentation