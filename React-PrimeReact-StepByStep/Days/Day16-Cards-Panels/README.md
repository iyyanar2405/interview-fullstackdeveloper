# Day 16 — Cards & Panels

## Objectives
- Container components for layout
- Card compositions and styling
- Panel variations and features

## Card Component
```tsx
function CardDemo() {
  const header = (
    <img alt="Card" src="https://primefaces.org/cdn/primereact/images/usercard.png" />
  );
  
  const footer = (
    <div className="p-d-flex p-flex-wrap p-jc-end">
      <Button label="Save" icon="pi pi-check" className="p-mr-2" />
      <Button label="Cancel" icon="pi pi-times" className="p-button-secondary" />
    </div>
  );

  return (
    <Card
      title="Advanced Card"
      subTitle="Subtitle"
      header={header}
      footer={footer}
      className="p-mb-2"
    >
      <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit.</p>
    </Card>
  );
}
```

## Panel Component
```tsx
function PanelDemo() {
  const [collapsed, setCollapsed] = useState(false);

  const template = (options) => {
    const toggleIcon = options.collapsed ? 'pi pi-chevron-down' : 'pi pi-chevron-up';
    const className = `${options.className} p-jc-start`;
    const titleClassName = `${options.titleClassName} p-pl-1`;

    return (
      <div className={className}>
        <button className={options.togglerClassName} onClick={options.onTogglerClick}>
          <span className={toggleIcon}></span>
        </button>
        <span className={titleClassName}>Custom Panel</span>
      </div>
    );
  };

  return (
    <div>
      <Panel header="Regular Panel" className="p-mb-2">
        <p>Lorem ipsum dolor sit amet...</p>
      </Panel>

      <Panel header="Toggleable Panel" toggleable className="p-mb-2">
        <p>Content can be collapsed...</p>
      </Panel>

      <Panel headerTemplate={template} toggleable className="p-mb-2">
        <p>Custom header template...</p>
      </Panel>
    </div>
  );
}
```

## Fieldset
```tsx
<Fieldset legend="Legend Text" toggleable>
  <p>Content goes here...</p>
</Fieldset>
```

## Exercise
- Create a dashboard with multiple cards showing different metrics
- Add collapsible panels for different sections

## Checklist
- [ ] Cards display with headers/footers
- [ ] Panels toggle correctly
- [ ] Custom templates render
- [ ] Responsive layout works