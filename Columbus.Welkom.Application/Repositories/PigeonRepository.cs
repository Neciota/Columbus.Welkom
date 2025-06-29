using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class PigeonRepository : BaseRepository<PigeonEntity>, IPigeonRepository
    {
        public PigeonRepository(DataContext context) : base(context) { }

        public async Task<PigeonEntity> GetByCountryAndYearAndRingNumberAsync(CountryCode country, int year, RingNumber ringNumber)
        {
            return await _context.Pigeons.Where(p => p.Country == country)
                .Where(p => p.Year == year)
                .Where(p => p.RingNumber == ringNumber)
                .FirstAsync();
        }

        public async Task<IEnumerable<PigeonEntity>> GetPigeonsByCountriesAndYearsAndRingNumbersAsync(IEnumerable<Pigeon> pigeons)
        {
            IEnumerable<CountryCode> countries = pigeons.Select(p => p.Country).Distinct();
            IEnumerable<int> years = pigeons.Select(p => p.Year).Distinct();
            IEnumerable<RingNumber> ringNumbers = pigeons.Select(p => p.RingNumber).Distinct();

            IEnumerable<PigeonEntity> result = await _context.Pigeons
                .Where(p => countries.Contains(p.Country))
                .Where(p => years.Contains(p.Year))
                .Where(p => ringNumbers.Contains(p.RingNumber))
                .ToListAsync();

            return result.Where(pe => pigeons.Any(p => p.Country == pe.Country && p.Year == pe.Year && p.RingNumber == pe.RingNumber));
        }
    }
}
