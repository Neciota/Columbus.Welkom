using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface ISelectedYoungPigeonRepository : IBaseRepository<SelectedYoungPigeonEntity>
    {
        Task<int> DeleteByOwnerAsync(OwnerId ownerId);
        Task<SelectedYoungPigeonEntity?> GetByOwnerAsync(OwnerId ownerId);
        Task<SelectedYoungPigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId);
    }
}
