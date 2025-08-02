using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;

public class Leagues : BaseDocumentModel
{
    public IEnumerable<League> AllLeagues { get; set; } = [];
}
