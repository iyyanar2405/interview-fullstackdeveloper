# Day 34 — Form Arrays

## Objectives
- Manage dynamic arrays of fields with useFieldArray
- Add/remove/reorder items
- Validate nested fields

## Contacts Form
```tsx
import { useForm, useFieldArray, Controller } from 'react-hook-form';

type Contact = { name: string; email: string };

type FormData = { contacts: Contact[] };

export default function FormArrays() {
  const { control, register, handleSubmit } = useForm<FormData>({ defaultValues: { contacts: [{ name: '', email: '' }] } });
  const { fields, append, remove, move } = useFieldArray({ control, name: 'contacts' });

  return (
    <form onSubmit={handleSubmit((v) => console.log(v))} className="grid">
      {fields.map((f, idx) => (
        <div key={f.id} className="col-12 md:col-6">
          <div className="p-inputgroup mb-2">
            <span className="p-inputgroup-addon">{idx + 1}</span>
            <InputText placeholder="Name" {...register(`contacts.${idx}.name` as const)} />
            <InputText placeholder="Email" {...register(`contacts.${idx}.email` as const)} />
            <Button type="button" icon="pi pi-times" severity="danger" onClick={() => remove(idx)} />
          </div>
          <div className="flex gap-2">
            <Button type="button" icon="pi pi-arrow-up" onClick={() => move(idx, Math.max(0, idx - 1))} />
            <Button type="button" icon="pi pi-arrow-down" onClick={() => move(idx, Math.min(fields.length - 1, idx + 1))} />
          </div>
        </div>
      ))}
      <div className="col-12 flex gap-2">
        <Button type="button" label="Add Contact" icon="pi pi-plus" onClick={() => append({ name: '', email: '' })} />
        <Button type="submit" label="Save" />
      </div>
    </form>
  );
}
```

## Exercises
- Add nested phone numbers array inside each contact
- Validate unique emails within the array
- Drag-and-drop reordering with a library

## Checklist
- [ ] Append/remove work
- [ ] Reordering updates values
- [ ] Nested arrays supported
- [ ] Validation covers array rules