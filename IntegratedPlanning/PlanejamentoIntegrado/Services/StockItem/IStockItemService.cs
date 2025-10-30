using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Services;

public interface IStockItemService
{
    Task<List<StockItem>> GetStockItemsWithFilters(
        string? supplierName,
        string? model,
        int? inventoryItemId,
        string? searchTerm,
        int pageNumber = 0,
        int pageSize = 0
    );
    Task<int> CountStockItemsWithFilters(
        string? supplierName,
        string? model,
        int? inventoryItemId,
        string? searchTerm
    );
    Task<List<string>> GetDistinctModels();
    Task<List<string>> GetDistinctStockItemsByInventoryId();
}
