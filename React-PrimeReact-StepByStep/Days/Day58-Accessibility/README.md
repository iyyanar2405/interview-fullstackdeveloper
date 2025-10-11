# Day 58 — Accessibility (a11y)

## Objectives
- Ensure components meet WCAG basics
- Keyboard navigation for dialogs, menus, and widgets
- Color contrast and ARIA attributes

## Practices
- Use proper labels with p-float-label
- Provide aria-labels for icon-only buttons
- Ensure focus is visible and not obstructed
- Trap focus in modal dialogs; restore focus on close

## Example
```tsx
<Button icon="pi pi-trash" aria-label="Delete item" />
<Dialog aria-label="Create user dialog" />
<InputText aria-invalid={!!errors.name} />
```

## Checklist
- [ ] Keyboard navigation works
- [ ] Focus management correct in modals
- [ ] Contrast meets AA
- [ ] Landmarks used (main, nav, header)