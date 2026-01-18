using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.SharedKernel.Persistence;

/// <summary>
/// Central DbContext for the entire Postal platform. Each module registers its entity
/// configurations via IEntityTypeConfiguration discovered from referenced assemblies.
/// </summary>
public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ICurrentUserService? currentUserService = null) : DbContext(options)
{
    private readonly ICurrentUserService? _currentUserService = currentUserService;

    /// <summary>
    /// Assemblies whose IEntityTypeConfiguration classes should be scanned.
    /// Host projects register module assemblies here at startup.
    /// </summary>
    public static List<Assembly> ConfigurationAssemblies { get; } = [];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var assembly in ConfigurationAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        // AuditLog configuration (no soft delete)
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100).HasColumnName("entity_type");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50).HasColumnName("action");
            entity.Property(e => e.PerformedBy).IsRequired().HasMaxLength(256).HasColumnName("performed_by");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.EntityId);
        });

        // Global query filter for soft delete on all BaseEntity subtypes
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditFields();
        // System.Diagnostics.Debug.WriteLine($"SaveChanges invoked. Tracked entries={ChangeTracker.Entries().Count()}");
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        // System.Diagnostics.Debug.WriteLine($"SaveChangesAsync invoked. Tracked entries={ChangeTracker.Entries().Count()}");
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditFields()
    {
        var now = DateTimeOffset.UtcNow;
        var username = _currentUserService?.Username ?? "system";

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = username;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = username;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseTranslation>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = username;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = username;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }
    }

    private static LambdaExpression BuildSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var deletedAt = Expression.Property(parameter, nameof(BaseEntity.DeletedAt));
        var nullConstant = Expression.Constant(null, typeof(DateTimeOffset?));
        var comparison = Expression.Equal(deletedAt, nullConstant);
        return Expression.Lambda(comparison, parameter);
    }
}
