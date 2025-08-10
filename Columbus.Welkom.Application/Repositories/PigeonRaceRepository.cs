using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRaceRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<PigeonRaceEntity>(contextFactory), IPigeonRaceRepository
    {
        public async Task DeleteAllByRaceCodeAsync(string raceCode)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            await context.PigeonRaces.Where(pr => pr.RaceCode == raceCode)
                .ExecuteDeleteAsync();
        }
    }
}
