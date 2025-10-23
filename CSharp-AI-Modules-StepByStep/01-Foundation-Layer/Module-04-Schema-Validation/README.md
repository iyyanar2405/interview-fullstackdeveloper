# Module 04: Schema Validation

Comprehensive data validation using Data Annotations, FluentValidation, and custom validators for AI applications.

## Learning Objectives

- Master Data Annotations for basic validation
- Implement FluentValidation for complex scenarios
- Create custom validation attributes
- Build composite validators
- Handle validation in API controllers
- Implement client-side validation
- Create validation pipelines

## Project Structure

```
Module-04-Schema-Validation/
├── README.md
├── AI.SchemaValidation.csproj
├── Program.cs
├── Models/
│   ├── ValidationModels.cs
│   ├── ApiModels.cs
│   └── ComplexModels.cs
├── Validators/
│   ├── FluentValidators/
│   │   ├── UserValidator.cs
│   │   ├── EmailMessageValidator.cs
│   │   └── AIRequestValidator.cs
│   ├── CustomAttributes/
│   │   ├── EmailDomainAttribute.cs
│   │   ├── PhoneNumberAttribute.cs
│   │   └── JsonValidAttribute.cs
│   └── CompositeValidators/
│       ├── BusinessRuleValidator.cs
│       └── SecurityValidator.cs
├── Extensions/
│   ├── ValidationExtensions.cs
│   └── ModelStateExtensions.cs
├── Middleware/
│   └── ValidationMiddleware.cs
├── Services/
│   ├── IValidationService.cs
│   └── ValidationService.cs
└── Examples/
    ├── DataAnnotationsExample.cs
    ├── FluentValidationExample.cs
    ├── CustomValidationExample.cs
    └── ApiValidationExample.cs
```

## Key Features

### 1. **Data Annotations**
- Built-in validation attributes
- Range and length validation
- Regular expression validation
- Custom validation attributes

### 2. **FluentValidation**
- Complex business rule validation
- Conditional validation
- Async validation
- Custom validators

### 3. **API Integration**
- Model state validation
- Automatic error responses
- Validation middleware
- Error message localization

## Dependencies

```xml
<PackageReference Include="FluentValidation" Version="11.8.1" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```

## Getting Started

1. **Basic validation**:
   ```csharp
   public class User
   {
       [Required]
       [StringLength(50)]
       public string Name { get; set; }
       
       [EmailAddress]
       public string Email { get; set; }
   }
   ```

2. **FluentValidation**:
   ```csharp
   public class UserValidator : AbstractValidator<User>
   {
       public UserValidator()
       {
           RuleFor(x => x.Name).NotEmpty().Length(2, 50);
           RuleFor(x => x.Email).NotEmpty().EmailAddress();
       }
   }
   ```

## Best Practices

- Use Data Annotations for simple validation
- Use FluentValidation for complex business rules
- Create reusable custom validators
- Implement proper error messaging
- Validate at multiple layers (client, API, business)
- Use async validation for external checks