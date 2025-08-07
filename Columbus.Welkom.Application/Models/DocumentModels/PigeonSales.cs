using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;

public class PigeonSales : BaseDocumentModel
{
    public ICollection<PigeonSale> AllPigeonSales { get; set; } = [];
    public ICollection<SimpleRace> Races { get; set; } = [];
}
