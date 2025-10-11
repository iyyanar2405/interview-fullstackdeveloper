# Day 51 — State Management (Redux Toolkit / Zustand)

## Objectives
- Choose between Redux Toolkit and Zustand
- Set up global state management
- Connect components to state

## Option 1: Redux Toolkit
```bash
npm install @reduxjs/toolkit react-redux
```

```tsx
// store/store.ts
import { configureStore } from '@reduxjs/toolkit';
import counterSlice from './counterSlice';

export const store = configureStore({
  reducer: {
    counter: counterSlice,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

// store/counterSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface CounterState {
  value: number;
}

const initialState: CounterState = {
  value: 0,
};

export const counterSlice = createSlice({
  name: 'counter',
  initialState,
  reducers: {
    increment: (state) => {
      state.value += 1;
    },
    decrement: (state) => {
      state.value -= 1;
    },
    incrementByAmount: (state, action: PayloadAction<number>) => {
      state.value += action.payload;
    },
  },
});

export const { increment, decrement, incrementByAmount } = counterSlice.actions;
export default counterSlice.reducer;
```

## Option 2: Zustand (Simpler)
```bash
npm install zustand
```

```tsx
// store/useStore.ts
import { create } from 'zustand';

interface StoreState {
  count: number;
  increment: () => void;
  decrement: () => void;
  incrementByAmount: (amount: number) => void;
}

export const useStore = create<StoreState>((set) => ({
  count: 0,
  increment: () => set((state) => ({ count: state.count + 1 })),
  decrement: () => set((state) => ({ count: state.count - 1 })),
  incrementByAmount: (amount) => set((state) => ({ count: state.count + amount })),
}));

// Component usage
function Counter() {
  const { count, increment, decrement } = useStore();
  
  return (
    <div className="flex align-items-center gap-2">
      <Button icon="pi pi-minus" onClick={decrement} />
      <span className="text-2xl font-bold">{count}</span>
      <Button icon="pi pi-plus" onClick={increment} />
    </div>
  );
}
```

## Exercise
- Choose your preferred state management solution
- Create a todo app with global state
- Connect PrimeReact components to the store

## Checklist
- [ ] State management configured
- [ ] Components connected to state
- [ ] Actions dispatch correctly