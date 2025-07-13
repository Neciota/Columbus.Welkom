using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class SelectedYearPigeonRepository : BaseRepository<SelectedYearPigeonEntity>, ISelectedYearPigeonRepository
    {
        public SelectedYearPigeonRepository(DataContext context): base(context) { }

        public async Task<int> DeleteByOwnerAsync(OwnerId ownerId)
        {
            return await _context.SelectedYearPigeons.Where(syp => syp.OwnerId == ownerId)
                .ExecuteDeleteAsync();
        }

        public override async Task<IEnumerable<SelectedYearPigeonEntity>> GetAllAsync()
        {
            return await _context.SelectedYearPigeons.Include(syp => syp.Owner)
                .Include(syp => syp.Pigeon)
                .ToListAsync();
        }

        public async Task<SelectedYearPigeonEntity?> GetByOwnerAsync(OwnerId ownerId)
        {
            return await _context.SelectedYearPigeons.FirstOrDefaultAsync(syp => syp.OwnerId == ownerId);
        }

        public async Task<SelectedYearPigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId)
        {
            return await _context.SelectedYearPigeons.Where(syp => syp.PigeonId == pigeonId)
                .Include(syp => syp.Pigeon)
                .Include(syp => syp.Owner)
                .FirstOrDefaultAsync();
        }
    }
}
