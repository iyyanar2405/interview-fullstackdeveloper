# Day 06: Exception Handling, Logging & Debugging

## 🎯 Learning Objectives
By the end of today you will:
- Master exception handling strategies and best practices
- Implement structured logging with different log levels
- Use debugging techniques and tools effectively
- Create custom exceptions for domain-specific scenarios
- Handle async exceptions and cancellation
- Implement retry patterns and circuit breakers

## 1. Theory: Exception Handling

### Exception Hierarchy
```
System.Exception
├── SystemException
│   ├── ArgumentException
│   ├── InvalidOperationException
│   ├── NotSupportedException
│   └── ...
└── ApplicationException (avoid inheriting from this)
```

### Best Practices
- Catch specific exceptions, not `Exception`
- Don't swallow exceptions silently
- Log exceptions with context
- Use custom exceptions for domain logic
- Fail fast, recover gracefully
- Clean up resources with `using` statements

## 2. Hands-on Examples

### Example 1: Exception Handling Patterns
Create `ExceptionHandlingExample.cs`:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;

namespace Day06Examples
{
    // Custom exceptions for domain logic
    public class CustomerNotFoundException : Exception
    {
        public int CustomerId { get; }

        public CustomerNotFoundException(int customerId) 
            : base($"Customer with ID {customerId} was not found")
        {
            CustomerId = customerId;
        }

        public CustomerNotFoundException(int customerId, Exception innerException)
            : base($"Customer with ID {customerId} was not found", innerException)
        {
            CustomerId = customerId;
        }
    }

    public class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal AvailableAmount { get; }

        public InsufficientFundsException(decimal requested, decimal available)
            : base($"Insufficient funds. Requested: ${requested:F2}, Available: ${available:F2}")
        {
            RequestedAmount = requested;
            AvailableAmount = available;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public Customer(int id, string name, decimal balance)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Balance = balance;
        }
    }

    public class BankService
    {
        private readonly Dictionary<int, Customer> _customers = new()
        {
            { 1, new Customer(1, "John Doe", 1000) },
            { 2, new Customer(2, "Jane Smith", 500) }
        };

        public Customer GetCustomer(int customerId)
        {
            if (!_customers.TryGetValue(customerId, out var customer))
            {
                throw new CustomerNotFoundException(customerId);
            }
            return customer;
        }

        public void Withdraw(int customerId, decimal amount)
        {
            // Input validation
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            var customer = GetCustomer(customerId); // May throw CustomerNotFoundException

            if (customer.Balance < amount)
                throw new InsufficientFundsException(amount, customer.Balance);

            customer.Balance -= amount;
            Console.WriteLine($"${amount:F2} withdrawn from {customer.Name}'s account. New balance: ${customer.Balance:F2}");
        }

        public void Transfer(int fromCustomerId, int toCustomerId, decimal amount)
        {
            try
            {
                Withdraw(fromCustomerId, amount);
                var toCustomer = GetCustomer(toCustomerId);
                toCustomer.Balance += amount;
                Console.WriteLine($"${amount:F2} transferred to {toCustomer.Name}'s account");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transfer failed: {ex.Message}");
                // In real applications, you might need to compensate/rollback
                throw; // Re-throw to let caller handle
            }
        }
    }

    public class FileProcessor
    {
        public string ReadFileContent(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
                return string.Empty;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
                throw; // Re-throw security exceptions
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error reading file: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw; // Re-throw unexpected exceptions
            }
        }

        public void ProcessFilesSafely(string[] filePaths)
        {
            var errors = new List<string>();

            foreach (var path in filePaths)
            {
                try
                {
                    var content = ReadFileContent(path);
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine($"Processed {path}: {content.Length} characters");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{path}: {ex.Message}");
                    // Continue processing other files
                }
            }

            if (errors.Any())
            {
                Console.WriteLine($"\nEncountered {errors.Count} errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"- {error}");
                }
            }
        }
    }

    class ExceptionHandlingExample
    {
        static void Main()
        {
            var bankService = new BankService();

            // Test successful operations
            try
            {
                bankService.Withdraw(1, 100);
                bankService.Transfer(1, 2, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }

            Console.WriteLine();

            // Test exception scenarios
            try
            {
                bankService.Withdraw(1, 2000); // Insufficient funds
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"Cannot withdraw: {ex.Message}");
                Console.WriteLine($"Requested: ${ex.RequestedAmount:F2}, Available: ${ex.AvailableAmount:F2}");
            }

            try
            {
                bankService.Withdraw(99, 100); // Customer not found
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message} (Customer ID: {ex.CustomerId})");
            }

            Console.WriteLine();

            // File processing example
            var fileProcessor = new FileProcessor();
            var testFiles = new[] { "existing.txt", "nonexistent.txt", "restricted.txt" };
            fileProcessor.ProcessFilesSafely(testFiles);
        }
    }
}
```

### Example 2: Structured Logging
Create `LoggingExample.cs`:

```csharp
using System;
using System.IO;
using System.Text.Json;

namespace Day06Examples
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, object>? Properties { get; set; }
    }

    public interface ILogger
    {
        void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        void Debug(string message, Dictionary<string, object>? properties = null);
        void Info(string message, Dictionary<string, object>? properties = null);
        void Warning(string message, Dictionary<string, object>? properties = null);
        void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        void Critical(string message, Exception? exception = null, Dictionary<string, object>? properties = null);
    }

    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minLevel;

        public ConsoleLogger(LogLevel minLevel = LogLevel.Info)
        {
            _minLevel = minLevel;
        }

        public void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            if (level < _minLevel) return;

            var color = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");

            if (properties != null && properties.Any())
            {
                foreach (var prop in properties)
                {
                    Console.WriteLine($"  {prop.Key}: {prop.Value}");
                }
            }

            if (exception != null)
            {
                Console.WriteLine($"  Exception: {exception.Message}");
                Console.WriteLine($"  StackTrace: {exception.StackTrace}");
            }

            Console.ResetColor();
        }

        public void Debug(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Debug, message, null, properties);
        public void Info(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Info, message, null, properties);
        public void Warning(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Warning, message, null, properties);
        public void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Error, message, exception, properties);
        public void Critical(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Critical, message, exception, properties);
    }

    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly LogLevel _minLevel;
        private readonly object _lock = new object();

        public FileLogger(string filePath, LogLevel minLevel = LogLevel.Info)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _minLevel = minLevel;

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            if (level < _minLevel) return;

            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Exception = exception?.ToString(),
                Source = Environment.MachineName,
                Properties = properties
            };

            var json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = false });

            lock (_lock)
            {
                File.AppendAllText(_filePath, json + Environment.NewLine);
            }
        }

        public void Debug(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Debug, message, null, properties);
        public void Info(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Info, message, null, properties);
        public void Warning(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Warning, message, null, properties);
        public void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Error, message, exception, properties);
        public void Critical(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Critical, message, exception, properties);
    }

    // Composite logger that writes to multiple outputs
    public class CompositeLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        public void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(level, message, exception, properties);
                }
                catch
                {
                    // Don't let logging failures break the application
                }
            }
        }

        public void Debug(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Debug, message, null, properties);
        public void Info(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Info, message, null, properties);
        public void Warning(string message, Dictionary<string, object>? properties = null) => Log(LogLevel.Warning, message, null, properties);
        public void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Error, message, exception, properties);
        public void Critical(string message, Exception? exception = null, Dictionary<string, object>? properties = null) => Log(LogLevel.Critical, message, exception, properties);
    }

    public class OrderService
    {
        private readonly ILogger _logger;

        public OrderService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ProcessOrder(int orderId, decimal amount)
        {
            _logger.Info("Processing order", new Dictionary<string, object>
            {
                { "OrderId", orderId },
                { "Amount", amount },
                { "ProcessorId", Environment.MachineName }
            });

            try
            {
                // Simulate processing
                if (amount <= 0)
                    throw new ArgumentException("Amount must be positive", nameof(amount));

                if (amount > 10000)
                {
                    _logger.Warning("Large order detected", new Dictionary<string, object>
                    {
                        { "OrderId", orderId },
                        { "Amount", amount }
                    });
                }

                // Simulate random failure
                if (Random.Shared.NextDouble() < 0.3)
                {
                    throw new InvalidOperationException("Payment processing failed");
                }

                _logger.Info("Order processed successfully", new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "Amount", amount }
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Order processing failed", ex, new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "Amount", amount }
                });
                throw;
            }
        }
    }

    class LoggingExample
    {
        static void Main()
        {
            // Setup composite logging
            var consoleLogger = new ConsoleLogger(LogLevel.Debug);
            var fileLogger = new FileLogger("logs/application.log", LogLevel.Info);
            var logger = new CompositeLogger(consoleLogger, fileLogger);

            var orderService = new OrderService(logger);

            // Test different log levels
            logger.Debug("Debug information");
            logger.Info("Application starting");
            logger.Warning("This is a warning");

            // Process some orders
            var orders = new[] { (1, 100m), (2, 15000m), (3, -50m), (4, 500m), (5, 200m) };

            foreach (var (orderId, amount) in orders)
            {
                try
                {
                    orderService.ProcessOrder(orderId, amount);
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to process order {orderId}", ex);
                }
            }

            logger.Info("Application finished");
        }
    }
}
```

### Example 3: Async Exception Handling & Cancellation
Create `AsyncExceptionExample.cs`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Day06Examples
{
    public class ApiService
    {
        private readonly ILogger _logger;

        public ApiService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> FetchDataAsync(string url, CancellationToken cancellationToken = default)
        {
            _logger.Info($"Fetching data from {url}");

            try
            {
                // Simulate network delay
                await Task.Delay(Random.Shared.Next(1000, 3000), cancellationToken);

                // Simulate random failures
                if (Random.Shared.NextDouble() < 0.3)
                {
                    throw new HttpRequestException($"Failed to fetch data from {url}");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var result = $"Data from {url} - {DateTime.Now:HH:mm:ss}";
                _logger.Info($"Successfully fetched data from {url}");
                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.Warning($"Request to {url} was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching data from {url}", ex);
                throw;
            }
        }

        public async Task<string[]> FetchMultipleDataAsync(string[] urls, CancellationToken cancellationToken = default)
        {
            var tasks = urls.Select(url => FetchDataAsync(url, cancellationToken)).ToArray();

            try
            {
                return await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.Error("One or more fetch operations failed", ex);
                
                // Check individual task results
                var results = new List<string>();
                for (int i = 0; i < tasks.Length; i++)
                {
                    try
                    {
                        results.Add(await tasks[i]);
                    }
                    catch (Exception taskEx)
                    {
                        _logger.Error($"Task {i} (URL: {urls[i]}) failed", taskEx);
                        results.Add($"ERROR: {taskEx.Message}");
                    }
                }
                return results.ToArray();
            }
        }
    }

    public class RetryPolicy
    {
        private readonly ILogger _logger;

        public RetryPolicy(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            delay ??= TimeSpan.FromSeconds(1);
            var attempt = 0;

            while (true)
            {
                attempt++;
                try
                {
                    var result = await operation();
                    if (attempt > 1)
                    {
                        _logger.Info($"Operation succeeded on attempt {attempt}");
                    }
                    return result;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    _logger.Warning($"Operation failed on attempt {attempt}, retrying in {delay.Value.TotalSeconds}s", null, new Dictionary<string, object>
                    {
                        { "Attempt", attempt },
                        { "MaxRetries", maxRetries },
                        { "Exception", ex.Message }
                    });

                    await Task.Delay(delay.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Operation failed after {maxRetries} attempts", ex);
                    throw;
                }
            }
        }
    }

    class AsyncExceptionExample
    {
        static async Task Main()
        {
            var logger = new ConsoleLogger(LogLevel.Debug);
            var apiService = new ApiService(logger);
            var retryPolicy = new RetryPolicy(logger);

            // Test cancellation
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            try
            {
                // Single request with retry
                var result = await retryPolicy.ExecuteWithRetryAsync(
                    () => apiService.FetchDataAsync("https://api.example.com/data"),
                    maxRetries: 3,
                    delay: TimeSpan.FromSeconds(1),
                    cts.Token
                );
                Console.WriteLine($"Result: {result}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }

            Console.WriteLine();

            // Multiple concurrent requests
            var urls = new[]
            {
                "https://api.example.com/users",
                "https://api.example.com/products",
                "https://api.example.com/orders",
                "https://api.example.com/analytics"
            };

            using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                var results = await apiService.FetchMultipleDataAsync(urls, cts2.Token);
                for (int i = 0; i < results.Length; i++)
                {
                    Console.WriteLine($"{urls[i]}: {results[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Batch operation failed: {ex.Message}");
            }

            // Demonstrate fire-and-forget with proper exception handling
            var fireAndForgetTask = Task.Run(async () =>
            {
                try
                {
                    await apiService.FetchDataAsync("https://api.example.com/background");
                }
                catch (Exception ex)
                {
                    logger.Error("Background task failed", ex);
                }
            });

            Console.WriteLine("Fire-and-forget task started");
            
            // Don't wait for fire-and-forget tasks in real applications
            // This is just for demonstration
            await fireAndForgetTask;
        }
    }
}
```

### Example 4: Debugging and Diagnostics
Create `DebuggingExample.cs`:

```csharp
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Day06Examples
{
    public class PerformanceProfiler : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _operationName;
        private readonly ILogger _logger;

        public PerformanceProfiler(string operationName, ILogger logger)
        {
            _operationName = operationName;
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
            _logger.Debug($"Started: {_operationName}");
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var elapsed = _stopwatch.ElapsedMilliseconds;
            
            var logLevel = elapsed switch
            {
                > 5000 => LogLevel.Warning,
                > 1000 => LogLevel.Info,
                _ => LogLevel.Debug
            };

            _logger.Log(logLevel, $"Completed: {_operationName}", null, new Dictionary<string, object>
            {
                { "ElapsedMs", elapsed },
                { "ElapsedTicks", _stopwatch.ElapsedTicks }
            });
        }
    }

    public class DiagnosticService
    {
        private readonly ILogger _logger;

        public DiagnosticService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ProcessData(int[] data, [CallerMemberName] string? callerName = null, [CallerFilePath] string? callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            _logger.Debug($"Method called by {callerName} at {Path.GetFileName(callerPath)}:{callerLine}");

            using var profiler = new PerformanceProfiler($"ProcessData({data?.Length ?? 0} items)", _logger);

            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (data.Length == 0)
                {
                    _logger.Warning("Empty data array provided");
                    return;
                }

                // Simulate processing with conditional breakpoint opportunity
                var sum = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    sum += data[i];
                    
                    // Debug assertion (only in debug builds)
                    Debug.Assert(sum >= 0, $"Sum became negative at index {i}");
                    
                    // Conditional compilation for debug logging
                    #if DEBUG
                    if (i % 1000 == 0)
                    {
                        _logger.Debug($"Processed {i} items, current sum: {sum}");
                    }
                    #endif
                }

                _logger.Info($"Processed {data.Length} items, total sum: {sum}");
            }
            catch (Exception ex)
            {
                _logger.Error("Error processing data", ex, new Dictionary<string, object>
                {
                    { "DataLength", data?.Length ?? 0 },
                    { "CallerName", callerName ?? "Unknown" },
                    { "CallerPath", callerPath ?? "Unknown" },
                    { "CallerLine", callerLine }
                });
                throw;
            }
        }

        public void DemonstrateDebuggingFeatures()
        {
            _logger.Info("Demonstrating debugging features");

            // Trace listeners (useful for debugging)
            Trace.WriteLine("This goes to trace output");
            
            // Debug vs Release behavior
            #if DEBUG
            _logger.Debug("Running in DEBUG mode");
            #else
            _logger.Info("Running in RELEASE mode");
            #endif

            // Stack trace example
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            
            _logger.Debug("Current stack trace:");
            foreach (var frame in frames.Take(3))
            {
                var method = frame.GetMethod();
                var fileName = frame.GetFileName();
                var lineNumber = frame.GetFileLineNumber();
                
                _logger.Debug($"  {method?.DeclaringType?.Name}.{method?.Name} at {Path.GetFileName(fileName)}:{lineNumber}");
            }

            // Memory usage
            var memoryBefore = GC.GetTotalMemory(false);
            var largeArray = new int[1000000]; // Allocate some memory
            var memoryAfter = GC.GetTotalMemory(false);
            
            _logger.Info($"Memory usage increased by {memoryAfter - memoryBefore:N0} bytes");
            
            // Force garbage collection (don't do this in production!)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var memoryAfterGC = GC.GetTotalMemory(true);
            
            _logger.Info($"Memory after GC: {memoryAfterGC:N0} bytes");
        }
    }

    class DebuggingExample
    {
        static void Main()
        {
            var logger = new ConsoleLogger(LogLevel.Debug);
            var diagnosticService = new DiagnosticService(logger);

            // Test normal operation
            var testData = Enumerable.Range(1, 10000).ToArray();
            diagnosticService.ProcessData(testData);

            Console.WriteLine();

            // Test error conditions
            try
            {
                diagnosticService.ProcessData(null!);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }

            // Test empty array
            diagnosticService.ProcessData(Array.Empty<int>());

            Console.WriteLine();

            // Demonstrate debugging features
            diagnosticService.DemonstrateDebuggingFeatures();

            // Performance monitoring example
            using (var profiler = new PerformanceProfiler("Complex calculation", logger))
            {
                // Simulate complex calculation
                Thread.Sleep(Random.Shared.Next(100, 2000));
                var result = testData.Sum();
                logger.Info($"Calculation result: {result}");
            }
        }
    }
}
```

## 3. Best Practices

### Exception Handling
- Catch specific exceptions, not generic `Exception`
- Use custom exceptions for domain-specific errors
- Include relevant context in exception messages
- Don't swallow exceptions silently
- Use `finally` blocks or `using` statements for cleanup
- Log exceptions with sufficient detail for troubleshooting

### Logging
- Use structured logging with properties
- Include correlation IDs for distributed systems
- Log at appropriate levels (Debug, Info, Warning, Error, Critical)
- Don't log sensitive information (passwords, tokens)
- Use async logging for high-throughput applications
- Configure different log levels for different environments

### Debugging
- Use conditional compilation for debug-only code
- Add meaningful debug assertions
- Use caller information attributes for context
- Profile performance-critical code
- Monitor memory usage in long-running applications

## 4. Exercises

1. **Robust File Processor**: Create a file processing service that handles various file system exceptions and logs operations with structured data.

2. **HTTP Client with Retry**: Build an HTTP client wrapper with exponential backoff retry logic and comprehensive logging.

3. **Database Service**: Create a database service with connection retry, timeout handling, and transaction rollback on errors.

4. **Event Processing Pipeline**: Design an event processing system that handles exceptions gracefully and maintains processing continuity.

## 5. How to Run Examples

```powershell
# Create and run examples
dotnet new console -n Day06Examples
# Copy the example files and run
cd Day06Examples
dotnet run
cd ..

# Check the generated log files
type logs\application.log
```

## 6. Summary

Today you learned:
- **Exception handling** patterns and custom exception design
- **Structured logging** with multiple outputs and log levels
- **Async exception handling** with cancellation support
- **Debugging techniques** and performance profiling
- **Retry patterns** and resilience strategies

Key takeaways:
- Handle exceptions at the right level of abstraction
- Use structured logging for better observability
- Always handle cancellation in async operations
- Profile performance-critical code paths
- Log enough context to troubleshoot issues effectively
- Design for failure - expect things to go wrong

Tomorrow: Day 07 — Generics, Collections & LINQ