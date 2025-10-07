# ➗ Mathematical Algorithms with LINQ

Use LINQ for sieve generation, factorization summaries, and numeric aggregations.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Sieve of Eratosthenes
```csharp
public static class Primes
{
    public static int[] Sieve(int n)
    {
        var isPrime = Enumerable.Repeat(true, n + 1).ToArray();
        isPrime[0] = isPrime[1] = false;
        for (int p = 2; p * p <= n; p++) if (isPrime[p]) for (int x = p * p; x <= n; x += p) isPrime[x] = false;
        return Enumerable.Range(2, n - 1).Where(i => isPrime[i]).ToArray();
    }
}
```

---

## GCD/LCM for sequences
```csharp
public static class GcdLcm
{
    public static int Gcd(int a, int b) => b == 0 ? Math.Abs(a) : Gcd(b, a % b);
    public static int Lcm(int a, int b) => Math.Abs(a / Gcd(a, b) * b);
    public static int GcdAll(IEnumerable<int> nums) => nums.Aggregate(Gcd);
    public static int LcmAll(IEnumerable<int> nums) => nums.Aggregate(Lcm);
}
```

---

## Factorization histogram
```csharp
public static class Factors
{
    public static Dictionary<int, int> FactorCount(int n)
    {
        var map = new Dictionary<int, int>();
        for (int p = 2; p * p <= n; p++)
        {
            while (n % p == 0) { map[p] = map.GetValueOrDefault(p) + 1; n /= p; }
        }
        if (n > 1) map[n] = map.GetValueOrDefault(n) + 1;
        return map;
    }
}
```
