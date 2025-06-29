using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class SelectedYoungPigeonRepository : BaseRepository<SelectedYoungPigeonEntity>, ISelectedYoungPigeonRepository
    {
        public SelectedYoungPigeonRepository(DataContext context): base(context) { }

        public async Task<int> DeleteByOwnerAsync(int ownerId)
        {
            return await _context.SelectedYoungPigeons.Where(syp => syp.OwnerId == ownerId)
                .ExecuteDeleteAsync();
        }

        public override async Task<IEnumerable<SelectedYoungPigeonEntity>> GetAllAsync()
        {
            return await _context.SelectedYoungPigeons.Include(syp => syp.Owner)
                .Include(syp => syp.Pigeon)
                .ToListAsync();
        }

        public async Task<SelectedYoungPigeonEntity?> GetByOwnerAsync(int ownerId)
        {
            return await _context.SelectedYoungPigeons.Where(syp => syp.OwnerId == ownerId)
                .FirstOrDefaultAsync();
        }

        public async Task<SelectedYoungPigeonEntity?> GetByPigeonAsync(int pigeonYear, CountryCode pigeonCountry, RingNumber pigeonRingNumber)
        {
            return await _context.SelectedYoungPigeons.Where(syp => syp.Pigeon!.Year == pigeonYear)
                .Where(syp => syp.Pigeon!.Country == pigeonCountry)
                .Where(syp => syp.Pigeon!.RingNumber == pigeonRingNumber)
                .Include(syp => syp.Pigeon)
                .Include(syp => syp.Owner)
                .FirstOrDefaultAsync();
        }
    }
}
