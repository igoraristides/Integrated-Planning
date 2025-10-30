namespace PlanejamentoIntegrado.Services;

public interface IMaterialProducedService
{
    Task<Dictionary<string, decimal>> GetConsumptionByWeek(
        string? concatenatedSegments,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    );
}
