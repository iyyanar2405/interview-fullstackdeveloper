# Day 20: Table & Data Display 📊

Welcome to Day 20! Today, you'll master Bootstrap's powerful table system to organize and display data in a clean, responsive, and accessible way. You'll learn everything from basic styling to advanced interactive features like sorting, filtering, and pagination.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's table styling options
- Create responsive tables that work on any device
- Implement advanced table features like sorting and filtering
- Add pagination to large data sets
- Customize table appearances for different contexts
- Ensure tables are accessible to all users
- Integrate tables with JavaScript for dynamic data handling
- Design visually appealing and highly functional data displays

## 📚 Bootstrap Table System Deep Dive

### Table Fundamentals

Bootstrap provides a comprehensive set of classes to style HTML tables. These classes allow you to create everything from simple data grids to complex, interactive tables.

### Core Table Classes
```css
/* Base Table */
.table                   /* Base styling for tables */

/* Table Styles */
.table-striped           /* Zebra-striping for rows */
.table-bordered          /* Adds borders to all sides */
.table-borderless        /* Removes all borders */
.table-hover             /* Enables a hover state on rows */
.table-sm                /* Reduces cell padding */

/* Color & Context */
.table-primary, .table-secondary, ... /* Contextual colors */
.table-active            /* Applies hover color to a row/cell */
.table-group-divider     /* Adds a thicker border between table groups */

/* Responsiveness */
.table-responsive        /* Makes the table scroll horizontally */
.table-responsive-sm, -md, -lg, -xl /* Responsive at specific breakpoints */
```

## 💻 Comprehensive Table & Data Display Showcase

### Complete Table Showcase

Create `table-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 20: Bootstrap Tables & Data Display | Interactive Data Grids</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
            --table-header-bg: #f8f9fa;
            --table-hover-bg: rgba(0, 123, 255, 0.075);
            --card-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            --border-radius: 12px;
            --transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f4f7f9;
        }
        
        .section-header {
            background: var(--primary-gradient);
            color: white;
            padding: 4rem 0;
            margin-bottom: 3rem;
            position: relative;
            overflow: hidden;
        }
        
        .section-header::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0) 70%);
            animation: pulse 5s infinite;
        }
        
        @keyframes pulse {
            0% { transform: scale(1); }
            50% { transform: scale(1.1); }
            100% { transform: scale(1); }
        }
        
        .section-header .content {
            position: relative;
            z-index: 2;
        }
        
        .demo-section {
            background: white;
            border-radius: var(--border-radius);
            padding: 2.5rem;
            margin-bottom: 3rem;
            box-shadow: var(--card-shadow);
            border: 1px solid rgba(0,0,0,0.05);
        }
        
        .demo-title {
            color: #2d3748;
            font-weight: 700;
            margin-bottom: 1.5rem;
            padding-bottom: 0.75rem;
            border-bottom: 3px solid #e2e8f0;
            position: relative;
        }
        
        .demo-title::after {
            content: '';
            position: absolute;
            bottom: -3px;
            left: 0;
            width: 60px;
            height: 3px;
            background: var(--primary-gradient);
            border-radius: 2px;
        }
        
        /* Custom Table Styles */
        .table {
            border-radius: var(--border-radius);
            overflow: hidden;
        }
        
        .table thead th {
            background-color: var(--table-header-bg);
            color: #495057;
            font-weight: 600;
            border-bottom-width: 2px;
            border-color: #dee2e6;
        }
        
        .table-hover tbody tr:hover {
            background-color: var(--table-hover-bg);
            cursor: pointer;
        }
        
        .table th.sortable {
            cursor: pointer;
            position: relative;
        }
        
        .table th.sortable::after {
            content: ' \2195'; /* Up-down arrow */
            opacity: 0.3;
            position: absolute;
            right: 0.5rem;
        }
        
        .table th.sortable.asc::after {
            content: ' \2191'; /* Up arrow */
            opacity: 1;
        }
        
        .table th.sortable.desc::after {
            content: ' \2193'; /* Down arrow */
            opacity: 1;
        }
        
        .table-striped > tbody > tr:nth-of-type(odd) > * {
            background-color: rgba(0,0,0,0.025);
        }
        
        .table-sm td, .table-sm th {
            padding: 0.5rem;
        }
        
        .pagination .page-item.active .page-link {
            background-color: #007bff;
            border-color: #007bff;
        }
        
        .pagination .page-link {
            color: #007bff;
        }
        
        .pagination .page-link:hover {
            color: #0056b3;
        }
        
        .table-controls {
            margin-bottom: 1.5rem;
        }
        
        .status-badge {
            padding: 0.35em 0.65em;
            font-size: .75em;
            font-weight: 700;
            line-height: 1;
            color: #fff;
            text-align: center;
            white-space: nowrap;
            vertical-align: baseline;
            border-radius: 50rem;
        }
        
        .status-badge.active { background-color: #28a745; }
        .status-badge.pending { background-color: #ffc107; color: #212529; }
        .status-badge.inactive { background-color: #6c757d; }
        
        @media (max-width: 768px) {
            .demo-section {
                padding: 1.5rem;
            }
        }
    </style>
</head>
<body>
    <!-- Header -->
    <header class="section-header">
        <div class="container">
            <div class="content text-center">
                <h1 class="display-4 fw-bold mb-3">Day 20: Tables & Data Display</h1>
                <p class="lead mb-0">Mastering Bootstrap's table system for organizing and displaying data effectively.</p>
            </div>
        </div>
    </header>

    <div class="container py-4">
        
        <!-- Basic Table Styles -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-table me-2"></i>
                Basic Table Styles
            </h2>
            <p class="text-muted mb-4">Explore fundamental table styles like striped, bordered, and hover states.</p>
            
            <table class="table table-striped table-bordered table-hover">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">First</th>
                        <th scope="col">Last</th>
                        <th scope="col">Handle</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th scope="row">1</th>
                        <td>Mark</td>
                        <td>Otto</td>
                        <td>@mdo</td>
                    </tr>
                    <tr>
                        <th scope="row">2</th>
                        <td>Jacob</td>
                        <td>Thornton</td>
                        <td>@fat</td>
                    </tr>
                    <tr>
                        <th scope="row">3</th>
                        <td colspan="2">Larry the Bird</td>
                        <td>@twitter</td>
                    </tr>
                </tbody>
            </table>
        </section>
        
        <!-- Responsive Tables -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-phone me-2"></i>
                Responsive Tables
            </h2>
            <p class="text-muted mb-4">Ensure your tables look great on any device by enabling horizontal scrolling.</p>
            
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                            <th scope="col">Header</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row">1</th>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                        </tr>
                        <tr>
                            <th scope="row">2</th>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                            <td>Cell</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </section>
        
        <!-- Interactive Data Table -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-funnel me-2"></i>
                Interactive Data Table
            </h2>
            <p class="text-muted mb-4">An advanced example with sorting, filtering, and pagination powered by JavaScript.</p>
            
            <div class="table-controls row g-3 align-items-center">
                <div class="col-md-6">
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-search"></i></span>
                        <input type="text" class="form-control" id="searchInput" placeholder="Filter by name or email...">
                    </div>
                </div>
                <div class="col-md-3">
                    <select class="form-select" id="statusFilter">
                        <option value="">All Statuses</option>
                        <option value="active">Active</option>
                        <option value="pending">Pending</option>
                        <option value="inactive">Inactive</option>
                    </select>
                </div>
                <div class="col-md-3 d-flex justify-content-end">
                    <select class="form-select w-auto" id="rowsPerPage">
                        <option value="5">5 rows</option>
                        <option value="10" selected>10 rows</option>
                        <option value="20">20 rows</option>
                    </select>
                </div>
            </div>
            
            <div class="table-responsive">
                <table class="table table-hover" id="interactiveTable">
                    <thead class="table-light">
                        <tr>
                            <th scope="col" class="sortable" data-column="id">ID <i class="bi"></i></th>
                            <th scope="col" class="sortable" data-column="name">Name <i class="bi"></i></th>
                            <th scope_col" class="sortable" data-column="email">Email <i class="bi"></i></th>
                            <th scope="col" class="sortable" data-column="role">Role <i class="bi"></i></th>
                            <th scope="col" class="sortable" data-column="status">Status <i class="bi"></i></th>
                            <th scope="col">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Rows will be injected by JavaScript -->
                    </tbody>
                </table>
            </div>
            
            <nav class="d-flex justify-content-between align-items-center mt-3">
                <span id="tableInfo"></span>
                <ul class="pagination mb-0" id="pagination">
                    <!-- Pagination links will be injected by JavaScript -->
                </ul>
            </nav>
        </section>
        
    </div>

    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript for Interactive Table -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const sampleData = [
                { id: 1, name: 'Alice Johnson', email: 'alice@example.com', role: 'Admin', status: 'active' },
                { id: 2, name: 'Bob Williams', email: 'bob@example.com', role: 'Editor', status: 'pending' },
                { id: 3, name: 'Charlie Brown', email: 'charlie@example.com', role: 'User', status: 'active' },
                { id: 4, name: 'Diana Miller', email: 'diana@example.com', role: 'User', status: 'inactive' },
                { id: 5, name: 'Ethan Davis', email: 'ethan@example.com', role: 'Editor', status: 'active' },
                { id: 6, name: 'Fiona Garcia', email: 'fiona@example.com', role: 'User', status: 'pending' },
                { id: 7, name: 'George Rodriguez', email: 'george@example.com', role: 'Admin', status: 'active' },
                { id: 8, name: 'Hannah Martinez', email: 'hannah@example.com', role: 'User', status: 'inactive' },
                { id: 9, name: 'Ian Hernandez', email: 'ian@example.com', role: 'Editor', status: 'active' },
                { id: 10, name: 'Jane Lopez', email: 'jane@example.com', role: 'User', status: 'active' },
                { id: 11, name: 'Kevin Gonzalez', email: 'kevin@example.com', role: 'User', status: 'pending' },
                { id: 12, name: 'Laura Wilson', email: 'laura@example.com', role: 'Admin', status: 'inactive' },
            ];

            let currentPage = 1;
            let rowsPerPage = parseInt(document.getElementById('rowsPerPage').value);
            let currentSort = { column: 'id', order: 'asc' };
            let filteredData = [...sampleData];

            const tableBody = document.querySelector('#interactiveTable tbody');
            const searchInput = document.getElementById('searchInput');
            const statusFilter = document.getElementById('statusFilter');
            const rowsPerPageSelect = document.getElementById('rowsPerPage');
            const paginationUl = document.getElementById('pagination');
            const tableInfoSpan = document.getElementById('tableInfo');

            function renderTable() {
                tableBody.innerHTML = '';
                const start = (currentPage - 1) * rowsPerPage;
                const end = start + rowsPerPage;
                const paginatedData = filteredData.slice(start, end);

                paginatedData.forEach(item => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <th scope="row">${item.id}</th>
                        <td>${item.name}</td>
                        <td>${item.email}</td>
                        <td>${item.role}</td>
                        <td><span class="status-badge ${item.status}">${item.status}</span></td>
                        <td>
                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-pencil-square"></i></button>
                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                        </td>
                    `;
                    tableBody.appendChild(row);
                });
                
                renderPagination();
                updateTableInfo();
            }

            function renderPagination() {
                paginationUl.innerHTML = '';
                const pageCount = Math.ceil(filteredData.length / rowsPerPage);

                // Previous button
                const prevLi = document.createElement('li');
                prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
                prevLi.innerHTML = `<a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>`;
                paginationUl.appendChild(prevLi);

                // Page numbers
                for (let i = 1; i <= pageCount; i++) {
                    const li = document.createElement('li');
                    li.className = `page-item ${currentPage === i ? 'active' : ''}`;
                    li.innerHTML = `<a class="page-link" href="#" data-page="${i}">${i}</a>`;
                    paginationUl.appendChild(li);
                }

                // Next button
                const nextLi = document.createElement('li');
                nextLi.className = `page-item ${currentPage === pageCount ? 'disabled' : ''}`;
                nextLi.innerHTML = `<a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>`;
                paginationUl.appendChild(nextLi);
            }
            
            function updateTableInfo() {
                const start = (currentPage - 1) * rowsPerPage + 1;
                const end = Math.min(start + rowsPerPage - 1, filteredData.length);
                if (filteredData.length > 0) {
                    tableInfoSpan.textContent = `Showing ${start} to ${end} of ${filteredData.length} entries`;
                } else {
                    tableInfoSpan.textContent = 'No entries found';
                }
            }

            function sortData(column) {
                if (currentSort.column === column) {
                    currentSort.order = currentSort.order === 'asc' ? 'desc' : 'asc';
                } else {
                    currentSort.column = column;
                    currentSort.order = 'asc';
                }

                filteredData.sort((a, b) => {
                    const valA = a[column];
                    const valB = b[column];
                    
                    if (valA < valB) return currentSort.order === 'asc' ? -1 : 1;
                    if (valA > valB) return currentSort.order === 'asc' ? 1 : -1;
                    return 0;
                });
                
                document.querySelectorAll('#interactiveTable th.sortable').forEach(th => {
                    th.classList.remove('asc', 'desc');
                    if (th.dataset.column === column) {
                        th.classList.add(currentSort.order);
                    }
                });

                currentPage = 1;
                renderTable();
            }

            function filterAndSearch() {
                const searchTerm = searchInput.value.toLowerCase();
                const status = statusFilter.value;

                filteredData = sampleData.filter(item => {
                    const matchesSearch = item.name.toLowerCase().includes(searchTerm) || item.email.toLowerCase().includes(searchTerm);
                    const matchesStatus = status ? item.status === status : true;
                    return matchesSearch && matchesStatus;
                });

                currentPage = 1;
                renderTable();
            }

            // Event Listeners
            searchInput.addEventListener('input', filterAndSearch);
            statusFilter.addEventListener('change', filterAndSearch);
            rowsPerPageSelect.addEventListener('change', (e) => {
                rowsPerPage = parseInt(e.target.value);
                currentPage = 1;
                renderTable();
            });

            document.querySelectorAll('#interactiveTable th.sortable').forEach(th => {
                th.addEventListener('click', () => sortData(th.dataset.column));
            });

            paginationUl.addEventListener('click', (e) => {
                e.preventDefault();
                if (e.target.matches('.page-link')) {
                    const page = parseInt(e.target.dataset.page);
                    if (page && page !== currentPage) {
                        currentPage = page;
                        renderTable();
                    }
                }
            });

            // Initial render
            renderTable();
            console.log('📊 Day 20 - Table & Data Display loaded successfully!');
        });
    </script>
</body>
</html>
```

## 📋 Day 20 Table & Data Display Checklist

### Basic Table Styling
- [ ] Basic `.table` class
- [ ] Striped rows with `.table-striped`
- [ ] Bordered tables with `.table-bordered`
- [ ] Hoverable rows with `.table-hover`
- [ ] Small tables with `.table-sm`
- [ ] Contextual color classes (`.table-primary`, etc.)

### Responsiveness
- [ ] Horizontal scrolling with `.table-responsive`
- [ ] Breakpoint-specific responsive classes (`.table-responsive-md`, etc.)

### Advanced Features
- [ ] Table head and body groups (`<thead>`, `<tbody>`)
- [ ] Captions for accessibility
- [ ] Data sorting (JavaScript implementation)
- [ ] Data filtering/searching (JavaScript implementation)
- [ ] Pagination for large datasets (JavaScript implementation)

### Accessibility & UX
- [ ] Proper use of `scope` attribute
- [ ] Clear table headers
- [ ] Keyboard navigation for interactive elements
- [ ] Clear visual feedback for sorting and hover states

## 🎯 Day 20 Complete!

### ✅ Achievements Unlocked:
- **Table Styling Pro:** Mastered all of Bootstrap's table styling options.
- **Responsive Design:** Created tables that are usable on any screen size.
- **Interactive Data:** Built a fully interactive table with sorting, filtering, and pagination.
- **Data Management:** Learned techniques to handle and display large datasets effectively.
- **Accessibility Champion:** Ensured tables are accessible and semantically correct.
- **JavaScript Integration:** Combined Bootstrap components with JavaScript to create dynamic user experiences.

### 🔗 Key Takeaways:
1. **Clarity is Key:** Tables are for presenting data clearly. Use styling to enhance readability, not obscure it.
2. **Mobile-First Data:** Always use responsive wrappers to prevent layout breaking on small screens.
3. **Interactivity Enhances UX:** Sorting, filtering, and pagination transform static tables into powerful data exploration tools.
4. **Performance Matters:** For very large datasets, consider server-side pagination and filtering to keep the UI fast.
5. **Accessibility is Non-Negotiable:** Use `scope`, `<caption>`, and proper headers to make your data understandable to everyone.

---

**Congratulations!** You've completed the core Bootstrap components section. You now have the skills to build complex, beautiful, and responsive layouts and interfaces.

**Next up: Day 21 - Advanced Bootstrap Utilities** where you'll dive into Bootstrap's powerful utility API to rapidly build custom designs without writing a single line of custom CSS! 🚀