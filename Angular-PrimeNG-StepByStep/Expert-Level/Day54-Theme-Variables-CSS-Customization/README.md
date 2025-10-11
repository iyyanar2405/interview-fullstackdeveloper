# Day 54 — Theme Variables & CSS Customization

Deepen your theming mastery by exploring PrimeNG theme variables and per-component customization patterns. You’ll map brand tokens to CSS variables, adjust component density, and build maintainable style layers.

## Learning objectives

- Understand theme variable layers and cascade order
- Customize density (compact/comfortable) and shape (radii) across components
- Override styles safely with CSS variables/scoped containers
- Fine-tune p-table, p-dialog, p-overlay, and form controls

## 1) Variable layering model

Recommended order:
1) Base Theme CSS (e.g., Lara/Aura)
2) PrimeNG core CSS
3) Global utilities
4) Brand variables (root scope)
5) Optional dark-mode/brand scopes (attribute/class)
6) Minimal component-level tweaks

Keep 4–6 in your `src/styles` and load after theme CSS.

## 2) Brand token map (recap)

```scss
/* src/styles/tokens.scss */
:root {
  /* Core brand */
  --brand-primary: #6366f1; /* maps to --primary-500 */
  --brand-accent:  #06b6d4;
  --brand-text:    #1f2937;

  /* Shape & focus */
  --brand-radius:  10px;
  --brand-focus:   0 0 0 0.2rem rgba(99, 102, 241, 0.35);
}
```

Bridge brand tokens to theme variables:

```scss
/* src/styles/theme-bridge.scss */
:root {
  --primary-500: var(--brand-primary);
  --text-color:  var(--brand-text);
  --border-radius: var(--brand-radius);
  --focus-ring:  var(--brand-focus);
}
```

## 3) Density system (compact tables and inputs)

```scss
/* compact density */
:root[data-density='compact'] {
  /* Inputs */
  --input-padding-y: 0.35rem; /* used by many themes */
  --input-padding-x: 0.6rem;

  /* Table */
  --datatable-header-cell-padding: 0.5rem 0.6rem;
  --datatable-cell-padding: 0.4rem 0.6rem;
}

/* usage */
document.documentElement.setAttribute('data-density', 'compact');
```

Target components that don’t read variables directly:

```scss
.p-inputtext { padding: var(--input-padding-y) var(--input-padding-x); }
.p-table .p-datatable-thead > tr > th { padding: var(--datatable-header-cell-padding); }
.p-table .p-datatable-tbody > tr > td { padding: var(--datatable-cell-padding); }
```

## 4) Dialogs and overlays

```scss
/* Dialog header/footer treatment */
.p-dialog .p-dialog-header { background: var(--surface-50); }
.p-dialog .p-dialog-footer { border-top: 1px solid rgba(0,0,0,.05); }

/* Overlay panel elevation and z-index discipline */
.p-overlaypanel {
  box-shadow: 0 8px 24px rgba(0,0,0,.12);
}
```

## 5) Form states and validation

```scss
/* error coloring through variables */
:root {
  --danger-500: #ef4444;
}
.p-inputtext.ng-invalid.ng-touched {
  border-color: var(--danger-500);
  box-shadow: 0 0 0 1px color-mix(in srgb, var(--danger-500) 30%, transparent);
}
.p-message.p-message-error .p-message-wrapper { border-left: 4px solid var(--danger-500); }
```

## 6) Scoped theming per container

Limit changes to a partial area:

```scss
.app-admin-theme {
  --brand-primary: #0ea5e9;
  --brand-radius: 8px;
}
.app-admin-theme .p-button { border-radius: var(--brand-radius); }
```

## 7) Checklist

- [ ] Variables loaded after theme CSS
- [ ] Density toggle verified on table and inputs
- [ ] Dialog/overlay styles consistent with brand
- [ ] Form error states readable and accessible
- [ ] Scoped overrides don’t leak globally

## Key takeaways

- Keep brand tokens separate from theme variables; bridge them via a small map
- Prefer CSS variables and scoped containers over deep selectors
- Test density, spacing, and state styles in both light and dark modes

## Next day

Move to performance optimization: rendering strategies, change detection, and data flow.
