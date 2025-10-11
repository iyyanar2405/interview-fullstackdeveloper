# Day 03 — State & Events

## Objectives
- useState for local state
- Event handlers

## Example
```tsx
export function Counter() {
  const [count, setCount] = useState(0);
  return (
    <div>
      <button className="p-button" onClick={() => setCount((c) => c + 1)}>+1</button>
      <span className="p-2">{count}</span>
    </div>
  );
}
```

## Exercise
- Build a Toggle component with a PrimeReact InputSwitch

## Checklist
- [ ] State updates
- [ ] Handlers work
