# 07 — Forms

Work with Template-driven and Reactive forms, plus validation.

## Template-driven
```ts
@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <form #f="ngForm" (ngSubmit)="submit(f)">
      <input name="email" ngModel required email />
      <input name="password" ngModel required minlength="6" type="password" />
      <button [disabled]="f.invalid">Login</button>
    </form>
  `
})
export class LoginComponent {
  submit(f: any){ console.log(f.value); }
}
```

## Reactive forms
```ts
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="save()">
      <input formControlName="name" />
      <button [disabled]="form.invalid">Save</button>
    </form>
  `
})
export class ProfileComponent {
  fb = inject(FormBuilder);
  form = this.fb.group({ name: ['', Validators.required] });
  save(){ console.log(this.form.value); }
}
```

Next: HTTP and interceptors.
