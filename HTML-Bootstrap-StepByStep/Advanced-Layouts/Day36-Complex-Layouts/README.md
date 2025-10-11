# Day 36: Complex Layouts & Masonry 🧱

Build advanced page layouts including masonry grids, dashboard layouts, and magazine-style designs.

## 💻 Masonry Layout Example

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Day 36: Complex Layouts</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .masonry { column-count: 3; column-gap: 1rem; }
        .masonry-item { break-inside: avoid; margin-bottom: 1rem; }
        .card { border: none; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    </style>
</head>
<body class="bg-light p-5">
    <div class="container">
        <h1 class="mb-4">Masonry Layout</h1>
        <div class="masonry">
            <div class="masonry-item"><div class="card p-3" style="height:200px">Item 1</div></div>
            <div class="masonry-item"><div class="card p-3" style="height:150px">Item 2</div></div>
            <div class="masonry-item"><div class="card p-3" style="height:250px">Item 3</div></div>
            <div class="masonry-item"><div class="card p-3" style="height:180px">Item 4</div></div>
            <div class="masonry-item"><div class="card p-3" style="height:220px">Item 5</div></div>
            <div class="masonry-item"><div class="card p-3" style="height:160px">Item 6</div></div>
        </div>
    </div>
</body>
</html>
```

**Next: Day 37 - Responsive Images & Media** 📸
