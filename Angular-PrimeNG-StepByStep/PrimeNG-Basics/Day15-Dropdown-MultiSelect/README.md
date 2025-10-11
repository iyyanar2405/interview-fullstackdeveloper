# Day 15: Dropdown & MultiSelect 📋

## 📋 Learning Objectives
- Master Dropdown component for single selections
- Use MultiSelect for multiple selections
- Implement filtering and search
- Apply virtual scrolling for large datasets
- Create custom templates
- Handle grouping and disabled options

---

## 📋 Dropdown Component

### Installation
```typescript
import { DropdownModule } from 'primeng/dropdown';
```

### Basic Dropdown
```html
<!-- Basic dropdown -->
<p-dropdown 
  [options]="cities"
  [(ngModel)]="selectedCity"
  placeholder="Select a City"
  optionLabel="name">
</p-dropdown>

<p>Selected: {{ selectedCity | json }}</p>
```

```typescript
export class DropdownDemoComponent {
  cities = [
    { name: 'New York', code: 'NY' },
    { name: 'Rome', code: 'RM' },
    { name: 'London', code: 'LDN' },
    { name: 'Istanbul', code: 'IST' },
    { name: 'Paris', code: 'PRS' }
  ];
  
  selectedCity: any;
}
```

### Dropdown with Filtering
```html
<p-dropdown 
  [options]="cities"
  [(ngModel)]="selectedCity"
  [filter]="true"
  filterBy="name"
  placeholder="Search and select"
  optionLabel="name">
</p-dropdown>
```

### Grouped Dropdown
```html
<p-dropdown 
  [options]="groupedCities"
  [(ngModel)]="selectedCity"
  placeholder="Select a City"
  [group]="true"
  optionLabel="label">
  <ng-template let-group pTemplate="group">
    <div class="flex align-items-center">
      <span>{{ group.label }}</span>
    </div>
  </ng-template>
</p-dropdown>
```

```typescript
groupedCities = [
  {
    label: 'USA',
    items: [
      { label: 'New York', value: 'NY' },
      { label: 'Los Angeles', value: 'LA' },
      { label: 'Chicago', value: 'CHI' }
    ]
  },
  {
    label: 'Europe',
    items: [
      { label: 'London', value: 'LDN' },
      { label: 'Paris', value: 'PRS' },
      { label: 'Rome', value: 'RM' }
    ]
  }
];
```

### Custom Template
```html
<p-dropdown 
  [options]="countries"
  [(ngModel)]="selectedCountry"
  optionLabel="name"
  [filter]="true">
  <ng-template pTemplate="selectedItem">
    <div class="flex align-items-center gap-2" *ngIf="selectedCountry">
      <img [src]="'assets/flags/' + selectedCountry.code + '.png'" 
           style="width: 20px" />
      <div>{{ selectedCountry.name }}</div>
    </div>
  </ng-template>
  <ng-template let-country pTemplate="item">
    <div class="flex align-items-center gap-2">
      <img [src]="'assets/flags/' + country.code + '.png'" 
           style="width: 20px" />
      <div>{{ country.name }}</div>
    </div>
  </ng-template>
</p-dropdown>
```

### Dropdown with Icons
```html
<p-dropdown 
  [options]="statuses"
  [(ngModel)]="selectedStatus"
  optionLabel="label"
  placeholder="Select Status">
  <ng-template let-status pTemplate="item">
    <div class="flex align-items-center gap-2">
      <i [class]="status.icon" [style.color]="status.color"></i>
      <span>{{ status.label }}</span>
    </div>
  </ng-template>
</p-dropdown>
```

```typescript
statuses = [
  { label: 'Active', icon: 'pi pi-check-circle', color: 'green', value: 'active' },
  { label: 'Inactive', icon: 'pi pi-times-circle', color: 'red', value: 'inactive' },
  { label: 'Pending', icon: 'pi pi-clock', color: 'orange', value: 'pending' }
];
```

---

## 🎯 MultiSelect Component

### Installation
```typescript
import { MultiSelectModule } from 'primeng/multiselect';
```

### Basic MultiSelect
```html
<p-multiSelect 
  [options]="cities"
  [(ngModel)]="selectedCities"
  optionLabel="name"
  placeholder="Select Cities">
</p-multiSelect>

<p>Selected: {{ selectedCities | json }}</p>
```

```typescript
export class MultiSelectDemoComponent {
  cities = [
    { name: 'New York', code: 'NY' },
    { name: 'Rome', code: 'RM' },
    { name: 'London', code: 'LDN' },
    { name: 'Istanbul', code: 'IST' },
    { name: 'Paris', code: 'PRS' }
  ];
  
  selectedCities: any[] = [];
}
```

### MultiSelect with Filtering
```html
<p-multiSelect 
  [options]="cities"
  [(ngModel)]="selectedCities"
  [filter]="true"
  filterBy="name"
  placeholder="Search cities"
  optionLabel="name">
</p-multiSelect>
```

### MultiSelect with Select All
```html
<p-multiSelect 
  [options]="cities"
  [(ngModel)]="selectedCities"
  [filter]="true"
  [showToggleAll]="true"
  [selectAll]="selectAll"
  placeholder="Select Cities"
  optionLabel="name"
  (onSelectAllChange)="onSelectAllChange($event)">
</p-multiSelect>
```

### Display as Chips
```html
<p-multiSelect 
  [options]="cities"
  [(ngModel)]="selectedCities"
  display="chip"
  placeholder="Select Cities"
  optionLabel="name">
</p-multiSelect>
```

### Grouped MultiSelect
```html
<p-multiSelect 
  [options]="groupedCities"
  [(ngModel)]="selectedCities"
  [group]="true"
  placeholder="Select Cities"
  optionLabel="label"
  [showToggleAll]="false">
  <ng-template let-group pTemplate="group">
    <div class="flex align-items-center">
      <span class="font-bold">{{ group.label }}</span>
    </div>
  </ng-template>
</p-multiSelect>
```

### Virtual Scrolling
```html
<p-multiSelect 
  [options]="largeDataset"
  [(ngModel)]="selectedItems"
  [virtualScroll]="true"
  [virtualScrollItemSize]="38"
  placeholder="Select Items"
  optionLabel="name">
</p-multiSelect>
```

```typescript
largeDataset: any[] = [];

ngOnInit() {
  // Generate 10000 items
  for (let i = 0; i < 10000; i++) {
    this.largeDataset.push({
      name: `Item ${i}`,
      code: `item-${i}`
    });
  }
}
```

---

## 🎯 Practical Exercise: Advanced Search Form

```typescript
export class AdvancedSearchComponent implements OnInit {
  searchForm!: FormGroup;
  
  countries = [
    { name: 'USA', code: 'US' },
    { name: 'UK', code: 'GB' },
    { name: 'Canada', code: 'CA' },
    { name: 'Australia', code: 'AU' },
    { name: 'Germany', code: 'DE' },
    { name: 'France', code: 'FR' }
  ];
  
  cities: any[] = [];
  
  categories = [
    { label: 'Electronics', value: 'electronics' },
    { label: 'Clothing', value: 'clothing' },
    { label: 'Books', value: 'books' },
    { label: 'Home & Garden', value: 'home' },
    { label: 'Sports', value: 'sports' },
    { label: 'Toys', value: 'toys' }
  ];
  
  priceRanges = [
    { label: 'Under $25', value: '0-25' },
    { label: '$25 - $50', value: '25-50' },
    { label: '$50 - $100', value: '50-100' },
    { label: '$100 - $500', value: '100-500' },
    { label: 'Over $500', value: '500+' }
  ];
  
  conditions = [
    { label: 'New', value: 'new' },
    { label: 'Like New', value: 'like-new' },
    { label: 'Good', value: 'good' },
    { label: 'Fair', value: 'fair' }
  ];
  
  sortOptions = [
    { label: 'Best Match', value: 'best-match' },
    { label: 'Price: Low to High', value: 'price-asc' },
    { label: 'Price: High to Low', value: 'price-desc' },
    { label: 'Newest First', value: 'newest' },
    { label: 'Popular', value: 'popular' }
  ];
  
  allCities = {
    US: [
      { name: 'New York', code: 'NY' },
      { name: 'Los Angeles', code: 'LA' },
      { name: 'Chicago', code: 'CHI' }
    ],
    GB: [
      { name: 'London', code: 'LDN' },
      { name: 'Manchester', code: 'MAN' },
      { name: 'Birmingham', code: 'BIR' }
    ],
    CA: [
      { name: 'Toronto', code: 'TOR' },
      { name: 'Vancouver', code: 'VAN' },
      { name: 'Montreal', code: 'MTL' }
    ]
  };
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.searchForm = this.fb.group({
      keywords: [''],
      country: [null],
      city: [{ value: null, disabled: true }],
      categories: [[]],
      priceRange: [null],
      conditions: [[]],
      sortBy: ['best-match']
    });
    
    // Watch country changes to update cities
    this.searchForm.get('country')?.valueChanges.subscribe(country => {
      if (country) {
        this.cities = this.allCities[country.code as keyof typeof this.allCities] || [];
        this.searchForm.get('city')?.enable();
        this.searchForm.patchValue({ city: null });
      } else {
        this.cities = [];
        this.searchForm.get('city')?.disable();
      }
    });
  }
  
  onSearch() {
    console.log('Search criteria:', this.searchForm.value);
    // Perform search
  }
  
  onReset() {
    this.searchForm.reset({
      keywords: '',
      country: null,
      city: null,
      categories: [],
      priceRange: null,
      conditions: [],
      sortBy: 'best-match'
    });
    this.cities = [];
  }
}
```

```html
<div class="search-card">
  <h2>Advanced Search</h2>
  
  <form [formGroup]="searchForm" (ngSubmit)="onSearch()">
    
    <!-- Keywords -->
    <div class="p-field">
      <label for="keywords">Keywords</label>
      <span class="p-input-icon-left">
        <i class="pi pi-search"></i>
        <input 
          id="keywords" 
          type="text" 
          pInputText 
          formControlName="keywords"
          placeholder="Search for items...">
      </span>
    </div>
    
    <!-- Location -->
    <div class="p-grid">
      <div class="p-col-12 p-md-6">
        <div class="p-field">
          <label for="country">Country</label>
          <p-dropdown 
            id="country"
            [options]="countries"
            formControlName="country"
            optionLabel="name"
            [filter]="true"
            filterBy="name"
            placeholder="Select Country"
            [showClear]="true">
          </p-dropdown>
        </div>
      </div>
      
      <div class="p-col-12 p-md-6">
        <div class="p-field">
          <label for="city">City</label>
          <p-dropdown 
            id="city"
            [options]="cities"
            formControlName="city"
            optionLabel="name"
            placeholder="Select City"
            [showClear]="true">
          </p-dropdown>
        </div>
      </div>
    </div>
    
    <!-- Categories -->
    <div class="p-field">
      <label for="categories">Categories</label>
      <p-multiSelect 
        id="categories"
        [options]="categories"
        formControlName="categories"
        optionLabel="label"
        placeholder="All Categories"
        [filter]="true"
        display="chip">
      </p-multiSelect>
    </div>
    
    <!-- Price Range -->
    <div class="p-field">
      <label for="priceRange">Price Range</label>
      <p-dropdown 
        id="priceRange"
        [options]="priceRanges"
        formControlName="priceRange"
        optionLabel="label"
        placeholder="Any Price"
        [showClear]="true">
      </p-dropdown>
    </div>
    
    <!-- Condition -->
    <div class="p-field">
      <label for="conditions">Condition</label>
      <p-multiSelect 
        id="conditions"
        [options]="conditions"
        formControlName="conditions"
        optionLabel="label"
        placeholder="All Conditions"
        display="chip">
      </p-multiSelect>
    </div>
    
    <!-- Sort By -->
    <div class="p-field">
      <label for="sortBy">Sort By</label>
      <p-dropdown 
        id="sortBy"
        [options]="sortOptions"
        formControlName="sortBy"
        optionLabel="label"
        placeholder="Best Match">
      </p-dropdown>
    </div>
    
    <!-- Actions -->
    <div class="form-actions">
      <p-button 
        label="Search" 
        icon="pi pi-search"
        type="submit">
      </p-button>
      <p-button 
        label="Reset Filters" 
        icon="pi pi-filter-slash"
        severity="secondary"
        type="button"
        (onClick)="onReset()">
      </p-button>
    </div>
  </form>
  
  <!-- Current Filters Preview -->
  <div class="filters-preview" *ngIf="searchForm.value | json">
    <h3>Active Filters:</h3>
    <pre>{{ searchForm.value | json }}</pre>
  </div>
</div>
```

```css
.search-card {
  max-width: 800px;
  margin: 20px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.p-field {
  margin-bottom: 20px;
}

.p-field label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #495057;
}

.p-field input,
.p-field p-dropdown,
.p-field p-multiselect {
  width: 100%;
}

.p-grid {
  margin: 0 -0.5rem;
}

.p-col-12 {
  padding: 0 0.5rem;
}

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.filters-preview {
  margin-top: 30px;
  padding: 20px;
  background: #f5f5f5;
  border-radius: 4px;
}

.filters-preview h3 {
  margin-bottom: 10px;
  font-size: 16px;
  color: #333;
}

.filters-preview pre {
  background: white;
  padding: 15px;
  border-radius: 4px;
  overflow-x: auto;
  font-size: 12px;
}

@media (max-width: 768px) {
  .search-card {
    padding: 20px;
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .p-col-12 {
    padding: 0;
  }
}
```

---

## ✅ Day 15 Checklist

- [ ] Implemented basic Dropdown
- [ ] Added filtering to Dropdown
- [ ] Created grouped Dropdowns
- [ ] Used custom templates
- [ ] Implemented MultiSelect
- [ ] Applied virtual scrolling
- [ ] Built advanced search form (Exercise)
- [ ] Handled dependent dropdowns

---

## 🔑 Key Takeaways

1. **Dropdown** - Single selection with filtering support
2. **MultiSelect** - Multiple selections with chips display
3. **Filtering** - Built-in search for large datasets
4. **Grouping** - Organize options into categories
5. **Custom templates** - Full control over display
6. **Virtual scrolling** - Performance for large datasets
7. **Dependent dropdowns** - Dynamic options based on selection

---

## 📚 Additional Resources

- [PrimeNG Dropdown](https://primeng.org/dropdown)
- [PrimeNG MultiSelect](https://primeng.org/multiselect)

---

✅ **Day 15 Complete!** Tomorrow: Calendar & Date Components 📅
