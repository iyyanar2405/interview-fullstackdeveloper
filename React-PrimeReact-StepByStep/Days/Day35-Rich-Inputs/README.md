# Day 35 — Rich Inputs

## Objectives
- Use Editor, InputMask, Chips, ColorPicker
- Integrate with React Hook Form
- Normalize output values

## Examples
```tsx
export default function RichInputs() {
  const { control, handleSubmit } = useForm({ defaultValues: { content: '', phone: '', tags: [], color: '#1976d2' } });

  return (
    <form onSubmit={handleSubmit((v) => console.log(v))} className="grid">
      <div className="col-12">
        <label className="block mb-1">Content</label>
        <Controller name="content" control={control} render={({ field }) => <Editor value={field.value} onTextChange={(e) => field.onChange(e.htmlValue)} style={{ height: '200px' }} />} />
      </div>
      <div className="col-12 md:col-6">
        <label className="block mb-1">Phone</label>
        <Controller name="phone" control={control} render={({ field }) => <InputMask {...field} mask="(999) 999-9999" placeholder="(555) 123-4567" />} />
      </div>
      <div className="col-12 md:col-6">
        <label className="block mb-1">Tags</label>
        <Controller name="tags" control={control} render={({ field }) => <Chips value={field.value} onChange={(e) => field.onChange(e.value)} />} />
      </div>
      <div className="col-12 md:col-6">
        <label className="block mb-1">Color</label>
        <Controller name="color" control={control} render={({ field }) => <ColorPicker value={field.value} onChange={(e) => field.onChange(e.value)} inline />} />
      </div>
      <div className="col-12"><Button type="submit" label="Save" /></div>
    </form>
  );
}
```

## Exercises
- Sanitize HTML from Editor before saving
- Add currency formatting with InputNumber mode="currency"
- Create a reusable ColorSwatch component

## Checklist
- [ ] Controlled inputs wired to form
- [ ] Values normalized
- [ ] Validation rules added where needed
- [ ] UX consistent with theme