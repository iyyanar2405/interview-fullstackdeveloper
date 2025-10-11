# Day 08 — React Router Basics

## Objectives
- Install and configure React Router
- Route components
- Navigation and links

## Setup
```bash
npm install react-router-dom
```

## Example
```tsx
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

function App() {
  return (
    <BrowserRouter>
      <nav>
        <Link to="/">Home</Link>
        <Link to="/about">About</Link>
        <Link to="/contact">Contact</Link>
      </nav>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/about" element={<AboutPage />} />
        <Route path="/contact" element={<ContactPage />} />
      </Routes>
    </BrowserRouter>
  );
}
```

## Exercise
- Create a multi-page app with PrimeReact MenuBar for navigation

## Checklist
- [ ] Routes configured
- [ ] Navigation works
- [ ] PrimeReact MenuBar integrated