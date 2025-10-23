using AI.KnowledgeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AI.KnowledgeManagement.Data;

public class KnowledgeDbContext : DbContext
{
    public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<KnowledgeBase> KnowledgeBases { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentVersion> DocumentVersions { get; set; }
    public DbSet<AccessPermission> AccessPermissions { get; set; }
    public DbSet<SyncJob> SyncJobs { get; set; }
    public DbSet<ChangeLog> ChangeLogs { get; set; }
    public DbSet<SearchAnalytics> SearchAnalytics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // KnowledgeBase configuration
        modelBuilder.Entity<KnowledgeBase>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Document configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.KnowledgeBaseId);
            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // DocumentVersion configuration
        modelBuilder.Entity<DocumentVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DocumentId, e.VersionNumber }).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });

        // AccessPermission configuration
        modelBuilder.Entity<AccessPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ResourceId, e.UserId });
            entity.HasIndex(e => e.UserId);
        });

        // SyncJob configuration
        modelBuilder.Entity<SyncJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.KnowledgeBaseId);
            entity.HasIndex(e => e.StartedAt);
        });

        // ChangeLog configuration
        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ResourceId);
            entity.HasIndex(e => e.ChangedBy);
            entity.HasIndex(e => e.ChangedAt);
        });

        // SearchAnalytics configuration
        modelBuilder.Entity<SearchAnalytics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.KnowledgeBaseId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
