# Day 24 — Tree & TreeTable

## Objectives
- Hierarchical data display with Tree
- TreeTable for tabular tree data
- Node selection and expansion

## Tree Component
```tsx
function TreeDemo() {
  const [nodes, setNodes] = useState(null);
  const [selectedKeys, setSelectedKeys] = useState(null);

  useEffect(() => {
    const treeNodes = [
      {
        key: '0',
        label: 'Documents',
        data: 'Documents Folder',
        icon: 'pi pi-fw pi-inbox',
        children: [
          {
            key: '0-0',
            label: 'Work',
            data: 'Work Folder',
            icon: 'pi pi-fw pi-cog',
            children: [
              { key: '0-0-0', label: 'Expenses.doc', icon: 'pi pi-fw pi-file', data: 'Expenses Document' },
              { key: '0-0-1', label: 'Resume.doc', icon: 'pi pi-fw pi-file', data: 'Resume Document' }
            ]
          },
          {
            key: '0-1',
            label: 'Home',
            data: 'Home Folder',
            icon: 'pi pi-fw pi-home',
            children: [{ key: '0-1-0', label: 'Invoices.txt', icon: 'pi pi-fw pi-file', data: 'Invoices for this month' }]
          }
        ]
      },
      {
        key: '1',
        label: 'Events',
        data: 'Events Folder',
        icon: 'pi pi-fw pi-calendar',
        children: [
          { key: '1-0', label: 'Meeting', icon: 'pi pi-fw pi-calendar-plus', data: 'Meeting' },
          { key: '1-1', label: 'Product Launch', icon: 'pi pi-fw pi-calendar-plus', data: 'Product Launch' },
          { key: '1-2', label: 'Report Review', icon: 'pi pi-fw pi-calendar-plus', data: 'Report Review' }
        ]
      }
    ];
    setNodes(treeNodes);
  }, []);

  return (
    <Tree 
      value={nodes} 
      selectionMode="checkbox" 
      selectionKeys={selectedKeys} 
      onSelectionChange={(e) => setSelectedKeys(e.value)} 
      className="w-full md:w-30rem"
    />
  );
}
```

## TreeTable Component
```tsx
function TreeTableDemo() {
  const [nodes, setNodes] = useState([]);
  const [selectedNodeKey, setSelectedNodeKey] = useState(null);
  const [expandedKeys, setExpandedKeys] = useState({});

  useEffect(() => {
    const treeTableNodes = [
      {
        key: '0',
        data: {
          name: 'Applications',
          size: '100kb',
          type: 'Folder'
        },
        children: [
          {
            key: '0-0',
            data: {
              name: 'React',
              size: '25kb',
              type: 'Folder'
            },
            children: [
              {
                key: '0-0-0',
                data: {
                  name: 'react.app',
                  size: '10kb',
                  type: 'Application'
                }
              },
              {
                key: '0-0-1',
                data: {
                  name: 'native.app',
                  size: '10kb',
                  type: 'Application'
                }
              }
            ]
          },
          {
            key: '0-1',
            data: {
              name: 'editor.app',
              size: '25kb',
              type: 'Application'
            }
          }
        ]
      },
      {
        key: '1',
        data: {
          name: 'Cloud',
          size: '20kb',
          type: 'Folder'
        },
        children: [
          {
            key: '1-0',
            data: {
              name: 'backup-1.zip',
              size: '10kb',
              type: 'Zip'
            }
          },
          {
            key: '1-1',
            data: {
              name: 'backup-2.zip',
              size: '10kb',
              type: 'Zip'
            }
          }
        ]
      }
    ];
    setNodes(treeTableNodes);
  }, []);

  const toggleApplications = () => {
    let _expandedKeys = { ...expandedKeys };
    if (_expandedKeys['0']) delete _expandedKeys['0'];
    else _expandedKeys['0'] = true;
    setExpandedKeys(_expandedKeys);
  };

  return (
    <div>
      <div className="flex justify-content-end mb-4">
        <Button onClick={toggleApplications} label="Toggle Applications" />
      </div>
      <TreeTable 
        value={nodes} 
        expandedKeys={expandedKeys} 
        onToggle={(e) => setExpandedKeys(e.value)}
        selectionMode="single" 
        selectionKeys={selectedNodeKey} 
        onSelectionChange={(e) => setSelectedNodeKey(e.value)}
        tableStyle={{ minWidth: '50rem' }}
      >
        <Column field="name" header="Name" expander></Column>
        <Column field="size" header="Size"></Column>
        <Column field="type" header="Type"></Column>
      </TreeTable>
    </div>
  );
}
```

## Custom Node Template
```tsx
const nodeTemplate = (node, options) => {
  let label = <b>{node.label}</b>;

  if (node.url) {
    label = <a href={node.url}>{node.label}</a>;
  }

  return (
    <span className={options.className}>
      {label}
      {node.children && node.children.length > 0 && (
        <span className="ml-2">
          <Badge value={node.children.length} />
        </span>
      )}
    </span>
  );
};
```

## Exercise
- Create a file system browser with Tree
- Build an organizational chart with TreeTable
- Add context menu for tree operations

## Checklist
- [ ] Tree displays hierarchical data
- [ ] TreeTable shows tabular tree structure
- [ ] Node selection and expansion work
- [ ] Custom templates render correctly