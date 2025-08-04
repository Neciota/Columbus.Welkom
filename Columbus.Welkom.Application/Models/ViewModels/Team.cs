namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class Team
    {
        public int Number { get; set; }
        public ICollection<TeamOwner> TeamOwners { get; set; } = [];

        public double TotalPoints => TeamOwners.Sum(to => to.Points);
    }
}
