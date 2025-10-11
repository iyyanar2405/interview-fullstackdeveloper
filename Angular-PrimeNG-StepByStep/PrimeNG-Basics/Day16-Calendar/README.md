# Day 16: Calendar & Date Components 📅

## 📋 Learning Objectives
- Master Calendar component
- Implement date range selection
- Use time picker
- Combine date and time
- Handle date restrictions and validation
- Format dates properly

---

## 📅 Calendar Component

### Installation
```typescript
import { CalendarModule } from 'primeng/calendar';
```

### Basic Calendar
```html
<!-- Basic date picker -->
<p-calendar [(ngModel)]="date" dateFormat="yy-mm-dd"></p-calendar>

<!-- With placeholder -->
<p-calendar [(ngModel)]="date" placeholder="Select a date"></p-calendar>

<!-- Inline calendar -->
<p-calendar [(ngModel)]="date" [inline]="true"></p-calendar>

<p>Selected Date: {{ date | date:'fullDate' }}</p>
```

```typescript
export class CalendarDemoComponent {
  date: Date | undefined;
}
```

### Date Formats
```html
<!-- Different date formats -->
<p-calendar [(ngModel)]="date1" dateFormat="dd/mm/yy"></p-calendar>
<p-calendar [(ngModel)]="date2" dateFormat="mm/dd/yy"></p-calendar>
<p-calendar [(ngModel)]="date3" dateFormat="yy-mm-dd"></p-calendar>
<p-calendar [(ngModel)]="date4" dateFormat="dd.mm.yy"></p-calendar>
<p-calendar [(ngModel)]="date5" dateFormat="dd M yy"></p-calendar>
```

### Date Range Selection
```html
<!-- Date range -->
<p-calendar 
  [(ngModel)]="rangeDates"
  selectionMode="range"
  [readonlyInput]="true"
  placeholder="Select date range">
</p-calendar>

<div *ngIf="rangeDates && rangeDates[0] && rangeDates[1]">
  <p>From: {{ rangeDates[0] | date:'mediumDate' }}</p>
  <p>To: {{ rangeDates[1] | date:'mediumDate' }}</p>
</div>
```

```typescript
rangeDates: Date[] | undefined;
```

### Time Picker
```html
<!-- Time only -->
<p-calendar 
  [(ngModel)]="time"
  [timeOnly]="true"
  placeholder="Select time">
</p-calendar>

<!-- 24-hour format -->
<p-calendar 
  [(ngModel)]="time"
  [timeOnly]="true"
  [hourFormat]="24">
</p-calendar>

<!-- With seconds -->
<p-calendar 
  [(ngModel)]="time"
  [timeOnly]="true"
  [showSeconds]="true">
</p-calendar>
```

### DateTime Combined
```html
<!-- Date and time together -->
<p-calendar 
  [(ngModel)]="datetime"
  [showTime]="true"
  placeholder="Select date and time">
</p-calendar>

<!-- With 24-hour format -->
<p-calendar 
  [(ngModel)]="datetime"
  [showTime]="true"
  [hourFormat]="24">
</p-calendar>

<!-- With seconds -->
<p-calendar 
  [(ngModel)]="datetime"
  [showTime]="true"
  [showSeconds]="true">
</p-calendar>
```

### Min/Max Dates
```html
<!-- Restrict date range -->
<p-calendar 
  [(ngModel)]="date"
  [minDate]="minDate"
  [maxDate]="maxDate"
  placeholder="Select date">
</p-calendar>
```

```typescript
export class CalendarDemoComponent {
  date: Date | undefined;
  minDate: Date;
  maxDate: Date;
  
  constructor() {
    // Set min date to today
    this.minDate = new Date();
    
    // Set max date to 30 days from now
    this.maxDate = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 30);
  }
}
```

### Disabled Dates
```html
<!-- Disable specific dates -->
<p-calendar 
  [(ngModel)]="date"
  [disabledDates]="invalidDates"
  [disabledDays]="[0,6]"
  placeholder="Weekdays only">
</p-calendar>
```

```typescript
invalidDates: Date[] = [];

constructor() {
  // Disable specific dates
  let today = new Date();
  let invalidDate = new Date();
  invalidDate.setDate(today.getDate() - 1);
  this.invalidDates = [today, invalidDate];
}
```

### Multiple Date Selection
```html
<!-- Select multiple dates -->
<p-calendar 
  [(ngModel)]="multipleDates"
  selectionMode="multiple"
  [readonlyInput]="true"
  placeholder="Select multiple dates">
</p-calendar>

<div *ngIf="multipleDates && multipleDates.length > 0">
  <h4>Selected Dates:</h4>
  <ul>
    <li *ngFor="let date of multipleDates">
      {{ date | date:'mediumDate' }}
    </li>
  </ul>
</div>
```

```typescript
multipleDates: Date[] | undefined;
```

### Month/Year Picker
```html
<!-- Month picker -->
<p-calendar 
  [(ngModel)]="date"
  view="month"
  dateFormat="mm/yy"
  placeholder="Select month">
</p-calendar>

<!-- Year picker -->
<p-calendar 
  [(ngModel)]="date"
  view="year"
  dateFormat="yy"
  placeholder="Select year">
</p-calendar>
```

### Touch UI
```html
<!-- Mobile-friendly touch UI -->
<p-calendar 
  [(ngModel)]="date"
  [touchUI]="true"
  placeholder="Touch UI calendar">
</p-calendar>
```

### Icons and Styling
```html
<!-- Custom icon -->
<p-calendar 
  [(ngModel)]="date"
  [showIcon]="true"
  placeholder="With calendar icon">
</p-calendar>

<!-- Button bar -->
<p-calendar 
  [(ngModel)]="date"
  [showButtonBar]="true"
  placeholder="With today/clear buttons">
</p-calendar>
```

---

## 🎯 Practical Exercise: Hotel Booking Form

```typescript
export class HotelBookingComponent implements OnInit {
  bookingForm!: FormGroup;
  
  minCheckIn: Date;
  maxCheckOut: Date;
  disabledDates: Date[] = [];
  
  roomTypes = [
    { label: 'Single Room', value: 'single' },
    { label: 'Double Room', value: 'double' },
    { label: 'Suite', value: 'suite' },
    { label: 'Family Room', value: 'family' }
  ];
  
  guestCounts = [
    { label: '1 Guest', value: 1 },
    { label: '2 Guests', value: 2 },
    { label: '3 Guests', value: 3 },
    { label: '4+ Guests', value: 4 }
  ];
  
  constructor(private fb: FormBuilder) {
    // Set minimum check-in to today
    this.minCheckIn = new Date();
    
    // Set maximum check-out to 1 year from now
    this.maxCheckOut = new Date();
    this.maxCheckOut.setFullYear(this.maxCheckOut.getFullYear() + 1);
    
    // Disable some dates (e.g., fully booked dates)
    this.disabledDates = this.getFullyBookedDates();
  }
  
  ngOnInit() {
    this.bookingForm = this.fb.group({
      checkIn: [null, Validators.required],
      checkOut: [null, Validators.required],
      roomType: ['double', Validators.required],
      guestCount: [2, Validators.required],
      specialRequests: ['']
    });
    
    // Watch check-in date changes
    this.bookingForm.get('checkIn')?.valueChanges.subscribe(checkInDate => {
      if (checkInDate) {
        // Set minimum check-out to next day
        const minCheckOut = new Date(checkInDate);
        minCheckOut.setDate(minCheckOut.getDate() + 1);
        
        // Reset check-out if it's before new minimum
        const currentCheckOut = this.bookingForm.get('checkOut')?.value;
        if (currentCheckOut && currentCheckOut < minCheckOut) {
          this.bookingForm.patchValue({ checkOut: null });
        }
      }
    });
  }
  
  getFullyBookedDates(): Date[] {
    // Simulate some fully booked dates
    const bookedDates: Date[] = [];
    const today = new Date();
    
    // Add some random dates as fully booked
    for (let i = 5; i < 15; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + i);
      bookedDates.push(date);
    }
    
    return bookedDates;
  }
  
  get numberOfNights(): number {
    const checkIn = this.bookingForm.get('checkIn')?.value;
    const checkOut = this.bookingForm.get('checkOut')?.value;
    
    if (checkIn && checkOut) {
      const diffTime = Math.abs(checkOut.getTime() - checkIn.getTime());
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      return diffDays;
    }
    
    return 0;
  }
  
  get totalPrice(): number {
    const nights = this.numberOfNights;
    const roomType = this.bookingForm.get('roomType')?.value;
    
    const prices: any = {
      single: 99,
      double: 149,
      suite: 299,
      family: 199
    };
    
    return nights * (prices[roomType] || 0);
  }
  
  onSubmit() {
    if (this.bookingForm.valid) {
      const bookingData = {
        ...this.bookingForm.value,
        numberOfNights: this.numberOfNights,
        totalPrice: this.totalPrice
      };
      
      console.log('Booking submitted:', bookingData);
      // Submit to backend
    } else {
      this.markFormGroupTouched(this.bookingForm);
    }
  }
  
  markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }
  
  resetForm() {
    this.bookingForm.reset({
      roomType: 'double',
      guestCount: 2
    });
  }
}
```

```html
<div class="booking-card">
  <h2>Hotel Booking</h2>
  <p class="subtitle">Find your perfect stay</p>
  
  <form [formGroup]="bookingForm" (ngSubmit)="onSubmit()">
    
    <!-- Check-in Date -->
    <div class="p-field">
      <label for="checkIn">Check-in Date*</label>
      <p-calendar 
        id="checkIn"
        formControlName="checkIn"
        [minDate]="minCheckIn"
        [maxDate]="maxCheckOut"
        [disabledDates]="disabledDates"
        [showIcon]="true"
        [showButtonBar]="true"
        placeholder="Select check-in date"
        dateFormat="dd M yy"
        [class.ng-invalid]="bookingForm.get('checkIn')?.invalid && bookingForm.get('checkIn')?.touched">
      </p-calendar>
      <small *ngIf="bookingForm.get('checkIn')?.invalid && bookingForm.get('checkIn')?.touched" 
             class="p-error">
        Check-in date is required
      </small>
    </div>
    
    <!-- Check-out Date -->
    <div class="p-field">
      <label for="checkOut">Check-out Date*</label>
      <p-calendar 
        id="checkOut"
        formControlName="checkOut"
        [minDate]="bookingForm.get('checkIn')?.value || minCheckIn"
        [maxDate]="maxCheckOut"
        [disabledDates]="disabledDates"
        [showIcon]="true"
        [showButtonBar]="true"
        placeholder="Select check-out date"
        dateFormat="dd M yy"
        [class.ng-invalid]="bookingForm.get('checkOut')?.invalid && bookingForm.get('checkOut')?.touched">
      </p-calendar>
      <small *ngIf="bookingForm.get('checkOut')?.invalid && bookingForm.get('checkOut')?.touched" 
             class="p-error">
        Check-out date is required
      </small>
    </div>
    
    <!-- Stay Duration -->
    <div class="duration-display" *ngIf="numberOfNights > 0">
      <i class="pi pi-moon"></i>
      <span>{{ numberOfNights }} night{{ numberOfNights > 1 ? 's' : '' }}</span>
    </div>
    
    <!-- Room Type -->
    <div class="p-field">
      <label for="roomType">Room Type*</label>
      <p-dropdown 
        id="roomType"
        [options]="roomTypes"
        formControlName="roomType"
        optionLabel="label"
        placeholder="Select room type">
      </p-dropdown>
    </div>
    
    <!-- Guest Count -->
    <div class="p-field">
      <label for="guestCount">Number of Guests*</label>
      <p-dropdown 
        id="guestCount"
        [options]="guestCounts"
        formControlName="guestCount"
        optionLabel="label"
        placeholder="Select guests">
      </p-dropdown>
    </div>
    
    <!-- Special Requests -->
    <div class="p-field">
      <label for="specialRequests">Special Requests</label>
      <textarea 
        id="specialRequests"
        pInputTextarea
        formControlName="specialRequests"
        rows="3"
        placeholder="Any special requirements? (Optional)">
      </textarea>
    </div>
    
    <!-- Price Summary -->
    <div class="price-summary" *ngIf="numberOfNights > 0">
      <div class="price-row">
        <span>Room rate per night:</span>
        <span>{{ (totalPrice / numberOfNights) | currency }}</span>
      </div>
      <div class="price-row">
        <span>Number of nights:</span>
        <span>{{ numberOfNights }}</span>
      </div>
      <div class="price-row total">
        <span>Total Price:</span>
        <span>{{ totalPrice | currency }}</span>
      </div>
    </div>
    
    <!-- Form Actions -->
    <div class="form-actions">
      <p-button 
        label="Book Now" 
        icon="pi pi-check"
        type="submit"
        [disabled]="bookingForm.invalid">
      </p-button>
      <p-button 
        label="Reset" 
        icon="pi pi-refresh"
        severity="secondary"
        type="button"
        (onClick)="resetForm()">
      </p-button>
    </div>
  </form>
</div>
```

```css
.booking-card {
  max-width: 600px;
  margin: 20px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0,0,0,0.1);
}

.booking-card h2 {
  margin-bottom: 8px;
  color: #333;
}

.subtitle {
  color: #666;
  margin-bottom: 30px;
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

.p-field p-calendar,
.p-field p-dropdown,
.p-field textarea {
  width: 100%;
}

.duration-display {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px;
  background: #e3f2fd;
  border-radius: 4px;
  margin-bottom: 20px;
  color: #1976d2;
  font-weight: 600;
}

.duration-display i {
  font-size: 1.2rem;
}

.price-summary {
  background: #f5f5f5;
  padding: 20px;
  border-radius: 4px;
  margin: 20px 0;
}

.price-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #e0e0e0;
}

.price-row:last-child {
  border-bottom: none;
}

.price-row.total {
  font-size: 1.2rem;
  font-weight: bold;
  color: #1976d2;
  margin-top: 10px;
  padding-top: 15px;
  border-top: 2px solid #1976d2;
}

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.p-error {
  color: #f44336;
  font-size: 0.875rem;
  display: block;
  margin-top: 4px;
}

@media (max-width: 768px) {
  .booking-card {
    padding: 20px;
    margin: 10px;
  }
  
  .form-actions {
    flex-direction: column;
  }
}
```

---

## ✅ Day 16 Checklist

- [ ] Implemented basic Calendar
- [ ] Used date range selection
- [ ] Added time picker
- [ ] Combined date and time
- [ ] Set min/max date restrictions
- [ ] Disabled specific dates
- [ ] Built hotel booking form (Exercise)
- [ ] Calculated date differences

---

## 🔑 Key Takeaways

1. **Calendar** - Flexible date/time selection component
2. **Date range** - Select start and end dates together
3. **Time picker** - 12/24 hour format with seconds
4. **Restrictions** - min/max dates and disabled dates
5. **Multiple selection** - Select multiple dates
6. **Month/Year picker** - For broader date selection
7. **Touch UI** - Mobile-friendly interface
8. **Validation** - Integrate with reactive forms

---

## 📚 Additional Resources

- [PrimeNG Calendar](https://primeng.org/calendar)
- [Date Formatting Guide](https://angular.io/api/common/DatePipe)

---

✅ **Day 16 Complete!** Tomorrow: Card, Panel & Fieldset 📦
