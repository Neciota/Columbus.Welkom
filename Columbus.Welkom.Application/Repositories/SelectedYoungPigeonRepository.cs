using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class SelectedYoungPigeonRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<SelectedYoungPigeonEntity>(contextFactory), ISelectedYoungPigeonRepository
    {
        public async Task<int> DeleteByOwnerAsync(OwnerId ownerId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYoungPigeons.Where(syp => syp.OwnerId == ownerId)
                .ExecuteDeleteAsync();
        }

        public override async Task<IEnumerable<SelectedYoungPigeonEntity>> GetAllAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYoungPigeons.Include(syp => syp.Owner)
                .Include(syp => syp.Pigeon)
                .ToListAsync();
        }

        public async Task<SelectedYoungPigeonEntity?> GetByOwnerAsync(OwnerId ownerId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYoungPigeons.Where(syp => syp.OwnerId == ownerId)
                .FirstOrDefaultAsync();
        }

        public async Task<SelectedYoungPigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYoungPigeons.Where(syp => syp.Pigeon!.Id == pigeonId)
                .Include(syp => syp.Pigeon)
                .Include(syp => syp.Owner)
                .FirstOrDefaultAsync();
        }
    }
}
