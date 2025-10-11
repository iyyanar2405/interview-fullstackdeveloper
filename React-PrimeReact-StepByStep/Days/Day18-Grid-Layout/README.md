# Day 18 — Grid & Layout

## Objectives
- PrimeFlex grid system
- Responsive layouts
- Flexbox utilities

## Setup PrimeFlex
```bash
npm install primeflex
```

Add to main.tsx:
```tsx
import 'primeflex/primeflex.css';
```

## Grid System
```tsx
function GridDemo() {
  return (
    <div className="grid">
      <div className="col-12">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">12</div>
      </div>
      <div className="col-6">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">6</div>
      </div>
      <div className="col-6">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">6</div>
      </div>
      <div className="col-4">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">4</div>
      </div>
      <div className="col-4">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">4</div>
      </div>
      <div className="col-4">
        <div className="text-center p-3 border-round-sm bg-primary font-bold">4</div>
      </div>
    </div>
  );
}
```

## Responsive Design
```tsx
function ResponsiveDemo() {
  return (
    <div className="grid">
      <div className="col-12 md:col-6 lg:col-4">
        <Card title="Card 1">
          <p>Responsive card that changes size based on screen size.</p>
        </Card>
      </div>
      <div className="col-12 md:col-6 lg:col-4">
        <Card title="Card 2">
          <p>Another responsive card.</p>
        </Card>
      </div>
      <div className="col-12 md:col-12 lg:col-4">
        <Card title="Card 3">
          <p>Third responsive card.</p>
        </Card>
      </div>
    </div>
  );
}
```

## Flexbox Utilities
```tsx
function FlexDemo() {
  return (
    <div>
      {/* Justify Content */}
      <div className="flex justify-content-between p-3 mb-3 bg-blue-100">
        <div className="bg-blue-500 text-white p-2">Item 1</div>
        <div className="bg-blue-500 text-white p-2">Item 2</div>
        <div className="bg-blue-500 text-white p-2">Item 3</div>
      </div>

      {/* Align Items */}
      <div className="flex align-items-center justify-content-center h-6rem bg-green-100 mb-3">
        <div className="bg-green-500 text-white p-2">Centered</div>
      </div>

      {/* Direction */}
      <div className="flex flex-column gap-2">
        <Button label="Button 1" />
        <Button label="Button 2" />
        <Button label="Button 3" />
      </div>
    </div>
  );
}
```

## Layout Components
```tsx
function LayoutDemo() {
  return (
    <div className="min-h-screen flex flex-column">
      {/* Header */}
      <div className="bg-blue-500 text-white p-3">
        <h1>Header</h1>
      </div>
      
      {/* Main Content */}
      <div className="flex flex-1">
        {/* Sidebar */}
        <div className="bg-gray-200 p-3" style={{ width: '250px' }}>
          <h3>Sidebar</h3>
          <ul className="list-none p-0 m-0">
            <li className="p-2">Menu Item 1</li>
            <li className="p-2">Menu Item 2</li>
            <li className="p-2">Menu Item 3</li>
          </ul>
        </div>
        
        {/* Content */}
        <div className="flex-1 p-3">
          <h2>Main Content</h2>
          <p>This is the main content area.</p>
        </div>
      </div>
      
      {/* Footer */}
      <div className="bg-gray-800 text-white p-3 text-center">
        <p>Footer</p>
      </div>
    </div>
  );
}
```

## Exercise
- Create a responsive product grid
- Build a typical app layout with header, sidebar, content, footer

## Checklist
- [ ] Grid system responsive
- [ ] Flexbox utilities working
- [ ] Layout components positioned correctly
- [ ] Mobile responsive design