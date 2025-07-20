using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISettingService
    {
        int Year { get; }
        int Club { get; }
        string AppDirectory { get; set; }

        Task SetYearAsync(int year);
        Task SetClubAsync(int club);
        Task<Settings> GetSettingsAsync();
        Task SaveSettingsAsync(Settings settings);
    }
}
