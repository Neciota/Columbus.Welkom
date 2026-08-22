using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class AppliedRaceTypes
{
    public IList<RaceType> SelectedYearPigeonRaceTypes { get; set; } = [];
    public IList<RaceType> SelectedYoungPigeonRaceTypes { get; set; } = [];
    public IList<RaceType> LeagueRaceTypes { get; set; } = [];
    public IList<RaceType> TeamRaceTypes { get; set; } = [];
    public IList<RaceType> PigeonSwapRaceTypes { get; set; } = [];

    /// <summary>
    /// The pigeon sale championship selects on the leading letter of the flight code rather than
    /// on the race type, because it runs over the natour flights only. Those are race type
    /// <c>J</c> just like the young-pigeon flights before the natour, and only the code tells
    /// them apart. See <see cref="RaceCodeLetter"/>.
    /// </summary>
    public IList<string> PigeonSaleCodeLetters { get; set; } = [];
}
