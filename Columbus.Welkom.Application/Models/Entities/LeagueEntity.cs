namespace Columbus.Welkom.Application.Models.Entities
{
    public class LeagueEntity : IEntity
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public League League { get; set; }

        public OwnerEntity? Owner { get; set; }
    }
}
