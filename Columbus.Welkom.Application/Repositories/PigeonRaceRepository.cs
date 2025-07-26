using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRaceRepository(DataContext context) : BaseRepository<PigeonRaceEntity>(context), IPigeonRaceRepository
    {
        public async Task DeleteAllByRaceCodeAsync(string raceCode)
        {
            await _context.PigeonRaces.Where(pr => pr.RaceCode == raceCode)
                .ExecuteDeleteAsync();
        }
    }
}
