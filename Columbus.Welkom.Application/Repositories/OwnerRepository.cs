using Columbus.Models.Owner;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class OwnerRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<OwnerEntity>(contextFactory), IOwnerRepository
    {
        public async Task<IEnumerable<OwnerEntity>> GetAllByOwnerIdsAsync(IEnumerable<OwnerId> ownerIds)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Owners.Where(o => ownerIds.Contains(o.OwnerId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetAllWithAllPigeonsAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Owners.Include(o => o.Pigeons)
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetAllWithPigeonsForYearAsync(int year, bool includeOwnersWithoutPigeons)
        {
            DataContext context = _contextFactory.CreateDbContext();

            var query = context.Owners.AsQueryable();

            if (!includeOwnersWithoutPigeons)
                query = query.Where(o => o.Pigeons!.Any());

            return await query.Include(o => o.Pigeons!.Where(p => p.Id.Year == year % 100))
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerEntity>> GetByOwnerIdsAsync(IEnumerable<OwnerId> ownerIds)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Owners.Where(o => ownerIds.Contains(o.OwnerId))
                .ToListAsync();
        }
    }
}
