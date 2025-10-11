# Day 07: Generics, Collections & LINQ

## 🎯 Learning Objectives
By the end of today you will:
- Master generic types, constraints, and variance
- Use built-in collections effectively (List, Dictionary, HashSet, etc.)
- Write powerful LINQ queries for data manipulation
- Implement custom generic classes and methods
- Understand performance characteristics of different collections
- Create fluent APIs using method chaining

## 1. Theory: Generics & Collections

### Generic Benefits
- **Type Safety**: Compile-time type checking
- **Performance**: No boxing/unboxing for value types
- **Code Reuse**: Same logic for different types
- **IntelliSense**: Better IDE support

### Collection Performance (Big O)
| Collection | Access | Search | Insert | Delete |
|------------|--------|--------|--------|--------|
| List<T> | O(1) | O(n) | O(1)* | O(n) |
| Dictionary<K,V> | O(1) | O(1) | O(1) | O(1) |
| HashSet<T> | N/A | O(1) | O(1) | O(1) |
| SortedList<K,V> | O(log n) | O(log n) | O(n) | O(n) |
| Queue<T> | N/A | N/A | O(1) | O(1) |
| Stack<T> | O(1) | N/A | O(1) | O(1) |

*Amortized - may be O(n) when resizing

## 2. Hands-on Examples

### Example 1: Generic Classes and Constraints
Create `GenericsExample.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day07Examples
{
    // Generic class with multiple type parameters
    public class Repository<TEntity, TKey> where TEntity : class, IEntity<TKey>
                                           where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, TEntity> _entities = new();

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities[entity.Id] = entity;
        }

        public TEntity? GetById(TKey id)
        {
            _entities.TryGetValue(id, out var entity);
            return entity;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entities.Values;
        }

        public bool Remove(TKey id)
        {
            return _entities.Remove(id);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!_entities.ContainsKey(entity.Id))
                throw new InvalidOperationException($"Entity with ID {entity.Id} not found");

            _entities[entity.Id] = entity;
        }

        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return _entities.Values.Where(predicate);
        }

        public int Count => _entities.Count;
    }

    // Interface constraint
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    // Sample entities
    public class Customer : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public override string ToString() => $"Customer: {Name} ({Email})";
    }

    public class Product : IEntity<string>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public override string ToString() => $"Product: {Name} - ${Price:F2} (Stock: {StockQuantity})";
    }

    // Generic method with constraints
    public static class CollectionExtensions
    {
        public static T FindMax<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.CompareTo(max) > 0)
                    max = enumerator.Current;
            }
            return max;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> source, int step)
        {
            return source.Where((item, index) => index % step == 0);
        }
    }

    // Generic delegate example
    public delegate TResult Transformer<in T, out TResult>(T input);

    public class DataProcessor<T>
    {
        public IEnumerable<TResult> Process<TResult>(IEnumerable<T> data, Transformer<T, TResult> transformer)
        {
            return data.Select(item => transformer(item));
        }
    }

    class GenericsExample
    {
        static void Main()
        {
            // Generic repository with different key types
            var customerRepo = new Repository<Customer, int>();
            var productRepo = new Repository<Product, string>();

            // Add customers
            customerRepo.Add(new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" });
            customerRepo.Add(new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com" });

            // Add products
            productRepo.Add(new Product { Id = "LAPTOP001", Name = "Gaming Laptop", Price = 1200m, StockQuantity = 5 });
            productRepo.Add(new Product { Id = "MOUSE001", Name = "Wireless Mouse", Price = 25m, StockQuantity = 50 });

            Console.WriteLine("=== Customers ===");
            customerRepo.GetAll().ForEach(Console.WriteLine);

            Console.WriteLine("\n=== Products ===");
            productRepo.GetAll().ForEach(Console.WriteLine);

            // Generic extension methods
            var numbers = new[] { 5, 2, 8, 1, 9, 3 };
            Console.WriteLine($"\nMax number: {numbers.FindMax()}");
            Console.WriteLine("Every 2nd number:");
            numbers.TakeEvery(2).ForEach(n => Console.Write($"{n} "));

            Console.WriteLine("\n\n=== Generic Delegate Example ===");
            var processor = new DataProcessor<int>();
            var squaredNumbers = processor.Process(numbers, x => x * x);
            Console.WriteLine($"Squared: [{string.Join(", ", squaredNumbers)}]");

            var stringProcessor = new DataProcessor<Customer>();
            var customerNames = stringProcessor.Process(customerRepo.GetAll(), c => c.Name.ToUpper());
            Console.WriteLine($"Customer names: [{string.Join(", ", customerNames)}]");
        }
    }
}
```

### Example 2: Advanced Collections Usage
Create `CollectionsExample.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Day07Examples
{
    public class InventoryManager
    {
        // Different collections for different use cases
        private readonly Dictionary<string, Product> _products = new();
        private readonly HashSet<string> _categories = new();
        private readonly SortedList<decimal, string> _productsByPrice = new();
        private readonly Queue<string> _processingQueue = new();
        private readonly Stack<string> _recentActions = new();
        private readonly ConcurrentDictionary<string, int> _stockLevels = new();

        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // Dictionary for fast lookup
            _products[product.Id] = product;
            
            // HashSet for unique categories
            _categories.Add(product.Category);
            
            // SortedList for price-based queries
            _productsByPrice[product.Price] = product.Id;
            
            // Queue for processing
            _processingQueue.Enqueue(product.Id);
            
            // Stack for recent actions
            _recentActions.Push($"Added product: {product.Name}");
            
            // Concurrent dictionary for thread-safe stock tracking
            _stockLevels.TryAdd(product.Id, product.StockQuantity);

            Console.WriteLine($"Added product: {product.Name} in category: {product.Category}");
        }

        public void ProcessNextProduct()
        {
            if (_processingQueue.Count == 0)
            {
                Console.WriteLine("No products to process");
                return;
            }

            var productId = _processingQueue.Dequeue();
            if (_products.TryGetValue(productId, out var product))
            {
                Console.WriteLine($"Processing: {product.Name}");
                _recentActions.Push($"Processed: {product.Name}");
            }
        }

        public void ShowRecentActions(int count = 5)
        {
            Console.WriteLine($"\n=== Last {Math.Min(count, _recentActions.Count)} Actions ===");
            var actions = new string[Math.Min(count, _recentActions.Count)];
            
            for (int i = 0; i < actions.Length && _recentActions.Count > 0; i++)
            {
                actions[i] = _recentActions.Pop();
            }

            // Put them back (LIFO)
            for (int i = actions.Length - 1; i >= 0; i--)
            {
                _recentActions.Push(actions[i]);
                Console.WriteLine($"- {actions[i]}");
            }
        }

        public void ShowProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            Console.WriteLine($"\n=== Products between ${minPrice:F2} and ${maxPrice:F2} ===");
            
            // SortedList allows efficient range queries
            var productsInRange = _productsByPrice
                .Where(kvp => kvp.Key >= minPrice && kvp.Key <= maxPrice)
                .Select(kvp => _products[kvp.Value]);

            foreach (var product in productsInRange)
            {
                Console.WriteLine($"- {product.Name}: ${product.Price:F2}");
            }
        }

        public void ShowCategories()
        {
            Console.WriteLine($"\n=== Categories ({_categories.Count}) ===");
            foreach (var category in _categories.OrderBy(c => c))
            {
                var productCount = _products.Values.Count(p => p.Category == category);
                Console.WriteLine($"- {category} ({productCount} products)");
            }
        }

        public void UpdateStock(string productId, int newQuantity)
        {
            _stockLevels.AddOrUpdate(productId, newQuantity, (key, oldValue) => newQuantity);
            _recentActions.Push($"Updated stock for {productId}: {newQuantity}");
        }

        public void ShowLowStockProducts(int threshold = 10)
        {
            Console.WriteLine($"\n=== Low Stock Products (< {threshold}) ===");
            var lowStockProducts = _stockLevels
                .Where(kvp => kvp.Value < threshold)
                .Join(_products.Values, kvp => kvp.Key, p => p.Id, 
                     (kvp, product) => new { Product = product, Stock = kvp.Value });

            foreach (var item in lowStockProducts.OrderBy(x => x.Stock))
            {
                Console.WriteLine($"- {item.Product.Name}: {item.Stock} units");
            }
        }

        public Dictionary<string, int> GetCategoryStats()
        {
            return _products.Values
                .GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }

    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public override string ToString() => $"{Name} (${Price:F2})";
    }

    class CollectionsExample
    {
        static void Main()
        {
            var inventory = new InventoryManager();

            // Add various products
            var products = new[]
            {
                new Product { Id = "L001", Name = "Gaming Laptop", Category = "Electronics", Price = 1299.99m, StockQuantity = 5 },
                new Product { Id = "M001", Name = "Wireless Mouse", Category = "Electronics", Price = 29.99m, StockQuantity = 50 },
                new Product { Id = "K001", Name = "Mechanical Keyboard", Category = "Electronics", Price = 89.99m, StockQuantity = 25 },
                new Product { Id = "B001", Name = "Office Chair", Category = "Furniture", Price = 199.99m, StockQuantity = 8 },
                new Product { Id = "D001", Name = "Standing Desk", Category = "Furniture", Price = 399.99m, StockQuantity = 3 },
                new Product { Id = "C001", Name = "Coffee Mug", Category = "Office Supplies", Price = 12.99m, StockQuantity = 100 }
            };

            foreach (var product in products)
            {
                inventory.AddProduct(product);
            }

            // Demonstrate different collection operations
            inventory.ShowCategories();
            inventory.ShowProductsByPriceRange(20, 100);
            inventory.ShowLowStockProducts(10);

            // Process some products
            Console.WriteLine("\n=== Processing Products ===");
            for (int i = 0; i < 3; i++)
            {
                inventory.ProcessNextProduct();
            }

            // Update some stock levels
            inventory.UpdateStock("L001", 2);
            inventory.UpdateStock("D001", 1);

            inventory.ShowLowStockProducts(10);
            inventory.ShowRecentActions();

            // Category statistics
            Console.WriteLine("\n=== Category Statistics ===");
            var stats = inventory.GetCategoryStats();
            foreach (var stat in stats.OrderByDescending(kvp => kvp.Value))
            {
                Console.WriteLine($"- {stat.Key}: {stat.Value} products");
            }
        }
    }
}
```

### Example 3: LINQ Mastery
Create `LinqExample.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day07Examples
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int? ManagerId { get; set; }
        public List<string> Skills { get; set; } = new();

        public override string ToString() => $"{Name} ({Department}) - ${Salary:N0}";
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> TeamMemberIds { get; set; } = new();
        public decimal Budget { get; set; }

        public bool IsActive => EndDate == null || EndDate > DateTime.Now;
    }

    public class LinqQueryEngine
    {
        private readonly List<Employee> _employees;
        private readonly List<Project> _projects;

        public LinqQueryEngine()
        {
            _employees = GenerateEmployees();
            _projects = GenerateProjects();
        }

        public void RunBasicQueries()
        {
            Console.WriteLine("=== Basic LINQ Queries ===");

            // Filtering
            var highSalaryEmployees = _employees
                .Where(e => e.Salary > 70000)
                .OrderByDescending(e => e.Salary)
                .Take(5);

            Console.WriteLine("Top 5 highest paid employees:");
            foreach (var emp in highSalaryEmployees)
            {
                Console.WriteLine($"- {emp}");
            }

            // Projection
            var employeeSummary = _employees
                .Select(e => new 
                { 
                    e.Name, 
                    e.Department, 
                    YearsOfService = DateTime.Now.Year - e.HireDate.Year,
                    SalaryCategory = e.Salary switch
                    {
                        < 50000 => "Junior",
                        < 80000 => "Mid-level",
                        _ => "Senior"
                    }
                })
                .OrderBy(x => x.Department)
                .ThenBy(x => x.Name);

            Console.WriteLine("\nEmployee Summary:");
            foreach (var summary in employeeSummary.Take(10))
            {
                Console.WriteLine($"- {summary.Name} ({summary.Department}): {summary.YearsOfService} years, {summary.SalaryCategory}");
            }
        }

        public void RunGroupingQueries()
        {
            Console.WriteLine("\n=== Grouping and Aggregation ===");

            // Group by department
            var departmentStats = _employees
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Department = g.Key,
                    Count = g.Count(),
                    AverageSalary = g.Average(e => e.Salary),
                    MinSalary = g.Min(e => e.Salary),
                    MaxSalary = g.Max(e => e.Salary),
                    TotalSalary = g.Sum(e => e.Salary)
                })
                .OrderByDescending(x => x.AverageSalary);

            Console.WriteLine("Department Statistics:");
            foreach (var stat in departmentStats)
            {
                Console.WriteLine($"- {stat.Department}: {stat.Count} employees, " +
                                $"Avg: ${stat.AverageSalary:N0}, Range: ${stat.MinSalary:N0}-${stat.MaxSalary:N0}");
            }

            // Multi-level grouping
            var hireDateStats = _employees
                .GroupBy(e => e.HireDate.Year)
                .Where(g => g.Count() > 1)
                .Select(g => new { Year = g.Key, Count = g.Count(), Employees = g.ToList() })
                .OrderBy(x => x.Year);

            Console.WriteLine("\nHiring by Year:");
            foreach (var yearGroup in hireDateStats)
            {
                Console.WriteLine($"- {yearGroup.Year}: {yearGroup.Count} employees");
            }
        }

        public void RunJoinQueries()
        {
            Console.WriteLine("\n=== Join Operations ===");

            // Inner join - employees with their managers
            var employeeWithManager = _employees
                .Where(e => e.ManagerId.HasValue)
                .Join(_employees,
                      emp => emp.ManagerId,
                      mgr => mgr.Id,
                      (emp, mgr) => new { Employee = emp.Name, Manager = mgr.Name, emp.Department })
                .OrderBy(x => x.Department)
                .ThenBy(x => x.Employee);

            Console.WriteLine("Employees with Managers:");
            foreach (var item in employeeWithManager.Take(10))
            {
                Console.WriteLine($"- {item.Employee} reports to {item.Manager} ({item.Department})");
            }

            // Left join - all employees, with manager info if available
            var allEmployeesWithManagerInfo = _employees
                .GroupJoin(_employees,
                          emp => emp.ManagerId,
                          mgr => mgr.Id,
                          (emp, managers) => new 
                          { 
                              Employee = emp, 
                              Manager = managers.FirstOrDefault() 
                          })
                .Select(x => new 
                { 
                    EmployeeName = x.Employee.Name,
                    ManagerName = x.Manager?.Name ?? "No Manager",
                    x.Employee.Department
                });

            Console.WriteLine("\nAll Employees (with manager info):");
            foreach (var item in allEmployeesWithManagerInfo.Take(10))
            {
                Console.WriteLine($"- {item.EmployeeName} -> {item.ManagerName} ({item.Department})");
            }
        }

        public void RunAdvancedQueries()
        {
            Console.WriteLine("\n=== Advanced LINQ Operations ===");

            // Complex filtering with multiple conditions
            var experiencedDevelopers = _employees
                .Where(e => e.Department == "Engineering" || e.Department == "IT")
                .Where(e => e.Skills.Any(s => s.Contains("C#") || s.Contains("Python")))
                .Where(e => DateTime.Now.Year - e.HireDate.Year >= 3)
                .OrderByDescending(e => e.Salary);

            Console.WriteLine("Experienced Developers:");
            foreach (var dev in experiencedDevelopers.Take(5))
            {
                Console.WriteLine($"- {dev.Name}: [{string.Join(", ", dev.Skills)}]");
            }

            // Quantifiers (Any, All)
            var managersWithAllSkills = _employees
                .Where(e => !e.ManagerId.HasValue || _employees.Any(mgr => mgr.Id == e.ManagerId))
                .Where(e => e.Skills.Count >= 3)
                .Select(e => new { e.Name, SkillCount = e.Skills.Count, e.Skills });

            Console.WriteLine("\nManagers/Senior employees with multiple skills:");
            foreach (var item in managersWithAllSkills.Take(5))
            {
                Console.WriteLine($"- {item.Name}: {item.SkillCount} skills");
            }

            // Set operations
            var engineeringSkills = _employees
                .Where(e => e.Department == "Engineering")
                .SelectMany(e => e.Skills)
                .Distinct()
                .OrderBy(s => s);

            var itSkills = _employees
                .Where(e => e.Department == "IT")
                .SelectMany(e => e.Skills)
                .Distinct();

            var commonSkills = engineeringSkills.Intersect(itSkills);
            var uniqueToEngineering = engineeringSkills.Except(itSkills);

            Console.WriteLine($"\nCommon skills between Engineering and IT: [{string.Join(", ", commonSkills)}]");
            Console.WriteLine($"Skills unique to Engineering: [{string.Join(", ", uniqueToEngineering)}]");

            // Windowing functions simulation
            var salaryRankings = _employees
                .OrderByDescending(e => e.Salary)
                .Select((e, index) => new { Employee = e, Rank = index + 1 })
                .Where(x => x.Rank <= 10);

            Console.WriteLine("\nTop 10 Salary Rankings:");
            foreach (var ranking in salaryRankings)
            {
                Console.WriteLine($"{ranking.Rank}. {ranking.Employee.Name}: ${ranking.Employee.Salary:N0}");
            }
        }

        public void RunProjectQueries()
        {
            Console.WriteLine("\n=== Project Analysis ===");

            // Project statistics
            var projectStats = _projects
                .Select(p => new
                {
                    p.Name,
                    TeamSize = p.TeamMemberIds.Count,
                    Duration = p.EndDate?.Subtract(p.StartDate).Days ?? 
                              DateTime.Now.Subtract(p.StartDate).Days,
                    p.Budget,
                    Status = p.IsActive ? "Active" : "Completed"
                })
                .OrderByDescending(x => x.Budget);

            Console.WriteLine("Project Overview:");
            foreach (var project in projectStats)
            {
                Console.WriteLine($"- {project.Name}: {project.TeamSize} members, " +
                                $"{project.Duration} days, ${project.Budget:N0} ({project.Status})");
            }

            // Employee project participation
            var employeeProjectCount = _projects
                .SelectMany(p => p.TeamMemberIds.Select(id => new { ProjectId = p.Id, EmployeeId = id }))
                .GroupBy(x => x.EmployeeId)
                .Join(_employees, g => g.Key, e => e.Id, (g, e) => new { Employee = e.Name, ProjectCount = g.Count() })
                .OrderByDescending(x => x.ProjectCount);

            Console.WriteLine("\nEmployee Project Participation:");
            foreach (var participation in employeeProjectCount.Take(10))
            {
                Console.WriteLine($"- {participation.Employee}: {participation.ProjectCount} projects");
            }
        }

        private List<Employee> GenerateEmployees()
        {
            var departments = new[] { "Engineering", "IT", "Sales", "Marketing", "HR", "Finance" };
            var skills = new[] { "C#", "Python", "JavaScript", "SQL", "Docker", "Azure", "AWS", "Leadership", "Project Management", "Communication" };
            var random = new Random(42); // Fixed seed for reproducible results

            var employees = new List<Employee>();
            
            for (int i = 1; i <= 50; i++)
            {
                var employee = new Employee
                {
                    Id = i,
                    Name = $"Employee {i:D2}",
                    Department = departments[random.Next(departments.Length)],
                    Salary = random.Next(40000, 120000),
                    HireDate = DateTime.Now.AddYears(-random.Next(1, 10)).AddDays(-random.Next(0, 365)),
                    ManagerId = i > 10 ? random.Next(1, Math.Min(i, 20)) : null, // First 10 are managers
                    Skills = skills.OrderBy(x => random.Next()).Take(random.Next(2, 6)).ToList()
                };
                employees.Add(employee);
            }

            return employees;
        }

        private List<Project> GenerateProjects()
        {
            var random = new Random(42);
            var projects = new List<Project>();

            for (int i = 1; i <= 15; i++)
            {
                var startDate = DateTime.Now.AddMonths(-random.Next(1, 24));
                var isCompleted = random.NextDouble() < 0.6;
                
                var project = new Project
                {
                    Id = i,
                    Name = $"Project {i:D2}",
                    StartDate = startDate,
                    EndDate = isCompleted ? startDate.AddDays(random.Next(30, 365)) : null,
                    Budget = random.Next(50000, 500000),
                    TeamMemberIds = Enumerable.Range(1, 50)
                                             .OrderBy(x => random.Next())
                                             .Take(random.Next(3, 8))
                                             .ToList()
                };
                projects.Add(project);
            }

            return projects;
        }
    }

    class LinqExample
    {
        static void Main()
        {
            var queryEngine = new LinqQueryEngine();

            queryEngine.RunBasicQueries();
            queryEngine.RunGroupingQueries();
            queryEngine.RunJoinQueries();
            queryEngine.RunAdvancedQueries();
            queryEngine.RunProjectQueries();

            // Method chaining example
            Console.WriteLine("\n=== Method Chaining Example ===");
            var result = Enumerable.Range(1, 100)
                .Where(x => x % 2 == 0)           // Even numbers
                .Select(x => x * x)               // Square them
                .Where(x => x > 100)              // Greater than 100
                .OrderByDescending(x => x)        // Descending order
                .Take(5)                          // Top 5
                .Sum();                           // Sum them up

            Console.WriteLine($"Result of complex chain: {result}");
        }
    }
}
```

### Example 4: Custom Generic Collections
Create `CustomCollectionsExample.cs`:

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Day07Examples
{
    // Custom generic collection with fluent interface
    public class FluentList<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public FluentList<T> Add(T item)
        {
            _items.Add(item);
            return this;
        }

        public FluentList<T> AddRange(IEnumerable<T> items)
        {
            _items.AddRange(items);
            return this;
        }

        public FluentList<T> RemoveWhere(Predicate<T> predicate)
        {
            _items.RemoveAll(predicate);
            return this;
        }

        public FluentList<TResult> Transform<TResult>(Func<T, TResult> selector)
        {
            var result = new FluentList<TResult>();
            result.AddRange(_items.Select(selector));
            return result;
        }

        public FluentList<T> Filter(Func<T, bool> predicate)
        {
            var result = new FluentList<T>();
            result.AddRange(_items.Where(predicate));
            return result;
        }

        public FluentList<T> Sort(Comparison<T> comparison)
        {
            var sortedItems = _items.ToList();
            sortedItems.Sort(comparison);
            var result = new FluentList<T>();
            result.AddRange(sortedItems);
            return result;
        }

        public FluentList<T> Take(int count)
        {
            var result = new FluentList<T>();
            result.AddRange(_items.Take(count));
            return result;
        }

        public T[] ToArray() => _items.ToArray();
        public List<T> ToList() => new(_items);
        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // Thread-safe cache with expiration
    public class ExpiringCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheItem> _cache = new();
        private readonly object _lock = new object();
        private readonly TimeSpan _defaultExpiration;

        public ExpiringCache(TimeSpan defaultExpiration)
        {
            _defaultExpiration = defaultExpiration;
        }

        public void Set(TKey key, TValue value, TimeSpan? expiration = null)
        {
            var exp = expiration ?? _defaultExpiration;
            var item = new CacheItem(value, DateTime.UtcNow.Add(exp));

            lock (_lock)
            {
                _cache[key] = item;
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var item))
                {
                    if (item.ExpiresAt > DateTime.UtcNow)
                    {
                        value = item.Value;
                        return true;
                    }
                    else
                    {
                        _cache.Remove(key); // Remove expired item
                    }
                }

                value = default!;
                return false;
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory, TimeSpan? expiration = null)
        {
            if (TryGet(key, out var value))
                return value;

            value = factory(key);
            Set(key, value, expiration);
            return value;
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    // Remove expired items and return count
                    var now = DateTime.UtcNow;
                    var expiredKeys = _cache.Where(kvp => kvp.Value.ExpiresAt <= now).Select(kvp => kvp.Key).ToList();
                    foreach (var key in expiredKeys)
                    {
                        _cache.Remove(key);
                    }
                    return _cache.Count;
                }
            }
        }

        private class CacheItem
        {
            public TValue Value { get; }
            public DateTime ExpiresAt { get; }

            public CacheItem(TValue value, DateTime expiresAt)
            {
                Value = value;
                ExpiresAt = expiresAt;
            }
        }
    }

    // Generic builder pattern
    public class QueryBuilder<T>
    {
        private readonly IEnumerable<T> _source;
        private Func<T, bool>? _whereClause;
        private Func<T, object>? _orderByClause;
        private bool _descending;
        private int? _takeCount;
        private int? _skipCount;

        public QueryBuilder(IEnumerable<T> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public QueryBuilder<T> Where(Func<T, bool> predicate)
        {
            _whereClause = _whereClause == null ? predicate : item => _whereClause(item) && predicate(item);
            return this;
        }

        public QueryBuilder<T> OrderBy<TKey>(Func<T, TKey> keySelector)
        {
            _orderByClause = item => keySelector(item)!;
            _descending = false;
            return this;
        }

        public QueryBuilder<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
        {
            _orderByClause = item => keySelector(item)!;
            _descending = true;
            return this;
        }

        public QueryBuilder<T> Take(int count)
        {
            _takeCount = count;
            return this;
        }

        public QueryBuilder<T> Skip(int count)
        {
            _skipCount = count;
            return this;
        }

        public IEnumerable<T> Execute()
        {
            IEnumerable<T> query = _source;

            if (_whereClause != null)
                query = query.Where(_whereClause);

            if (_orderByClause != null)
            {
                query = _descending 
                    ? query.OrderByDescending(_orderByClause)
                    : query.OrderBy(_orderByClause);
            }

            if (_skipCount.HasValue)
                query = query.Skip(_skipCount.Value);

            if (_takeCount.HasValue)
                query = query.Take(_takeCount.Value);

            return query;
        }

        public List<T> ToList() => Execute().ToList();
        public T[] ToArray() => Execute().ToArray();
        public T First() => Execute().First();
        public T? FirstOrDefault() => Execute().FirstOrDefault();
    }

    public static class QueryBuilderExtensions
    {
        public static QueryBuilder<T> Query<T>(this IEnumerable<T> source)
        {
            return new QueryBuilder<T>(source);
        }
    }

    class CustomCollectionsExample
    {
        static void Main()
        {
            // Fluent List example
            Console.WriteLine("=== Fluent List Example ===");
            var numbers = new FluentList<int>()
                .AddRange(Enumerable.Range(1, 20))
                .Filter(x => x % 2 == 0)
                .Transform(x => x * x)
                .Sort((a, b) => b.CompareTo(a))
                .Take(5);

            Console.WriteLine($"Fluent result: [{string.Join(", ", numbers)}]");

            // Expiring Cache example
            Console.WriteLine("\n=== Expiring Cache Example ===");
            var cache = new ExpiringCache<string, string>(TimeSpan.FromSeconds(2));

            cache.Set("key1", "value1");
            cache.Set("key2", "value2", TimeSpan.FromSeconds(5));

            Console.WriteLine($"Cache count: {cache.Count}");
            Console.WriteLine($"Get key1: {cache.TryGet("key1", out var val1)} -> {val1}");
            
            // Test GetOrAdd
            var computed = cache.GetOrAdd("computed", key => $"Computed value for {key}");
            Console.WriteLine($"Computed value: {computed}");

            Console.WriteLine("Waiting 3 seconds...");
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine($"Cache count after expiration: {cache.Count}");
            Console.WriteLine($"Get key1 after expiration: {cache.TryGet("key1", out var val1b)} -> {val1b}");
            Console.WriteLine($"Get key2 after expiration: {cache.TryGet("key2", out var val2)} -> {val2}");

            // Query Builder example
            Console.WriteLine("\n=== Query Builder Example ===");
            var employees = Enumerable.Range(1, 100)
                .Select(i => new { Id = i, Name = $"Employee {i}", Salary = 30000 + (i * 1000), Department = i % 5 })
                .ToList();

            var highEarners = employees
                .Query()
                .Where(e => e.Salary > 60000)
                .Where(e => e.Department == 1 || e.Department == 2)
                .OrderByDescending(e => e.Salary)
                .Take(5)
                .ToList();

            Console.WriteLine("High earners in departments 1 and 2:");
            foreach (var emp in highEarners)
            {
                Console.WriteLine($"- {emp.Name}: ${emp.Salary:N0} (Dept: {emp.Department})");
            }

            // Performance comparison
            Console.WriteLine("\n=== Performance Comparison ===");
            var largeList = Enumerable.Range(1, 1000000).ToList();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var standardResult = largeList
                .Where(x => x % 100 == 0)
                .Select(x => x * 2)
                .OrderByDescending(x => x)
                .Take(10)
                .ToList();
            stopwatch.Stop();
            Console.WriteLine($"Standard LINQ: {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Restart();
            var fluentResult = new FluentList<int>()
                .AddRange(largeList)
                .Filter(x => x % 100 == 0)
                .Transform(x => x * 2)
                .Sort((a, b) => b.CompareTo(a))
                .Take(10)
                .ToList();
            stopwatch.Stop();
            Console.WriteLine($"Fluent List: {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Results equal: {standardResult.SequenceEqual(fluentResult)}");
        }
    }
}
```

## 3. Best Practices

### Generics
- Use meaningful constraint names
- Prefer interfaces over concrete types in constraints
- Use `in`/`out` variance keywords when appropriate
- Avoid complex generic hierarchies
- Document generic type parameters

### Collections
- Choose the right collection for your use case
- Consider thread safety requirements
- Use `IEnumerable<T>` for method parameters when possible
- Initialize collections with expected capacity when known
- Prefer composition over inheritance for custom collections

### LINQ
- Use method chaining for readability
- Avoid side effects in LINQ queries
- Consider performance implications of multiple enumerations
- Use `Any()` instead of `Count() > 0`
- Prefer `Where().Select()` over `Select().Where()` for better performance

## 4. Exercises

1. **Generic Cache System**: Build a multi-level cache with different eviction policies (LRU, LFU, TTL).

2. **Data Analysis Pipeline**: Create a fluent API for data processing with filtering, transformation, and aggregation operations.

3. **Custom LINQ Provider**: Implement a simple LINQ provider that translates expressions to SQL-like queries.

4. **Thread-Safe Collections**: Design thread-safe versions of common collections with different locking strategies.

## 5. How to Run Examples

```powershell
# Create and run examples
dotnet new console -n Day07Examples
# Copy the example files to the project
cd Day07Examples
dotnet run
cd ..

# For performance testing, run in Release mode
dotnet run --configuration Release
```

## 6. Summary

Today you mastered:
- **Generic programming** with constraints and variance
- **Collection selection** based on performance characteristics
- **LINQ queries** for powerful data manipulation
- **Custom collections** with fluent interfaces
- **Performance considerations** for different data operations

Key takeaways:
- Generics provide type safety and performance benefits
- Choose collections based on access patterns and performance needs
- LINQ enables expressive and readable data queries
- Fluent interfaces improve API usability
- Always consider performance implications of your choices
- Composition often beats inheritance for flexibility

This completes the Foundation phase! Tomorrow we'll move into the Core phase with ASP.NET Core fundamentals.