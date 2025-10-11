# Day 32: Advanced Grid Techniques 📐

Master complex grid layouts, nested grids, grid offsets, and advanced responsive patterns for sophisticated page structures.

## 🎯 Key Topics
- Complex multi-column layouts
- Nested grids
- Grid offsets and ordering
- Responsive column wrapping
- Auto-layout columns
- Custom grid breakpoints

## 💻 HTML Examples

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 32: Advanced Grid Techniques</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .grid-demo [class^="col"] { background: #667eea; color: white; padding: 1rem; margin-bottom: 1rem; border: 2px solid white; text-align: center; }
        .section { background: white; padding: 3rem; margin-bottom: 2rem; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); }
    </style>
</head>
<body class="bg-light">
    <div class="container py-5">
        <h1 class="text-center mb-5">Day 32: Advanced Grid Techniques</h1>
        
        <!-- Complex Layout -->
        <section class="section">
            <h2>Complex Masonry-Style Layout</h2>
            <div class="row g-3 grid-demo">
                <div class="col-md-8">Span 8 columns</div>
                <div class="col-md-4">Span 4 columns</div>
                <div class="col-md-4">Span 4</div>
                <div class="col-md-4">Span 4</div>
                <div class="col-md-4">Span 4</div>
                <div class="col-md-6">Span 6</div>
                <div class="col-md-6">Span 6</div>
            </div>
        </section>
        
        <!-- Nested Grids -->
        <section class="section">
            <h2>Nested Grids</h2>
            <div class="row grid-demo">
                <div class="col-md-9">
                    Level 1: col-md-9
                    <div class="row mt-2">
                        <div class="col-md-6" style="background: #f093fb;">Level 2: col-md-6</div>
                        <div class="col-md-6" style="background: #f093fb;">Level 2: col-md-6</div>
                    </div>
                </div>
                <div class="col-md-3">Level 1: col-md-3</div>
            </div>
        </section>
        
        <!-- Offset Columns -->
        <section class="section">
            <h2>Column Offsets</h2>
            <div class="row grid-demo">
                <div class="col-md-4">col-md-4</div>
                <div class="col-md-4 offset-md-4">col-md-4 offset-md-4</div>
            </div>
            <div class="row grid-demo">
                <div class="col-md-3 offset-md-3">col-md-3 offset-md-3</div>
                <div class="col-md-3 offset-md-3">col-md-3 offset-md-3</div>
            </div>
        </section>
        
        <!-- Column Ordering -->
        <section class="section">
            <h2>Column Ordering</h2>
            <div class="row grid-demo">
                <div class="col order-last">First in DOM, last visually</div>
                <div class="col">Second</div>
                <div class="col order-first">Third in DOM, first visually</div>
            </div>
        </section>
    </div>
</body>
</html>
```

## 📋 Techniques Covered
- [ ] Multi-column layouts
- [ ] Nested grids
- [ ] Column offsets
- [ ] Column ordering
- [ ] Auto-layout columns
- [ ] Responsive stacking

**Next: Day 33 - Utilities API** 🛠️
