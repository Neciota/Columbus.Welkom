using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonSwapRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<PigeonSwapEntity>(contextFactory), IPigeonSwapRepository
    {
        public async Task<IEnumerable<PigeonSwapEntity>> GetAllWithOwnersAndPigeonAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.PigeonSwaps.Include(ps => ps.Player)
                .Include(ps => ps.Owner)
                .Include(ps => ps.Pigeon)
                .Include(ps => ps.CoupledPlayer)
                .ToListAsync();
        }

        public async Task<int> DeleteByPlayerAndPigeonAsync(OwnerId playerId, PigeonId pigeonId)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.PigeonSwaps.Where(ps => ps.PlayerId == playerId)
                .Where(ps => ps.Pigeon!.Id == pigeonId)
                .ExecuteDeleteAsync();
        }

        public async Task<PigeonSwapEntity?> GetByIdAsync(int id)
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.PigeonSwaps.SingleOrDefaultAsync(ps => ps.Id == id);
        }
    }
}
