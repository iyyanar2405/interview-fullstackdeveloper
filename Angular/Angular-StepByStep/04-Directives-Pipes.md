# 04 — Directives & Pipes

Use built-in directives, new control flow, and custom pipes.

## Built-in directives
- `*ngIf`, `*ngFor`, `ngClass`, `ngStyle`

## New control flow (Angular v17+)
```html
@if (items.length === 0) {
  <p>No items</p>
} @else {
  @for (item of items; track item) {
    <p>{{item}}</p>
  }
}
```

## Custom pipe
```ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'titleCase', standalone: true })
export class TitleCasePipe implements PipeTransform {
  transform(value: string) {
    return value.replace(/\w\S*/g, (txt) => txt[0].toUpperCase() + txt.slice(1).toLowerCase());
  }
}
```

Next: Services and dependency injection.
