using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class LeagueOwner : IPoints
    {
        public required Owner? Owner { get; set; }
        public required ICollection<RacePoints> RacePoints { get; set; } = [];
        public double TotalPoints => RacePoints.Select(p => p.Points).Sum();
    }
}
