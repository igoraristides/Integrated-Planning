using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Data.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data;

[ExcludeFromCodeCoverage]
public class IntegratedPlanningDbContext(DbContextOptions<IntegratedPlanningDbContext> options)
    : DbContext(options)
{
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<MaterialConsumed> MaterialsConsumed { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserBuilder());
        modelBuilder.ApplyConfiguration(new SupplierBuilder());
        modelBuilder.ApplyConfiguration(new ScheduleBuilder());
        modelBuilder.ApplyConfiguration(new StockItemBuilder());
        modelBuilder.ApplyConfiguration(new MaterialConsumedBuilder());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var insertedEntries = this
            .ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            var createdAtProperty = insertedEntry.GetType().GetProperty("CreatedAt");
            if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime?))
            {
                createdAtProperty.SetValue(insertedEntry, DateTime.Now);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
