namespace Columbus.Welkom.Application.Models.ViewModels;

public class Settings
{
    public IEnumerable<RaceTypeDescription> RaceTypeDescriptions { get; set; } = [];
    public AppliedRaceTypes AppliedRaceTypes { get; set; } = new();
    public IEnumerable<RacePointsSettings> RacePointsSettings { get; set; } = [];
}
