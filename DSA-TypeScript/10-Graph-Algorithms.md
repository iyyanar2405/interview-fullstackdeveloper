# 🧭 Graph Algorithms (TypeScript)

Dijkstra (shortest paths), Topological sort (Kahn), and directed cycle detection.

```ts
export {};
```

---

## Dijkstra (non-negative weights)
```ts
export function dijkstra(g: Map<number, [number, number][]>, src: number): { dist: number[]; prev: (number|null)[] } {
  const n = g.size;
  const dist = Array(n).fill(Number.POSITIVE_INFINITY);
  const prev = Array<(number|null)>(n).fill(null);
  dist[src] = 0;

  // Simple priority queue using array (okay for teaching; replace with binary heap for perf)
  const pq: [node: number, d: number][] = [[src, 0]];
  while (pq.length) {
    pq.sort((a,b) => a[1]-b[1]);
    const [u, du] = pq.shift()!;
    if (du !== dist[u]) continue;
    for (const [v, w] of g.get(u) ?? []) {
      if (dist[u] + w < dist[v]) {
        dist[v] = dist[u] + w;
        prev[v] = u;
        pq.push([v, dist[v]]);
      }
    }
  }
  return { dist, prev };
}

export function reconstructPath(prev: (number|null)[], target: number): number[] {
  const path: number[] = [];
  for (let at: number|null = target; at !== null; at = prev[at]) path.push(at);
  return path.reverse();
}
```

---

## Topological Sort (Kahn)
```ts
export function topoSort(g: Map<number, number[]>): number[] {
  const indeg = new Map<number, number>();
  for (const [u, nbrs] of g) {
    if (!indeg.has(u)) indeg.set(u, 0);
    for (const v of nbrs) indeg.set(v, (indeg.get(v) ?? 0) + 1);
  }
  const q: number[] = [...indeg.entries()].filter(([,d]) => d === 0).map(([u]) => u);
  const order: number[] = [];
  while (q.length) {
    const u = q.shift()!; order.push(u);
    for (const v of g.get(u) ?? []) {
      indeg.set(v, indeg.get(v)! - 1);
      if (indeg.get(v) === 0) q.push(v);
    }
  }
  return order;
}
```

---

## Directed Cycle Detection (DFS)
```ts
export function hasDirectedCycle(g: Map<number, number[]>): boolean {
  const state = new Map<number, number>(); // 0=unseen,1=visiting,2=done
  const dfs = (u: number): boolean => {
    if (state.get(u) === 1) return true;
    if (state.get(u) === 2) return false;
    state.set(u, 1);
    for (const v of g.get(u) ?? []) if (dfs(v)) return true;
    state.set(u, 2);
    return false;
  };
  for (const u of g.keys()) if (dfs(u)) return true;
  return false;
}
```
