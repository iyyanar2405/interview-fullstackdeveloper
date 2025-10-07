# 🔤 String Algorithms (TypeScript)

Frequencies, anagrams, palindromes, and sliding windows.

```ts
export {};
```

---

## Frequency and Anagrams
```ts
export function charFrequency(s: string): Map<string, number> {
  return [...s].reduce((m, c) => m.set(c, (m.get(c) ?? 0) + 1), new Map<string, number>());
}

export function areAnagrams(a: string, b: string): boolean {
  if (a.length !== b.length) return false;
  const fa = charFrequency(a), fb = charFrequency(b);
  if (fa.size !== fb.size) return false;
  for (const [k, v] of fa) if (fb.get(k) !== v) return false;
  return true;
}
```

## Palindrome check (alnum only)
```ts
export function isPalindrome(s: string): boolean {
  const t = [...s].filter(ch => /[a-z0-9]/i.test(ch)).map(ch => ch.toLowerCase());
  for (let i = 0, j = t.length - 1; i < j; i++, j--) if (t[i] !== t[j]) return false;
  return true;
}
```

## Longest Substring Without Repeating Characters
```ts
export function lengthOfLongestUnique(s: string): number {
  const seen = new Map<string, number>();
  let start = 0, best = 0;
  for (let i = 0; i < s.length; i++) {
    const ch = s[i];
    if (seen.has(ch) && seen.get(ch)! >= start) start = seen.get(ch)! + 1;
    seen.set(ch, i); best = Math.max(best, i - start + 1);
  }
  return best;
}
```

## Group Anagrams
```ts
export function groupAnagrams(words: string[]): string[][] {
  const key = (w: string) => [...w].sort().join("");
  const groups = new Map<string, string[]>();
  for (const w of words) (groups.get(key(w)) ?? groups.set(key(w), []).get(key(w))!).push(w);
  return [...groups.values()];
}
```
