# Day 55 — Performance Optimization

Make your Angular + PrimeNG app fast and snappy. Today covers change detection, immutability, virtual scrolling, and production diagnostics.

## Learning objectives

- Use OnPush change detection and immutable data patterns
- Reduce re-renders with trackBy, pure pipes, and memoized selectors
- Virtualize heavy lists and tables
- Identify and fix performance bottlenecks

## 1) Change detection strategy

```ts
@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsComponent {
  @Input() products: Product[] | null = null;
}
```

Guidelines:
- Treat inputs as immutable; replace arrays/objects instead of mutating
- Prefer async pipe over manual subscribe where possible

## 2) trackBy for ngFor

```html
<tr *ngFor="let p of products; trackBy: trackById">
  <td>{{ p.name }}</td>
</tr>
```

```ts
trackById(index: number, item: { id: string }) { return item.id; }
```

## 3) Memoization: selectors & pipes

- Use NgRx selectors to derive data once per input set
- Prefer pure pipes for cheap computations; avoid heavy work in templates

## 4) Virtual scrolling

PrimeNG options:
- `p-virtualScroller` for custom templates
- `p-table` with `virtualScroll` and `virtualScrollItemSize`

```html
<p-table [value]="rows" [virtualScroll]="true" [virtualScrollItemSize]="48" [scrollHeight]="'500px'">
  <!-- columns -->
</p-table>
```

Tips:
- Fixed row height improves scroll performance
- Avoid complex cell templates and heavy pipes per cell

## 5) Avoid unnecessary bindings

- Minimize use of functions directly in templates
- Break large components into smaller OnPush children
- Debounce valueChanges (forms, search)

## 6) Diagnostics

- Build with source maps and analyze bundle
- Identify large dependencies and consider lazy loading

Optional commands (documentation only):

```powershell
# Production build
ng build --configuration production

# Budget warnings can be tuned in angular.json (budgets section)
```

## 7) RxJS performance

- Prefer `switchMap` for cancelable streams (search)
- Use `shareReplay({ bufferSize: 1, refCount: true })` for hot streams
- Throttle/debounce noisy sources

## Checklist

- [ ] Changed heavy components to OnPush
- [ ] Added trackBy to large ngFor lists
- [ ] Virtual scroll enabled where appropriate
- [ ] Debounced search and form streams
- [ ] Bundle size checked in production build

## Key takeaways

- Rendering cost is driven by bindings and change detection; make them predictable
- Virtualization is essential for large datasets
- Measure in production mode; optimize based on data

## Next day

Implement lazy loading and code splitting for routes and feature modules.
