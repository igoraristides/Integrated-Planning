using System.Linq.Expressions;
using Moq;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Services.MaterialProduced;

public class MaterialProducedServiceTests : BaseTest
{
    private readonly Mock<
        IRepository<PlanejamentoIntegrado.Models.MaterialProduced>
    > _mockMaterialProducedRepository;
    private readonly MaterialProducedService _materialProducedService;

    public MaterialProducedServiceTests()
    {
        _mockMaterialProducedRepository =
            new Mock<IRepository<PlanejamentoIntegrado.Models.MaterialProduced>>();
        _materialProducedService = new MaterialProducedService(
            _mockMaterialProducedRepository.Object
        );
    }

    private static List<PlanejamentoIntegrado.Models.MaterialProduced> CreateTestMaterialsProduced()
    {
        return new List<PlanejamentoIntegrado.Models.MaterialProduced>
        {
            new()
            {
                TransactionId = 1,
                ConcatenatedSegments = "TEST-ITEM-001",
                TransactionQuantity = 100m,
                InvoiceDate = DateTime.Now.AddDays(-7),
            },
            new()
            {
                TransactionId = 2,
                ConcatenatedSegments = "TEST-ITEM-001",
                TransactionQuantity = 150m,
                InvoiceDate = DateTime.Now.AddDays(-6),
            },
            new()
            {
                TransactionId = 3,
                ConcatenatedSegments = "TEST-ITEM-001",
                TransactionQuantity = 200m,
                InvoiceDate = DateTime.Now.AddDays(-3),
            },
        };
    }

    [Fact]
    public async Task GetConsumptionByWeek_WithValidData_ReturnsDictionary()
    {
        var materialsProduced = CreateTestMaterialsProduced();
        _mockMaterialProducedRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<
                        Expression<Func<PlanejamentoIntegrado.Models.MaterialProduced, bool>>
                    >(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(materialsProduced);

        var result = await _materialProducedService.GetConsumptionByWeek(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now
        );

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Values.All(v => v > 0));
    }

    [Fact]
    public async Task GetConsumptionByWeek_WithNoData_ReturnsEmptyDictionary()
    {
        _mockMaterialProducedRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<
                        Expression<Func<PlanejamentoIntegrado.Models.MaterialProduced, bool>>
                    >(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(new List<PlanejamentoIntegrado.Models.MaterialProduced>());

        var result = await _materialProducedService.GetConsumptionByWeek(
            "NON-EXISTENT",
            DateTime.Now.AddDays(-10),
            DateTime.Now
        );

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetConsumptionByWeek_WithNullDates_UsesDefaultDates()
    {
        var materialsProduced = CreateTestMaterialsProduced();
        _mockMaterialProducedRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<
                        Expression<Func<PlanejamentoIntegrado.Models.MaterialProduced, bool>>
                    >(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(materialsProduced);

        var result = await _materialProducedService.GetConsumptionByWeek(
            "TEST-ITEM-001",
            null,
            null
        );

        Assert.NotNull(result);
    }
}
