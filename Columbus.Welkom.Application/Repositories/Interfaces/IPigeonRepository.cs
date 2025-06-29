using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IPigeonRepository : IBaseRepository<PigeonEntity>
    {
        Task<PigeonEntity> GetByCountryAndYearAndRingNumberAsync(CountryCode country, int year, RingNumber ringNumber);
        Task<IEnumerable<PigeonEntity>> GetPigeonsByCountriesAndYearsAndRingNumbersAsync(IEnumerable<Pigeon> pigeons);
    }
}
