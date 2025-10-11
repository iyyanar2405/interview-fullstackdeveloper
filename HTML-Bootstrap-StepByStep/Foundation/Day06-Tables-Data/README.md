# Day 06: Tables & Data Display 📊

Welcome to Day 6! Today we'll master HTML tables for displaying structured data effectively. You'll learn to create accessible, semantic tables that present information clearly and work well across all devices.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Create semantic and accessible table structures
- Use table headers, captions, and grouping elements
- Implement responsive table design patterns
- Apply table accessibility best practices
- Build complex data tables with sorting and filtering
- Understand when to use tables vs other layouts
- Create tables for different data types and use cases

## 📚 Lesson Content

### Table Basics

Tables display data in rows and columns. They should only be used for tabular data, not for page layout.

#### Basic Table Structure
```html
<table>
    <caption>Monthly Sales Report - Q1 2025</caption>
    
    <thead>
        <tr>
            <th scope="col">Month</th>
            <th scope="col">Sales</th>
            <th scope="col">Target</th>
            <th scope="col">Variance</th>
        </tr>
    </thead>
    
    <tbody>
        <tr>
            <th scope="row">January</th>
            <td>$45,000</td>
            <td>$40,000</td>
            <td>+$5,000</td>
        </tr>
        <tr>
            <th scope="row">February</th>
            <td>$52,000</td>
            <td>$45,000</td>
            <td>+$7,000</td>
        </tr>
        <tr>
            <th scope="row">March</th>
            <td>$48,000</td>
            <td>$50,000</td>
            <td>-$2,000</td>
        </tr>
    </tbody>
    
    <tfoot>
        <tr>
            <th scope="row">Total</th>
            <td>$145,000</td>
            <td>$135,000</td>
            <td>+$10,000</td>
        </tr>
    </tfoot>
</table>
```

**Essential Table Elements:**
- `<table>`: Container for entire table
- `<caption>`: Table title/description
- `<thead>`: Groups header content
- `<tbody>`: Groups body content
- `<tfoot>`: Groups footer content
- `<tr>`: Table row
- `<th>`: Header cell
- `<td>`: Data cell

### Table Headers and Scope

Proper headers make tables accessible to screen readers:

#### Column and Row Headers
```html
<table>
    <caption>Product Comparison Chart</caption>
    
    <thead>
        <tr>
            <th scope="col">Feature</th>
            <th scope="col">Basic Plan</th>
            <th scope="col">Pro Plan</th>
            <th scope="col">Enterprise Plan</th>
        </tr>
    </thead>
    
    <tbody>
        <tr>
            <th scope="row">Storage</th>
            <td>10 GB</td>
            <td>100 GB</td>
            <td>Unlimited</td>
        </tr>
        <tr>
            <th scope="row">Users</th>
            <td>1</td>
            <td>5</td>
            <td>Unlimited</td>
        </tr>
        <tr>
            <th scope="row">Support</th>
            <td>Email</td>
            <td>Email + Chat</td>
            <td>24/7 Phone</td>
        </tr>
        <tr>
            <th scope="row">Price/Month</th>
            <td>$9.99</td>
            <td>$19.99</td>
            <td>$49.99</td>
        </tr>
    </tbody>
</table>
```

#### Complex Headers with Grouping
```html
<table>
    <caption>Quarterly Financial Results by Region</caption>
    
    <thead>
        <tr>
            <th rowspan="2" scope="col">Region</th>
            <th colspan="3" scope="colgroup">Q1 2025</th>
            <th colspan="3" scope="colgroup">Q2 2025</th>
        </tr>
        <tr>
            <th scope="col">Revenue</th>
            <th scope="col">Expenses</th>
            <th scope="col">Profit</th>
            <th scope="col">Revenue</th>
            <th scope="col">Expenses</th>
            <th scope="col">Profit</th>
        </tr>
    </thead>
    
    <tbody>
        <tr>
            <th scope="row">North America</th>
            <td>$250,000</td>
            <td>$180,000</td>
            <td>$70,000</td>
            <td>$280,000</td>
            <td>$190,000</td>
            <td>$90,000</td>
        </tr>
        <tr>
            <th scope="row">Europe</th>
            <td>$180,000</td>
            <td>$140,000</td>
            <td>$40,000</td>
            <td>$220,000</td>
            <td>$150,000</td>
            <td>$70,000</td>
        </tr>
        <tr>
            <th scope="row">Asia Pacific</th>
            <td>$120,000</td>
            <td>$95,000</td>
            <td>$25,000</td>
            <td>$145,000</td>
            <td>$105,000</td>
            <td>$40,000</td>
        </tr>
    </tbody>
    
    <tfoot>
        <tr>
            <th scope="row">Total</th>
            <td>$550,000</td>
            <td>$415,000</td>
            <td>$135,000</td>
            <td>$645,000</td>
            <td>$445,000</td>
            <td>$200,000</td>
        </tr>
    </tfoot>
</table>
```

### Advanced Table Features

#### Cell Spanning
```html
<table>
    <caption>Employee Schedule - Week of January 15, 2025</caption>
    
    <thead>
        <tr>
            <th scope="col">Employee</th>
            <th scope="col">Monday</th>
            <th scope="col">Tuesday</th>
            <th scope="col">Wednesday</th>
            <th scope="col">Thursday</th>
            <th scope="col">Friday</th>
        </tr>
    </thead>
    
    <tbody>
        <tr>
            <th scope="row">Sarah Johnson</th>
            <td>9:00-17:00</td>
            <td>9:00-17:00</td>
            <td colspan="2">Conference (9:00-17:00)</td>
            <td>9:00-17:00</td>
        </tr>
        <tr>
            <th scope="row">Mike Chen</th>
            <td>10:00-18:00</td>
            <td>10:00-18:00</td>
            <td>10:00-18:00</td>
            <td>10:00-18:00</td>
            <td rowspan="2">Team Meeting<br>(14:00-16:00)</td>
        </tr>
        <tr>
            <th scope="row">Emma Davis</th>
            <td>8:00-16:00</td>
            <td>8:00-16:00</td>
            <td>8:00-16:00</td>
            <td>8:00-16:00</td>
        </tr>
    </tbody>
</table>
```

#### Table with Data Attributes
```html
<table data-sortable="true" data-filterable="true">
    <caption>
        Customer Database
        <details>
            <summary>Table Description</summary>
            <p>This table shows customer information including contact details, 
               registration date, and status. Use the sort arrows to order data 
               or the filter boxes to search.</p>
        </details>
    </caption>
    
    <thead>
        <tr>
            <th scope="col" data-sort="string">
                Customer Name
                <button type="button" aria-label="Sort by name">⇅</button>
            </th>
            <th scope="col" data-sort="email">
                Email
                <button type="button" aria-label="Sort by email">⇅</button>
            </th>
            <th scope="col" data-sort="date">
                Registration Date
                <button type="button" aria-label="Sort by date">⇅</button>
            </th>
            <th scope="col" data-sort="string">
                Status
                <button type="button" aria-label="Sort by status">⇅</button>
            </th>
            <th scope="col">Actions</th>
        </tr>
    </thead>
    
    <tbody>
        <tr data-status="active">
            <td>
                <strong>John Smith</strong><br>
                <small>ID: CU001</small>
            </td>
            <td>
                <a href="mailto:john.smith@email.com">john.smith@email.com</a>
            </td>
            <td data-sort-value="2024-03-15">March 15, 2024</td>
            <td>
                <span class="status-badge status-active">Active</span>
            </td>
            <td>
                <button type="button">Edit</button>
                <button type="button">Delete</button>
            </td>
        </tr>
        <tr data-status="pending">
            <td>
                <strong>Sarah Johnson</strong><br>
                <small>ID: CU002</small>
            </td>
            <td>
                <a href="mailto:sarah.j@email.com">sarah.j@email.com</a>
            </td>
            <td data-sort-value="2024-11-20">November 20, 2024</td>
            <td>
                <span class="status-badge status-pending">Pending</span>
            </td>
            <td>
                <button type="button">Edit</button>
                <button type="button">Activate</button>
            </td>
        </tr>
        <tr data-status="inactive">
            <td>
                <strong>Mike Wilson</strong><br>
                <small>ID: CU003</small>
            </td>
            <td>
                <a href="mailto:m.wilson@email.com">m.wilson@email.com</a>
            </td>
            <td data-sort-value="2023-08-10">August 10, 2023</td>
            <td>
                <span class="status-badge status-inactive">Inactive</span>
            </td>
            <td>
                <button type="button">Edit</button>
                <button type="button">Reactivate</button>
            </td>
        </tr>
    </tbody>
</table>
```

## 💻 Hands-On Practice

### Exercise 1: E-commerce Product Comparison Table

Create `product-comparison.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>TechStore - Laptop Comparison</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            line-height: 1.6;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        caption {
            font-size: 1.5em;
            font-weight: bold;
            margin-bottom: 15px;
            text-align: left;
            color: #333;
        }
        
        th, td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }
        
        th {
            background-color: #f8f9fa;
            font-weight: bold;
            color: #333;
            position: sticky;
            top: 0;
        }
        
        tbody tr:hover {
            background-color: #f5f5f5;
        }
        
        .feature-category {
            background-color: #e9ecef;
            font-weight: bold;
            color: #495057;
        }
        
        .price {
            font-size: 1.2em;
            font-weight: bold;
            color: #28a745;
        }
        
        .spec-highlight {
            background-color: #fff3cd;
            font-weight: bold;
        }
        
        .unavailable {
            color: #6c757d;
            font-style: italic;
        }
        
        .rating {
            color: #ffc107;
        }
        
        .product-image {
            width: 80px;
            height: 60px;
            object-fit: cover;
            border-radius: 4px;
        }
        
        .action-button {
            background: #007bff;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            font-size: 14px;
        }
        
        .action-button:hover {
            background: #0056b3;
        }
        
        .action-button.secondary {
            background: #6c757d;
        }
        
        .action-button.secondary:hover {
            background: #545b62;
        }
        
        /* Responsive table */
        @media (max-width: 768px) {
            .table-container {
                overflow-x: auto;
            }
            
            table {
                min-width: 800px;
            }
            
            th, td {
                padding: 8px;
                font-size: 14px;
            }
        }
        
        .table-description {
            background: #e7f3ff;
            padding: 15px;
            border-radius: 4px;
            margin-bottom: 20px;
        }
        
        .filter-controls {
            margin: 20px 0;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 4px;
        }
        
        .filter-controls label {
            margin-right: 15px;
        }
        
        .filter-controls select,
        .filter-controls input {
            margin-right: 15px;
            padding: 5px;
        }
    </style>
</head>
<body>
    <header>
        <h1>🛒 TechStore - Laptop Comparison</h1>
        <p>Compare features, specifications, and prices to find your perfect laptop.</p>
    </header>
    
    <main>
        <div class="table-description">
            <h2>Product Comparison Guide</h2>
            <p><strong>How to use this table:</strong></p>
            <ul>
                <li>Compare up to 4 laptop models side by side</li>
                <li>Scroll horizontally on mobile devices to see all columns</li>
                <li>Highlighted cells show standout features</li>
                <li>Click "View Details" for complete specifications</li>
                <li>Use "Add to Cart" to purchase directly</li>
            </ul>
        </div>
        
        <div class="filter-controls">
            <label for="price-filter">Filter by Max Price:</label>
            <select id="price-filter">
                <option value="">All Prices</option>
                <option value="1000">Under $1,000</option>
                <option value="1500">Under $1,500</option>
                <option value="2000">Under $2,000</option>
                <option value="2500">Under $2,500</option>
            </select>
            
            <label for="brand-filter">Filter by Brand:</label>
            <select id="brand-filter">
                <option value="">All Brands</option>
                <option value="apple">Apple</option>
                <option value="dell">Dell</option>
                <option value="hp">HP</option>
                <option value="lenovo">Lenovo</option>
            </select>
            
            <button type="button" onclick="resetFilters()">Reset Filters</button>
        </div>
        
        <div class="table-container">
            <table id="comparison-table">
                <caption>
                    Laptop Comparison Chart - Updated January 2025
                    <details>
                        <summary>Detailed Table Information</summary>
                        <p>This comprehensive comparison includes 4 popular laptop models 
                           across different price ranges. All specifications are current 
                           as of January 2025 and prices are in USD before taxes.</p>
                    </details>
                </caption>
                
                <thead>
                    <tr>
                        <th scope="col">Feature</th>
                        <th scope="col">
                            Apple MacBook Pro 14"<br>
                            <small>Model: MBP14-M3-2024</small>
                        </th>
                        <th scope="col">
                            Dell XPS 13 Plus<br>
                            <small>Model: XPS13-9320</small>
                        </th>
                        <th scope="col">
                            HP Spectre x360 14<br>
                            <small>Model: HP-SPC-X360-14</small>
                        </th>
                        <th scope="col">
                            Lenovo ThinkPad X1 Carbon<br>
                            <small>Model: TP-X1C-G11</small>
                        </th>
                    </tr>
                </thead>
                
                <tbody>
                    <!-- Product Images -->
                    <tr>
                        <th scope="row">Product Image</th>
                        <td>
                            <img src="macbook-pro-14.jpg" 
                                 alt="Apple MacBook Pro 14 inch in Space Gray" 
                                 class="product-image">
                        </td>
                        <td>
                            <img src="dell-xps-13.jpg" 
                                 alt="Dell XPS 13 Plus in Platinum Silver" 
                                 class="product-image">
                        </td>
                        <td>
                            <img src="hp-spectre-x360.jpg" 
                                 alt="HP Spectre x360 14 in Nightfall Black" 
                                 class="product-image">
                        </td>
                        <td>
                            <img src="lenovo-thinkpad-x1.jpg" 
                                 alt="Lenovo ThinkPad X1 Carbon in Carbon Black" 
                                 class="product-image">
                        </td>
                    </tr>
                    
                    <!-- Pricing Information -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">💰 Pricing & Availability</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Starting Price</th>
                        <td class="price" data-price="1999">$1,999</td>
                        <td class="price" data-price="1299">$1,299</td>
                        <td class="price" data-price="1349">$1,349</td>
                        <td class="price" data-price="1479">$1,479</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Configuration Price</th>
                        <td class="price" data-price="2499">$2,499</td>
                        <td class="price" data-price="1899">$1,899</td>
                        <td class="price" data-price="1799">$1,799</td>
                        <td class="price" data-price="2149">$2,149</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Availability</th>
                        <td>✅ In Stock</td>
                        <td>✅ In Stock</td>
                        <td>⚠️ Limited Stock</td>
                        <td>✅ In Stock</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Warranty</th>
                        <td>1 Year Limited</td>
                        <td>1 Year Premium</td>
                        <td>1 Year Limited</td>
                        <td>3 Year On-Site</td>
                    </tr>
                    
                    <!-- Performance Specifications -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">⚡ Performance Specifications</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Processor</th>
                        <td class="spec-highlight">Apple M3 Pro<br>12-core CPU</td>
                        <td>Intel Core i7-1360P<br>12th Gen</td>
                        <td>Intel Core i7-1255U<br>12th Gen</td>
                        <td>Intel Core i7-1365U<br>13th Gen</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Graphics</th>
                        <td class="spec-highlight">Apple M3 Pro GPU<br>18-core</td>
                        <td>Intel Iris Xe</td>
                        <td>Intel Iris Xe</td>
                        <td>Intel Iris Xe</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">RAM</th>
                        <td>18 GB Unified Memory</td>
                        <td class="spec-highlight">32 GB LPDDR5</td>
                        <td>16 GB LPDDR4x</td>
                        <td>16 GB LPDDR5</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Storage</th>
                        <td>512 GB SSD</td>
                        <td class="spec-highlight">1 TB PCIe SSD</td>
                        <td>512 GB PCIe SSD</td>
                        <td>512 GB PCIe SSD</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Performance Score</th>
                        <td class="spec-highlight">95/100</td>
                        <td>87/100</td>
                        <td>82/100</td>
                        <td>85/100</td>
                    </tr>
                    
                    <!-- Display & Design -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">🖥️ Display & Design</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Screen Size</th>
                        <td>14.2" Liquid Retina XDR</td>
                        <td>13.4" InfinityEdge</td>
                        <td>13.5" OLED Touch</td>
                        <td>14" WUXGA</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Resolution</th>
                        <td class="spec-highlight">3024 x 1964<br>254 PPI</td>
                        <td>1920 x 1200<br>169 PPI</td>
                        <td class="spec-highlight">1920 x 1280<br>201 PPI</td>
                        <td>1920 x 1200<br>157 PPI</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Display Features</th>
                        <td>
                            ✅ HDR10<br>
                            ✅ P3 Wide Color<br>
                            ✅ True Tone
                        </td>
                        <td>
                            ✅ Anti-glare<br>
                            ✅ 100% sRGB<br>
                            ❌ Touch
                        </td>
                        <td>
                            ✅ OLED<br>
                            ✅ Touch Screen<br>
                            ✅ 400 nits brightness
                        </td>
                        <td>
                            ✅ Anti-glare<br>
                            ✅ Low Blue Light<br>
                            ❌ Touch
                        </td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Weight</th>
                        <td>3.5 lbs (1.6 kg)</td>
                        <td class="spec-highlight">2.73 lbs (1.24 kg)</td>
                        <td>3.0 lbs (1.36 kg)</td>
                        <td>2.48 lbs (1.12 kg)</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Thickness</th>
                        <td>0.61" (15.5 mm)</td>
                        <td class="spec-highlight">0.57" (14.5 mm)</td>
                        <td>0.67" (17 mm)</td>
                        <td>0.57" (14.5 mm)</td>
                    </tr>
                    
                    <!-- Connectivity & Ports -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">🔌 Connectivity & Ports</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">USB Ports</th>
                        <td>
                            3x Thunderbolt 4<br>
                            (USB-C)
                        </td>
                        <td>
                            2x Thunderbolt 4<br>
                            (USB-C)
                        </td>
                        <td>
                            2x Thunderbolt 4<br>
                            1x USB-A 3.2
                        </td>
                        <td class="spec-highlight">
                            2x Thunderbolt 4<br>
                            2x USB-A 3.2
                        </td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Other Ports</th>
                        <td>
                            ✅ HDMI 2.1<br>
                            ✅ SD Card Slot<br>
                            ✅ MagSafe 3
                        </td>
                        <td>
                            ❌ HDMI<br>
                            ❌ SD Card<br>
                            ❌ Audio Jack
                        </td>
                        <td>
                            ❌ HDMI<br>
                            ✅ MicroSD<br>
                            ✅ Audio Jack
                        </td>
                        <td>
                            ✅ HDMI 2.0b<br>
                            ❌ SD Card<br>
                            ✅ Audio Jack
                        </td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Wireless</th>
                        <td>
                            Wi-Fi 6E<br>
                            Bluetooth 5.3
                        </td>
                        <td>
                            Wi-Fi 6E<br>
                            Bluetooth 5.2
                        </td>
                        <td>
                            Wi-Fi 6E<br>
                            Bluetooth 5.3
                        </td>
                        <td class="spec-highlight">
                            Wi-Fi 6E<br>
                            Bluetooth 5.1<br>
                            Optional 5G
                        </td>
                    </tr>
                    
                    <!-- Battery & Features -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">🔋 Battery & Special Features</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Battery Life</th>
                        <td class="spec-highlight">18 hours</td>
                        <td>13 hours</td>
                        <td>16 hours</td>
                        <td>15 hours</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Charging</th>
                        <td>96W USB-C<br>MagSafe 3</td>
                        <td>65W USB-C</td>
                        <td>65W USB-C</td>
                        <td>65W USB-C</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Security Features</th>
                        <td>
                            ✅ Touch ID<br>
                            ✅ T2 Security Chip<br>
                            ✅ Secure Boot
                        </td>
                        <td>
                            ✅ Fingerprint Reader<br>
                            ✅ TPM 2.0<br>
                            ✅ Windows Hello
                        </td>
                        <td>
                            ✅ Fingerprint Reader<br>
                            ✅ TPM 2.0<br>
                            ✅ IR Camera
                        </td>
                        <td class="spec-highlight">
                            ✅ Fingerprint Reader<br>
                            ✅ TPM 2.0<br>
                            ✅ IR Camera<br>
                            ✅ Privacy Shutter
                        </td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Keyboard</th>
                        <td>Magic Keyboard<br>with Touch Bar</td>
                        <td>Backlit Keyboard<br>Function Keys</td>
                        <td>Backlit Keyboard<br>360° Convertible</td>
                        <td class="spec-highlight">ThinkPad Keyboard<br>with TrackPoint</td>
                    </tr>
                    
                    <!-- Ratings & Reviews -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">⭐ Ratings & Reviews</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Overall Rating</th>
                        <td class="rating">★★★★★ 4.8/5</td>
                        <td class="rating">★★★★☆ 4.3/5</td>
                        <td class="rating">★★★★☆ 4.1/5</td>
                        <td class="rating">★★★★☆ 4.5/5</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Customer Reviews</th>
                        <td>2,847 reviews</td>
                        <td>1,923 reviews</td>
                        <td>756 reviews</td>
                        <td>1,432 reviews</td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Pros</th>
                        <td>
                            • Excellent performance<br>
                            • Amazing display<br>
                            • Long battery life
                        </td>
                        <td>
                            • Compact design<br>
                            • Good performance<br>
                            • Premium build
                        </td>
                        <td>
                            • OLED display<br>
                            • 2-in-1 design<br>
                            • Good battery
                        </td>
                        <td>
                            • Business focused<br>
                            • Excellent keyboard<br>
                            • Reliable build
                        </td>
                    </tr>
                    
                    <tr>
                        <th scope="row">Cons</th>
                        <td>
                            • Expensive<br>
                            • Limited ports<br>
                            • macOS only
                        </td>
                        <td>
                            • Few ports<br>
                            • No touchscreen<br>
                            • Premium price
                        </td>
                        <td>
                            • Average performance<br>
                            • Heavy for size<br>
                            • Limited ports
                        </td>
                        <td>
                            • Conservative design<br>
                            • Display quality<br>
                            • Premium price
                        </td>
                    </tr>
                    
                    <!-- Actions -->
                    <tr class="feature-category">
                        <th scope="row" colspan="5">🛍️ Purchase Options</th>
                    </tr>
                    
                    <tr>
                        <th scope="row">Actions</th>
                        <td>
                            <a href="/laptops/macbook-pro-14" class="action-button">
                                View Details
                            </a><br><br>
                            <a href="/cart/add/mbp14" class="action-button">
                                Add to Cart
                            </a>
                        </td>
                        <td>
                            <a href="/laptops/dell-xps-13" class="action-button">
                                View Details
                            </a><br><br>
                            <a href="/cart/add/xps13" class="action-button">
                                Add to Cart
                            </a>
                        </td>
                        <td>
                            <a href="/laptops/hp-spectre-x360" class="action-button">
                                View Details
                            </a><br><br>
                            <a href="/cart/add/spectre" class="action-button secondary">
                                Limited Stock
                            </a>
                        </td>
                        <td>
                            <a href="/laptops/lenovo-thinkpad-x1" class="action-button">
                                View Details
                            </a><br><br>
                            <a href="/cart/add/thinkpad" class="action-button">
                                Add to Cart
                            </a>
                        </td>
                    </tr>
                </tbody>
                
                <tfoot>
                    <tr>
                        <td colspan="5">
                            <small>
                                <strong>Last Updated:</strong> January 15, 2025 | 
                                <strong>Prices:</strong> USD, excluding taxes and shipping | 
                                <strong>Availability:</strong> Subject to change
                            </small>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
        
        <!-- Additional Information -->
        <section>
            <h2>Shopping Guidelines</h2>
            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 20px; margin: 20px 0;">
                <div style="padding: 20px; background: #f8f9fa; border-radius: 8px;">
                    <h3>🎯 For Students</h3>
                    <p>Consider the <strong>Dell XPS 13 Plus</strong> for its balance of performance and portability, or check student discounts on all models.</p>
                </div>
                
                <div style="padding: 20px; background: #f8f9fa; border-radius: 8px;">
                    <h3>💼 For Business</h3>
                    <p>The <strong>Lenovo ThinkPad X1 Carbon</strong> offers enterprise features, durability, and excellent support options.</p>
                </div>
                
                <div style="padding: 20px; background: #f8f9fa; border-radius: 8px;">
                    <h3>🎨 For Creatives</h3>
                    <p>Choose the <strong>MacBook Pro 14"</strong> for video editing and design work, or the <strong>HP Spectre x360</strong> for versatility.</p>
                </div>
                
                <div style="padding: 20px; background: #f8f9fa; border-radius: 8px;">
                    <h3>💰 Best Value</h3>
                    <p>The <strong>HP Spectre x360 14</strong> offers premium features at a competitive price point with 2-in-1 functionality.</p>
                </div>
            </div>
        </section>
    </main>
    
    <footer>
        <p>Need help choosing? Contact our experts at <a href="tel:+15551234567">(555) 123-4567</a> 
           or <a href="mailto:help@techstore.com">help@techstore.com</a></p>
        <p><small>&copy; 2025 TechStore. All prices and specifications subject to change without notice.</small></p>
    </footer>
    
    <script>
        // Simple table filtering functionality
        function resetFilters() {
            document.getElementById('price-filter').value = '';
            document.getElementById('brand-filter').value = '';
            filterTable();
        }
        
        function filterTable() {
            const priceFilter = document.getElementById('price-filter').value;
            const brandFilter = document.getElementById('brand-filter').value.toLowerCase();
            const table = document.getElementById('comparison-table');
            const headerCells = table.querySelectorAll('thead th');
            
            // Simple example - in real app, this would be more sophisticated
            console.log('Filtering by price:', priceFilter, 'and brand:', brandFilter);
        }
        
        // Add event listeners
        document.getElementById('price-filter').addEventListener('change', filterTable);
        document.getElementById('brand-filter').addEventListener('change', filterTable);
        
        // Accessibility: Add keyboard navigation for table
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Tab' && e.target.tagName === 'TD') {
                // Custom tab behavior could be added here
            }
        });
    </script>
</body>
</html>
```

## 📋 Table Accessibility Best Practices

### 1. Proper Header Association
```html
<!-- Use scope attribute for simple tables -->
<th scope="col">Product Name</th>
<th scope="row">Price</th>

<!-- Use headers attribute for complex tables -->
<th id="q1-revenue" scope="col">Q1 Revenue</th>
<td headers="north-america q1-revenue">$250,000</td>
```

### 2. Table Descriptions
```html
<table>
    <caption>
        Sales Data for 2025
        <details>
            <summary>Table Description</summary>
            <p>This table shows quarterly sales data by region, 
               including revenue, expenses, and profit margins.</p>
        </details>
    </caption>
    <!-- table content -->
</table>
```

### 3. Responsive Table Patterns
```html
<!-- Option 1: Horizontal scroll -->
<div class="table-container" style="overflow-x: auto;">
    <table style="min-width: 600px;">
        <!-- table content -->
    </table>
</div>

<!-- Option 2: Stacked layout for mobile -->
<style>
@media (max-width: 768px) {
    .responsive-table {
        display: block;
    }
    
    .responsive-table thead {
        display: none;
    }
    
    .responsive-table tr {
        display: block;
        border: 1px solid #ccc;
        margin-bottom: 10px;
    }
    
    .responsive-table td {
        display: block;
        text-align: right;
        border: none;
        padding: 10px;
    }
    
    .responsive-table td:before {
        content: attr(data-label) ": ";
        float: left;
        font-weight: bold;
    }
}
</style>

<table class="responsive-table">
    <thead>
        <tr>
            <th>Product</th>
            <th>Price</th>
            <th>Stock</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td data-label="Product">Laptop</td>
            <td data-label="Price">$999</td>
            <td data-label="Stock">25</td>
        </tr>
    </tbody>
</table>
```

## 🎉 Day 6 Checklist

- [ ] Created comprehensive product comparison table
- [ ] Implemented proper table structure with thead, tbody, tfoot
- [ ] Used appropriate th elements with scope attributes
- [ ] Added table caption and description
- [ ] Applied responsive table design patterns
- [ ] Implemented table accessibility best practices
- [ ] Added interactive elements and filtering
- [ ] Created complex table with merged cells and grouping

## 🔗 Additional Resources

- **[MDN Tables Guide](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/table)** - Complete table documentation
- **[WebAIM Table Accessibility](https://webaim.org/techniques/tables/)** - Table accessibility guidelines
- **[Responsive Table Patterns](https://css-tricks.com/responsive-data-tables/)** - Various responsive approaches
- **[Table Design Inspiration](https://codepen.io/collection/AKkZro)** - Creative table designs

---

## 🚀 What's Next?

Tomorrow in **Day 07: HTML5 Semantic Elements & Accessibility**, you'll master:
- Advanced semantic HTML5 elements
- ARIA attributes and accessibility best practices
- Screen reader optimization
- Building fully accessible web pages

**Outstanding work on Day 6!** You've learned to create complex, accessible tables that present data clearly and work well for all users.

---

**Remember: Great tables organize information clearly and remain accessible to everyone!** 📊✨