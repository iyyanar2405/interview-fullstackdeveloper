# Day 12 — Buttons & Icons

## Objectives
- Master all button variations
- Icon usage and customization
- Button groups and split buttons

## Button Types
```tsx
function ButtonShowcase() {
  return (
    <div className="p-d-flex p-flex-wrap p-gap-2">
      <Button label="Primary" />
      <Button label="Secondary" className="p-button-secondary" />
      <Button label="Success" className="p-button-success" />
      <Button label="Info" className="p-button-info" />
      <Button label="Warning" className="p-button-warning" />
      <Button label="Danger" className="p-button-danger" />
      <Button label="Help" className="p-button-help" />
      
      {/* Icon buttons */}
      <Button icon="pi pi-check" />
      <Button icon="pi pi-search" label="Search" />
      <Button icon="pi pi-user" label="Profile" iconPos="right" />
      
      {/* Sizes */}
      <Button label="Small" className="p-button-sm" />
      <Button label="Large" className="p-button-lg" />
      
      {/* Outlined and Text */}
      <Button label="Outlined" outlined />
      <Button label="Text" text />
      
      {/* Loading */}
      <Button label="Loading" loading />
    </div>
  );
}
```

## Split Button
```tsx
const items = [
  { label: 'Update', icon: 'pi pi-refresh' },
  { label: 'Delete', icon: 'pi pi-times' },
  { separator: true },
  { label: 'Home', icon: 'pi pi-home' }
];

<SplitButton label="Save" icon="pi pi-plus" model={items} />
```

## Exercise
- Create a toolbar with various button types
- Add confirmation dialogs to destructive actions

## Checklist
- [ ] All button variations displayed
- [ ] Icons properly aligned
- [ ] Split button menu works
- [ ] Button states (disabled, loading) functional