# Day 05: Interfaces, Abstract Classes & SOLID Principles

## 🎯 Learning Objectives
By the end of today you will:
- Master interface design and implementation patterns
- Understand when to use abstract classes vs interfaces
- Learn the SOLID principles and apply them in code
- Implement dependency injection patterns
- Create loosely coupled, testable code
- Design extensible architectures

## 1. Theory: Advanced OOP Concepts

### Interfaces vs Abstract Classes
| Interface | Abstract Class |
|-----------|----------------|
| Contract only (what) | Partial implementation (what + how) |
| Multiple inheritance | Single inheritance |
| No fields (only properties) | Can have fields |
| No constructors | Can have constructors |
| All members public | Any access modifier |

### SOLID Principles
1. **S**ingle Responsibility Principle (SRP)
2. **O**pen/Closed Principle (OCP)
3. **L**iskov Substitution Principle (LSP)
4. **I**nterface Segregation Principle (ISP)
5. **D**ependency Inversion Principle (DIP)

## 2. Hands-on Examples

### Example 1: Interface Segregation Principle (ISP)
Create `InterfaceSegregationExample.cs`:

```csharp
using System;

namespace Day05Examples
{
    // BAD: Fat interface that violates ISP
    public interface IBadWorker
    {
        void Work();
        void Eat();
        void Sleep();
        void Program(); // Not all workers program!
    }

    // GOOD: Segregated interfaces
    public interface IWorkable
    {
        void Work();
    }

    public interface IFeedable
    {
        void Eat();
    }

    public interface ISleepable
    {
        void Sleep();
    }

    public interface IProgrammable
    {
        void Program();
    }

    // Human worker implements all interfaces
    public class HumanWorker : IWorkable, IFeedable, ISleepable
    {
        public string Name { get; }

        public HumanWorker(string name)
        {
            Name = name;
        }

        public void Work()
        {
            Console.WriteLine($"{Name} is working");
        }

        public void Eat()
        {
            Console.WriteLine($"{Name} is eating");
        }

        public void Sleep()
        {
            Console.WriteLine($"{Name} is sleeping");
        }
    }

    // Robot worker only implements relevant interfaces
    public class RobotWorker : IWorkable, IProgrammable
    {
        public string Model { get; }

        public RobotWorker(string model)
        {
            Model = model;
        }

        public void Work()
        {
            Console.WriteLine($"Robot {Model} is working");
        }

        public void Program()
        {
            Console.WriteLine($"Robot {Model} is being programmed");
        }
    }

    // Developer implements work and programming
    public class Developer : IWorkable, IFeedable, ISleepable, IProgrammable
    {
        public string Name { get; }

        public Developer(string name)
        {
            Name = name;
        }

        public void Work()
        {
            Console.WriteLine($"{Name} is analyzing requirements");
        }

        public void Program()
        {
            Console.WriteLine($"{Name} is writing code");
        }

        public void Eat()
        {
            Console.WriteLine($"{Name} is having coffee and snacks");
        }

        public void Sleep()
        {
            Console.WriteLine($"{Name} is taking a power nap");
        }
    }

    class InterfaceSegregationExample
    {
        static void Main()
        {
            var human = new HumanWorker("Alice");
            var robot = new RobotWorker("R2D2");
            var dev = new Developer("Bob");

            // Each worker can do what it's designed for
            ManageWorkable(human);
            ManageWorkable(robot);
            ManageWorkable(dev);

            Console.WriteLine();

            ManageProgrammable(robot);
            ManageProgrammable(dev);
            // ManageProgrammable(human); // Won't compile - good!
        }

        static void ManageWorkable(IWorkable worker)
        {
            worker.Work();
        }

        static void ManageProgrammable(IProgrammable programmer)
        {
            programmer.Program();
        }
    }
}
```

### Example 2: Dependency Inversion Principle (DIP)
Create `DependencyInversionExample.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq

namespace Day05Examples
{
    // BAD: High-level module depends on low-level module
    public class BadEmailService
    {
        public void SendEmail(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }

    public class BadNotificationService
    {
        private readonly BadEmailService _emailService; // Direct dependency!

        public BadNotificationService()
        {
            _emailService = new BadEmailService(); // Tight coupling!
        }

        public void SendNotification(string message)
        {
            _emailService.SendEmail(message);
        }
    }

    // GOOD: Use abstraction (DIP)
    public interface IMessageSender
    {
        void SendMessage(string recipient, string message);
        string SenderType { get; }
    }

    public class EmailSender : IMessageSender
    {
        public string SenderType => "Email";

        public void SendMessage(string recipient, string message)
        {
            Console.WriteLine($"📧 Email to {recipient}: {message}");
        }
    }

    public class SmsSender : IMessageSender
    {
        public string SenderType => "SMS";

        public void SendMessage(string recipient, string message)
        {
            Console.WriteLine($"📱 SMS to {recipient}: {message}");
        }
    }

    public class SlackSender : IMessageSender
    {
        public string SenderType => "Slack";

        public void SendMessage(string recipient, string message)
        {
            Console.WriteLine($"💬 Slack to {recipient}: {message}");
        }
    }

    // High-level module depends on abstraction
    public class NotificationService
    {
        private readonly IEnumerable<IMessageSender> _messageSenders;

        public NotificationService(IEnumerable<IMessageSender> messageSenders)
        {
            _messageSenders = messageSenders ?? throw new ArgumentNullException(nameof(messageSenders));
        }

        public void SendNotification(string recipient, string message)
        {
            foreach (var sender in _messageSenders)
            {
                sender.SendMessage(recipient, message);
            }
        }

        public void SendNotification(string recipient, string message, string senderType)
        {
            var sender = _messageSenders.FirstOrDefault(s => s.SenderType.Equals(senderType, StringComparison.OrdinalIgnoreCase));
            if (sender != null)
            {
                sender.SendMessage(recipient, message);
            }
            else
            {
                Console.WriteLine($"No sender found for type: {senderType}");
            }
        }
    }

    class DependencyInversionExample
    {
        static void Main()
        {
            // Dependency injection - we control the dependencies
            var messageSenders = new List<IMessageSender>
            {
                new EmailSender(),
                new SmsSender(),
                new SlackSender()
            };

            var notificationService = new NotificationService(messageSenders);

            // Send via all channels
            notificationService.SendNotification("john@example.com", "Welcome to our service!");

            Console.WriteLine();

            // Send via specific channel
            notificationService.SendNotification("john@example.com", "Password reset link", "Email");
            notificationService.SendNotification("+1234567890", "Your OTP is 123456", "SMS");
        }
    }
}
```

### Example 3: Single Responsibility & Open/Closed Principles
Create `SolidPrinciplesExample.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day05Examples
{
    // Single Responsibility: Each class has one reason to change
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal CreditLimit { get; set; }

        public Customer(int id, string name, string email, decimal creditLimit)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            CreditLimit = creditLimit;
        }
    }

    // SRP: Only handles customer data persistence
    public interface ICustomerRepository
    {
        void Save(Customer customer);
        Customer GetById(int id);
        IEnumerable<Customer> GetAll();
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new();

        public void Save(Customer customer)
        {
            var existing = _customers.Find(c => c.Id == customer.Id);
            if (existing != null)
            {
                _customers.Remove(existing);
            }
            _customers.Add(customer);
            Console.WriteLine($"Customer {customer.Name} saved to database");
        }

        public Customer GetById(int id)
        {
            return _customers.Find(c => c.Id == id) ?? throw new InvalidOperationException($"Customer {id} not found");
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customers.AsReadOnly();
        }
    }

    // SRP: Only handles customer business rules
    public class CustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void CreateCustomer(string name, string email, decimal creditLimit)
        {
            // Business rule validation
            if (creditLimit < 0)
                throw new ArgumentException("Credit limit cannot be negative");

            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format");

            var customer = new Customer(
                id: GenerateId(),
                name: name,
                email: email,
                creditLimit: creditLimit
            );

            _repository.Save(customer);
        }

        public bool CanPlaceOrder(int customerId, decimal orderAmount)
        {
            var customer = _repository.GetById(customerId);
            return orderAmount <= customer.CreditLimit;
        }

        private static int GenerateId() => Random.Shared.Next(1000, 9999);
    }

    // Open/Closed Principle: Open for extension, closed for modification
    public abstract class DiscountCalculator
    {
        public abstract decimal CalculateDiscount(decimal amount, Customer customer);
    }

    public class RegularDiscountCalculator : DiscountCalculator
    {
        public override decimal CalculateDiscount(decimal amount, Customer customer)
        {
            return amount * 0.05m; // 5% discount
        }
    }

    public class PremiumDiscountCalculator : DiscountCalculator
    {
        public override decimal CalculateDiscount(decimal amount, Customer customer)
        {
            return amount * 0.15m; // 15% discount for premium customers
        }
    }

    public class VipDiscountCalculator : DiscountCalculator
    {
        public override decimal CalculateDiscount(decimal amount, Customer customer)
        {
            var baseDiscount = amount * 0.20m; // 20% base discount
            var bonusDiscount = customer.CreditLimit > 10000 ? amount * 0.05m : 0; // Extra 5% for high credit
            return baseDiscount + bonusDiscount;
        }
    }

    // Open/Closed: We can add new discount types without modifying this class
    public class OrderService
    {
        private readonly DiscountCalculator _discountCalculator;

        public OrderService(DiscountCalculator discountCalculator)
        {
            _discountCalculator = discountCalculator ?? throw new ArgumentNullException(nameof(discountCalculator));
        }

        public decimal CalculateFinalAmount(decimal orderAmount, Customer customer)
        {
            var discount = _discountCalculator.CalculateDiscount(orderAmount, customer);
            return orderAmount - discount;
        }
    }

    class SolidPrinciplesExample
    {
        static void Main()
        {
            // Setup dependencies
            var repository = new CustomerRepository();
            var customerService = new CustomerService(repository);

            // Create customers
            customerService.CreateCustomer("John Doe", "john@example.com", 5000);
            customerService.CreateCustomer("Jane Smith", "jane@example.com", 15000);

            var customers = repository.GetAll().ToList();
            var customer1 = customers[0];
            var customer2 = customers[1];

            Console.WriteLine($"\nCreated customers:");
            foreach (var c in customers)
            {
                Console.WriteLine($"- {c.Name} (Credit: ${c.CreditLimit})");
            }

            // Test different discount calculators (Open/Closed Principle)
            var orderAmount = 1000m;

            var regularService = new OrderService(new RegularDiscountCalculator());
            var premiumService = new OrderService(new PremiumDiscountCalculator());
            var vipService = new OrderService(new VipDiscountCalculator());

            Console.WriteLine($"\nOrder amount: ${orderAmount}");
            Console.WriteLine($"Regular discount final amount: ${regularService.CalculateFinalAmount(orderAmount, customer1)}");
            Console.WriteLine($"Premium discount final amount: ${premiumService.CalculateFinalAmount(orderAmount, customer1)}");
            Console.WriteLine($"VIP discount final amount: ${vipService.CalculateFinalAmount(orderAmount, customer2)}");
        }
    }
}
```

### Example 4: Liskov Substitution Principle (LSP)
Create `LiskovSubstitutionExample.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day05Examples
{
    // BAD: Violates LSP
    public class BadBird
    {
        public virtual void Fly()
        {
            Console.WriteLine("Flying high!");
        }
    }

    public class BadPenguin : BadBird
    {
        public override void Fly()
        {
            throw new NotSupportedException("Penguins can't fly!"); // LSP violation!
        }
    }

    // GOOD: Follows LSP
    public abstract class Bird
    {
        public string Name { get; protected set; }
        public abstract void Move();
        public virtual void Eat()
        {
            Console.WriteLine($"{Name} is eating");
        }
    }

    public abstract class FlyingBird : Bird
    {
        public virtual void Fly()
        {
            Console.WriteLine($"{Name} is flying");
        }

        public override void Move()
        {
            Fly();
        }
    }

    public abstract class FlightlessBird : Bird
    {
        public virtual void Walk()
        {
            Console.WriteLine($"{Name} is walking");
        }

        public override void Move()
        {
            Walk();
        }
    }

    public class Eagle : FlyingBird
    {
        public Eagle()
        {
            Name = "Eagle";
        }

        public override void Fly()
        {
            Console.WriteLine($"{Name} soars majestically through the sky");
        }
    }

    public class Penguin : FlightlessBird
    {
        public Penguin()
        {
            Name = "Penguin";
        }

        public override void Walk()
        {
            Console.WriteLine($"{Name} waddles adorably");
        }

        public void Swim()
        {
            Console.WriteLine($"{Name} swims gracefully underwater");
        }
    }

    public class Sparrow : FlyingBird
    {
        public Sparrow()
        {
            Name = "Sparrow";
        }
    }

    // Shape example demonstrating LSP
    public abstract class Shape
    {
        public abstract double CalculateArea();
        
        // This should work for all derived classes
        public void PrintArea()
        {
            Console.WriteLine($"Area: {CalculateArea():F2}");
        }
    }

    public class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override double CalculateArea()
        {
            return Width * Height;
        }
    }

    // BAD: Square violates LSP if it changes Width/Height behavior unexpectedly
    public class BadSquare : Rectangle
    {
        public BadSquare(double side) : base(side, side) { }

        // This breaks LSP! Setting width also changes height
        public new double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value; // Unexpected behavior!
            }
        }

        public new double Height
        {
            get => base.Height;
            set
            {
                base.Width = value;
                base.Height = value; // Unexpected behavior!
            }
        }
    }

    // GOOD: Square as separate shape following LSP
    public class Square : Shape
    {
        public double Side { get; set; }

        public Square(double side)
        {
            Side = side;
        }

        public override double CalculateArea()
        {
            return Side * Side;
        }
    }

    class LiskovSubstitutionExample
    {
        static void Main()
        {
            // LSP with birds - substitutable without breaking behavior
            var birds = new List<Bird>
            {
                new Eagle(),
                new Penguin(),
                new Sparrow()
            };

            foreach (var bird in birds)
            {
                bird.Move(); // Works correctly for all birds
                bird.Eat();
                Console.WriteLine();
            }

            // LSP with shapes
            var shapes = new List<Shape>
            {
                new Rectangle(5, 3),
                new Square(4)
            };

            foreach (var shape in shapes)
            {
                shape.PrintArea(); // Works correctly for all shapes
            }

            // Demonstrate the problem with BadSquare
            Console.WriteLine("\nDemonstrating LSP violation:");
            Rectangle rect = new BadSquare(5);
            Console.WriteLine($"Initial: Width={rect.Width}, Height={rect.Height}");
            
            rect.Width = 10; // Expectation: Width=10, Height=5
            Console.WriteLine($"After setting Width=10: Width={rect.Width}, Height={rect.Height}");
            // Actual: Width=10, Height=10 (unexpected!)
        }
    }
}
```

## 3. Best Practices

### Interface Design
- Keep interfaces small and focused (ISP)
- Use descriptive names that describe capabilities
- Consider default implementations for backward compatibility
- Prefer multiple small interfaces over one large interface

### SOLID Application
- **SRP**: One class, one responsibility, one reason to change
- **OCP**: Extend behavior through inheritance/composition, don't modify existing code
- **LSP**: Derived classes must be substitutable for their base classes
- **ISP**: Don't force clients to depend on interfaces they don't use
- **DIP**: Depend on abstractions, not concretions

### Dependency Injection
- Inject dependencies through constructors
- Use interfaces for all dependencies
- Keep constructors simple (just assignment)
- Validate dependencies (null checks)

## 4. Exercises

1. **Payment System**: Design a payment system following SOLID principles with multiple payment methods (Credit Card, PayPal, Bank Transfer).

2. **Logging Framework**: Create a logging system with multiple outputs (File, Console, Database) using DIP and ISP.

3. **Report Generator**: Build a report system that can generate different formats (PDF, Excel, CSV) without violating OCP.

4. **Authentication Service**: Design an auth system supporting multiple providers (Local, OAuth, LDAP) following all SOLID principles.

## 5. How to Run Examples

```powershell
# Create and run each example
dotnet new console -n Day05Examples
copy .\InterfaceSegregationExample.cs Day05Examples\Program.cs
cd Day05Examples
dotnet run
cd ..

# Or run all examples in sequence
# Copy all files to a single project and modify Main methods
```

## 6. Summary

Today you mastered:
- **Interface design** and when to use interfaces vs abstract classes
- **SOLID principles** and their practical application
- **Dependency injection** patterns for loosely coupled code
- **LSP** and how to design proper inheritance hierarchies
- **ISP** and interface segregation for cleaner APIs

Key takeaways:
- Interfaces define contracts, abstract classes provide partial implementations
- SOLID principles guide you toward maintainable, extensible code
- Dependency injection makes code testable and flexible
- Small, focused interfaces are better than large, monolithic ones
- Proper inheritance hierarchies respect behavioral expectations

Tomorrow: Day 06 — Exception Handling, Logging & Debugging