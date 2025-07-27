using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.DocumentModels;

public class SelectedYearPigeon
{
    public int ClubId { get; set; }
    public int Year { get; set; }
    public IEnumerable<OwnerPigeonPair> OwnerPigeonPairs { get; set; } = [];
    public IEnumerable<string> RaceCodes { get; set; } = [];
}
