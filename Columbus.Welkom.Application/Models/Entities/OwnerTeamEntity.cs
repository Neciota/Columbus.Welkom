using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.Entities;

public class OwnerTeamEntity : IEntity
{
    public OwnerId OwnerId { get; set; }
    public int TeamNumber { get; set; }
    public int Position { get; set; }

    public OwnerEntity? Owner { get; set; }
    public TeamEntity? Team { get; set; }
}
