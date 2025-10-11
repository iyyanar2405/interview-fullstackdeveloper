# Day 19 — Menus & Navigation

## Objectives
- Navigation menu components
- Breadcrumb navigation
- Context menus

## Menubar
```tsx
function MenubarDemo() {
  const items = [
    {
      label: 'File',
      icon: 'pi pi-fw pi-file',
      items: [
        {
          label: 'New',
          icon: 'pi pi-fw pi-plus',
          items: [
            {
              label: 'Bookmark',
              icon: 'pi pi-fw pi-bookmark'
            },
            {
              label: 'Video',
              icon: 'pi pi-fw pi-video'
            }
          ]
        },
        {
          label: 'Delete',
          icon: 'pi pi-fw pi-trash'
        },
        {
          separator: true
        },
        {
          label: 'Export',
          icon: 'pi pi-fw pi-external-link'
        }
      ]
    },
    {
      label: 'Edit',
      icon: 'pi pi-fw pi-pencil',
      items: [
        {
          label: 'Left',
          icon: 'pi pi-fw pi-align-left'
        },
        {
          label: 'Right',
          icon: 'pi pi-fw pi-align-right'
        },
        {
          label: 'Center',
          icon: 'pi pi-fw pi-align-center'
        },
        {
          label: 'Justify',
          icon: 'pi pi-fw pi-align-justify'
        }
      ]
    },
    {
      label: 'Users',
      icon: 'pi pi-fw pi-user',
      items: [
        {
          label: 'New',
          icon: 'pi pi-fw pi-user-plus'
        },
        {
          label: 'Delete',
          icon: 'pi pi-fw pi-user-minus'
        },
        {
          label: 'Search',
          icon: 'pi pi-fw pi-users',
          items: [
            {
              label: 'Filter',
              icon: 'pi pi-fw pi-filter',
              items: [
                {
                  label: 'Print',
                  icon: 'pi pi-fw pi-print'
                }
              ]
            },
            {
              icon: 'pi pi-fw pi-bars',
              label: 'List'
            }
          ]
        }
      ]
    },
    {
      label: 'Events',
      icon: 'pi pi-fw pi-calendar',
      items: [
        {
          label: 'Edit',
          icon: 'pi pi-fw pi-pencil',
          items: [
            {
              label: 'Save',
              icon: 'pi pi-fw pi-calendar-plus'
            },
            {
              label: 'Delete',
              icon: 'pi pi-fw pi-calendar-minus'
            }
          ]
        },
        {
          label: 'Archive',
          icon: 'pi pi-fw pi-calendar-times',
          items: [
            {
              label: 'Remove',
              icon: 'pi pi-fw pi-calendar-minus'
            }
          ]
        }
      ]
    },
    {
      label: 'Quit',
      icon: 'pi pi-fw pi-power-off'
    }
  ];

  const start = <img alt="logo" src="showcase/demo/images/logo.png" height="40" className="mr-2"></img>;
  const end = <Button label="Logout" icon="pi pi-power-off" className="p-button-text"></Button>;

  return <Menubar model={items} start={start} end={end} />;
}
```

## TieredMenu
```tsx
function TieredMenuDemo() {
  const items = [
    {
      label: 'Customers',
      icon: 'pi pi-fw pi-table',
      items: [
        {
          label: 'New',
          icon: 'pi pi-fw pi-user-plus'
        },
        {
          label: 'Edit',
          icon: 'pi pi-fw pi-user-edit'
        }
      ]
    },
    {
      label: 'Orders',
      icon: 'pi pi-fw pi-shopping-cart',
      items: [
        {
          label: 'View',
          icon: 'pi pi-fw pi-list'
        },
        {
          label: 'Search',
          icon: 'pi pi-fw pi-search'
        }
      ]
    }
  ];

  return <TieredMenu model={items} />;
}
```

## Breadcrumb
```tsx
function BreadcrumbDemo() {
  const items = [
    { label: 'Computer' },
    { label: 'Notebook' },
    { label: 'Accessories' },
    { label: 'Backpacks' },
    { label: 'Item' }
  ];

  const home = { icon: 'pi pi-home', url: '/' };

  return <BreadCrumb model={items} home={home} />;
}
```

## Steps Navigation
```tsx
function StepsDemo() {
  const [activeIndex, setActiveIndex] = useState(1);
  
  const items = [
    {
      label: 'Personal'
    },
    {
      label: 'Seat'
    },
    {
      label: 'Payment'
    },
    {
      label: 'Confirmation'
    }
  ];

  return (
    <Steps 
      model={items} 
      activeIndex={activeIndex} 
      onSelect={(e) => setActiveIndex(e.index)} 
      readOnly={false} 
    />
  );
}
```

## Exercise
- Create a navigation system for a multi-page app
- Add breadcrumb navigation with React Router integration

## Checklist
- [ ] Menubar with nested items works
- [ ] TieredMenu displays properly
- [ ] Breadcrumb navigation functional
- [ ] Steps component interactive