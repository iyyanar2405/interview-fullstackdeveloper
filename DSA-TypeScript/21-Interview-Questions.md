# 🎤 Interview Questions (TypeScript)

Quick prompts and reusable patterns.

```ts
export {};
```

---

## Remove Duplicates While Preserving Order
```ts
export function preserveOrderDistinct(a: number[]): number[] { const seen = new Set<number>(); return a.filter(x => (seen.has(x) ? false : (seen.add(x), true))); }
```

## Top N Frequent Users per Country
```ts
export type User = { country: string; name: string };
export function topUsers(users: User[], n: number): { country: string; name: string }[] {
  const byCountry = users.reduce<Record<string, string[]>>((acc, u) => { (acc[u.country] ||= []).push(u.name); return acc; }, {});
  const out: { country: string; name: string }[] = [];
  for (const [country, names] of Object.entries(byCountry)) {
    const counts = names.reduce((m, x) => m.set(x, (m.get(x) ?? 0) + 1), new Map<string, number>());
    const top = [...counts.entries()].sort((a,b) => b[1]-a[1]).slice(0, n).map(([name]) => ({ country, name }));
    out.push(...top);
  }
  return out;
}
```

## Anagram Groups Representative
```ts
export function anagramRepresentative(words: string[]): string[] {
  const key = (w: string) => [...w].sort().join("");
  const groups = new Map<string, string[]>();
  for (const w of words) (groups.get(key(w)) ?? groups.set(key(w), []).get(key(w))!).push(w);
  return [...groups.values()].map(g => g.sort()[0]);
}
```

## Join orders with latest status
```ts
export type Order = { id: number };
export type Status = { orderId: number; when: Date; state: string };
export function latestStatus(orders: Order[], status: Status[]): { orderId: number; state: string }[] {
  const byOrder = status.reduce<Record<number, Status[]>>((acc, s) => { (acc[s.orderId] ||= []).push(s); return acc; }, {});
  return orders.map(o => ({ orderId: o.id, state: (byOrder[o.id]?.slice().sort((a,b) => b.when.getTime()-a.when.getTime())[0]?.state) ?? "Unknown" }));
}
```
