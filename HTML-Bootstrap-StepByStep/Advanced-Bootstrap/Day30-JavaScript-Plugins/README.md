# Day 30: Bootstrap JavaScript Plugins Deep Dive ⚡

Master Bootstrap's JavaScript plugins, understand the API, create custom implementations, and control components programmatically.

## 🎯 Objectives
- Master Bootstrap JS API
- Control components with JavaScript
- Handle events and callbacks
- Create custom plugin configurations
- Implement programmatic controls

## 💻 Complete HTML with JS Examples

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 30: Bootstrap JavaScript Plugins</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .code-block { background: #1e293b; color: #e2e8f0; padding: 1.5rem; border-radius: 8px; overflow-x: auto; }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 30: Bootstrap JS Plugins</h1>
            <p class="lead">Master programmatic control</p>
        </div>
        
        <!-- Modal API -->
        <section class="demo-section">
            <h2 class="mb-4">Modal API</h2>
            <button class="btn btn-primary" id="showModalBtn">Show Modal</button>
            <button class="btn btn-secondary ms-2" id="hideModalBtn">Hide Modal</button>
            <button class="btn btn-info ms-2" id="toggleModalBtn">Toggle Modal</button>
            
            <div class="modal fade" id="exampleModal" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Controlled Modal</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            This modal is controlled via JavaScript!
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="code-block mt-3">
<pre>// Modal API
const modal = new bootstrap.Modal('#exampleModal');
modal.show();     // Show modal
modal.hide();     // Hide modal
modal.toggle();   // Toggle modal

// Event listeners
modal._element.addEventListener('show.bs.modal', () => {
    console.log('Modal is showing');
});
</pre>
            </div>
        </section>
        
        <!-- Toast API -->
        <section class="demo-section">
            <h2 class="mb-4">Toast API</h2>
            <button class="btn btn-success" id="showToastBtn">Show Toast</button>
            
            <div class="toast-container position-fixed top-0 end-0 p-3">
                <div id="liveToast" class="toast" role="alert">
                    <div class="toast-header">
                        <strong class="me-auto">Notification</strong>
                        <small>Just now</small>
                        <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
                    </div>
                    <div class="toast-body">
                        Hello! This is a programmatically shown toast.
                    </div>
                </div>
            </div>
            
            <div class="code-block mt-3">
<pre>// Toast API
const toast = new bootstrap.Toast('#liveToast', {
    animation: true,
    autohide: true,
    delay: 5000
});
toast.show();
</pre>
            </div>
        </section>
        
        <!-- Collapse API -->
        <section class="demo-section">
            <h2 class="mb-4">Collapse API</h2>
            <button class="btn btn-primary" id="showCollapseBtn">Show</button>
            <button class="btn btn-secondary ms-2" id="hideCollapseBtn">Hide</button>
            <button class="btn btn-info ms-2" id="toggleCollapseBtn">Toggle</button>
            
            <div class="collapse mt-3" id="collapseExample">
                <div class="card card-body">
                    This content is controlled by JavaScript!
                </div>
            </div>
            
            <div class="code-block mt-3">
<pre>// Collapse API
const collapse = new bootstrap.Collapse('#collapseExample', {
    toggle: false
});
collapse.show();
collapse.hide();
collapse.toggle();
</pre>
            </div>
        </section>
        
        <!-- Carousel API -->
        <section class="demo-section">
            <h2 class="mb-4">Carousel API</h2>
            
            <div id="carouselExample" class="carousel slide mb-3">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="bg-primary text-white p-5 text-center">Slide 1</div>
                    </div>
                    <div class="carousel-item">
                        <div class="bg-success text-white p-5 text-center">Slide 2</div>
                    </div>
                    <div class="carousel-item">
                        <div class="bg-info text-white p-5 text-center">Slide 3</div>
                    </div>
                </div>
            </div>
            
            <button class="btn btn-primary" id="prevSlideBtn">Previous</button>
            <button class="btn btn-secondary ms-2" id="nextSlideBtn">Next</button>
            <button class="btn btn-info ms-2" id="pauseCarouselBtn">Pause</button>
            <button class="btn btn-success ms-2" id="playCarouselBtn">Play</button>
            
            <div class="code-block mt-3">
<pre>// Carousel API
const carousel = new bootstrap.Carousel('#carouselExample');
carousel.next();     // Next slide
carousel.prev();     // Previous slide
carousel.pause();    // Pause auto-cycle
carousel.cycle();    // Start auto-cycle
carousel.to(2);      // Go to specific slide (0-indexed)
</pre>
            </div>
        </section>
        
        <!-- Dropdown API -->
        <section class="demo-section">
            <h2 class="mb-4">Dropdown API</h2>
            
            <div class="dropdown d-inline-block">
                <button class="btn btn-primary dropdown-toggle" type="button" id="dropdownMenu">
                    Dropdown
                </button>
                <ul class="dropdown-menu">
                    <li><a class="dropdown-item" href="#">Action</a></li>
                    <li><a class="dropdown-item" href="#">Another action</a></li>
                </ul>
            </div>
            
            <button class="btn btn-secondary ms-2" id="showDropdownBtn">Show</button>
            <button class="btn btn-info ms-2" id="hideDropdownBtn">Hide</button>
            <button class="btn btn-success ms-2" id="toggleDropdownBtn">Toggle</button>
            
            <div class="code-block mt-3">
<pre>// Dropdown API
const dropdown = new bootstrap.Dropdown('#dropdownMenu');
dropdown.show();
dropdown.hide();
dropdown.toggle();
dropdown.update();  // Update position
</pre>
            </div>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Modal controls
        const modal = new bootstrap.Modal('#exampleModal');
        document.getElementById('showModalBtn').addEventListener('click', () => modal.show());
        document.getElementById('hideModalBtn').addEventListener('click', () => modal.hide());
        document.getElementById('toggleModalBtn').addEventListener('click', () => modal.toggle());
        
        // Toast controls
        const toast = new bootstrap.Toast('#liveToast');
        document.getElementById('showToastBtn').addEventListener('click', () => toast.show());
        
        // Collapse controls
        const collapse = new bootstrap.Collapse('#collapseExample', { toggle: false });
        document.getElementById('showCollapseBtn').addEventListener('click', () => collapse.show());
        document.getElementById('hideCollapseBtn').addEventListener('click', () => collapse.hide());
        document.getElementById('toggleCollapseBtn').addEventListener('click', () => collapse.toggle());
        
        // Carousel controls
        const carousel = new bootstrap.Carousel('#carouselExample');
        document.getElementById('prevSlideBtn').addEventListener('click', () => carousel.prev());
        document.getElementById('nextSlideBtn').addEventListener('click', () => carousel.next());
        document.getElementById('pauseCarouselBtn').addEventListener('click', () => carousel.pause());
        document.getElementById('playCarouselBtn').addEventListener('click', () => carousel.cycle());
        
        // Dropdown controls
        const dropdown = new bootstrap.Dropdown('#dropdownMenu');
        document.getElementById('showDropdownBtn').addEventListener('click', () => dropdown.show());
        document.getElementById('hideDropdownBtn').addEventListener('click', () => dropdown.hide());
        document.getElementById('toggleDropdownBtn').addEventListener('click', () => dropdown.toggle());
        
        // Event listeners examples
        document.getElementById('exampleModal').addEventListener('shown.bs.modal', () => {
            console.log('Modal fully shown');
        });
        
        document.getElementById('collapseExample').addEventListener('shown.bs.collapse', () => {
            console.log('Collapse expanded');
        });
        
        console.log('⚡ Day 30 - Bootstrap JS Plugins loaded!');
    </script>
</body>
</html>
```

## 📋 Checklist
- [ ] Modal API and events
- [ ] Toast API and options
- [ ] Collapse API
- [ ] Carousel control methods
- [ ] Dropdown API
- [ ] Offcanvas API
- [ ] Popover/Tooltip initialization
- [ ] Event handling
- [ ] Custom configurations
- [ ] Dispose and cleanup methods

## 🎯 Key Methods

| Component | show() | hide() | toggle() | dispose() |
|-----------|--------|--------|----------|-----------|
| Modal     | ✅     | ✅     | ✅       | ✅        |
| Toast     | ✅     | ✅     | ❌       | ✅        |
| Collapse  | ✅     | ✅     | ✅       | ✅        |
| Dropdown  | ✅     | ✅     | ✅       | ✅        |
| Offcanvas | ✅     | ✅     | ✅       | ✅        |

**Next: Day 31 - Custom Component Architecture** 🏗️
