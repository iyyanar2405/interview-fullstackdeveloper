# Day 52: Learning Platform 📚
Online education platform with courses, lessons, progress tracking.
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>LearnHub</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>
    <nav class="navbar navbar-dark bg-success"><div class="container"><a class="navbar-brand" href="#">LearnHub</a></div></nav>
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-3 bg-light p-3">
                <h5>My Courses</h5>
                <div class="list-group"><a href="#" class="list-group-item">Web Development</a></div>
            </div>
            <div class="col-md-9 p-3">
                <h2>Course Content</h2>
                <div class="accordion" id="lessons">
                    <div class="accordion-item">
                        <h2 class="accordion-header"><button class="accordion-button" data-bs-toggle="collapse" data-bs-target="#lesson1">Lesson 1</button></h2>
                        <div id="lesson1" class="accordion-collapse collapse show"><div class="accordion-body">Content...</div></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```
**Next: Day 53 - Real Estate** 🏠
