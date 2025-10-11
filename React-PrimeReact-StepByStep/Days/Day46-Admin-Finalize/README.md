# Day 46 — Admin Finalize

## Objectives
- Polish admin UX: loading states, empty states, errors
- Add route-level code splitting for admin pages
- Add basic role-based access control (RBAC)

## Loading and Empty States
```tsx
function EmptyState({ title, action }: { title: string; action?: React.ReactNode }) {
  return (
    <div className="text-center p-5 border-1 surface-border border-round">
      <i className="pi pi-database text-4xl text-500" />
      <div className="mt-3 text-900 text-xl">{title}</div>
      {action && <div className="mt-3">{action}</div>}
    </div>
  );
}
```

## Code Splitting
```tsx
const UsersPage = React.lazy(() => import('./UsersPage'));
const SettingsPage = React.lazy(() => import('./SettingsPage'));

export const routes = [
  { path: '/admin/users', element: <Suspense fallback={<Skeleton height="2rem" />}> <UsersPage /> </Suspense> },
  { path: '/admin/settings', element: <Suspense fallback={<Skeleton height="2rem" />}> <SettingsPage /> </Suspense> }
];
```

## RBAC Guard
```tsx
type Role = 'guest' | 'user' | 'admin';

function RequireRole({ role, children }: { role: Role; children: React.ReactNode }) {
  const current: Role = 'admin'; // get from auth store
  if (role === 'admin' && current !== 'admin') return <Navigate to="/" replace />;
  return <>{children}</>;
}
```

## Checklist
- [ ] Empty/loading states available
- [ ] Admin pages lazy-loaded
- [ ] RBAC redirects unauthorized
- [ ] Errors surfaced via Toast