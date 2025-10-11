# Day 25: Badges, Labels & Progress Bars 🏷️

Master visual indicators including badges for counts/notifications, progress bars for tracking completion, and spinners for loading states.

## 💻 Complete HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 25: Badges & Progress</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .demo-title { color: #2d3748; font-weight: 700; margin-bottom: 2rem; }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 25: Badges & Progress</h1>
        </div>
        
        <section class="demo-section">
            <h2 class="demo-title">Badges</h2>
            <h1>Heading <span class="badge bg-primary">New</span></h1>
            <h2>Heading <span class="badge bg-secondary">4</span></h2>
            <button type="button" class="btn btn-primary">
                Notifications <span class="badge bg-danger">9</span>
            </button>
            <button type="button" class="btn btn-primary position-relative">
                Inbox
                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                    99+ <span class="visually-hidden">unread messages</span>
                </span>
            </button>
            
            <div class="mt-4">
                <span class="badge bg-primary">Primary</span>
                <span class="badge bg-secondary">Secondary</span>
                <span class="badge bg-success">Success</span>
                <span class="badge bg-danger">Danger</span>
                <span class="badge bg-warning text-dark">Warning</span>
                <span class="badge bg-info text-dark">Info</span>
                <span class="badge bg-light text-dark">Light</span>
                <span class="badge bg-dark">Dark</span>
            </div>
            
            <div class="mt-4">
                <span class="badge rounded-pill bg-primary">Pill Badge</span>
                <span class="badge rounded-pill bg-success">Success Pill</span>
                <span class="badge rounded-pill bg-danger">Danger Pill</span>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">Progress Bars</h2>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar" style="width: 25%">25%</div>
            </div>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar bg-success" style="width: 50%">50%</div>
            </div>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar bg-info" style="width: 75%">75%</div>
            </div>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar bg-danger" style="width: 100%">100%</div>
            </div>
            
            <h5 class="mt-4">Striped & Animated</h5>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar progress-bar-striped" style="width: 40%"></div>
            </div>
            <div class="progress mb-3" role="progressbar">
                <div class="progress-bar progress-bar-striped progress-bar-animated bg-success" style="width: 75%"></div>
            </div>
            
            <h5 class="mt-4">Multiple Bars</h5>
            <div class="progress" role="progressbar">
                <div class="progress-bar" style="width: 15%"></div>
                <div class="progress-bar bg-success" style="width: 30%"></div>
                <div class="progress-bar bg-info" style="width: 20%"></div>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">Spinners</h2>
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <div class="spinner-border text-success" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <div class="spinner-border text-danger" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            
            <div class="mt-4">
                <div class="spinner-grow text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div class="spinner-grow text-success" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div class="spinner-grow text-danger" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
            
            <div class="mt-4">
                <button class="btn btn-primary" type="button" disabled>
                    <span class="spinner-border spinner-border-sm" role="status"></span>
                    Loading...
                </button>
            </div>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>console.log('🏷️ Day 25 loaded!');</script>
</body>
</html>
```

## 📋 Checklist
- [ ] Text badges
- [ ] Button badges
- [ ] Pill badges
- [ ] Position badges
- [ ] Progress bars
- [ ] Striped/animated progress
- [ ] Multiple progress bars
- [ ] Spinners (border & grow)

**Next: Day 26 - Breadcrumbs & Pagination** 🗺️
