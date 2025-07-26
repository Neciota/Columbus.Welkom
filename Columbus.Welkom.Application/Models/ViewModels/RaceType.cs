using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class RaceTypeDescription
{
    public RaceType RaceType { get; set; }
    public string Description { get; set; } = string.Empty;
    public NeutralizationType NeutralizationType { get; set; } = NeutralizationType.None;
}
