using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;

namespace Columbus.Welkom.Application.Repositories;

public interface IPigeonSaleRepository : IBaseRepository<PigeonSaleEntity>
{
    Task<ICollection<PigeonSaleEntity>> GetAllWithOwnersAndPigeonsAsync();
    Task<PigeonSaleEntity?> GetByIdAsync(int id);
}
