# 10 — Signals

Use Angular Signals in an Nx app and interop with RxJS/NgRx.

## Local state with signals
```ts
const count = signal(0);
const doubled = computed(() => count() * 2);
```

## Convert selectors to signals
```ts
import { toSignal } from '@angular/core/rxjs-interop';
const value = toSignal(store.select(selectValue), { initialValue: 0 });
```

## Bridge RxJS streams
```ts
import { toObservable } from '@angular/core/rxjs-interop';
const value$ = toObservable(value);
```

Use signals for local UI state, NgRx for app/domain state.
