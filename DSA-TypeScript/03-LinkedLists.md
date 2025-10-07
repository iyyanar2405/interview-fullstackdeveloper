# 🔗 Linked Lists in TypeScript

Singly and doubly linked lists with idiomatic classes and common algorithms.

```ts
export {};
```

---

## Singly Linked List
```ts
class SNode<T> { constructor(public value: T, public next: SNode<T> | null = null) {} }

export class SinglyLinkedList<T> {
  private head: SNode<T> | null = null;
  get isEmpty() { return this.head === null; }

  pushFront(value: T) { this.head = new SNode(value, this.head); }

  pushBack(value: T) {
    const n = new SNode(value);
    if (!this.head) { this.head = n; return; }
    let cur = this.head;
    while (cur.next) cur = cur.next;
    cur.next = n;
  }

  find(pred: (v: T) => boolean): T | undefined {
    for (let cur = this.head; cur; cur = cur.next) if (pred(cur.value)) return cur.value;
    return undefined;
  }

  removeWhere(pred: (v: T) => boolean) {
    const dummy = new SNode<T>(null as unknown as T, this.head);
    let prev = dummy; let cur = this.head;
    while (cur) {
      if (pred(cur.value)) prev.next = cur.next; else prev = cur;
      cur = cur.next;
    }
    this.head = dummy.next;
  }

  toArray(): T[] { const out: T[] = []; for (let c = this.head; c; c = c.next) out.push(c.value); return out; }
}
```

### Reverse (Iterative)
```ts
export function reverseList<T>(head: SNode<T> | null): SNode<T> | null {
  let prev: SNode<T> | null = null, cur = head;
  while (cur) { const nxt = cur.next; cur.next = prev; prev = cur; cur = nxt; }
  return prev;
}
```

### Merge Two Sorted Lists
```ts
export function mergeSorted(a: SNode<number> | null, b: SNode<number> | null): SNode<number> | null {
  const dummy = new SNode<number>(0); let tail = dummy;
  while (a && b) {
    if (a.value <= b.value) { tail.next = a; a = a.next; }
    else { tail.next = b; b = b.next; }
    tail = tail.next;
  }
  tail.next = a ?? b; return dummy.next;
}
```

### Detect Cycle (Floyd)
```ts
export function hasCycle<T>(head: SNode<T> | null): boolean {
  let slow = head, fast = head;
  while (fast && fast.next) { slow = slow!.next; fast = fast.next.next; if (slow === fast) return true; }
  return false;
}
```

---

## Doubly Linked List (minimal)
```ts
class DNode<T> { constructor(public value: T, public prev: DNode<T> | null = null, public next: DNode<T> | null = null) {} }

export class DoublyLinkedList<T> {
  private head: DNode<T> | null = null;
  private tail: DNode<T> | null = null;

  pushBack(value: T) {
    const n = new DNode(value, this.tail, null);
    if (!this.head) this.head = this.tail = n; else { this.tail!.next = n; this.tail = n; }
  }

  remove(value: T) {
    for (let cur = this.head; cur; cur = cur.next) {
      if (cur.value === value) {
        if (cur.prev) cur.prev.next = cur.next; else this.head = cur.next;
        if (cur.next) cur.next.prev = cur.prev; else this.tail = cur.prev;
        return true;
      }
    }
    return false;
  }

  toArray(): T[] { const out: T[] = []; for (let c = this.head; c; c = c.next) out.push(c.value); return out; }
}
```
