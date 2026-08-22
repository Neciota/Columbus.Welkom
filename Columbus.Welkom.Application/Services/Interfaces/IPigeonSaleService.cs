using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces;

public interface IPigeonSaleService
{
    Task<ICollection<PigeonSaleClass>> GetAllClassesAsync();
    Task DeleteAsync(PigeonSale pigeonSale);
    Task UpdateAsync(PigeonSaleClass pigeonSaleClass, PigeonSale pigeonSale);
    Task ExportAsync(IEnumerable<PigeonSaleClass> pigeonSaleClasses);

    /// <summary>
    /// Exports a document showing only the points of the given race next to the overall total.
    /// </summary>
    Task ExportRaceAsync(IEnumerable<PigeonSaleClass> pigeonSaleClasses, SimpleRace race);

    /// <summary>
    /// Gets the races that count towards the pigeon sale, most recent first.
    /// </summary>
    Task<ICollection<SimpleRace>> GetRacesAsync();

    Task AddClassAsync(PigeonSaleClass pigeonSaleClass);
    Task UpdateClassAsync(PigeonSaleClass pigeonSaleClass);
    Task DeleteClassAsync(PigeonSaleClass pigeonSaleClass);
}
