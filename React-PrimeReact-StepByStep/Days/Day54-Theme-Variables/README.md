# Day 54 — Theme Variables

## Objectives
- Understand and tune PrimeOne CSS variables
- Create a design token map and usage guidelines
- Apply semantic tokens for consistent styling

## Tokens
```ts
export const tokens = {
  color: {
    brand: '#7c3aed',
    success: '#16a34a',
    danger: '#dc2626',
    warning: '#f59e0b'
  },
  spacing: {
    xs: '0.25rem',
    sm: '0.5rem',
    md: '1rem',
    lg: '1.5rem',
    xl: '2rem'
  },
  radius: {
    sm: '4px',
    md: '8px',
    lg: '12px'
  }
} as const;
```

## Applying Tokens
```css
:root {
  --brand-500: #7c3aed;
  --radius-md: 8px;
}

.button-brand {
  background: var(--brand-500);
  border-radius: var(--radius-md);
}
```

## Checklist
- [ ] Token definitions centralized
- [ ] CSS variables mapped clearly
- [ ] Components use semantic tokens
- [ ] Docs explain usage and scale