# Day 02 — JSX, Components, Props

## Objectives
- Create functional components with TS props
- Compose components and pass props

## Example
```tsx
type CardProps = { title: string; children?: React.ReactNode };
function SectionCard({ title, children }: CardProps) {
  return (
    <div className="p-card">
      <div className="p-card-header">{title}</div>
      <div className="p-card-body">{children}</div>
    </div>
  );
}
```

## Exercise
- Build a ProfileCard consuming props and children.

## Checklist
- [ ] Components render
- [ ] Props typed
