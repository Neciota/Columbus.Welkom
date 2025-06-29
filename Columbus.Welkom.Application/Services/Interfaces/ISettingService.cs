namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISettingService
    {
        int Year { get; }
        int Club { get; }

        Task SetYearAsync(int year);
        Task SetClubAsync(int club);
    }
}
