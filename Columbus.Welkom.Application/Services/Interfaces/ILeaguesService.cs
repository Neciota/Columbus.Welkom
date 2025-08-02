using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ILeaguesService
    {
        Task AddLeagueAsync(League league);
        Task<Leagues> GetLeaguesAsync();
        Task UpdateLeagueAsync(League league);
        Task DeleteLeagueAsync(League league);
        Task ExportAsync(Leagues leagues);
    }
}