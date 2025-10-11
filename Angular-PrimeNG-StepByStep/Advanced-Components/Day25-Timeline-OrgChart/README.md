# Day 25: Timeline and OrganizationChart - Visual Storytelling

Today you’ll build two highly visual components: Timeline for sequences/history and OrganizationChart for hierarchical org structures. These are perfect for project roadmaps, order tracking, and company org views.

## What you’ll build
- Order tracking timeline with statuses, icons, and custom templates
- Project roadmap timeline (horizontal)
- Organization chart with people cards, expand/collapse, and actions

## Prerequisites
Install modules: `TimelineModule`, `OrganizationChartModule`, `CardModule`, `AvatarModule`, `TagModule`, `ButtonModule`.

## 1) Timeline basics: Order tracking

```ts
// order-timeline.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimelineModule } from 'primeng/timeline';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';

interface OrderEvent {
  status: 'Ordered' | 'Processing' | 'Shipped' | 'Delivered' | 'Returned';
  date: string;
  icon?: string;
  color?: string;
  detail?: string;
}

@Component({
  selector: 'app-order-timeline',
  standalone: true,
  imports: [CommonModule, TimelineModule, CardModule, TagModule],
  templateUrl: './order-timeline.component.html',
  styleUrls: ['./order-timeline.component.scss']
})
export class OrderTimelineComponent {
  events: OrderEvent[] = [
    { status: 'Ordered', date: '2025-10-01 10:30', icon: 'pi pi-shopping-cart', color: '#0ea5e9', detail: 'Order #12345 placed' },
    { status: 'Processing', date: '2025-10-01 12:00', icon: 'pi pi-cog', color: '#f59e0b', detail: 'Payment confirmed' },
    { status: 'Shipped', date: '2025-10-02 09:15', icon: 'pi pi-truck', color: '#6366f1', detail: 'Left warehouse' },
    { status: 'Delivered', date: '2025-10-03 16:45', icon: 'pi pi-check', color: '#10b981', detail: 'Signed by John' }
  ];

  severity(status: OrderEvent['status']) {
    switch (status) {
      case 'Delivered': return 'success';
      case 'Shipped': return 'info';
      case 'Processing': return 'warning';
      case 'Returned': return 'danger';
      default: return 'info';
    }
  }
}
```

```html
<!-- order-timeline.component.html -->
<div class="card">
  <h3>Order #12345</h3>
  <p-timeline [value]="events">
    <ng-template pTemplate="marker" let-event>
      <span class="custom-marker" [style.backgroundColor]="event.color">
        <i [class]="event.icon"></i>
      </span>
    </ng-template>
    <ng-template pTemplate="content" let-event>
      <p-card>
        <div class="row">
          <div class="left">
            <p-tag [value]="event.status" [severity]="severity(event.status)"></p-tag>
            <div class="date">{{event.date | date:'medium'}}</div>
          </div>
          <div class="detail">{{event.detail}}</div>
        </div>
      </p-card>
    </ng-template>
  </p-timeline>
</div>
```

```scss
/* order-timeline.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.custom-marker { display:flex; align-items:center; justify-content:center; width:2rem; height:2rem; border-radius:50%; color:#fff; }
.row { display:flex; justify-content:space-between; align-items:center; gap:1rem; }
.left { display:flex; align-items:center; gap:.5rem; }
.date { color:#64748b; font-size:.875rem; }
```

### Horizontal timeline (project roadmap)

```html
<p-timeline [value]="events" layout="horizontal" align="top">
  <ng-template pTemplate="marker" let-event>
    <span class="custom-marker" [style.backgroundColor]="event.color">
      <i [class]="event.icon"></i>
    </span>
  </ng-template>
  <ng-template pTemplate="content" let-event>
    <p-card>
      <div class="title">{{event.status}}</div>
      <div class="date">{{event.date | date:'MMM d'}}</div>
    </p-card>
  </ng-template>
</p-timeline>
```

## 2) OrganizationChart: People hierarchy

```ts
// company-orgchart.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrganizationChartModule } from 'primeng/organizationchart';
import { CardModule } from 'primeng/card';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';

interface PersonNode {
  label: string;
  type: string;
  styleClass?: string;
  expanded?: boolean;
  data?: {
    name: string;
    title: string;
    avatar: string;
  };
  children?: PersonNode[];
}

@Component({
  selector: 'app-company-orgchart',
  standalone: true,
  imports: [CommonModule, OrganizationChartModule, CardModule, AvatarModule, ButtonModule],
  templateUrl: './company-orgchart.component.html',
  styleUrls: ['./company-orgchart.component.scss']
})
export class CompanyOrgChartComponent {
  data: PersonNode = {
    label: 'CEO', type: 'person', expanded: true,
    data: { name: 'Alex Morgan', title: 'Chief Executive Officer', avatar: 'https://i.pravatar.cc/100?img=12' },
    children: [
      {
        label: 'CTO', type: 'person', expanded: true,
        data: { name: 'Priya Patel', title: 'Chief Technology Officer', avatar: 'https://i.pravatar.cc/100?img=32' },
        children: [
          { label: 'FE Lead', type: 'person', data: { name: 'John Chen', title: 'Frontend Lead', avatar: 'https://i.pravatar.cc/100?img=18' } },
          { label: 'BE Lead', type: 'person', data: { name: 'Sara Park', title: 'Backend Lead', avatar: 'https://i.pravatar.cc/100?img=47' } }
        ]
      },
      {
        label: 'CFO', type: 'person',
        data: { name: 'Liam Scott', title: 'Chief Financial Officer', avatar: 'https://i.pravatar.cc/100?img=5' }
      }
    ]
  } as PersonNode;
}
```

```html
<!-- company-orgchart.component.html -->
<div class="card">
  <h3>Company Org Structure</h3>
  <p-organizationChart [value]="data">
    <ng-template let-node pTemplate="person">
      <div class="person">
        <p-avatar [image]="node.data?.avatar" shape="circle" size="large"></p-avatar>
        <div class="info">
          <div class="name">{{node.data?.name}}</div>
          <div class="title">{{node.data?.title}}</div>
        </div>
      </div>
    </ng-template>
  </p-organizationChart>
</div>
```

```scss
/* company-orgchart.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.person { display:flex; align-items:center; gap:.75rem; padding:.5rem 1rem; border-radius:8px; background:#f8fafc; }
.info .name { font-weight:600; }
.info .title { color:#64748b; font-size:.875rem; }
```

## Tips
- Use `layout="horizontal"` for timelines when you have few events
- For OrganizationChart, template by node type and keep nodes minimal for performance
- Make nodes clickable to open side panels with details

## Exercises
- Add status-based marker colors and icons to a release roadmap
- Add actions (email, call, view profile) to org chart nodes
- Add search to focus/expand matching org chart nodes

## Summary
You created engaging visual components for timelines and org structures. Next: Day 26 - Carousel and Galleria for rich media browsing.