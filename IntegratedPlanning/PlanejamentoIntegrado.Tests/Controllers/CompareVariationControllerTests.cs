using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanejamentoIntegrado.Controllers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Controllers;

public class CompareVariationControllerTests : BaseTest
{
    private readonly Mock<ISupplierService> _mockSupplierService;
    private readonly Mock<IStockItemService> _mockStockItemService;
    private readonly Mock<IScheduleService> _mockScheduleService;
    private readonly CompareVariationController _controller;

    public CompareVariationControllerTests()
    {
        _mockSupplierService = new Mock<ISupplierService>();
        _mockStockItemService = new Mock<IStockItemService>();
        _mockScheduleService = new Mock<IScheduleService>();

        _controller = new CompareVariationController(
            _mockSupplierService.Object,
            _mockStockItemService.Object,
            _mockScheduleService.Object
        );

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "test@test.com") }, "mock")
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };
    }

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        var suppliers = new List<Supplier>
        {
            new() { SupplierCode = "SUP001", SupplierName = "Supplier A" },
        };
        var models = new List<string> { "Model X" };
        var parts = new List<string> { "PART-001" };

        _mockSupplierService.Setup(x => x.GetAllSuppliers()).ReturnsAsync(suppliers);
        _mockStockItemService.Setup(x => x.GetDistinctModels()).ReturnsAsync(models);
        _mockStockItemService
            .Setup(x => x.GetDistinctStockItemsByInventoryId())
            .ReturnsAsync(parts);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult);
    }

    [Fact]
    public async Task GetData_WithValidRequest_ReturnsJsonResult()
    {
        var request = new DataTablesRequest
        {
            Draw = 1,
            Start = 0,
            Length = 10,
        };
        var stockItems = new List<StockItem>
        {
            new()
            {
                InventoryItemId = 1,
                ProductCode = "TEST-PROD-001",
                Description = "Test Product",
                SupplierName = "Supplier A",
                NewCost = 10.5m,
                OriginOfGoods = "Nacional",
            },
        };

        _mockStockItemService
            .Setup(x => x.CountStockItemsWithFilters(null, null, null, null))
            .ReturnsAsync(1);
        _mockStockItemService
            .Setup(x => x.GetStockItemsWithFilters(null, null, null, null, 1, 10))
            .ReturnsAsync(stockItems);

        _mockScheduleService
            .Setup(x =>
                x.GetLastQuantityAndVariation(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync((100m, Enums.VariationType.Up));

        var result = await _controller.GetData(request);

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewResult()
    {
        var stockItem = new StockItem
        {
            InventoryItemId = 1,
            ProductCode = "TEST-PROD-001",
            Description = "Test Product",
            NewCost = 35m,
        };

        _mockStockItemService
            .Setup(x => x.GetStockItemsWithFilters(null, null, 1, null, 0, 0))
            .ReturnsAsync(new List<StockItem> { stockItem });

        _mockScheduleService
            .Setup(x =>
                x.GetChartData(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(new List<ChartDataPoint>());

        _mockScheduleService
            .Setup(x =>
                x.GetQuantityMatrix(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(new List<MatrixRow>());

        _mockScheduleService
            .Setup(x =>
                x.GetValueMatrix(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<decimal>()
                )
            )
            .ReturnsAsync(new List<MatrixRow>());

        _mockScheduleService.Setup(x => x.GetCurrentWeek()).Returns("2443");

        var result = await _controller.Details(1, null, null, null, null, null, null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        _mockStockItemService
            .Setup(x => x.GetStockItemsWithFilters(null, null, 999, null, 0, 0))
            .ReturnsAsync(new List<StockItem>());

        var result = await _controller.Details(999, null, null, null, null, null, null, null);

        Assert.IsType<NotFoundResult>(result);
    }
}
