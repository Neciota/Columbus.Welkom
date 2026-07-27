using Columbus.Welkom.Application.Models;
using Columbus.Welkom.Application.Venira.Rows;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Reads the NPO sunrise/sunset table from Venira's <c>NEUTTIJD.DB</c>, which is the same table
/// <see cref="SolarPeriods"/> was transcribed from by hand.
/// </summary>
public class VeniraSolarPeriodProvider(IVeniraReader reader) : ISolarPeriodProvider
{
    public async Task<Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)>> GetSolarPeriodsAsync(int year)
    {
        IReadOnlyList<VeniraSolarPeriodRow> rows;
        try
        {
            rows = await reader.GetSolarPeriodsAsync();
        }
        catch (VeniraException)
        {
            // Neutralization must not be the thing that stops a championship from being shown
            // when Venira is unreachable, so fall back to the transcribed tables.
            return GetFallbackSolarPeriods(year);
        }

        Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)> solarPeriods = [];
        foreach (VeniraSolarPeriodRow row in rows)
        {
            // Venira stores the table against a fixed reference year; only month and day carry
            // meaning, so rebase each entry onto the season being calculated.
            if (row.Date is not DateTime date || row.SunUp is not TimeSpan sunUp || row.SunDown is not TimeSpan sunDown)
                continue;

            DateOnly day;
            try
            {
                day = new DateOnly(year, date.Month, date.Day);
            }
            catch (ArgumentOutOfRangeException)
            {
                // 29 February in the reference year, in a year that has no 29 February.
                continue;
            }

            solarPeriods[day] = (
                day.ToDateTime(TimeOnly.FromTimeSpan(sunUp), DateTimeKind.Local),
                day.ToDateTime(TimeOnly.FromTimeSpan(sunDown), DateTimeKind.Local));
        }

        return solarPeriods.Count > 0 ? solarPeriods : GetFallbackSolarPeriods(year);
    }

    private static Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)> GetFallbackSolarPeriods(int year)
    {
        try
        {
            return SolarPeriods.GetSolarPeriods(year);
        }
        catch (NotImplementedException e)
        {
            // The transcribed tables only cover a handful of seasons.
            throw new VeniraException(
                $"De zonop-/zonondergangstijden voor {year} konden niet uit Venira worden gelezen en zijn ook niet ingebouwd.", e);
        }
    }
}
