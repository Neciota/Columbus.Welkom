using Columbus.Models.Owner;
using System.Text.Json.Serialization;

namespace Columbus.Welkom.Application.Models
{
    public class Leagues
    {
        private IEnumerable<LeagueOwner> _participants;

        [JsonConstructor]
        public Leagues(IEnumerable<LeagueOwner> participants)
        {
            _participants = participants;
        }

        public IEnumerable<LeagueOwner> AllParticipants
        {
            get => _participants;
            set  {
                _participants = value;
                Console.WriteLine(_participants.Count());
            }
        }

        [JsonIgnore]
        public IEnumerable<LeagueOwner> FirstLeagueOwners => _participants.Where(p => p.League == League.First).OrderByDescending(p => p.Points);

        [JsonIgnore]
        public IEnumerable<LeagueOwner> SecondLeagueOwners => _participants.Where(p => p.League == League.Second).OrderByDescending(p => p.Points);

        [JsonIgnore]
        public IEnumerable<LeagueOwner> ThirdLeagueOwners => _participants.Where(p => p.League == League.Third).OrderByDescending(p => p.Points);

        public void Promote(LeagueOwner participant)
        {
            participant.League--;
        }

        public void Demote(LeagueOwner participant)
        {
            participant.League++;
        }

        private LeagueOwner GetLeagueOwner(Owner owner) => _participants.First(p => p.Owner.Id == owner.Id);
    }
}