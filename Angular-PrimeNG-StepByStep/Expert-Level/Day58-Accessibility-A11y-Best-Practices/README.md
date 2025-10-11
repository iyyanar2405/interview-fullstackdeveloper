# Day 58 — Accessibility (A11y) Best Practices

Make your app usable for everyone. Improve keyboard navigation, screen reader support, and color contrast while using PrimeNG.

## Learning objectives

- Ensure keyboard accessibility across dialogs, menus, and tables
- Provide proper labels and roles
- Maintain sufficient color contrast and focus indicators

## 1) Keyboard navigation

- Ensure focus moves into dialogs and returns on close
- Use `autofocus` or programmatic focus for first actionable element

```ts
@ViewChild('firstInput') firstInput!: ElementRef<HTMLInputElement>;
ngAfterViewInit() { setTimeout(() => this.firstInput?.nativeElement.focus(), 0); }
```

## 2) Labels and ARIA

- Use `aria-label` or `aria-labelledby` for icon-only buttons
- Provide table captions for context

```html
<button pButton icon="pi pi-search" aria-label="Search"></button>
<p-table [value]="data">
  <caption>Inventory</caption>
  <!-- columns -->
</p-table>
```

## 3) Focus visibility

```scss
:root {
  --focus-ring: 0 0 0 0.2rem rgba(99,102,241,.35);
}
:focus-visible { outline: var(--focus-ring); }
```

## 4) Contrast checks

- Ensure text/background contrast ratio >= 4.5:1 (AA)
- Validate states (hover, active, disabled) in light and dark modes

## 5) Forms and errors

- Associate labels with inputs using `for`/`id`
- Provide `aria-describedby` for error messages

```html
<label for="price">Price</label>
<input id="price" pInputText [ngModel]="price" aria-describedby="price-help price-error" />
<small id="price-help">Enter product price in USD.</small>
<p-message id="price-error" *ngIf="hasError" severity="error" text="Price is required"></p-message>
```

## 6) Tables and selection

- Use `aria-selected` for selected rows and ensure keyboard row focus
- Provide clear focus styles on row selection

## Checklist

- [ ] Dialogs trap focus and restore on close
- [ ] Icon buttons have accessible labels
- [ ] Focus ring visibly present on keyboard nav
- [ ] Contrast verified for brand colors
- [ ] Labels and errors properly associated

## Key takeaways

- A11y is a first-class quality attribute; bake it into components
- Test with keyboard only and a screen reader during development

## Next day

Prepare production builds and deploy with correct base paths and caching.
