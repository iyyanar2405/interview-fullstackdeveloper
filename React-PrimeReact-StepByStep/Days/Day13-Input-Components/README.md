# Day 13 — Input Components

## Objectives
- Master text inputs and variations
- Number inputs with formatting
- Textarea and input groups

## Text Inputs
```tsx
function InputShowcase() {
  const [value1, setValue1] = useState('');
  const [value2, setValue2] = useState('');
  const [value3, setValue3] = useState('');

  return (
    <div className="p-fluid">
      <div className="p-field">
        <label htmlFor="basic">Basic</label>
        <InputText 
          id="basic"
          value={value1} 
          onChange={(e) => setValue1(e.target.value)} 
        />
      </div>

      <div className="p-field">
        <label htmlFor="float">Float Label</label>
        <span className="p-float-label">
          <InputText 
            id="float"
            value={value2} 
            onChange={(e) => setValue2(e.target.value)} 
          />
          <label htmlFor="float">Username</label>
        </span>
      </div>

      <div className="p-field">
        <label htmlFor="disabled">Disabled</label>
        <InputText id="disabled" disabled value="Disabled Text" />
      </div>
    </div>
  );
}
```

## Number Input
```tsx
<InputNumber 
  value={price} 
  onValueChange={(e) => setPrice(e.value)} 
  mode="currency" 
  currency="USD" 
  locale="en-US" 
/>

<InputNumber 
  value={percentage} 
  onValueChange={(e) => setPercentage(e.value)} 
  suffix="%" 
  min={0} 
  max={100} 
/>
```

## Textarea
```tsx
<InputTextarea 
  value={description} 
  onChange={(e) => setDescription(e.target.value)} 
  rows={5} 
  cols={30} 
  autoResize 
/>
```

## Exercise
- Build a product form with all input types
- Add validation and error states

## Checklist
- [ ] All input variations work
- [ ] Number formatting displays correctly
- [ ] Validation states show properly
- [ ] Accessibility labels present