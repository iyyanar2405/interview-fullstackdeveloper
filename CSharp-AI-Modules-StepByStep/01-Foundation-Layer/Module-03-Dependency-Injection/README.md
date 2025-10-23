# Module 03: Dependency Injection

Master dependency injection patterns and service lifetime management for scalable AI applications.

## Learning Objectives

- Understand DI container fundamentals
- Implement service registration patterns
- Manage service lifetimes effectively
- Create custom service providers
- Use factory patterns with DI
- Implement service decorators
- Handle circular dependencies

## Project Structure

```
Module-03-Dependency-Injection/
├── README.md
├── AI.DependencyInjection.csproj
├── Program.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IUserService.cs
│   │   ├── IEmailService.cs
│   │   ├── ILoggerService.cs
│   │   └── IDataService.cs
│   ├── Implementations/
│   │   ├── UserService.cs
│   │   ├── EmailService.cs
│   │   ├── ConsoleLoggerService.cs
│   │   ├── FileLoggerService.cs
│   │   └── DatabaseService.cs
│   └── Factories/
│       ├── IServiceFactory.cs
│       ├── LoggerServiceFactory.cs
│       └── DataServiceFactory.cs
├── Decorators/
│   ├── CachedDataServiceDecorator.cs
│   ├── LoggedEmailServiceDecorator.cs
│   └── RetryDataServiceDecorator.cs
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── DIExtensions.cs
├── Models/
│   ├── User.cs
│   ├── EmailMessage.cs
│   └── ServiceConfiguration.cs
└── Examples/
    ├── BasicDIExample.cs
    ├── LifetimeManagementExample.cs
    ├── FactoryPatternExample.cs
    └── DecoratorPatternExample.cs
```

## Key Concepts

### 1. **Service Lifetimes**
- **Singleton**: One instance for the application
- **Scoped**: One instance per request/scope
- **Transient**: New instance every time

### 2. **Registration Patterns**
- Interface-based registration
- Generic service registration
- Open generic types
- Multiple implementations

### 3. **Advanced Patterns**
- Factory pattern with DI
- Decorator pattern
- Strategy pattern
- Service locator pattern

## Getting Started

1. **Install packages**:
   ```bash
   dotnet add package Microsoft.Extensions.DependencyInjection
   dotnet add package Microsoft.Extensions.Hosting
   dotnet add package Microsoft.Extensions.Logging
   ```

2. **Register services**:
   ```csharp
   services.AddScoped<IUserService, UserService>();
   services.AddSingleton<ILoggerService, ConsoleLoggerService>();
   services.AddTransient<IEmailService, EmailService>();
   ```

3. **Use services**:
   ```csharp
   public class UserController
   {
       private readonly IUserService _userService;
       
       public UserController(IUserService userService)
       {
           _userService = userService;
       }
   }
   ```