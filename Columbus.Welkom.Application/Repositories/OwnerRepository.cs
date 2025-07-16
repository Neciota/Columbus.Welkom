using Columbus.Models.Owner;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class OwnerRepository : BaseRepository<OwnerEntity>, IOwnerRepository
    {
        public OwnerRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<OwnerEntity>> GetAllByOwnerIdsAsync(IEnumerable<OwnerId> ownerIds)
        {
            return await _context.Owners.Where(o => ownerIds.Contains(o.OwnerId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetAllWithAllPigeonsAsync()
        {
            return await _context.Owners.Include(o => o.Pigeons)
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetAllWithPigeonsForYearAsync(int year, bool includeOwnersWithoutPigeons)
        {
            var query = _context.Owners.AsQueryable();

            if (!includeOwnersWithoutPigeons)
                query = query.Where(o => o.Pigeons!.Any());

            return await query.Include(o => o.Pigeons!.Where(p => p.Id.Year == year % 100))
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetByOwnerIdsAsync(IEnumerable<OwnerId> ownerIds)
        {
            return await _context.Owners.Where(o => ownerIds.Contains(o.OwnerId))
                .ToListAsync();
        }
    }
}
