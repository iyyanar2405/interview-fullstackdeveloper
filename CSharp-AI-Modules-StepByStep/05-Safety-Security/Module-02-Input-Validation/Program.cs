using Module_02_Input_Validation.Services;
using Module_02_Input_Validation.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();

// Register validation services
builder.Services.AddScoped<ISanitizationService, SanitizationService>();
builder.Services.AddScoped<IInjectionDetectionService, InjectionDetectionService>();
builder.Services.AddScoped<IXssProtectionService, XssProtectionService>();
builder.Services.AddScoped<ISchemaValidationService, SchemaValidationService>();
builder.Services.AddScoped<IFileValidationService, FileValidationService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Input Validation API", 
        Version = "v1",
        Description = "Comprehensive input validation, sanitization, and security API"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
