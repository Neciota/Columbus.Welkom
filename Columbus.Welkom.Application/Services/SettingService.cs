using Columbus.Welkom.Application.Services.Interfaces;

namespace Columbus.Welkom.Application.Services;

public class SettingService : ISettingService
{
    public int Year { get; private set; } = DateTime.Now.Year;

    public int Club { get; private set; } = 2151;

    public Task SetClubAsync(int club)
    {
        Club = club;

        return Task.CompletedTask;
    }

    public Task SetYearAsync(int year)
    {
        Year = year;

        return Task.CompletedTask;
    }
}
