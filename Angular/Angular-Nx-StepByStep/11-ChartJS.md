# 11 — Chart.js in Nx Angular

Add Chart.js (via chart.js + ng2-charts) to an Nx Angular app.

## Install
```powershell
npm i chart.js ng2-charts
```

## Standalone chart component
```ts
import { Component } from '@angular/core';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-sales-chart',
  standalone: true,
  imports: [NgChartsModule],
  template: `<canvas baseChart
    [data]="data"
    [type]="'line'"
    [options]="options">
  </canvas>`
})
export class SalesChartComponent {
  data: ChartConfiguration['data'] = {
    labels: ['Jan', 'Feb', 'Mar'],
    datasets: [{ label: 'Sales', data: [10, 20, 15] }]
  };
  options: ChartConfiguration['options'] = { responsive: true };
}
```

## Nx tips
- Place reusable charts in a `shared-ui` lib
- Keep data fetching in `data-access` libs (NgRx Effects or services)
- Use module boundaries to avoid circular deps
