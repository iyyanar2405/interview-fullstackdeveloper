# React + PrimeReact — 60-Day Learning Path 🚀

From beginner to expert: a complete, practical journey to master React with PrimeReact. Each day includes goals, theory, examples, exercises, and a checklist.

## Course Structure

- Phase 1 (Days 01–10): React Fundamentals
- Phase 2 (Days 11–20): PrimeReact Basics
- Phase 3 (Days 21–30): Data & Advanced Components
- Phase 4 (Days 31–40): Forms, Validation, and Overlays
- Phase 5 (Days 41–50): Real-World Apps
- Phase 6 (Days 51–60): Expert Topics (State, Performance, Testing, Deploy)

## Prerequisites

- Node.js LTS
- Basic JavaScript/TypeScript
- VS Code recommended

## Setup (PowerShell)

```powershell
# Create a new Vite + React + TS app (recommended for speed)
npm create vite@latest primereact-app -- --template react-ts
cd primereact-app

# Install PrimeReact, PrimeIcons, and theme
npm install primereact primeicons

# Install styling peer dependencies (optional)
npm install classnames

# Run
npm install
npm run dev
```

Add theme imports to `src/main.tsx`:

```ts
import 'primereact/resources/themes/lara-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import './index.css';
```

## Learning Path (Status)

| Phase | Days | Status |
|------|------|--------|
| Fundamentals | 01–10 | ✅ |
| PrimeReact Basics | 11–20 | ✅ |
| Advanced Components | 21–30 | ✅ |
| Forms & Overlays | 31–40 | ✅ |
| Real-World Apps | 41–50 | ✅ |
| Expert | 51–60 | ✅ |

## How to Use

- Follow days in order
- Code along every example
- Build the mini-projects and exercises
- Review and refactor regularly

## Quick Links

- ROADMAP.md — details for all days
- PROGRESS-REPORT.md — track completion
- MISSING-DAYS-SUMMARY.md — remaining items

Happy Learning! 🎉
