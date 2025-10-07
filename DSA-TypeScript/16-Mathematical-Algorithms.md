# ➗ Mathematical Algorithms (TypeScript)

Primes sieve, GCD/LCM, simple factorization.

```ts
export {};
```

---

## Sieve of Eratosthenes
```ts
export function sieve(n: number): number[] {
  const isPrime = Array(n + 1).fill(true);
  isPrime[0] = isPrime[1] = false;
  for (let p = 2; p * p <= n; p++) if (isPrime[p]) for (let x = p*p; x <= n; x += p) isPrime[x] = false;
  return Array.from({length: n-1}, (_, i) => i + 2).filter(i => isPrime[i]);
}
```

## GCD/LCM
```ts
export function gcd(a: number, b: number): number { return b === 0 ? Math.abs(a) : gcd(b, a % b); }
export function lcm(a: number, b: number): number { return Math.abs((a / gcd(a, b)) * b); }
export function gcdAll(nums: number[]): number { return nums.reduce((acc, x) => gcd(acc, x)); }
export function lcmAll(nums: number[]): number { return nums.reduce((acc, x) => lcm(acc, x)); }
```

## Factorization counts
```ts
export function factorCount(n: number): Map<number, number> {
  const map = new Map<number, number>();
  for (let p = 2; p * p <= n; p++) {
    while (n % p === 0) { map.set(p, (map.get(p) ?? 0) + 1); n = Math.floor(n / p); }
  }
  if (n > 1) map.set(n, (map.get(n) ?? 0) + 1);
  return map;
}
```
