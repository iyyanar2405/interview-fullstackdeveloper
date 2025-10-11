# Day 50 — Admin Store

## Objectives
- Centralize admin data with a lightweight store (Context or Zustand)
- Handle async fetch/update with loading and error states
- Share state across admin pages

## Zustand Store
```tsx
import create from 'zustand';

type AdminState = {
  users: User[];
  loading: boolean;
  error?: string;
  fetchUsers: () => Promise<void>;
};

export const useAdminStore = create<AdminState>((set) => ({
  users: [],
  loading: false,
  error: undefined,
  fetchUsers: async () => {
    try {
      set({ loading: true, error: undefined });
      const data = await api.list();
      set({ users: data, loading: false });
    } catch (e: any) {
      set({ error: 'Failed to load users', loading: false });
    }
  }
}));
```

## Usage
```tsx
function UsersPage() {
  const { users, loading, error, fetchUsers } = useAdminStore();
  useEffect(() => { fetchUsers(); }, [fetchUsers]);
  if (loading) return <Skeleton height="2rem" />;
  if (error) return <Message severity="error" text={error} />;
  return <DataTable value={users}>...</DataTable>;
}
```

## Checklist
- [ ] Store exposes typed state
- [ ] Async actions set loading/error
- [ ] Components subscribe to slices
- [ ] No prop drilling across admin