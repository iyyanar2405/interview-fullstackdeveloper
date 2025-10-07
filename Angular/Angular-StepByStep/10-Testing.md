# 10 — Testing

Test components and services with Angular TestBed.

## Component test
```ts
import { TestBed } from '@angular/core/testing';

it('renders greeting', async () => {
  const fixture = await TestBed.configureTestingModule({
    imports: [HelloComponent]
  }).createComponent(HelloComponent);
  fixture.detectChanges();
  expect(fixture.nativeElement.textContent).toContain('Hello');
});
```

## Service test
```ts
it('adds todos', () => {
  const svc = new TodoService();
  svc.add('X');
  expect(svc.list()).toContain('X');
});
```

Next: Performance techniques.
