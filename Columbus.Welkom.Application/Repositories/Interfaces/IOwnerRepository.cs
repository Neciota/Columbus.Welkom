using Columbus.Models.Owner;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IOwnerRepository : IBaseRepository<OwnerEntity>
    {
        Task<IEnumerable<OwnerEntity>> GetAllWithAllPigeonsAsync();
        Task<IEnumerable<OwnerEntity>> GetAllWithPigeonsForYearAsync(int year, bool includeOwnersWithoutPigeons);
        Task<IEnumerable<OwnerEntity>> GetByOwnerIdsAsync(IEnumerable<OwnerId> ownerIds);
    }
}
