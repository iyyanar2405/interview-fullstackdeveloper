# 🧱 Stacks & Queues in TypeScript

Classic LIFO/FIFO structures with idiomatic classes and algorithm patterns.

## Prerequisites
```ts
export {};
```

---

## Stack<T>
```ts
export class Stack<T> {
  private data: T[] = [];
  push(x: T) { this.data.push(x); }
  pop(): T | undefined { return this.data.pop(); }
  peek(): T | undefined { return this.data[this.data.length - 1]; }
  get size() { return this.data.length; }
  get isEmpty() { return this.data.length === 0; }
  toArray() { return [...this.data]; }
}
```

## Queue<T>
```ts
export class Queue<T> {
  private data: T[] = [];
  private head = 0;
  enqueue(x: T) { this.data.push(x); }
  dequeue(): T | undefined { return this.head < this.data.length ? this.data[this.head++] : undefined; }
  get size() { return this.data.length - this.head; }
  get isEmpty() { return this.size === 0; }
  toArray() { return this.data.slice(this.head); }
}
```

---

## Valid Parentheses (Stack)
```ts
export function isValidParentheses(s: string): boolean {
  const map: Record<string, string> = { ')': '(', ']': '[', '}': '{' };
  const st = new Stack<string>();
  for (const c of s) {
    if (c in map) {
      const top = st.pop();
      if (top !== map[c]) return false;
    } else {
      st.push(c);
    }
  }
  return st.isEmpty;
}
```

## Next Greater Element (Monotonic Stack)
```ts
export function nextGreater(nums: number[]): number[] {
  const res = new Array(nums.length).fill(-1);
  const st: number[] = []; // store indices
  for (let i = 0; i < nums.length; i++) {
    while (st.length && nums[i] > nums[st[st.length - 1]]) {
      const idx = st.pop()!;
      res[idx] = nums[i];
    }
    st.push(i);
  }
  return res;
}
```

## Sliding Window Maximum (Deque)
```ts
export function slidingWindowMax(nums: number[], k: number): number[] {
  const dq: number[] = []; // store indices
  const ans: number[] = [];
  for (let i = 0; i < nums.length; i++) {
    while (dq.length && dq[0] <= i - k) dq.shift();
    while (dq.length && nums[dq[dq.length - 1]] < nums[i]) dq.pop();
    dq.push(i);
    if (i >= k - 1) ans.push(nums[dq[0]]);
  }
  return ans;
}
```

## Round Robin Scheduling (Queue)
```ts
export type TaskItem = { name: string; burstMs: number };

export function* roundRobin(tasks: TaskItem[], quantum: number): Generator<{ name: string; slice: number }>{
  const q = new Queue<TaskItem>();
  tasks.forEach(t => q.enqueue({ ...t }));
  while (!q.isEmpty) {
    const t = q.dequeue()!;
    const slice = Math.min(quantum, t.burstMs);
    yield { name: t.name, slice };
    const remaining = t.burstMs - slice;
    if (remaining > 0) q.enqueue({ name: t.name, burstMs: remaining });
  }
}
```

## Examples
```ts
console.log(isValidParentheses("()[]{}"));
console.log(nextGreater([2,1,2,4,3]));
console.log([...roundRobin([{name:'A',burstMs:5},{name:'B',burstMs:3}], 2)]);
```
