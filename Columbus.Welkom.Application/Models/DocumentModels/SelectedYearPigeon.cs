using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;

public class SelectedYearPigeon : BaseDocumentModel
{
    public IEnumerable<OwnerPigeonPair> OwnerPigeonPairs { get; set; } = [];
}
