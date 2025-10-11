# Day 11 — PrimeReact Setup & Themes

## Objectives
- Deep dive into PrimeReact theming
- Switch themes dynamically
- Customize theme variables

## Theme Options
- lara-light-blue, lara-dark-blue
- bootstrap4-light-blue, bootstrap4-dark-blue
- md-light-indigo, md-dark-indigo

## Dynamic Theme Switching
```tsx
function ThemeSwitcher() {
  const [theme, setTheme] = useState('lara-light-blue');
  
  const changeTheme = (newTheme: string) => {
    const themeLink = document.getElementById('theme-css') as HTMLLinkElement;
    if (themeLink) {
      themeLink.href = `/node_modules/primereact/resources/themes/${newTheme}/theme.css`;
    }
    setTheme(newTheme);
  };

  return (
    <Dropdown 
      value={theme} 
      options={themeOptions} 
      onChange={(e) => changeTheme(e.value)} 
      placeholder="Select Theme" 
    />
  );
}
```

## Custom CSS Variables
```css
:root {
  --primary-color: #007ad9;
  --primary-color-text: #ffffff;
  --surface-0: #ffffff;
  --surface-50: #fafafa;
}
```

## Exercise
- Create theme switcher with at least 4 themes
- Add custom brand colors

## Checklist
- [ ] Multiple themes working
- [ ] Dynamic theme switching
- [ ] Custom variables applied