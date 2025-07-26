namespace Columbus.Welkom.Application.Models.Entities
{
    public class LeagueEntity : IEntity
    {
        public int Rank { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<LeagueOwnerEntity> LeagueOwners { get; set; } = [];
    }
}
