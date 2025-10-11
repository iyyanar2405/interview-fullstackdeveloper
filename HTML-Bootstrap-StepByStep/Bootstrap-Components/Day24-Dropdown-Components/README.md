# Day 24: Dropdown Components ⬇️

Master Bootstrap dropdowns for creating interactive menus, navigation options, and action selectors with style and functionality.

## 🎯 Objectives

- Create dropdown menus
- Build split button dropdowns
- Implement dropdown variations
- Custom styling and positioning
- Menu items and dividers

## 💻 Complete HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 24: Dropdown Components</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .demo-title { color: #2d3748; font-weight: 700; margin-bottom: 2rem; border-bottom: 3px solid #667eea; padding-bottom: 1rem; }
        .dropdown-demo { display: flex; gap: 1rem; flex-wrap: wrap; padding: 2rem; background: #f8f9fa; border-radius: 10px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 24: Dropdowns</h1>
            <p class="lead">Interactive menus and selectors</p>
        </div>
        
        <section class="demo-section">
            <h2 class="demo-title">Basic Dropdowns</h2>
            <div class="dropdown-demo">
                <div class="dropdown">
                    <button class="btn btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Dropdown
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="#">Action</a></li>
                        <li><a class="dropdown-item" href="#">Another action</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#">Separated link</a></li>
                    </ul>
                </div>
                
                <div class="btn-group">
                    <button type="button" class="btn btn-success">Split Button</button>
                    <button type="button" class="btn btn-success dropdown-toggle dropdown-toggle-split" data-bs-toggle="dropdown">
                        <span class="visually-hidden">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="#">Option 1</a></li>
                        <li><a class="dropdown-item" href="#">Option 2</a></li>
                        <li><a class="dropdown-item" href="#">Option 3</a></li>
                    </ul>
                </div>
                
                <div class="dropup">
                    <button class="btn btn-danger dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Dropup
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="#">Item 1</a></li>
                        <li><a class="dropdown-item" href="#">Item 2</a></li>
                    </ul>
                </div>
                
                <div class="dropend">
                    <button class="btn btn-warning dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Dropend
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="#">Item 1</a></li>
                        <li><a class="dropdown-item" href="#">Item 2</a></li>
                    </ul>
                </div>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">Advanced Dropdown Features</h2>
            <div class="dropdown-demo">
                <div class="dropdown">
                    <button class="btn btn-info dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Menu with Headers
                    </button>
                    <ul class="dropdown-menu">
                        <li><h6 class="dropdown-header">Section 1</h6></li>
                        <li><a class="dropdown-item" href="#">Item 1.1</a></li>
                        <li><a class="dropdown-item" href="#">Item 1.2</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><h6 class="dropdown-header">Section 2</h6></li>
                        <li><a class="dropdown-item" href="#">Item 2.1</a></li>
                    </ul>
                </div>
                
                <div class="dropdown">
                    <button class="btn btn-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Menu with Icons
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="#"><i class="bi bi-house me-2"></i>Home</a></li>
                        <li><a class="dropdown-item" href="#"><i class="bi bi-gear me-2"></i>Settings</a></li>
                        <li><a class="dropdown-item" href="#"><i class="bi bi-person me-2"></i>Profile</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#"><i class="bi bi-box-arrow-right me-2"></i>Logout</a></li>
                    </ul>
                </div>
            </div>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>console.log('⬇️ Day 24 - Dropdowns loaded!');</script>
</body>
</html>
```

## 📋 Checklist
- [ ] Basic dropdown
- [ ] Split button dropdown
- [ ] Dropup, dropend, dropstart
- [ ] Dropdown with headers
- [ ] Dropdown with icons
- [ ] Dropdown dividers

**Next: Day 25 - Badges, Labels & Progress** 🏷️
