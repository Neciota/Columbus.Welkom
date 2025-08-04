namespace Columbus.Welkom.Application.Models.Entities
{
    public class TeamEntity : IEntity
    {
        public int Number { get; set; }
        public ICollection<OwnerTeamEntity> TeamOwners { get; set; } = [];
    }
}
