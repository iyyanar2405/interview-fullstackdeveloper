# Day 04 — Lists, Keys, Conditional Rendering

## Objectives
- Render lists with keys
- Conditional rendering patterns

## Example
```tsx
const items = [{ id: 1, name: 'A' }, { id: 2, name: 'B' }];
<ul>
  {items.map((it) => <li key={it.id}>{it.name}</li>)}
</ul>
```

## Exercise
- Render a DataTable with selection on a small dataset

## Checklist
- [ ] Keys used correctly
- [ ] Conditional blocks render
