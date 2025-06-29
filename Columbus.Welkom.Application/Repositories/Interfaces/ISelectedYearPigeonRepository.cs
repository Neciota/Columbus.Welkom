using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface ISelectedYearPigeonRepository : IBaseRepository<SelectedYearPigeonEntity>
    {
        Task<int> DeleteByOwnerAsync(int ownerId);
        Task<IEnumerable<SelectedYearPigeonEntity>> GetAllAsync();
        Task<SelectedYearPigeonEntity?> GetByOwnerAsync(int ownerId);
        Task<SelectedYearPigeonEntity?> GetByPigeonAsync(int pigeonYear, CountryCode pigeonCountry, RingNumber pigeonRingNumber);
    }
}
