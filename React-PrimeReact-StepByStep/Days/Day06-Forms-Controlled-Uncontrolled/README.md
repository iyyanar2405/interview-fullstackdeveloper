# Day 06 — Forms: Controlled/Uncontrolled

## Objectives
- Controlled components with state
- Uncontrolled with refs
- Form handling patterns

## Example (Controlled)
```tsx
function LoginForm() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log({ email, password });
  };

  return (
    <form onSubmit={handleSubmit}>
      <InputText 
        value={email} 
        onChange={(e) => setEmail(e.target.value)} 
        placeholder="Email" 
      />
      <Password 
        value={password} 
        onChange={(e) => setPassword(e.target.value)} 
        placeholder="Password" 
      />
      <Button type="submit" label="Login" />
    </form>
  );
}
```

## Exercise
- Build a contact form with name, email, message using PrimeReact inputs

## Checklist
- [ ] Controlled inputs work
- [ ] Form submission handled
- [ ] Validation basics added