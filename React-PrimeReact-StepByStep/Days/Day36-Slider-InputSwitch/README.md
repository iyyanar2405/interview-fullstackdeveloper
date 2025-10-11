# Day 36 — Slider & InputSwitch

## Objectives
- Use Slider for ranges and InputSwitch for toggles
- Sync with numeric/text inputs
- Provide accessible labels and feedback

## Range with Slider
```tsx
export default function SliderDemo() {
  const { control, watch, setValue } = useForm({ defaultValues: { price: [10, 50], notifications: true, volume: 25 } });
  const price = watch('price');

  return (
    <div className="grid">
      <div className="col-12 md:col-6">
        <label className="block mb-2">Price Range: ${price[0]} - ${price[1]}</label>
        <Controller name="price" control={control} render={({ field }) => (
          <Slider value={field.value} onChange={(e) => field.onChange(e.value)} range min={0} max={100} step={1} />
        )} />
      </div>
      <div className="col-12 md:col-6">
        <label className="block mb-2">Volume</label>
        <Controller name="volume" control={control} render={({ field }) => (
          <Slider value={field.value} onChange={(e) => field.onChange(e.value)} min={0} max={100} />
        )} />
        <InputNumber value={watch('volume')} onValueChange={(e) => setValue('volume', e.value || 0)} className="mt-2" />
      </div>
      <div className="col-12 md:col-6">
        <label className="block mb-2">Notifications</label>
        <Controller name="notifications" control={control} render={({ field }) => (
          <div className="flex align-items-center gap-2">
            <InputSwitch checked={field.value} onChange={(e) => field.onChange(e.value)} />
            <span>{field.value ? 'Enabled' : 'Disabled'}</span>
          </div>
        )} />
      </div>
    </div>
  );
}
```

## Exercises
- Add a Slider with custom icons and colors
- Combine InputSwitch with a disabled state for inputs
- Persist preferences to localStorage

## Checklist
- [ ] Keyboard interaction works
- [ ] ARIA labels present
- [ ] Values synced with inputs
- [ ] Theme spacing consistent