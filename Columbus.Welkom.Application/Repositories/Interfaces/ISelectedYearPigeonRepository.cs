using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface ISelectedYearPigeonRepository : IBaseRepository<SelectedYearPigeonEntity>
    {
        Task<int> DeleteByOwnerAsync(OwnerId ownerId);
        Task<SelectedYearPigeonEntity?> GetByIdAsync(int selectedYearPigeonId);
        Task<SelectedYearPigeonEntity?> GetByOwnerAsync(OwnerId ownerId);
        Task<SelectedYearPigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId);
    }
}
