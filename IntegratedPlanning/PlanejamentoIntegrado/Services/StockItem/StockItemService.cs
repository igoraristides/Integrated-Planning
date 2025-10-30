using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class StockItemService(IRepository<StockItem> stockItemRepository) : IStockItemService
{
    private const int DefaultOrganizationId = 192; // TODO: Tornar configurável

    public async Task<List<StockItem>> GetStockItemsWithFilters(
        string? supplierName,
        string? model,
        int? inventoryItemId,
        string? searchTerm,
        int pageNumber = 0,
        int pageSize = 0
    )
    {
        return await stockItemRepository.GetAll(
            filter: s =>
                s.OrganizationId == DefaultOrganizationId
                && (
                    string.IsNullOrEmpty(supplierName)
                    || (s.SupplierName != null && s.SupplierName.Contains(supplierName))
                )
                && (
                    string.IsNullOrEmpty(model)
                    || (s.Description != null && s.Description.Contains(model))
                )
                && (!inventoryItemId.HasValue || s.InventoryItemId == inventoryItemId.Value)
                && (
                    string.IsNullOrEmpty(searchTerm)
                    || (s.ProductCode != null && s.ProductCode.Contains(searchTerm))
                    || (s.Description != null && s.Description.Contains(searchTerm))
                ),
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderByDescending(s => s.TotalValue)
        );
    }

    public async Task<int> CountStockItemsWithFilters(
        string? supplierName,
        string? model,
        int? inventoryItemId,
        string? searchTerm
    )
    {
        return await stockItemRepository.Count(predicate: s =>
            s.OrganizationId == DefaultOrganizationId
            && (
                string.IsNullOrEmpty(supplierName)
                || (s.SupplierName != null && s.SupplierName.Contains(supplierName))
            )
            && (
                string.IsNullOrEmpty(model)
                || (s.Description != null && s.Description.Contains(model))
            )
            && (!inventoryItemId.HasValue || s.InventoryItemId == inventoryItemId.Value)
            && (
                string.IsNullOrEmpty(searchTerm)
                || (s.ProductCode != null && s.ProductCode.Contains(searchTerm))
                || (s.Description != null && s.Description.Contains(searchTerm))
            )
        );
    }

    public async Task<List<string>> GetDistinctModels()
    {
        return await stockItemRepository.GetAllDistinctBy(
            selector: s => s.Description!,
            filter: s =>
                s.OrganizationId == DefaultOrganizationId && !string.IsNullOrEmpty(s.Description),
            orderBy: q => q.OrderBy(d => d)
        );
    }

    public async Task<List<string>> GetDistinctStockItemsByInventoryId()
    {
        return await stockItemRepository.GetAllDistinctBy(
            selector: s => s.ProductCode!,
            filter: s =>
                s.OrganizationId == DefaultOrganizationId && !string.IsNullOrEmpty(s.ProductCode),
            orderBy: q => q.OrderBy(p => p)
        );
    }
}
