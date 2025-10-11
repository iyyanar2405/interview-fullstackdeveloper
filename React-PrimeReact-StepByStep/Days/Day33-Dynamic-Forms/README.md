# Day 33 — Dynamic Forms

## Objectives
- Render form fields from a JSON schema
- Support various PrimeReact inputs
- Centralize validation and default values

## Schema-driven fields
```tsx
type Field = {
  name: string;
  label: string;
  type: 'text' | 'password' | 'number' | 'dropdown' | 'calendar' | 'checkbox';
  options?: { label: string; value: any }[];
  rules?: any;
};

const fields: Field[] = [
  { name: 'firstName', label: 'First Name', type: 'text', rules: { required: 'Required' } },
  { name: 'age', label: 'Age', type: 'number' },
  { name: 'role', label: 'Role', type: 'dropdown', options: [ { label: 'User', value: 'user' }, { label: 'Admin', value: 'admin' } ] },
  { name: 'joinDate', label: 'Join Date', type: 'calendar' },
  { name: 'active', label: 'Active', type: 'checkbox' }
];

export default function DynamicForm() {
  const { control, register, handleSubmit, formState: { errors } } = useForm({ defaultValues: { active: true } });

  const renderField = (f: Field) => {
    switch (f.type) {
      case 'text':
        return <InputText {...register(f.name, f.rules)} className={errors[f.name] && 'p-invalid'} />;
      case 'password':
        return <Password {...register(f.name, f.rules)} toggleMask className={errors[f.name] && 'p-invalid'} />;
      case 'number':
        return (
          <Controller name={f.name} control={control} render={({ field }) => <InputNumber {...field} />} />
        );
      case 'dropdown':
        return (
          <Controller name={f.name} control={control} render={({ field }) => <Dropdown {...field} options={f.options} optionLabel="label" optionValue="value" />} />
        );
      case 'calendar':
        return (
          <Controller name={f.name} control={control} render={({ field }) => <Calendar {...field} showIcon />} />
        );
      case 'checkbox':
        return (
          <Controller name={f.name} control={control} render={({ field }) => <Checkbox checked={field.value} onChange={(e) => field.onChange(e.checked)} />} />
        );
    }
  };

  return (
    <form onSubmit={handleSubmit((v) => console.log(v))} className="grid">
      {fields.map((f) => (
        <div className="col-12 md:col-6" key={f.name}>
          <label className="block mb-1">{f.label}</label>
          {renderField(f)}
          {errors[f.name] && <small className="p-error">{String(errors[f.name]?.message)}</small>}
        </div>
      ))}
      <div className="col-12"><Button type="submit" label="Submit" /></div>
    </form>
  );
}
```

## Exercises
- Add RadioButton and MultiSelect support
- Support conditional fields (show/hide based on other values)
- Persist schema to JSON and load dynamically

## Checklist
- [ ] Schema maps to inputs
- [ ] Validation rules applied
- [ ] Errors show under fields
- [ ] Extensible for new types