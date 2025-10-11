# Day 31 — React Hook Form + PrimeReact

## Objectives
- Integrate React Hook Form with PrimeReact
- Form validation and error handling
- Performance benefits

## Setup
```bash
npm install react-hook-form @hookform/resolvers yup
```

## Basic Integration
```tsx
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object({
  name: yup.string().required('Name is required'),
  email: yup.string().email('Invalid email').required('Email is required'),
  age: yup.number().positive().integer().required('Age is required')
});

function HookFormDemo() {
  const { control, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
    defaultValues: {
      name: '',
      email: '',
      age: null
    }
  });

  const onSubmit = (data) => {
    console.log(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="p-fluid">
      <div className="field">
        <label htmlFor="name">Name</label>
        <Controller
          name="name"
          control={control}
          render={({ field, fieldState }) => (
            <InputText 
              id={field.name} 
              {...field} 
              className={fieldState.error ? 'p-invalid' : ''} 
            />
          )}
        />
        {errors.name && <small className="p-error">{errors.name.message}</small>}
      </div>

      <div className="field">
        <label htmlFor="email">Email</label>
        <Controller
          name="email"
          control={control}
          render={({ field, fieldState }) => (
            <InputText 
              id={field.name} 
              {...field} 
              className={fieldState.error ? 'p-invalid' : ''} 
            />
          )}
        />
        {errors.email && <small className="p-error">{errors.email.message}</small>}
      </div>

      <div className="field">
        <label htmlFor="age">Age</label>
        <Controller
          name="age"
          control={control}
          render={({ field, fieldState }) => (
            <InputNumber 
              id={field.name} 
              value={field.value}
              onValueChange={(e) => field.onChange(e.value)}
              className={fieldState.error ? 'p-invalid' : ''} 
            />
          )}
        />
        {errors.age && <small className="p-error">{errors.age.message}</small>}
      </div>

      <Button type="submit" label="Submit" />
    </form>
  );
}
```

## Exercise
- Create a comprehensive user registration form
- Add all PrimeReact form components with validation

## Checklist
- [ ] React Hook Form integrated
- [ ] Validation schema working
- [ ] Error states display correctly