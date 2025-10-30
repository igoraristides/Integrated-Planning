namespace PlanejamentoIntegrado.Models;

public class StockItem
{
    /// <summary>ORGANIZATION_ID - Identificador da organização</summary>
    public int OrganizationId { get; set; }

    /// <summary>INVENTORY_ITEM_ID - Identificador do item no inventário</summary>
    public int InventoryItemId { get; set; }

    /// <summary>SEGMENT1 - Código do produto</summary>
    public string? ProductCode { get; set; }

    /// <summary>DESCRIPTION - Descrição do item</summary>
    public string? Description { get; set; }

    /// <summary>PRIMARY_UNIT_OF_MEASURE - Unidade de medida primária</summary>
    public string? PrimaryUnitOfMeasure { get; set; }

    /// <summary>PRIMARY_UOM_CODE - Código da unidade de medida primária</summary>
    public string? PrimaryUomCode { get; set; }

    /// <summary>INVENTORY_ITEM_STATUS_CODE - Código do status do item</summary>
    public string? Status { get; set; }

    /// <summary>ITEM_TYPE - Tipo do item</summary>
    public string? ItemType { get; set; }

    /// <summary>COUNTRY_OF_ORIGIN_CODE - Código do país de origem</summary>
    public string? OriginOfGoods { get; set; }

    /// <summary>ON_HAND - Quantidade disponível em estoque</summary>
    public decimal? OnHand { get; set; }

    /// <summary>ITEM_COST - Custo atual do item</summary>
    public decimal? NewCost { get; set; }

    /// <summary>VENDOR_NAME - Nome do fornecedor</summary>
    public string? SupplierName { get; set; }

    /// <summary>Valor total calculado (ON_HAND * ITEM_COST)</summary>
    public decimal? TotalValue { get; set; }
}
