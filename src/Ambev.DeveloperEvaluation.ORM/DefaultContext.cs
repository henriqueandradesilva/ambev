using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    public DefaultContext(
        DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        AuditLog();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AuditLog()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
                Log.Debug($"Entity: {entry.Entity.GetType().Name}, Date Updated after set: {entry.CurrentValues["UpdatedAt"]}");

            if (entry.State == EntityState.Added)
                Log.Debug($"Entity: {entry.Entity.GetType().Name}, Date Created set: {entry.CurrentValues["CreatedAt"]}");

            if (entry.State == EntityState.Deleted)
            {
                var entityType = entry.Entity.GetType().Name;
                var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue;

                if (entityId != null)
                    Log.Debug($"Entity: {entityType}, Id: {entityId}");
            }
        }
    }
}