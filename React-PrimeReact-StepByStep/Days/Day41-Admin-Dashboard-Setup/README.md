# Day 41 — Admin Dashboard Setup

## Objectives
- Scaffold Admin module pages and routes
- Install PrimeIcons layout helpers and set base layout
- Add topbar, sidebar menu, and content area

## Routes
```tsx
import { createBrowserRouter } from 'react-router-dom';

export const router = createBrowserRouter([
  {
    path: '/admin',
    element: <AdminLayout />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'users', element: <UsersPage /> },
      { path: 'settings', element: <SettingsPage /> }
    ]
  }
]);
```

## Layout Skeleton
```tsx
export default function AdminLayout() {
  return (
    <div className="flex min-h-screen">
      <aside className="surface-100 p-3 w-15rem">
        <div className="text-900 font-bold mb-3">Admin</div>
        <ul className="list-none p-0 m-0">
          <li><Link to="/admin">Dashboard</Link></li>
          <li><Link to="/admin/users">Users</Link></li>
          <li><Link to="/admin/settings">Settings</Link></li>
        </ul>
      </aside>
      <main className="flex-1 p-4">
        <div className="flex align-items-center justify-content-between mb-4">
          <div className="text-2xl font-bold">Admin</div>
          <span className="text-600">v1.0</span>
        </div>
        <Outlet />
      </main>
    </div>
  );
}
```

## Exercises
- Protect /admin routes with an auth guard
- Add breadcrumb trail using react-router location
- Make sidebar collapsible for small screens

## Checklist
- [ ] Admin routes load and nest
- [ ] Layout has sidebar + content
- [ ] Navigation highlights current route
- [ ] Auth guard in place