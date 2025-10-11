# Day 03: Control Structures & Methods

## 🎯 Learning Objectives
By the end of today you will:
- Understand and use control flow constructs in C# (if/else, switch, loops)
- Learn to write reusable methods and understand parameter passing
- Use method overloading, optional parameters, and named arguments
- Understand recursion and when to use it
- Learn best practices for method design and unit-testable code

## 1. Theory: Control Flow Overview
- Conditional statements: `if`, `else if`, `else`
- Pattern matching and `switch` expressions
- Loops: `for`, `foreach`, `while`, `do-while`
- Jump statements: `break`, `continue`, `return`
- Exception-driven flow vs conditional checks

## 2. Hands-on Examples

### Example 1: Conditionals (If / Else / Else If)
Create `ConditionalsExample.cs` in this folder:

```csharp
using System;

namespace Day03Examples
{
    class ConditionalsExample
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a score (0-100): ");
            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int score))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            if (score < 0 || score > 100)
            {
                Console.WriteLine("Score should be between 0 and 100.");
            }
            else if (score >= 90)
            {
                Console.WriteLine("Grade: A");
            }
            else if (score >= 80)
            {
                Console.WriteLine("Grade: B");
            }
            else if (score >= 70)
            {
                Console.WriteLine("Grade: C");
            }
            else if (score >= 60)
            {
                Console.WriteLine("Grade: D");
            }
            else
            {
                Console.WriteLine("Grade: F");
            }
        }
    }
}
```

### Example 2: `switch` and Pattern Matching
Create `SwitchPatternExample.cs`:

```csharp
using System;

namespace Day03Examples
{
    class SwitchPatternExample
    {
        static void Main()
        {
            Console.Write("Enter a value (int or text): ");
            string? raw = Console.ReadLine();

            // Try integer first
            if (int.TryParse(raw, out int n))
            {
                var message = n switch
                {
                    < 0 => "Negative number",
                    0 => "Zero",
                    > 0 and < 10 => "Small positive",
                    >= 10 => "Large number",
                    _ => "Unknown"
                };
                Console.WriteLine(message);
            }
            else
            {
                var message = raw switch
                {
                    null => "Empty input",
                    string s when s.Length == 0 => "Empty string",
                    string s when s.Equals("hello", StringComparison.OrdinalIgnoreCase) => "Greeting detected",
                    _ => $"You typed: {raw}"
                };
                Console.WriteLine(message);
            }
        }
    }
}
```

### Example 3: Loops (for, foreach, while, do-while)
Create `LoopsExample.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace Day03Examples
{
    class LoopsExample
    {
        static void Main()
        {
            // for loop
            for (int i = 0; i < 5; i++)
                Console.WriteLine($"for: {i}");

            // foreach loop
            var list = new List<string> { "apple", "banana", "cherry" };
            foreach (var item in list)
                Console.WriteLine($"foreach: {item}");

            // while loop
            int cnt = 3;
            while (cnt-- > 0)
                Console.WriteLine($"while: {cnt}");

            // do-while
            int x = 0;
            do
            {
                Console.WriteLine($"do-while: {x}");
                x++;
            } while (x < 2);

            // break and continue
            for (int i = 0; i < 10; i++)
            {
                if (i == 7) break;          // stop the loop
                if (i % 2 == 0) continue;   // skip even numbers
                Console.WriteLine($"odd: {i}");
            }
        }
    }
}
```

### Example 4: Methods, Parameters and Overloading
Create `MethodsExample.cs`:

```csharp
using System;

namespace Day03Examples
{
    class MethodsExample
    {
        static void Main()
        {
            Console.WriteLine("Sum: " + Sum(2, 3));
            Console.WriteLine("Sum (optional): " + Sum(2));

            Console.WriteLine(Greet("Alice"));
            Console.WriteLine(Greet("Bob", "Mr."));

            Console.WriteLine("Factorial(5): " + Factorial(5));
        }

        // Simple method
        static int Sum(int a, int b = 0) => a + b; // optional parameter

        // Overloaded methods
        static string Greet(string name) => Greet(name, "");
        static string Greet(string name, string title) => string.IsNullOrEmpty(title) ? $"Hello, {name}" : $"Hello, {title} {name}";

        // Recursive method example
        static long Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }
    }
}
```

## 3. Best Practices for Methods
- Keep methods small and focused (single responsibility)
- Prefer pure functions where possible (no side effects)
- Use meaningful parameter names and avoid too many parameters
- Validate inputs and throw appropriate exceptions for invalid state
- Favor composition over inheritance
- Make methods testable: avoid static global state

## 4. Exercises
1. Write a program that reads numbers until the user types `done` and prints the average, min, and max.
2. Implement a `PrimeChecker.IsPrime(int n)` method and write a loop that prints primes up to 1,000.
3. Implement Fibonacci with iterative and recursive methods and compare performance.
4. Create a small menu-driven calculator that uses methods for operations and keeps a history list.

## 5. How to run examples
Open PowerShell in the folder for the example you want and run:

```powershell
# build and run current project (if using dotnet console project)
dotnet run --project .\ConditionalsExample\

# or directly compile single file for quick testing
dotnet new console -n tmp && copy .\ConditionalsExample.cs tmp\Program.cs ; cd tmp ; dotnet run ; cd .. ; rmdir /s /q tmp
```

(If you prefer Visual Studio, create a new Console project and add the `.cs` files.)

## 6. Summary
- Control flow and methods are building blocks for any application logic.
- Practice writing small, well-typed functions and favor readability and testability.

Tomorrow: Day 04 — Object-Oriented Programming (classes, encapsulation, inheritance, interfaces).