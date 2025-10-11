# Day 09 — Fetching Data

## Objectives
- Fetch API and axios
- Loading states
- Error handling

## Example
```tsx
function UsersList() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch('https://jsonplaceholder.typicode.com/users')
      .then(res => res.json())
      .then(data => {
        setUsers(data);
        setLoading(false);
      })
      .catch(err => {
        setError('Failed to fetch users');
        setLoading(false);
      });
  }, []);

  if (loading) return <ProgressSpinner />;
  if (error) return <Message severity="error" text={error} />;

  return (
    <DataTable value={users}>
      <Column field="name" header="Name" />
      <Column field="email" header="Email" />
      <Column field="phone" header="Phone" />
    </DataTable>
  );
}
```

## Exercise
- Build a posts list with comments using JSONPlaceholder API

## Checklist
- [ ] Data fetched successfully
- [ ] Loading spinner shows
- [ ] Errors handled gracefully