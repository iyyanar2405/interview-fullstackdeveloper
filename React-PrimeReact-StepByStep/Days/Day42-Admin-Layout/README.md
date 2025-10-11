# Day 42 — Admin Layout

## Objectives
- Design responsive admin layout using PrimeFlex
- Create reusable Topbar and Sidebar components
- Implement dark/light theme toggle

## Components
```tsx
export function Topbar({ onMenu }: { onMenu: () => void }) {
  return (
    <div className="flex align-items-center justify-content-between px-4 py-3 border-bottom-1 surface-border">
      <div className="flex align-items-center gap-2">
        <Button icon="pi pi-bars" text onClick={onMenu} />
        <span className="font-bold">Admin</span>
      </div>
      <div className="flex align-items-center gap-2">
        <Button icon="pi pi-bell" text />
        <Button icon="pi pi-user" text />
      </div>
    </div>
  );
}

export function SidebarMenu({ visible, onHide }: { visible: boolean; onHide: () => void }) {
  return (
    <Sidebar visible={visible} onHide={onHide} dismissable position="left">
      <nav className="flex flex-column gap-2">
        <Link to="/admin">Dashboard</Link>
        <Link to="/admin/users">Users</Link>
        <Link to="/admin/settings">Settings</Link>
      </nav>
    </Sidebar>
  );
}
```

## Theme Toggle
```tsx
function ThemeToggle() {
  const [dark, setDark] = useState(false);
  useEffect(() => {
    document.documentElement.classList.toggle('p-darktheme', dark);
  }, [dark]);

  return (
    <div className="flex align-items-center gap-2">
      <span>Dark</span>
      <InputSwitch checked={dark} onChange={(e) => setDark(e.value)} />
    </div>
  );
}
```

## Exercises
- Persist theme preference to localStorage
- Add skeletons to dashboard widgets while loading
- Add a footer with version and links

## Checklist
- [ ] Topbar and sidebar reusable
- [ ] Layout responsive on mobile
- [ ] Theme toggle works
- [ ] ARIA roles and labels applied