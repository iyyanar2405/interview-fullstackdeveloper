# ⚡ TypeScript Performance Tips

Write fast, memory-conscious code with simple patterns and micro-benchmarks.

```ts
export {};
```

---

## Avoid unnecessary allocations
```ts
export function good(a: number[]): number[] { return a.filter(x => x > 0).map(x => x*x).filter(x => x < 1000); }
export function bad(a: number[]): number[] {
  const s1 = a.filter(x => x > 0);
  const s2 = s1.map(x => x*x);
  return s2.filter(x => x < 1000);
}
```

## Prefer early exits and short-circuiting
```ts
export function hasAnyPositive(a: number[]): boolean { return a.some(x => x > 0); }
export function hasAnyPositiveBad(a: number[]): boolean { return a.filter(x => x > 0).length > 0; }
```

## Micro-benchmark template
```ts
export function compare(): { mapMs: number; loopMs: number } {
  const data = Array.from({length: 100_000}, (_,i) => i+1);
  const t1 = performance.now();
  const r1 = data.filter(x => (x & 1) === 0).map(x => x*x).filter(x => x > 1000);
  const mapMs = performance.now() - t1;

  const t2 = performance.now();
  const r2: number[] = []; for (const x of data) if ((x & 1) === 0) { const y = x*x; if (y > 1000) r2.push(y); }
  const loopMs = performance.now() - t2;

  return { mapMs, loopMs };
}
```

## Tips
- Push filters earlier in the chain.
- Avoid re-enumeration unless you cache on purpose.
- Prefer Sets/Maps for membership/frequency.
- Measure with `performance.now()`; beware of console noise.
