using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleOOPDemo;

public class Person
{
    private int _age;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int Age
    {
        get => _age;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException("Age must be >= 0");
            _age = value;
        }
    }
    public string FullName() => $"{FirstName} {LastName}";
}

public interface IClock { DateTime Now { get; } }
public class SystemClock : IClock { public DateTime Now => DateTime.UtcNow; }

public abstract class Animal { public abstract string Speak(); }
public class Dog : Animal { public override string Speak() => "Woof"; }

public class Greeter
{
    private readonly IClock _clock;
    public Greeter(IClock clock) => _clock = clock;
    public string Greet(Person p) => $"Hello {p.FullName()} at {_clock.Now:O}";
}

public static class Program
{
    public static async Task Main()
    {
        var people = new List<Person>
        {
            new() { FirstName = "Ada", LastName = "Lovelace", Age = 36 },
            new() { FirstName = "Alan", LastName = "Turing", Age = 41 },
            new() { FirstName = "Grace", LastName = "Hopper", Age = 85 }
        };

        var adults = people.Where(p => p.Age >= 18).Select(p => p.FullName());
        Console.WriteLine("Adults: " + string.Join(", ", adults));

        var g = new Greeter(new SystemClock());
        Console.WriteLine(g.Greet(people.First()));

        var dog = new Dog();
        Console.WriteLine("Dog says: " + dog.Speak());

        // async demo
        int result = await FetchAnswerAsync();
        Console.WriteLine($"Async result: {result}");
    }

    public static async Task<int> FetchAnswerAsync()
    {
        await Task.Delay(100);
        return 42;
    }
}
