# Day 38 — Sidebar & Wizard

## Objectives
- Use Sidebar for off-canvas navigation
- Build multi-step wizards with Stepper or custom logic
- Persist state between steps

## Sidebar Navigation
```tsx
export default function SidebarDemo() {
  const [visible, setVisible] = useState(false);

  return (
    <div className="card">
      <Button icon="pi pi-bars" onClick={() => setVisible(true)} />
      <Sidebar visible={visible} onHide={() => setVisible(false)} position="left">
        <h3>Menu</h3>
        <ul className="list-none p-0 m-0">
          <li><a className="p-menuitem-link">Dashboard</a></li>
          <li><a className="p-menuitem-link">Users</a></li>
          <li><a className="p-menuitem-link">Settings</a></li>
        </ul>
      </Sidebar>
    </div>
  );
}
```

## Wizard with Steps
```tsx
type Form = { name: string; email: string; plan: string };

function Wizard() {
  const [activeIndex, setActiveIndex] = useState(0);
  const { register, handleSubmit, getValues } = useForm<Form>({ defaultValues: { plan: 'basic' } });

  const steps = [
    { label: 'Profile' },
    { label: 'Plan' },
    { label: 'Review' }
  ];

  const next = () => setActiveIndex((i) => Math.min(steps.length - 1, i + 1));
  const prev = () => setActiveIndex((i) => Math.max(0, i - 1));

  const onSubmit = (v: Form) => console.log('Submit', v);

  return (
    <div className="card">
      <Steps model={steps} activeIndex={activeIndex} readOnly />

      {activeIndex === 0 && (
        <div className="mt-4 grid gap-3">
          <InputText placeholder="Name" {...register('name')} />
          <InputText placeholder="Email" {...register('email')} />
        </div>
      )}

      {activeIndex === 1 && (
        <div className="mt-4">
          <Dropdown options={[{ label: 'Basic', value: 'basic' }, { label: 'Pro', value: 'pro' }]} {...register('plan')} />
        </div>
      )}

      {activeIndex === 2 && (
        <pre className="mt-4">{JSON.stringify(getValues(), null, 2)}</pre>
      )}

      <div className="flex justify-content-between mt-4">
        <Button label="Back" onClick={prev} disabled={activeIndex === 0} />
        {activeIndex < steps.length - 1 ? (
          <Button label="Next" onClick={next} />
        ) : (
          <Button label="Finish" onClick={handleSubmit(onSubmit)} />
        )}
      </div>
    </div>
  );
}
```

## Exercises
- Guard navigation unless current step is valid
- Store wizard state in URL query or localStorage
- Add a stepper sidebar that highlights current step

## Checklist
- [ ] Sidebar opens/closes smoothly
- [ ] Steps indicate progress
- [ ] State preserved across steps
- [ ] Validation gates step changes