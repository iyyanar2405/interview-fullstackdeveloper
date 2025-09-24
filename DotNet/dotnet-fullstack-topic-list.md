# .NET Full Stack Developer - Complete Topic List & Categories

## Table of Contents

1. [Backend Development - .NET Core/5/6/7/8](#backend-development---net-core5678)
2. [Frontend Development](#frontend-development)
3. [Database & Data Access](#database--data-access)
4. [Web APIs & Services](#web-apis--services)
5. [Authentication & Security](#authentication--security)
6. [Cloud & DevOps](#cloud--devops)
7. [Testing](#testing)
8. [Performance & Optimization](#performance--optimization)
9. [Design Patterns & Architecture](#design-patterns--architecture)
10. [Tools & Development Environment](#tools--development-environment)
11. [Soft Skills & Best Practices](#soft-skills--best-practices)

---

## Backend Development - .NET Core/5/6/7/8

### 🔧 Core Fundamentals
- **.NET Framework vs .NET Core vs .NET 5/6/7/8**
  - Evolution and differences
  - Cross-platform capabilities
  - Performance improvements
  - Migration strategies
  
  ```csharp
  // .NET 8 minimal API example
  var builder = WebApplication.CreateBuilder(args);
  var app = builder.Build();
  
  app.MapGet("/", () => "Hello .NET 8!");
  app.Run();
  ```

- **C# Language Features**
  - C# 8.0, 9.0, 10.0, 11.0, 12.0 new features
  - Nullable reference types
  - Pattern matching
  - Records and init-only properties
  - Top-level statements
  - Global using directives
  - File-scoped namespaces
  - Generic math and static virtual members

  ```csharp
  // C# 12 features example
  public record Person(string Name, int Age); // Records
  
  // Pattern matching with switch expressions
  public static string DescribeAge(Person person) => person.Age switch
  {
      < 18 => "Minor",
      >= 18 and < 65 => "Adult",
      >= 65 => "Senior",
      _ => "Unknown"
  };
  
  // Nullable reference types
  public string? GetUserName(int? userId) => userId?.ToString();
  
  // File-scoped namespace (C# 10)
  namespace MyApp.Services;
  
  // Global using (in GlobalUsings.cs)
  global using System;
  global using Microsoft.AspNetCore.Mvc;
  ```

- **Runtime & CLR**
  - Common Language Runtime
  - Garbage Collection
  - Memory management
  - Assembly loading
  - Just-In-Time (JIT) compilation

  ```csharp
  // Memory management example
  public class MemoryExample
  {
      // IDisposable pattern for unmanaged resources
      public void DisposableExample()
      {
          using var fileStream = new FileStream("file.txt", FileMode.Open);
          // Automatically disposed at end of scope
      }
      
      // Weak reference for large objects
      public void WeakReferenceExample()
      {
          var largeObject = new byte[1000000];
          var weakRef = new WeakReference(largeObject);
          largeObject = null; // Allow GC to collect
          
          if (weakRef.IsAlive)
              Console.WriteLine("Object still in memory");
      }
  }
  ```

### 🏗️ Application Architecture
- **Dependency Injection (DI)**
  - Built-in DI container
  - Service lifetimes (Transient, Scoped, Singleton)
  - Custom service registration
  - Third-party containers (Autofac, Unity)

  ```csharp
  // Service registration in Program.cs
  var builder = WebApplication.CreateBuilder(args);
  
  // Different service lifetimes
  builder.Services.AddTransient<IEmailService, EmailService>(); // New instance each time
  builder.Services.AddScoped<IUserService, UserService>();      // One per request
  builder.Services.AddSingleton<ICacheService, CacheService>(); // One for application
  
  var app = builder.Build();
  
  // Service interfaces and implementations
  public interface IUserService
  {
      Task<User> GetUserAsync(int id);
  }
  
  public class UserService : IUserService
  {
      private readonly IEmailService _emailService;
      
      public UserService(IEmailService emailService) // Constructor injection
      {
          _emailService = emailService;
      }
      
      public async Task<User> GetUserAsync(int id)
      {
          var user = await GetFromDatabaseAsync(id);
          await _emailService.SendWelcomeEmailAsync(user.Email);
          return user;
      }
  }
  ```

- **Configuration System**
  - appsettings.json
  - Environment variables
  - User secrets
  - Options pattern
  - Configuration providers
  - Hot reload configuration

  ```csharp
  // appsettings.json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;"
    },
    "JwtSettings": {
      "SecretKey": "super-secret-key",
      "ExpireHours": 24
    }
  }
  
  // Options pattern
  public class JwtSettings
  {
      public string SecretKey { get; set; } = string.Empty;
      public int ExpireHours { get; set; }
  }
  
  // Configuration in Program.cs
  builder.Services.Configure<JwtSettings>(
      builder.Configuration.GetSection("JwtSettings"));
  
  // Usage in service
  public class TokenService
  {
      private readonly JwtSettings _jwtSettings;
      
      public TokenService(IOptions<JwtSettings> jwtSettings)
      {
          _jwtSettings = jwtSettings.Value;
      }
      
      public string GenerateToken(User user)
      {
          // Use _jwtSettings.SecretKey and _jwtSettings.ExpireHours
          return "generated-jwt-token";
      }
  }
  ```

- **Middleware Pipeline**
  - Built-in middleware
  - Custom middleware
  - Middleware ordering
  - Exception handling middleware
  - CORS middleware
  - Authentication middleware

  ```csharp
  // Custom middleware
  public class RequestLoggingMiddleware
  {
      private readonly RequestDelegate _next;
      private readonly ILogger<RequestLoggingMiddleware> _logger;
      
      public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
      {
          _next = next;
          _logger = logger;
      }
      
      public async Task InvokeAsync(HttpContext context)
      {
          var stopwatch = Stopwatch.StartNew();
          _logger.LogInformation("Request: {Method} {Path}", 
              context.Request.Method, context.Request.Path);
          
          await _next(context); // Call next middleware
          
          stopwatch.Stop();
          _logger.LogInformation("Response: {StatusCode} in {ElapsedMs}ms", 
              context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
      }
  }
  
  // Register middleware in Program.cs
  var app = builder.Build();
  
  app.UseMiddleware<RequestLoggingMiddleware>(); // Custom middleware
  app.UseAuthentication();  // Built-in middleware
  app.UseAuthorization();   // Order matters!
  app.MapControllers();
  ```

- **Hosting & Deployment**
  - Generic Host
  - Web Host vs Generic Host
  - Background services
  - Hosted services
  - Application lifecycle

  ```csharp
  // Background service example
  public class EmailBackgroundService : BackgroundService
  {
      private readonly IServiceProvider _serviceProvider;
      private readonly ILogger<EmailBackgroundService> _logger;
      
      public EmailBackgroundService(IServiceProvider serviceProvider, ILogger<EmailBackgroundService> logger)
      {
          _serviceProvider = serviceProvider;
          _logger = logger;
      }
      
      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
          while (!stoppingToken.IsCancellationRequested)
          {
              using var scope = _serviceProvider.CreateScope();
              var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
              
              await emailService.ProcessPendingEmailsAsync();
              await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
          }
      }
  }
  
  // Register background service
  builder.Services.AddHostedService<EmailBackgroundService>();
  ```

### 📊 Data Access Layer
- **Entity Framework Core**
  - Code First vs Database First
  - DbContext and DbSet
  - Migrations and seeding
  - Relationships (One-to-One, One-to-Many, Many-to-Many)
  - Query optimization
  - Change tracking
  - Lazy vs Eager loading

  ```csharp
  // Entity models
  public class User
  {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public List<Order> Orders { get; set; } = new();
  }
  
  public class Order
  {
      public int Id { get; set; }
      public decimal Amount { get; set; }
      public int UserId { get; set; }
      public User User { get; set; } = null!;
  }
  
  // DbContext
  public class AppDbContext : DbContext
  {
      public DbSet<User> Users { get; set; }
      public DbSet<Order> Orders { get; set; }
      
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          // Relationships and constraints
          modelBuilder.Entity<User>()
              .HasIndex(u => u.Email)
              .IsUnique();
              
          modelBuilder.Entity<Order>()
              .HasOne(o => o.User)
              .WithMany(u => u.Orders)
              .HasForeignKey(o => o.UserId);
      }
  }
  
  // Repository pattern
  public class UserRepository
  {
      private readonly AppDbContext _context;
      
      public UserRepository(AppDbContext context) => _context = context;
      
      // Eager loading
      public async Task<User?> GetUserWithOrdersAsync(int id)
      {
          return await _context.Users
              .Include(u => u.Orders)
              .FirstOrDefaultAsync(u => u.Id == id);
      }
      
      // Projection for performance
      public async Task<List<UserSummary>> GetUserSummariesAsync()
      {
          return await _context.Users
              .Select(u => new UserSummary
              {
                  Id = u.Id,
                  Name = u.Name,
                  OrderCount = u.Orders.Count,
                  TotalSpent = u.Orders.Sum(o => o.Amount)
              })
              .ToListAsync();
      }
  }
  ```

- **Dapper (Micro ORM)**
  - Basic queries
  - Multi-mapping
  - Stored procedures
  - Performance comparison with EF Core

  ```csharp
  // Dapper usage
  public class DapperUserRepository
  {
      private readonly IDbConnection _connection;
      
      public DapperUserRepository(IDbConnection connection) => _connection = connection;
      
      public async Task<User?> GetUserAsync(int id)
      {
          const string sql = "SELECT * FROM Users WHERE Id = @Id";
          return await _connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
      }
      
      public async Task<List<User>> GetUsersAsync()
      {
          const string sql = "SELECT * FROM Users";
          return (await _connection.QueryAsync<User>(sql)).ToList();
      }
      
      // Multi-mapping (joining tables)
      public async Task<List<User>> GetUsersWithOrdersAsync()
      {
          const string sql = @"
              SELECT u.*, o.* 
              FROM Users u 
              LEFT JOIN Orders o ON u.Id = o.UserId";
              
          var userDict = new Dictionary<int, User>();
          
          return (await _connection.QueryAsync<User, Order, User>(sql,
              (user, order) =>
              {
                  if (!userDict.TryGetValue(user.Id, out var existingUser))
                  {
                      existingUser = user;
                      existingUser.Orders = new List<Order>();
                      userDict.Add(user.Id, existingUser);
                  }
                  
                  if (order != null)
                      existingUser.Orders.Add(order);
                      
                  return existingUser;
              }, splitOn: "Id")).Distinct().ToList();
      }
  }
  ```

- **ADO.NET**
  - Connection management
  - Command execution
  - Data readers
  - Parameterized queries

  ```csharp
  // ADO.NET example
  public class AdoUserRepository
  {
      private readonly string _connectionString;
      
      public AdoUserRepository(string connectionString) => _connectionString = connectionString;
      
      public async Task<User?> GetUserAsync(int id)
      {
          using var connection = new SqlConnection(_connectionString);
          using var command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
          
          command.Parameters.AddWithValue("@Id", id);
          await connection.OpenAsync();
          
          using var reader = await command.ExecuteReaderAsync();
          if (await reader.ReadAsync())
          {
              return new User
              {
                  Id = reader.GetInt32("Id"),
                  Name = reader.GetString("Name"),
                  Email = reader.GetString("Email")
              };
          }
          return null;
      }
      
      public async Task<int> CreateUserAsync(User user)
      {
          using var connection = new SqlConnection(_connectionString);
          using var command = new SqlCommand(
              "INSERT INTO Users (Name, Email) OUTPUT INSERTED.Id VALUES (@Name, @Email)", 
              connection);
              
          command.Parameters.AddWithValue("@Name", user.Name);
          command.Parameters.AddWithValue("@Email", user.Email);
          
          await connection.OpenAsync();
          return (int)await command.ExecuteScalarAsync();
      }
  }
  ```

### 🔄 Asynchronous Programming
- **Async/Await Pattern**
  - Task and Task<T>
  - ConfigureAwait
  - Async best practices
  - Deadlock prevention
  - CancellationToken

  ```csharp
  public class AsyncExamples
  {
      // Basic async method
      public async Task<string> GetDataAsync()
      {
          using var httpClient = new HttpClient();
          var response = await httpClient.GetStringAsync("https://api.example.com/data");
          return response;
      }
      
      // Using CancellationToken
      public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
      {
          await Task.Delay(1000, cancellationToken); // Simulate work
          return new List<User>();
      }
      
      // ConfigureAwait for library code
      public async Task<string> LibraryMethodAsync()
      {
          var result = await SomeAsyncOperation().ConfigureAwait(false);
          return result.ToUpperInvariant();
      }
      
      // Async enumerable (C# 8.0)
      public async IAsyncEnumerable<int> GenerateNumbersAsync()
      {
          for (int i = 0; i < 10; i++)
          {
              await Task.Delay(100);
              yield return i;
          }
      }
      
      // Using async enumerable
      public async Task ConsumeAsyncEnumerableAsync()
      {
          await foreach (var number in GenerateNumbersAsync())
          {
              Console.WriteLine(number);
          }
      }
  }
  ```

- **Parallel Programming**
  - Parallel LINQ (PLINQ)
  - Task Parallel Library (TPL)
  - Parallel.ForEach
  - ConcurrentCollections

  ```csharp
  public class ParallelExamples
  {
      // PLINQ example
      public List<int> ProcessNumbersParallel(List<int> numbers)
      {
          return numbers
              .AsParallel()
              .Where(n => n % 2 == 0)
              .Select(n => n * 2)
              .OrderBy(n => n)
              .ToList();
      }
      
      // Parallel.ForEach
      public async Task ProcessItemsParallelAsync(List<string> items)
      {
          await Parallel.ForEachAsync(items, async (item, cancellationToken) =>
          {
              await ProcessItemAsync(item);
          });
      }
      
      // Task.WhenAll for concurrent execution
      public async Task<List<string>> FetchMultipleUrlsAsync(List<string> urls)
      {
          using var httpClient = new HttpClient();
          
          var tasks = urls.Select(url => httpClient.GetStringAsync(url));
          var results = await Task.WhenAll(tasks);
          
          return results.ToList();
      }
      
      // ConcurrentCollection usage
      private readonly ConcurrentQueue<WorkItem> _workQueue = new();
      private readonly ConcurrentDictionary<string, string> _cache = new();
      
      public void ProducerConsumerPattern()
      {
          // Producer
          Task.Run(() =>
          {
              for (int i = 0; i < 100; i++)
              {
                  _workQueue.Enqueue(new WorkItem { Id = i });
              }
          });
          
          // Consumer
          Task.Run(() =>
          {
              while (_workQueue.TryDequeue(out var workItem))
              {
                  ProcessWorkItem(workItem);
              }
          });
      }
  }
  ```

### 📝 Logging & Monitoring
- **Built-in Logging**
  - ILogger interface
  - Log levels
  - Structured logging
  - Log providers (Console, Debug, EventSource)

  ```csharp
  public class LoggingExamples
  {
      private readonly ILogger<LoggingExamples> _logger;
      
      public LoggingExamples(ILogger<LoggingExamples> logger) => _logger = logger;
      
      public async Task<User> GetUserAsync(int userId)
      {
          // Structured logging with parameters
          _logger.LogInformation("Getting user with ID {UserId}", userId);
          
          try
          {
              var user = await GetUserFromDatabaseAsync(userId);
              
              if (user == null)
              {
                  _logger.LogWarning("User with ID {UserId} not found", userId);
                  return null;
              }
              
              _logger.LogDebug("User {UserName} retrieved successfully", user.Name);
              return user;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
              throw;
          }
      }
      
      // Using LoggerMessage for high-performance logging
      private static readonly Action<ILogger, int, Exception?> _getUserLog =
          LoggerMessage.Define<int>(
              LogLevel.Information,
              new EventId(1001, nameof(GetUserAsync)),
              "Getting user with ID {UserId}");
              
      public void HighPerformanceLogging(int userId)
      {
          _getUserLog(_logger, userId, null);
      }
  }
  
  // Program.cs logging configuration
  var builder = WebApplication.CreateBuilder(args);
  
  builder.Logging.ClearProviders();
  builder.Logging.AddConsole();
  builder.Logging.AddDebug();
  builder.Logging.SetMinimumLevel(LogLevel.Information);
  ```

- **Third-party Logging**
  - Serilog
  - NLog
  - Log4Net
  - Application Insights integration

  ```csharp
  // Serilog configuration
  using Serilog;
  
  var builder = WebApplication.CreateBuilder(args);
  
  // Configure Serilog
  builder.Host.UseSerilog((context, configuration) =>
      configuration
          .ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
          .WriteTo.ApplicationInsights(TelemetryClient, TelemetryConverter.Traces));
  
  // Serilog in appsettings.json
  {
    "Serilog": {
      "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
      "MinimumLevel": "Information",
      "WriteTo": [
        { "Name": "Console" },
        { 
          "Name": "File", 
          "Args": { 
            "path": "logs/log-.txt",
            "rollingInterval": "Day" 
          } 
        }
      ]
    }
  }
  
  // Structured logging with Serilog
  public class SerilogExample
  {
      private readonly ILogger _logger = Log.ForContext<SerilogExample>();
      
      public void LogUserActivity(User user, string action)
      {
          _logger.Information("User {UserId} {UserName} performed {Action} at {Timestamp}", 
              user.Id, user.Name, action, DateTimeOffset.UtcNow);
      }
  }
  ```

### 🔐 Error Handling
- **Exception Handling**
  - Try-catch-finally
  - Custom exceptions
  - Global exception handling
  - Problem Details (RFC 7807)

---

## Frontend Development

### 🌐 Client-Side Technologies
- **HTML5 & CSS3**
  - Semantic HTML
  - CSS Grid and Flexbox
  - Responsive design
  - CSS preprocessors (SASS, LESS)

  ```html
  <!-- HTML5 semantic elements -->
  <!DOCTYPE html>
  <html lang="en">
  <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Semantic HTML5</title>
  </head>
  <body>
      <header>
          <nav>
              <ul>
                  <li><a href="#home">Home</a></li>
                  <li><a href="#about">About</a></li>
              </ul>
          </nav>
      </header>
      
      <main>
          <article>
              <section>
                  <h1>Article Title</h1>
                  <p>Article content...</p>
              </section>
          </article>
          
          <aside>
              <h2>Related Links</h2>
          </aside>
      </main>
      
      <footer>
          <p>&copy; 2025 My Website</p>
      </footer>
  </body>
  </html>
  ```

  ```css
  /* CSS Grid and Flexbox example */
  .container {
      display: grid;
      grid-template-columns: 1fr 3fr 1fr;
      grid-template-rows: auto 1fr auto;
      grid-template-areas: 
          "header header header"
          "nav main aside"
          "footer footer footer";
      min-height: 100vh;
      gap: 1rem;
  }
  
  header { grid-area: header; }
  nav { grid-area: nav; }
  main { grid-area: main; }
  aside { grid-area: aside; }
  footer { grid-area: footer; }
  
  /* Flexbox for navigation */
  nav ul {
      display: flex;
      list-style: none;
      gap: 1rem;
      padding: 0;
  }
  
  /* Responsive design */
  @media (max-width: 768px) {
      .container {
          grid-template-columns: 1fr;
          grid-template-areas: 
              "header"
              "nav"
              "main"
              "aside"
              "footer";
      }
  }
  
  /* SCSS example */
  $primary-color: #007bff;
  $secondary-color: #6c757d;
  
  .button {
      background-color: $primary-color;
      border: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      
      &:hover {
          background-color: darken($primary-color, 10%);
      }
      
      &--secondary {
          background-color: $secondary-color;
      }
  }
  ```

- **JavaScript & TypeScript**
  - ES6+ features
  - TypeScript fundamentals
  - Module systems
  - DOM manipulation
  - Event handling
  - AJAX and Fetch API

  ```javascript
  // ES6+ features
  
  // Arrow functions and destructuring
  const users = [
      { id: 1, name: 'John', email: 'john@example.com' },
      { id: 2, name: 'Jane', email: 'jane@example.com' }
  ];
  
  const getUserEmails = (users) => users.map(({ email }) => email);
  
  // Async/await with fetch
  const fetchUserData = async (userId) => {
      try {
          const response = await fetch(`/api/users/${userId}`);
          if (!response.ok) throw new Error('User not found');
          
          const user = await response.json();
          return user;
      } catch (error) {
          console.error('Error fetching user:', error);
          return null;
      }
  };
  
  // Classes and modules
  class UserService {
      constructor(baseUrl) {
          this.baseUrl = baseUrl;
      }
      
      async getUsers() {
          const response = await fetch(`${this.baseUrl}/users`);
          return response.json();
      }
      
      async createUser(userData) {
          const response = await fetch(`${this.baseUrl}/users`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(userData)
          });
          return response.json();
      }
  }
  
  // DOM manipulation
  document.addEventListener('DOMContentLoaded', () => {
      const button = document.getElementById('load-users');
      const userList = document.getElementById('user-list');
      
      button.addEventListener('click', async () => {
          const users = await fetchUserData();
          
          userList.innerHTML = users
              .map(user => `<li>${user.name} - ${user.email}</li>`)
              .join('');
      });
  });
  ```

  ```typescript
  // TypeScript examples
  
  interface User {
      id: number;
      name: string;
      email: string;
      isActive?: boolean;
  }
  
  interface ApiResponse<T> {
      data: T;
      success: boolean;
      message: string;
  }
  
  class TypedUserService {
      private baseUrl: string;
      
      constructor(baseUrl: string) {
          this.baseUrl = baseUrl;
      }
      
      async getUser(id: number): Promise<User | null> {
          const response = await fetch(`${this.baseUrl}/users/${id}`);
          
          if (!response.ok) {
              return null;
          }
          
          const apiResponse: ApiResponse<User> = await response.json();
          return apiResponse.success ? apiResponse.data : null;
      }
      
      async getUsers(): Promise<User[]> {
          const response = await fetch(`${this.baseUrl}/users`);
          const apiResponse: ApiResponse<User[]> = await response.json();
          return apiResponse.data || [];
      }
  }
  
  // Generic function
  function createApiClient<T>(baseUrl: string): ApiClient<T> {
      return new ApiClient<T>(baseUrl);
  }
  
  // Enum
  enum UserRole {
      Admin = 'admin',
      User = 'user',
      Guest = 'guest'
  }
  
  // Union types
  type Status = 'loading' | 'success' | 'error';
  
  // Utility types
  type CreateUserRequest = Omit<User, 'id'>;
  type UpdateUserRequest = Partial<Pick<User, 'name' | 'email'>>;
  ```

### ⚛️ Frontend Frameworks
- **React.js**
  - Components and JSX
  - State management (Redux, Context API)
  - Hooks (useState, useEffect, custom hooks)
  - Routing (React Router)
  - Form handling
  - Performance optimization

  ```jsx
  // React functional component with hooks
  import React, { useState, useEffect, useContext } from 'react';
  import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom';
  
  // Context for state management
  const UserContext = React.createContext();
  
  // Custom hook
  const useApi = (url) => {
      const [data, setData] = useState(null);
      const [loading, setLoading] = useState(true);
      const [error, setError] = useState(null);
      
      useEffect(() => {
          const fetchData = async () => {
              try {
                  const response = await fetch(url);
                  if (!response.ok) throw new Error('Failed to fetch');
                  
                  const result = await response.json();
                  setData(result);
              } catch (err) {
                  setError(err.message);
              } finally {
                  setLoading(false);
              }
          };
          
          fetchData();
      }, [url]);
      
      return { data, loading, error };
  };
  
  // Component with form handling
  const UserForm = ({ onSubmit }) => {
      const [formData, setFormData] = useState({ name: '', email: '' });
      
      const handleSubmit = (e) => {
          e.preventDefault();
          onSubmit(formData);
          setFormData({ name: '', email: '' });
      };
      
      const handleChange = (e) => {
          setFormData(prev => ({
              ...prev,
              [e.target.name]: e.target.value
          }));
      };
      
      return (
          <form onSubmit={handleSubmit}>
              <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  placeholder="Name"
                  required
              />
              <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  placeholder="Email"
                  required
              />
              <button type="submit">Submit</button>
          </form>
      );
  };
  
  // Main component using custom hook
  const UserList = () => {
      const { data: users, loading, error } = useApi('/api/users');
      const { currentUser } = useContext(UserContext);
      
      if (loading) return <div>Loading...</div>;
      if (error) return <div>Error: {error}</div>;
      
      return (
          <div>
              <h2>Welcome, {currentUser?.name}</h2>
              <ul>
                  {users?.map(user => (
                      <li key={user.id}>
                          {user.name} - {user.email}
                      </li>
                  ))}
              </ul>
          </div>
      );
  };
  
  // App with routing
  const App = () => {
      const [currentUser, setCurrentUser] = useState(null);
      
      return (
          <UserContext.Provider value={{ currentUser, setCurrentUser }}>
              <Router>
                  <Routes>
                      <Route path="/" element={<UserList />} />
                      <Route path="/create" element={<UserForm onSubmit={handleCreateUser} />} />
                  </Routes>
              </Router>
          </UserContext.Provider>
      );
  };
  ```

- **Angular**
  - Components and modules
  - Services and dependency injection
  - Routing and navigation
  - Reactive forms
  - RxJS and observables
  - Angular CLI

  ```typescript
  // Angular component
  import { Component, OnInit, inject } from '@angular/core';
  import { FormBuilder, FormGroup, Validators } from '@angular/forms';
  import { Router } from '@angular/router';
  import { Observable } from 'rxjs';
  import { map, catchError } from 'rxjs/operators';
  
  // Service
  @Injectable({ providedIn: 'root' })
  export class UserService {
      private http = inject(HttpClient);
      private baseUrl = '/api/users';
      
      getUsers(): Observable<User[]> {
          return this.http.get<User[]>(this.baseUrl);
      }
      
      createUser(user: CreateUserRequest): Observable<User> {
          return this.http.post<User>(this.baseUrl, user);
      }
      
      getUserById(id: number): Observable<User> {
          return this.http.get<User>(`${this.baseUrl}/${id}`);
      }
  }
  
  // Component
  @Component({
      selector: 'app-user-list',
      template: `
          <div>
              <h2>Users</h2>
              <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
                  <input 
                      type="text" 
                      formControlName="name" 
                      placeholder="Name"
                      [class.error]="userForm.get('name')?.invalid && userForm.get('name')?.touched">
                  <input 
                      type="email" 
                      formControlName="email" 
                      placeholder="Email"
                      [class.error]="userForm.get('email')?.invalid && userForm.get('email')?.touched">
                  <button type="submit" [disabled]="userForm.invalid">Create User</button>
              </form>
              
              <ul>
                  <li *ngFor="let user of users$ | async; trackBy: trackByUserId">
                      {{ user.name }} - {{ user.email }}
                      <button (click)="viewUser(user.id)">View</button>
                  </li>
              </ul>
          </div>
      `,
      styles: [`
          .error { border-color: red; }
          button:disabled { opacity: 0.6; }
      `]
  })
  export class UserListComponent implements OnInit {
      private userService = inject(UserService);
      private formBuilder = inject(FormBuilder);
      private router = inject(Router);
      
      users$: Observable<User[]>;
      userForm: FormGroup;
      
      constructor() {
          this.userForm = this.formBuilder.group({
              name: ['', [Validators.required, Validators.minLength(2)]],
              email: ['', [Validators.required, Validators.email]]
          });
      }
      
      ngOnInit() {
          this.users$ = this.userService.getUsers().pipe(
              catchError(error => {
                  console.error('Error loading users:', error);
                  return [];
              })
          );
      }
      
      onSubmit() {
          if (this.userForm.valid) {
              const userData = this.userForm.value;
              this.userService.createUser(userData).subscribe({
                  next: (user) => {
                      console.log('User created:', user);
                      this.userForm.reset();
                      // Refresh users list
                      this.users$ = this.userService.getUsers();
                  },
                  error: (error) => console.error('Error creating user:', error)
              });
          }
      }
      
      viewUser(userId: number) {
          this.router.navigate(['/users', userId]);
      }
      
      trackByUserId(index: number, user: User): number {
          return user.id;
      }
  }
  
  // Module
  @NgModule({
      declarations: [UserListComponent],
      imports: [
          CommonModule,
          ReactiveFormsModule,
          HttpClientModule,
          RouterModule
      ]
  })
  export class UserModule { }
  ```

- **Vue.js**
  - Vue 3 Composition API
  - Component communication
  - Vuex state management
  - Vue Router
  - Single File Components

  ```vue
  <!-- Vue 3 Single File Component -->
  <template>
    <div class="user-manager">
      <h2>User Management</h2>
      
      <!-- User Form -->
      <form @submit.prevent="createUser" class="user-form">
        <input
          v-model="newUser.name"
          type="text"
          placeholder="Name"
          required
        />
        <input
          v-model="newUser.email"
          type="email"
          placeholder="Email"
          required
        />
        <button type="submit" :disabled="loading">
          {{ loading ? 'Creating...' : 'Create User' }}
        </button>
      </form>
      
      <!-- User List -->
      <div v-if="error" class="error">
        Error: {{ error }}
      </div>
      
      <div v-else-if="loading" class="loading">
        Loading users...
      </div>
      
      <ul v-else class="user-list">
        <li v-for="user in users" :key="user.id" class="user-item">
          <span>{{ user.name }} - {{ user.email }}</span>
          <button @click="$router.push(`/users/${user.id}`)">
            View Details
          </button>
        </li>
      </ul>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted, computed } from 'vue'
  import { useRouter } from 'vue-router'
  import { useStore } from 'vuex'
  
  // Interfaces
  interface User {
    id: number
    name: string
    email: string
  }
  
  interface NewUser {
    name: string
    email: string
  }
  
  // Composables
  const router = useRouter()
  const store = useStore()
  
  // Reactive data
  const users = ref<User[]>([])
  const newUser = ref<NewUser>({ name: '', email: '' })
  const loading = ref(false)
  const error = ref<string | null>(null)
  
  // Computed
  const userCount = computed(() => users.value.length)
  
  // Methods
  const fetchUsers = async () => {
    try {
      loading.value = true
      const response = await fetch('/api/users')
      if (!response.ok) throw new Error('Failed to fetch users')
      
      users.value = await response.json()
      error.value = null
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
    } finally {
      loading.value = false
    }
  }
  
  const createUser = async () => {
    try {
      loading.value = true
      const response = await fetch('/api/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newUser.value)
      })
      
      if (!response.ok) throw new Error('Failed to create user')
      
      const createdUser = await response.json()
      users.value.push(createdUser)
      newUser.value = { name: '', email: '' }
      
      // Update Vuex store
      store.commit('ADD_USER', createdUser)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create user'
    } finally {
      loading.value = false
    }
  }
  
  // Lifecycle
  onMounted(() => {
    fetchUsers()
  })
  </script>
  
  <style scoped>
  .user-manager {
    max-width: 600px;
    margin: 0 auto;
    padding: 20px;
  }
  
  .user-form {
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
  }
  
  .user-form input {
    flex: 1;
    padding: 8px;
    border: 1px solid #ddd;
    border-radius: 4px;
  }
  
  .user-list {
    list-style: none;
    padding: 0;
  }
  
  .user-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px;
    border: 1px solid #eee;
    margin-bottom: 5px;
    border-radius: 4px;
  }
  
  .error {
    color: red;
    padding: 10px;
    background-color: #fee;
    border-radius: 4px;
  }
  
  .loading {
    text-align: center;
    padding: 20px;
  }
  </style>
  ```

### 🎨 UI Frameworks & Libraries
- **Bootstrap**
  - Grid system
  - Components
  - Utilities
  - Customization

- **Material-UI / Ant Design**
  - Component libraries
  - Theming
  - Custom styling

- **Blazor**
  - Blazor Server vs WebAssembly
  - Component model
  - Data binding
  - JavaScript interop
  - Blazor routing

  ```csharp
  // Blazor Server component (.razor file)
  @page "/users"
  @using System.ComponentModel.DataAnnotations
  @inject IUserService UserService
  @inject IJSRuntime JSRuntime
  
  <PageTitle>User Management</PageTitle>
  
  <h3>Users</h3>
  
  @* User Creation Form *@
  <EditForm Model="@newUser" OnValidSubmit="@CreateUser">
      <DataAnnotationsValidator />
      <ValidationSummary />
      
      <div class="form-group">
          <label for="name">Name:</label>
          <InputText id="name" @bind-Value="newUser.Name" class="form-control" />
          <ValidationMessage For="@(() => newUser.Name)" />
      </div>
      
      <div class="form-group">
          <label for="email">Email:</label>
          <InputText id="email" @bind-Value="newUser.Email" class="form-control" />
          <ValidationMessage For="@(() => newUser.Email)" />
      </div>
      
      <button type="submit" class="btn btn-primary" disabled="@loading">
          @if (loading)
          {
              <span class="spinner-border spinner-border-sm me-2"></span>
          }
          Create User
      </button>
  </EditForm>
  
  @* User List *@
  @if (users == null)
  {
      <p><em>Loading users...</em></p>
  }
  else
  {
      <table class="table table-striped">
          <thead>
              <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Actions</th>
              </tr>
          </thead>
          <tbody>
              @foreach (var user in users)
              {
                  <tr>
                      <td>@user.Name</td>
                      <td>@user.Email</td>
                      <td>
                          <button class="btn btn-info btn-sm" @onclick="() => ViewUser(user.Id)">
                              View
                          </button>
                          <button class="btn btn-danger btn-sm ms-2" @onclick="() => DeleteUser(user.Id)">
                              Delete
                          </button>
                      </td>
                  </tr>
              }
          </tbody>
      </table>
  }
  
  @code {
      private List<User>? users;
      private CreateUserModel newUser = new();
      private bool loading = false;
      
      protected override async Task OnInitializedAsync()
      {
          await LoadUsers();
      }
      
      private async Task LoadUsers()
      {
          users = await UserService.GetUsersAsync();
      }
      
      private async Task CreateUser()
      {
          loading = true;
          try
          {
              var user = new User
              {
                  Name = newUser.Name,
                  Email = newUser.Email
              };
              
              await UserService.CreateUserAsync(user);
              await LoadUsers(); // Refresh list
              newUser = new(); // Reset form
              
              // Show success message using JavaScript interop
              await JSRuntime.InvokeVoidAsync("alert", "User created successfully!");
          }
          catch (Exception ex)
          {
              await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
          }
          finally
          {
              loading = false;
          }
      }
      
      private async Task ViewUser(int userId)
      {
          // Navigate to user details page
          NavigationManager.NavigateTo($"/users/{userId}");
      }
      
      private async Task DeleteUser(int userId)
      {
          var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
              "Are you sure you want to delete this user?");
              
          if (confirmed)
          {
              await UserService.DeleteUserAsync(userId);
              await LoadUsers(); // Refresh list
          }
      }
      
      // Model for form validation
      public class CreateUserModel
      {
          [Required(ErrorMessage = "Name is required")]
          [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
          public string Name { get; set; } = string.Empty;
          
          [Required(ErrorMessage = "Email is required")]
          [EmailAddress(ErrorMessage = "Please enter a valid email address")]
          public string Email { get; set; } = string.Empty;
      }
  }
  
  // Custom Blazor component for reusable user card
  @* UserCard.razor *@
  <div class="card mb-3">
      <div class="card-body">
          <h5 class="card-title">@User.Name</h5>
          <p class="card-text">@User.Email</p>
          <button class="btn btn-primary" @onclick="OnViewClicked">
              View Details
          </button>
          <button class="btn btn-outline-secondary ms-2" @onclick="OnEditClicked">
              Edit
          </button>
      </div>
  </div>
  
  @code {
      [Parameter] public User User { get; set; } = null!;
      [Parameter] public EventCallback<User> OnView { get; set; }
      [Parameter] public EventCallback<User> OnEdit { get; set; }
      
      private async Task OnViewClicked() => await OnView.InvokeAsync(User);
      private async Task OnEditClicked() => await OnEdit.InvokeAsync(User);
  }
  
  // Blazor WebAssembly Program.cs
  public class Program
  {
      public static async Task Main(string[] args)
      {
          var builder = WebAssemblyHostBuilder.CreateDefault(args);
          builder.RootComponents.Add<App>("#app");
          
          // Configure services
          builder.Services.AddScoped(sp => new HttpClient 
          { 
              BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
          });
          builder.Services.AddScoped<IUserService, UserService>();
          
          await builder.Build().RunAsync();
      }
  }
  ```

---

## Database & Data Access

### 🗄️ Database Systems
- **SQL Server**
  - T-SQL fundamentals
  - Stored procedures and functions
  - Indexes and query optimization
  - Triggers
  - Views
  - Security and permissions

  ```sql
  -- T-SQL Examples
  
  -- Create table with constraints
  CREATE TABLE Users (
      Id INT IDENTITY(1,1) PRIMARY KEY,
      Name NVARCHAR(100) NOT NULL,
      Email NVARCHAR(255) NOT NULL UNIQUE,
      DateCreated DATETIME2 DEFAULT GETUTCDATE(),
      IsActive BIT DEFAULT 1,
      INDEX IX_Users_Email NONCLUSTERED (Email),
      INDEX IX_Users_DateCreated NONCLUSTERED (DateCreated DESC)
  );
  
  -- Stored procedure with parameters
  CREATE PROCEDURE GetUsersByDateRange
      @StartDate DATETIME2,
      @EndDate DATETIME2,
      @IsActive BIT = 1
  AS
  BEGIN
      SET NOCOUNT ON;
      
      SELECT 
          u.Id,
          u.Name,
          u.Email,
          u.DateCreated,
          COUNT(o.Id) as OrderCount,
          COALESCE(SUM(o.Amount), 0) as TotalSpent
      FROM Users u
      LEFT JOIN Orders o ON u.Id = o.UserId
      WHERE u.DateCreated BETWEEN @StartDate AND @EndDate
        AND u.IsActive = @IsActive
      GROUP BY u.Id, u.Name, u.Email, u.DateCreated
      ORDER BY u.DateCreated DESC;
  END
  
  -- Function to calculate user age
  CREATE FUNCTION dbo.CalculateAge(@BirthDate DATE)
  RETURNS INT
  AS
  BEGIN
      RETURN DATEDIFF(YEAR, @BirthDate, GETDATE()) - 
             CASE WHEN MONTH(@BirthDate) > MONTH(GETDATE()) 
                     OR (MONTH(@BirthDate) = MONTH(GETDATE()) AND DAY(@BirthDate) > DAY(GETDATE()))
                  THEN 1 ELSE 0 END
  END
  
  -- Trigger for audit logging
  CREATE TRIGGER tr_Users_Audit
  ON Users
  AFTER INSERT, UPDATE, DELETE
  AS
  BEGIN
      SET NOCOUNT ON;
      
      IF EXISTS(SELECT * FROM inserted)
      BEGIN
          INSERT INTO UserAudit (UserId, Action, ChangeDate, ChangedBy)
          SELECT Id, 
                 CASE WHEN EXISTS(SELECT * FROM deleted) THEN 'UPDATE' ELSE 'INSERT' END,
                 GETUTCDATE(), 
                 SYSTEM_USER
          FROM inserted;
      END
      
      IF EXISTS(SELECT * FROM deleted) AND NOT EXISTS(SELECT * FROM inserted)
      BEGIN
          INSERT INTO UserAudit (UserId, Action, ChangeDate, ChangedBy)
          SELECT Id, 'DELETE', GETUTCDATE(), SYSTEM_USER
          FROM deleted;
      END
  END
  
  -- Advanced query with window functions
  SELECT 
      u.Name,
      u.Email,
      o.Amount,
      ROW_NUMBER() OVER (PARTITION BY u.Id ORDER BY o.DateCreated DESC) as OrderRank,
      LAG(o.Amount) OVER (PARTITION BY u.Id ORDER BY o.DateCreated) as PreviousOrderAmount,
      AVG(o.Amount) OVER (PARTITION BY u.Id) as AvgOrderAmount
  FROM Users u
  INNER JOIN Orders o ON u.Id = o.UserId
  WHERE u.IsActive = 1;
  
  -- Common Table Expression (CTE)
  WITH MonthlySales AS (
      SELECT 
          YEAR(DateCreated) as Year,
          MONTH(DateCreated) as Month,
          SUM(Amount) as MonthlyTotal,
          COUNT(*) as OrderCount
      FROM Orders
      GROUP BY YEAR(DateCreated), MONTH(DateCreated)
  ),
  YearlySales AS (
      SELECT 
          Year,
          SUM(MonthlyTotal) as YearlyTotal,
          AVG(MonthlyTotal) as AvgMonthlyTotal
      FROM MonthlySales
      GROUP BY Year
  )
  SELECT * FROM YearlySales ORDER BY Year DESC;
  ```

- **PostgreSQL**
  - Advanced data types
  - JSON support
  - Full-text search
  - Extensions

  ```sql
  -- PostgreSQL advanced features
  
  -- JSON column type
  CREATE TABLE ProductCatalog (
      id SERIAL PRIMARY KEY,
      name VARCHAR(255) NOT NULL,
      specifications JSONB,
      tags TEXT[],
      created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
  );
  
  -- Insert with JSON data
  INSERT INTO ProductCatalog (name, specifications, tags) VALUES
  ('Laptop', 
   '{"brand": "Dell", "cpu": "Intel i7", "ram": "16GB", "storage": {"type": "SSD", "size": "512GB"}}',
   ARRAY['electronics', 'computers', 'portable']),
  ('Smartphone',
   '{"brand": "Apple", "model": "iPhone 14", "storage": "128GB", "color": "Blue"}',
   ARRAY['electronics', 'mobile', 'communication']);
  
  -- Query JSON data
  SELECT 
      name,
      specifications->>'brand' as brand,
      specifications->'storage'->>'size' as storage_size,
      array_to_string(tags, ', ') as tag_list
  FROM ProductCatalog
  WHERE specifications->>'brand' = 'Dell'
     OR 'electronics' = ANY(tags);
  
  -- Full-text search
  CREATE EXTENSION IF NOT EXISTS pg_trgm;
  
  ALTER TABLE ProductCatalog 
  ADD COLUMN search_vector tsvector;
  
  UPDATE ProductCatalog 
  SET search_vector = to_tsvector('english', name || ' ' || array_to_string(tags, ' '));
  
  CREATE INDEX idx_product_search ON ProductCatalog USING GIN(search_vector);
  
  -- Search query
  SELECT name, ts_rank(search_vector, query) as rank
  FROM ProductCatalog, plainto_tsquery('english', 'laptop computer') as query
  WHERE search_vector @@ query
  ORDER BY rank DESC;
  
  -- Advanced aggregation with FILTER
  SELECT 
      date_trunc('month', created_at) as month,
      COUNT(*) as total_products,
      COUNT(*) FILTER (WHERE 'electronics' = ANY(tags)) as electronics_count,
      AVG(CASE WHEN specifications->>'brand' = 'Apple' THEN 1.0 ELSE 0.0 END) as apple_percentage
  FROM ProductCatalog
  GROUP BY date_trunc('month', created_at)
  ORDER BY month;
  ```

- **MySQL**
  - Performance tuning
  - Replication
  - Storage engines

  ```sql
  -- MySQL specific features
  
  -- Create table with different storage engines
  CREATE TABLE Users (
      id INT AUTO_INCREMENT PRIMARY KEY,
      name VARCHAR(100) NOT NULL,
      email VARCHAR(255) NOT NULL UNIQUE,
      created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
      INDEX idx_email (email),
      INDEX idx_created_at (created_at)
  ) ENGINE=InnoDB;
  
  -- Partitioned table for large datasets
  CREATE TABLE OrdersPartitioned (
      id INT NOT NULL,
      user_id INT NOT NULL,
      amount DECIMAL(10,2),
      order_date DATE NOT NULL,
      PRIMARY KEY (id, order_date)
  ) ENGINE=InnoDB
  PARTITION BY RANGE (YEAR(order_date)) (
      PARTITION p2022 VALUES LESS THAN (2023),
      PARTITION p2023 VALUES LESS THAN (2024),
      PARTITION p2024 VALUES LESS THAN (2025),
      PARTITION pFuture VALUES LESS THAN MAXVALUE
  );
  
  -- Performance optimization with EXPLAIN
  EXPLAIN SELECT 
      u.name, 
      COUNT(o.id) as order_count,
      SUM(o.amount) as total_amount
  FROM Users u
  LEFT JOIN Orders o ON u.id = o.user_id
  WHERE u.created_at >= DATE_SUB(NOW(), INTERVAL 30 DAY)
  GROUP BY u.id, u.name
  HAVING total_amount > 1000;
  
  -- Memory engine for temporary data
  CREATE TABLE SessionData (
      session_id VARCHAR(128) PRIMARY KEY,
      user_id INT,
      data JSON,
      expires_at TIMESTAMP,
      KEY idx_expires (expires_at)
  ) ENGINE=MEMORY;
  ```

- **SQLite**
  - Embedded database scenarios
  - Migration strategies

### 🏪 NoSQL Databases
- **MongoDB**
  - Document-based storage
  - Aggregation pipeline
  - Indexing strategies
  - Replica sets

  ```csharp
  // MongoDB with C# driver
  using MongoDB.Driver;
  using MongoDB.Bson;
  
  public class User
  {
      public ObjectId Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public DateTime CreatedAt { get; set; }
      public List<string> Tags { get; set; } = new();
      public Address Address { get; set; } = new();
  }
  
  public class Address
  {
      public string Street { get; set; } = string.Empty;
      public string City { get; set; } = string.Empty;
      public string Country { get; set; } = string.Empty;
      public string PostalCode { get; set; } = string.Empty;
  }
  
  public class MongoUserRepository
  {
      private readonly IMongoCollection<User> _users;
      
      public MongoUserRepository(IMongoDatabase database)
      {
          _users = database.GetCollection<User>("users");
          
          // Create indexes
          var indexKeys = Builders<User>.IndexKeys
              .Ascending(u => u.Email)
              .Ascending(u => u.CreatedAt);
          _users.Indexes.CreateOne(new CreateIndexModel<User>(indexKeys));
      }
      
      // Basic CRUD operations
      public async Task<User> CreateUserAsync(User user)
      {
          await _users.InsertOneAsync(user);
          return user;
      }
      
      public async Task<User?> GetUserByIdAsync(ObjectId id)
      {
          return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
      }
      
      public async Task<List<User>> GetUsersByTagAsync(string tag)
      {
          var filter = Builders<User>.Filter.AnyEq(u => u.Tags, tag);
          return await _users.Find(filter).ToListAsync();
      }
      
      // Aggregation pipeline
      public async Task<List<UserSummary>> GetUserSummariesAsync()
      {
          var pipeline = new BsonDocument[]
          {
              new("$match", new BsonDocument("createdAt", new BsonDocument("$gte", DateTime.UtcNow.AddDays(-30)))),
              new("$group", new BsonDocument
              {
                  ["_id"] = "$address.country",
                  ["userCount"] = new BsonDocument("$sum", 1),
                  ["users"] = new BsonDocument("$push", new BsonDocument
                  {
                      ["name"] = "$name",
                      ["email"] = "$email"
                  })
              }),
              new("$sort", new BsonDocument("userCount", -1))
          };
          
          return await _users.Aggregate<UserSummary>(pipeline).ToListAsync();
      }
      
      // Text search
      public async Task<List<User>> SearchUsersAsync(string searchTerm)
      {
          var filter = Builders<User>.Filter.Or(
              Builders<User>.Filter.Regex(u => u.Name, new BsonRegularExpression(searchTerm, "i")),
              Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression(searchTerm, "i"))
          );
          
          return await _users.Find(filter).Limit(10).ToListAsync();
      }
  }
  ```

- **Redis**
  - Caching strategies
  - Data structures
  - Pub/Sub messaging
  - Session storage

  ```csharp
  // Redis with StackExchange.Redis
  using StackExchange.Redis;
  
  public interface ICacheService
  {
      Task<T?> GetAsync<T>(string key);
      Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
      Task RemoveAsync(string key);
      Task<bool> ExistsAsync(string key);
  }
  
  public class RedisCacheService : ICacheService
  {
      private readonly IDatabase _database;
      private readonly ISubscriber _subscriber;
      
      public RedisCacheService(IConnectionMultiplexer redis)
      {
          _database = redis.GetDatabase();
          _subscriber = redis.GetSubscriber();
      }
      
      public async Task<T?> GetAsync<T>(string key)
      {
          var value = await _database.StringGetAsync(key);
          return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
      }
      
      public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
      {
          var serializedValue = JsonSerializer.Serialize(value);
          await _database.StringSetAsync(key, serializedValue, expiry);
      }
      
      public async Task RemoveAsync(string key)
      {
          await _database.KeyDeleteAsync(key);
      }
      
      public async Task<bool> ExistsAsync(string key)
      {
          return await _database.KeyExistsAsync(key);
      }
      
      // Redis data structures
      public async Task AddToListAsync(string key, string value)
      {
          await _database.ListLeftPushAsync(key, value);
      }
      
      public async Task<string[]> GetListAsync(string key)
      {
          var values = await _database.ListRangeAsync(key);
          return values.Select(v => v.ToString()).ToArray();
      }
      
      public async Task AddToSetAsync(string key, string value)
      {
          await _database.SetAddAsync(key, value);
      }
      
      public async Task<bool> IsInSetAsync(string key, string value)
      {
          return await _database.SetContainsAsync(key, value);
      }
      
      // Pub/Sub messaging
      public async Task PublishAsync(string channel, string message)
      {
          await _subscriber.PublishAsync(channel, message);
      }
      
      public async Task SubscribeAsync(string channel, Action<string> handler)
      {
          await _subscriber.SubscribeAsync(channel, (ch, message) => handler(message!));
      }
  }
  
  // Session storage with Redis
  public class RedisSessionService
  {
      private readonly ICacheService _cacheService;
      private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);
      
      public RedisSessionService(ICacheService cacheService)
      {
          _cacheService = cacheService;
      }
      
      public async Task<SessionData?> GetSessionAsync(string sessionId)
      {
          return await _cacheService.GetAsync<SessionData>($"session:{sessionId}");
      }
      
      public async Task SetSessionAsync(string sessionId, SessionData data)
      {
          await _cacheService.SetAsync($"session:{sessionId}", data, _sessionTimeout);
      }
      
      public async Task ExtendSessionAsync(string sessionId)
      {
          var session = await GetSessionAsync(sessionId);
          if (session != null)
          {
              await SetSessionAsync(sessionId, session);
          }
      }
  }
  ```

- **Azure Cosmos DB**
  - Multi-model database
  - Global distribution
  - Consistency levels

  ```csharp
  // Azure Cosmos DB with .NET SDK
  using Microsoft.Azure.Cosmos;
  
  public class CosmosUserRepository
  {
      private readonly Container _container;
      
      public CosmosUserRepository(CosmosClient cosmosClient, string databaseName, string containerName)
      {
          _container = cosmosClient.GetContainer(databaseName, containerName);
      }
      
      public async Task<User> CreateUserAsync(User user)
      {
          var response = await _container.CreateItemAsync(user, new PartitionKey(user.Country));
          return response.Resource;
      }
      
      public async Task<User?> GetUserAsync(string id, string country)
      {
          try
          {
              var response = await _container.ReadItemAsync<User>(id, new PartitionKey(country));
              return response.Resource;
          }
          catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
          {
              return null;
          }
      }
      
      // Query with SQL API
      public async Task<List<User>> GetUsersByCountryAsync(string country)
      {
          var query = new QueryDefinition(
              "SELECT * FROM c WHERE c.address.country = @country")
              .WithParameter("@country", country);
              
          var iterator = _container.GetItemQueryIterator<User>(query);
          var results = new List<User>();
          
          while (iterator.HasMoreResults)
          {
              var response = await iterator.ReadNextAsync();
              results.AddRange(response);
          }
          
          return results;
      }
      
      // Change feed processor
      public async Task StartChangeFeedProcessorAsync()
      {
          var changeFeedProcessor = _container
              .GetChangeFeedProcessorBuilder<User>("userProcessor", HandleChangesAsync)
              .WithInstanceName("instance1")
              .WithLeaseContainer(_container) // Using same container for leases
              .Build();
              
          await changeFeedProcessor.StartAsync();
      }
      
      private async Task HandleChangesAsync(
          IReadOnlyCollection<User> changes, 
          CancellationToken cancellationToken)
      {
          foreach (var user in changes)
          {
              Console.WriteLine($"User changed: {user.Name}");
              // Process the change (e.g., update search index, send notifications)
          }
      }
  }
  
  // Program.cs setup for Cosmos DB
  builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
  {
      var connectionString = builder.Configuration.GetConnectionString("CosmosDB");
      return new CosmosClient(connectionString, new CosmosClientOptions
      {
          ConsistencyLevel = ConsistencyLevel.Session,
          MaxRetryAttemptsOnRateLimitedRequests = 3,
          MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10)
      });
  });
  ```

### 📋 Data Modeling
- **Database Design Principles**
  - Normalization (1NF, 2NF, 3NF)
  - Denormalization strategies
  - ACID properties
  - CAP theorem

- **Entity Relationship Modeling**
  - ER diagrams
  - Cardinality
  - Primary and foreign keys
  - Composite keys

---

## Web APIs & Services

### 🚀 ASP.NET Core Web API
- **RESTful API Design**
  - HTTP verbs and status codes
  - Resource naming conventions
  - Content negotiation
  - Versioning strategies
  - HATEOAS (Hypermedia)

  ```csharp
  // RESTful API Controller example
  [ApiController]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersion("1.0")]
  [ApiVersion("2.0")]
  public class UsersController : ControllerBase
  {
      private readonly IUserService _userService;
      private readonly ILogger<UsersController> _logger;
      
      public UsersController(IUserService userService, ILogger<UsersController> logger)
      {
          _userService = userService;
          _logger = logger;
      }
      
      /// <summary>
      /// Get all users with pagination
      /// </summary>
      [HttpGet]
      [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(
          [FromQuery] int page = 1, 
          [FromQuery] int pageSize = 10,
          [FromQuery] string? search = null)
      {
          if (page < 1 || pageSize < 1 || pageSize > 100)
              return BadRequest("Invalid pagination parameters");
              
          var result = await _userService.GetUsersAsync(page, pageSize, search);
          
          // Add HATEOAS links
          var response = new PagedResult<UserDto>
          {
              Data = result.Data.Select(MapToDto).ToList(),
              Page = result.Page,
              PageSize = result.PageSize,
              TotalCount = result.TotalCount,
              Links = GeneratePaginationLinks(page, pageSize, result.TotalCount)
          };
          
          return Ok(response);
      }
      
      /// <summary>
      /// Get user by ID
      /// </summary>
      [HttpGet("{id:int}")]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<ActionResult<UserDto>> GetUser(int id)
      {
          var user = await _userService.GetUserByIdAsync(id);
          
          if (user == null)
              return NotFound($"User with ID {id} not found");
              
          return Ok(MapToDto(user));
      }
      
      /// <summary>
      /// Create a new user
      /// </summary>
      [HttpPost]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
      [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
      {
          if (!ModelState.IsValid)
              return BadRequest(ModelState);
              
          var user = await _userService.CreateUserAsync(request);
          var userDto = MapToDto(user);
          
          return CreatedAtAction(
              nameof(GetUser), 
              new { id = user.Id }, 
              userDto);
      }
      
      /// <summary>
      /// Update user - Full update
      /// </summary>
      [HttpPut("{id:int}")]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
      {
          if (!ModelState.IsValid)
              return BadRequest(ModelState);
              
          var user = await _userService.UpdateUserAsync(id, request);
          
          if (user == null)
              return NotFound($"User with ID {id} not found");
              
          return Ok(MapToDto(user));
      }
      
      /// <summary>
      /// Partial update user
      /// </summary>
      [HttpPatch("{id:int}")]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<ActionResult<UserDto>> PatchUser(int id, [FromBody] JsonPatchDocument<User> patchDoc)
      {
          var user = await _userService.GetUserByIdAsync(id);
          
          if (user == null)
              return NotFound($"User with ID {id} not found");
              
          patchDoc.ApplyTo(user, ModelState);
          
          if (!ModelState.IsValid)
              return BadRequest(ModelState);
              
          var updatedUser = await _userService.SaveUserAsync(user);
          return Ok(MapToDto(updatedUser));
      }
      
      /// <summary>
      /// Delete user
      /// </summary>
      [HttpDelete("{id:int}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> DeleteUser(int id)
      {
          var deleted = await _userService.DeleteUserAsync(id);
          
          if (!deleted)
              return NotFound($"User with ID {id} not found");
              
          return NoContent();
      }
      
      // Version-specific endpoint
      [HttpGet("profile")]
      [MapToApiVersion("2.0")]
      [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
      public async Task<ActionResult<UserProfileDto>> GetUserProfile()
      {
          // Version 2.0 specific implementation
          var userId = User.GetUserId();
          var profile = await _userService.GetUserProfileAsync(userId);
          return Ok(profile);
      }
      
      private UserDto MapToDto(User user) => new()
      {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          CreatedAt = user.CreatedAt,
          Links = new Dictionary<string, string>
          {
              ["self"] = Url.Action(nameof(GetUser), new { id = user.Id })!,
              ["update"] = Url.Action(nameof(UpdateUser), new { id = user.Id })!,
              ["delete"] = Url.Action(nameof(DeleteUser), new { id = user.Id })!
          }
      };
      
      private Dictionary<string, string> GeneratePaginationLinks(int page, int pageSize, int totalCount)
      {
          var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
          var links = new Dictionary<string, string>();
          
          if (page > 1)
              links["prev"] = Url.Action(nameof(GetUsers), new { page = page - 1, pageSize })!;
              
          if (page < totalPages)
              links["next"] = Url.Action(nameof(GetUsers), new { page = page + 1, pageSize })!;
              
          links["self"] = Url.Action(nameof(GetUsers), new { page, pageSize })!;
          
          return links;
      }
  }
  
  // DTOs and Request Models
  public class UserDto
  {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public DateTime CreatedAt { get; set; }
      public Dictionary<string, string> Links { get; set; } = new();
  }
  
  public class CreateUserRequest
  {
      [Required]
      [StringLength(100, MinimumLength = 2)]
      public string Name { get; set; } = string.Empty;
      
      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;
  }
  
  public class UpdateUserRequest
  {
      [Required]
      [StringLength(100, MinimumLength = 2)]
      public string Name { get; set; } = string.Empty;
      
      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;
  }
  
  public class PagedResult<T>
  {
      public List<T> Data { get; set; } = new();
      public int Page { get; set; }
      public int PageSize { get; set; }
      public int TotalCount { get; set; }
      public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
      public Dictionary<string, string> Links { get; set; } = new();
  }
  ```

- **API Development**
  - Controllers and actions
  - Model binding
  - Data validation
  - Response formatting
  - Custom formatters

  ```csharp
  // Custom model binding and validation
  public class CustomModelBindingController : ControllerBase
  {
      [HttpGet("search")]
      public ActionResult<List<UserDto>> SearchUsers([FromQuery] UserSearchFilter filter)
      {
          if (!ModelState.IsValid)
              return BadRequest(ModelState);
              
          // Process search
          return Ok(new List<UserDto>());
      }
      
      [HttpPost("upload")]
      public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
      {
          if (model.File == null || model.File.Length == 0)
              return BadRequest("No file uploaded");
              
          // Process file upload
          using var stream = new MemoryStream();
          await model.File.CopyToAsync(stream);
          
          return Ok(new { Message = "File uploaded successfully", Size = model.File.Length });
      }
  }
  
  public class UserSearchFilter : IValidatableObject
  {
      [FromQuery(Name = "q")]
      public string? Query { get; set; }
      
      [FromQuery]
      [Range(1, int.MaxValue)]
      public int Page { get; set; } = 1;
      
      [FromQuery]
      [Range(1, 100)]
      public int Size { get; set; } = 10;
      
      [FromQuery]
      public DateTime? From { get; set; }
      
      [FromQuery]
      public DateTime? To { get; set; }
      
      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
      {
          if (From.HasValue && To.HasValue && From > To)
          {
              yield return new ValidationResult(
                  "From date cannot be greater than To date",
                  new[] { nameof(From), nameof(To) });
          }
          
          if (string.IsNullOrEmpty(Query) && !From.HasValue && !To.HasValue)
          {
              yield return new ValidationResult(
                  "At least one search criterion must be provided",
                  new[] { nameof(Query) });
          }
      }
  }
  
  public class FileUploadModel
  {
      [Required]
      public IFormFile File { get; set; } = null!;
      
      [StringLength(255)]
      public string? Description { get; set; }
      
      [Required]
      public string Category { get; set; } = string.Empty;
  }
  
  // Custom JSON formatter with camelCase
  public class CamelCaseJsonFormatter : IOutputFormatter
  {
      private static readonly JsonSerializerOptions Options = new()
      {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          WriteIndented = true
      };
      
      public bool CanWriteResult(OutputFormatterCanWriteContext context)
      {
          return context.ContentType?.MediaType == "application/json";
      }
      
      public async Task WriteAsync(OutputFormatterWriteContext context)
      {
          var response = context.HttpContext.Response;
          var json = JsonSerializer.Serialize(context.Object, Options);
          await response.WriteAsync(json);
      }
  }
  
  // Global exception handling
  [ApiController]
  public abstract class BaseController : ControllerBase
  {
      protected IActionResult HandleError(Exception ex)
      {
          return ex switch
          {
              ValidationException validationEx => BadRequest(new
              {
                  Error = "Validation failed",
                  Details = validationEx.Message
              }),
              NotFoundException notFoundEx => NotFound(new
              {
                  Error = "Resource not found",
                  Details = notFoundEx.Message
              }),
              UnauthorizedAccessException => Unauthorized(new
              {
                  Error = "Access denied"
              }),
              _ => StatusCode(500, new
              {
                  Error = "Internal server error",
                  Details = "An unexpected error occurred"
              })
          };
      }
  }
  ```

- **OpenAPI/Swagger**
  - API documentation
  - Swagger UI
  - Custom schemas
  - Authentication in Swagger

### 🔄 API Integration Patterns
- **HTTP Clients**
  - HttpClient and HttpClientFactory
  - Polly for resilience
  - Retry policies
  - Circuit breaker pattern

- **GraphQL**
  - Schema definition
  - Queries and mutations
  - Hot Chocolate framework
  - Query optimization

- **gRPC**
  - Protocol buffers
  - Service definition
  - Streaming
  - Performance benefits

### 🎯 Microservices Architecture
- **Service Communication**
  - Synchronous vs Asynchronous
  - Message queues (RabbitMQ, Azure Service Bus)
  - Event-driven architecture
  - Saga pattern

- **Service Discovery**
  - Consul
  - Eureka
  - Azure Service Fabric

- **API Gateways**
  - Ocelot
  - Azure API Management
  - Kong
  - Load balancing

---

## Authentication & Security

### 🔒 Authentication Methods
- **Identity Framework**
  - ASP.NET Core Identity
  - User management
  - Role-based authorization
  - Claims-based identity
  - External login providers

- **JWT (JSON Web Tokens)**
  - Token structure
  - Signing and validation
  - Refresh tokens
  - Token expiration handling

- **OAuth 2.0 / OpenID Connect**
  - Authorization flows
  - Scopes and claims
  - Identity providers (Google, Microsoft, Facebook)
  - IdentityServer4/Duende IdentityServer

### 🛡️ Security Best Practices
- **Web Security**
  - HTTPS/TLS
  - CORS (Cross-Origin Resource Sharing)
  - CSRF protection
  - XSS prevention
  - SQL injection prevention
  - Content Security Policy (CSP)

- **Data Protection**
  - Encryption at rest and in transit
  - Key management
  - Secrets management (Azure Key Vault, HashiCorp Vault)
  - GDPR compliance

- **API Security**
  - Rate limiting
  - Input validation
  - API versioning
  - Security headers
  - API key management

---

## Cloud & DevOps

### ☁️ Microsoft Azure
- **Core Services**
  - Azure App Service
  - Azure Functions (Serverless)
  - Azure SQL Database
  - Azure Storage (Blob, Table, Queue)
  - Azure Key Vault
  - Azure Application Insights

- **Container Services**
  - Azure Container Instances
  - Azure Kubernetes Service (AKS)
  - Azure Container Registry

- **Integration Services**
  - Azure Service Bus
  - Azure Event Grid
  - Azure Logic Apps
  - Azure API Management

### 🐳 Containerization
- **Docker**
  - Dockerfile creation
  - Multi-stage builds
  - Container orchestration
  - Volume management
  - Networking

- **Kubernetes**
  - Pods and services
  - Deployments and replica sets
  - ConfigMaps and secrets
  - Ingress controllers
  - Helm charts

### 🔄 CI/CD Pipeline
- **Azure DevOps**
  - Azure Repos (Git)
  - Azure Pipelines (YAML)
  - Azure Boards
  - Azure Artifacts

- **GitHub Actions**
  - Workflow automation
  - Custom actions
  - Secrets management
  - Matrix builds

- **Jenkins**
  - Pipeline as code
  - Plugin ecosystem
  - Master-slave architecture

### 📊 Monitoring & Observability
- **Application Performance Monitoring**
  - Azure Application Insights
  - New Relic
  - Datadog
  - Custom telemetry

- **Logging and Metrics**
  - Centralized logging
  - Log aggregation
  - Dashboards and alerts
  - Health checks

---

## Testing

### 🧪 Unit Testing
- **Testing Frameworks**
  - xUnit
  - NUnit
  - MSTest
  - Test runner integration

- **Mocking Frameworks**
  - Moq
  - NSubstitute
  - FakeItEasy
  - Dependency injection in tests

- **Test Patterns**
  - Arrange-Act-Assert (AAA)
  - Test data builders
  - Object mothers
  - Fluent assertions

### 🔍 Integration Testing
- **ASP.NET Core Testing**
  - TestServer and WebApplicationFactory
  - In-memory databases
  - Test containers
  - Configuration in tests

- **Database Testing**
  - Entity Framework testing
  - Transaction rollback
  - Test data seeding
  - Database fixtures

### 🎭 End-to-End Testing
- **Web UI Testing**
  - Selenium WebDriver
  - Playwright
  - Cypress (for JavaScript apps)
  - Page Object Model

- **API Testing**
  - Postman/Newman
  - REST Assured
  - Custom test clients
  - Contract testing

### 📈 Test-Driven Development (TDD)
- **TDD Cycle**
  - Red-Green-Refactor
  - Test-first approach
  - Refactoring techniques
  - Code coverage metrics

---

## Performance & Optimization

### ⚡ Application Performance
- **Memory Management**
  - Garbage collection tuning
  - Memory profiling
  - Memory leaks detection
  - IDisposable pattern

- **Async Performance**
  - Async best practices
  - Thread pool optimization
  - Avoiding blocking calls
  - CancellationToken usage

- **Caching Strategies**
  - In-memory caching
  - Distributed caching (Redis)
  - Output caching
  - Cache-aside pattern
  - Cache invalidation

### 🗃️ Database Optimization
- **Query Optimization**
  - Index strategies
  - Query execution plans
  - N+1 query problem
  - Bulk operations

- **Entity Framework Performance**
  - No-tracking queries
  - Split queries
  - Batch operations
  - Connection pooling

### 🌐 Web Performance
- **Frontend Optimization**
  - Minification and bundling
  - CDN usage
  - Image optimization
  - Lazy loading
  - Code splitting

- **HTTP Performance**
  - Compression (GZIP, Brotli)
  - HTTP/2 and HTTP/3
  - Connection keep-alive
  - Caching headers

---

## Design Patterns & Architecture

### 🏛️ Architectural Patterns
- **Layered Architecture**
  - Presentation layer
  - Business logic layer
  - Data access layer
  - Cross-cutting concerns

- **Clean Architecture**
  - Dependency inversion
  - Use cases and entities
  - Infrastructure independence
  - Testable architecture

- **Domain-Driven Design (DDD)**
  - Bounded contexts
  - Aggregates and entities
  - Value objects
  - Domain events
  - Repository pattern

### 🔧 Design Patterns
- **Creational Patterns**
  - Singleton
  - Factory Method
  - Abstract Factory
  - Builder
  - Dependency Injection

- **Structural Patterns**
  - Adapter
  - Decorator
  - Facade
  - Composite
  - Proxy

- **Behavioral Patterns**
  - Observer
  - Strategy
  - Command
  - Chain of Responsibility
  - Mediator (MediatR)

### 🏗️ Enterprise Patterns
- **Data Access Patterns**
  - Repository
  - Unit of Work
  - Data Mapper
  - Active Record

- **Service Patterns**
  - Service Layer
  - Domain Services
  - Application Services
  - Anti-corruption Layer

---

## Tools & Development Environment

### 🛠️ Development Tools
- **IDEs and Editors**
  - Visual Studio 2022
  - Visual Studio Code
  - JetBrains Rider
  - Extensions and plugins

- **Version Control**
  - Git fundamentals
  - Branching strategies (Git Flow, GitHub Flow)
  - Pull requests and code reviews
  - Merge vs. rebase

### 📦 Package Management
- **NuGet**
  - Package creation and publishing
  - Package versioning
  - Private feeds
  - Dependency management

- **NPM/Yarn** (for frontend)
  - Package.json management
  - Lock files
  - Security auditing
  - Monorepo management

### 🔧 Build Tools
- **MSBuild**
  - Project file structure
  - Custom targets and tasks
  - Build configurations
  - Multi-targeting

- **.NET CLI**
  - Project templates
  - Build and publish commands
  - Tool management
  - Global tools

### 🐛 Debugging & Profiling
- **Debugging Tools**
  - Visual Studio debugger
  - Remote debugging
  - Dump analysis
  - IntelliTrace

- **Profiling Tools**
  - dotTrace
  - PerfView
  - Application Insights Profiler
  - Memory profilers

---

## Soft Skills & Best Practices

### 📚 Code Quality
- **Clean Code Principles**
  - SOLID principles
  - DRY (Don't Repeat Yourself)
  - KISS (Keep It Simple, Stupid)
  - YAGNI (You Aren't Gonna Need It)

- **Code Review Practices**
  - Review checklists
  - Constructive feedback
  - Security considerations
  - Performance implications

### 📖 Documentation
- **Technical Documentation**
  - API documentation
  - Architecture decisions (ADRs)
  - Code comments
  - README files
  - Wiki maintenance

- **Diagramming**
  - UML diagrams
  - Architecture diagrams
  - Database ER diagrams
  - Flow charts

### 🤝 Collaboration
- **Agile Methodologies**
  - Scrum framework
  - Kanban boards
  - Sprint planning
  - Retrospectives
  - Daily standups

- **Communication Skills**
  - Technical presentations
  - Cross-functional collaboration
  - Stakeholder management
  - Requirement gathering

### 📈 Continuous Learning
- **Staying Current**
  - Technology trends
  - Community involvement
  - Conference attendance
  - Open source contributions
  - Certification paths

- **Problem-Solving**
  - Analytical thinking
  - Debugging strategies
  - Research skills
  - Decision-making frameworks

---

## Advanced Topics

### 🚀 Advanced .NET Features
- **Reflection and Metadata**
  - Runtime type inspection
  - Dynamic code generation
  - Attributes and annotations
  - Expression trees

- **Interoperability**
  - P/Invoke
  - COM interop
  - Native library integration
  - Cross-platform considerations

### 🔬 Advanced Database Concepts
- **Database Administration**
  - Backup and recovery
  - High availability
  - Disaster recovery
  - Performance monitoring

- **Advanced Querying**
  - Window functions
  - CTEs (Common Table Expressions)
  - Recursive queries
  - Dynamic SQL

### 🌐 Distributed Systems
- **Scalability Patterns**
  - Load balancing
  - Database sharding
  - CQRS (Command Query Responsibility Segregation)
  - Event sourcing

- **Reliability Patterns**
  - Circuit breaker
  - Bulkhead isolation
  - Timeout patterns
  - Retry with exponential backoff

---

## Interview Preparation Categories

### 🎯 Technical Interview Focus Areas

#### **Junior Level (0-2 years)**
- C# fundamentals and OOP concepts
- Basic .NET Core knowledge
- Simple CRUD operations with Entity Framework
- Basic HTML/CSS/JavaScript
- Understanding of HTTP and REST APIs
- Git basics
- Simple unit testing

#### **Mid-Level (2-5 years)**
- Advanced C# features
- Design patterns implementation
- API design and best practices
- Database optimization
- Frontend framework proficiency
- CI/CD pipeline setup
- Integration testing
- Performance considerations

#### **Senior Level (5+ years)**
- Architecture design decisions
- System scalability and reliability
- Advanced performance optimization
- Team leadership and mentoring
- Technology evaluation and selection
- Complex problem-solving
- Cross-functional collaboration
- Strategic technical planning

### 📋 Common Interview Question Categories
1. **Coding Challenges**
   - Algorithm and data structure problems
   - System design questions
   - Architecture scenarios
   - Code review exercises

2. **Behavioral Questions**
   - Leadership examples
   - Conflict resolution
   - Project management
   - Learning and adaptation

3. **Scenario-Based Questions**
   - Performance issues
   - Security vulnerabilities
   - Scaling challenges
   - Technology choices

---

*This comprehensive topic list covers the essential knowledge areas for .NET Full Stack Development. Each topic can be explored in depth based on your experience level and career goals.*

**Last Updated:** September 2025
**Target Audience:** .NET Full Stack Developers (All Levels)