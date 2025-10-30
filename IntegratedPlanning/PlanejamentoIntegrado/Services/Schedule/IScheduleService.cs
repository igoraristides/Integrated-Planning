using PlanejamentoIntegrado.Enums;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Services;

public interface IScheduleService
{
    Task<(decimal? LastQuantity, VariationType? Variation)> GetLastQuantityAndVariation(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    );

    Task<List<ChartDataPoint>> GetChartData(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    );

    Task<List<MatrixRow>> GetQuantityMatrix(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    );

    Task<List<MatrixRow>> GetValueMatrix(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        decimal unitValue
    );

    Task<List<MatrixRow>> GetQuantityMatrixWithConsumption(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        Dictionary<string, decimal> consumptionByWeek,
        decimal? stockOnHand = null
    );

    Task<List<MatrixRow>> GetValueMatrixWithConsumption(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        decimal unitValue,
        Dictionary<string, decimal> consumptionByWeek
    );

    string GetCurrentWeek();
}
