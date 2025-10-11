# Day 35: Accessibility Best Practices ♿

Ensure your Bootstrap applications are accessible to all users with proper ARIA, semantic HTML, and keyboard navigation.

## 📋 Accessibility Checklist
- Semantic HTML elements
- ARIA attributes
- Keyboard navigation
- Color contrast (WCAG AA/AAA)
- Screen reader support
- Focus management

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 35: Accessibility Best Practices</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>
    <a href="#main" class="visually-hidden-focusable">Skip to main content</a>
    
    <nav aria-label="Main navigation" class="navbar navbar-expand-lg navbar-dark bg-primary">
        <div class="container">
            <a class="navbar-brand" href="#">Accessible Site</a>
        </div>
    </nav>
    
    <main id="main" class="container py-5">
        <h1>Accessibility Features</h1>
        
        <!-- Accessible Form -->
        <form>
            <div class="mb-3">
                <label for="name" class="form-label">Name</label>
                <input type="text" class="form-control" id="name" aria-describedby="nameHelp" required>
                <div id="nameHelp" class="form-text">Enter your full name.</div>
            </div>
            
            <fieldset class="mb-3">
                <legend>Choose an option</legend>
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="option" id="option1" value="1">
                    <label class="form-check-label" for="option1">Option 1</label>
                </div>
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="option" id="option2" value="2">
                    <label class="form-check-label" for="option2">Option 2</label>
                </div>
            </fieldset>
            
            <button type="submit" class="btn btn-primary">
                <span aria-hidden="true">→</span> Submit
            </button>
        </form>
        
        <!-- Accessible Table -->
        <table class="table table-striped mt-5" aria-label="User data">
            <caption>List of users</caption>
            <thead>
                <tr>
                    <th scope="col">Name</th>
                    <th scope="col">Email</th>
                    <th scope="col">Status</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th scope="row">John Doe</th>
                    <td>john@example.com</td>
                    <td><span class="badge bg-success">Active</span></td>
                </tr>
            </tbody>
        </table>
    </main>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

## ✅ WCAG Guidelines
- **Perceivable**: Alt text, captions, color contrast
- **Operable**: Keyboard navigation, no time limits
- **Understandable**: Clear language, consistent navigation
- **Robust**: Valid HTML, ARIA attributes

**Next: Day 36 - Complex Layouts & Masonry** 🧱
