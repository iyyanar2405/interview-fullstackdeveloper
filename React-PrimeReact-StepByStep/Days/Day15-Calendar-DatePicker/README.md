# Day 15 — Calendar & DatePicker

## Objectives
- Date selection components
- Date ranges and formatting
- Localization and constraints

## Basic Calendar
```tsx
function CalendarDemo() {
  const [date, setDate] = useState(null);
  const [dates, setDates] = useState(null);
  const [datetime, setDateTime] = useState(null);

  return (
    <div className="p-fluid">
      <div className="p-field">
        <label htmlFor="basic">Basic</label>
        <Calendar 
          id="basic" 
          value={date} 
          onChange={(e) => setDate(e.value)} 
          showIcon 
        />
      </div>

      <div className="p-field">
        <label htmlFor="range">Range</label>
        <Calendar 
          id="range" 
          value={dates} 
          onChange={(e) => setDates(e.value)} 
          selectionMode="range" 
          readOnlyInput 
        />
      </div>

      <div className="p-field">
        <label htmlFor="datetime">Date Time</label>
        <Calendar 
          id="datetime" 
          value={datetime} 
          onChange={(e) => setDateTime(e.value)} 
          showTime 
          hourFormat="12" 
        />
      </div>
    </div>
  );
}
```

## Advanced Features
```tsx
// Min/Max dates
const today = new Date();
const minDate = new Date();
minDate.setMonth(today.getMonth() - 1);
const maxDate = new Date();
maxDate.setMonth(today.getMonth() + 1);

<Calendar 
  value={date} 
  onChange={(e) => setDate(e.value)} 
  minDate={minDate} 
  maxDate={maxDate} 
  disabledDates={[today]} 
  disabledDays={[0, 6]} // Sunday and Saturday
/>

// Multiple selection
<Calendar 
  value={multipleDates} 
  onChange={(e) => setMultipleDates(e.value)} 
  selectionMode="multiple" 
  inline 
/>
```

## Localization
```tsx
import { addLocale } from 'primereact/api';

addLocale('es', {
  firstDayOfWeek: 1,
  dayNames: ['domingo', 'lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado'],
  dayNamesShort: ['dom', 'lun', 'mar', 'mié', 'jue', 'vie', 'sáb'],
  dayNamesMin: ['D', 'L', 'M', 'X', 'J', 'V', 'S'],
  monthNames: ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
  monthNamesShort: ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'],
  today: 'Hoy',
  clear: 'Limpiar'
});

<Calendar value={date} onChange={(e) => setDate(e.value)} locale="es" />
```

## Exercise
- Build an event booking form with date/time selection
- Add date range validation

## Checklist
- [ ] All calendar modes work
- [ ] Date constraints enforced
- [ ] Localization applied
- [ ] Time selection functional