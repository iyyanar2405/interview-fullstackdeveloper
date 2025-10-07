# 🔎 Searching Algorithms (TypeScript)

Linear, binary, 2D matrix, and object search patterns.

```ts
export {};
```

---

## Linear Search
```ts
export function indexOf<T>(arr: T[], target: T): number {
  return arr.findIndex(x => Object.is(x, target));
}

export function find<T>(arr: T[], pred: (x: T) => boolean): T | undefined {
  return arr.find(pred);
}
```

## Binary Search (sorted)
```ts
export function binaryIndexOf(arr: number[], target: number): number {
  let lo = 0, hi = arr.length - 1;
  while (lo <= hi) {
    const mid = lo + ((hi - lo) >> 1);
    if (arr[mid] === target) return mid;
    if (arr[mid] < target) lo = mid + 1; else hi = mid - 1;
  }
  return -1;
}

export function lowerBound(arr: number[], target: number): number {
  let lo = 0, hi = arr.length;
  while (lo < hi) {
    const mid = (lo + hi) >> 1;
    if (arr[mid] < target) lo = mid + 1; else hi = mid;
  }
  return lo;
}
```

## 2D Matrix Search
```ts
export function matrixFind(m: number[][], target: number): [number, number] | null {
  for (let r = 0; r < m.length; r++) {
    const c = m[r].indexOf(target);
    if (c !== -1) return [r, c];
  }
  return null;
}
```

## Object Search
```ts
export type Person = { id: number; name: string; age: number; city: string };

export function findByName(people: Person[], name: string): Person | undefined {
  const n = name.toLowerCase();
  return people.find(p => p.name.toLowerCase() === n);
}

export function adultsInCity(people: Person[], city: string, minAge = 18): Person[] {
  return people.filter(p => p.city === city && p.age >= minAge);
}

export function avgAgeByCity(people: Person[]): { city: string; avg: number }[] {
  const groups = people.reduce<Record<string, number[]>>((acc, p) => {
    (acc[p.city] ||= []).push(p.age); return acc;
  }, {});
  return Object.entries(groups)
    .map(([city, ages]) => ({ city, avg: ages.reduce((a, b) => a + b, 0) / ages.length }))
    .sort((a, b) => b.avg - a.avg);
}
```
