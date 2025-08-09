using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces;

public interface IPigeonSaleRepository : IBaseRepository<PigeonSaleEntity>
{
    Task<ICollection<PigeonSaleEntity>> GetAllWithOwnersAndPigeonsAsync();
    Task<PigeonSaleEntity?> GetByIdAsync(int id);
}
