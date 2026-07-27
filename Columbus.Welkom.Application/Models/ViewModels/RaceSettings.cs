using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class RaceSettings
{
    /// <summary>
    /// Folder holding Venira Rekenaar's Paradox tables, which races, members and lofts are read
    /// from. This is Venira's default install location.
    /// </summary>
    public string VeniraDataPath { get; set; } = @"C:\ProgramData\Venira\Rekenaar4\Data";

    public IEnumerable<RaceTypeDescription> RaceTypeDescriptions { get; set; } = [];
    public AppliedRaceTypes AppliedRaceTypes { get; set; } = new();
    public IEnumerable<RacePointsSettings> RacePointsSettings { get; set; } = [];

    public Dictionary<RaceType, RaceTypeDescription> GetRaceTypeDescriptionsByRaceType() => RaceTypeDescriptions.ToDictionary(rtd => rtd.RaceType);
    public Dictionary<RaceType, RacePointsSettings> GetRacePointsSettingsByRaceType() => RacePointsSettings.ToDictionary(rts => rts.RaceType);
    public Dictionary<RaceType, INeutralizationTime> GetNeutralizationTimesByRaceType(Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)> solarPeriods) => RaceTypeDescriptions.ToDictionary(rtd => rtd.RaceType, rtd => GetNeutralizationTimeByType(rtd.NeutralizationType, solarPeriods));
    public INeutralizationTime GetNeutralizationTimeForRaceType(RaceType raceType, Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)> solarPeriods) => GetNeutralizationTimeByType(RaceTypeDescriptions.FirstOrDefault(rtd => rtd.RaceType == raceType)?.NeutralizationType, solarPeriods);


    public static INeutralizationTime GetNeutralizationTimeByType(NeutralizationType? neutralizationType, Dictionary<DateOnly, (DateTime SunUp, DateTime SunDown)> solarPeriods) => neutralizationType switch
    {
        NeutralizationType.Nf14 => new Nf14(solarPeriods),
        _ => new NoNeutralization(),
    };
}
