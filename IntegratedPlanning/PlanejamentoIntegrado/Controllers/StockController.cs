using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejamentoIntegrado.Helpers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;

namespace PlanejamentoIntegrado.Controllers;

[Authorize]
public class StockController(
    ISupplierService supplierService,
    IStockItemService stockItemService,
    IScheduleService scheduleService,
    IMaterialProducedService materialProducedService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var suppliers = await supplierService.GetAllSuppliers();
        ViewBag.Suppliers = suppliers;

        var models = await stockItemService.GetDistinctModels();
        ViewBag.Models = models;

        var parts = await stockItemService.GetDistinctStockItemsByInventoryId();
        ViewBag.Parts = parts;

        return View();
    }

    [HttpPost]
    [Route("Stock/GetData")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        try
        {
            var searchTerm = request.Search?.Value;
            if (!string.IsNullOrEmpty(request.PartId))
            {
                searchTerm = request.PartId;
            }

            var totalFiltered = await stockItemService.CountStockItemsWithFilters(
                supplierName: request.SupplierCode,
                model: request.ModelId,
                inventoryItemId: null,
                searchTerm: searchTerm
            );

            var pageNumber = (request.Start / request.Length) + 1;
            var pageSize = request.Length;

            var stockItems = await stockItemService.GetStockItemsWithFilters(
                supplierName: request.SupplierCode,
                model: request.ModelId,
                inventoryItemId: null,
                searchTerm: searchTerm,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            var data = stockItems.Select(s => new
            {
                inventoryItemId = s.InventoryItemId,
                productCode = s.ProductCode,
                supplierName = s.SupplierName,
                unitValue = s.NewCost,
                quantity = s.OnHand,
                totalValue = s.TotalValue,
                itemType = s.OriginOfGoods,
            });

            return Json(
                new
                {
                    draw = request.Draw,
                    recordsTotal = totalFiltered,
                    recordsFiltered = totalFiltered,
                    data,
                }
            );
        }
        catch (Exception ex)
        {
            return Json(
                new
                {
                    draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = ex.Message,
                }
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        string? supplierName,
        string? model,
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        var stockItems = await stockItemService.GetStockItemsWithFilters(
            supplierName: supplierName,
            model: model,
            inventoryItemId: id,
            searchTerm: searchTerm
        );

        var stockItem = stockItems.FirstOrDefault();
        if (stockItem == null)
        {
            return NotFound();
        }

        ViewBag.PecaNome = $"{stockItem.ProductCode} - {stockItem.Description}";
        var unitValue = stockItem.NewCost ?? 35.00m;

        var concatenatedSegments = stockItem.ProductCode;
        var customerItemExt = StringHelper.ExtractMiddlePartFromCode(stockItem.ProductCode);

        var chartData = await scheduleService.GetChartData(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate
        );
        ViewBag.ChartData = chartData;

        var consumptionByWeek = await materialProducedService.GetConsumptionByWeek(
            concatenatedSegments: concatenatedSegments,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate
        );

        var quantityMatrix = await scheduleService.GetQuantityMatrixWithConsumption(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate,
            consumptionByWeek: consumptionByWeek,
            stockOnHand: stockItem.OnHand
        );
        ViewBag.QuantityMatrix = quantityMatrix;

        var valueMatrix = await scheduleService.GetValueMatrixWithConsumption(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate,
            unitValue: unitValue,
            consumptionByWeek: consumptionByWeek
        );
        ViewBag.ValueMatrix = valueMatrix;

        ViewBag.CurrentWeek = scheduleService.GetCurrentWeek();

        return View();
    }
}
