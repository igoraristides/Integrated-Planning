using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlanejamentoIntegrado.Data.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data;

[ExcludeFromCodeCoverage]
public class IntegratedPlanningDbContext(
    DbContextOptions<IntegratedPlanningDbContext> options,
    ILogger<IntegratedPlanningDbContext> logger
) : DbContext(options)
{
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<MaterialProduced> MaterialsProduced { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserBuilder());
        modelBuilder.ApplyConfiguration(new SupplierBuilder());
        modelBuilder.ApplyConfiguration(new ScheduleBuilder());
        modelBuilder.ApplyConfiguration(new StockItemBuilder());
        modelBuilder.ApplyConfiguration(new MaterialProducedBuilder());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var insertedEntries = ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Added)
                .ToList();

            logger.LogInformation(
                "SaveChangesAsync iniciado. Entidades a inserir: {Count}",
                insertedEntries.Count
            );

            foreach (var entry in insertedEntries)
            {
                var entityType = entry.Entity.GetType().Name;
                logger.LogInformation(
                    "Processando entidade: {EntityType}, State: {State}",
                    entityType,
                    entry.State
                );

                // Log de todas as propriedades
                foreach (var prop in entry.Entity.GetType().GetProperties())
                {
                    var value = prop.GetValue(entry.Entity);
                    logger.LogInformation(
                        "  {EntityType}.{Property} = {Value}",
                        entityType,
                        prop.Name,
                        value
                    );
                }

                var createdAtProperty = entry.Entity.GetType().GetProperty("CreatedAt");
                if (
                    createdAtProperty != null
                    && createdAtProperty.PropertyType == typeof(DateTime?)
                )
                {
                    var now = DateTime.Now;
                    var monthAbbr = new[]
                    {
                        "JAN",
                        "FEB",
                        "MAR",
                        "APR",
                        "MAY",
                        "JUN",
                        "JUL",
                        "AUG",
                        "SEP",
                        "OCT",
                        "NOV",
                        "DEC",
                    }[now.Month - 1];
                    var oracleFormat = $"{now:dd}-{monthAbbr}-{now:yy}";

                    logger.LogInformation("=== LOG DE DATA (CreatedAt) ===");
                    logger.LogInformation("DateTime.Now: {Now}", now);
                    logger.LogInformation("DateTime.Now.ToString(): {ToString}", now.ToString());
                    logger.LogInformation(
                        "DateTime.Now.ToString('yyyy-MM-dd HH:mm:ss'): {Format1}",
                        now.ToString("yyyy-MM-dd HH:mm:ss")
                    );
                    logger.LogInformation(
                        "DateTime.Now.ToString('dd/MM/yyyy HH:mm:ss'): {Format2}",
                        now.ToString("dd/MM/yyyy HH:mm:ss")
                    );
                    logger.LogInformation(
                        "Formato Oracle esperado (DD-MON-YY): {OracleFormat}",
                        oracleFormat
                    );
                    logger.LogInformation(
                        "DateTime.Now.Year: {Year}, Month: {Month}, Day: {Day}, Hour: {Hour}, Minute: {Minute}, Second: {Second}",
                        now.Year,
                        now.Month,
                        now.Day,
                        now.Hour,
                        now.Minute,
                        now.Second
                    );

                    var currentValue = createdAtProperty.GetValue(entry.Entity);
                    logger.LogInformation(
                        "CreatedAt ANTES de definir: {CurrentValue}",
                        currentValue ?? "NULL"
                    );

                    createdAtProperty.SetValue(entry.Entity, now);

                    var afterValue = createdAtProperty.GetValue(entry.Entity);
                    logger.LogInformation("CreatedAt DEPOIS de definir: {AfterValue}", afterValue);
                    logger.LogInformation(
                        "CreatedAt tipo: {Type}",
                        afterValue?.GetType().FullName ?? "NULL"
                    );
                    if (afterValue is DateTime dt)
                    {
                        logger.LogInformation(
                            "CreatedAt como DateTime - ToString(): {ToString}",
                            dt.ToString()
                        );
                        logger.LogInformation(
                            "CreatedAt como DateTime - ToString('O'): {Iso}",
                            dt.ToString("O")
                        );
                    }
                    logger.LogInformation("=== FIM LOG DE DATA ===");
                }

                // Verificar se ID está null/0 (trigger deve gerar)
                var idProperty = entry.Entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(entry.Entity);
                    logger.LogInformation("ID antes do SaveChanges: {Id}", idValue ?? "NULL");

                    // Garantir que ID seja null/0 para o trigger gerar (Oracle trigger espera NULL)
                    if (idValue == null || (idValue is int intId && intId == 0))
                    {
                        // Para int não-nullable, usar 0; para nullable, usar null
                        if (idProperty.PropertyType == typeof(int?))
                            idProperty.SetValue(entry.Entity, null);
                        else
                            idProperty.SetValue(entry.Entity, 0);
                        logger.LogInformation(
                            "ID definido como {Value} para o trigger gerar",
                            idProperty.PropertyType == typeof(int?) ? "NULL" : "0"
                        );
                    }
                }
            }

            logger.LogInformation("=== ANTES DE EXECUTAR SaveChangesAsync ===");
            logger.LogInformation("Verificando entidades no ChangeTracker:");
            foreach (var entry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
            {
                var createdAtProp = entry.Entity.GetType().GetProperty("CreatedAt");
                if (createdAtProp != null)
                {
                    var value = createdAtProp.GetValue(entry.Entity);
                    logger.LogInformation(
                        "  {EntityType}.CreatedAt no ChangeTracker: {Value} (Tipo: {Type})",
                        entry.Entity.GetType().Name,
                        value,
                        value?.GetType().FullName ?? "NULL"
                    );
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "SaveChangesAsync concluído. Registros afetados: {Result}",
                result
            );

            // Log dos IDs após salvar
            foreach (var entry in insertedEntries)
            {
                var idProperty = entry.Entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(entry.Entity);
                    logger.LogInformation("ID após SaveChanges: {Id}", idValue);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro no SaveChangesAsync: {Message}", ex.Message);
            logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
            throw;
        }
    }
}
