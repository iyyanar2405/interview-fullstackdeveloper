# Day 12: Button & SplitButton Components 🔘

## 📋 Learning Objectives
- Master PrimeNG Button component
- Use different button styles and sizes
- Implement SplitButton for action menus
- Apply icons and badges
- Create button groups

---

## 🔘 Button Component

### Installation
```typescript
import { ButtonModule } from 'primeng/button';

@NgModule({
  imports: [ButtonModule]
})
export class AppModule { }
```

### Basic Buttons
```html
<!-- Basic buttons -->
<p-button label="Primary"></p-button>
<p-button label="Secondary" severity="secondary"></p-button>
<p-button label="Success" severity="success"></p-button>
<p-button label="Info" severity="info"></p-button>
<p-button label="Warning" severity="warning"></p-button>
<p-button label="Danger" severity="danger"></p-button>
<p-button label="Help" severity="help"></p-button>

<!-- With icons -->
<p-button label="Save" icon="pi pi-check"></p-button>
<p-button label="Delete" icon="pi pi-trash" iconPos="right"></p-button>

<!-- Icon only -->
<p-button icon="pi pi-check"></p-button>
<p-button icon="pi pi-search" [rounded]="true"></p-button>

<!-- Outlined -->
<p-button label="Outlined" [outlined]="true"></p-button>

<!-- Text button -->
<p-button label="Text" [text]="true"></p-button>

<!-- Raised -->
<p-button label="Raised" [raised]="true"></p-button>

<!-- Rounded -->
<p-button label="Rounded" [rounded]="true"></p-button>

<!-- Loading state -->
<p-button label="Save" [loading]="loading" (onClick)="save()"></p-button>

<!-- Disabled -->
<p-button label="Disabled" [disabled]="true"></p-button>

<!-- Sizes -->
<p-button label="Small" size="small"></p-button>
<p-button label="Normal"></p-button>
<p-button label="Large" size="large"></p-button>
```

### Button with Badge
```html
<p-button label="Messages" icon="pi pi-envelope" badge="8"></p-button>
<p-button label="Errors" icon="pi pi-times" badge="3" badgeSeverity="danger"></p-button>
```

### Link Buttons
```html
<p-button label="Go to Page" [link]="true" (onClick)="navigate()"></p-button>
```

---

## 🎯 SplitButton Component

### Installation
```typescript
import { SplitButtonModule } from 'primeng/splitbutton';
```

### Basic SplitButton
```typescript
export class ButtonDemoComponent {
  items: MenuItem[] = [
    {
      label: 'Update',
      icon: 'pi pi-refresh',
      command: () => this.update()
    },
    {
      label: 'Delete',
      icon: 'pi pi-times',
      command: () => this.delete()
    },
    {
      separator: true
    },
    {
      label: 'Angular',
      icon: 'pi pi-external-link',
      url: 'http://angular.io'
    }
  ];
  
  save() {
    console.log('Save clicked');
  }
  
  update() {
    console.log('Update clicked');
  }
  
  delete() {
    console.log('Delete clicked');
  }
}
```

```html
<p-splitButton 
  label="Save" 
  icon="pi pi-check"
  (onClick)="save()"
  [model]="items">
</p-splitButton>

<!-- Different severities -->
<p-splitButton label="Success" [model]="items" severity="success"></p-splitButton>
<p-splitButton label="Danger" [model]="items" severity="danger"></p-splitButton>

<!-- Raised -->
<p-splitButton label="Raised" [model]="items" [raised]="true"></p-splitButton>

<!-- Rounded -->
<p-splitButton label="Rounded" [model]="items" [rounded]="true"></p-splitButton>

<!-- Icon only -->
<p-splitButton icon="pi pi-check" [model]="items"></p-splitButton>
```

---

## 🎯 Practical Exercise: Action Toolbar

```typescript
export class ToolbarComponent {
  loading = false;
  
  fileItems: MenuItem[] = [
    { label: 'New', icon: 'pi pi-file', command: () => this.newFile() },
    { label: 'Open', icon: 'pi pi-folder-open', command: () => this.open() },
    { separator: true },
    { label: 'Save', icon: 'pi pi-save', command: () => this.save() },
    { label: 'Save As', icon: 'pi pi-save', command: () => this.saveAs() }
  ];
  
  editItems: MenuItem[] = [
    { label: 'Undo', icon: 'pi pi-undo', command: () => this.undo() },
    { label: 'Redo', icon: 'pi pi-replay', command: () => this.redo() },
    { separator: true },
    { label: 'Cut', icon: 'pi pi-times', command: () => this.cut() },
    { label: 'Copy', icon: 'pi pi-copy', command: () => this.copy() },
    { label: 'Paste', icon: 'pi pi-clone', command: () => this.paste() }
  ];
  
  newFile() { console.log('New file'); }
  open() { console.log('Open'); }
  save() {
    this.loading = true;
    setTimeout(() => this.loading = false, 2000);
  }
  saveAs() { console.log('Save As'); }
  undo() { console.log('Undo'); }
  redo() { console.log('Redo'); }
  cut() { console.log('Cut'); }
  copy() { console.log('Copy'); }
  paste() { console.log('Paste'); }
}
```

```html
<div class="toolbar">
  <div class="toolbar-section">
    <p-splitButton label="File" [model]="fileItems" icon="pi pi-file"></p-splitButton>
    <p-splitButton label="Edit" [model]="editItems" icon="pi pi-pencil"></p-splitButton>
  </div>
  
  <div class="toolbar-section">
    <p-button icon="pi pi-save" [loading]="loading" (onClick)="save()" 
              pTooltip="Save" tooltipPosition="bottom"></p-button>
    <p-button icon="pi pi-undo" (onClick)="undo()" 
              pTooltip="Undo" tooltipPosition="bottom"></p-button>
    <p-button icon="pi pi-replay" (onClick)="redo()" 
              pTooltip="Redo" tooltipPosition="bottom"></p-button>
  </div>
  
  <div class="toolbar-section">
    <p-button icon="pi pi-times" severity="danger" [text]="true" 
              pTooltip="Delete"></p-button>
  </div>
</div>
```

```css
.toolbar {
  display: flex;
  gap: 20px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 4px;
}

.toolbar-section {
  display: flex;
  gap: 10px;
  align-items: center;
}
```

---

✅ **Day 12 Complete!** Tomorrow: Input Components (InputText, InputNumber, Textarea) 📝
