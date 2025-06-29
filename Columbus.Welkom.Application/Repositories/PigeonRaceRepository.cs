using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRaceRepository : BaseRepository<PigeonRaceEntity>, IPigeonRaceRepository
    {
        public PigeonRaceRepository(DataContext context) : base(context) { }

        public async Task DeleteAllByRaceId(int raceId)
        {
            await _context.PigeonRaces.Where(pr => pr.RaceId == raceId)
                .ExecuteDeleteAsync();
        }
    }
}
