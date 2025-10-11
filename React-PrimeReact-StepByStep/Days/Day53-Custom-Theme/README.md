# Day 53 — Custom Theme

## Objectives
- Create a custom PrimeReact theme by overriding variables
- Set up a maintainable theming strategy
- Support light/dark variants

## PrimeReact Theme Override
- Create a CSS file that imports a base theme and overrides CSS variables.

```css
/* src/theme/custom-light.css */
@import "primereact/resources/themes/lara-light-indigo/theme.css";

:root {
  --primary-500: #7c3aed; /* grape */
  --primary-600: #6d28d9;
  --surface-border: #e6e6f0;
}
```

```css
/* src/theme/custom-dark.css */
@import "primereact/resources/themes/lara-dark-indigo/theme.css";

:root {
  --primary-500: #a78bfa;
  --surface-900: #0f1226;
}
```

## Applying Theme
```tsx
function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [dark, setDark] = useState(false);
  useEffect(() => {
    const linkId = 'theme-link';
    let link = document.getElementById(linkId) as HTMLLinkElement | null;
    if (!link) {
      link = document.createElement('link');
      link.id = linkId;
      link.rel = 'stylesheet';
      document.head.appendChild(link);
    }
    link.href = dark ? '/theme/custom-dark.css' : '/theme/custom-light.css';
  }, [dark]);

  return (
    <div>
      <div className="p-3"><InputSwitch checked={dark} onChange={(e) => setDark(e.value)} /> Dark</div>
      {children}
    </div>
  );
}
```

## Checklist
- [ ] Theme files load via link
- [ ] Variables override base theme
- [ ] Toggle persists preference
- [ ] No FOUC on theme swap