using PlanejamentoIntegrado.Enums;

namespace PlanejamentoIntegrado.Models;

public class CompareVariationViewModel
{
    public int InventoryItemId { get; set; }
    public string? ProductCode { get; set; }
    public string? Description { get; set; }
    public string? SupplierName { get; set; }
    public decimal? UnitValue { get; set; }
    public decimal? DemandQuantity { get; set; }
    public decimal? TotalValue { get; set; }
    public string? ItemType { get; set; }
    public VariationType? Variation { get; set; }
}
