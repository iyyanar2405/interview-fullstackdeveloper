# 🧮 Bit Manipulation (TypeScript)

Hamming weight, single number (XOR), and count bits.

```ts
export {};
```

---

## Hamming Weight
```ts
export function hammingWeight(x: number): number {
  let cnt = 0; let v = x >>> 0; // treat as unsigned
  while (v) { v &= (v - 1); cnt++; }
  return cnt;
}
```

## Single Number (XOR aggregate)
```ts
export function singleNumber(nums: number[]): number {
  return nums.reduce((acc, x) => acc ^ x, 0);
}
```

## Count Bits [0..n]
```ts
export function countBits(n: number): number[] {
  const res = Array(n + 1).fill(0);
  for (let i = 1; i <= n; i++) res[i] = res[i >> 1] + (i & 1);
  return res;
}
```
