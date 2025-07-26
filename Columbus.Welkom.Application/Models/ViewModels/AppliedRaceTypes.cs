using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class AppliedRaceTypes
{
    public IList<RaceType> SelectedYearPigeonRaceTypes { get; set; } = [];
    public IList<RaceType> SelectedYoungPigeonRaceTypes { get; set; } = [];
    public IList<RaceType> LeagueRaceTypes { get; set; } = [];
    public IList<RaceType> TeamRaceTypes { get; set; } = [];
    public IList<RaceType> PigeonSwapRaceTypes { get; set; } = [];
}
