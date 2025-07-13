using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IPigeonSwapRepository : IBaseRepository<PigeonSwapEntity>
    {
        Task<int> DeleteByPlayerAndPigeonAsync(OwnerId playerId, PigeonId pigeonId);
        Task<IEnumerable<PigeonSwapEntity>> GetAllWithOwnersAndPigeonAsync();
        Task<PigeonSwapEntity?> GetByIdAsync(int id);
    }
}
