using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Module_01_Authentication_Authorization.Services;
using Module_01_Authentication_Authorization.Models;

namespace Module_01_Authentication_Authorization;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add JWT authentication
        services.AddJwtAuthentication(configuration);

        // Add OAuth authentication
        services.AddOAuthAuthentication(configuration);

        // Register services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddScoped<IRbacService, RbacService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();

        // Add HttpClient for OAuth
        services.AddHttpClient<IOAuthService, OAuthService>();

        // Configure JWT settings
        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
        services.Configure<TotpConfig>(configuration.GetSection("Totp"));

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("Jwt").Get<JwtConfig>() ?? new JwtConfig();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "Unauthorized",
                        message = "You are not authorized to access this resource"
                    });
                    
                    return context.Response.WriteAsync(result);
                }
            };
        });

        return services;
    }

    private static IServiceCollection AddOAuthAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["OAuth:Google:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["OAuth:Google:ClientSecret"] ?? string.Empty;
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = configuration["OAuth:Microsoft:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["OAuth:Microsoft:ClientSecret"] ?? string.Empty;
            })
            .AddGitHub(options =>
            {
                options.ClientId = configuration["OAuth:GitHub:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["OAuth:GitHub:ClientSecret"] ?? string.Empty;
            });

        return services;
    }

    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        // This would use NWebsec in a full implementation
        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        // This would use AspNetCoreRateLimit in a full implementation
        services.AddMemoryCache();
        return services;
    }

    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
