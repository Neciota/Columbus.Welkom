using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Services.Interfaces;

namespace Columbus.Welkom.Application.Services;

public class SettingService : ISettingService
{
    private readonly SettingsProvider _settingsProvider;

    public SettingService(SettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public int Year { get; private set; } = DateTime.Now.Year;

    public int Club { get; private set; } = 2151;

    public string AppDirectory { get; set; } = string.Empty;

    public Task<Settings> GetSettingsAsync()
    {
        return _settingsProvider.GetSettingsAsync();
    }

    public Task SaveSettingsAsync(Settings settings)
    {
        return _settingsProvider.SaveSettingsAsync(settings);
    }

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
