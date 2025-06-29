using Columbus.Models.Owner;
using System.Text.Json.Serialization;

namespace Columbus.Welkom.Application.Models
{
    public class LeagueOwner
    {
        [JsonConstructor]
        public LeagueOwner(Owner owner, League league, int points)
        {
            Owner = owner;
            League = league;
            Points = points;
        }

        public LeagueOwner(Owner owner)
        {
            Owner = owner;
            League = League.Third;
            Points = 0;
        }

        public Owner Owner { get; set; }
        public League League { get; set; }
        public int Points { get; set; }
    }
}
