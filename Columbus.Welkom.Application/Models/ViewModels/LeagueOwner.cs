using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class LeagueOwner
    {
        public required Owner Owner { get; set; }
        public required League League { get; set; }
        public required int Points { get; set; }
    }
}
