using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRepository : BaseRepository<PigeonEntity>, IPigeonRepository
    {
        public PigeonRepository(DataContext context) : base(context) { }

        public async Task<PigeonEntity> GetByPigeonIdAsync(PigeonId pigeonId)
        {
            return await _context.Pigeons.Where(p => p.Id.CountryCode == pigeonId.CountryCode)
                .Where(p => p.Id.Year == pigeonId.Year)
                .Where(p => p.Id.RingNumber == pigeonId.RingNumber)
                .FirstAsync();
        }

        public async Task<IEnumerable<PigeonEntity>> GetByPigeonIdsAsync(IEnumerable<PigeonId> pigeonIds)
        {
            HashSet<PigeonId> uniqueIds = pigeonIds.ToHashSet();

            var result = await _context.Pigeons
                .Where(p => pigeonIds.Select(p => p.CountryCode).Contains(p.Id.CountryCode))
                .Where(p => pigeonIds.Select(p => p.Year).Contains(p.Id.Year))
                .Where(p => pigeonIds.Select(p => p.RingNumber).Contains(p.Id.RingNumber))
                .ToListAsync();

            return result.Where(p => uniqueIds.Contains(p.Id)).ToList();
        }
    }
}
