using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data.Builders;

public class StockItemBuilder : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("VW_M2V_ITENS_ESTOQUE_ONHAND");

        builder.HasKey(s => new { s.OrganizationId, s.InventoryItemId });

        builder.Property(s => s.OrganizationId).HasColumnName("ORGANIZATION_ID");

        builder.Property(s => s.InventoryItemId).HasColumnName("INVENTORY_ITEM_ID");

        builder.Property(s => s.ProductCode).HasColumnName("COD_PRODUTO").HasMaxLength(40);

        builder.Property(s => s.Description).HasColumnName("DESCRICAO").HasMaxLength(240);

        builder
            .Property(s => s.PrimaryUnitOfMeasure)
            .HasColumnName("PRIMARY_UNIT_OF_MEASURE")
            .HasMaxLength(25);

        builder.Property(s => s.PrimaryUomCode).HasColumnName("PRIMARY_UOM_CODE").HasMaxLength(3);

        builder.Property(s => s.Status).HasColumnName("STATUS").HasMaxLength(10);

        builder.Property(s => s.ItemType).HasColumnName("TIPO_ITEM").HasMaxLength(30);

        builder.Property(s => s.OriginOfGoods).HasColumnName("ORIGEM_MERC").HasMaxLength(9);

        builder.Property(s => s.OnHand).HasColumnName("ON_HAND");

        builder.Property(s => s.NewCost).HasColumnName("NEW_COST");

        builder.Property(s => s.SupplierName).HasColumnName("FORNECEDOR").HasMaxLength(240);

        builder.Property(s => s.TotalValue).HasColumnName("VALOR_TOTAL");
    }
}
