# Day 39 — Toast & Messages

## Objectives
- Show feedback with Toast and Message/Inline messages
- Centralize notifications
- Handle success, warning, and error states

## Toast Basics
```tsx
export default function ToastDemo() {
  const toast = useRef<Toast>(null);

  const show = (severity: 'success' | 'info' | 'warn' | 'error', summary: string, detail?: string) => {
    toast.current?.show({ severity, summary, detail, life: 2500 });
  };

  return (
    <div className="card flex flex-column gap-2">
      <Toast ref={toast} />
      <div className="flex gap-2">
        <Button label="Success" onClick={() => show('success', 'Saved', 'Changes saved')} />
        <Button label="Warn" severity="warning" onClick={() => show('warn', 'Warning', 'Be careful')} />
        <Button label="Error" severity="danger" onClick={() => show('error', 'Error', 'Something failed')} />
      </div>
    </div>
  );
}
```

## Inline Field Errors
```tsx
function InlineErrors() {
  const { register, handleSubmit, formState: { errors } } = useForm({});

  return (
    <form onSubmit={handleSubmit(console.log)} className="grid gap-2">
      <span className="p-float-label">
        <InputText id="name" {...register('name', { required: 'Name is required' })} className={errors.name && 'p-invalid'} />
        <label htmlFor="name">Name</label>
      </span>
      {errors.name && <small className="p-error">{String(errors.name.message)}</small>}
      <Button label="Submit" />
    </form>
  );
}
```

## Central Notification Hook
```tsx
const ToastContext = createContext<{ show: (m: ToastMessage) => void } | null>(null);

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const ref = useRef<Toast>(null);
  const show = (m: ToastMessage) => ref.current?.show(m);
  return (
    <ToastContext.Provider value={{ show }}>
      <Toast ref={ref} />
      {children}
    </ToastContext.Provider>
  );
}

export function useNotify() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('Wrap in ToastProvider');
  return ctx.show;
}
```

## Exercises
- Replace ad-hoc toasts with useNotify hook
- Add global error boundary that shows a toast on error
- Queue server validation errors into individual field messages

## Checklist
- [ ] Toast displays severities
- [ ] Inline errors styled with p-error
- [ ] Global notify hook works
- [ ] No duplicate messages