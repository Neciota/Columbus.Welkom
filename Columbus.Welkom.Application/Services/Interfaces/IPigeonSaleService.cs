using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces;

public interface IPigeonSaleService
{
    Task<ICollection<PigeonSaleClass>> GetAllClassesAsync();
    Task DeleteAsync(PigeonSale pigeonSale);
    Task UpdateAsync(PigeonSaleClass pigeonSaleClass, PigeonSale pigeonSale);
    Task ExportAsync(IEnumerable<PigeonSaleClass> pigeonSaleClasses);
    Task AddClassAsync(PigeonSaleClass pigeonSaleClass);
    Task UpdateClassAsync(PigeonSaleClass pigeonSaleClass);
    Task DeleteClassAsync(PigeonSaleClass pigeonSaleClass);
}
