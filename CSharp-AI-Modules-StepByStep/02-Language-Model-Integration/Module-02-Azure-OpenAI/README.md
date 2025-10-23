# Module 02: Azure OpenAI Integration

Enterprise-grade Azure OpenAI Service integration with authentication, content filtering, and advanced features.

## Learning Objectives

- Set up Azure OpenAI Service
- Implement Azure AD authentication
- Use managed identity for security
- Handle content filtering
- Monitor usage and costs
- Implement deployment management
- Use Azure-specific features

## Project Structure

```
Module-02-Azure-OpenAI/
├── README.md
├── AI.AzureOpenAI.csproj
├── Program.cs
├── Services/
│   ├── IAzureOpenAIService.cs
│   ├── AzureOpenAIService.cs
│   ├── ITokenService.cs
│   ├── TokenService.cs
│   └── ContentFilterService.cs
├── Authentication/
│   ├── IAzureAuthService.cs
│   ├── AzureAuthService.cs
│   ├── ManagedIdentityService.cs
│   └── TokenCredentialProvider.cs
├── Models/
│   ├── AzureOpenAIModels.cs
│   ├── ContentFilterResult.cs
│   ├── UsageMetrics.cs
│   └── DeploymentInfo.cs
├── Configuration/
│   ├── AzureOpenAISettings.cs
│   └── ContentFilterSettings.cs
├── Extensions/
│   ├── AzureServiceExtensions.cs
│   └── AuthenticationExtensions.cs
└── Examples/
    ├── BasicAzureOpenAIExample.cs
    ├── ManagedIdentityExample.cs
    ├── ContentFilteringExample.cs
    └── UsageMonitoringExample.cs
```

## Key Features

### 1. **Authentication Methods**
- Azure AD authentication
- Managed identity support
- Service principal authentication
- API key authentication (fallback)

### 2. **Enterprise Features**
- Content filtering and safety
- Usage monitoring and billing
- Deployment management
- Regional deployment support

### 3. **Security & Compliance**
- Private endpoints
- VNet integration
- Audit logging
- Data residency compliance

## Dependencies

```xml
<PackageReference Include="Azure.AI.OpenAI" Version="1.0.0-beta.12" />
<PackageReference Include="Azure.Identity" Version="1.10.4" />
<PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.5.0" />
<PackageReference Include="Microsoft.Extensions.Azure" Version="1.7.1" />
```

## Getting Started

1. **Configure Azure OpenAI**:
   ```json
   {
     "AzureOpenAI": {
       "Endpoint": "https://your-resource.openai.azure.com/",
       "DeploymentName": "gpt-35-turbo",
       "ApiVersion": "2024-02-01",
       "UseManagedIdentity": true
     }
   }
   ```

2. **Use managed identity**:
   ```csharp
   var credential = new DefaultAzureCredential();
   var client = new OpenAIClient(endpoint, credential);
   ```

## Best Practices

- Always use managed identity in production
- Enable content filtering
- Monitor usage and costs
- Use private endpoints for security
- Implement proper error handling
- Cache responses when appropriate
- Rotate API keys regularly