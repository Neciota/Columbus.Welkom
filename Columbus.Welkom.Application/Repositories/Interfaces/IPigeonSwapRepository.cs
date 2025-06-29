using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IPigeonSwapRepository : IBaseRepository<PigeonSwapEntity>
    {
        Task<int> DeleteByYearAndPlayerAndPigeonAsync(int year, int playerId, CountryCode pigeonCountry, int pigeonYear, RingNumber pigeonRingNumber);
        Task<IEnumerable<PigeonSwapEntity>> GetAllByYearAsync(int year);
    }
}
