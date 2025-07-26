using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISettingService
    {
        Task SetYearAsync(int year);
        Task SetClubAsync(int club);
        Task<RaceSettings> GetSettingsAsync();
        Task SaveSettingsAsync(RaceSettings settings);
        Task ExportAsync();
        Task ImportAsync();
        string GetDatabasePath();
    }
}
