# Day 27: Alert & Toast Components 🔔

Master notification components for displaying feedback messages, alerts, and temporary notifications with style and accessibility.

## 💻 Complete HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 27: Alerts & Toasts</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .demo-title { color: #2d3748; font-weight: 700; margin-bottom: 2rem; }
        .toast-container { position: fixed; top: 20px; right: 20px; z-index: 1090; }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 27: Alerts & Toasts</h1>
            <p class="lead">Notification and feedback components</p>
        </div>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-exclamation-triangle me-2"></i>
                Alert Components
            </h2>
            
            <div class="alert alert-primary" role="alert">
                A simple primary alert—check it out!
            </div>
            <div class="alert alert-secondary" role="alert">
                A simple secondary alert—check it out!
            </div>
            <div class="alert alert-success" role="alert">
                A simple success alert—check it out!
            </div>
            <div class="alert alert-danger" role="alert">
                A simple danger alert—check it out!
            </div>
            <div class="alert alert-warning" role="alert">
                A simple warning alert—check it out!
            </div>
            <div class="alert alert-info" role="alert">
                A simple info alert—check it out!
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-x-circle me-2"></i>
                Dismissible Alerts
            </h2>
            
            <div class="alert alert-warning alert-dismissible fade show" role="alert">
                <strong>Warning!</strong> You should check in on some of those fields below.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
            
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <strong>Success!</strong> Your changes have been saved.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
            
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <strong>Error!</strong> There was a problem processing your request.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-star me-2"></i>
                Alert with Icons & Links
            </h2>
            
            <div class="alert alert-success d-flex align-items-center" role="alert">
                <i class="bi bi-check-circle-fill me-2"></i>
                <div>
                    Success! Your profile has been updated. <a href="#" class="alert-link">View profile</a>
                </div>
            </div>
            
            <div class="alert alert-warning d-flex align-items-center" role="alert">
                <i class="bi bi-exclamation-triangle-fill me-2"></i>
                <div>
                    Warning! Please verify your email address. <a href="#" class="alert-link">Resend verification</a>
                </div>
            </div>
            
            <div class="alert alert-info d-flex align-items-center" role="alert">
                <i class="bi bi-info-circle-fill me-2"></i>
                <div>
                    Info! A new version is available. <a href="#" class="alert-link">Learn more</a>
                </div>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-bell me-2"></i>
                Toast Notifications
            </h2>
            
            <button type="button" class="btn btn-primary" id="showToast1">Show Toast</button>
            <button type="button" class="btn btn-success ms-2" id="showToast2">Success Toast</button>
            <button type="button" class="btn btn-danger ms-2" id="showToast3">Error Toast</button>
            <button type="button" class="btn btn-warning ms-2" id="showToast4">Warning Toast</button>
            
            <div class="mt-4">
                <h5>Toast Example (Live Demo)</h5>
                <div class="toast align-items-center text-white bg-primary border-0" role="alert" id="toast1">
                    <div class="d-flex">
                        <div class="toast-body">
                            Hello! This is a toast message.
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                    </div>
                </div>
            </div>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-stack me-2"></i>
                Advanced Toast
            </h2>
            
            <div class="toast" role="alert" id="advancedToast">
                <div class="toast-header">
                    <i class="bi bi-bell-fill text-primary me-2"></i>
                    <strong class="me-auto">Bootstrap</strong>
                    <small>11 mins ago</small>
                    <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body">
                    Hello, world! This is a toast message with a header.
                </div>
            </div>
            
            <button type="button" class="btn btn-info mt-3" id="showAdvancedToast">Show Advanced Toast</button>
        </section>
    </div>
    
    <!-- Toast Container -->
    <div class="toast-container"></div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Toast button handlers
        document.getElementById('showToast1').addEventListener('click', function() {
            const toast = new bootstrap.Toast(document.getElementById('toast1'));
            toast.show();
        });
        
        document.getElementById('showToast2').addEventListener('click', function() {
            showToast('Success!', 'Operation completed successfully.', 'success');
        });
        
        document.getElementById('showToast3').addEventListener('click', function() {
            showToast('Error!', 'Something went wrong.', 'danger');
        });
        
        document.getElementById('showToast4').addEventListener('click', function() {
            showToast('Warning!', 'Please review your input.', 'warning');
        });
        
        document.getElementById('showAdvancedToast').addEventListener('click', function() {
            const toast = new bootstrap.Toast(document.getElementById('advancedToast'));
            toast.show();
        });
        
        // Dynamic toast creation
        function showToast(title, message, type = 'primary') {
            const toastContainer = document.querySelector('.toast-container');
            const toastHtml = `
                <div class="toast align-items-center text-white bg-${type} border-0" role="alert">
                    <div class="d-flex">
                        <div class="toast-body">
                            <strong>${title}</strong><br>${message}
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                    </div>
                </div>
            `;
            toastContainer.insertAdjacentHTML('beforeend', toastHtml);
            const toastElement = toastContainer.lastElementChild;
            const toast = new bootstrap.Toast(toastElement);
            toast.show();
            
            toastElement.addEventListener('hidden.bs.toast', function() {
                toastElement.remove();
            });
        }
        
        console.log('🔔 Day 27 - Alerts & Toasts loaded!');
    </script>
</body>
</html>
```

## 📋 Checklist
- [ ] Basic alerts (all colors)
- [ ] Dismissible alerts
- [ ] Alerts with icons
- [ ] Alerts with links
- [ ] Basic toasts
- [ ] Toast with header
- [ ] Auto-dismiss toasts
- [ ] Dynamic toast generation
- [ ] Toast positioning

**Next: Day 28 - Offcanvas Components** 📱
