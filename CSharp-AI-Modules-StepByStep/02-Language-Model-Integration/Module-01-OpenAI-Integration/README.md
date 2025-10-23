# Module 01: OpenAI Integration

Complete integration with OpenAI API including chat completions, function calling, and streaming.

## Learning Objectives

- Set up OpenAI API client in C#
- Implement chat completions
- Handle function calling
- Process streaming responses
- Manage API quotas and rate limits
- Implement proper error handling

## Key Features

### 1. **Chat Completions**
- Single-turn conversations
- Multi-turn conversations with context
- System message configuration
- Temperature and parameter control

### 2. **Function Calling**
- Define function schemas
- Handle function execution
- Parallel function calls
- Error handling for functions

### 3. **Streaming Responses**
- Real-time response processing
- Chunk handling
- Connection management
- Error recovery

### 4. **Advanced Features**
- Token counting and management
- Cost tracking
- Response caching
- Retry mechanisms

## Project Structure

```
Module-01-OpenAI-Integration/
├── README.md
├── AI.OpenAI.Integration.csproj
├── Program.cs
├── Models/
│   ├── ChatModels.cs
│   ├── FunctionModels.cs
│   └── StreamingModels.cs
├── Services/
│   ├── OpenAIService.cs
│   ├── FunctionCallingService.cs
│   ├── StreamingService.cs
│   └── TokenCountingService.cs
├── Functions/
│   ├── WeatherFunction.cs
│   ├── CalculatorFunction.cs
│   └── DatabaseFunction.cs
├── Configuration/
│   └── OpenAISettings.cs
└── Examples/
    ├── BasicChatExample.cs
    ├── FunctionCallingExample.cs
    └── StreamingExample.cs
```

## Dependencies

```xml
<PackageReference Include="OpenAI" Version="1.10.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="System.Text.Json" Version="8.0.0" />
```

## Getting Started

1. **Install OpenAI package**: `dotnet add package OpenAI`
2. **Configure API key**: Set up secure API key management
3. **Initialize client**: Create OpenAI client with proper configuration
4. **Test basic chat**: Implement simple chat completion
5. **Add function calling**: Define and execute functions
6. **Implement streaming**: Handle real-time responses

## Example Usage

```csharp
// Basic chat completion
var response = await openAIService.ChatAsync("Hello, how are you?");

// Function calling
var weatherResult = await functionCallingService.GetWeatherAsync("New York");

// Streaming chat
await foreach (var chunk in streamingService.ChatStreamAsync("Tell me a story"))
{
    Console.Write(chunk.Content);
}
```

## Best Practices

- Always handle rate limits gracefully
- Implement proper token counting
- Use streaming for long responses
- Cache responses when appropriate
- Monitor API usage and costs
- Implement retry logic with exponential backoff