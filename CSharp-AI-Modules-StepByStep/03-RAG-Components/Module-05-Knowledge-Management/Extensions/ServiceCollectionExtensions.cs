using AI.KnowledgeManagement.Data;
using AI.KnowledgeManagement.Models;
using AI.KnowledgeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace AI.KnowledgeManagement.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKnowledgeManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<KnowledgeManagementSettings>(
            configuration.GetSection("KnowledgeManagement"));

        // Database
        var dbProvider = configuration.GetValue<string>("KnowledgeManagement:DatabaseProvider") ?? "SqlServer";
        var connectionString = configuration.GetConnectionString("KnowledgeDb");

        if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<KnowledgeDbContext>(options =>
                options.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<KnowledgeDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Services
        services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IVersioningService, VersioningService>();
        services.AddScoped<IAccessControlService, AccessControlService>();
        services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IChangeTrackingService, ChangeTrackingService>();

        // Hangfire (optional - for background jobs)
        var enableHangfire = configuration.GetValue<bool>("KnowledgeManagement:EnableBackgroundJobs");
        if (enableHangfire)
        {
            services.AddHangfire(config =>
                config.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();
        }

        return services;
    }
}
