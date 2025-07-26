using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonSwapRepository : BaseRepository<PigeonSwapEntity>, IPigeonSwapRepository
    {
        public PigeonSwapRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<PigeonSwapEntity>> GetAllWithOwnersAndPigeonAsync()
        {
            return await _context.PigeonSwaps.Include(ps => ps.Player)
                .Include(ps => ps.Owner)
                .Include(ps => ps.Pigeon)
                .Include(ps => ps.CoupledPlayer)
                .ToListAsync();
        }

        public async Task<int> DeleteByPlayerAndPigeonAsync(OwnerId playerId, PigeonId pigeonId)
        {
            return await _context.PigeonSwaps.Where(ps => ps.PlayerId == playerId)
                .Where(ps => ps.Pigeon!.Id == pigeonId)
                .ExecuteDeleteAsync();
        }

        public async Task<PigeonSwapEntity?> GetByIdAsync(int id)
        {
            return await _context.PigeonSwaps.SingleOrDefaultAsync(ps => ps.Id == id);
        }
    }
}
