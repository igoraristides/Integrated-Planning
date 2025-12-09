using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data.Builders;

public class MaterialConsumedBuilder : IEntityTypeConfiguration<MaterialConsumed>
{
    public void Configure(EntityTypeBuilder<MaterialConsumed> builder)
    {
        builder.ToTable("VW_M2V_PI_MATERIAL_CONSUMIDO");

        builder.HasKey(mc => mc.TransactionId);

        builder.Property(mc => mc.TransactionId).HasColumnName("TRANSACTION_ID").IsRequired();

        builder.Property(mc => mc.InventoryItemId).HasColumnName("INVENTORY_ITEM_ID");

        builder
            .Property(mc => mc.ConcatenatedSegments)
            .HasColumnName("CONCATENATED_SEGMENTS")
            .HasMaxLength(255);

        builder.Property(mc => mc.TransactionDate).HasColumnName("TRANSACTION_DATE");

        builder
            .Property(mc => mc.TransactionQuantity)
            .HasColumnName("TRANSACTION_QUANTITY")
            .HasPrecision(18, 2);

        builder.Property(mc => mc.OrganizationId).HasColumnName("ORGANIZATION_ID");
    }
}
