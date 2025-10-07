# 🧰 Advanced Data Structures (TypeScript)

Trie and Disjoint Set Union (Union-Find).

```ts
export {};
```

---

## Trie
```ts
class TrieNode { end = false; next = new Map<string, TrieNode>(); }

export class Trie {
  private root = new TrieNode();

  insert(word: string) {
    let n = this.root;
    for (const ch of word) { if (!n.next.has(ch)) n.next.set(ch, new TrieNode()); n = n.next.get(ch)!; }
    n.end = true;
  }

  search(word: string): boolean {
    let n = this.root; for (const ch of word) { n = n.next.get(ch)!; if (!n) return false; }
    return !!n?.end;
  }

  startsWith(prefix: string): string[] {
    let n = this.root; for (const ch of prefix) { const t = n.next.get(ch); if (!t) return []; n = t; }
    const out: string[] = [];
    const dfs = (node: TrieNode, path: string) => {
      if (node.end) out.push(path);
      [...node.next.entries()].sort(([a],[b]) => a.localeCompare(b)).forEach(([ch, child]) => dfs(child, path + ch));
    };
    dfs(n, prefix); return out;
  }
}
```

---

## Disjoint Set Union (Union-Find)
```ts
export class DSU {
  private parent: number[]; private size: number[];
  constructor(n: number) { this.parent = Array.from({length:n}, (_,i)=>i); this.size = Array(n).fill(1); }
  find(x: number): number { return this.parent[x] === x ? x : (this.parent[x] = this.find(this.parent[x])); }
  union(a: number, b: number) { a = this.find(a); b = this.find(b); if (a===b) return; if (this.size[a]<this.size[b]) [a,b]=[b,a]; this.parent[b]=a; this.size[a]+=this.size[b]; }
  components(): Map<number, number> { const roots = this.parent.map((_,i)=>this.find(i)); return roots.reduce((m,r)=>m.set(r,(m.get(r)??0)+1), new Map<number,number>()); }
}
```
