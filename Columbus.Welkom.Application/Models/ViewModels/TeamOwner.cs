using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class TeamOwner
{
    public required Owner? Owner { get; set; }
    public required int Position { get; set; }
    public required double Points { get; set; }
}
