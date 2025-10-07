# 🧮 Dynamic Programming (TypeScript)

Representative DP problems with concise TypeScript implementations.

```ts
export {};
```

---

## 0/1 Knapsack (1D DP)
```ts
export function knapsack01(items: { w: number; v: number }[], cap: number): number {
  const dp = Array(cap + 1).fill(0);
  for (const it of items) {
    for (let w = cap; w >= it.w; w--) {
      dp[w] = Math.max(dp[w], dp[w - it.w] + it.v);
    }
  }
  return Math.max(...dp);
}
```

## Coin Change (min coins)
```ts
export function coinChangeMin(coins: number[], amount: number): number {
  const INF = amount + 1;
  const dp = Array(amount + 1).fill(INF);
  dp[0] = 0;
  for (const c of coins) {
    for (let a = c; a <= amount; a++) dp[a] = Math.min(dp[a], dp[a - c] + 1);
  }
  return dp[amount] > amount ? -1 : dp[amount];
}
```

## Longest Increasing Subsequence (patience)
```ts
export function lisLength(nums: number[]): number {
  const tails: number[] = [];
  for (const x of nums) {
    let i = lowerBound(tails, x);
    if (i === tails.length) tails.push(x); else tails[i] = x;
  }
  return tails.length;
}

function lowerBound(arr: number[], target: number): number {
  let lo = 0, hi = arr.length;
  while (lo < hi) { const mid = (lo + hi) >> 1; if (arr[mid] < target) lo = mid + 1; else hi = mid; }
  return lo;
}
```
