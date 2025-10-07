# 🕸️ Graphs in TypeScript

Adjacency list representation with BFS/DFS and components.

```ts
export {};
```

---

## Build Graph (Undirected)
```ts
export function buildUndirected(n: number, edges: [number, number][]): Map<number, number[]> {
  const g = new Map<number, number[]>();
  for (let i = 0; i < n; i++) g.set(i, []);
  for (const [u, v] of edges) { g.get(u)!.push(v); g.get(v)!.push(u); }
  return g;
}
```

## BFS / DFS
```ts
export function bfs(g: Map<number, number[]>, src: number): number[] {
  const q: number[] = [src];
  const vis = new Set<number>([src]);
  const order: number[] = [];
  while (q.length) {
    const u = q.shift()!; order.push(u);
    for (const v of g.get(u) ?? []) if (!vis.has(v)) { vis.add(v); q.push(v); }
  }
  return order;
}

export function dfs(g: Map<number, number[]>, src: number): number[] {
  const st: number[] = [src];
  const vis = new Set<number>();
  const order: number[] = [];
  while (st.length) {
    const u = st.pop()!;
    if (vis.has(u)) continue; vis.add(u); order.push(u);
    const nbrs = g.get(u) ?? [];
    for (let i = nbrs.length - 1; i >= 0; i--) st.push(nbrs[i]);
  }
  return order;
}
```

## Connected Components
```ts
export function connectedComponents(g: Map<number, number[]>): number[][] {
  const vis = new Set<number>();
  const comps: number[][] = [];
  for (const start of g.keys()) {
    if (vis.has(start)) continue;
    const q = [start]; vis.add(start); const comp: number[] = [];
    while (q.length) {
      const u = q.shift()!; comp.push(u);
      for (const v of g.get(u) ?? []) if (!vis.has(v)) { vis.add(v); q.push(v); }
    }
    comps.push(comp.sort((a,b) => a-b));
  }
  return comps.sort((a,b) => a[0] - b[0]);
}
```
