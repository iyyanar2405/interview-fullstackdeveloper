# 13 — State Management with NgRx

Use NgRx for predictable, testable state in medium-to-large apps.

## Install
```powershell
npm i @ngrx/store @ngrx/effects @ngrx/entity @ngrx/store-devtools
# schematics (optional) for generators
npm i -D @ngrx/schematics
```

Enable schematics default (optional):
```powershell
ng config cli.schematicCollections ["@ngrx/schematics"]
```

## Minimal store setup
```ts
// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { counterReducer } from './state/counter.reducer';
import { CounterEffects } from './state/counter.effects';

bootstrapApplication(AppComponent, {
  providers: [
    provideStore({ counter: counterReducer }),
    provideEffects([CounterEffects])
  ]
});
```

## Actions and reducer
```ts
// state/counter.actions.ts
import { createAction, props } from '@ngrx/store';
export const increment = createAction('[Counter] Increment');
export const decrement = createAction('[Counter] Decrement');
export const set = createAction('[Counter] Set', props<{ value: number }>());

// state/counter.reducer.ts
import { createReducer, on } from '@ngrx/store';
import * as CounterActions from './counter.actions';

export interface CounterState { value: number; }
export const initialState: CounterState = { value: 0 };

export const counterReducer = createReducer(
  initialState,
  on(CounterActions.increment, (s) => ({ ...s, value: s.value + 1 })),
  on(CounterActions.decrement, (s) => ({ ...s, value: s.value - 1 })),
  on(CounterActions.set, (s, { value }) => ({ ...s, value }))
);
```

## Selectors
```ts
// state/counter.selectors.ts
import { createSelector, createFeatureSelector } from '@ngrx/store';
import { CounterState } from './counter.reducer';

export const selectCounter = createFeatureSelector<CounterState>('counter');
export const selectValue = createSelector(selectCounter, s => s.value);
```

## Effects (side-effects)
```ts
// state/counter.effects.ts
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import * as CounterActions from './counter.actions';
import { tap } from 'rxjs/operators';

export class CounterEffects {
  private actions$ = inject(Actions);
  logInc$ = createEffect(() => this.actions$.pipe(
    ofType(CounterActions.increment),
    tap(() => console.log('Increment clicked'))
  ), { dispatch: false });
}
```

## Component usage
```ts
import { Store } from '@ngrx/store';
import * as CounterActions from './state/counter.actions';
import { selectValue } from './state/counter.selectors';

@Component({ selector: 'app-counter', standalone: true, template: `
  <button (click)="dec()">-</button>
  <span>{{ value | async }}</span>
  <button (click)="inc()">+</button>
`})
export class CounterComponent {
  constructor(private store: Store) {}
  value = this.store.select(selectValue);
  inc(){ this.store.dispatch(CounterActions.increment()); }
  dec(){ this.store.dispatch(CounterActions.decrement()); }
}
```

## Entity adapter (CRUD collections)
```ts
import { createEntityAdapter, EntityState } from '@ngrx/entity';

export interface Todo { id: string; title: string; done: boolean; }
export interface TodosState extends EntityState<Todo> { loading: boolean; }

const adapter = createEntityAdapter<Todo>();
const initial = adapter.getInitialState({ loading: false });

export const todosReducer = createReducer(initial,
  on(loadTodos, (s) => ({ ...s, loading: true })),
  on(loadTodosSuccess, (s, { todos }) => adapter.setAll(todos, { ...s, loading: false })),
  on(addTodoSuccess, (s, { todo }) => adapter.addOne(todo, s)),
);

export const { selectAll: selectAllTodos } = adapter.getSelectors();
```

## DevTools
```ts
import { provideStoreDevtools } from '@ngrx/store-devtools';
bootstrapApplication(AppComponent, {
  providers: [
    provideStore({ counter: counterReducer }),
    provideStoreDevtools({ maxAge: 25, logOnly: false })
  ]
});
```

## NgRx + Signals interop
- Use `toSignal` (from `@angular/core/rxjs-interop`) to convert selector observables to signals for template ergonomics.

```ts
import { toSignal } from '@angular/core/rxjs-interop';

@Component({ /* ... */})
export class CounterWithSignalComponent {
  value = toSignal(this.store.select(selectValue), { initialValue: 0 });
}
```

Tips:
- Keep reducers pure; put side effects in Effects
- Use feature slices per domain
- Prefer `@ngrx/entity` for lists
