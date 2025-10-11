# Day 60 — Final Capstone Project

Build and ship a polished application that demonstrates your Angular + PrimeNG expertise. Choose one of the scenarios or adapt to your domain.

## Project options

1) CRM Mini Suite
- Contacts, deals, pipeline stages
- Kanban board (drag & drop), charts, and reporting

2) Inventory & Orders
- Product catalog, stock levels, purchase orders
- Supplier management and analytics

3) Project Management
- Tasks, sprints, burndown charts, team board
- Gantt-like timeline and resource overview

## Requirements (acceptance criteria)

- Authentication flow (mocked or real)
- Responsive layout with top nav + sidebar
- At least 8 PrimeNG components including Table, Dialog, Toast, Dropdown, Calendar, Chart, FileUpload, and Menus
- Forms with validation (template and reactive where appropriate)
- NgRx for at least one domain slice
- Theming: custom brand variables + dark mode toggle
- Accessibility: keyboard nav, labels, focus, and contrast checks
- Performance: OnPush components, trackBy, and virtual scrolling for large lists
- Routing: lazy-loaded feature(s) with preloading
- Testing: 5+ meaningful unit tests for UI and services
- Deployment: production build with correct base path

## Suggested structure

```
src/app/
  core/ (services, interceptors)
  shared/ (ui atoms, directives, pipes)
  state/ (app-level selectors)
  features/
    <domain>/
      store/ (ngrx)
      pages/ (containers)
      components/ (presentational)
```

## Milestones

- Day 60 AM: Scope, data models, and routes
- Day 60 Midday: Core features and pages
- Day 60 PM: Theming, A11y, tests, polish

## Quality checklist

- [ ] Lint and unit tests pass
- [ ] Lighthouse > 90 for Accessibility and Performance
- [ ] No console errors and error states handled
- [ ] Mobile viewport verified

## Submission

- README with setup/run instructions and screenshots
- Note design decisions and trade-offs

## Congratulations

You’ve completed the 60-day Angular + PrimeNG learning path. Keep improving by building real-world features, contributing to docs and examples, and mentoring others.
