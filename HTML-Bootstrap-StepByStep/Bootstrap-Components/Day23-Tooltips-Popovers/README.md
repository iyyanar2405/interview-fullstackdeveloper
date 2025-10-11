# Day 23: Tooltips & Popovers 💬

Master interactive UI helpers that provide contextual information and enhance user experience with elegant, accessible tooltips and popovers.

## 🎯 Learning Objectives

- Create and customize tooltips
- Build interactive popovers with rich content
- Control positioning and triggers
- Style with custom CSS
- Ensure accessibility
- JavaScript integration

## 📚 Core Classes

```css
/* Tooltips - Must be initialized with JavaScript */
data-bs-toggle="tooltip"
data-bs-placement="top|bottom|left|right"
data-bs-title="Tooltip text"

/* Popovers - Must be initialized with JavaScript */
data-bs-toggle="popover"
data-bs-placement="top|bottom|left|right"
data-bs-title="Popover title"
data-bs-content="Popover content"
```

## 💻 Complete Showcase

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 23: Tooltips & Popovers</title>
    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 3rem 0;
        }
        
        .demo-section {
            background: white;
            border-radius: 15px;
            padding: 3rem;
            margin-bottom: 2rem;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
        }
        
        .demo-title {
            color: #2d3748;
            font-weight: 700;
            margin-bottom: 2rem;
            border-bottom: 3px solid #667eea;
            padding-bottom: 1rem;
        }
        
        .tooltip-demo-area {
            display: flex;
            justify-content: space-around;
            align-items: center;
            padding: 3rem;
            background: #f8f9fa;
            border-radius: 10px;
            margin: 2rem 0;
        }
        
        /* Custom Tooltip Styles */
        .tooltip .tooltip-inner {
            background-color: #667eea;
            color: white;
            font-weight: 500;
            padding: 0.5rem 1rem;
        }
        
        .tooltip .tooltip-arrow::before {
            border-top-color: #667eea;
        }
        
        /* Custom Popover Styles */
        .popover {
            border: 2px solid #667eea;
        }
        
        .popover-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            font-weight: 600;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 23: Tooltips & Popovers</h1>
            <p class="lead">Interactive UI helpers for better UX</p>
        </div>
        
        <!-- Tooltips -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-chat-dots me-2"></i>
                Tooltips
            </h2>
            
            <div class="tooltip-demo-area">
                <button type="button" class="btn btn-primary" 
                        data-bs-toggle="tooltip" 
                        data-bs-placement="top" 
                        data-bs-title="Tooltip on top">
                    Top
                </button>
                
                <button type="button" class="btn btn-success" 
                        data-bs-toggle="tooltip" 
                        data-bs-placement="right" 
                        data-bs-title="Tooltip on right">
                    Right
                </button>
                
                <button type="button" class="btn btn-danger" 
                        data-bs-toggle="tooltip" 
                        data-bs-placement="bottom" 
                        data-bs-title="Tooltip on bottom">
                    Bottom
                </button>
                
                <button type="button" class="btn btn-warning" 
                        data-bs-toggle="tooltip" 
                        data-bs-placement="left" 
                        data-bs-title="Tooltip on left">
                    Left
                </button>
            </div>
            
            <div class="mt-4">
                <p>Hover over icons: 
                    <i class="bi bi-info-circle text-primary" 
                       data-bs-toggle="tooltip" 
                       data-bs-title="Information icon"></i>
                    <i class="bi bi-exclamation-triangle text-warning ms-3" 
                       data-bs-toggle="tooltip" 
                       data-bs-title="Warning icon"></i>
                    <i class="bi bi-check-circle text-success ms-3" 
                       data-bs-toggle="tooltip" 
                       data-bs-title="Success icon"></i>
                </p>
            </div>
        </section>
        
        <!-- Popovers -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-chat-square-text me-2"></i>
                Popovers
            </h2>
            
            <div class="tooltip-demo-area">
                <button type="button" class="btn btn-outline-primary" 
                        data-bs-toggle="popover" 
                        data-bs-placement="top"
                        data-bs-title="Popover Title"
                        data-bs-content="This is a popover with more detailed content than a tooltip.">
                    Top Popover
                </button>
                
                <button type="button" class="btn btn-outline-success" 
                        data-bs-toggle="popover" 
                        data-bs-placement="right"
                        data-bs-title="Right Popover"
                        data-bs-content="Popovers can contain longer text and more information.">
                    Right Popover
                </button>
                
                <button type="button" class="btn btn-outline-danger" 
                        data-bs-toggle="popover" 
                        data-bs-placement="bottom"
                        data-bs-title="Bottom Popover"
                        data-bs-content="Click to toggle this popover with rich content.">
                    Bottom Popover
                </button>
                
                <button type="button" class="btn btn-outline-warning" 
                        data-bs-toggle="popover" 
                        data-bs-placement="left"
                        data-bs-title="Left Popover"
                        data-bs-content="Popovers are great for contextual help.">
                    Left Popover
                </button>
            </div>
            
            <div class="alert alert-info mt-4">
                <strong>Tip:</strong> Click on the buttons to show popovers. Click again or click outside to dismiss.
            </div>
        </section>
        
        <!-- Dismissible Popover -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-x-circle me-2"></i>
                Dismissible Popover
            </h2>
            
            <button type="button" class="btn btn-lg btn-danger" 
                    data-bs-toggle="popover" 
                    data-bs-trigger="focus"
                    data-bs-title="Dismissible popover"
                    data-bs-content="Click anywhere outside this popover to dismiss it.">
                Dismissible Popover
            </button>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Initialize tooltips
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
        
        // Initialize popovers
        const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');
        const popoverList = [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl));
        
        console.log('💬 Day 23 - Tooltips & Popovers loaded!');
    </script>
</body>
</html>
```

## 📋 Checklist

- [ ] Basic tooltips (4 directions)
- [ ] Icon tooltips
- [ ] Popovers with title and content
- [ ] Dismissible popovers
- [ ] Custom styling
- [ ] JavaScript initialization
- [ ] Accessibility features

## 🎯 Complete!

**Next: Day 24 - Dropdown Components** ⬇️
