using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.Entities;

public class LeagueOwnerEntity : IEntity
{
    public int LeagueRank { get; set; }
    public OwnerId OwnerId { get; set; }

    public LeagueEntity? League { get; set; }
    public OwnerEntity? Owner { get; set; }
}
