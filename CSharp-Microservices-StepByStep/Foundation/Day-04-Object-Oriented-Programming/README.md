# Day 04: Object-Oriented Programming (OOP)

## 🎯 Learning Objectives
By the end of today you will:
- Understand core OOP principles: Encapsulation, Inheritance, Polymorphism, Abstraction
- Create classes with fields, properties, constructors, and methods
- Implement inheritance and method overriding
- Use interfaces to define contracts
- Apply access modifiers and understand visibility
- Learn when to use abstract classes vs interfaces

## 1. Theory: OOP Fundamentals

### Four Pillars of OOP
1. **Encapsulation**: Bundle data and methods together, control access via access modifiers
2. **Inheritance**: Create new classes based on existing ones (`is-a` relationship)
3. **Polymorphism**: Same interface, different implementations (method overriding, interfaces)
4. **Abstraction**: Hide implementation details, expose only essential features

### Access Modifiers
- `public`: Accessible everywhere
- `private`: Only within the same class
- `protected`: Within class and derived classes
- `internal`: Within the same assembly
- `protected internal`: Protected OR internal

## 2. Hands-on Examples

### Example 1: Basic Class with Encapsulation
Create `PersonClass.cs`:

```csharp
using System;

namespace Day04Examples
{
    // Basic class demonstrating encapsulation
    public class Person
    {
        // Private fields (encapsulated data)
        private string _firstName;
        private string _lastName;
        private int _age;

        // Constructor
        public Person(string firstName, string lastName, int age)
        {
            _firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            _lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Age = age; // Use property to validate
        }

        // Properties with validation (controlled access)
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value < 0 || value > 150)
                    throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 150");
                _age = value;
            }
        }

        // Computed property
        public string FullName => $"{FirstName} {LastName}";

        // Methods
        public void Introduce()
        {
            Console.WriteLine($"Hello, I'm {FullName} and I'm {Age} years old.");
        }

        public virtual void Work()
        {
            Console.WriteLine($"{FullName} is working.");
        }

        // Override ToString for better object representation
        public override string ToString() => $"Person: {FullName}, Age: {Age}";
    }

    class PersonExample
    {
        static void Main()
        {
            try
            {
                var person = new Person("John", "Doe", 30);
                person.Introduce();
                person.Work();
                
                Console.WriteLine($"Full name: {person.FullName}");
                Console.WriteLine($"ToString: {person}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
```

### Example 2: Inheritance and Polymorphism
Create `InheritanceExample.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day04Examples
{
    // Base class
    public class Employee : Person
    {
        public string Department { get; set; }
        public decimal Salary { get; set; }

        public Employee(string firstName, string lastName, int age, string department, decimal salary)
            : base(firstName, lastName, age)
        {
            Department = department ?? throw new ArgumentNullException(nameof(department));
            Salary = salary;
        }

        // Override base method (polymorphism)
        public override void Work()
        {
            Console.WriteLine($"{FullName} is working in {Department} department.");
        }

        public virtual decimal CalculateBonus()
        {
            return Salary * 0.1m; // 10% bonus
        }
    }

    // Derived class
    public class Manager : Employee
    {
        public int TeamSize { get; set; }

        public Manager(string firstName, string lastName, int age, string department, decimal salary, int teamSize)
            : base(firstName, lastName, age, department, salary)
        {
            TeamSize = teamSize;
        }

        // Override Work method
        public override void Work()
        {
            Console.WriteLine($"{FullName} is managing {TeamSize} people in {Department}.");
        }

        // Override bonus calculation
        public override decimal CalculateBonus()
        {
            return base.CalculateBonus() + (TeamSize * 1000); // Extra bonus per team member
        }

        public void HoldMeeting()
        {
            Console.WriteLine($"{FullName} is holding a team meeting.");
        }
    }

    // Another derived class
    public class Developer : Employee
    {
        public string ProgrammingLanguage { get; set; }

        public Developer(string firstName, string lastName, int age, string department, decimal salary, string language)
            : base(firstName, lastName, age, department, salary)
        {
            ProgrammingLanguage = language ?? "C#";
        }

        public override void Work()
        {
            Console.WriteLine($"{FullName} is coding in {ProgrammingLanguage}.");
        }

        public void DeployCode()
        {
            Console.WriteLine($"{FullName} is deploying code to production.");
        }
    }

    class InheritanceExample
    {
        static void Main()
        {
            // Polymorphism: same reference type, different behaviors
            var employees = new List<Employee>
            {
                new Manager("Alice", "Johnson", 35, "Engineering", 80000, 5),
                new Developer("Bob", "Smith", 28, "Engineering", 70000, "C#"),
                new Employee("Charlie", "Brown", 32, "HR", 60000)
            };

            foreach (var emp in employees)
            {
                emp.Work(); // Calls overridden method
                Console.WriteLine($"Bonus: ${emp.CalculateBonus():F2}");
                Console.WriteLine();
            }

            // Type checking and casting
            foreach (var emp in employees)
            {
                if (emp is Manager manager)
                {
                    manager.HoldMeeting();
                }
                else if (emp is Developer dev)
                {
                    dev.DeployCode();
                }
            }
        }
    }
}
```

### Example 3: Interfaces and Multiple Inheritance
Create `InterfaceExample.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day04Examples
{
    // Interface defining a contract
    public interface IPayable
    {
        decimal CalculatePay();
        void ProcessPayment();
    }

    public interface IReportable
    {
        string GenerateReport();
    }

    // Interface with default implementation (C# 8+)
    public interface INotifiable
    {
        void SendNotification(string message);
        
        // Default implementation
        void SendEmail(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }

    // Class implementing multiple interfaces
    public class Contractor : Person, IPayable, IReportable, INotifiable
    {
        public decimal HourlyRate { get; set; }
        public int HoursWorked { get; set; }

        public Contractor(string firstName, string lastName, int age, decimal hourlyRate)
            : base(firstName, lastName, age)
        {
            HourlyRate = hourlyRate;
        }

        // Implement IPayable
        public decimal CalculatePay()
        {
            return HourlyRate * HoursWorked;
        }

        public void ProcessPayment()
        {
            Console.WriteLine($"Processing payment of ${CalculatePay():F2} for {FullName}");
        }

        // Implement IReportable
        public string GenerateReport()
        {
            return $"Contractor: {FullName}, Hours: {HoursWorked}, Rate: ${HourlyRate:F2}, Total: ${CalculatePay():F2}";
        }

        // Implement INotifiable
        public void SendNotification(string message)
        {
            Console.WriteLine($"SMS to {FullName}: {message}");
        }

        public override void Work()
        {
            Console.WriteLine($"{FullName} is working as a contractor (${HourlyRate}/hour).");
            HoursWorked++;
        }
    }

    // Abstract class (cannot be instantiated)
    public abstract class Shape
    {
        public abstract double CalculateArea();
        public abstract double CalculatePerimeter();

        // Concrete method in abstract class
        public virtual void Display()
        {
            Console.WriteLine($"Area: {CalculateArea():F2}, Perimeter: {CalculatePerimeter():F2}");
        }
    }

    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override double CalculateArea()
        {
            return Math.PI * Radius * Radius;
        }

        public override double CalculatePerimeter()
        {
            return 2 * Math.PI * Radius;
        }

        public override void Display()
        {
            Console.WriteLine($"Circle - Radius: {Radius:F2}");
            base.Display();
        }
    }

    class InterfaceExample
    {
        static void Main()
        {
            var contractor = new Contractor("Jane", "Doe", 30, 50.0m);
            contractor.Work();
            contractor.Work();
            contractor.Work();

            // Using interface references
            IPayable payable = contractor;
            IReportable reportable = contractor;
            INotifiable notifiable = contractor;

            payable.ProcessPayment();
            Console.WriteLine(reportable.GenerateReport());
            notifiable.SendNotification("Your payment has been processed");
            notifiable.SendEmail("Payment confirmation sent via email");

            Console.WriteLine();

            // Abstract class example
            Shape circle = new Circle(5.0);
            circle.Display();
        }
    }
}
```

### Example 4: Advanced OOP Patterns
Create `AdvancedOOPExample.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day04Examples
{
    // Static class (cannot be instantiated)
    public static class MathUtils
    {
        public static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }
    }

    // Sealed class (cannot be inherited)
    public sealed class Configuration
    {
        private static Configuration? _instance;
        private static readonly object _lock = new object();

        private Configuration() { }

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new Configuration();
                    }
                }
                return _instance;
            }
        }

        public string DatabaseConnection { get; set; } = "DefaultConnection";
        public int MaxRetries { get; set; } = 3;
    }

    // Generic class
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.AsReadOnly();
        }

        public T? Find(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }
    }

    // Record (C# 9+) - immutable reference type
    public record PersonRecord(string FirstName, string LastName, int Age)
    {
        public string FullName => $"{FirstName} {LastName}";
    }

    class AdvancedOOPExample
    {
        static void Main()
        {
            // Static class usage
            Console.WriteLine($"Distance: {MathUtils.CalculateDistance(0, 0, 3, 4)}");
            Console.WriteLine($"Max: {MathUtils.Max(10, 20)}");

            // Singleton pattern
            var config = Configuration.Instance;
            config.DatabaseConnection = "Server=localhost;Database=MyDB";
            Console.WriteLine($"DB Connection: {config.DatabaseConnection}");

            // Generic repository
            var personRepo = new Repository<PersonRecord>();
            personRepo.Add(new PersonRecord("John", "Doe", 30));
            personRepo.Add(new PersonRecord("Jane", "Smith", 25));

            var person = personRepo.Find(p => p.FirstName == "John");
            Console.WriteLine($"Found: {person?.FullName}");

            // Record equality
            var person1 = new PersonRecord("Alice", "Johnson", 28);
            var person2 = new PersonRecord("Alice", "Johnson", 28);
            Console.WriteLine($"Records equal: {person1 == person2}"); // True

            // Record with modification
            var person3 = person1 with { Age = 29 };
            Console.WriteLine($"Modified record: {person3}");
        }
    }
}
```

## 3. Best Practices

### Class Design
- Keep classes focused (Single Responsibility Principle)
- Prefer composition over inheritance
- Use interfaces to define contracts
- Make fields private, expose via properties
- Validate inputs in constructors and property setters

### Inheritance Guidelines
- Use inheritance for "is-a" relationships
- Prefer interfaces for "can-do" relationships
- Mark methods as `virtual` if they should be overridable
- Use `sealed` to prevent further inheritance when appropriate

### Interface Design
- Keep interfaces small and focused
- Use descriptive names (IDisposable, IComparable)
- Consider default implementations for backward compatibility

## 4. Exercises

1. **Bank Account System**: Create a base `Account` class and derive `CheckingAccount` and `SavingsAccount` with different interest calculations.

2. **Shape Calculator**: Implement an abstract `Shape` class and concrete classes (`Rectangle`, `Triangle`, `Circle`) with area/perimeter calculations.

3. **Plugin System**: Create an `IPlugin` interface and implement multiple plugins. Create a `PluginManager` that can load and execute plugins.

4. **Vehicle Hierarchy**: Design a vehicle system with `Vehicle` base class, `Car`/`Motorcycle` derived classes, and interfaces like `IElectric`, `IGasoline`.

## 5. How to Run Examples

Open PowerShell in this folder and run:

```powershell
# For PersonClass example
dotnet new console -n PersonExample
copy .\PersonClass.cs PersonExample\Program.cs
cd PersonExample
dotnet run
cd ..
rmdir /s /q PersonExample

# For other examples, replace the .cs file name
```

Or create a single console project and test multiple classes:

```powershell
dotnet new console -n OOPExamples
# Copy all .cs files to the project and modify namespaces/Main methods as needed
cd OOPExamples
dotnet run
```

## 6. Summary

Today you learned the four pillars of OOP and how to apply them in C#:
- **Encapsulation**: Control access to data through properties and methods
- **Inheritance**: Build new classes based on existing ones
- **Polymorphism**: Same interface, different implementations
- **Abstraction**: Hide complexity behind simple interfaces

Key takeaways:
- Classes encapsulate data and behavior
- Inheritance creates "is-a" relationships
- Interfaces define contracts for "can-do" capabilities
- Proper access modifiers protect your data
- Abstract classes provide partial implementations
- Records offer immutable data structures

Tomorrow: Day 05 — Interfaces, Abstract Classes, and SOLID Principles