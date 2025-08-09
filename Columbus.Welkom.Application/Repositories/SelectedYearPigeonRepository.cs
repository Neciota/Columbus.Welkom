using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class SelectedYearPigeonRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<SelectedYearPigeonEntity>(contextFactory), ISelectedYearPigeonRepository
    {
        public async Task<int> DeleteByOwnerAsync(OwnerId ownerId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYearPigeons.Where(syp => syp.OwnerId == ownerId)
                .ExecuteDeleteAsync();
        }

        public override async Task<IEnumerable<SelectedYearPigeonEntity>> GetAllAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYearPigeons.Include(syp => syp.Owner)
                .Include(syp => syp.Pigeon)
                .ToListAsync();
        }

        public async Task<SelectedYearPigeonEntity?> GetByIdAsync(int selectedYearPigeonId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYearPigeons.FirstOrDefaultAsync(syp => syp.Id == selectedYearPigeonId);
        }

        public async Task<SelectedYearPigeonEntity?> GetByOwnerAsync(OwnerId ownerId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYearPigeons.FirstOrDefaultAsync(syp => syp.OwnerId == ownerId);
        }

        public async Task<SelectedYearPigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.SelectedYearPigeons.Where(syp => syp.PigeonId == pigeonId)
                .Include(syp => syp.Pigeon)
                .Include(syp => syp.Owner)
                .FirstOrDefaultAsync();
        }
    }
}
