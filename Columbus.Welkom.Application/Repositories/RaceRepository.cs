using Columbus.Models.Race;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class RaceRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<RaceEntity>(contextFactory), IRaceRepository
    {
        public async Task<int> DeleteRangeAsync()
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.ExecuteDeleteAsync();
        }

        public async Task<ICollection<SimpleRaceEntity>> GetAllSimpleAsync()
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.OrderByDescending(r => r.StartTime)
                .Select(r => new SimpleRaceEntity(r.Number, r.Type, r.Name, r.Code, r.StartTime, r.Latitude, r.Longitude, r.PigeonRaces!.Select(pr => pr.Pigeon!.Owner).Distinct().Count(), r.PigeonRaces!.Count()))
                .ToListAsync();
        }

        public async Task<ICollection<RaceEntity>> GetAllByTypesAsync(RaceType[] types)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => types.Contains(r.Type))
                .Include(r => r.PigeonRaces!)
                .ThenInclude(pr => pr.Pigeon!)
                .ThenInclude(p => p.Owner)
                .ToListAsync();
        }

        public async Task<ICollection<SimpleRaceEntity>> GetAllSimpleByTypesAsync(RaceType[] types)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => types.Contains(r.Type))
                .Select(r => new SimpleRaceEntity(r.Number, r.Type, r.Name, r.Code, r.StartTime, r.Latitude, r.Longitude, r.PigeonRaces!.Select(pr => pr.Pigeon!.Owner).Distinct().Count(), r.PigeonRaces!.Count()))
                .ToListAsync();
        }

        public async Task<ICollection<RaceEntity>> GetAllByCodeLettersAsync(string[] codeLetters)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => codeLetters.Contains(r.Code.Substring(0, 1)))
                .Include(r => r.PigeonRaces!)
                .ThenInclude(pr => pr.Pigeon!)
                .ThenInclude(p => p.Owner)
                .ToListAsync();
        }

        public async Task<ICollection<SimpleRaceEntity>> GetAllSimpleByCodeLettersAsync(string[] codeLetters)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => codeLetters.Contains(r.Code.Substring(0, 1)))
                .Select(r => new SimpleRaceEntity(r.Number, r.Type, r.Name, r.Code, r.StartTime, r.Latitude, r.Longitude, r.PigeonRaces!.Select(pr => pr.Pigeon!.Owner).Distinct().Count(), r.PigeonRaces!.Count()))
                .ToListAsync();
        }

        public async Task<ICollection<RaceCodeLetter>> GetCodeLettersAsync()
        {
            using DataContext context = _contextFactory.CreateDbContext();

            // Grouped in memory: a season holds a few dozen races, and a DISTINCT over a
            // projection that carries a RaceType does not translate.
            var codesAndTypes = await context.Races.Select(r => new { r.Code, r.Type })
                .ToListAsync();

            return codesAndTypes.Where(ct => ct.Code.Length > 0)
                .GroupBy(ct => ct.Code[..1])
                .OrderBy(g => g.Key)
                .Select(g => new RaceCodeLetter(g.Key, [.. g.Select(ct => ct.Type).Distinct().Order()]))
                .ToList();
        }

        public async Task<RaceEntity> GetByCodeAsync(string code)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => r.Code == code)
                .Include(r => r.PigeonRaces!)
                .ThenInclude(pr => pr.Pigeon!)
                .ThenInclude(p => p.Owner)
                .FirstAsync();
        }

        public async Task<bool> IsRaceCodePresentAsync(string code)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.AnyAsync(r => r.Code == code);
        }

        public async Task<int> DeleteRaceByCodeAsync(string code)
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.Where(r => r.Code == code)
                .ExecuteDeleteAsync();
        }

        public async Task<RaceEntity> GetMostRecentRaceAsync()
        {
            using DataContext context = _contextFactory.CreateDbContext();

            return await context.Races.OrderByDescending(r => r.StartTime)
                .FirstAsync();
        }
    }
}
