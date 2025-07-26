using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IRaceService
    {
        Task DeleteRaceByCodeAsync(string code);
        Task<IEnumerable<SimpleRace>> GetAllRacesByTypeAsync(RaceType type);
        Task<IEnumerable<SimpleRace>> GetAllRacesAsync();
        Task<Race> GetRaceByCodeAsync(string code);
        Task OverwriteRacesAsync(IEnumerable<Race> races);
        Task<Race?> ReadRaceAsync();
        Task<IEnumerable<Race>> ReadRacesAsync();
        Task StoreRaceAsync(Race race);
    }
}