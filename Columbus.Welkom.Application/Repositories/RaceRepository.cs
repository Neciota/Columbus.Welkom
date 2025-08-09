using Columbus.Models.Race;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class RaceRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<RaceEntity>(contextFactory), IRaceRepository
    {
        public async Task<int> DeleteRangeAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.ExecuteDeleteAsync();
        }

        public async Task<ICollection<SimpleRaceEntity>> GetAllSimpleAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.OrderByDescending(r => r.StartTime)
                .Select(r => new SimpleRaceEntity(r.Number, r.Type, r.Name, r.Code, r.StartTime, r.Latitude, r.Longitude, r.PigeonRaces!.Select(pr => pr.Pigeon!.Owner).Distinct().Count(), r.PigeonRaces!.Count()))
                .ToListAsync();
        }

        public async Task<ICollection<RaceEntity>> GetAllByTypesAsync(RaceType[] types)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => types.Contains(r.Type))
                .Include(r => r.PigeonRaces!)
                .ThenInclude(pr => pr.Pigeon!)
                .ThenInclude(p => p.Owner)
                .ToListAsync();
        }

        public async Task<ICollection<SimpleRaceEntity>> GetAllSimpleByTypesAsync(RaceType[] types)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => types.Contains(r.Type))
                .Select(r => new SimpleRaceEntity(r.Number, r.Type, r.Name, r.Code, r.StartTime, r.Latitude, r.Longitude, r.PigeonRaces!.Select(pr => pr.Pigeon!.Owner).Distinct().Count(), r.PigeonRaces!.Count()))
                .ToListAsync();
        }

        public async Task<RaceEntity> GetByCodeAsync(string code)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => r.Code == code)
                .Include(r => r.PigeonRaces!)
                .ThenInclude(pr => pr.Pigeon!)
                .ThenInclude(p => p.Owner)
                .FirstAsync();
        }

        public async Task<bool> IsRaceCodePresentAsync(string code)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.AnyAsync(r => r.Code == code);
        }

        public async Task<int> DeleteRaceByCodeAsync(string code)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => r.Code == code)
                .ExecuteDeleteAsync();
        }

        public async Task<RaceEntity> GetMostRecentRaceAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.OrderByDescending(r => r.StartTime)
                .FirstAsync();
        }
    }
}
