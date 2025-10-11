# Day 53 — Custom PrimeNG Theme Creation

Give your app a unique brand identity by customizing PrimeNG’s theme system. Today you’ll set up a brand palette, override CSS variables, support light/dark modes, and apply component-level tweaks safely.

## Learning objectives

- Understand PrimeNG theming layers (base CSS + theme CSS + variables)
- Define a brand palette and map it to PrimeNG CSS variables
- Apply global and per-scope (container) theme overrides
- Support light/dark modes and on-the-fly theme switching
- Customize common components (Button, Input, Table, Dialog, Toast)

## Prerequisites

- Angular 16+ project using PrimeNG (Lara/Aura themes recommended)
- PrimeIcons and PrimeFlex installed (optional but recommended)
- Global styles configured to process CSS/SCSS

## Approaches overview

- Approach A — CSS Variables Override (Recommended): Safest and future-proof. Override the theme’s exposed CSS variables (e.g., Lara) without forking theme files.
- Approach B — Full Custom Theme Build (Advanced): Create your own CSS theme bundle (copy/extend) and reference it. This is more maintenance-heavy and generally not needed.

We’ll use Approach A.

## 1) Confirm base theme setup

Ensure you’re importing a stock theme plus base styles in `angular.json` or `styles.scss`.

Common setup in `angular.json` (styles section):

```json
{
  "styles": [
    "node_modules/primeng/resources/themes/lara-light-blue/theme.css",
    "node_modules/primeng/resources/primeng.min.css",
    "node_modules/primeicons/primeicons.css",
    "src/styles.scss"
  ]
}
```

Alternatively import in `src/styles.scss`:

```scss
@import 'primeng/resources/themes/lara-light-blue/theme.css';
@import 'primeng/resources/primeng.min.css';
@import 'primeicons/primeicons.css';
```

## 2) Create a brand variables file

Create `src/styles/theme-brand.scss` and define a palette. We’ll override CSS variables exposed by Lara/Aura themes.

```scss
/* src/styles/theme-brand.scss */
:root {
  /* Brand colors (primary) */
  --primary-50:  #eef2ff;
  --primary-100: #e0e7ff;
  --primary-200: #c7d2fe;
  --primary-300: #a5b4fc;
  --primary-400: #818cf8;
  --primary-500: #6366f1;  /* main */
  --primary-600: #4f46e5;
  --primary-700: #4338ca;
  --primary-800: #3730a3;
  --primary-900: #312e81;

  /* Accent & semantic */
  --accent-500:  #06b6d4; /* cyan */
  --success-500: #22c55e;
  --info-500:    #3b82f6;
  --warn-500:    #f59e0b;
  --danger-500:  #ef4444;

  /* Surfaces & text */
  --surface-0:  #ffffff;
  --surface-50: #f9fafb;
  --surface-100:#f3f4f6;
  --surface-900:#0b1020;
  --text-color: #1f2937;

  /* Radii & focus */
  --border-radius: 10px;
  --focus-ring: 0 0 0 0.2rem rgba(99, 102, 241, 0.35);
}

/* Optional: scoped brand container if you want to theme only a section */
.app-brand {
  --border-radius: 12px;
}
```

Then import it last in `src/styles.scss` so overrides win CSS specificity:

```scss
/* src/styles.scss */
@import 'primeng/resources/themes/lara-light-blue/theme.css';
@import 'primeng/resources/primeng.min.css';
@import 'primeicons/primeicons.css';

/* Your global utilities */
@import './styles/utilities.scss';

/* Brand overrides last */
@import './styles/theme-brand.scss';
```

## 3) Component-level polish (examples)

Buttons and inputs pick up `--primary-*` and radii automatically. Fine-tune specific components if needed.

```scss
/* Buttons */
.p-button {
  border-radius: var(--border-radius);
}
.p-button.p-button-secondary {
  background: var(--surface-100);
  color: var(--text-color);
}

/* Inputs */
.p-inputtext, .p-dropdown, .p-calendar .p-inputtext {
  border-radius: var(--border-radius);
}

/* Tables */
.p-table .p-datatable-thead > tr > th {
  background: var(--surface-50);
}
.p-table .p-datatable-tbody > tr:hover {
  background: color-mix(in srgb, var(--primary-500) 6%, white);
}

/* Dialog */
.p-dialog .p-dialog-header {
  background: var(--surface-50);
}

/* Toast */
.p-toast .p-toast-message.p-toast-message-success {
  border-left: 4px solid var(--success-500);
}
```

Tip: Prefer variable-driven styling to keep changes centralized.

## 4) Dark mode support

Create a dark theme scope by overriding the same variables under a class or attribute.

```scss
/* src/styles/theme-dark.scss */
:root[data-theme='dark'] {
  --surface-0:  #0f172a;
  --surface-50: #0b1020;
  --surface-100:#111827;
  --text-color: #e5e7eb;

  --primary-500: #22d3ee; /* lighter primary for dark backgrounds */
  --focus-ring:  0 0 0 0.2rem rgba(34, 211, 238, 0.35);
}
```

Import after the base theme as well (order matters):

```scss
@import './styles/theme-brand.scss';
@import './styles/theme-dark.scss';
```

Toggle at runtime in a root component:

```ts
// theme.service.ts
@Injectable({ providedIn: 'root' })
export class ThemeService {
  setDark(enabled: boolean) {
    document.documentElement.setAttribute('data-theme', enabled ? 'dark' : 'light');
  }
}
```

```html
<!-- header.component.html -->
<p-inputSwitch (onChange)="theme.setDark($event.checked)"></p-inputSwitch>
```

## 5) Theme switching (multiple palettes)

If you want multiple brands, create additional variable files and scope them:

```scss
/* Blue brand */
:root[data-brand='blue'] { --primary-500: #3b82f6; }
/* Emerald brand */
:root[data-brand='emerald'] { --primary-500: #10b981; }
```

Switch at runtime:

```ts
setBrand(name: 'blue' | 'emerald') {
  document.documentElement.setAttribute('data-brand', name);
}
```

## 6) Verifying contrast and accessibility

- Ensure sufficient contrast between text and background (WCAG AA or AAA)
- Check focus ring visibility on keyboard navigation
- Verify table row hover and selection are distinguishable in both modes

## 7) Common pitfalls

- Import order: your overrides must come after the theme CSS
- Specificity wars: prefer variable overrides over deep selectors
- Cache busting: after changing global CSS, do a hard refresh
- Don’t edit files under node_modules; keep overrides in your src/

## 8) Quick test checklist

- [ ] Buttons adopt brand color and radius
- [ ] Inputs and dropdowns match brand radius
- [ ] Table header background uses surface color
- [ ] Dark mode flips backgrounds and text color
- [ ] Toast/severity colors look balanced

## 9) Optional: Theming PrimeFlex utilities

PrimeFlex utilities can be extended using custom classes that reference your variables:

```scss
/* spacing & color helpers */
.brand-accent { color: var(--accent-500); }
.bg-surface { background: var(--surface-0); }
.rounded-brand { border-radius: var(--border-radius); }
```

## 10) Try it (Windows PowerShell)

```powershell
# If you haven’t chosen a theme yet, install PrimeNG and set a base theme
npm install primeng primeicons; npm install @angular/animations

# Run the dev server and validate brand overrides
ng serve
```

## Key takeaways

- Modern PrimeNG themes expose CSS variables—override them, don’t fork
- Keep all brand knobs as variables for consistency and scale
- Support dark mode by overriding the same variables under a separate scope
- Test for contrast and accessibility early

## Next day

Continue with performance optimization: tune change detection, virtual scrolling, and bundle size improvements.
