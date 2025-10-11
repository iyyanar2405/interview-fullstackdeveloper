# Day 57 — Testing PrimeNG Components

Build confidence with unit tests around PrimeNG-driven UIs. Learn to configure TestBed, interact with components, and assert DOM behavior.

## Learning objectives

- Configure Angular testing with PrimeNG modules and services
- Test DOM rendering and interactions (table, dialog, inputs)
- Mock PrimeNG services like MessageService
- Avoid brittle tests tied to internal markup

## 1) TestBed setup

```ts
beforeEach(async () => {
  await TestBed.configureTestingModule({
    declarations: [ProductsTableComponent],
    imports: [NoopAnimationsModule, TableModule, ButtonModule, InputTextModule],
    providers: [MessageService],
  }).compileComponents();
});
```

## 2) Render and assert table rows

```ts
it('renders rows', () => {
  const fixture = TestBed.createComponent(ProductsTableComponent);
  fixture.componentInstance.products = [
    { id: '1', name: 'A', price: 10, stock: 5 },
    { id: '2', name: 'B', price: 20, stock: 3 },
  ];
  fixture.detectChanges();

  const rows = fixture.nativeElement.querySelectorAll('tbody tr');
  expect(rows.length).toBe(2);
});
```

## 3) Interaction: click button to open dialog

```ts
it('opens dialog on button click', () => {
  const fixture = TestBed.createComponent(ProductsTableComponent);
  fixture.detectChanges();

  const button = fixture.debugElement.query(By.css('button'));
  button.triggerEventHandler('click', {});
  fixture.detectChanges();

  const dialog = fixture.nativeElement.querySelector('div.p-dialog');
  expect(dialog).toBeTruthy();
});
```

## 4) Mock MessageService

```ts
const addSpy = jasmine.createSpy('add');
TestBed.overrideProvider(MessageService, { useValue: { add: addSpy } });
```

## 5) Tips for stable tests

- Prefer By.css selectors on your own test ids/classes
- Avoid deep reliance on PrimeNG internal class names
- Use NoopAnimationsModule to prevent timing flakiness
- For async UI, use fixture.whenStable() or fakeAsync/tick

## Checklist

- [ ] TestBed config imports required PrimeNG modules
- [ ] Render assertions avoid brittle selectors
- [ ] Interactions (click, input) covered
- [ ] Services mocked as needed

## Key takeaways

- Test your behavior and DOM outputs, not PrimeNG internals
- Keep components thin to make testing easier

## Next day

Ensure accessibility and inclusive UX with A11y best practices.
