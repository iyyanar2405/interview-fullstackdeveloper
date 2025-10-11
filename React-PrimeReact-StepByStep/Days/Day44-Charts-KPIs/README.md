# Day 44 — Charts & KPIs

## Objectives
- Add KPIs and charts using Chart.js via PrimeReact Chart
- Create responsive dashboard grid
- Load/transform data for visualizations

## KPI Cards
```tsx
function KPI({ title, value, icon, trend }: { title: string; value: string; icon: string; trend?: 'up' | 'down' }) {
  return (
    <div className="p-3 border-1 surface-border border-round flex justify-content-between align-items-center">
      <div>
        <div className="text-600 text-sm">{title}</div>
        <div className="text-2xl font-bold">{value}</div>
      </div>
      <i className={`${icon} text-2xl ${trend === 'up' ? 'text-green-500' : trend === 'down' ? 'text-red-500' : ''}`}></i>
    </div>
  );
}
```

## Chart Examples
```tsx
function SalesChart() {
  const data = {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
    datasets: [{ label: 'Sales', data: [12, 19, 3, 5, 2, 3], backgroundColor: '#42A5F5' }]
  };
  const options = { plugins: { legend: { position: 'bottom' } }, maintainAspectRatio: false } as any;
  return <Chart type="bar" data={data} options={options} height="300px" />;
}

function LineVisitors() {
  const data = {
    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    datasets: [{ label: 'Visitors', data: [130, 160, 120, 180, 220, 190, 170], borderColor: '#66BB6A' }]
  };
  return <Chart type="line" data={data} height="300px" />;
}
```

## Dashboard Grid
```tsx
export default function DashboardPage() {
  return (
    <div className="grid">
      <div className="col-12 md:col-3"><KPI title="Revenue" value="$23k" icon="pi pi-dollar" trend="up" /></div>
      <div className="col-12 md:col-3"><KPI title="Orders" value="1.2k" icon="pi pi-shopping-cart" /></div>
      <div className="col-12 md:col-3"><KPI title="Users" value="8.1k" icon="pi pi-users" /></div>
      <div className="col-12 md:col-3"><KPI title="Refunds" value="21" icon="pi pi-undo" trend="down" /></div>

      <div className="col-12 md:col-8"><div className="p-3 border-1 surface-border border-round"><SalesChart /></div></div>
      <div className="col-12 md:col-4"><div className="p-3 border-1 surface-border border-round"><LineVisitors /></div></div>
    </div>
  );
}
```

## Checklist
- [ ] KPI grid responsive
- [ ] Charts render and resize
- [ ] Data mapping is typed
- [ ] Empty/loading states handled