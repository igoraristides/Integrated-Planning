using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Enums;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class ScheduleService(IRepository<Schedule> scheduleRepository) : IScheduleService
{
    private const int DefaultOrganizationId = 83;

    private async Task<List<Schedule>> GetSchedulesByFilter(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        if (
            !startDate.HasValue
            && !endDate.HasValue
            && !creationStartDate.HasValue
            && !creationEndDate.HasValue
        )
        {
            endDate = DateTime.Now;
            startDate = endDate.Value.AddDays(-7 * 10);
            creationEndDate = DateTime.Now.Date;
            creationStartDate = creationEndDate.Value.AddDays(-7 * 10);
        }

        Expression<Func<Schedule, bool>> filter = s => s.OrganizationId == DefaultOrganizationId;

        if (!string.IsNullOrEmpty(customerItemExt))
        {
            var upperCustomerItemExt = customerItemExt.ToUpper();

            filter = ApplyFilter(
                filter,
                s =>
                    s.CustomerItemExt != null
                    && (
                        s.CustomerItemExt.ToUpper() == upperCustomerItemExt
                        || s.CustomerItemExt.ToUpper().Contains(upperCustomerItemExt)
                        || EF.Functions.Like(
                            s.CustomerItemExt.ToUpper(),
                            $"%{upperCustomerItemExt}%"
                        )
                    )
            );
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            filter = ApplyFilter(
                filter,
                s => s.StartDateTime >= startDate.Value && s.StartDateTime <= endDate.Value
            );
        }

        if (creationStartDate.HasValue && creationEndDate.HasValue)
        {
            filter = ApplyFilter(
                filter,
                s => s.CreatedAt >= creationStartDate.Value && s.CreatedAt <= creationEndDate.Value
            );
        }

        var schedules = await scheduleRepository.GetAll(
            filter: filter,
            orderBy: q =>
                q.OrderBy(s => s.HeaderId).ThenBy(s => s.LineId).ThenBy(s => s.StartDateTime)
        );

        return schedules;
    }

    public async Task<(
        decimal? LastQuantity,
        VariationType? Variation
    )> GetLastQuantityAndVariation(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        var schedules = await GetSchedulesByFilter(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate
        );

        var groupedByHeader = schedules
            .Where(s => s.StartDateTime.HasValue)
            .GroupBy(s => s.HeaderId)
            .Select(g => new { HeaderId = g.Key, Total = g.Sum(s => s.ItemDetailQuantity ?? 0) })
            .OrderBy(x => x.HeaderId)
            .ToList();

        if (groupedByHeader.Count == 0)
        {
            return (null, null);
        }

        var firstTotal = groupedByHeader.First().Total;
        var lastTotal = groupedByHeader.Last().Total;

        var difference = firstTotal - lastTotal;

        VariationType? variation = null;

        if (difference < 0)
        {
            variation = VariationType.Up;
        }
        else if (difference > 0)
        {
            variation = VariationType.Down;
        }

        return (lastTotal, variation);
    }

    public async Task<List<ChartDataPoint>> GetChartData(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        var schedules = await GetSchedulesByFilter(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate
        );

        var chartData = schedules
            .GroupBy(s => s.HeaderId)
            .Select(g => new ChartDataPoint
            {
                Date = $"{g.Key} - {g.FirstOrDefault()?.CreatedAt?.ToString("dd/MM/yyyy") ?? ""}",
                Quantity = g.Sum(s => s.ItemDetailQuantity ?? 0),
            })
            .OrderBy(c => c.Date)
            .ToList();

        return chartData;
    }

    public async Task<List<MatrixRow>> GetQuantityMatrix(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate
    )
    {
        var schedules = await GetSchedulesByFilter(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate
        );

        var matrixData = schedules
            .Where(s => s.StartDateTime.HasValue)
            .GroupBy(s => s.HeaderId)
            .Select(g => new MatrixRow
            {
                OriginName =
                    $"{g.Key} - {g.FirstOrDefault()?.CreatedAt?.ToString("dd/MM/yyyy") ?? ""}",
                WeekValues = g.GroupBy(s => GetISOWeekNumber(s.StartDateTime!.Value))
                    .ToDictionary(
                        wg => wg.Key,
                        wg => (decimal?)wg.Sum(s => s.ItemDetailQuantity ?? 0)
                    ),
                Total = g.Sum(s => s.ItemDetailQuantity ?? 0),
                CV = CalculateCV(g.Select(s => s.ItemDetailQuantity ?? 0).ToList()),
                CLCV = ClassifyCV(CalculateCV(g.Select(s => s.ItemDetailQuantity ?? 0).ToList())),
            })
            .OrderBy(m => m.OriginName)
            .ToList();

        return matrixData;
    }

    public async Task<List<MatrixRow>> GetValueMatrix(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        decimal unitValue
    )
    {
        var schedules = await GetSchedulesByFilter(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate
        );

        var matrixData = schedules
            .Where(s => s.StartDateTime.HasValue)
            .GroupBy(s => s.HeaderId)
            .Select(g => new MatrixRow
            {
                OriginName =
                    $"{g.Key} - {g.FirstOrDefault()?.CreatedAt?.ToString("dd/MM/yyyy") ?? ""}",
                WeekValues = g.GroupBy(s => GetISOWeekNumber(s.StartDateTime!.Value))
                    .ToDictionary(
                        wg => wg.Key,
                        wg => (decimal?)(wg.Sum(s => s.ItemDetailQuantity ?? 0) * unitValue)
                    ),
                Total = g.Sum(s => s.ItemDetailQuantity ?? 0) * unitValue,
                CV = CalculateCV(g.Select(s => (s.ItemDetailQuantity ?? 0) * unitValue).ToList()),
                CLCV = ClassifyCV(
                    CalculateCV(g.Select(s => (s.ItemDetailQuantity ?? 0) * unitValue).ToList())
                ),
            })
            .OrderBy(m => m.OriginName)
            .ToList();

        return matrixData;
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

    private static decimal CalculateCV(List<decimal> values)
    {
        if (values.Count == 0)
            return 0;

        var mean = values.Average();
        if (mean == 0)
            return 0;

        var variance = values.Sum(v => Math.Pow((double)(v - mean), 2)) / values.Count;
        var stdDev = Math.Sqrt(variance);

        return (decimal)(stdDev / (double)mean);
    }

    private static string ClassifyCV(decimal cv)
    {
        if (cv < 0.5m)
            return "L";
        if (cv < 1.0m)
            return "M";
        return "H";
    }

    public async Task<List<MatrixRow>> GetQuantityMatrixWithConsumption(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        Dictionary<string, decimal> consumptionByWeek,
        decimal? stockOnHand = null
    )
    {
        var matrixData = await GetQuantityMatrix(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate
        );

        var allWeeks = CollectAllWeeks(matrixData, consumptionByWeek);

        matrixData = NormalizeWeekValues(matrixData, allWeeks);

        if (allWeeks.Count > 0)
        {
            var consumptionRow = CreateConsumptionRow(
                "Consumo em Quantidades",
                consumptionByWeek,
                allWeeks,
                unitMultiplier: 1
            );
            matrixData.Add(consumptionRow);
        }

        if (stockOnHand.HasValue && stockOnHand.Value > 0)
        {
            matrixData = ApplyDoHToLastRow(matrixData, stockOnHand.Value);
        }

        return matrixData;
    }

    public async Task<List<MatrixRow>> GetValueMatrixWithConsumption(
        string? customerItemExt,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? creationStartDate,
        DateTime? creationEndDate,
        decimal unitValue,
        Dictionary<string, decimal> consumptionByWeek
    )
    {
        var matrixData = await GetValueMatrix(
            customerItemExt,
            startDate,
            endDate,
            creationStartDate,
            creationEndDate,
            unitValue
        );

        var allWeeks = CollectAllWeeks(matrixData, consumptionByWeek);
        matrixData = NormalizeWeekValues(matrixData, allWeeks);

        if (allWeeks.Count > 0)
        {
            var consumptionRow = CreateConsumptionRow(
                "Consumo em Valores",
                consumptionByWeek,
                allWeeks,
                unitValue
            );
            matrixData.Add(consumptionRow);
        }

        return matrixData;
    }

    public string GetCurrentWeek()
    {
        return GetISOWeekNumber(DateTime.Now);
    }

    private List<string> CollectAllWeeks(
        List<MatrixRow> matrixData,
        Dictionary<string, decimal> consumptionByWeek
    )
    {
        var allWeeksSet = new HashSet<string>();

        foreach (var row in matrixData)
        {
            if (row.WeekValues != null)
            {
                foreach (var week in row.WeekValues.Keys)
                {
                    allWeeksSet.Add(week);
                }
            }
        }

        foreach (var week in consumptionByWeek.Keys)
        {
            allWeeksSet.Add(week);
        }

        return allWeeksSet.OrderBy(w => w).ToList();
    }

    private static List<MatrixRow> NormalizeWeekValues(
        List<MatrixRow> matrixData,
        List<string> allWeeks
    )
    {
        var normalizedData = new List<MatrixRow>();

        foreach (var row in matrixData)
        {
            var normalizedRow = new MatrixRow
            {
                OriginName = row.OriginName,
                WeekValues = row.WeekValues ?? new Dictionary<string, decimal?>(),
                Total = row.Total,
                CV = row.CV,
                CLCV = row.CLCV,
                DoH = row.DoH,
            };

            foreach (var week in allWeeks)
            {
                if (!normalizedRow.WeekValues.ContainsKey(week))
                {
                    normalizedRow.WeekValues[week] = null;
                }
            }

            normalizedData.Add(normalizedRow);
        }

        return normalizedData;
    }

    private MatrixRow CreateConsumptionRow(
        string originName,
        Dictionary<string, decimal> consumptionByWeek,
        List<string> allWeeks,
        decimal unitMultiplier
    )
    {
        var currentWeek = GetCurrentWeek();

        var consumptionRow = new MatrixRow
        {
            OriginName = originName,
            WeekValues = new Dictionary<string, decimal?>(),
            Total = null,
            CV = null,
            CLCV = null,
            DoH = null,
        };

        foreach (var week in allWeeks)
        {
            if (
                string.Compare(week, currentWeek, StringComparison.Ordinal) < 0
                && consumptionByWeek.ContainsKey(week)
            )
            {
                consumptionRow.WeekValues[week] = consumptionByWeek[week] * unitMultiplier;
            }
            else
            {
                consumptionRow.WeekValues[week] = null;
            }
        }

        return consumptionRow;
    }

    private static List<MatrixRow> ApplyDoHToLastRow(
        List<MatrixRow> matrixData,
        decimal stockOnHand
    )
    {
        if (matrixData.Count == 0)
            return matrixData;

        var lastNonConsumptionIndex = matrixData
            .Select((row, index) => new { row, index })
            .Where(x => x.row.OriginName == null || !x.row.OriginName.Contains("Consumo"))
            .Select(x => (int?)x.index)
            .LastOrDefault();

        if (!lastNonConsumptionIndex.HasValue)
            return matrixData;

        var calculatedDoH = CalculateDoH(matrixData[lastNonConsumptionIndex.Value], stockOnHand);

        if (calculatedDoH.HasValue)
        {
            var updatedData = new List<MatrixRow>(matrixData);
            var rowToUpdate = updatedData[lastNonConsumptionIndex.Value];

            updatedData[lastNonConsumptionIndex.Value] = new MatrixRow
            {
                OriginName = rowToUpdate.OriginName,
                WeekValues = rowToUpdate.WeekValues,
                Total = rowToUpdate.Total,
                CV = rowToUpdate.CV,
                CLCV = rowToUpdate.CLCV,
                DoH = calculatedDoH.Value,
            };

            return updatedData;
        }

        return matrixData;
    }

    private static decimal? CalculateDoH(MatrixRow row, decimal stockOnHand)
    {
        if (row.WeekValues == null || row.WeekValues.Count == 0)
            return null;

        var numberOfWeeks = row.WeekValues.Values.Count(v => v.HasValue && v.Value > 0);

        if (numberOfWeeks == 0)
            return null;

        var totalDays = numberOfWeeks * 5;

        return Math.Round(stockOnHand / totalDays, 1);
    }

    private static Expression<Func<Schedule, bool>> ApplyFilter(
        Expression<Func<Schedule, bool>> defaultFilter,
        Expression<Func<Schedule, bool>> filterToAdd
    )
    {
        var param = defaultFilter.Parameters[0];

        var invoked = Expression.Invoke(filterToAdd, param);

        return Expression.Lambda<Func<Schedule, bool>>(
            Expression.AndAlso(defaultFilter.Body, invoked),
            param
        );
    }
}
