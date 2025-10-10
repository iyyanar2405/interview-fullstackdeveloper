# Day 02: Data Types, Variables & Operators

## 🎯 Learning Objectives
By the end of today, you will:
- Master all C# data types (value types and reference types)
- Understand type conversion and casting
- Work with nullable types and null handling
- Use all C# operators effectively
- Implement proper variable naming conventions
- Handle memory management concepts

## 📖 Theory: C# Type System

### The C# Type System Hierarchy
```
System.Object
├── Value Types (struct)
│   ├── Numeric Types
│   │   ├── Integral: byte, sbyte, short, ushort, int, uint, long, ulong
│   │   └── Floating: float, double, decimal
│   ├── Boolean: bool
│   ├── Character: char
│   ├── Enumerations: enum
│   └── User-defined structs
└── Reference Types (class)
    ├── String: string
    ├── Object: object
    ├── Arrays: T[]
    ├── Classes: class
    ├── Interfaces: interface
    └── Delegates: delegate
```

### Value Types vs Reference Types

**Value Types:**
- Stored on the stack
- Contain the actual data
- Copied when assigned
- Cannot be null (unless nullable)

**Reference Types:**
- Stored on the heap
- Contain reference to data
- Reference is copied when assigned
- Can be null

## 💻 Hands-on Examples

### Example 1: Comprehensive Data Types Demo

Create `DataTypesAdvanced.cs`:

```csharp
using System;
using System.Numerics;

namespace Day02Examples
{
    class DataTypesAdvanced
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# Data Types Comprehensive Demo ===\n");
            
            DemonstrateIntegralTypes();
            DemonstrateFloatingPointTypes();
            DemonstrateOtherValueTypes();
            DemonstrateReferenceTypes();
            DemonstrateNullableTypes();
            DemonstrateTypeConversions();
            DemonstrateOperators();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        static void DemonstrateIntegralTypes()
        {
            Console.WriteLine("📊 INTEGRAL TYPES");
            Console.WriteLine(new string('-', 50));
            
            // Signed integers
            sbyte sbyteValue = -128;        // -128 to 127
            short shortValue = -32768;      // -32,768 to 32,767
            int intValue = -2147483648;     // -2.1B to 2.1B
            long longValue = -9223372036854775808; // Very large range
            
            // Unsigned integers
            byte byteValue = 255;           // 0 to 255
            ushort ushortValue = 65535;     // 0 to 65,535
            uint uintValue = 4294967295;    // 0 to 4.3B
            ulong ulongValue = 18446744073709551615; // 0 to 18.4 quintillion
            
            Console.WriteLine($"sbyte:  {sbyteValue,20} | Size: {sizeof(sbyte)} byte  | Range: {sbyte.MinValue} to {sbyte.MaxValue}");
            Console.WriteLine($"byte:   {byteValue,20} | Size: {sizeof(byte)} byte  | Range: {byte.MinValue} to {byte.MaxValue}");
            Console.WriteLine($"short:  {shortValue,20} | Size: {sizeof(short)} bytes | Range: {short.MinValue} to {short.MaxValue}");
            Console.WriteLine($"ushort: {ushortValue,20} | Size: {sizeof(ushort)} bytes | Range: {ushort.MinValue} to {ushort.MaxValue}");
            Console.WriteLine($"int:    {intValue,20} | Size: {sizeof(int)} bytes | Range: {int.MinValue} to {int.MaxValue}");
            Console.WriteLine($"uint:   {uintValue,20} | Size: {sizeof(uint)} bytes | Range: {uint.MinValue} to {uint.MaxValue}");
            Console.WriteLine($"long:   {longValue,20} | Size: {sizeof(long)} bytes | Range: {long.MinValue} to {long.MaxValue}");
            Console.WriteLine($"ulong:  {ulongValue,20} | Size: {sizeof(ulong)} bytes | Range: {ulong.MinValue} to {ulong.MaxValue}");
            
            // Special numeric types
            BigInteger bigInt = BigInteger.Parse("123456789012345678901234567890");
            Console.WriteLine($"BigInteger: {bigInt} (unlimited precision)");
            
            Console.WriteLine();
        }
        
        static void DemonstrateFloatingPointTypes()
        {
            Console.WriteLine("🔢 FLOATING-POINT TYPES");
            Console.WriteLine(new string('-', 50));
            
            float floatValue = 3.14159265f;        // ~7 digits precision
            double doubleValue = 3.14159265358979; // ~15-17 digits precision
            decimal decimalValue = 3.14159265358979323846m; // ~28-29 digits precision
            
            Console.WriteLine($"float:   {floatValue,25} | Size: {sizeof(float)} bytes | Precision: ~7 digits");
            Console.WriteLine($"double:  {doubleValue,25} | Size: {sizeof(double)} bytes | Precision: ~15-17 digits");
            Console.WriteLine($"decimal: {decimalValue,25} | Size: {sizeof(decimal)} bytes | Precision: ~28-29 digits");
            
            // Demonstrating precision differences
            Console.WriteLine("\n📐 Precision Comparison:");
            float f1 = 0.1f + 0.2f;
            double d1 = 0.1 + 0.2;
            decimal dec1 = 0.1m + 0.2m;
            
            Console.WriteLine($"float:   0.1 + 0.2 = {f1}");
            Console.WriteLine($"double:  0.1 + 0.2 = {d1}");
            Console.WriteLine($"decimal: 0.1 + 0.2 = {dec1}");
            Console.WriteLine("💡 Use decimal for financial calculations!");
            
            // Special values
            Console.WriteLine("\n🔴 Special Floating-Point Values:");
            Console.WriteLine($"Positive Infinity: {double.PositiveInfinity}");
            Console.WriteLine($"Negative Infinity: {double.NegativeInfinity}");
            Console.WriteLine($"Not a Number: {double.NaN}");
            Console.WriteLine($"Max Double: {double.MaxValue}");
            Console.WriteLine($"Min Double: {double.MinValue}");
            
            Console.WriteLine();
        }
        
        static void DemonstrateOtherValueTypes()
        {
            Console.WriteLine("📝 OTHER VALUE TYPES");
            Console.WriteLine(new string('-', 50));
            
            // Boolean
            bool isLearning = true;
            bool isExpert = false;
            Console.WriteLine($"bool: {isLearning} | {isExpert} | Size: {sizeof(bool)} byte");
            
            // Character
            char letter = 'A';
            char unicode = '\u0041'; // Unicode for 'A'
            char escaped = '\'';     // Escaped single quote
            Console.WriteLine($"char: {letter} | {unicode} | {escaped} | Size: {sizeof(char)} bytes");
            
            // DateTime (struct)
            DateTime now = DateTime.Now;
            DateTime utcNow = DateTime.UtcNow;
            DateTime custom = new DateTime(2024, 12, 25, 10, 30, 0);
            
            Console.WriteLine($"DateTime Now: {now}");
            Console.WriteLine($"DateTime UTC: {utcNow}");
            Console.WriteLine($"Custom Date: {custom:yyyy-MM-dd HH:mm:ss}");
            
            // TimeSpan (struct)
            TimeSpan courseLength = TimeSpan.FromDays(60);
            TimeSpan dailyStudy = new TimeSpan(3, 30, 0); // 3 hours 30 minutes
            
            Console.WriteLine($"Course Length: {courseLength.TotalDays} days");
            Console.WriteLine($"Daily Study: {dailyStudy.Hours}h {dailyStudy.Minutes}m");
            
            // Guid (struct)
            Guid uniqueId = Guid.NewGuid();
            Console.WriteLine($"GUID: {uniqueId}");
            
            Console.WriteLine();
        }
        
        static void DemonstrateReferenceTypes()
        {
            Console.WriteLine("🔗 REFERENCE TYPES");
            Console.WriteLine(new string('-', 50));
            
            // String
            string greeting = "Hello, Microservices!";
            string nullString = null;
            string emptyString = string.Empty;
            string whitespace = "   ";
            
            Console.WriteLine($"String: '{greeting}'");
            Console.WriteLine($"Null String: {nullString ?? "NULL"}");
            Console.WriteLine($"Empty String: '{emptyString}'");
            Console.WriteLine($"Whitespace: '{whitespace}'");
            
            // String methods
            Console.WriteLine($"Length: {greeting.Length}");
            Console.WriteLine($"Is Null or Empty: {string.IsNullOrEmpty(emptyString)}");
            Console.WriteLine($"Is Null or Whitespace: {string.IsNullOrWhiteSpace(whitespace)}");
            
            // Object
            object obj1 = 42;           // Boxing - value type to reference type
            object obj2 = "Hello";      // Reference type
            object obj3 = DateTime.Now; // Boxing
            
            Console.WriteLine($"Object 1: {obj1} (Type: {obj1.GetType()})");
            Console.WriteLine($"Object 2: {obj2} (Type: {obj2.GetType()})");
            Console.WriteLine($"Object 3: {obj3} (Type: {obj3.GetType()})");
            
            // Arrays
            int[] numbers = { 1, 2, 3, 4, 5 };
            string[] technologies = new string[] { "C#", "ASP.NET", "Docker", "Kubernetes" };
            
            Console.WriteLine($"Int Array: [{string.Join(", ", numbers)}]");
            Console.WriteLine($"String Array: [{string.Join(", ", technologies)}]");
            
            Console.WriteLine();
        }
        
        static void DemonstrateNullableTypes()
        {
            Console.WriteLine("❓ NULLABLE TYPES");
            Console.WriteLine(new string('-', 50));
            
            // Nullable value types
            int? nullableInt = null;
            double? nullableDouble = 3.14;
            bool? nullableBool = null;
            DateTime? nullableDate = DateTime.Now;
            
            Console.WriteLine($"Nullable int: {nullableInt?.ToString() ?? "NULL"}");
            Console.WriteLine($"Nullable double: {nullableDouble}");
            Console.WriteLine($"Nullable bool: {nullableBool?.ToString() ?? "NULL"}");
            Console.WriteLine($"Nullable DateTime: {nullableDate}");
            
            // HasValue and Value properties
            if (nullableDouble.HasValue)
            {
                Console.WriteLine($"Double has value: {nullableDouble.Value}");
            }
            
            // Null coalescing operator
            int value1 = nullableInt ?? 0;
            string message = nullableInt?.ToString() ?? "No value";
            
            Console.WriteLine($"Coalesced value: {value1}");
            Console.WriteLine($"Conditional message: {message}");
            
            // Null conditional operator
            string? text = null;
            int? length = text?.Length;
            Console.WriteLine($"Text length: {length?.ToString() ?? "NULL"}");
            
            Console.WriteLine();
        }
        
        static void DemonstrateTypeConversions()
        {
            Console.WriteLine("🔄 TYPE CONVERSIONS");
            Console.WriteLine(new string('-', 50));
            
            // Implicit conversions (safe)
            int intValue = 42;
            long longValue = intValue;      // int to long
            double doubleValue = intValue;  // int to double
            
            Console.WriteLine($"Implicit: int {intValue} → long {longValue} → double {doubleValue}");
            
            // Explicit conversions (casting)
            double pi = 3.14159;
            int truncatedPi = (int)pi;      // Truncates decimal part
            float floatPi = (float)pi;      // May lose precision
            
            Console.WriteLine($"Explicit: double {pi} → int {truncatedPi} → float {floatPi}");
            
            // Parse methods
            string numberString = "12345";
            string doubleString = "123.45";
            string boolString = "true";
            
            int parsedInt = int.Parse(numberString);
            double parsedDouble = double.Parse(doubleString);
            bool parsedBool = bool.Parse(boolString);
            
            Console.WriteLine($"Parse: '{numberString}' → {parsedInt}");
            Console.WriteLine($"Parse: '{doubleString}' → {parsedDouble}");
            Console.WriteLine($"Parse: '{boolString}' → {parsedBool}");
            
            // TryParse methods (safe parsing)
            string invalidNumber = "abc123";
            if (int.TryParse(invalidNumber, out int result))
            {
                Console.WriteLine($"TryParse successful: {result}");
            }
            else
            {
                Console.WriteLine($"TryParse failed for '{invalidNumber}'");
            }
            
            // Convert class
            string stringValue = "456";
            int convertedInt = Convert.ToInt32(stringValue);
            bool convertedBool = Convert.ToBoolean(1);
            string convertedString = Convert.ToString(123.45);
            
            Console.WriteLine($"Convert: '{stringValue}' → {convertedInt}");
            Console.WriteLine($"Convert: 1 → {convertedBool}");
            Console.WriteLine($"Convert: 123.45 → '{convertedString}'");
            
            Console.WriteLine();
        }
        
        static void DemonstrateOperators()
        {
            Console.WriteLine("⚡ OPERATORS");
            Console.WriteLine(new string('-', 50));
            
            // Arithmetic operators
            int a = 10, b = 3;
            Console.WriteLine("Arithmetic Operators:");
            Console.WriteLine($"{a} + {b} = {a + b}");
            Console.WriteLine($"{a} - {b} = {a - b}");
            Console.WriteLine($"{a} * {b} = {a * b}");
            Console.WriteLine($"{a} / {b} = {a / b}");           // Integer division
            Console.WriteLine($"{a} % {b} = {a % b}");           // Remainder
            Console.WriteLine($"{a} / {b}.0 = {a / (double)b}"); // Floating division
            
            // Increment/Decrement
            int x = 5;
            Console.WriteLine($"\nIncrement/Decrement:");
            Console.WriteLine($"x = {x}");
            Console.WriteLine($"x++ = {x++} (post-increment, x is now {x})");
            Console.WriteLine($"++x = {++x} (pre-increment)");
            Console.WriteLine($"x-- = {x--} (post-decrement, x is now {x})");
            Console.WriteLine($"--x = {--x} (pre-decrement)");
            
            // Comparison operators
            Console.WriteLine($"\nComparison Operators:");
            Console.WriteLine($"{a} == {b}: {a == b}");
            Console.WriteLine($"{a} != {b}: {a != b}");
            Console.WriteLine($"{a} > {b}: {a > b}");
            Console.WriteLine($"{a} < {b}: {a < b}");
            Console.WriteLine($"{a} >= {b}: {a >= b}");
            Console.WriteLine($"{a} <= {b}: {a <= b}");
            
            // Logical operators
            bool p = true, q = false;
            Console.WriteLine($"\nLogical Operators:");
            Console.WriteLine($"{p} && {q}: {p && q}"); // AND
            Console.WriteLine($"{p} || {q}: {p || q}"); // OR
            Console.WriteLine($"!{p}: {!p}");           // NOT
            Console.WriteLine($"{p} ^ {q}: {p ^ q}");   // XOR
            
            // Bitwise operators
            int m = 12, n = 8; // 1100 and 1000 in binary
            Console.WriteLine($"\nBitwise Operators:");
            Console.WriteLine($"{m} & {n} = {m & n}");   // AND
            Console.WriteLine($"{m} | {n} = {m | n}");   // OR
            Console.WriteLine($"{m} ^ {n} = {m ^ n}");   // XOR
            Console.WriteLine($"~{m} = {~m}");           // NOT
            Console.WriteLine($"{m} << 1 = {m << 1}");   // Left shift
            Console.WriteLine($"{m} >> 1 = {m >> 1}");   // Right shift
            
            // Assignment operators
            int y = 10;
            Console.WriteLine($"\nAssignment Operators:");
            Console.WriteLine($"y = {y}");
            y += 5; Console.WriteLine($"y += 5: {y}");
            y -= 3; Console.WriteLine($"y -= 3: {y}");
            y *= 2; Console.WriteLine($"y *= 2: {y}");
            y /= 4; Console.WriteLine($"y /= 4: {y}");
            y %= 3; Console.WriteLine($"y %= 3: {y}");
            
            // Null coalescing operators
            string? text1 = null;
            string? text2 = "Hello";
            string result1 = text1 ?? "Default";
            string result2 = text2 ?? "Default";
            
            Console.WriteLine($"\nNull Coalescing:");
            Console.WriteLine($"null ?? 'Default': '{result1}'");
            Console.WriteLine($"'Hello' ?? 'Default': '{result2}'");
            
            // Null conditional operators
            string? nullText = null;
            string? validText = "Microservices";
            
            Console.WriteLine($"\nNull Conditional:");
            Console.WriteLine($"null?.Length: {nullText?.Length?.ToString() ?? "NULL"}");
            Console.WriteLine($"'Microservices'?.Length: {validText?.Length}");
            Console.WriteLine($"'Microservices'?[0]: {validText?[0]}");
            
            Console.WriteLine();
        }
    }
}
```

### Example 2: Variable Best Practices

Create `VariableBestPractices.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day02Examples
{
    /// <summary>
    /// Demonstrates C# variable naming conventions and best practices
    /// </summary>
    class VariableBestPractices
    {
        // Class-level fields (private by default)
        private static readonly string CompanyName = "MicroservicesCorp";
        private static int _totalUsers = 0;
        private readonly DateTime _createdDate = DateTime.Now;
        
        // Constants
        private const int MaxRetryAttempts = 3;
        private const double TaxRate = 0.08;
        
        // Properties
        public string UserName { get; set; } = "";
        public int UserId { get; private set; }
        public bool IsActive { get; set; } = true;
        
        static void Main(string[] args)
        {
            Console.WriteLine("=== Variable Best Practices Demo ===\n");
            
            DemonstrateNamingConventions();
            DemonstrateVariableScopes();
            DemonstrateConstantsAndReadonly();
            DemonstrateVarKeyword();
            DemonstrateCollectionInitialization();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        static void DemonstrateNamingConventions()
        {
            Console.WriteLine("📝 NAMING CONVENTIONS");
            Console.WriteLine(new string('-', 50));
            
            // ✅ Good naming practices
            string firstName = "John";              // camelCase for local variables
            string lastName = "Developer";
            int currentAge = 30;
            bool isEmailVerified = true;
            DateTime registrationDate = DateTime.Now;
            List<string> programmingLanguages = new List<string>();
            
            // Method names use PascalCase
            string fullName = GetFullName(firstName, lastName);
            
            // Constants use PascalCase
            const int MinPasswordLength = 8;
            const string DefaultCurrency = "USD";
            
            Console.WriteLine($"User: {fullName}");
            Console.WriteLine($"Age: {currentAge}");
            Console.WriteLine($"Email Verified: {isEmailVerified}");
            Console.WriteLine($"Registered: {registrationDate:yyyy-MM-dd}");
            Console.WriteLine($"Min Password Length: {MinPasswordLength}");
            Console.WriteLine($"Currency: {DefaultCurrency}");
            
            // ❌ Poor naming practices (avoid these)
            /*
            string n = "John";          // Too short, not descriptive
            string data = "info";       // Too generic
            bool flag = true;           // Meaningless
            int temp = 42;              // Temporary variables should still be descriptive
            string string1 = "text";    // Using numbers instead of descriptive names
            */
            
            Console.WriteLine();
        }
        
        static void DemonstrateVariableScopes()
        {
            Console.WriteLine("🔍 VARIABLE SCOPES");
            Console.WriteLine(new string('-', 50));
            
            // Method scope
            string methodVariable = "I'm in method scope";
            Console.WriteLine($"Method variable: {methodVariable}");
            
            // Block scope
            if (true)
            {
                string blockVariable = "I'm in block scope";
                Console.WriteLine($"Block variable: {blockVariable}");
                
                // Can access method variable from block
                Console.WriteLine($"Accessing method variable from block: {methodVariable}");
            }
            
            // blockVariable is not accessible here
            // Console.WriteLine(blockVariable); // This would cause a compilation error
            
            // Loop scope
            for (int i = 0; i < 3; i++)
            {
                string loopVariable = $"Loop iteration {i}";
                Console.WriteLine($"Loop variable: {loopVariable}");
            }
            
            // i and loopVariable are not accessible here
            
            // Using scope
            using (var disposableResource = new System.IO.StringWriter())
            {
                disposableResource.WriteLine("Using scope variable");
                Console.WriteLine("Resource is available in using block");
            }
            // disposableResource is automatically disposed here
            
            Console.WriteLine();
        }
        
        static void DemonstrateConstantsAndReadonly()
        {
            Console.WriteLine("🔒 CONSTANTS AND READONLY");
            Console.WriteLine(new string('-', 50));
            
            // Compile-time constants
            const string ApiVersion = "v1.0";
            const int BufferSize = 1024;
            const double Pi = 3.14159265359;
            
            Console.WriteLine($"API Version: {ApiVersion}");
            Console.WriteLine($"Buffer Size: {BufferSize} bytes");
            Console.WriteLine($"Pi: {Pi}");
            
            // Runtime constants (readonly)
            var example = new VariableBestPractices();
            Console.WriteLine($"Created Date: {example._createdDate}");
            Console.WriteLine($"Company: {CompanyName}");
            
            // Static readonly vs const
            Console.WriteLine($"Max Retry Attempts: {MaxRetryAttempts}");
            
            // Readonly collections
            var readonlyList = new List<string> { "C#", "ASP.NET", "Docker" }.AsReadOnly();
            Console.WriteLine($"Technologies: [{string.Join(", ", readonlyList)}]");
            
            Console.WriteLine();
        }
        
        static void DemonstrateVarKeyword()
        {
            Console.WriteLine("🔤 VAR KEYWORD");
            Console.WriteLine(new string('-', 50));
            
            // var with obvious types
            var message = "Hello World";              // string
            var number = 42;                          // int
            var price = 99.99;                        // double
            var isValid = true;                       // bool
            var currentTime = DateTime.Now;           // DateTime
            
            Console.WriteLine($"var message: {message} (Type: {message.GetType().Name})");
            Console.WriteLine($"var number: {number} (Type: {number.GetType().Name})");
            Console.WriteLine($"var price: {price} (Type: {price.GetType().Name})");
            Console.WriteLine($"var isValid: {isValid} (Type: {isValid.GetType().Name})");
            Console.WriteLine($"var currentTime: {currentTime} (Type: {currentTime.GetType().Name})");
            
            // var with complex types
            var userDictionary = new Dictionary<string, int>
            {
                ["John"] = 25,
                ["Jane"] = 30,
                ["Bob"] = 35
            };
            
            var anonymousObject = new { Name = "Anonymous", Age = 25, IsActive = true };
            
            Console.WriteLine($"Dictionary type: {userDictionary.GetType().Name}");
            Console.WriteLine($"Anonymous object: {anonymousObject.Name}, Age: {anonymousObject.Age}");
            
            // When NOT to use var
            // ❌ var result = GetSomeValue(); // Type not obvious
            // ✅ string result = GetSomeValue(); // Type is clear
            
            Console.WriteLine();
        }
        
        static void DemonstrateCollectionInitialization()
        {
            Console.WriteLine("📦 COLLECTION INITIALIZATION");
            Console.WriteLine(new string('-', 50));
            
            // Array initialization
            int[] numbers1 = { 1, 2, 3, 4, 5 };
            int[] numbers2 = new int[] { 10, 20, 30, 40, 50 };
            int[] numbers3 = new int[5] { 100, 200, 300, 400, 500 };
            
            Console.WriteLine($"Array 1: [{string.Join(", ", numbers1)}]");
            Console.WriteLine($"Array 2: [{string.Join(", ", numbers2)}]");
            Console.WriteLine($"Array 3: [{string.Join(", ", numbers3)}]");
            
            // List initialization
            var technologies = new List<string> { "C#", "ASP.NET Core", "Docker", "Kubernetes" };
            var frameworks = new List<string>
            {
                "Entity Framework",
                "AutoMapper",
                "Newtonsoft.Json",
                "Serilog"
            };
            
            Console.WriteLine($"Technologies: [{string.Join(", ", technologies)}]");
            Console.WriteLine($"Frameworks: [{string.Join(", ", frameworks)}]");
            
            // Dictionary initialization
            var microservices = new Dictionary<string, string>
            {
                ["UserService"] = "Manages user accounts and authentication",
                ["ProductService"] = "Handles product catalog and inventory",
                ["OrderService"] = "Processes customer orders",
                ["PaymentService"] = "Handles payment processing"
            };
            
            Console.WriteLine("Microservices:");
            foreach (var service in microservices)
            {
                Console.WriteLine($"  {service.Key}: {service.Value}");
            }
            
            // Object initialization
            var user = new User
            {
                Id = 1,
                Name = "John Developer",
                Email = "john@microservices.com",
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            
            Console.WriteLine($"\nUser: {user.Name} ({user.Email})");
            Console.WriteLine($"Active: {user.IsActive}, Created: {user.CreatedDate:yyyy-MM-dd}");
            
            Console.WriteLine();
        }
        
        static string GetFullName(string firstName, string lastName)
        {
            return $"{firstName} {lastName}";
        }
    }
    
    // Example class for object initialization
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
```

### Example 3: Memory Management and Performance

Create `MemoryManagement.cs`:

```csharp
using System;
using System.Diagnostics;
using System.Text;

namespace Day02Examples
{
    class MemoryManagement
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Memory Management Demo ===\n");
            
            DemonstrateValueVsReference();
            DemonstrateBoxingUnboxing();
            DemonstrateStringVsStringBuilder();
            DemonstrateGarbageCollection();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        static void DemonstrateValueVsReference()
        {
            Console.WriteLine("💾 VALUE vs REFERENCE TYPES");
            Console.WriteLine(new string('-', 50));
            
            // Value types
            int a = 10;
            int b = a;    // Copy the value
            b = 20;       // Only b changes
            
            Console.WriteLine($"Value types - a: {a}, b: {b}");
            
            // Reference types
            var person1 = new Person { Name = "John", Age = 25 };
            var person2 = person1;  // Copy the reference
            person2.Age = 30;       // Both person1 and person2 point to same object
            
            Console.WriteLine($"Reference types - person1.Age: {person1.Age}, person2.Age: {person2.Age}");
            
            // Struct vs Class demonstration
            var point1 = new Point { X = 10, Y = 20 };
            var point2 = point1;  // Value copy for struct
            point2.X = 30;        // Only point2 changes
            
            Console.WriteLine($"Struct - point1.X: {point1.X}, point2.X: {point2.X}");
            
            var location1 = new Location { X = 10, Y = 20 };
            var location2 = location1;  // Reference copy for class
            location2.X = 30;           // Both change
            
            Console.WriteLine($"Class - location1.X: {location1.X}, location2.X: {location2.X}");
            Console.WriteLine();
        }
        
        static void DemonstrateBoxingUnboxing()
        {
            Console.WriteLine("📦 BOXING and UNBOXING");
            Console.WriteLine(new string('-', 50));
            
            // Boxing - value type to reference type
            int value = 42;
            object boxedValue = value;  // Boxing occurs here
            
            Console.WriteLine($"Original value: {value}");
            Console.WriteLine($"Boxed value: {boxedValue}");
            Console.WriteLine($"Are they equal? {value.Equals(boxedValue)}");
            
            // Unboxing - reference type back to value type
            int unboxedValue = (int)boxedValue;  // Unboxing occurs here
            Console.WriteLine($"Unboxed value: {unboxedValue}");
            
            // Performance impact
            var sw = Stopwatch.StartNew();
            
            // Boxing in loop (slow)
            object[] objects = new object[1000000];
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = i;  // Boxing each integer
            }
            
            sw.Stop();
            Console.WriteLine($"Boxing 1M integers took: {sw.ElapsedMilliseconds}ms");
            
            // Better approach - use generic collections
            sw.Restart();
            int[] integers = new int[1000000];
            for (int i = 0; i < integers.Length; i++)
            {
                integers[i] = i;  // No boxing
            }
            sw.Stop();
            Console.WriteLine($"Direct assignment took: {sw.ElapsedMilliseconds}ms");
            
            Console.WriteLine();
        }
        
        static void DemonstrateStringVsStringBuilder()
        {
            Console.WriteLine("🔤 STRING vs STRINGBUILDER");
            Console.WriteLine(new string('-', 50));
            
            const int iterations = 10000;
            
            // String concatenation (creates new string each time)
            var sw = Stopwatch.StartNew();
            string result1 = "";
            for (int i = 0; i < iterations; i++)
            {
                result1 += $"Item {i} ";  // Creates new string object each time
            }
            sw.Stop();
            Console.WriteLine($"String concatenation ({iterations} iterations): {sw.ElapsedMilliseconds}ms");
            
            // StringBuilder (efficient for multiple concatenations)
            sw.Restart();
            var sb = new StringBuilder();
            for (int i = 0; i < iterations; i++)
            {
                sb.Append($"Item {i} ");  // Modifies internal buffer
            }
            string result2 = sb.ToString();
            sw.Stop();
            Console.WriteLine($"StringBuilder ({iterations} iterations): {sw.ElapsedMilliseconds}ms");
            
            Console.WriteLine($"Results are equal: {result1.Equals(result2)}");
            Console.WriteLine($"String length: {result1.Length} characters");
            
            // When to use what
            Console.WriteLine("\n📋 Guidelines:");
            Console.WriteLine("- Use string for < 5 concatenations");
            Console.WriteLine("- Use StringBuilder for loops or many concatenations");
            Console.WriteLine("- Use string interpolation for readability");
            
            Console.WriteLine();
        }
        
        static void DemonstrateGarbageCollection()
        {
            Console.WriteLine("🗑️ GARBAGE COLLECTION");
            Console.WriteLine(new string('-', 50));
            
            // Check initial memory
            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before allocation: {memoryBefore:N0} bytes");
            
            // Allocate some objects
            var objects = new object[100000];
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = new Person { Name = $"Person {i}", Age = i % 100 };
            }
            
            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after allocation: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory used: {memoryAfter - memoryBefore:N0} bytes");
            
            // Show GC information
            Console.WriteLine($"Generation 0 collections: {GC.CollectionCount(0)}");
            Console.WriteLine($"Generation 1 collections: {GC.CollectionCount(1)}");
            Console.WriteLine($"Generation 2 collections: {GC.CollectionCount(2)}");
            
            // Clear references
            objects = null;
            
            // Force garbage collection (don't do this in production!)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryAfterGC = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after GC: {memoryAfterGC:N0} bytes");
            Console.WriteLine($"Memory freed: {memoryAfter - memoryAfterGC:N0} bytes");
            
            Console.WriteLine("\n📋 GC Best Practices:");
            Console.WriteLine("- Don't call GC.Collect() manually");
            Console.WriteLine("- Minimize object allocations in loops");
            Console.WriteLine("- Use object pooling for frequently created objects");
            Console.WriteLine("- Dispose IDisposable objects properly");
            
            Console.WriteLine();
        }
    }
    
    // Example classes for demonstrations
    public class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
    
    public struct Point  // Value type
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    
    public class Location  // Reference type
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
```

## 🏗️ Mini-Project: Advanced Calculator

Create `AdvancedCalculator.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day02Examples
{
    /// <summary>
    /// Advanced calculator demonstrating various C# data types and operators
    /// </summary>
    public class AdvancedCalculator
    {
        private readonly List<string> _history = new List<string>();
        private double _memory = 0;
        
        static void Main(string[] args)
        {
            var calculator = new AdvancedCalculator();
            calculator.Run();
        }
        
        public void Run()
        {
            Console.Clear();
            DisplayWelcome();
            
            bool running = true;
            while (running)
            {
                DisplayMenu();
                string? choice = Console.ReadLine();
                
                try
                {
                    switch (choice?.ToUpper())
                    {
                        case "1":
                            BasicCalculation();
                            break;
                        case "2":
                            ScientificCalculation();
                            break;
                        case "3":
                            BitwiseOperations();
                            break;
                        case "4":
                            TypeConversions();
                            break;
                        case "5":
                            MemoryOperations();
                            break;
                        case "6":
                            DisplayHistory();
                            break;
                        case "7":
                            ClearHistory();
                            break;
                        case "0":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                if (running && choice != "6")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
            
            DisplayGoodbye();
        }
        
        private void DisplayWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║          ADVANCED CALCULATOR            ║");
            Console.WriteLine("║      C# Data Types & Operators Demo     ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
        
        private void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine($"Memory: {_memory}");
            Console.WriteLine(new string('=', 45));
            Console.WriteLine("1. Basic Arithmetic");
            Console.WriteLine("2. Scientific Functions");
            Console.WriteLine("3. Bitwise Operations");
            Console.WriteLine("4. Type Conversions");
            Console.WriteLine("5. Memory Operations");
            Console.WriteLine("6. View History");
            Console.WriteLine("7. Clear History");
            Console.WriteLine("0. Exit");
            Console.WriteLine(new string('=', 45));
            Console.Write("Choose an option: ");
        }
        
        private void BasicCalculation()
        {
            Console.Clear();
            Console.WriteLine("🔢 BASIC ARITHMETIC");
            Console.WriteLine(new string('-', 30));
            
            double num1 = GetNumber("Enter first number: ");
            double num2 = GetNumber("Enter second number: ");
            
            Console.WriteLine("\nResults:");
            LogAndDisplay($"{num1} + {num2} = {num1 + num2}");
            LogAndDisplay($"{num1} - {num2} = {num1 - num2}");
            LogAndDisplay($"{num1} * {num2} = {num1 * num2}");
            
            if (num2 != 0)
            {
                LogAndDisplay($"{num1} / {num2} = {num1 / num2}");
                LogAndDisplay($"{num1} % {num2} = {num1 % num2}");
            }
            else
            {
                Console.WriteLine("Cannot divide by zero!");
            }
            
            LogAndDisplay($"{num1} ^ {num2} = {Math.Pow(num1, num2)}");
        }
        
        private void ScientificCalculation()
        {
            Console.Clear();
            Console.WriteLine("🧮 SCIENTIFIC FUNCTIONS");
            Console.WriteLine(new string('-', 30));
            
            double number = GetNumber("Enter a number: ");
            
            Console.WriteLine("\nTrigonometric Functions:");
            LogAndDisplay($"sin({number}) = {Math.Sin(number)}");
            LogAndDisplay($"cos({number}) = {Math.Cos(number)}");
            LogAndDisplay($"tan({number}) = {Math.Tan(number)}");
            
            Console.WriteLine("\nLogarithmic Functions:");
            if (number > 0)
            {
                LogAndDisplay($"ln({number}) = {Math.Log(number)}");
                LogAndDisplay($"log10({number}) = {Math.Log10(number)}");
            }
            else
            {
                Console.WriteLine("Logarithm requires positive number!");
            }
            
            Console.WriteLine("\nOther Functions:");
            LogAndDisplay($"sqrt({number}) = {Math.Sqrt(Math.Abs(number))}");
            LogAndDisplay($"abs({number}) = {Math.Abs(number)}");
            LogAndDisplay($"ceil({number}) = {Math.Ceiling(number)}");
            LogAndDisplay($"floor({number}) = {Math.Floor(number)}");
            LogAndDisplay($"round({number}) = {Math.Round(number)}");
        }
        
        private void BitwiseOperations()
        {
            Console.Clear();
            Console.WriteLine("🔣 BITWISE OPERATIONS");
            Console.WriteLine(new string('-', 30));
            
            int num1 = (int)GetNumber("Enter first integer: ");
            int num2 = (int)GetNumber("Enter second integer: ");
            
            Console.WriteLine($"\nBinary representation:");
            Console.WriteLine($"{num1} = {Convert.ToString(num1, 2).PadLeft(8, '0')}");
            Console.WriteLine($"{num2} = {Convert.ToString(num2, 2).PadLeft(8, '0')}");
            
            Console.WriteLine("\nBitwise Operations:");
            int andResult = num1 & num2;
            int orResult = num1 | num2;
            int xorResult = num1 ^ num2;
            int notResult = ~num1;
            
            LogAndDisplay($"{num1} & {num2} = {andResult} ({Convert.ToString(andResult, 2).PadLeft(8, '0')})");
            LogAndDisplay($"{num1} | {num2} = {orResult} ({Convert.ToString(orResult, 2).PadLeft(8, '0')})");
            LogAndDisplay($"{num1} ^ {num2} = {xorResult} ({Convert.ToString(xorResult, 2).PadLeft(8, '0')})");
            LogAndDisplay($"~{num1} = {notResult} ({Convert.ToString(notResult, 2)})");
            
            Console.WriteLine("\nShift Operations:");
            LogAndDisplay($"{num1} << 1 = {num1 << 1}");
            LogAndDisplay($"{num1} >> 1 = {num1 >> 1}");
        }
        
        private void TypeConversions()
        {
            Console.Clear();
            Console.WriteLine("🔄 TYPE CONVERSIONS");
            Console.WriteLine(new string('-', 30));
            
            Console.Write("Enter a number (can be decimal): ");
            string? input = Console.ReadLine() ?? "0";
            
            Console.WriteLine("\nConversion Results:");
            
            // Parse to different numeric types
            if (double.TryParse(input, out double doubleValue))
            {
                Console.WriteLine($"As double: {doubleValue}");
                Console.WriteLine($"As int (truncated): {(int)doubleValue}");
                Console.WriteLine($"As long: {(long)doubleValue}");
                Console.WriteLine($"As decimal: {(decimal)doubleValue}");
                Console.WriteLine($"As float: {(float)doubleValue}");
                
                // Show precision differences
                float floatValue = (float)doubleValue;
                decimal decimalValue = (decimal)doubleValue;
                
                Console.WriteLine($"\nPrecision Comparison:");
                Console.WriteLine($"Original: {doubleValue}");
                Console.WriteLine($"Float:    {floatValue}");
                Console.WriteLine($"Decimal:  {decimalValue}");
                
                LogAndDisplay($"Converted {input} to various types");
            }
            else
            {
                Console.WriteLine("Invalid number format!");
            }
            
            // Binary, Octal, Hexadecimal conversions
            if (int.TryParse(input, out int intValue))
            {
                Console.WriteLine($"\nBase Conversions:");
                Console.WriteLine($"Decimal: {intValue}");
                Console.WriteLine($"Binary:  {Convert.ToString(intValue, 2)}");
                Console.WriteLine($"Octal:   {Convert.ToString(intValue, 8)}");
                Console.WriteLine($"Hex:     {Convert.ToString(intValue, 16).ToUpper()}");
            }
        }
        
        private void MemoryOperations()
        {
            Console.Clear();
            Console.WriteLine("💾 MEMORY OPERATIONS");
            Console.WriteLine(new string('-', 30));
            Console.WriteLine($"Current Memory: {_memory}");
            Console.WriteLine();
            Console.WriteLine("1. Store value in memory (MS)");
            Console.WriteLine("2. Recall memory (MR)");
            Console.WriteLine("3. Add to memory (M+)");
            Console.WriteLine("4. Subtract from memory (M-)");
            Console.WriteLine("5. Clear memory (MC)");
            Console.Write("Choose operation: ");
            
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    _memory = GetNumber("Enter value to store: ");
                    LogAndDisplay($"Stored {_memory} in memory");
                    break;
                case "2":
                    Console.WriteLine($"Memory value: {_memory}");
                    LogAndDisplay($"Recalled memory: {_memory}");
                    break;
                case "3":
                    double addValue = GetNumber("Enter value to add: ");
                    _memory += addValue;
                    LogAndDisplay($"Added {addValue} to memory, new value: {_memory}");
                    break;
                case "4":
                    double subValue = GetNumber("Enter value to subtract: ");
                    _memory -= subValue;
                    LogAndDisplay($"Subtracted {subValue} from memory, new value: {_memory}");
                    break;
                case "5":
                    _memory = 0;
                    LogAndDisplay("Memory cleared");
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
        
        private void DisplayHistory()
        {
            Console.Clear();
            Console.WriteLine("📜 CALCULATION HISTORY");
            Console.WriteLine(new string('-', 30));
            
            if (_history.Count == 0)
            {
                Console.WriteLine("No calculations in history.");
            }
            else
            {
                for (int i = 0; i < _history.Count; i++)
                {
                    Console.WriteLine($"{i + 1:D2}. {_history[i]}");
                }
            }
            
            Console.WriteLine($"\nTotal calculations: {_history.Count}");
        }
        
        private void ClearHistory()
        {
            _history.Clear();
            Console.WriteLine("History cleared!");
        }
        
        private double GetNumber(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                
                if (double.TryParse(input, out double result))
                {
                    return result;
                }
                
                Console.WriteLine("Invalid number. Please try again.");
            }
        }
        
        private void LogAndDisplay(string calculation)
        {
            Console.WriteLine(calculation);
            _history.Add($"{DateTime.Now:HH:mm:ss} - {calculation}");
        }
        
        private void DisplayGoodbye()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thank you for using Advanced Calculator!");
            Console.WriteLine($"You performed {_history.Count} calculations today.");
            Console.WriteLine("See you tomorrow for Day 03: Control Structures & Methods!");
            Console.ResetColor();
        }
    }
}
```

## 🎯 Exercises

### Exercise 1: Data Type Explorer
Create a program that demonstrates the range and precision of all numeric types.

### Exercise 2: Temperature Converter
Build a converter that handles Celsius, Fahrenheit, and Kelvin with proper decimal precision.

### Exercise 3: Binary Calculator
Create a calculator that works entirely with binary numbers.

### Exercise 4: Variable Lifetime Tracker
Build a program that demonstrates variable scopes and lifetimes.

## 📚 Summary

Today you mastered:
- ✅ Complete C# type system (value types & reference types)
- ✅ Type conversions and casting techniques
- ✅ Nullable types and null handling
- ✅ All C# operators and their usage
- ✅ Variable naming conventions and best practices
- ✅ Memory management concepts
- ✅ Performance considerations

**Tomorrow: Day 03 - Control Structures & Methods** 🚀