using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<PigeonEntity>(contextFactory), IPigeonRepository
    {
        public async Task<PigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Pigeons.Where(p => p.Id.CountryCode == pigeonId.CountryCode)
                .Where(p => p.Id.Year == pigeonId.Year)
                .Where(p => p.Id.RingNumber == pigeonId.RingNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PigeonEntity>> GetByPigeonIdsAsync(IEnumerable<PigeonId> pigeonIds)
        {
            DataContext context = _contextFactory.CreateDbContext();

            HashSet<PigeonId> uniqueIds = pigeonIds.ToHashSet();

            var result = await context.Pigeons
                .Where(p => pigeonIds.Select(p => p.CountryCode).Contains(p.Id.CountryCode))
                .Where(p => pigeonIds.Select(p => p.Year).Contains(p.Id.Year))
                .Where(p => pigeonIds.Select(p => p.RingNumber).Contains(p.Id.RingNumber))
                .ToListAsync();

            return result.Where(p => uniqueIds.Contains(p.Id)).ToList();
        }
    }
}
