# Day 20 — Overlay Basics (Dialog & Sidebar)

## Objectives
- Modal dialogs and overlays
- Sidebar navigation
- Overlay positioning

## Dialog Component
```tsx
function DialogDemo() {
  const [displayBasic, setDisplayBasic] = useState(false);
  const [displayModal, setDisplayModal] = useState(false);
  const [displayMaximizable, setDisplayMaximizable] = useState(false);
  const [displayPosition, setDisplayPosition] = useState(false);

  const renderFooter = (name) => {
    return (
      <div>
        <Button 
          label="No" 
          icon="pi pi-times" 
          onClick={() => setDisplayBasic(false)} 
          className="p-button-text" 
        />
        <Button 
          label="Yes" 
          icon="pi pi-check" 
          onClick={() => setDisplayBasic(false)} 
          autoFocus 
        />
      </div>
    );
  };

  return (
    <div>
      <Button 
        label="Show" 
        icon="pi pi-external-link" 
        onClick={() => setDisplayBasic(true)} 
      />

      <Dialog 
        header="Header" 
        visible={displayBasic} 
        style={{ width: '50vw' }} 
        footer={renderFooter('displayBasic')}
        onHide={() => setDisplayBasic(false)}
      >
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
          Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
        </p>
      </Dialog>

      {/* Modal Dialog */}
      <Button 
        label="Modal" 
        icon="pi pi-external-link" 
        onClick={() => setDisplayModal(true)} 
      />
      
      <Dialog 
        header="Modal Dialog" 
        visible={displayModal} 
        modal
        style={{ width: '50vw' }} 
        onHide={() => setDisplayModal(false)}
      >
        <p>This is a modal dialog.</p>
      </Dialog>

      {/* Maximizable Dialog */}
      <Button 
        label="Maximizable" 
        icon="pi pi-external-link" 
        onClick={() => setDisplayMaximizable(true)} 
      />
      
      <Dialog 
        header="Maximizable Dialog" 
        visible={displayMaximizable} 
        maximizable
        style={{ width: '50vw' }} 
        onHide={() => setDisplayMaximizable(false)}
      >
        <p>This dialog can be maximized.</p>
      </Dialog>
    </div>
  );
}
```

## Dynamic Dialog
```tsx
function DynamicDialogDemo() {
  const [product, setProduct] = useState(null);
  const dialogRef = useRef(null);

  const showProducts = () => {
    const ref = dialogRef.current.show({
      header: 'Select a Product',
      content: <ProductListDemo onSelect={(product) => {
        setProduct(product);
        ref.close();
      }} />,
      width: '70%',
      contentStyle: { padding: '0', border: '0' },
      onHide: () => ref.destroy()
    });
  };

  return (
    <div>
      <Button label="Select Product" onClick={showProducts} />
      <DynamicDialog ref={dialogRef} />
      {product && <div>Selected: {product.name}</div>}
    </div>
  );
}
```

## Sidebar
```tsx
function SidebarDemo() {
  const [visibleLeft, setVisibleLeft] = useState(false);
  const [visibleRight, setVisibleRight] = useState(false);
  const [visibleTop, setVisibleTop] = useState(false);
  const [visibleBottom, setVisibleBottom] = useState(false);
  const [visibleFullScreen, setVisibleFullScreen] = useState(false);

  return (
    <div>
      <Sidebar visible={visibleLeft} onHide={() => setVisibleLeft(false)}>
        <h3>Left Sidebar</h3>
        <p>Lorem ipsum dolor sit amet...</p>
      </Sidebar>

      <Sidebar 
        visible={visibleRight} 
        position="right" 
        onHide={() => setVisibleRight(false)}
      >
        <h3>Right Sidebar</h3>
        <p>Lorem ipsum dolor sit amet...</p>
      </Sidebar>

      <Sidebar 
        visible={visibleTop} 
        position="top" 
        onHide={() => setVisibleTop(false)}
      >
        <h3>Top Sidebar</h3>
        <p>Lorem ipsum dolor sit amet...</p>
      </Sidebar>

      <div className="p-d-flex p-flex-wrap">
        <Button 
          icon="pi pi-arrow-right" 
          onClick={() => setVisibleLeft(true)} 
          className="p-mr-2 p-mb-2"
        />
        <Button 
          icon="pi pi-arrow-left" 
          onClick={() => setVisibleRight(true)} 
          className="p-mr-2 p-mb-2"
        />
        <Button 
          icon="pi pi-arrow-down" 
          onClick={() => setVisibleTop(true)} 
          className="p-mr-2 p-mb-2"
        />
      </div>
    </div>
  );
}
```

## OverlayPanel
```tsx
function OverlayPanelDemo() {
  const op = useRef(null);

  return (
    <div>
      <Button 
        type="button" 
        icon="pi pi-image" 
        label="Image" 
        onClick={(e) => op.current.toggle(e)} 
      />
      <OverlayPanel ref={op}>
        <img src="demo/images/nature/nature1.jpg" alt="Nature" />
      </OverlayPanel>
    </div>
  );
}
```

## Exercise
- Create a settings dialog with form inputs
- Build a navigation sidebar with menu items
- Add confirmation dialogs for destructive actions

## Checklist
- [ ] Dialog opens and closes properly
- [ ] Modal backdrop prevents interaction
- [ ] Sidebar slides from different positions
- [ ] OverlayPanel positions correctly
- [ ] Focus management works properly