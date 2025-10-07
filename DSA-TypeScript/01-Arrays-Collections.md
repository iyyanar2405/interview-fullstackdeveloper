# 📚 Arrays & Collections in TypeScript

Idiomatic TS patterns for arrays, tuples, Sets, and Maps — plus algorithmic utilities.

## Prerequisites
```ts
// Run with: npx ts-node file.ts
export {};
```

---

## Arrays: Build, Query, Transform
```ts
// Creation
const seq = Array.from({ length: 5 }, (_, i) => i + 1); // [1,2,3,4,5]
const filled = Array(3).fill("hi");                    // ["hi","hi","hi"]

// Query
const evens = seq.filter(x => x % 2 === 0);             // [2,4]
const firstGt3 = seq.find(x => x > 3);                   // 4
const hasAnyOdd = seq.some(x => x % 2 === 1);            // true
const allPositive = seq.every(x => x > 0);               // true

// Transform
const squares = seq.map(x => x * x);                     // [1,4,9,16,25]
const sum = seq.reduce((a, b) => a + b, 0);              // 15

// Compose lazily with generators
function* range(n: number) { for (let i = 0; i < n; i++) yield i; }
const lazyFirst10Squares = [...range(10)].map(x => x * x);
```

### Advanced: Index-aware operations
```ts
const withIndex = seq.map((v, i) => ({ v, i }));
const greaterThanIndex = withIndex.filter(t => t.v > t.i).map(t => t.v);
```

---

## Slicing, Dicing, Grouping
```ts
// Chunking
function chunk<T>(arr: T[], size: number): T[][] {
  return Array.from({ length: Math.ceil(arr.length / size) }, (_, i) => arr.slice(i * size, i * size + size));
}

// GroupBy (ES not built-in yet):
function groupBy<T, K extends PropertyKey>(arr: T[], key: (x: T) => K): Record<K, T[]> {
  return arr.reduce((acc, cur) => {
    const k = key(cur);
    (acc[k] ||= []).push(cur);
    return acc;
  }, {} as Record<K, T[]>);
}

const words = ["apple","banana","apricot","blueberry","avocado"];
const byFirst = groupBy(words, w => w[0]); // { a: [apple,apricot,avocado], b: [banana,blueberry] }

// Frequency
function frequency<T>(arr: T[]): Map<T, number> {
  return arr.reduce((m, x) => m.set(x, (m.get(x) ?? 0) + 1), new Map<T, number>());
}
```

---

## Sets and Maps
```ts
// Unique
const uniques = [...new Set([1,2,2,3,3,3])]; // [1,2,3]

// Set ops
function setIntersection<T>(a: Set<T>, b: Set<T>): Set<T> { return new Set([...a].filter(x => b.has(x))); }
function setUnion<T>(a: Set<T>, b: Set<T>): Set<T> { return new Set([...a, ...b]); }
function setExcept<T>(a: Set<T>, b: Set<T>): Set<T> { return new Set([...a].filter(x => !b.has(x))); }

// Map usage
const ages = new Map<string, number>([["Alice", 30],["Bob", 25]]);
ages.set("Carol", 27);
const under28 = [...ages.entries()].filter(([_, age]) => age < 28).map(([name]) => name);
```

---

## Algorithm Utilities

### Two Sum (Map)
```ts
export function twoSum(nums: number[], target: number): [number, number] | null {
  const map = new Map<number, number>();
  for (let i = 0; i < nums.length; i++) {
    const need = target - nums[i];
    if (map.has(need)) return [map.get(need)!, i];
    map.set(nums[i], i);
  }
  return null;
}
```

### Sliding Window Max (Deque)
```ts
export function slidingWindowMax(nums: number[], k: number): number[] {
  const dq: number[] = []; // store indices
  const ans: number[] = [];
  for (let i = 0; i < nums.length; i++) {
    while (dq.length && dq[0] <= i - k) dq.shift();
    while (dq.length && nums[dq[dq.length - 1]] < nums[i]) dq.pop();
    dq.push(i);
    if (i >= k - 1) ans.push(nums[dq[0]]);
  }
  return ans;
}
```

### Merge Intervals (Sort + Reduce)
```ts
export function mergeIntervals(intervals: [number, number][]): [number, number][] {
  if (intervals.length <= 1) return intervals;
  const sorted = intervals.slice().sort((a, b) => a[0] - b[0]);
  return sorted.reduce<[number, number][]>((acc, cur) => {
    const last = acc[acc.length - 1];
    if (!last || last[1] < cur[0]) acc.push([cur[0], cur[1]]);
    else last[1] = Math.max(last[1], cur[1]);
    return acc;
  }, []);
}
```

---

## Tests / Examples
```ts
console.log(seq, filled, evens, firstGt3, sum);
console.log(uniques);
console.log(twoSum([2,7,11,15], 9));
console.log(slidingWindowMax([1,3,-1,-3,5,3,6,7], 3));
console.log(mergeIntervals([[1,3],[2,6],[8,10],[15,18]]));
```
