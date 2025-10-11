# Day 01 — Project Setup (Vite + React + TS)

## Objectives
- Initialize Vite React-TS app
- Install PrimeReact + theme
- Verify dev server

## Steps (PowerShell)
```powershell
npm create vite@latest primereact-app -- --template react-ts
cd primereact-app
npm install
npm install primereact primeicons classnames
npm run dev
```

Import styles in `src/main.tsx`:
```ts
import 'primereact/resources/themes/lara-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
```

## Exercise
- Replace App with a Button and a Card from PrimeReact

## Checklist
- [ ] App created
- [ ] Styles load
- [ ] Button renders
