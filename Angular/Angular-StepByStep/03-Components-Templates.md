# 03 — Components & Templates

Create standalone components and use bindings and events.

## Standalone component
```ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-hello',
  standalone: true,
  template: `<h2>Hello, {{name}}!</h2>`
})
export class HelloComponent {
  name = 'Angular';
}
```

## Data binding types
- Interpolation: `{{ value }}`
- Property binding: `[prop]="value"`
- Event binding: `(event)="handler()"`
- Two-way binding: `[(ngModel)]="value"`

## Example: list and click handler
```ts
@Component({
  selector: 'app-list',
  standalone: true,
  template: `
    <ul>
      <li *ngFor="let item of items; index as i" (click)="select(i)">
        {{i + 1}}. {{item}}
      </li>
    </ul>
    <p *ngIf="selectedIndex !== null">Selected: {{items[selectedIndex!]}}</p>
  `
})
export class ListComponent {
  items = ['One', 'Two', 'Three'];
  selectedIndex: number | null = null;
  select(i: number) { this.selectedIndex = i; }
}
```

Next: Directives and pipes.
