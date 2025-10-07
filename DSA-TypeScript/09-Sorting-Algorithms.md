# 🔢 Sorting Algorithms (TypeScript)

Built-in sorts and classical implementations.

```ts
export {};
```

---

## Built-in Sorts
```ts
export function asc(arr: number[]): number[] { return arr.slice().sort((a,b) => a-b); }
export function desc(arr: number[]): number[] { return arr.slice().sort((a,b) => b-a); }

export type Person = { name: string; age: number; score: number };
export function byMultiple(people: Person[]): Person[] {
  return people.slice().sort((a,b) => a.age - b.age || b.score - a.score);
}
```

## Insertion Sort
```ts
export function insertionSort(a: number[]): number[] {
  const arr = a.slice();
  for (let i = 1; i < arr.length; i++) {
    const key = arr[i]; let j = i - 1;
    while (j >= 0 && arr[j] > key) { arr[j+1] = arr[j]; j--; }
    arr[j+1] = key;
  }
  return arr;
}
```

## Merge Sort
```ts
export function mergeSort(a: number[]): number[] {
  if (a.length <= 1) return a.slice();
  const mid = a.length >> 1;
  const left = mergeSort(a.slice(0, mid));
  const right = mergeSort(a.slice(mid));
  return merge(left, right);
}

function merge(a: number[], b: number[]): number[] {
  const res: number[] = []; let i = 0, j = 0;
  while (i < a.length && j < b.length) res.push(a[i] <= b[j] ? a[i++] : b[j++]);
  while (i < a.length) res.push(a[i++]);
  while (j < b.length) res.push(b[j++]);
  return res;
}
```

## Quick Sort (functional)
```ts
export function quickSort(a: number[]): number[] {
  if (a.length <= 1) return a.slice();
  const pivot = a[a.length >> 1];
  const less = a.filter(x => x < pivot);
  const equal = a.filter(x => x === pivot);
  const greater = a.filter(x => x > pivot);
  return [...quickSort(less), ...equal, ...quickSort(greater)];
}
```

## Counting Sort (non-negative)
```ts
export function countingSort(a: number[]): number[] {
  if (a.length === 0) return [];
  const max = Math.max(...a);
  const counts = Array(max + 1).fill(0);
  for (const x of a) counts[x]++;
  const res: number[] = [];
  counts.forEach((cnt, val) => { for (let i = 0; i < cnt; i++) res.push(val); });
  return res;
}
```
