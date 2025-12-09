namespace PlanejamentoIntegrado.Services;

public interface IMaterialConsumedService
{
    Task<Dictionary<string, decimal>> GetConsumptionByWeek(
        string? concatenatedSegments,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    );
}
