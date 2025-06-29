using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface ISelectedYoungPigeonRepository : IBaseRepository<SelectedYoungPigeonEntity>
    {
        Task<int> DeleteByOwnerAsync(int ownerId);
        Task<IEnumerable<SelectedYoungPigeonEntity>> GetAllAsync();
        Task<SelectedYoungPigeonEntity?> GetByOwnerAsync(int ownerId);
        Task<SelectedYoungPigeonEntity?> GetByPigeonAsync(int pigeonYear, CountryCode pigeonCountry, RingNumber pigeonRingNumber);
    }
}
