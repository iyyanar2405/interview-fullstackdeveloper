# 📊 Arrays and Lists with LINQ - Complete Guide

*Master arrays, lists, and collection operations using LINQ in C#*

## 🎯 Learning Objectives

By the end of this guide, you'll master:
- Array and List operations using LINQ
- Advanced LINQ queries for data manipulation
- Performance considerations when using LINQ
- Real-world problem-solving with arrays and LINQ

## 📚 Table of Contents

1. [Fundamentals](#fundamentals)
2. [Basic Array Operations](#basic-array-operations)
3. [List Operations with LINQ](#list-operations-with-linq)
4. [Advanced LINQ Techniques](#advanced-linq-techniques)
5. [Performance Optimization](#performance-optimization)
6. [Practice Problems](#practice-problems)

---

## 🔰 Fundamentals

### Essential Imports

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
```

### Array vs List vs IEnumerable

```csharp
public class ArrayListFundamentals
{
    public static void DemonstrateCollectionTypes()
    {
        // Traditional Array
        int[] array = { 1, 2, 3, 4, 5 };
        
        // List<T> - Dynamic array
        List<int> list = new List<int> { 1, 2, 3, 4, 5 };
        
        // IEnumerable - LINQ's foundation
        IEnumerable<int> enumerable = Enumerable.Range(1, 5);
        
        Console.WriteLine("Array Length: " + array.Length);
        Console.WriteLine("List Count: " + list.Count);
        Console.WriteLine("Enumerable Count: " + enumerable.Count());
    }
}
```

---

## 🔧 Basic Array Operations

### 1. Array Creation and Initialization

```csharp
public class ArrayCreation
{
    // Create arrays using LINQ
    public static int[] CreateSequentialArray(int start, int count)
    {
        return Enumerable.Range(start, count).ToArray();
    }
    
    // Create array with repeated values
    public static string[] CreateRepeatedArray(string value, int count)
    {
        return Enumerable.Repeat(value, count).ToArray();
    }
    
    // Create array from conditions
    public static int[] CreateConditionalArray(int max)
    {
        return Enumerable.Range(1, max)
                         .Where(x => x % 2 == 0)  // Even numbers only
                         .ToArray();
    }
    
    public static void DemonstrateArrayCreation()
    {
        // Sequential: [1, 2, 3, 4, 5]
        var sequential = CreateSequentialArray(1, 5);
        Console.WriteLine("Sequential: [" + string.Join(", ", sequential) + "]");
        
        // Repeated: ["Hello", "Hello", "Hello"]
        var repeated = CreateRepeatedArray("Hello", 3);
        Console.WriteLine("Repeated: [" + string.Join(", ", repeated) + "]");
        
        // Even numbers: [2, 4, 6, 8, 10]
        var evens = CreateConditionalArray(10);
        Console.WriteLine("Evens: [" + string.Join(", ", evens) + "]");
    }
}
```

### 2. Array Searching with LINQ

```csharp
public class ArraySearching
{
    private static readonly int[] numbers = { 64, 34, 25, 12, 22, 11, 90, 88, 76, 50 };
    
    // Find single element
    public static int FindElement(int target)
    {
        // Using FirstOrDefault - returns 0 if not found
        return numbers.FirstOrDefault(x => x == target);
    }
    
    // Find element with condition
    public static int FindFirstGreaterThan(int threshold)
    {
        return numbers.FirstOrDefault(x => x > threshold);
    }
    
    // Find all elements matching condition
    public static int[] FindAllEven()
    {
        return numbers.Where(x => x % 2 == 0).ToArray();
    }
    
    // Find elements in range
    public static int[] FindInRange(int min, int max)
    {
        return numbers.Where(x => x >= min && x <= max).ToArray();
    }
    
    // Check if any/all elements meet condition
    public static bool HasElementsGreaterThan(int threshold)
    {
        return numbers.Any(x => x > threshold);
    }
    
    public static bool AllElementsPositive()
    {
        return numbers.All(x => x > 0);
    }
    
    // Advanced: Find elements with indices
    public static (int value, int index)[] FindWithIndices(Func<int, bool> predicate)
    {
        return numbers
            .Select((value, index) => new { value, index })
            .Where(x => predicate(x.value))
            .Select(x => (x.value, x.index))
            .ToArray();
    }
    
    public static void DemonstrateSearching()
    {
        Console.WriteLine("Original array: [" + string.Join(", ", numbers) + "]");
        
        // Find specific element
        var found = FindElement(25);
        Console.WriteLine($"Found 25: {found}");
        
        // Find first greater than threshold
        var firstGreater = FindFirstGreaterThan(50);
        Console.WriteLine($"First > 50: {firstGreater}");
        
        // Find all even numbers
        var evens = FindAllEven();
        Console.WriteLine("Even numbers: [" + string.Join(", ", evens) + "]");
        
        // Find in range
        var inRange = FindInRange(20, 60);
        Console.WriteLine("Range [20-60]: [" + string.Join(", ", inRange) + "]");
        
        // Check conditions
        Console.WriteLine($"Has elements > 80: {HasElementsGreaterThan(80)}");
        Console.WriteLine($"All positive: {AllElementsPositive()}");
        
        // Find with indices
        var withIndices = FindWithIndices(x => x > 70);
        Console.WriteLine("Elements > 70 with indices:");
        foreach (var (value, index) in withIndices)
        {
            Console.WriteLine($"  Value: {value}, Index: {index}");
        }
    }
}
```

### 3. Array Manipulation

```csharp
public class ArrayManipulation
{
    // Transform elements
    public static int[] Square(int[] array)
    {
        return array.Select(x => x * x).ToArray();
    }
    
    // Filter and transform in one operation
    public static string[] GetEvenSquareStrings(int[] array)
    {
        return array
            .Where(x => x % 2 == 0)     // Filter evens
            .Select(x => x * x)          // Square them
            .Select(x => x.ToString())   // Convert to string
            .ToArray();
    }
    
    // Concatenate arrays
    public static T[] ConcatenateArrays<T>(params T[][] arrays)
    {
        return arrays.SelectMany(arr => arr).ToArray();
    }
    
    // Remove duplicates
    public static T[] RemoveDuplicates<T>(T[] array)
    {
        return array.Distinct().ToArray();
    }
    
    // Reverse array using LINQ
    public static T[] ReverseArray<T>(T[] array)
    {
        return array.Reverse().ToArray();
    }
    
    // Chunk array into smaller arrays
    public static T[][] ChunkArray<T>(T[] array, int chunkSize)
    {
        return array
            .Select((value, index) => new { value, index })
            .GroupBy(x => x.index / chunkSize)
            .Select(group => group.Select(x => x.value).ToArray())
            .ToArray();
    }
    
    public static void DemonstrateManipulation()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 2, 3, 6 };
        
        // Square all numbers
        var squared = Square(numbers);
        Console.WriteLine("Squared: [" + string.Join(", ", squared) + "]");
        
        // Even squares as strings
        var evenSquareStrings = GetEvenSquareStrings(numbers);
        Console.WriteLine("Even squares as strings: [" + string.Join(", ", evenSquareStrings) + "]");
        
        // Concatenate multiple arrays
        int[] arr1 = { 1, 2, 3 };
        int[] arr2 = { 4, 5, 6 };
        int[] arr3 = { 7, 8, 9 };
        var concatenated = ConcatenateArrays(arr1, arr2, arr3);
        Console.WriteLine("Concatenated: [" + string.Join(", ", concatenated) + "]");
        
        // Remove duplicates
        var unique = RemoveDuplicates(numbers);
        Console.WriteLine("Unique: [" + string.Join(", ", unique) + "]");
        
        // Reverse
        var reversed = ReverseArray(numbers);
        Console.WriteLine("Reversed: [" + string.Join(", ", reversed) + "]");
        
        // Chunk into groups of 3
        var chunked = ChunkArray(numbers, 3);
        Console.WriteLine("Chunked (size 3):");
        for (int i = 0; i < chunked.Length; i++)
        {
            Console.WriteLine($"  Chunk {i}: [" + string.Join(", ", chunked[i]) + "]");
        }
    }
}
```

---

## 📋 List Operations with LINQ

### 1. Dynamic List Operations

```csharp
public class ListOperations
{
    // Add elements conditionally
    public static List<int> AddElementsConditionally(List<int> list, int[] candidates)
    {
        var newList = new List<int>(list);
        
        // Add elements that meet criteria
        var toAdd = candidates
            .Where(x => x > 0 && !newList.Contains(x))
            .ToList();
            
        newList.AddRange(toAdd);
        return newList;
    }
    
    // Remove elements using LINQ logic
    public static List<T> RemoveElementsWhere<T>(List<T> list, Func<T, bool> predicate)
    {
        return list.Where(x => !predicate(x)).ToList();
    }
    
    // Update elements in place
    public static List<T> UpdateElementsWhere<T>(List<T> list, Func<T, bool> condition, Func<T, T> update)
    {
        return list
            .Select(item => condition(item) ? update(item) : item)
            .ToList();
    }
    
    // Partition list into multiple lists
    public static (List<T> matching, List<T> nonMatching) PartitionList<T>(List<T> list, Func<T, bool> predicate)
    {
        var grouped = list.ToLookup(predicate);
        return (grouped[true].ToList(), grouped[false].ToList());
    }
    
    public static void DemonstrateListOperations()
    {
        var originalList = new List<int> { 1, 2, 3, 4, 5 };
        var candidates = new int[] { 3, 6, 7, -1, 8 };
        
        Console.WriteLine("Original list: [" + string.Join(", ", originalList) + "]");
        Console.WriteLine("Candidates: [" + string.Join(", ", candidates) + "]");
        
        // Add conditionally
        var extended = AddElementsConditionally(originalList, candidates);
        Console.WriteLine("After conditional add: [" + string.Join(", ", extended) + "]");
        
        // Remove even numbers
        var oddOnly = RemoveElementsWhere(extended, x => x % 2 == 0);
        Console.WriteLine("Odd numbers only: [" + string.Join(", ", oddOnly) + "]");
        
        // Double numbers greater than 3
        var updated = UpdateElementsWhere(extended, x => x > 3, x => x * 2);
        Console.WriteLine("Numbers >3 doubled: [" + string.Join(", ", updated) + "]");
        
        // Partition into even/odd
        var (evens, odds) = PartitionList(extended, x => x % 2 == 0);
        Console.WriteLine("Evens: [" + string.Join(", ", evens) + "]");
        Console.WriteLine("Odds: [" + string.Join(", ", odds) + "]");
    }
}
```

### 2. List Aggregation and Statistics

```csharp
public class ListAggregation
{
    public static void CalculateStatistics(List<double> numbers)
    {
        if (!numbers.Any())
        {
            Console.WriteLine("Empty list - no statistics available");
            return;
        }
        
        // Basic statistics using LINQ
        var count = numbers.Count();
        var sum = numbers.Sum();
        var average = numbers.Average();
        var min = numbers.Min();
        var max = numbers.Max();
        
        // Advanced calculations
        var median = CalculateMedian(numbers);
        var variance = CalculateVariance(numbers);
        var standardDeviation = Math.Sqrt(variance);
        
        // Frequency analysis
        var frequency = numbers
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());
        
        Console.WriteLine("=== Statistics ===");
        Console.WriteLine($"Count: {count}");
        Console.WriteLine($"Sum: {sum:F2}");
        Console.WriteLine($"Average: {average:F2}");
        Console.WriteLine($"Min: {min:F2}");
        Console.WriteLine($"Max: {max:F2}");
        Console.WriteLine($"Median: {median:F2}");
        Console.WriteLine($"Variance: {variance:F2}");
        Console.WriteLine($"Std Dev: {standardDeviation:F2}");
        
        Console.WriteLine("\n=== Frequency (Top 5) ===");
        foreach (var (value, freq) in frequency.Take(5))
        {
            Console.WriteLine($"{value:F2}: {freq} times");
        }
    }
    
    private static double CalculateMedian(List<double> numbers)
    {
        var sorted = numbers.OrderBy(x => x).ToList();
        var count = sorted.Count;
        
        return count % 2 == 0
            ? sorted.Skip(count / 2 - 1).Take(2).Average()
            : sorted[count / 2];
    }
    
    private static double CalculateVariance(List<double> numbers)
    {
        var average = numbers.Average();
        return numbers.Select(x => Math.Pow(x - average, 2)).Average();
    }
    
    // Group-based aggregation
    public static void GroupedAggregation<T, TKey>(List<T> items, Func<T, TKey> keySelector, Func<T, double> valueSelector)
    {
        var grouped = items
            .GroupBy(keySelector)
            .Select(g => new
            {
                Key = g.Key,
                Count = g.Count(),
                Sum = g.Sum(valueSelector),
                Average = g.Average(valueSelector),
                Min = g.Min(valueSelector),
                Max = g.Max(valueSelector)
            })
            .OrderByDescending(x => x.Count);
        
        Console.WriteLine("\n=== Grouped Statistics ===");
        foreach (var group in grouped)
        {
            Console.WriteLine($"Group: {group.Key}");
            Console.WriteLine($"  Count: {group.Count}");
            Console.WriteLine($"  Sum: {group.Sum:F2}");
            Console.WriteLine($"  Avg: {group.Average:F2}");
            Console.WriteLine($"  Min: {group.Min:F2}");
            Console.WriteLine($"  Max: {group.Max:F2}");
            Console.WriteLine();
        }
    }
    
    public static void DemonstrateAggregation()
    {
        var numbers = new List<double> 
        { 
            1.5, 2.3, 1.5, 4.7, 3.2, 2.3, 5.1, 1.5, 3.8, 4.2,
            2.9, 3.6, 4.1, 2.7, 3.4, 1.8, 4.9, 2.1, 3.7, 4.3
        };
        
        CalculateStatistics(numbers);
        
        // Example of grouped aggregation with custom objects
        var students = new List<(string Name, string Grade, double Score)>
        {
            ("Alice", "A", 95.5),
            ("Bob", "B", 87.2),
            ("Charlie", "A", 92.8),
            ("Diana", "C", 78.1),
            ("Eve", "B", 89.3),
            ("Frank", "A", 96.7),
            ("Grace", "C", 81.4)
        };
        
        GroupedAggregation(students, s => s.Grade, s => s.Score);
    }
}
```

---

## 🚀 Advanced LINQ Techniques

### 1. Complex Queries and Joins

```csharp
public class AdvancedLinqTechniques
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Department { get; set; }
    }
    
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int Credits { get; set; }
    }
    
    public class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public double Grade { get; set; }
    }
    
    // Join operations
    public static void DemonstrateJoins()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "Alice", Age = 20, Department = "CS" },
            new Student { Id = 2, Name = "Bob", Age = 22, Department = "Math" },
            new Student { Id = 3, Name = "Charlie", Age = 21, Department = "CS" },
            new Student { Id = 4, Name = "Diana", Age = 23, Department = "Physics" }
        };
        
        var courses = new List<Course>
        {
            new Course { Id = 101, Name = "Data Structures", Department = "CS", Credits = 3 },
            new Course { Id = 102, Name = "Calculus", Department = "Math", Credits = 4 },
            new Course { Id = 103, Name = "Algorithms", Department = "CS", Credits = 3 },
            new Course { Id = 104, Name = "Quantum Physics", Department = "Physics", Credits = 4 }
        };
        
        var enrollments = new List<Enrollment>
        {
            new Enrollment { StudentId = 1, CourseId = 101, Grade = 92.5 },
            new Enrollment { StudentId = 1, CourseId = 103, Grade = 88.7 },
            new Enrollment { StudentId = 2, CourseId = 102, Grade = 95.3 },
            new Enrollment { StudentId = 3, CourseId = 101, Grade = 87.9 },
            new Enrollment { StudentId = 3, CourseId = 103, Grade = 91.2 },
            new Enrollment { StudentId = 4, CourseId = 104, Grade = 89.8 }
        };
        
        // Inner join - Students with their enrollments and course details
        var studentEnrollments = from s in students
                               join e in enrollments on s.Id equals e.StudentId
                               join c in courses on e.CourseId equals c.Id
                               select new
                               {
                                   StudentName = s.Name,
                                   StudentDepartment = s.Department,
                                   CourseName = c.Name,
                                   Credits = c.Credits,
                                   Grade = e.Grade
                               };
        
        Console.WriteLine("=== Student Enrollments (Inner Join) ===");
        foreach (var enrollment in studentEnrollments)
        {
            Console.WriteLine($"{enrollment.StudentName} ({enrollment.StudentDepartment}) - " +
                            $"{enrollment.CourseName} ({enrollment.Credits} credits) - Grade: {enrollment.Grade:F1}");
        }
        
        // Left join - All students with their courses (if any)
        var allStudentsWithCourses = from s in students
                                   join e in enrollments on s.Id equals e.StudentId into studentEnrollments
                                   from se in studentEnrollments.DefaultIfEmpty()
                                   join c in courses on se?.CourseId equals c.Id into studentCourses
                                   from sc in studentCourses.DefaultIfEmpty()
                                   select new
                                   {
                                       StudentName = s.Name,
                                       CourseName = sc?.Name ?? "No Enrollment",
                                       Grade = se?.Grade ?? 0
                                   };
        
        Console.WriteLine("\n=== All Students with Courses (Left Join) ===");
        foreach (var item in allStudentsWithCourses)
        {
            Console.WriteLine($"{item.StudentName} - {item.CourseName}" +
                            (item.Grade > 0 ? $" (Grade: {item.Grade:F1})" : ""));
        }
        
        // Group join - Students with all their courses
        var studentsWithAllCourses = from s in students
                                   join e in enrollments on s.Id equals e.StudentId into studentEnrollments
                                   select new
                                   {
                                       StudentName = s.Name,
                                       Courses = from se in studentEnrollments
                                               join c in courses on se.CourseId equals c.Id
                                               select new { c.Name, se.Grade }
                                   };
        
        Console.WriteLine("\n=== Students with All Their Courses (Group Join) ===");
        foreach (var student in studentsWithAllCourses)
        {
            Console.WriteLine($"{student.StudentName}:");
            foreach (var course in student.Courses)
            {
                Console.WriteLine($"  - {course.Name} (Grade: {course.Grade:F1})");
            }
            if (!student.Courses.Any())
            {
                Console.WriteLine("  - No enrollments");
            }
        }
    }
    
    // Complex aggregations with grouping
    public static void DemonstrateComplexAggregations()
    {
        var salesData = new List<(DateTime Date, string Product, string Category, string Region, double Amount)>
        {
            (new DateTime(2024, 1, 1), "Laptop", "Electronics", "North", 1200),
            (new DateTime(2024, 1, 2), "Phone", "Electronics", "South", 800),
            (new DateTime(2024, 1, 3), "Tablet", "Electronics", "North", 500),
            (new DateTime(2024, 2, 1), "Laptop", "Electronics", "East", 1300),
            (new DateTime(2024, 2, 2), "Book", "Education", "North", 25),
            (new DateTime(2024, 2, 3), "Pen", "Office", "South", 5),
            (new DateTime(2024, 3, 1), "Laptop", "Electronics", "West", 1150),
            (new DateTime(2024, 3, 2), "Phone", "Electronics", "North", 850)
        };
        
        // Multi-level grouping and aggregation
        var monthlySalesByRegion = salesData
            .GroupBy(s => new { Month = s.Date.Month, s.Region })
            .Select(g => new
            {
                Month = g.Key.Month,
                Region = g.Key.Region,
                TotalSales = g.Sum(s => s.Amount),
                ProductCount = g.Count(),
                AverageOrderValue = g.Average(s => s.Amount),
                TopProduct = g.GroupBy(s => s.Product)
                             .OrderByDescending(pg => pg.Sum(s => s.Amount))
                             .First()
                             .Key
            })
            .OrderBy(x => x.Month)
            .ThenBy(x => x.Region);
        
        Console.WriteLine("\n=== Monthly Sales by Region ===");
        foreach (var sale in monthlySalesByRegion)
        {
            Console.WriteLine($"Month {sale.Month} - {sale.Region}:");
            Console.WriteLine($"  Total Sales: ${sale.TotalSales:F2}");
            Console.WriteLine($"  Orders: {sale.ProductCount}");
            Console.WriteLine($"  Avg Order Value: ${sale.AverageOrderValue:F2}");
            Console.WriteLine($"  Top Product: {sale.TopProduct}");
        }
        
        // Cross-tabulation using LINQ
        var categoryRegionPivot = salesData
            .GroupBy(s => new { s.Category, s.Region })
            .Select(g => new { g.Key.Category, g.Key.Region, Total = g.Sum(s => s.Amount) })
            .ToList();
        
        var categories = categoryRegionPivot.Select(x => x.Category).Distinct().OrderBy(x => x);
        var regions = categoryRegionPivot.Select(x => x.Region).Distinct().OrderBy(x => x);
        
        Console.WriteLine("\n=== Category vs Region Pivot Table ===");
        Console.Write("Category".PadRight(15));
        foreach (var region in regions)
        {
            Console.Write(region.PadLeft(10));
        }
        Console.WriteLine();
        
        foreach (var category in categories)
        {
            Console.Write(category.PadRight(15));
            foreach (var region in regions)
            {
                var total = categoryRegionPivot
                    .Where(x => x.Category == category && x.Region == region)
                    .Sum(x => x.Total);
                Console.Write(total.ToString("F0").PadLeft(10));
            }
            Console.WriteLine();
        }
    }
}
```

### 2. Performance Optimization with LINQ

```csharp
public class LinqPerformanceOptimization
{
    // Deferred execution demonstration
    public static void DemonstrateDeferredExecution()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        
        // This doesn't execute immediately - it's deferred
        var evenNumbers = numbers.Where(x =>
        {
            Console.WriteLine($"Checking {x}");
            return x % 2 == 0;
        });
        
        Console.WriteLine("Query created, but not executed yet");
        
        // Now it executes
        Console.WriteLine("Executing query:");
        var result = evenNumbers.ToList();
        
        Console.WriteLine("Result: [" + string.Join(", ", result) + "]");
        
        // If we iterate again, it executes again
        Console.WriteLine("Executing query again:");
        foreach (var num in evenNumbers)
        {
            Console.WriteLine($"Even number: {num}");
        }
    }
    
    // Efficient filtering and transformation
    public static T[] OptimizedFilterAndTransform<T, TResult>(
        IEnumerable<T> source,
        Func<T, bool> filter,
        Func<T, TResult> transform) where TResult : T
    {
        // Single pass instead of multiple LINQ operations
        return source
            .Where(filter)
            .Select(transform)
            .ToArray();
    }
    
    // Use Span<T> for high-performance scenarios
    public static int[] ProcessArrayWithSpan(int[] source, Func<int, bool> predicate)
    {
        var span = source.AsSpan();
        var results = new List<int>();
        
        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(span[i]))
            {
                results.Add(span[i]);
            }
        }
        
        return results.ToArray();
    }
    
    // Benchmark LINQ vs traditional loops
    public static void BenchmarkPerformance()
    {
        var data = Enumerable.Range(1, 1_000_000).ToArray();
        var sw = new Stopwatch();
        
        // LINQ approach
        sw.Restart();
        var linqResult = data
            .Where(x => x % 2 == 0)
            .Select(x => x * x)
            .Where(x => x > 1000)
            .ToArray();
        sw.Stop();
        var linqTime = sw.ElapsedMilliseconds;
        
        // Traditional loop approach
        sw.Restart();
        var loopResult = new List<int>();
        foreach (var item in data)
        {
            if (item % 2 == 0)
            {
                var squared = item * item;
                if (squared > 1000)
                {
                    loopResult.Add(squared);
                }
            }
        }
        sw.Stop();
        var loopTime = sw.ElapsedMilliseconds;
        
        // Optimized LINQ (single pass)
        sw.Restart();
        var optimizedResult = data
            .Where(x => x % 2 == 0)
            .Select(x => x * x)
            .Where(x => x > 1000)
            .ToArray();
        sw.Stop();
        var optimizedTime = sw.ElapsedMilliseconds;
        
        Console.WriteLine("=== Performance Comparison ===");
        Console.WriteLine($"LINQ time: {linqTime}ms (Result count: {linqResult.Length})");
        Console.WriteLine($"Loop time: {loopTime}ms (Result count: {loopResult.Count})");
        Console.WriteLine($"Optimized LINQ time: {optimizedTime}ms (Result count: {optimizedResult.Length})");
        Console.WriteLine($"Loop is {(double)linqTime / loopTime:F2}x faster than LINQ");
    }
    
    // Memory-efficient processing with yield return
    public static IEnumerable<TResult> ProcessLargeDataset<T, TResult>(
        IEnumerable<T> source,
        Func<T, TResult> processor)
    {
        foreach (var item in source)
        {
            yield return processor(item);
            
            // This allows processing huge datasets without loading everything into memory
            // Perfect for file processing, database results, etc.
        }
    }
}
```

---

## 🎯 Practice Problems

### Problem Set 1: Basic Array Operations

```csharp
public class BasicArrayProblems
{
    // Problem 1: Find the maximum sum of subarray (Kadane's Algorithm with LINQ)
    public static int MaxSubarraySum(int[] array)
    {
        if (array.Length == 0) return 0;
        
        return array
            .Aggregate(
                new { maxSoFar = array[0], maxEndingHere = array[0] },
                (acc, current) => new
                {
                    maxSoFar = Math.Max(acc.maxSoFar, Math.Max(current, acc.maxEndingHere + current)),
                    maxEndingHere = Math.Max(current, acc.maxEndingHere + current)
                }
            ).maxSoFar;
    }
    
    // Problem 2: Rotate array to the right by k positions using LINQ
    public static int[] RotateArray(int[] array, int k)
    {
        if (array.Length == 0) return array;
        
        k = k % array.Length; // Handle k > array.Length
        
        return array
            .Skip(array.Length - k)
            .Concat(array.Take(array.Length - k))
            .ToArray();
    }
    
    // Problem 3: Find intersection of two arrays
    public static int[] FindIntersection(int[] array1, int[] array2)
    {
        return array1.Intersect(array2).ToArray();
    }
    
    // Problem 4: Two Sum problem using LINQ
    public static (int, int)? TwoSum(int[] array, int target)
    {
        var lookup = array
            .Select((value, index) => new { value, index })
            .ToLookup(x => x.value, x => x.index);
        
        foreach (var item in array.Select((value, index) => new { value, index }))
        {
            var complement = target - item.value;
            var complementIndices = lookup[complement];
            
            var validIndex = complementIndices.FirstOrDefault(i => i != item.index);
            if (validIndex != 0 || (complement == 0 && complementIndices.Count() > 1))
            {
                return (item.index, validIndex);
            }
        }
        
        return null;
    }
    
    // Problem 5: Find missing number in array 1 to n
    public static int FindMissingNumber(int[] array, int n)
    {
        var expectedSum = Enumerable.Range(1, n).Sum();
        var actualSum = array.Sum();
        return expectedSum - actualSum;
    }
    
    public static void TestBasicProblems()
    {
        Console.WriteLine("=== Basic Array Problems ===");
        
        // Test Maximum Subarray Sum
        int[] subarrayTest = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
        Console.WriteLine($"Max subarray sum of [{string.Join(", ", subarrayTest)}]: {MaxSubarraySum(subarrayTest)}");
        
        // Test Array Rotation
        int[] rotateTest = { 1, 2, 3, 4, 5, 6, 7 };
        var rotated = RotateArray(rotateTest, 3);
        Console.WriteLine($"Array [{string.Join(", ", rotateTest)}] rotated by 3: [{string.Join(", ", rotated)}]");
        
        // Test Intersection
        int[] arr1 = { 1, 2, 2, 1 };
        int[] arr2 = { 2, 2 };
        var intersection = FindIntersection(arr1, arr2);
        Console.WriteLine($"Intersection of [{string.Join(", ", arr1)}] and [{string.Join(", ", arr2)}]: [{string.Join(", ", intersection)}]");
        
        // Test Two Sum
        int[] twoSumTest = { 2, 7, 11, 15 };
        var twoSumResult = TwoSum(twoSumTest, 9);
        Console.WriteLine($"Two sum indices for target 9 in [{string.Join(", ", twoSumTest)}]: {twoSumResult}");
        
        // Test Missing Number
        int[] missingTest = { 1, 2, 4, 5, 6 };
        var missing = FindMissingNumber(missingTest, 6);
        Console.WriteLine($"Missing number in range 1-6 from [{string.Join(", ", missingTest)}]: {missing}");
    }
}
```

### Problem Set 2: Advanced Array Challenges

```csharp
public class AdvancedArrayProblems
{
    // Problem 1: Merge intervals using LINQ
    public static int[][] MergeIntervals(int[][] intervals)
    {
        if (intervals.Length <= 1) return intervals;
        
        return intervals
            .OrderBy(x => x[0])
            .Aggregate(
                new List<int[]>(),
                (merged, current) =>
                {
                    if (!merged.Any() || merged.Last()[1] < current[0])
                    {
                        merged.Add(current);
                    }
                    else
                    {
                        merged.Last()[1] = Math.Max(merged.Last()[1], current[1]);
                    }
                    return merged;
                }
            ).ToArray();
    }
    
    // Problem 2: Find all unique triplets that sum to zero
    public static int[][] ThreeSum(int[] nums)
    {
        return nums
            .Select((value, index) => new { value, index })
            .OrderBy(x => x.value)
            .ToArray()
            .SelectMany((first, i) =>
                nums.Skip(i + 1)
                    .SelectMany((second, j) =>
                        nums.Skip(i + j + 2)
                            .Where(third => first.value + second + third == 0)
                            .Select(third => new[] { first.value, second, third }.OrderBy(x => x).ToArray())
                    )
            )
            .Distinct(new ArrayComparer())
            .ToArray();
    }
    
    // Helper class for array comparison
    public class ArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y) => x.SequenceEqual(y);
        public int GetHashCode(int[] obj) => obj.Aggregate(0, (hash, value) => hash ^ value.GetHashCode());
    }
    
    // Problem 3: Product of array except self
    public static int[] ProductExceptSelf(int[] nums)
    {
        var result = new int[nums.Length];
        
        // Left products
        result[0] = 1;
        for (int i = 1; i < nums.Length; i++)
        {
            result[i] = result[i - 1] * nums[i - 1];
        }
        
        // Right products using LINQ aggregate
        nums.Reverse()
            .Skip(1)
            .Aggregate(
                new { rightProduct = 1, index = nums.Length - 1 },
                (acc, current) =>
                {
                    result[acc.index - 1] *= acc.rightProduct;
                    return new { rightProduct = acc.rightProduct * current, index = acc.index - 1 };
                }
            );
        
        return result;
    }
    
    // Problem 4: Container with most water using LINQ approach
    public static int MaxArea(int[] heights)
    {
        return heights
            .SelectMany((leftHeight, left) =>
                heights
                    .Skip(left + 1)
                    .Select((rightHeight, right) => new
                    {
                        area = Math.Min(leftHeight, rightHeight) * (right + left + 1),
                        left,
                        right = right + left + 1
                    })
            )
            .Max(x => x.area);
    }
    
    // Problem 5: Sliding window maximum using LINQ
    public static int[] SlidingWindowMaximum(int[] nums, int k)
    {
        return nums
            .Select((value, index) => new { value, index })
            .Where(x => x.index <= nums.Length - k)
            .Select(x => nums.Skip(x.index).Take(k).Max())
            .ToArray();
    }
    
    public static void TestAdvancedProblems()
    {
        Console.WriteLine("\n=== Advanced Array Problems ===");
        
        // Test Merge Intervals
        int[][] intervals = { new[] { 1, 3 }, new[] { 2, 6 }, new[] { 8, 10 }, new[] { 15, 18 } };
        var merged = MergeIntervals(intervals);
        Console.WriteLine("Merged intervals:");
        foreach (var interval in merged)
        {
            Console.WriteLine($"  [{interval[0]}, {interval[1]}]");
        }
        
        // Test Three Sum
        int[] threeSumTest = { -1, 0, 1, 2, -1, -4 };
        var triplets = ThreeSum(threeSumTest);
        Console.WriteLine($"\nThree sum triplets for [{string.Join(", ", threeSumTest)}]:");
        foreach (var triplet in triplets)
        {
            Console.WriteLine($"  [{string.Join(", ", triplet)}]");
        }
        
        // Test Product Except Self
        int[] productTest = { 1, 2, 3, 4 };
        var products = ProductExceptSelf(productTest);
        Console.WriteLine($"\nProduct except self for [{string.Join(", ", productTest)}]: [{string.Join(", ", products)}]");
        
        // Test Max Area
        int[] heightTest = { 1, 8, 6, 2, 5, 4, 8, 3, 7 };
        var maxArea = MaxArea(heightTest);
        Console.WriteLine($"\nMax water area for heights [{string.Join(", ", heightTest)}]: {maxArea}");
        
        // Test Sliding Window Maximum
        int[] windowTest = { 1, 3, -1, -3, 5, 3, 6, 7 };
        var windowMax = SlidingWindowMaximum(windowTest, 3);
        Console.WriteLine($"\nSliding window max (k=3) for [{string.Join(", ", windowTest)}]: [{string.Join(", ", windowMax)}]");
    }
}
```

---

## 📊 Performance Analysis

### Time Complexity Comparison

| Operation | Traditional Loop | LINQ | Notes |
|-----------|-----------------|------|-------|
| Simple Filter | O(n) | O(n) | LINQ has overhead but same complexity |
| Transform | O(n) | O(n) | Select() is equivalent to map |
| Find Element | O(n) | O(n) | First() stops on match |
| Sort | O(n log n) | O(n log n) | OrderBy uses efficient algorithms |
| Group By | O(n) | O(n) | Efficient grouping implementation |
| Aggregate | O(n) | O(n) | Functional approach with same complexity |

### Memory Usage Considerations

```csharp
public class MemoryConsiderations
{
    // Memory-efficient streaming with yield
    public static IEnumerable<int> ProcessHugeDataset(IEnumerable<int> source)
    {
        foreach (var item in source)
        {
            // Process one item at a time - constant memory usage
            yield return item * 2;
        }
    }
    
    // Memory-intensive approach (loads everything)
    public static List<int> ProcessHugeDatasetToList(IEnumerable<int> source)
    {
        return source.Select(x => x * 2).ToList(); // Loads all results into memory
    }
    
    // Demonstrate memory usage
    public static void DemonstrateMemoryUsage()
    {
        // Streaming approach - processes items one by one
        var streaming = Enumerable.Range(1, 1_000_000)
            .Where(x => x % 2 == 0)
            .Select(x => x * x)
            .Take(10); // Only processes what's needed
        
        Console.WriteLine("Streaming (first 10 results): [" + 
                         string.Join(", ", streaming) + "]");
        
        // This would use much more memory:
        // var allResults = Enumerable.Range(1, 1_000_000)
        //     .Where(x => x % 2 == 0)
        //     .Select(x => x * x)
        //     .ToList(); // Loads 500,000 integers into memory
    }
}
```

---

## 🎓 Summary and Best Practices

### Key Takeaways

1. **LINQ Strengths:**
   - Readable and expressive code
   - Functional programming paradigm
   - Built-in optimizations
   - Composable operations

2. **When to Use LINQ:**
   - Data transformation and filtering
   - Complex queries and aggregations
   - Readable code is priority
   - Working with collections

3. **When to Consider Alternatives:**
   - Performance-critical code
   - Simple operations on large datasets
   - Memory-constrained environments
   - Real-time systems

### Best Practices

```csharp
public class BestPractices
{
    // ✅ Good: Chain operations efficiently
    public static int[] EfficientChaining(int[] source)
    {
        return source
            .Where(x => x > 0)      // Filter first (reduces subsequent work)
            .Select(x => x * x)     // Transform
            .Where(x => x < 1000)   // Filter again if needed
            .ToArray();             // Materialize once at the end
    }
    
    // ❌ Avoid: Multiple materializations
    public static int[] InefficientChaining(int[] source)
    {
        var step1 = source.Where(x => x > 0).ToArray();    // Materialization 1
        var step2 = step1.Select(x => x * x).ToArray();    // Materialization 2
        var step3 = step2.Where(x => x < 1000).ToArray();  // Materialization 3
        return step3;
    }
    
    // ✅ Good: Use appropriate methods
    public static bool HasAnyPositive(int[] numbers)
    {
        return numbers.Any(x => x > 0);  // Stops at first match
    }
    
    // ❌ Avoid: Unnecessary work
    public static bool HasAnyPositiveBad(int[] numbers)
    {
        return numbers.Where(x => x > 0).Count() > 0;  // Processes all elements
    }
    
    // ✅ Good: Null safety
    public static int SafeSum(int[] numbers)
    {
        return numbers?.Sum() ?? 0;
    }
    
    // ✅ Good: Use FirstOrDefault for safety
    public static int SafeFind(int[] numbers, int target)
    {
        return numbers?.FirstOrDefault(x => x == target) ?? -1;
    }
}
```

### Next Steps

🎯 **Continue Learning:**
- [02-Stacks-Queues.md](02-Stacks-Queues.md) - Stack and Queue operations with LINQ
- [08-Searching-Algorithms.md](08-Searching-Algorithms.md) - Advanced searching techniques
- [09-Sorting-Algorithms.md](09-Sorting-Algorithms.md) - Sorting algorithms with LINQ

🚀 **Practice More:**
- Try implementing classic algorithms using only LINQ
- Optimize existing code by replacing loops with LINQ
- Benchmark your solutions against traditional approaches

---

*Mastery comes through practice. Keep coding with LINQ! 🚀*