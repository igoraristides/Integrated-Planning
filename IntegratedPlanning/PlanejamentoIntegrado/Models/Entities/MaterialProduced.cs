using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanejamentoIntegrado.Models;

[Table("VW_M2V_PI_MATERIAL_PRODUZIDO")]
public class MaterialProduced
{
    [Key]
    [Column("TRANSACTION_ID")]
    public int TransactionId { get; set; }

    [Column("INVENTORY_ITEM_ID")]
    public int? InventoryItemId { get; set; }

    [Column("CONCATENATED_SEGMENTS")]
    public string? ConcatenatedSegments { get; set; }

    [Column("INVOICE_DATE")]
    public DateTime? InvoiceDate { get; set; }

    [Column("TRANSACTION_QUANTITY")]
    public decimal? TransactionQuantity { get; set; }

    [Column("ORG_ID")]
    public int? OrgId { get; set; }
}
