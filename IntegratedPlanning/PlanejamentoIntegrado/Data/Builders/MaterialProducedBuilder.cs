using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data.Builders;

public class MaterialProducedBuilder : IEntityTypeConfiguration<MaterialProduced>
{
    public void Configure(EntityTypeBuilder<MaterialProduced> builder)
    {
        builder.ToTable("VW_M2V_PI_MATERIAL_PRODUZIDO");

        builder.HasKey(mp => mp.TransactionId);

        builder.Property(mp => mp.TransactionId).HasColumnName("TRANSACTION_ID").IsRequired();

        builder.Property(mp => mp.InventoryItemId).HasColumnName("INVENTORY_ITEM_ID");

        builder
            .Property(mp => mp.ConcatenatedSegments)
            .HasColumnName("CONCATENATED_SEGMENTS")
            .HasMaxLength(255);

        builder.Property(mp => mp.InvoiceDate).HasColumnName("INVOICE_DATE");

        builder
            .Property(mp => mp.TransactionQuantity)
            .HasColumnName("TRANSACTION_QUANTITY")
            .HasPrecision(18, 2);

        builder.Property(mp => mp.OrgId).HasColumnName("ORG_ID");
    }
}
