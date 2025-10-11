# Day 14 — Dropdown, MultiSelect & AutoComplete

## Objectives
- Selection components mastery
- Custom templates and filtering
- Data binding patterns

## Dropdown
```tsx
const cities = [
  { name: 'New York', code: 'NY' },
  { name: 'Rome', code: 'RM' },
  { name: 'London', code: 'LDN' },
  { name: 'Istanbul', code: 'IST' },
  { name: 'Paris', code: 'PRS' }
];

function DropdownDemo() {
  const [selectedCity, setSelectedCity] = useState(null);

  const cityOptionTemplate = (option) => (
    <div className="p-d-flex p-ai-center">
      <div>{option.name}</div>
    </div>
  );

  return (
    <Dropdown 
      value={selectedCity} 
      options={cities} 
      onChange={(e) => setSelectedCity(e.value)} 
      optionLabel="name" 
      placeholder="Select a City"
      itemTemplate={cityOptionTemplate}
      filter 
      showClear 
    />
  );
}
```

## MultiSelect
```tsx
<MultiSelect 
  value={selectedCities} 
  options={cities} 
  onChange={(e) => setSelectedCities(e.value)} 
  optionLabel="name" 
  placeholder="Select Cities" 
  maxSelectedLabels={3} 
  filter 
/>
```

## AutoComplete
```tsx
function AutoCompleteDemo() {
  const [selectedCountry, setSelectedCountry] = useState(null);
  const [filteredCountries, setFilteredCountries] = useState([]);

  const searchCountry = (event) => {
    const filtered = countries.filter(country => 
      country.name.toLowerCase().includes(event.query.toLowerCase())
    );
    setFilteredCountries(filtered);
  };

  return (
    <AutoComplete 
      value={selectedCountry} 
      suggestions={filteredCountries} 
      completeMethod={searchCountry} 
      field="name" 
      onChange={(e) => setSelectedCountry(e.value)} 
    />
  );
}
```

## Exercise
- Create a user preferences form with all selection types
- Add custom item templates with icons

## Checklist
- [ ] Dropdown with filtering works
- [ ] MultiSelect shows selected count
- [ ] AutoComplete suggests properly
- [ ] Custom templates render correctly