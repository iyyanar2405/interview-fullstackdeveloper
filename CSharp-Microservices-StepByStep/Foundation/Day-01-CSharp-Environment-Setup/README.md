# Day 01: C# Environment Setup & Basic Syntax

## 🎯 Learning Objectives
By the end of today, you will:
- Set up a complete C# development environment
- Understand the .NET ecosystem and its components
- Write your first C# program
- Master basic C# syntax and program structure
- Use the command line tools effectively
- Create and run console applications

## 📖 Theory: Understanding C# and .NET

### What is C#?
C# (pronounced "C-Sharp") is a modern, object-oriented programming language developed by Microsoft. It's part of the .NET ecosystem and is widely used for:
- Web applications (ASP.NET Core)
- Desktop applications (WPF, WinUI)
- Mobile applications (Xamarin, .NET MAUI)
- Cloud applications (Azure)
- **Microservices** (our focus!)

### The .NET Ecosystem
```
┌─────────────────────────────────────┐
│              .NET 8                 │
├─────────────────────────────────────┤
│  Runtime Components                 │
│  ├── Common Language Runtime (CLR) │
│  ├── Base Class Library (BCL)      │
│  └── Just-In-Time Compiler (JIT)   │
├─────────────────────────────────────┤
│  Development Tools                  │
│  ├── .NET CLI                      │
│  ├── NuGet Package Manager         │
│  └── MSBuild                       │
├─────────────────────────────────────┤
│  Application Frameworks            │
│  ├── ASP.NET Core (Web)            │
│  ├── Entity Framework (Data)       │
│  └── ML.NET (Machine Learning)     │
└─────────────────────────────────────┘
```

### Why C# for Microservices?
- **Performance**: Fast execution and low memory footprint
- **Cross-platform**: Runs on Windows, macOS, and Linux
- **Rich ecosystem**: Extensive libraries and tools
- **Enterprise-ready**: Battle-tested in production environments
- **Container-friendly**: Excellent Docker support
- **Cloud-native**: First-class Azure and AWS support

## 💻 Hands-on: Environment Setup

### Step 1: Verify .NET Installation
Open your terminal/command prompt and run:

```bash
dotnet --version
```

Expected output:
```
8.0.xxx
```

If not installed, follow the installation guide in GETTING-STARTED.md.

### Step 2: Explore .NET CLI Commands
The .NET CLI is your primary tool for C# development:

```bash
# Get help
dotnet --help

# List installed SDKs
dotnet --list-sdks

# List installed runtimes
dotnet --list-runtimes

# Create a new project
dotnet new --help
```

### Step 3: Create Your First Project
```bash
# Create a new directory for today's work
mkdir Day01-CSharp-Basics
cd Day01-CSharp-Basics

# Create a new console application
dotnet new console -n HelloWorld
cd HelloWorld

# Examine the project structure
ls -la  # On Windows: dir
```

You should see:
```
HelloWorld/
├── HelloWorld.csproj    # Project file
├── Program.cs          # Main program file
└── obj/               # Build artifacts
```

### Step 4: Understanding the Project File
Open `HelloWorld.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

**Key elements:**
- `OutputType`: Specifies this creates an executable
- `TargetFramework`: We're using .NET 8
- `Nullable`: Enables nullable reference types
- `ImplicitUsings`: Automatically includes common namespaces

## 🔧 Practical Examples

### Example 1: Basic C# Syntax
Replace the content of `Program.cs`:

```csharp
// Using directives - import namespaces
using System;

// Namespace declaration
namespace HelloWorld
{
    // Class declaration
    class Program
    {
        // Main method - entry point of the application
        static void Main(string[] args)
        {
            // Variable declarations
            string message = "Hello, C# Microservices World!";
            int year = 2024;
            double version = 8.0;
            bool isLearning = true;
            
            // Output to console
            Console.WriteLine(message);
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($".NET Version: {version}");
            Console.WriteLine($"Currently Learning: {isLearning}");
            
            // Getting user input
            Console.Write("Enter your name: ");
            string? userName = Console.ReadLine();
            
            // String interpolation
            Console.WriteLine($"Welcome to C# Microservices, {userName}!");
            
            // Wait for user input before closing
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### Example 2: Modern C# Syntax (Top-level programs)
Create a new file `ModernProgram.cs`:

```csharp
// Top-level program (C# 9+)
// No need for Main method, class, or namespace

using System;

// Direct executable statements
string message = "Modern C# Syntax";
Console.WriteLine(message);

// Method declaration at the bottom
void DisplayWelcomeMessage(string name)
{
    Console.WriteLine($"Welcome {name} to C# Microservices!");
    Console.WriteLine("Today we start our journey to becoming experts!");
}

// Call the method
Console.Write("Enter your name: ");
string? name = Console.ReadLine();
DisplayWelcomeMessage(name ?? "Developer");
```

### Example 3: Data Types and Variables
Create `DataTypesExample.cs`:

```csharp
using System;

namespace HelloWorld
{
    class DataTypesExample
    {
        static void DemonstrateDataTypes()
        {
            // Numeric types
            byte smallNumber = 255;              // 0 to 255
            short mediumNumber = 32767;          // -32,768 to 32,767
            int regularNumber = 2147483647;      // -2.1B to 2.1B
            long bigNumber = 9223372036854775807; // Very large range
            
            // Floating-point types
            float preciseNumber = 3.14159f;      // 7 digits precision
            double morePrecise = 3.14159265359;  // 15-17 digits precision
            decimal financial = 99.99m;         // 28-29 digits precision (for money)
            
            // Character and string types
            char singleCharacter = 'A';
            string text = "Hello Microservices";
            
            // Boolean type
            bool isActive = true;
            
            // Display all values
            Console.WriteLine("=== Data Types Demo ===");
            Console.WriteLine($"Byte: {smallNumber}");
            Console.WriteLine($"Short: {mediumNumber}");
            Console.WriteLine($"Int: {regularNumber}");
            Console.WriteLine($"Long: {bigNumber}");
            Console.WriteLine($"Float: {preciseNumber}");
            Console.WriteLine($"Double: {morePrecise}");
            Console.WriteLine($"Decimal: {financial}");
            Console.WriteLine($"Char: {singleCharacter}");
            Console.WriteLine($"String: {text}");
            Console.WriteLine($"Boolean: {isActive}");
            
            // Type information
            Console.WriteLine("\n=== Type Information ===");
            Console.WriteLine($"Int size: {sizeof(int)} bytes");
            Console.WriteLine($"Double size: {sizeof(double)} bytes");
            Console.WriteLine($"Decimal size: {sizeof(decimal)} bytes");
        }
    }
}
```

### Example 4: String Operations
Create `StringOperations.cs`:

```csharp
using System;

namespace HelloWorld
{
    class StringOperations
    {
        static void DemonstrateStrings()
        {
            Console.WriteLine("=== String Operations Demo ===");
            
            // String declaration and initialization
            string firstName = "John";
            string lastName = "Developer";
            
            // String concatenation
            string fullName1 = firstName + " " + lastName;
            string fullName2 = string.Concat(firstName, " ", lastName);
            
            // String interpolation (recommended)
            string fullName3 = $"{firstName} {lastName}";
            
            Console.WriteLine($"Concatenation: {fullName1}");
            Console.WriteLine($"String.Concat: {fullName2}");
            Console.WriteLine($"Interpolation: {fullName3}");
            
            // String properties and methods
            string message = "  Welcome to C# Microservices  ";
            
            Console.WriteLine($"\nOriginal: '{message}'");
            Console.WriteLine($"Length: {message.Length}");
            Console.WriteLine($"Trimmed: '{message.Trim()}'");
            Console.WriteLine($"Upper: {message.ToUpper()}");
            Console.WriteLine($"Lower: {message.ToLower()}");
            Console.WriteLine($"Contains 'C#': {message.Contains("C#")}");
            Console.WriteLine($"Starts with 'Welcome': {message.Trim().StartsWith("Welcome")}");
            Console.WriteLine($"Replace 'C#' with 'CSharp': {message.Replace("C#", "CSharp")}");
            
            // String splitting
            string technologies = "C#,ASP.NET,Docker,Kubernetes,Azure";
            string[] techArray = technologies.Split(',');
            
            Console.WriteLine($"\nTechnologies: {technologies}");
            Console.WriteLine("Split result:");
            foreach (string tech in techArray)
            {
                Console.WriteLine($"  - {tech.Trim()}");
            }
            
            // StringBuilder for efficient string building
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Building microservices with:");
            foreach (string tech in techArray)
            {
                sb.AppendLine($"  • {tech.Trim()}");
            }
            
            Console.WriteLine($"\nStringBuilder result:\n{sb}");
        }
    }
}
```

### Example 5: Console Input/Output
Create `InputOutputExample.cs`:

```csharp
using System;

namespace HelloWorld
{
    class InputOutputExample
    {
        static void DemonstrateInputOutput()
        {
            Console.WriteLine("=== Console I/O Demo ===");
            
            // Basic output
            Console.WriteLine("Welcome to C# Microservices Course!");
            Console.Write("Your instructor: ");
            Console.WriteLine("AI Assistant");
            
            // Formatted output
            string courseName = "C# Microservices";
            int totalDays = 60;
            double completionRate = 0.0167; // Day 1 of 60
            
            Console.WriteLine($"\nCourse: {courseName}");
            Console.WriteLine($"Duration: {totalDays} days");
            Console.WriteLine($"Progress: {completionRate:P2}"); // Percentage format
            
            // Getting user input
            Console.Write("\nEnter your experience level (Beginner/Intermediate/Advanced): ");
            string? experience = Console.ReadLine();
            
            Console.Write("How many hours can you dedicate daily? ");
            string? hoursInput = Console.ReadLine();
            
            // Input validation and conversion
            if (int.TryParse(hoursInput, out int dailyHours))
            {
                int totalHours = dailyHours * totalDays;
                Console.WriteLine($"\nGreat! With {dailyHours} hours daily:");
                Console.WriteLine($"Total learning time: {totalHours} hours");
                Console.WriteLine($"Experience level: {experience ?? "Not specified"}");
                
                // Personalized message
                string motivation = experience?.ToLower() switch
                {
                    "beginner" => "Perfect! This course will take you from zero to hero!",
                    "intermediate" => "Excellent! You'll deepen your existing knowledge!",
                    "advanced" => "Great! You'll master enterprise-level patterns!",
                    _ => "Regardless of your level, you'll learn something new!"
                };
                
                Console.WriteLine($"\n{motivation}");
            }
            else
            {
                Console.WriteLine("Invalid input for hours. Please enter a number.");
            }
            
            // Colored output (works in most terminals)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ Day 1 Setup Complete!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🚀 Ready for Day 2: Data Types & Variables!");
            Console.ResetColor();
        }
    }
}
```

## 🎯 Mini-Project: Personal Information System

Let's build a small console application that demonstrates everything we've learned:

Create `PersonalInfoSystem.cs`:

```csharp
using System;
using System.Text;

namespace HelloWorld
{
    /// <summary>
    /// A simple personal information system demonstrating C# basics
    /// </summary>
    class PersonalInfoSystem
    {
        static void RunPersonalInfoSystem()
        {
            Console.Clear();
            DisplayHeader();
            
            // Collect user information
            var userInfo = CollectUserInformation();
            
            // Display collected information
            DisplayUserInformation(userInfo);
            
            // Save to "file" (simulate)
            SimulateSaveToFile(userInfo);
            
            DisplayFooter();
        }
        
        static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║    Personal Information System       ║");
            Console.WriteLine("║         C# Microservices Day 1       ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
        
        static UserInfo CollectUserInformation()
        {
            var info = new UserInfo();
            
            Console.WriteLine("Please provide your information:");
            Console.WriteLine(new string('-', 40));
            
            // Name
            Console.Write("Full Name: ");
            info.FullName = Console.ReadLine() ?? "Unknown";
            
            // Age
            Console.Write("Age: ");
            if (int.TryParse(Console.ReadLine(), out int age))
            {
                info.Age = age;
            }
            
            // Email
            Console.Write("Email: ");
            info.Email = Console.ReadLine() ?? "";
            
            // Programming experience
            Console.Write("Years of programming experience: ");
            if (double.TryParse(Console.ReadLine(), out double experience))
            {
                info.ProgrammingExperience = experience;
            }
            
            // Favorite programming language
            Console.Write("Favorite programming language: ");
            info.FavoriteLanguage = Console.ReadLine() ?? "C#";
            
            // Learning goals
            Console.Write("What do you hope to learn? ");
            info.LearningGoals = Console.ReadLine() ?? "";
            
            return info;
        }
        
        static void DisplayUserInformation(UserInfo info)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Information Summary:");
            Console.ResetColor();
            Console.WriteLine(new string('=', 50));
            
            Console.WriteLine($"Name: {info.FullName}");
            Console.WriteLine($"Age: {info.Age}");
            Console.WriteLine($"Email: {info.Email}");
            Console.WriteLine($"Programming Experience: {info.ProgrammingExperience} years");
            Console.WriteLine($"Favorite Language: {info.FavoriteLanguage}");
            Console.WriteLine($"Learning Goals: {info.LearningGoals}");
            
            // Generate some insights
            string experienceLevel = info.ProgrammingExperience switch
            {
                < 1 => "Beginner",
                < 3 => "Junior",
                < 7 => "Mid-level",
                < 10 => "Senior",
                _ => "Expert"
            };
            
            Console.WriteLine($"Experience Level: {experienceLevel}");
            
            // Personalized recommendation
            string recommendation = experienceLevel switch
            {
                "Beginner" => "Focus on fundamentals and practice daily coding",
                "Junior" => "Build more projects and learn design patterns",
                "Mid-level" => "Master advanced concepts and system architecture",
                "Senior" => "Lead projects and mentor others",
                "Expert" => "Contribute to open source and share knowledge",
                _ => "Keep learning and growing!"
            };
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Recommendation: {recommendation}");
            Console.ResetColor();
        }
        
        static void SimulateSaveToFile(UserInfo info)
        {
            Console.WriteLine();
            Console.WriteLine("Saving information...");
            
            // Simulate file content
            var sb = new StringBuilder();
            sb.AppendLine($"User Information - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Name: {info.FullName}");
            sb.AppendLine($"Age: {info.Age}");
            sb.AppendLine($"Email: {info.Email}");
            sb.AppendLine($"Experience: {info.ProgrammingExperience} years");
            sb.AppendLine($"Favorite Language: {info.FavoriteLanguage}");
            sb.AppendLine($"Goals: {info.LearningGoals}");
            
            // In a real application, you would save this to a file
            // System.IO.File.WriteAllText("user_info.txt", sb.ToString());
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Information saved successfully!");
            Console.ResetColor();
            
            Console.WriteLine("\nFile content preview:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(sb.ToString());
            Console.ResetColor();
        }
        
        static void DisplayFooter()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Thank you for using Personal Information System!");
            Console.WriteLine("Tomorrow we'll learn about data types and control structures.");
            Console.ResetColor();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
    
    /// <summary>
    /// Data class to hold user information
    /// </summary>
    class UserInfo
    {
        public string FullName { get; set; } = "";
        public int Age { get; set; }
        public string Email { get; set; } = "";
        public double ProgrammingExperience { get; set; }
        public string FavoriteLanguage { get; set; } = "";
        public string LearningGoals { get; set; } = "";
    }
}
```

## 🏭 Updated Program.cs

Replace your `Program.cs` with this comprehensive example:

```csharp
using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# Microservices - Day 01 Examples");
            Console.WriteLine("==================================");
            
            bool running = true;
            
            while (running)
            {
                DisplayMenu();
                
                string? choice = Console.ReadLine();
                Console.Clear();
                
                switch (choice)
                {
                    case "1":
                        RunBasicExample();
                        break;
                    case "2":
                        DataTypesExample.DemonstrateDataTypes();
                        break;
                    case "3":
                        StringOperations.DemonstrateStrings();
                        break;
                    case "4":
                        InputOutputExample.DemonstrateInputOutput();
                        break;
                    case "5":
                        PersonalInfoSystem.RunPersonalInfoSystem();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Goodbye! See you tomorrow for Day 02!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                
                if (running && choice != "5")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        
        static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║         Day 01 - C# Basics           ║");
            Console.WriteLine("╠═══════════════════════════════════════╣");
            Console.WriteLine("║ 1. Basic C# Syntax                   ║");
            Console.WriteLine("║ 2. Data Types Demo                   ║");
            Console.WriteLine("║ 3. String Operations                 ║");
            Console.WriteLine("║ 4. Input/Output Examples             ║");
            Console.WriteLine("║ 5. Personal Info System (Project)    ║");
            Console.WriteLine("║ 0. Exit                              ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write("\nSelect an option: ");
        }
        
        static void RunBasicExample()
        {
            Console.WriteLine("=== Basic C# Syntax Demo ===");
            
            // Variables
            string message = "Hello, C# Microservices!";
            int currentDay = 1;
            double progress = 1.0 / 60.0 * 100;
            
            Console.WriteLine(message);
            Console.WriteLine($"Day: {currentDay}");
            Console.WriteLine($"Course Progress: {progress:F2}%");
            
            // Simple calculation
            int totalDays = 60;
            int remainingDays = totalDays - currentDay;
            
            Console.WriteLine($"Days remaining: {remainingDays}");
            
            // Current date and time
            DateTime now = DateTime.Now;
            Console.WriteLine($"Started on: {now:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
```

## 🔄 Running Your Programs

### Method 1: Using .NET CLI
```bash
# Build the project
dotnet build

# Run the project
dotnet run
```

### Method 2: Using Visual Studio
1. Open the project in Visual Studio
2. Press F5 to run with debugging
3. Or Ctrl+F5 to run without debugging

### Method 3: Direct execution
```bash
# Build and run in one command
dotnet run --project HelloWorld
```

## ✅ Exercises

### Exercise 1: Calculator
Create a simple calculator that:
1. Asks for two numbers
2. Asks for an operation (+, -, *, /)
3. Displays the result
4. Handles division by zero

### Exercise 2: Student Grade System
Create a program that:
1. Collects student name and 5 test scores
2. Calculates average score
3. Determines letter grade (A, B, C, D, F)
4. Displays a report

### Exercise 3: Text Analyzer
Create a program that:
1. Takes a sentence as input
2. Counts words, characters, and vowels
3. Finds the longest word
4. Displays statistics

## 🎯 Self-Assessment

Rate your understanding (1-5 scale):
- [ ] C# development environment setup
- [ ] Basic C# syntax and structure
- [ ] Variable declaration and initialization
- [ ] String operations and interpolation
- [ ] Console input/output operations
- [ ] Project creation and compilation

## 📚 Additional Resources

### Documentation
- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)

### Practice Platforms
- [LeetCode C# Track](https://leetcode.com/problemset/all/?search=c%23)
- [HackerRank C# Domain](https://www.hackerrank.com/domains/tutorials/30-days-of-code)

### Video Resources
- Channel 9 C# Fundamentals
- Microsoft Learn C# Path

## 🚀 Tomorrow's Preview

**Day 02: Data Types, Variables & Operators**
- Deep dive into C# type system
- Value types vs reference types
- Type conversion and casting
- Operators and expressions
- Nullable types and null handling

---

## 📝 Day 01 Summary

Today you accomplished:
- ✅ Set up complete C# development environment
- ✅ Learned basic C# syntax and program structure
- ✅ Mastered variable declaration and string operations
- ✅ Built your first interactive console application
- ✅ Understanding of .NET CLI tools

**Congratulations! You've completed Day 01 of your C# Microservices journey!** 🎉

*Total estimated time: 3-4 hours*
*Lines of code written: ~200*
*New concepts learned: 15+*