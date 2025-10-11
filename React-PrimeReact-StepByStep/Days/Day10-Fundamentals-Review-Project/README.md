# Day 10 — Fundamentals Review Project

## Objectives
- Combine all learned concepts
- Build a mini task manager app
- Review and refactor

## Project Requirements
- Task CRUD operations
- Categories with Context API
- Routing between views
- Data persistence (localStorage)
- PrimeReact components throughout

## Components to Build
- TaskList (DataTable)
- TaskForm (Dialog)
- CategoryManager
- TaskFilters (Dropdown, Calendar)

## Exercise Structure
```
src/
  components/
    TaskList.tsx
    TaskForm.tsx
    CategoryFilter.tsx
  context/
    TaskContext.tsx
    CategoryContext.tsx
  pages/
    Dashboard.tsx
    Categories.tsx
  types/
    Task.ts
    Category.ts
```

## Features
- Add/edit/delete tasks
- Filter by category and date
- Mark tasks complete
- Responsive layout

## Checklist
- [ ] All CRUD operations work
- [ ] Context manages global state
- [ ] Router navigation works
- [ ] Data persists in localStorage
- [ ] PrimeReact components styled properly