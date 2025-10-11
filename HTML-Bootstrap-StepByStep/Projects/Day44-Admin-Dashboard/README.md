# Day 44: Admin Dashboard Project 📊

Build a fully functional admin dashboard with sidebar navigation, statistics, charts, data tables, and user management.

## 🎯 Learning Objectives
- Create fixed sidebar layouts
- Design dashboard statistics cards
- Build data tables with Bootstrap
- Implement user management interfaces
- Create notification systems
- Design settings panels
- Build responsive admin layouts

## 📊 Project Features
- ✅ Fixed sidebar navigation
- ✅ Top navigation bar with search
- ✅ Statistics cards with icons
- ✅ Data tables with actions
- ✅ Charts section (Chart.js ready)
- ✅ User management table
- ✅ Notification dropdown
- ✅ Profile dropdown
- ✅ Responsive mobile menu
- ✅ Dark mode toggle

## 💻 Complete Admin Dashboard

```html
<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Dashboard</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { font-size: 0.875rem; }
        .sidebar {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            z-index: 100;
            padding: 48px 0 0;
            width: 260px;
            background: var(--bs-dark);
        }
        .sidebar-sticky {
            height: calc(100vh - 48px);
            overflow-x: hidden;
            overflow-y: auto;
        }
        .sidebar .nav-link {
            color: rgba(255,255,255,.75);
            border-radius: 0.25rem;
            margin: 0 8px;
            padding: 10px 12px;
        }
        .sidebar .nav-link:hover, .sidebar .nav-link.active {
            color: #fff;
            background: rgba(255,255,255,.1);
        }
        .sidebar .nav-link i {
            width: 16px;
            margin-right: 8px;
        }
        .navbar { position: fixed; top: 0; right: 0; left: 260px; z-index: 99; }
        .main-content { margin-left: 260px; margin-top: 48px; padding: 2rem; }
        .stat-card {
            border-left: 4px solid;
            transition: transform 0.3s;
        }
        .stat-card:hover { transform: translateY(-5px); }
        @media (max-width: 768px) {
            .sidebar { left: -260px; }
            .sidebar.show { left: 0; }
            .navbar, .main-content { left: 0; margin-left: 0; }
        }
    </style>
</head>
<body>
    <!-- Top Navbar -->
    <nav class="navbar navbar-dark bg-dark border-bottom">
        <div class="container-fluid">
            <button class="navbar-toggler d-md-none" type="button" onclick="toggleSidebar()">
                <span class="navbar-toggler-icon"></span>
            </button>
            <form class="d-flex flex-grow-1 mx-4">
                <input class="form-control" type="search" placeholder="Search..." style="max-width: 400px;">
            </form>
            <div class="d-flex align-items-center gap-3">
                <button class="btn btn-dark position-relative" data-bs-toggle="dropdown">
                    <i class="bi bi-bell"></i>
                    <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">3</span>
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><h6 class="dropdown-header">Notifications</h6></li>
                    <li><a class="dropdown-item" href="#"><i class="bi bi-info-circle me-2"></i>New user registered</a></li>
                    <li><a class="dropdown-item" href="#"><i class="bi bi-cart me-2"></i>New order received</a></li>
                    <li><a class="dropdown-item" href="#"><i class="bi bi-exclamation-triangle me-2"></i>Server warning</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><a class="dropdown-item text-center" href="#">View all</a></li>
                </ul>
                
                <div class="dropdown">
                    <button class="btn btn-dark d-flex align-items-center gap-2" data-bs-toggle="dropdown">
                        <img src="https://via.placeholder.com/32" class="rounded-circle" alt="User">
                        <span class="d-none d-md-inline">Admin User</span>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#"><i class="bi bi-person me-2"></i>Profile</a></li>
                        <li><a class="dropdown-item" href="#"><i class="bi bi-gear me-2"></i>Settings</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#" id="themeToggle"><i class="bi bi-moon me-2"></i>Dark Mode</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#"><i class="bi bi-box-arrow-right me-2"></i>Logout</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </nav>

    <!-- Sidebar -->
    <nav class="sidebar" id="sidebar">
        <div class="position-sticky sidebar-sticky">
            <div class="text-white text-center py-3 border-bottom border-secondary">
                <h5 class="mb-0"><i class="bi bi-speedometer2 me-2"></i>Admin Panel</h5>
            </div>
            <ul class="nav flex-column mt-3">
                <li class="nav-item">
                    <a class="nav-link active" href="#dashboard">
                        <i class="bi bi-house-door"></i>Dashboard
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#users">
                        <i class="bi bi-people"></i>Users
                        <span class="badge bg-primary ms-auto">1,234</span>
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#products">
                        <i class="bi bi-box-seam"></i>Products
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#orders">
                        <i class="bi bi-cart3"></i>Orders
                        <span class="badge bg-warning ms-auto">12</span>
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#analytics">
                        <i class="bi bi-graph-up"></i>Analytics
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#reports">
                        <i class="bi bi-file-earmark-text"></i>Reports
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#settings">
                        <i class="bi bi-gear"></i>Settings
                    </a>
                </li>
                <li><hr class="border-secondary"></li>
                <li class="nav-item">
                    <a class="nav-link" href="#help">
                        <i class="bi bi-question-circle"></i>Help & Support
                    </a>
                </li>
            </ul>
        </div>
    </nav>

    <!-- Main Content -->
    <main class="main-content">
        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h2 class="mb-1">Dashboard Overview</h2>
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb mb-0">
                        <li class="breadcrumb-item"><a href="#">Home</a></li>
                        <li class="breadcrumb-item active">Dashboard</li>
                    </ol>
                </nav>
            </div>
            <div class="d-flex gap-2">
                <button class="btn btn-outline-primary">
                    <i class="bi bi-download me-1"></i>Export
                </button>
                <button class="btn btn-primary">
                    <i class="bi bi-plus-lg me-1"></i>Add New
                </button>
            </div>
        </div>

        <!-- Statistics Cards -->
        <div class="row g-4 mb-4">
            <div class="col-md-6 col-xl-3">
                <div class="card stat-card border-0 shadow-sm" style="border-left-color: #0d6efd !important;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <p class="text-muted mb-1">Total Users</p>
                                <h3 class="mb-0">1,234</h3>
                                <small class="text-success"><i class="bi bi-arrow-up"></i> 12.5% from last month</small>
                            </div>
                            <div class="fs-1 text-primary">
                                <i class="bi bi-people"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-xl-3">
                <div class="card stat-card border-0 shadow-sm" style="border-left-color: #198754 !important;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <p class="text-muted mb-1">Total Revenue</p>
                                <h3 class="mb-0">$54,219</h3>
                                <small class="text-success"><i class="bi bi-arrow-up"></i> 8.2% from last month</small>
                            </div>
                            <div class="fs-1 text-success">
                                <i class="bi bi-currency-dollar"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-xl-3">
                <div class="card stat-card border-0 shadow-sm" style="border-left-color: #ffc107 !important;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <p class="text-muted mb-1">Pending Orders</p>
                                <h3 class="mb-0">567</h3>
                                <small class="text-warning"><i class="bi bi-dash"></i> 2.1% from last month</small>
                            </div>
                            <div class="fs-1 text-warning">
                                <i class="bi bi-cart3"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-xl-3">
                <div class="card stat-card border-0 shadow-sm" style="border-left-color: #dc3545 !important;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <p class="text-muted mb-1">Issues</p>
                                <h3 class="mb-0">23</h3>
                                <small class="text-danger"><i class="bi bi-arrow-down"></i> 5.4% from last month</small>
                            </div>
                            <div class="fs-1 text-danger">
                                <i class="bi bi-exclamation-triangle"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Charts Row -->
        <div class="row g-4 mb-4">
            <div class="col-lg-8">
                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white">
                        <h5 class="card-title mb-0">Revenue Overview</h5>
                    </div>
                    <div class="card-body">
                        <div class="bg-light rounded p-5 text-center">
                            <i class="bi bi-graph-up fs-1 text-muted mb-3"></i>
                            <p class="text-muted mb-0">Chart.js integration area</p>
                            <small class="text-muted">Add Chart.js library for interactive charts</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-header bg-white">
                        <h5 class="card-title mb-0">Traffic Sources</h5>
                    </div>
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <div>
                                <span class="badge bg-primary">Direct</span>
                                <p class="mb-0 mt-1">45.2%</p>
                            </div>
                            <div class="progress flex-grow-1 mx-3" style="height: 8px;">
                                <div class="progress-bar" style="width: 45.2%"></div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <div>
                                <span class="badge bg-success">Search</span>
                                <p class="mb-0 mt-1">32.8%</p>
                            </div>
                            <div class="progress flex-grow-1 mx-3" style="height: 8px;">
                                <div class="progress-bar bg-success" style="width: 32.8%"></div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <div>
                                <span class="badge bg-warning">Social</span>
                                <p class="mb-0 mt-1">15.5%</p>
                            </div>
                            <div class="progress flex-grow-1 mx-3" style="height: 8px;">
                                <div class="progress-bar bg-warning" style="width: 15.5%"></div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <span class="badge bg-danger">Other</span>
                                <p class="mb-0 mt-1">6.5%</p>
                            </div>
                            <div class="progress flex-grow-1 mx-3" style="height: 8px;">
                                <div class="progress-bar bg-danger" style="width: 6.5%"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Recent Activity & Users Table -->
        <div class="row g-4">
            <div class="col-lg-8">
                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white d-flex justify-content-between align-items-center">
                        <h5 class="card-title mb-0">Recent Users</h5>
                        <a href="#" class="btn btn-sm btn-outline-primary">View All</a>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <table class="table table-hover mb-0">
                                <thead class="table-light">
                                    <tr>
                                        <th>User</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                        <th>Status</th>
                                        <th>Registered</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <img src="https://via.placeholder.com/32" class="rounded-circle me-2" alt="User">
                                                <span>John Doe</span>
                                            </div>
                                        </td>
                                        <td>john@example.com</td>
                                        <td><span class="badge bg-primary">Admin</span></td>
                                        <td><span class="badge bg-success">Active</span></td>
                                        <td>2024-01-15</td>
                                        <td>
                                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-eye"></i></button>
                                            <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <img src="https://via.placeholder.com/32" class="rounded-circle me-2" alt="User">
                                                <span>Jane Smith</span>
                                            </div>
                                        </td>
                                        <td>jane@example.com</td>
                                        <td><span class="badge bg-info">User</span></td>
                                        <td><span class="badge bg-success">Active</span></td>
                                        <td>2024-02-20</td>
                                        <td>
                                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-eye"></i></button>
                                            <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <img src="https://via.placeholder.com/32" class="rounded-circle me-2" alt="User">
                                                <span>Mike Johnson</span>
                                            </div>
                                        </td>
                                        <td>mike@example.com</td>
                                        <td><span class="badge bg-warning">Moderator</span></td>
                                        <td><span class="badge bg-warning">Pending</span></td>
                                        <td>2024-03-10</td>
                                        <td>
                                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-eye"></i></button>
                                            <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <img src="https://via.placeholder.com/32" class="rounded-circle me-2" alt="User">
                                                <span>Sarah Wilson</span>
                                            </div>
                                        </td>
                                        <td>sarah@example.com</td>
                                        <td><span class="badge bg-info">User</span></td>
                                        <td><span class="badge bg-danger">Inactive</span></td>
                                        <td>2024-01-05</td>
                                        <td>
                                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-eye"></i></button>
                                            <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <img src="https://via.placeholder.com/32" class="rounded-circle me-2" alt="User">
                                                <span>Tom Brown</span>
                                            </div>
                                        </td>
                                        <td>tom@example.com</td>
                                        <td><span class="badge bg-info">User</span></td>
                                        <td><span class="badge bg-success">Active</span></td>
                                        <td>2024-04-01</td>
                                        <td>
                                            <button class="btn btn-sm btn-outline-primary"><i class="bi bi-eye"></i></button>
                                            <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                                            <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="card-footer bg-white">
                        <nav>
                            <ul class="pagination pagination-sm mb-0 justify-content-center">
                                <li class="page-item disabled"><a class="page-link" href="#">Previous</a></li>
                                <li class="page-item active"><a class="page-link" href="#">1</a></li>
                                <li class="page-item"><a class="page-link" href="#">2</a></li>
                                <li class="page-item"><a class="page-link" href="#">3</a></li>
                                <li class="page-item"><a class="page-link" href="#">Next</a></li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="card border-0 shadow-sm mb-4">
                    <div class="card-header bg-white">
                        <h5 class="card-title mb-0">Recent Activity</h5>
                    </div>
                    <div class="card-body">
                        <div class="d-flex mb-3">
                            <div class="flex-shrink-0">
                                <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                                    <i class="bi bi-person-plus"></i>
                                </div>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <p class="mb-1"><strong>New user registered</strong></p>
                                <small class="text-muted">5 minutes ago</small>
                            </div>
                        </div>
                        <div class="d-flex mb-3">
                            <div class="flex-shrink-0">
                                <div class="bg-success text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                                    <i class="bi bi-cart-check"></i>
                                </div>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <p class="mb-1"><strong>New order placed</strong></p>
                                <small class="text-muted">12 minutes ago</small>
                            </div>
                        </div>
                        <div class="d-flex mb-3">
                            <div class="flex-shrink-0">
                                <div class="bg-warning text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                                    <i class="bi bi-box-seam"></i>
                                </div>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <p class="mb-1"><strong>Product updated</strong></p>
                                <small class="text-muted">1 hour ago</small>
                            </div>
                        </div>
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <div class="bg-danger text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                                    <i class="bi bi-exclamation-triangle"></i>
                                </div>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <p class="mb-1"><strong>Server alert</strong></p>
                                <small class="text-muted">2 hours ago</small>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white">
                        <h5 class="card-title mb-0">Quick Actions</h5>
                    </div>
                    <div class="card-body">
                        <div class="d-grid gap-2">
                            <button class="btn btn-primary"><i class="bi bi-person-plus me-2"></i>Add User</button>
                            <button class="btn btn-success"><i class="bi bi-plus-lg me-2"></i>Create Product</button>
                            <button class="btn btn-info"><i class="bi bi-file-earmark-text me-2"></i>Generate Report</button>
                            <button class="btn btn-warning"><i class="bi bi-gear me-2"></i>System Settings</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Mobile Sidebar Toggle
        function toggleSidebar() {
            document.getElementById('sidebar').classList.toggle('show');
        }

        // Dark Mode Toggle
        document.getElementById('themeToggle').addEventListener('click', function(e) {
            e.preventDefault();
            const html = document.documentElement;
            const currentTheme = html.getAttribute('data-bs-theme');
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';
            html.setAttribute('data-bs-theme', newTheme);
            this.innerHTML = newTheme === 'light' 
                ? '<i class="bi bi-moon me-2"></i>Dark Mode' 
                : '<i class="bi bi-sun me-2"></i>Light Mode';
        });

        // Active Nav Link
        document.querySelectorAll('.sidebar .nav-link').forEach(link => {
            link.addEventListener('click', function() {
                document.querySelectorAll('.sidebar .nav-link').forEach(l => l.classList.remove('active'));
                this.classList.add('active');
            });
        });
    </script>
</body>
</html>
```

## 📝 Practice Exercises

1. **Add Chart.js**: Integrate Chart.js for data visualization
2. **Real-Time Updates**: Add WebSocket for live data
3. **Advanced Filtering**: Add table sorting and filtering
4. **Export Functionality**: Implement CSV/PDF export
5. **User Roles**: Create role-based access control
6. **Settings Panel**: Build complete settings interface

## ✅ Completion Checklist

- [ ] Fixed sidebar navigation
- [ ] Top navigation with notifications
- [ ] Statistics cards with icons
- [ ] Data table with pagination
- [ ] Charts section
- [ ] Activity feed
- [ ] Dark mode toggle
- [ ] Responsive mobile menu
- [ ] Breadcrumb navigation
- [ ] Quick actions panel

**Next: Day 45 - Portfolio Website** 💼
