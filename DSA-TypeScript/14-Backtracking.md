# 🧩 Backtracking (TypeScript)

Use sets and arrays to track state; generate candidates cleanly.

```ts
export {};
```

---

## Permutations
```ts
export function permutations(nums: number[]): number[][] {
  const n = nums.length; const used = Array(n).fill(false); const cur: number[] = []; const out: number[][] = [];
  const dfs = () => {
    if (cur.length === n) { out.push([...cur]); return; }
    for (let i = 0; i < n; i++) if (!used[i]) { used[i] = true; cur.push(nums[i]); dfs(); cur.pop(); used[i] = false; }
  };
  dfs(); return out;
}
```

## N-Queens
```ts
export function nQueens(n: number): number[][] {
  const cols = new Set<number>(); const d1 = new Set<number>(); const d2 = new Set<number>();
  const board = Array(n).fill(-1); const out: number[][] = [];
  const place = (r: number) => {
    if (r === n) { out.push([...board]); return; }
    for (let c = 0; c < n; c++) if (!cols.has(c) && !d1.has(r-c) && !d2.has(r+c)) {
      board[r] = c; cols.add(c); d1.add(r-c); d2.add(r+c);
      place(r+1);
      cols.delete(c); d1.delete(r-c); d2.delete(r+c);
    }
  };
  place(0); return out;
}
```
