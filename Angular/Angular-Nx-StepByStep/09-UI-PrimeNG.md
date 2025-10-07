# 09 — UI with PrimeNG

Add PrimeNG components and theme.

## Install
```powershell
npm i primeng primeicons
```

## Configure styles (angular.json)
- Include a PrimeNG theme CSS and `primeicons/primeicons.css` in global styles

## Use a component
```ts
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-cta',
  standalone: true,
  imports: [ButtonModule],
  template: `<button pButton type="button" label="Click"></button>`
})
export class CtaComponent {}
```

Refer to PrimeNG docs for themes and layout options.
