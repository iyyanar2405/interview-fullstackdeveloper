# Day 34: Performance Optimization ⚡

Optimize Bootstrap applications for speed: tree-shaking, lazy loading, critical CSS, and bundle optimization.

## 📋 Techniques
- Remove unused CSS
- Lazy load components
- Optimize images
- Minify and compress
- Use CDN effectively
- Critical CSS extraction

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Day 34: Performance Optimization</title>
    <!-- Preconnect to CDN -->
    <link rel="preconnect" href="https://cdn.jsdelivr.net">
    <!-- Critical CSS inline -->
    <style>body{margin:0;font-family:system-ui}</style>
    <!-- Async load Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" media="print" onload="this.media='all'">
</head>
<body>
    <div class="container py-5">
        <h1>Performance Optimization</h1>
        <ul>
            <li>Minimize HTTP requests</li>
            <li>Use compression (gzip/brotli)</li>
            <li>Optimize images (WebP)</li>
            <li>Lazy load off-screen content</li>
            <li>Tree-shake unused code</li>
        </ul>
    </div>
    <!-- Defer JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js" defer></script>
</body>
</html>
```

**Next: Day 35 - Accessibility Best Practices** ♿
