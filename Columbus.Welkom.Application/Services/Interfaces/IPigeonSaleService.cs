using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces;

public interface IPigeonSaleService
{
    Task<IEnumerable<PigeonSale>> GetAllAsync();
    Task DeleteAsync(PigeonSale pigeonSale);
    Task UpdateAsync(PigeonSale pigeonSale);
}
