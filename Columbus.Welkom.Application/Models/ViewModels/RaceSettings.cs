using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class RaceSettings
{
    public IEnumerable<RaceTypeDescription> RaceTypeDescriptions { get; set; } = [];
    public AppliedRaceTypes AppliedRaceTypes { get; set; } = new();
    public IEnumerable<RacePointsSettings> RacePointsSettings { get; set; } = [];

    public Dictionary<RaceType, RaceTypeDescription> GetRaceTypeDescriptionsByRaceType() => RaceTypeDescriptions.ToDictionary(rtd => rtd.RaceType);
    public Dictionary<RaceType, RacePointsSettings> GetRacePointsSettingsByRaceType() => RacePointsSettings.ToDictionary(rts => rts.RaceType);
    public Dictionary<RaceType, INeutralizationTime> GetNeutralizationTimesByRaceType(int year) => RaceTypeDescriptions.ToDictionary(rtd => rtd.RaceType, rtd => GetNeutralizationTimeByType(rtd.NeutralizationType, year));
    public INeutralizationTime GetNeutralizationTimeForRaceType(RaceType raceType, int year) => GetNeutralizationTimeByType(RaceTypeDescriptions.FirstOrDefault(rtd => rtd.RaceType == raceType)?.NeutralizationType, year);


    public static INeutralizationTime GetNeutralizationTimeByType(NeutralizationType? neutralizationType, int year) => neutralizationType switch
    {
        NeutralizationType.Nf14 => new Nf14(SolarPeriods.GetSolarPeriods(year)),
        _ => new NoNeutralization(),
    };
}
