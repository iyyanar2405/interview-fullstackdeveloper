# Day 37 — Dialog Patterns

## Objectives
- Build accessible modal dialogs with PrimeReact Dialog
- Manage open/close state and focus trapping
- Compose confirm dialogs and forms within dialogs

## Basic Dialog
```tsx
export default function DialogBasics() {
  const [visible, setVisible] = useState(false);

  return (
    <div className="card">
      <Button label="Open" icon="pi pi-external-link" onClick={() => setVisible(true)} />
      <Dialog header="Details" visible={visible} style={{ width: '30rem' }} modal onHide={() => setVisible(false)}>
        <p className="m-0">Content goes here. Dialog is modal by default and traps focus.</p>
      </Dialog>
    </div>
  );
}
```

## Form in Dialog
```tsx
function DialogWithForm() {
  const [visible, setVisible] = useState(false);
  const { register, handleSubmit, reset } = useForm({ defaultValues: { name: '' } });

  const onSubmit = (data: any) => {
    console.log(data);
    setVisible(false);
    reset();
  };

  return (
    <>
      <Button label="New Item" onClick={() => setVisible(true)} />
      <Dialog header="Create Item" visible={visible} modal onHide={() => setVisible(false)}>
        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-3">
          <span className="p-float-label">
            <InputText id="name" {...register('name', { required: 'Required' })} />
            <label htmlFor="name">Name</label>
          </span>
          <div className="flex justify-content-end gap-2">
            <Button type="button" label="Cancel" severity="secondary" onClick={() => setVisible(false)} />
            <Button type="submit" label="Save" />
          </div>
        </form>
      </Dialog>
    </>
  );
}
```

## Confirmation
```tsx
function ConfirmDelete({ onConfirm }: { onConfirm: () => void }) {
  const confirm = useRef<ConfirmDialog>(null);

  const show = () => {
    confirmDialog({
      message: 'Delete this record?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      accept: onConfirm
    });
  };

  return (
    <>
      <ConfirmDialog />
      <Button label="Delete" icon="pi pi-trash" severity="danger" onClick={show} />
    </>
  );
}
```

## Exercises
- Create a reusable Modal component with size, header, and footer props
- Add ESC key handling and restore focus to trigger button
- Prevent closing when form is dirty unless confirmed

## Checklist
- [ ] Modal opens/closes and traps focus
- [ ] Forms submit and reset within dialog
- [ ] Confirm dialog integrated
- [ ] Accessibility attributes present