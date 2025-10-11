# Day 26: Breadcrumbs & Pagination 🗺️

Master navigation components that help users understand their location in your site hierarchy and navigate through paginated content.

## 💻 Complete HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 26: Breadcrumbs & Pagination</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .demo-title { color: #2d3748; font-weight: 700; margin-bottom: 2rem; border-bottom: 3px solid #667eea; padding-bottom: 1rem; }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 26: Breadcrumbs & Pagination</h1>
            <p class="lead">Navigation indicators for better UX</p>
        </div>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-signpost me-2"></i>
                Breadcrumbs
            </h2>
            
            <nav aria-label="breadcrumb">
                <ol class="breadcrumb">
                    <li class="breadcrumb-item"><a href="#">Home</a></li>
                    <li class="breadcrumb-item"><a href="#">Library</a></li>
                    <li class="breadcrumb-item active" aria-current="page">Data</li>
                </ol>
            </nav>
            
            <nav aria-label="breadcrumb" class="mt-3">
                <ol class="breadcrumb">
                    <li class="breadcrumb-item"><a href="#"><i class="bi bi-house-door"></i> Home</a></li>
                    <li class="breadcrumb-item"><a href="#"><i class="bi bi-folder"></i> Products</a></li>
                    <li class="breadcrumb-item"><a href="#"><i class="bi bi-tag"></i> Categories</a></li>
                    <li class="breadcrumb-item active" aria-current="page">Electronics</li>
                </ol>
            </nav>
            
            <nav aria-label="breadcrumb" class="mt-3">
                <ol class="breadcrumb bg-light p-3 rounded">
                    <li class="breadcrumb-item"><a href="#">Dashboard</a></li>
                    <li class="breadcrumb-item"><a href="#">Settings</a></li>
                    <li class="breadcrumb-item active">Profile</li>
                </ol>
            </nav>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-chevron-bar-contract me-2"></i>
                Pagination - Basic
            </h2>
            
            <nav aria-label="Page navigation">
                <ul class="pagination">
                    <li class="page-item"><a class="page-link" href="#">Previous</a></li>
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">Next</a></li>
                </ul>
            </nav>
            
            <nav aria-label="Page navigation" class="mt-3">
                <ul class="pagination">
                    <li class="page-item disabled">
                        <a class="page-link" href="#" tabindex="-1" aria-disabled="true">Previous</a>
                    </li>
                    <li class="page-item active" aria-current="page">
                        <a class="page-link" href="#">1</a>
                    </li>
                    <li class="page-item"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">Next</a></li>
                </ul>
            </nav>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-arrows-angle-expand me-2"></i>
                Pagination Sizes
            </h2>
            
            <nav aria-label="Large pagination">
                <ul class="pagination pagination-lg">
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                </ul>
            </nav>
            
            <nav aria-label="Default pagination">
                <ul class="pagination">
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                </ul>
            </nav>
            
            <nav aria-label="Small pagination">
                <ul class="pagination pagination-sm">
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                </ul>
            </nav>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-align-center me-2"></i>
                Pagination Alignment
            </h2>
            
            <nav aria-label="Left aligned">
                <ul class="pagination">
                    <li class="page-item"><a class="page-link" href="#">«</a></li>
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">»</a></li>
                </ul>
            </nav>
            
            <nav aria-label="Center aligned">
                <ul class="pagination justify-content-center">
                    <li class="page-item"><a class="page-link" href="#">«</a></li>
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">»</a></li>
                </ul>
            </nav>
            
            <nav aria-label="Right aligned">
                <ul class="pagination justify-content-end">
                    <li class="page-item"><a class="page-link" href="#">«</a></li>
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item active"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">»</a></li>
                </ul>
            </nav>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        console.log('🗺️ Day 26 - Breadcrumbs & Pagination loaded!');
        
        // Example: Dynamic pagination
        document.querySelectorAll('.pagination .page-link').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                console.log('Page clicked:', this.textContent);
            });
        });
    </script>
</body>
</html>
```

## 📋 Checklist
- [ ] Basic breadcrumbs
- [ ] Breadcrumbs with icons
- [ ] Styled breadcrumbs
- [ ] Basic pagination
- [ ] Active/disabled states
- [ ] Pagination sizes (sm, default, lg)
- [ ] Pagination alignment
- [ ] Icon pagination

**Next: Day 27 - Alert & Toast Components** 🔔
