using System.Linq.Expressions;
using Moq;
using PlanejamentoIntegrado.Enums;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Services.Schedule;

public class ScheduleServiceTests : BaseTest
{
    private readonly Mock<
        IRepository<PlanejamentoIntegrado.Models.Schedule>
    > _mockScheduleRepository;
    private readonly ScheduleService _scheduleService;

    public ScheduleServiceTests()
    {
        _mockScheduleRepository = new Mock<IRepository<PlanejamentoIntegrado.Models.Schedule>>();
        _scheduleService = new ScheduleService(_mockScheduleRepository.Object);
    }

    private static List<PlanejamentoIntegrado.Models.Schedule> CreateTestSchedules()
    {
        return new List<PlanejamentoIntegrado.Models.Schedule>
        {
            new()
            {
                HeaderId = 1001,
                OrganizationId = 83,
                CustomerItemExt = "TEST-ITEM-001",
                ItemDetailQuantity = 100,
                StartDateTime = DateTime.Now.AddDays(-7),
                CreatedAt = DateTime.Now.AddDays(-10),
            },
            new()
            {
                HeaderId = 1001,
                OrganizationId = 83,
                CustomerItemExt = "TEST-ITEM-001",
                ItemDetailQuantity = 150,
                StartDateTime = DateTime.Now.AddDays(-6),
                CreatedAt = DateTime.Now.AddDays(-10),
            },
            new()
            {
                HeaderId = 1002,
                OrganizationId = 83,
                CustomerItemExt = "TEST-ITEM-001",
                ItemDetailQuantity = 200,
                StartDateTime = DateTime.Now.AddDays(-3),
                CreatedAt = DateTime.Now.AddDays(-5),
            },
            new()
            {
                HeaderId = 1002,
                OrganizationId = 83,
                CustomerItemExt = "TEST-ITEM-001",
                ItemDetailQuantity = 180,
                StartDateTime = DateTime.Now.AddDays(-2),
                CreatedAt = DateTime.Now.AddDays(-5),
            },
        };
    }

    [Fact]
    public async Task GetLastQuantityAndVariation_WithValidData_ReturnsQuantityAndVariation()
    {
        var schedules = CreateTestSchedules();
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var result = await _scheduleService.GetLastQuantityAndVariation(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now
        );

        Assert.NotNull(result.LastQuantity);
        Assert.Equal(380, result.LastQuantity);
        Assert.Equal(VariationType.Up, result.Variation);
    }

    [Fact]
    public async Task GetLastQuantityAndVariation_WithNoData_ReturnsNull()
    {
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(new List<PlanejamentoIntegrado.Models.Schedule>());

        var result = await _scheduleService.GetLastQuantityAndVariation(
            "NON-EXISTENT",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now
        );

        Assert.Null(result.LastQuantity);
        Assert.Null(result.Variation);
    }

    [Fact]
    public async Task GetChartData_ReturnsChartDataPoints()
    {
        var schedules = CreateTestSchedules();
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var result = await _scheduleService.GetChartData(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now
        );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(250, result[0].Quantity);
        Assert.Equal(380, result[1].Quantity);
    }

    [Fact]
    public async Task GetQuantityMatrix_ReturnsMatrixRows()
    {
        var schedules = CreateTestSchedules();
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var result = await _scheduleService.GetQuantityMatrix(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now
        );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.NotNull(result[0].WeekValues);
        Assert.NotNull(result[0].CV);
        Assert.NotNull(result[0].CLCV);
    }

    [Fact]
    public async Task GetValueMatrix_ReturnsMatrixRowsWithValues()
    {
        var schedules = CreateTestSchedules();
        var unitValue = 10.5m;
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var result = await _scheduleService.GetValueMatrix(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now,
            unitValue
        );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(250 * unitValue, result[0].Total);
        Assert.Equal(380 * unitValue, result[1].Total);
    }

    [Fact]
    public async Task GetQuantityMatrixWithConsumption_AddsConsumptionRow()
    {
        var schedules = CreateTestSchedules();
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var consumptionByWeek = new Dictionary<string, decimal>
        {
            { "2443", 50m },
            { "2444", 60m },
        };

        var result = await _scheduleService.GetQuantityMatrixWithConsumption(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now,
            consumptionByWeek,
            500m
        );

        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, r => r.OriginName != null && r.OriginName.Contains("Consumo"));
    }

    [Fact]
    public async Task GetValueMatrixWithConsumption_AddsConsumptionRow()
    {
        var schedules = CreateTestSchedules();
        var unitValue = 10.5m;
        _mockScheduleRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.Schedule, bool>>>(),
                    0,
                    0,
                    It.IsAny<
                        Func<
                            IQueryable<PlanejamentoIntegrado.Models.Schedule>,
                            IOrderedQueryable<PlanejamentoIntegrado.Models.Schedule>
                        >
                    >()
                )
            )
            .ReturnsAsync(schedules);

        var consumptionByWeek = new Dictionary<string, decimal>
        {
            { "2443", 50m },
            { "2444", 60m },
        };

        var result = await _scheduleService.GetValueMatrixWithConsumption(
            "TEST-ITEM-001",
            DateTime.Now.AddDays(-10),
            DateTime.Now,
            DateTime.Now.AddDays(-15),
            DateTime.Now,
            unitValue,
            consumptionByWeek
        );

        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, r => r.OriginName != null && r.OriginName.Contains("Consumo"));
    }

    [Fact]
    public void GetCurrentWeek_ReturnsValidWeekNumber()
    {
        var result = _scheduleService.GetCurrentWeek();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(4, result.Length);
    }
}
