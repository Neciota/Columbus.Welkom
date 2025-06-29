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

        public async Task<int> DeleteByOwnerAsync(int ownerId)
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

        public async Task<SelectedYearPigeonEntity?> GetByOwnerAsync(int ownerId)
        {
            return await _context.SelectedYearPigeons.FirstOrDefaultAsync(syp => syp.OwnerId == ownerId);
        }

        public async Task<SelectedYearPigeonEntity?> GetByPigeonAsync(int pigeonYear, CountryCode pigeonCountry, RingNumber pigeonRingNumber)
        {
            return await _context.SelectedYearPigeons.Where(syp => syp.Pigeon!.Year == pigeonYear)
                .Where(syp => syp.Pigeon!.Country == pigeonCountry)
                .Where(syp => syp.Pigeon!.RingNumber == pigeonRingNumber)
                .Include(syp => syp.Pigeon)
                .Include(syp => syp.Owner)
                .FirstOrDefaultAsync();
        }
    }
}
