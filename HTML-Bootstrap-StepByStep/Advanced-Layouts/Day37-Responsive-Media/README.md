# Day 37: Responsive Images & Media 📸

Optimize images for responsive design with srcset, picture element, and lazy loading.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Day 37: Responsive Images</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="p-5">
    <div class="container">
        <h1>Responsive Images</h1>
        <img src="image.jpg" class="img-fluid" alt="Responsive image" loading="lazy">
        <picture>
            <source media="(min-width: 768px)" srcset="large.jpg">
            <img src="small.jpg" class="img-fluid" alt="Art direction example">
        </picture>
    </div>
</body>
</html>
```

**Next: Day 38 - Dark Mode & Themes** 🌙
