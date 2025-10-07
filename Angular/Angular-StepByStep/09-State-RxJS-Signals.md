# 09 — State (RxJS/Signals)

Manage component and app state using RxJS and Signals.

## Signals basics
```ts
import { signal, computed } from '@angular/core';

const count = signal(0);
const doubled = computed(() => count() * 2);

count.set(1);
console.log(doubled()); // 2
```

## Simple store service (RxJS)
```ts
@Injectable({ providedIn: 'root' })
export class CounterStore {
  private _count = new BehaviorSubject(0);
  count$ = this._count.asObservable();
  inc(){ this._count.next(this._count.value + 1); }
}
```

## Component with signals
```ts
@Component({ selector: 'app-counter', standalone: true, template: `
  <button (click)="inc()">+</button>
  <span>{{count()}}</span>
`})
export class CounterComponent {
  count = signal(0);
  inc(){ this.count.update(v => v + 1); }
}
```

Next: Testing components and services.
