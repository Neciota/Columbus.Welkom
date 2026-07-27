namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Supplies the sunrise/sunset table that NF-14 neutralization is calculated against.
/// </summary>
public interface ISolarPeriodProvider
{
    /// <param name="year">The season the times are wanted for.</param>
    Task<Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)>> GetSolarPeriodsAsync(int year);
}
