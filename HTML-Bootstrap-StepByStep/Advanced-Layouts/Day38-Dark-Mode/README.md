# Day 38: Dark Mode & Theme Switching 🌙

Implement dark mode with CSS variables and JavaScript theme switcher.

```html
<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="UTF-8">
    <title>Day 38: Dark Mode</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>
    <nav class="navbar navbar-expand-lg bg-body-tertiary">
        <div class="container">
            <span class="navbar-brand">Dark Mode Demo</span>
            <button class="btn btn-outline-primary" id="themeToggle">Toggle Theme</button>
        </div>
    </nav>
    <div class="container py-5">
        <h1>Dark Mode Support</h1>
        <p class="lead">Bootstrap 5.3+ includes built-in dark mode support.</p>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        document.getElementById('themeToggle').addEventListener('click', () => {
            const html = document.documentElement;
            const current = html.getAttribute('data-bs-theme');
            html.setAttribute('data-bs-theme', current === 'dark' ? 'light' : 'dark');
        });
    </script>
</body>
</html>
```

**Next: Day 39 - Bootstrap + React Integration** ⚛️
