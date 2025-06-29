using Columbus.Models.Race;
using Columbus.Welkom.Application.Models;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IRaceService
    {
        Task DeleteRaceByCodeAsync(string code);
        Task<IEnumerable<SimpleRace>> GetAllRacesByTypeAsync(RaceType type);
        Task<IEnumerable<SimpleRace>> GetAllRacesAsync();
        Task<Race> GetRaceByCodeAsync(string code);
        Task OverwriteRacesAsync(IEnumerable<Race> races);
        Task<Race> ReadRaceFromFileAsync(string filePath);
        Task<IEnumerable<Race>> ReadRacesFromDirectoryAsync(string directoryPath);
        Task StoreRaceAsync(Race race);
    }
}