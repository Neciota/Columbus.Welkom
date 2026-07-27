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

        /// <summary>
        /// Reads every calculated flight of the configured year from the Venira database.
        /// </summary>
        Task<IEnumerable<Race>> ReadRacesFromVeniraAsync();

        /// <summary>
        /// Stores the races that are not in the database yet, leaving existing ones alone.
        /// </summary>
        Task SyncRacesAsync(IEnumerable<Race> races);

        Task StoreRaceAsync(Race race);
    }
}