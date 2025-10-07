# 🧩 Practice Problems (TypeScript)

A set of interview problems with concise solutions.

```ts
export {};
```

---

## Top K Frequent Elements
```ts
export function topKFrequent(nums: number[], k: number): number[] {
  const freq = nums.reduce((m, x) => m.set(x, (m.get(x) ?? 0) + 1), new Map<number, number>());
  return [...freq.entries()].sort((a,b) => b[1]-a[1]).slice(0, k).map(([v]) => v);
}
```

## Range Buckets (Group by ranges)
```ts
export function rangeBuckets(nums: number[], size: number): { bucket: string; count: number }[] {
  const label = (x: number) => `[${Math.floor(x/size)*size}-${Math.floor(x/size)*size + size - 1}]`;
  const groups = nums.reduce<Record<string, number>>((acc, x) => { const k = label(x); acc[k] = (acc[k] ?? 0) + 1; return acc; }, {});
  return Object.entries(groups).map(([bucket, count]) => ({ bucket, count })).sort((a,b) => a.bucket.localeCompare(b.bucket));
}
```

## K Most Common Words
```ts
export function kCommonWords(text: string, k: number): string[] {
  const words = text.split(/[\s\.,;!?]+/).filter(Boolean).map(w => w.toLowerCase());
  const freq = words.reduce((m, w) => m.set(w, (m.get(w) ?? 0) + 1), new Map<string, number>());
  return [...freq.entries()].sort((a,b) => b[1]-a[1] || a[0].localeCompare(b[0])).slice(0,k).map(([w]) => w);
}
```

## Merge Overlapping Intervals
```ts
export function mergeIntervals(intervals: [number, number][]): [number, number][] {
  if (intervals.length <= 1) return intervals;
  const sorted = intervals.slice().sort((a,b) => a[0]-b[0]);
  return sorted.reduce<[number, number][]>((acc, cur) => {
    const last = acc[acc.length - 1];
    if (!last || last[1] < cur[0]) acc.push([cur[0], cur[1]]);
    else last[1] = Math.max(last[1], cur[1]);
    return acc;
  }, []);
}
```

## Moving Average (Time Series)
```ts
export function movingAverage(series: number[], k: number): number[] {
  const out: number[] = [];
  for (let i = 0; i < series.length; i++) out.push(series.slice(Math.max(0, i - k + 1), i + 1).reduce((a,b)=>a+b,0) / Math.min(k, i+1));
  return out;
}
```

## Distinct by Key
```ts
export function distinctBy<T, K>(items: T[], key: (x: T) => K): T[] {
  const seen = new Set<K>();
  return items.filter(x => { const k = key(x); if (seen.has(k)) return false; seen.add(k); return true; });
}
```
