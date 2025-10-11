# Day 44 — Admin Dashboard: Charts & Analytics

Objectives
- Build a dashboard with KPI cards and charts (line, bar, pie) using PrimeNG Chart (Chart.js).
- Add date range filters and reload charts.
- Implement responsive grid layout.

Components
- Card, Chart, Dropdown, Calendar, Button, Skeleton

Dashboard page
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, CardModule, ChartModule, DropdownModule, CalendarModule, ButtonModule],
  templateUrl: './dashboard.page.html'
})
export class DashboardPage {
  range: Date[] = [];
  interval = 'weekly';
  salesData = { labels: [], datasets: [] as any[] };
  salesOptions = { responsive: true, maintainAspectRatio: false };

  ngOnInit() { this.load(); }
  load() {
    const labels = ['Mon','Tue','Wed','Thu','Fri','Sat','Sun'];
    this.salesData = {
      labels,
      datasets: [
        { label: 'Sales', data: [12,19,3,5,2,3,9], borderColor: '#3B82F6', backgroundColor: '#93C5FD55', fill: true },
        { label: 'Orders', data: [2,5,1,3,2,1,4], type: 'bar', backgroundColor: '#34D399AA' }
      ]
    };
  }
}
```

Template
```html
<h2>Analytics</h2>
<div class="p-grid p-nogutter p-ai-stretch">
  <div class="p-col-12 p-md-6 p-xl-3">
    <p-card header="Revenue" subheader="This week"><div class="kpi">$12,340</div></p-card>
  </div>
  <div class="p-col-12 p-md-6 p-xl-3">
    <p-card header="Orders" subheader="This week"><div class="kpi">267</div></p-card>
  </div>
  <div class="p-col-12 p-md-6 p-xl-3">
    <p-card header="Customers" subheader="New"><div class="kpi">78</div></p-card>
  </div>
  <div class="p-col-12 p-md-6 p-xl-3">
    <p-card header="Refunds" subheader="This week"><div class="kpi">$640</div></p-card>
  </div>

  <div class="p-col-12">
    <div class="p-d-flex p-ai-center p-jc-between p-mb-2">
      <div class="p-d-flex p-ai-center p-gap-2">
        <p-dropdown [options]="[{label:'Weekly',value:'weekly'},{label:'Monthly',value:'monthly'}]" [(ngModel)]="interval"></p-dropdown>
        <p-calendar selectionMode="range" [(ngModel)]="range" dateFormat="yy-mm-dd"></p-calendar>
      </div>
      <button pButton label="Reload" icon="pi pi-refresh" (click)="load()"></button>
    </div>
    <div style="height:360px">
      <p-chart type="line" [data]="salesData" [options]="salesOptions"></p-chart>
    </div>
  </div>
</div>

<style>
.kpi{ font-size:1.5rem; font-weight:700; }
</style>
```

Exercises
1) Add pie chart for category distribution.
2) Add skeletons while loading.
3) Extract a ChartService to format datasets consistently.
