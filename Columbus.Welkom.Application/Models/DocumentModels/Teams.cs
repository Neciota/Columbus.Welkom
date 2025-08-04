using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;
public class Teams : BaseDocumentModel
{
    public ICollection<Team> AllTeams { get; set; } = [];
}
