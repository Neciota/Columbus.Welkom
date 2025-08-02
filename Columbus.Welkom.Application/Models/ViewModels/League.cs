namespace Columbus.Welkom.Application.Models.ViewModels;

public class League
{
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<LeagueOwner> LeagueOwners { get; set; } = [];
}
