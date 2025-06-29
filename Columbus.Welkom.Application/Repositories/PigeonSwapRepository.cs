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

        public async Task<IEnumerable<PigeonSwapEntity>> GetAllByYearAsync(int year)
        {
            return await _context.PigeonSwaps.Where(ps => ps.Year == year)
                .Include(ps => ps.Player)
                .Include(ps => ps.Owner)
                .Include(ps => ps.Pigeon)
                .Include(ps => ps.CoupledPlayer)
                .ToListAsync();
        }

        public async Task<int> DeleteByYearAndPlayerAndPigeonAsync(int year, int playerId, CountryCode pigeonCountry, int pigeonYear, RingNumber pigeonRingNumber)
        {
            return await _context.PigeonSwaps.Where(ps => ps.Year == year)
                .Where(ps => ps.PlayerId == playerId)
                .Where(ps => ps.Pigeon!.Country == pigeonCountry && ps.Pigeon.Year == pigeonYear && ps.Pigeon.RingNumber == pigeonRingNumber)
                .ExecuteDeleteAsync();
        }
    }
}
