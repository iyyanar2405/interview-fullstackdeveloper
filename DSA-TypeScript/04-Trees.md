# 🌲 Trees & BSTs in TypeScript

Generic tree nodes, traversals, and common queries.

```ts
export {};
```

---

## Node and BST Builder
```ts
export class TreeNode<T> {
  constructor(public value: T, public left: TreeNode<T> | null = null, public right: TreeNode<T> | null = null) {}
}

export function bstInsert(root: TreeNode<number> | null, v: number): TreeNode<number> {
  if (!root) return new TreeNode(v);
  if (v < root.value) root.left = bstInsert(root.left, v); else root.right = bstInsert(root.right, v);
  return root;
}

export function bstBuild(...values: number[]): TreeNode<number> | null {
  let root: TreeNode<number> | null = null;
  for (const v of values) root = bstInsert(root, v);
  return root;
}
```

---

## Traversals (as generators)
```ts
export function* inOrder<T>(n: TreeNode<T> | null): Generator<T> {
  if (!n) return; yield* inOrder(n.left); yield n.value; yield* inOrder(n.right);
}
export function* preOrder<T>(n: TreeNode<T> | null): Generator<T> {
  if (!n) return; yield n.value; yield* preOrder(n.left); yield* preOrder(n.right);
}
export function* postOrder<T>(n: TreeNode<T> | null): Generator<T> {
  if (!n) return; yield* postOrder(n.left); yield* postOrder(n.right); yield n.value;
}

export function levelOrderLayers<T>(root: TreeNode<T> | null): T[][] {
  if (!root) return [];
  const q: (TreeNode<T>)[] = [root];
  const out: T[][] = [];
  while (q.length) {
    const size = q.length; const layer: T[] = [];
    for (let i = 0; i < size; i++) {
      const n = q.shift()!; layer.push(n.value);
      if (n.left) q.push(n.left); if (n.right) q.push(n.right);
    }
    out.push(layer);
  }
  return out;
}
```

---

## Queries and Helpers
```ts
export function treeHeight<T>(root: TreeNode<T> | null): number {
  return root ? 1 + Math.max(treeHeight(root.left), treeHeight(root.right)) : 0;
}

export function contains(root: TreeNode<number> | null, x: number): boolean {
  for (const v of inOrder(root)) if (v === x) return true; return false;
}

export function kthSmallest(root: TreeNode<number> | null, k: number): number | undefined {
  let i = 0; for (const v of inOrder(root)) if (i++ === k) return v; return undefined;
}

export function lcaBst(root: TreeNode<number> | null, p: number, q: number): TreeNode<number> | null {
  while (root) {
    if (p < root.value && q < root.value) root = root.left;
    else if (p > root.value && q > root.value) root = root.right;
    else return root;
  }
  return null;
}
```

---

## Example
```ts
const root = bstBuild(5,3,7,2,4,6,8);
console.log([...inOrder(root)]);            // [2,3,4,5,6,7,8]
console.log(treeHeight(root));              // 3
console.log(contains(root, 6));             // true
console.log(kthSmallest(root, 3));          // 5 (0-indexed)
console.log(levelOrderLayers(root));        // [[5],[3,7],[2,4,6,8]]
```
