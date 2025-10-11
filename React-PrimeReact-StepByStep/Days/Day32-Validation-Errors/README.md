# Day 32 — Validation & Error Messages

## Objectives
- Add schema-based validation to React Hook Form
- Display PrimeReact-friendly error messages
- Handle async validation and server errors

## Setup with Zod
```tsx
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

const schema = z.object({
  email: z.string().email('Invalid email'),
  password: z.string().min(8, 'Min 8 characters'),
  age: z.coerce.number().min(18, 'Must be 18+')
});

type FormData = z.infer<typeof schema>;

export default function ValidationDemo() {
  const { register, handleSubmit, formState: { errors, isSubmitting }, setError } = useForm<FormData>({ resolver: zodResolver(schema) });

  const onSubmit = async (data: FormData) => {
    try {
      await fakeLogin(data);
    } catch (e: any) {
      setError('email', { type: 'server', message: 'Email not found' });
      setError('password', { type: 'server', message: 'Incorrect password' });
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid gap-3" noValidate>
      <span className="p-float-label">
        <InputText id="email" {...register('email')} className={errors.email && 'p-invalid'} />
        <label htmlFor="email">Email</label>
      </span>
      {errors.email && <small className="p-error">{errors.email.message}</small>}

      <span className="p-float-label">
        <Password id="password" {...register('password')} feedback toggleMask className={errors.password && 'p-invalid'} />
        <label htmlFor="password">Password</label>
      </span>
      {errors.password && <small className="p-error">{errors.password.message}</small>}

      <span className="p-float-label">
        <InputNumber inputId="age" {...register('age', { valueAsNumber: true })} className={errors.age && 'p-invalid'} />
        <label htmlFor="age">Age</label>
      </span>
      {errors.age && <small className="p-error">{errors.age.message}</small>}

      <Button type="submit" label={isSubmitting ? 'Submitting...' : 'Submit'} disabled={isSubmitting} />
    </form>
  );
}
```

## Async Field Validation
```tsx
const checkEmailExists = async (email: string) => {
  await new Promise(r => setTimeout(r, 300));
  return email.toLowerCase().includes('taken');
};

// in register: validate: async value => (await checkEmailExists(value)) ? 'Email already registered' : true
```

## Exercises
- Show a message component with summary of all errors
- Build a reusable FieldError component
- Validate cross-fields (password/confirm)

## Checklist
- [ ] Client + server errors displayed
- [ ] PrimeReact styles applied (p-invalid, p-error)
- [ ] Async validators work
- [ ] NoSubmit if invalid