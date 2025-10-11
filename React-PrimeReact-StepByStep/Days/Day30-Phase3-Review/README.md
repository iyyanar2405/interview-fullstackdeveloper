# Day 30 — Phase 3 Review Project

## Objectives
- Consolidate PrimeReact UI fundamentals
- Build a small feature integrating tables, uploads, and overlays

## Mini Project: Product Manager
- Features:
  - Upload product images (Day 27)
  - Virtualized product list for performance (Day 28)
  - Advanced table with row expansion and inline edit (Day 29)
  - Overlay panel to show quick product info (Day 20)

## Suggested Structure
- components/
  - ProductTable.tsx
  - ProductUpload.tsx
  - ProductQuickView.tsx
- services/
  - products.api.ts (mock service)
- pages/
  - ProductsPage.tsx

## Acceptance Criteria
- [ ] Users can upload images for a product
- [ ] Products list renders 1000+ items smoothly
- [ ] Edit product name/price inline and persist in local state
- [ ] Quick view overlay shows image and details

## Extensions
- Persist to localStorage
- Add filtering and sorting controls
- Export table to CSV

## Handoff Checklist
- [ ] README documents setup and commands
- [ ] Components are typed with TS interfaces
- [ ] No lint errors; basic unit test for table edit
