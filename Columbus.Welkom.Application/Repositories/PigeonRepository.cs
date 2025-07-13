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
            return await _context.Pigeons.Where(p => p.Id == pigeonId)
                .FirstAsync();
        }

        public async Task<IEnumerable<PigeonEntity>> GetByPigeonIdsAsync(IEnumerable<PigeonId> pigeonIds)
        {
            return await _context.Pigeons.Where(p => pigeonIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}
