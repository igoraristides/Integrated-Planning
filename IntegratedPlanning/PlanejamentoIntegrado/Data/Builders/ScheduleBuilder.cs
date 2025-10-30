using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data.Builders;

public class ScheduleBuilder : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("VW_M2V_PI_SCHEDULES");

        // Usar chave composta: HEADER_ID + LINE_ID
        builder.HasKey(s => new { s.HeaderId, s.LineId });

        builder.Property(s => s.HeaderId).HasColumnName("HEADER_ID");
        builder.Property(s => s.LineId).HasColumnName("LINE_ID");
        builder.Property(s => s.OrganizationId).HasColumnName("ORG_ID");
        builder.Property(s => s.CreatedAt).HasColumnName("CREATION_DATE");

        builder.Property(s => s.InterfaceHeaderId).HasColumnName("INTERFACE_HEADER_ID");

        builder.Property(s => s.InterfaceLineId).HasColumnName("INTERFACE_LINE_ID");

        builder
            .Property(s => s.CustomerShipToExt)
            .HasColumnName("CUST_SHIP_TO_EXT")
            .HasMaxLength(35);

        builder
            .Property(s => s.IndustryAttribute10)
            .HasColumnName("INDUSTRY_ATTRIBUTE10")
            .HasMaxLength(150);

        builder
            .Property(s => s.IndustryAttribute3)
            .HasColumnName("INDUSTRY_ATTRIBUTE3")
            .HasMaxLength(150);

        builder.Property(s => s.CustomerId).HasColumnName("CUSTOMER_ID");

        builder.Property(s => s.ScheduleType).HasColumnName("SCHEDULE_TYPE").HasMaxLength(30);

        builder.Property(s => s.ScheduleHorizonStartDate).HasColumnName("SCHED_HORIZON_START_DATE");

        builder.Property(s => s.ScheduleHorizonEndDate).HasColumnName("SCHED_HORIZON_END_DATE");

        builder
            .Property(s => s.ScheduleReferenceNum)
            .HasColumnName("SCHEDULE_REFERENCE_NUM")
            .HasMaxLength(35);

        builder.Property(s => s.CustomerNameExt).HasColumnName("CUST_NAME_EXT").HasMaxLength(360);

        builder.Property(s => s.ScheduleGenerationDate).HasColumnName("SCHED_GENERATION_DATE");

        builder.Property(s => s.LineNumber).HasColumnName("LINE_NUMBER");

        builder.Property(s => s.StartDateTime).HasColumnName("START_DATE_TIME");

        builder
            .Property(s => s.CustomerItemExt)
            .HasColumnName("CUSTOMER_ITEM_EXT")
            .HasMaxLength(50);

        builder.Property(s => s.CustomerItemId).HasColumnName("CUSTOMER_ITEM_ID");

        builder.Property(s => s.InventoryItemId).HasColumnName("INVENTORY_ITEM_ID");

        builder.Property(s => s.ItemDetailQuantity).HasColumnName("ITEM_DETAIL_QUANTITY");

        builder.Property(s => s.UomCode).HasColumnName("UOM_CODE").HasMaxLength(3);

        builder.Property(s => s.ShipToOrgId).HasColumnName("SHIP_TO_ORG_ID");

        builder.Property(s => s.ShipFromOrgId).HasColumnName("SHIP_FROM_ORG_ID");

        builder.Property(s => s.ShipToAddressId).HasColumnName("SHIP_TO_ADDRESS_ID");

        builder.Property(s => s.ShipToNameExt).HasColumnName("SHIP_TO_NAME_EXT").HasMaxLength(60);

        builder.Property(s => s.ProcessStatus).HasColumnName("PROCESS_STATUS");
    }
}
