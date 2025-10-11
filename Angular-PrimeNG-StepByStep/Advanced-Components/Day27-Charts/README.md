# Day 27: Charts Integration (Chart.js + PrimeNG)

Today you’ll integrate interactive charts using PrimeNG’s Chart component powered by Chart.js v4. You’ll build line, bar, pie/doughnut, and combo charts, update data dynamically, theme charts, and handle responsive layouts.

Note: Ensure Chart.js is installed in your Angular workspace.

Optional install steps:
- npm i chart.js

## What you’ll build
- Sales analytics dashboard with line + bar charts
- Category breakdown with pie/doughnut
- Live-updating chart and custom tooltips/legends

## Setup

```ts
// Import ChartModule where needed (NgModule) or in a standalone component imports
import { ChartModule } from 'primeng/chart';
```

## 1) Basic Line Chart

```ts
// sales-line-chart.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-sales-line-chart',
  standalone: true,
  imports: [CommonModule, ChartModule],
  template: `<p-chart type="line" [data]="data" [options]="options"></p-chart>`
})
export class SalesLineChartComponent implements OnInit {
  data: any;
  options: any;

  ngOnInit() {
    this.data = {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
      datasets: [
        {
          label: 'Revenue',
          data: [12000, 15000, 18000, 16000, 19000, 22000, 25000],
          fill: false,
          borderColor: '#6366f1',
          tension: 0.4
        },
        {
          label: 'Costs',
          data: [9000, 10000, 11000, 13000, 12000, 14000, 15000],
          fill: false,
          borderColor: '#f59e0b',
          tension: 0.4
        }
      ]
    };

    this.options = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom' },
        tooltip: {
          callbacks: {
            label: (ctx: any) => `${ctx.dataset.label}: $${ctx.parsed.y.toLocaleString()}`
          }
        }
      },
      scales: {
        y: {
          ticks: {
            callback: (value: number) => `$${value / 1000}k`
          }
        }
      }
    };
  }
}
```

```html
<!-- usage in a parent -->
<div class="card" style="height: 360px">
  <app-sales-line-chart />
</div>
```

## 2) Bar + Line Combo Chart

```ts
// combo-chart.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-combo-chart',
  standalone: true,
  imports: [CommonModule, ChartModule],
  template: `<p-chart type="bar" [data]="data" [options]="options"></p-chart>`
})
export class ComboChartComponent implements OnInit {
  data: any;
  options: any;

  ngOnInit() {
    this.data = {
      labels: ['Q1', 'Q2', 'Q3', 'Q4'],
      datasets: [
        {
          type: 'bar',
          label: 'Orders',
          backgroundColor: '#10b981',
          data: [320, 450, 380, 520]
        },
        {
          type: 'line',
          label: 'Revenue',
          borderColor: '#6366f1',
          borderWidth: 2,
          fill: false,
          data: [120000, 165000, 140000, 210000]
        }
      ]
    };

    this.options = {
      responsive: true,
      plugins: {
        legend: { position: 'bottom' }
      },
      scales: {
        y: { beginAtZero: true }
      }
    };
  }
}
```

## 3) Pie / Doughnut Chart

```ts
// category-pie.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-category-pie',
  standalone: true,
  imports: [CommonModule, ChartModule],
  template: `<p-chart type="doughnut" [data]="data" [options]="options"></p-chart>`
})
export class CategoryPieComponent implements OnInit {
  data: any;
  options: any;

  ngOnInit() {
    const docStyle = getComputedStyle(document.documentElement);
    const colors = ['#6366f1','#22c55e','#f59e0b','#ef4444','#06b6d4'];

    this.data = {
      labels: ['Electronics', 'Clothing', 'Fitness', 'Home', 'Other'],
      datasets: [{
        data: [35, 20, 15, 22, 8],
        backgroundColor: colors,
        hoverBackgroundColor: colors.map(c => c + 'cc')
      }]
    };

    this.options = {
      cutout: '60%',
      plugins: {
        legend: { position: 'right' },
        tooltip: {
          callbacks: {
            label: (ctx: any) => `${ctx.label}: ${ctx.parsed}%`
          }
        }
      }
    };
  }
}
```

## 4) Live Updates

```ts
// live-chart.component.ts
import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-live-chart',
  standalone: true,
  imports: [CommonModule, ChartModule],
  template: `<p-chart type="line" [data]="data" [options]="options"></p-chart>`
})
export class LiveChartComponent implements OnInit, OnDestroy {
  data: any;
  options: any = { animation: false };
  timer: any;

  ngOnInit() {
    this.data = {
      labels: Array.from({length: 20}, (_, i) => i.toString()),
      datasets: [{ label: 'Active Users', data: Array(20).fill(0), borderColor: '#06b6d4', fill: false }]
    };
    this.timer = setInterval(() => this.tick(), 1500);
  }

  tick() {
    const ds = this.data.datasets[0].data;
    ds.shift();
    ds.push(Math.floor(Math.random()*200)+20);
    this.data = { ...this.data }; // trigger change detection
  }

  ngOnDestroy() { clearInterval(this.timer); }
}
```

## 5) Theming and Global Options

- PrimeNG themes apply to chart legends and fonts; chart colors come from dataset configs.
- You can set Chart.js defaults globally:

```ts
import { Chart as ChartJS } from 'chart.js';
ChartJS.defaults.font.family = 'Inter, system-ui, Segoe UI, Roboto, Arial';
ChartJS.defaults.color = '#334155';
```

## 6) Dashboard Layout Example

```html
<div class="grid">
  <div class="col-12 lg:col-8 card" style="height:360px"><app-combo-chart /></div>
  <div class="col-12 lg:col-4 card" style="height:360px"><app-category-pie /></div>
  <div class="col-12 card" style="height:300px"><app-live-chart /></div>
</div>
```

## Tips
- Keep datasets small for readability; paginate data if needed
- Use `maintainAspectRatio=false` and fixed container height for responsive dashboards
- Memoize heavy calculations and reuse chart options objects

## Exercises
- Add a stacked bar chart for monthly revenue vs cost
- Add custom tooltip that shows delta vs previous month
- Implement a theme switcher that updates chart colors

## Summary
You can now create interactive, themed charts using PrimeNG with Chart.js. Next: Day 28 - FileUpload and Image components for handling files.