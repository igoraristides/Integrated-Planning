using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejamentoIntegrado.Helpers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;

namespace PlanejamentoIntegrado.Controllers;

[Authorize]
public class CompareVariationController(
    ISupplierService supplierService,
    IStockItemService stockItemService,
    IScheduleService scheduleService
) : Controller
{
    private const int DefaultOrganizationId = 192;

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

    private static DateTime? ParseDate(string? dateString)
    {
        if (
            !string.IsNullOrEmpty(dateString)
            && DateTime.TryParseExact(
                dateString,
                "dd/MM/yyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out var parsedDate
            )
        )
        {
            return parsedDate;
        }
        return null;
    }

    [HttpPost]
    [Route("CompareVariation/GetData")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        try
        {
            var startDate = ParseDate(request.StartDate);
            var endDate = ParseDate(request.EndDate);
            var creationStartDate = ParseDate(request.CreationStartDate);
            var creationEndDate = ParseDate(request.CreationEndDate);

            var searchTerm = request.Search?.Value;
            if (!string.IsNullOrEmpty(request.PartId))
            {
                searchTerm = request.PartId;
            }

            var recordsFiltered = await stockItemService.CountStockItemsWithFilters(
                supplierName: request.SupplierCode,
                model: request.ModelId,
                inventoryItemId: null,
                searchTerm: searchTerm
            );

            var pageNumber = request.Start / request.Length + 1;
            var pageSize = request.Length;

            var pagedItems = await stockItemService.GetStockItemsWithFilters(
                supplierName: request.SupplierCode,
                model: request.ModelId,
                inventoryItemId: null,
                searchTerm: searchTerm,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            var viewModel = new List<CompareVariationViewModel>();
            foreach (var item in pagedItems)
            {
                var customerItemExt = StringHelper.ExtractMiddlePartFromCode(item.ProductCode);

                var result = await scheduleService.GetLastQuantityAndVariation(
                    customerItemExt: customerItemExt,
                    startDate: startDate,
                    endDate: endDate,
                    creationStartDate: creationStartDate,
                    creationEndDate: creationEndDate
                );

                viewModel.Add(
                    new CompareVariationViewModel
                    {
                        InventoryItemId = item.InventoryItemId,
                        ProductCode = item.ProductCode,
                        Description = item.Description,
                        SupplierName = item.SupplierName,
                        UnitValue = item.NewCost,
                        DemandQuantity = result.LastQuantity ?? 0,
                        TotalValue = (result.LastQuantity ?? 0) * (item.NewCost ?? 0),
                        ItemType = item.OriginOfGoods,
                        Variation = result.Variation,
                    }
                );
            }

            return Json(
                new
                {
                    draw = request.Draw,
                    recordsTotal = recordsFiltered,
                    recordsFiltered,
                    data = viewModel,
                }
            );
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
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

        var customerItemExt = StringHelper.ExtractMiddlePartFromCode(stockItem.ProductCode);

        var chartData = await scheduleService.GetChartData(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate
        );
        ViewBag.ChartData = chartData;

        var quantityMatrix = await scheduleService.GetQuantityMatrix(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate
        );
        ViewBag.QuantityMatrix = quantityMatrix;

        var valueMatrix = await scheduleService.GetValueMatrix(
            customerItemExt: customerItemExt,
            startDate: startDate,
            endDate: endDate,
            creationStartDate: creationStartDate,
            creationEndDate: creationEndDate,
            unitValue: unitValue
        );
        ViewBag.ValueMatrix = valueMatrix;

        ViewBag.CurrentWeek = scheduleService.GetCurrentWeek();

        return View();
    }
}
