# Day 57 — Testing

## Objectives
- Test components with React Testing Library and Jest
- Mock PrimeReact components where heavy
- Write unit and integration tests for forms and tables

## Example Test
```tsx
// ProductCard.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import ProductCard from './ProductCard';

test('adds to cart', () => {
  const onAdd = vi.fn();
  const p = { id: '1', name: 'Prod', price: 9.99 } as any;
  render(<ProductCard p={p} onAdd={onAdd} />);
  fireEvent.click(screen.getByRole('button', { name: /add/i }));
  expect(onAdd).toHaveBeenCalledWith(p);
});
```

## Form Test
```tsx
// UserForm.test.tsx
render(<UserForm />);
fireEvent.change(screen.getByLabelText(/name/i), { target: { value: '' } });
fireEvent.click(screen.getByRole('button', { name: /save/i }));
expect(await screen.findByText(/required/i)).toBeInTheDocument();
```

## Checklist
- [ ] Critical components have tests
- [ ] Async flows await UI updates
- [ ] Mocks used for heavy deps
- [ ] CI runs tests in PRs