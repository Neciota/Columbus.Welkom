using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IPigeonRepository : IBaseRepository<PigeonEntity>
    {
        Task<PigeonEntity?> GetByPigeonIdAsync(PigeonId pigeonId);
        Task<IEnumerable<PigeonEntity>> GetByPigeonIdsAsync(IEnumerable<PigeonId> pigeonIds);
    }
}
