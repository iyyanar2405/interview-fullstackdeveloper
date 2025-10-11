# Day 24: Tree and TreeTable - Hierarchical Data

In this lesson, you’ll learn to display and manage hierarchical data with PrimeNG's Tree and TreeTable components. We’ll cover loading nodes, selection, drag & drop, context menus, and templating for both simple navigation trees and data-rich tree tables.

## What you’ll build
- File Explorer with lazy loading, drag & drop, checkboxes, and context menu
- Organization departments TreeTable with columns, row toggles, and custom templates

## Prerequisites
Install required modules: `TreeModule`, `TreeTableModule`, `ContextMenuModule`, `ButtonModule`, `InputTextModule`, `TagModule`, `ToastModule`.

## 1) Tree basics: File Explorer

```ts
// file-explorer.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeModule } from 'primeng/tree';
import { ContextMenuModule } from 'primeng/contextmenu';
import { MenuItem, MessageService, TreeNode } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-file-explorer',
  standalone: true,
  imports: [CommonModule, TreeModule, ContextMenuModule, ToastModule, ButtonModule],
  providers: [MessageService],
  templateUrl: './file-explorer.component.html',
  styleUrls: ['./file-explorer.component.scss']
})
export class FileExplorerComponent implements OnInit {
  files: TreeNode[] = [];
  selectedNodes: TreeNode[] = [];
  contextMenuItems: MenuItem[] = [];
  loading = true;

  ngOnInit() {
    // Simulate lazy root load
    setTimeout(() => {
      this.files = [
        {
          key: '0',
          label: 'Documents',
          icon: 'pi pi-folder',
          leaf: false, // has children
        },
        {
          key: '1',
          label: 'Pictures',
          icon: 'pi pi-folder',
          leaf: false,
        }
      ];
      this.loading = false;
    }, 600);

    this.contextMenuItems = [
      { label: 'Open', icon: 'pi pi-folder-open', command: () => this.openSelected() },
      { label: 'Rename', icon: 'pi pi-pencil', command: () => this.renameSelected() },
      { separator: true },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteSelected() }
    ];
  }

  loadNode(event: any) {
    if (event.node) {
      // Simulate async child loading
      setTimeout(() => {
        event.node.children = [
          { key: event.node.key + '-0', label: 'Report.docx', icon: 'pi pi-file', leaf: true },
          { key: event.node.key + '-1', label: 'Presentation.pptx', icon: 'pi pi-file', leaf: true },
          { key: event.node.key + '-2', label: 'Archive', icon: 'pi pi-folder', leaf: false }
        ];
      }, 400);
    }
  }

  onNodeSelect(event: any) {}
  onNodeUnselect(event: any) {}

  openSelected() {}
  renameSelected() {}
  deleteSelected() {}
}
```

```html
<!-- file-explorer.component.html -->
<p-toast></p-toast>
<div class="card">
  <div class="header">
    <h3>File Explorer</h3>
    <button pButton icon="pi pi-refresh" label="Refresh"></button>
  </div>

  <p-contextMenu #cm [model]="contextMenuItems"></p-contextMenu>

  <p-tree 
    [value]="files" 
    selectionMode="checkbox" 
    [(selection)]="selectedNodes"
    [contextMenu]="cm"
    (onNodeExpand)="loadNode($event)"
    [lazy]="true"
    [loading]="loading"
    dragdropScope="files">

    <ng-template let-node pTemplate="default">
      <span [class.folder]="!node.leaf" [class.file]="node.leaf">
        <i [class]="node.icon" style="margin-right: .5rem"></i>
        {{node.label}}
      </span>
    </ng-template>
  </p-tree>
</div>
```

```scss
/* file-explorer.component.scss */
.card { padding: 1rem; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,.06); }
.header { display:flex; justify-content:space-between; align-items:center; margin-bottom:.5rem; }
.folder { font-weight:600; }
.file { color:#475569; }
```

### Drag & Drop between nodes
Enable with `draggableNodes` and `droppableNodes` on `<p-tree>` and handle `(onNodeDrop)`.

```html
<p-tree [value]="files" draggableNodes droppableNodes (onNodeDrop)="onDrop($event)"></p-tree>
```

```ts
onDrop(event: any) {
  // event.dragNode, event.dropNode, event.index
}
```

## 2) TreeTable: Departments with metrics

```ts
// department-tree-table.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeTableModule } from 'primeng/treetable';
import { TreeNode } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';

interface DeptNodeData {
  name: string;
  head: string;
  employees: number;
  budget: number; // USD
  status: 'Healthy' | 'Warning' | 'Critical';
}

@Component({
  selector: 'app-department-tree-table',
  standalone: true,
  imports: [CommonModule, TreeTableModule, TagModule, ButtonModule],
  templateUrl: './department-tree-table.component.html',
  styleUrls: ['./department-tree-table.component.scss']
})
export class DepartmentTreeTableComponent implements OnInit {
  nodes: TreeNode<DeptNodeData>[] = [];
  cols = [
    { field: 'name', header: 'Department' },
    { field: 'head', header: 'Head' },
    { field: 'employees', header: 'Employees' },
    { field: 'budget', header: 'Budget' },
    { field: 'status', header: 'Status' },
  ];

  ngOnInit() {
    this.nodes = [
      {
        data: { name: 'Engineering', head: 'Jane Doe', employees: 120, budget: 2500000, status: 'Healthy' },
        children: [
          { data: { name: 'Frontend', head: 'Alex Kim', employees: 30, budget: 600000, status: 'Healthy' } },
          { data: { name: 'Backend', head: 'Sam Lee', employees: 40, budget: 900000, status: 'Warning' } },
          { data: { name: 'QA', head: 'Maria Perez', employees: 20, budget: 300000, status: 'Healthy' } },
        ]
      },
      {
        data: { name: 'Sales', head: 'Robert Fox', employees: 60, budget: 1800000, status: 'Critical' },
        children: [
          { data: { name: 'EMEA', head: 'Laura Li', employees: 25, budget: 700000, status: 'Warning' } },
          { data: { name: 'AMER', head: 'Chris Paul', employees: 20, budget: 600000, status: 'Healthy' } },
        ]
      }
    ];
  }

  severity(status: DeptNodeData['status']) {
    return status === 'Healthy' ? 'success' : status === 'Warning' ? 'warning' : 'danger';
  }
}
```

```html
<!-- department-tree-table.component.html -->
<div class="card">
  <h3>Organization Structure</h3>
  <p-treeTable [value]="nodes" tableStyleClass="p-datatable-sm">
    <ng-template pTemplate="header">
      <tr>
        <th>Department</th>
        <th>Head</th>
        <th>Employees</th>
        <th>Budget</th>
        <th>Status</th>
        <th style="width:6rem">Actions</th>
      </tr>
    </ng-template>

    <ng-template pTemplate="body" let-rowNode let-rowData="rowData">
      <tr>
        <td>
          <p-treeTableToggler [rowNode]="rowNode"></p-treeTableToggler>
          {{rowData.name}}
        </td>
        <td>{{rowData.head}}</td>
        <td>{{rowData.employees}}</td>
        <td>{{rowData.budget | currency:'USD'}}</td>
        <td><p-tag [value]="rowData.status" [severity]="severity(rowData.status)"></p-tag></td>
        <td>
          <button pButton icon="pi pi-pencil" class="p-button-text p-button-rounded"></button>
          <button pButton icon="pi pi-plus" class="p-button-text p-button-rounded"></button>
        </td>
      </tr>
    </ng-template>

    <ng-template pTemplate="emptymessage">
      <tr><td colspan="6" class="text-center">No data</td></tr>
    </ng-template>
  </p-treeTable>
</div>
```

```scss
/* department-tree-table.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.text-center { text-align:center; }
```

## Lazy loading TreeTable
Use `(onNodeExpand)` to fetch children from server only when a node is expanded, setting `rowNode.node.children` accordingly and `rowNode.node.leaf=false`.

## Best practices
- For large trees, prefer lazy loading
- Use stable `key` for nodes to optimize toggling
- Keep selection state in sync on updates
- Debounce search when filtering tree nodes from server

## Exercises
- Add checkbox selection to TreeTable and implement bulk actions
- Implement drag & drop to reorder departments
- Add a toolbar to add/remove departments with confirm dialogs

## Summary
You can now present hierarchical data elegantly using Tree and TreeTable with lazy loading and rich interactions. Next: Day 25 - Timeline and OrganizationChart for visual storytelling of sequences and org structures.