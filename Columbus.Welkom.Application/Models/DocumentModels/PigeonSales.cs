using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;

public class PigeonSales : BaseDocumentModel
{
    public ICollection<PigeonSaleClass> PigeonSaleClasses { get; set; } = [];
    public ICollection<SimpleRace> Races { get; set; } = [];

    /// <summary>
    /// Set when the document covers a single race, in which case <see cref="Races"/> only holds that race.
    /// </summary>
    public SimpleRace? SelectedRace { get; set; }
}
