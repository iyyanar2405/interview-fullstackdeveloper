# Day 43 — Users CRUD

## Objectives
- Implement CRUD for users with DataTable + Dialog forms
- Use React Hook Form with schema validation
- Mock API service with async functions

## Types and Service
```tsx
type User = { id: string; name: string; email: string; role: 'user' | 'admin' };

const api = {
  list: async (): Promise<User[]> => [
    { id: '1', name: 'Alice', email: 'alice@mail.com', role: 'admin' },
    { id: '2', name: 'Bob', email: 'bob@mail.com', role: 'user' }
  ],
  create: async (u: Omit<User, 'id'>): Promise<User> => ({ ...u, id: crypto.randomUUID() }),
  update: async (u: User): Promise<User> => u,
  remove: async (id: string): Promise<void> => { /* no-op */ }
};
```

## Page
```tsx
export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [selected, setSelected] = useState<User | null>(null);
  const [visible, setVisible] = useState(false);
  const notify = useNotify();

  useEffect(() => { api.list().then(setUsers); }, []);

  const onDelete = async (u: User) => {
    await api.remove(u.id);
    setUsers(users.filter(x => x.id !== u.id));
    notify({ severity: 'success', summary: 'Deleted', detail: u.name });
  };

  return (
    <div className="card">
      <div className="flex justify-content-between mb-3">
        <h3>Users</h3>
        <Button label="New" icon="pi pi-plus" onClick={() => { setSelected(null); setVisible(true); }} />
      </div>
      <DataTable value={users} dataKey="id" paginator rows={5} selectionMode="single" selection={selected} onSelectionChange={(e) => setSelected(e.value)}>
        <Column field="name" header="Name" />
        <Column field="email" header="Email" />
        <Column field="role" header="Role" />
        <Column header="Actions" body={(row: User) => (
          <div className="flex gap-2">
            <Button icon="pi pi-pencil" text onClick={() => { setSelected(row); setVisible(true); }} />
            <Button icon="pi pi-trash" text severity="danger" onClick={() => onDelete(row)} />
          </div>
        )} />
      </DataTable>

      <UserDialog visible={visible} value={selected} onHide={() => setVisible(false)} onSave={(u) => {
        if (u.id) setUsers(users.map(x => x.id === u.id ? u : x)); else setUsers([{ ...u, id: crypto.randomUUID() }, ...users]);
        setVisible(false);
      }} />
    </div>
  );
}
```

## Dialog Form
```tsx
function UserDialog({ visible, value, onHide, onSave }: { visible: boolean; value: User | null; onHide: () => void; onSave: (u: User) => void }) {
  const schema = z.object({ name: z.string().min(1), email: z.string().email(), role: z.enum(['user', 'admin']) });
  const { register, handleSubmit, reset, formState: { errors } } = useForm<User>({ resolver: zodResolver(schema), defaultValues: value ?? { name: '', email: '', role: 'user', id: '' } as any });

  useEffect(() => { reset(value ?? { name: '', email: '', role: 'user', id: '' } as any); }, [value, reset]);

  return (
    <Dialog header={(value?.id ? 'Edit' : 'New') + ' User'} visible={visible} modal onHide={onHide} style={{ width: '30rem' }}>
      <form onSubmit={handleSubmit(onSave)} className="grid gap-2">
        <span className="p-float-label">
          <InputText id="name" {...register('name')} className={errors.name && 'p-invalid'} />
          <label htmlFor="name">Name</label>
        </span>
        {errors.name && <small className="p-error">{errors.name.message}</small>}

        <span className="p-float-label">
          <InputText id="email" {...register('email')} className={errors.email && 'p-invalid'} />
          <label htmlFor="email">Email</label>
        </span>
        {errors.email && <small className="p-error">{errors.email.message}</small>}

        <div>
          <label className="block mb-1">Role</label>
          <select className="p-inputtext" {...register('role')}>
            <option value="user">User</option>
            <option value="admin">Admin</option>
          </select>
        </div>

        <div className="flex justify-content-end gap-2 mt-3">
          <Button type="button" label="Cancel" severity="secondary" onClick={onHide} />
          <Button type="submit" label="Save" />
        </div>
      </form>
    </Dialog>
  );
}
```

## Checklist
- [ ] CRUD operations work
- [ ] Validation and error display
- [ ] Dialog form reusable
- [ ] Toast feedback on actions
