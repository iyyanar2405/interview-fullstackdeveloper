# Module 01: API Framework

Learn to build robust Web APIs using ASP.NET Core for AI applications.

## Learning Objectives

- Set up ASP.NET Core Web API project
- Implement both Controller-based and Minimal APIs
- Configure middleware pipeline
- Handle HTTP requests/responses
- Implement proper error handling

## Project Structure

```
Module-01-API-Framework/
├── README.md
├── AI.Foundation.API.csproj
├── Program.cs
├── Controllers/
│   ├── AIController.cs
│   └── HealthController.cs
├── Models/
│   ├── APIRequest.cs
│   ├── APIResponse.cs
│   └── ErrorResponse.cs
├── Middleware/
│   ├── ErrorHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

## Getting Started

1. **Create the project**:
   ```bash
   dotnet new webapi -n AI.Foundation.API
   cd AI.Foundation.API
   ```

2. **Install required packages**:
   ```bash
   dotnet add package Microsoft.AspNetCore.OpenApi
   dotnet add package Swashbuckle.AspNetCore
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Test the API**:
   - Open browser to `https://localhost:7xxx/swagger`
   - Test endpoints using Swagger UI

## Key Concepts

### 1. **Controller-based APIs**
Traditional approach using controllers and actions with attribute routing.

### 2. **Minimal APIs**
Lightweight approach for simple APIs with less boilerplate code.

### 3. **Middleware Pipeline**
Custom middleware for cross-cutting concerns like logging and error handling.

### 4. **Dependency Injection**
Built-in DI container for managing service dependencies.

## Next Steps

After completing this module, move to [Module-02-Configuration-Management](../Module-02-Configuration-Management/) to learn about application configuration.