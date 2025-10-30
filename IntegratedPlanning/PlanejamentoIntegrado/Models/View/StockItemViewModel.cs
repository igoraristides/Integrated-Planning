namespace PlanejamentoIntegrado.Models;

public class StockItemViewModel
{
    public int InventoryItemId { get; set; }
    public string? ProductCode { get; set; }
    public string? Description { get; set; }
    public string? SupplierName { get; set; }
    public decimal? UnitValue { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? TotalValue { get; set; }
    public string? ItemType { get; set; }
}
