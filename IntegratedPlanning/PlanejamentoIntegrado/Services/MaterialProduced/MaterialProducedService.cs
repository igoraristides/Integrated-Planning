using System.Globalization;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class MaterialProducedService(IRepository<MaterialProduced> materialProducedRepository)
    : IMaterialProducedService
{
    private const int DefaultOrganizationId = 83;

    public async Task<Dictionary<string, decimal>> GetConsumptionByWeek(
        string? concatenatedSegments,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        if (!creationStartDate.HasValue || !creationEndDate.HasValue)
        {
            creationEndDate = DateTime.Now.Date;
            creationStartDate = creationEndDate.Value.AddDays(-7 * 10);
        }

        var materials = await materialProducedRepository.GetAll(filter: m =>
            m.OrgId == DefaultOrganizationId
            && !string.IsNullOrEmpty(concatenatedSegments)
            && m.ConcatenatedSegments == concatenatedSegments
            && m.InvoiceDate.HasValue
            && m.InvoiceDate.Value >= creationStartDate.Value
            && m.InvoiceDate.Value <= creationEndDate.Value
        );

        var consumptionByWeek = materials
            .Where(m => m.InvoiceDate.HasValue)
            .GroupBy(m => GetISOWeekNumber(m.InvoiceDate!.Value))
            .ToDictionary(g => g.Key, g => g.Sum(m => m.TransactionQuantity ?? 0));

        return consumptionByWeek;
    }

    private static string GetISOWeekNumber(DateTime date)
    {
        var calendar = CultureInfo.CurrentCulture.Calendar;
        var weekNumber = calendar.GetWeekOfYear(
            date,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday
        );

        var yearTwoDigits = date.Year % 100;
        return $"{yearTwoDigits:D2}{weekNumber:D2}";
    }
}
