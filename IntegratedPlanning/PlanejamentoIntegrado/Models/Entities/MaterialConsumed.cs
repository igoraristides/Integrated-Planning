using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanejamentoIntegrado.Models;

[Table("VW_M2V_PI_MATERIAL_CONSUMIDO")]
public class MaterialConsumed
{
    [Key]
    [Column("TRANSACTION_ID")]
    public int TransactionId { get; set; }

    [Column("INVENTORY_ITEM_ID")]
    public int? InventoryItemId { get; set; }

    [Column("CONCATENATED_SEGMENTS")]
    public string? ConcatenatedSegments { get; set; }

    [Column("TRANSACTION_DATE")]
    public DateTime? TransactionDate { get; set; }

    [Column("TRANSACTION_QUANTITY")]
    public decimal? TransactionQuantity { get; set; }

    [Column("ORGANIZATION_ID")]
    public int? OrganizationId { get; set; }
}
