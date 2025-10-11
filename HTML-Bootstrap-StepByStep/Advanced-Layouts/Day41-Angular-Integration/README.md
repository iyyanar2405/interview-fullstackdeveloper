# Day 41: Bootstrap + Angular Integration 🅰️

Integrate Bootstrap with Angular using ng-bootstrap.

```typescript
// npm install @ng-bootstrap/ng-bootstrap bootstrap
// app.component.ts
import { Component } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgbModule],
  template: `
    <div class="container py-5">
      <div class="card">
        <div class="card-body">
          <h5 class="card-title">Angular Bootstrap</h5>
          <p class="card-text">Bootstrap widgets for Angular</p>
          <button class="btn btn-primary" (click)="handleClick()">Click Me</button>
        </div>
      </div>
    </div>
  `
})
export class AppComponent {
  handleClick() {
    alert('Button clicked!');
  }
}
```

**Next: Day 42 - Testing Bootstrap Components** 🧪
