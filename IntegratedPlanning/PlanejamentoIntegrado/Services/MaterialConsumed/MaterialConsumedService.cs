using System.Globalization;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class MaterialConsumedService(IRepository<MaterialConsumed> materialConsumedRepository)
    : IMaterialConsumedService
{
    private const int DefaultOrganizationId = 192;

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

        var materials = await materialConsumedRepository.GetAll(filter: m =>
            m.OrganizationId == DefaultOrganizationId
            && !string.IsNullOrEmpty(concatenatedSegments)
            && m.ConcatenatedSegments == concatenatedSegments
            && m.TransactionDate.HasValue
            && m.TransactionDate.Value.Date >= creationStartDate.Value.Date
            && m.TransactionDate.Value.Date <= creationEndDate.Value.Date
        );

        var consumptionByWeek = materials
            .Where(m => m.TransactionDate.HasValue)
            .GroupBy(m => GetISOWeekNumber(m.TransactionDate!.Value))
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
