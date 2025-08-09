using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces;

public interface IPigeonSaleClassRepository : IBaseRepository<PigeonSaleClassEntity>
{
    Task<ICollection<PigeonSaleClassEntity>> GetAllWithPigeonSalesAsync();
    Task<PigeonSaleClassEntity?> GetByIdAsync(int id);
}
