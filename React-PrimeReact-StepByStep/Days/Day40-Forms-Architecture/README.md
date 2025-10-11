# Day 40 — Forms Architecture

## Objectives
- Architect scalable forms with React Hook Form + PrimeReact
- Separate field components, validation, and persistence
- Support edit/create flows and optimistic updates

## Folder Structure
- forms/
  - components/
    - FieldText.tsx
    - FieldNumber.tsx
    - FieldDropdown.tsx
  - hooks/
    - useFormPersist.ts
  - schemas/
    - user.schema.ts
  - UserForm.tsx

## Field Component Example
```tsx
export function FieldText({ name, label, register, error }: { name: string; label: string; register: any; error?: string }) {
  return (
    <div className="field">
      <label className="block mb-1" htmlFor={name}>{label}</label>
      <InputText id={name} {...register(name)} className={error && 'p-invalid'} />
      {error && <small className="p-error">{error}</small>}
    </div>
  );
}
```

## Persist Hook
```tsx
export function useFormPersist<T>(key: string, watch: (name?: string) => T) {
  useEffect(() => {
    const sub = watch((value) => localStorage.setItem(key, JSON.stringify(value)));
    return () => sub.unsubscribe && sub.unsubscribe();
  }, [key, watch]);
}
```

## UserForm Composition
```tsx
export default function UserForm() {
  const schema = z.object({ name: z.string().min(1), email: z.string().email() });
  const { register, handleSubmit, formState: { errors }, watch } = useForm({ resolver: zodResolver(schema) });
  useFormPersist('userForm', watch);

  const onSubmit = async (v: any) => { /* optimistic save */ };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <FieldText name="name" label="Name" register={register} error={errors.name?.message as string} />
      <FieldText name="email" label="Email" register={register} error={errors.email?.message as string} />
      <Button type="submit" label="Save" />
    </form>
  );
}
```

## Exercises
- Create reusable field components for Dropdown and Calendar
- Add optimistic update with fallback toast on failure
- Extract validation schemas and reuse across pages

## Checklist
- [ ] Reusable field components
- [ ] Persisted draft state
- [ ] Optimistic create/update flows
- [ ] Validation centralized