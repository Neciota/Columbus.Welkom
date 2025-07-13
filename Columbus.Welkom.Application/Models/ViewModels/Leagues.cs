using Columbus.Models.Owner;
using System.Text.Json.Serialization;

namespace Columbus.Welkom.Application.Models.ViewModels
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

        public IEnumerable<LeagueOwner> GetLeagueOwnersByLeagueRank(int leagueRank) => _participants.Where(p => p.League.Rank == leagueRank).OrderByDescending(p => p.Points);

        public IEnumerable<League> GetLeagues() => _participants.Select(p => p.League).DistinctBy(l => l.Rank).OrderByDescending(l => l.Rank);

        public void Promote(LeagueOwner participant)
        {
            participant.League.Rank--;
        }

        public void Demote(LeagueOwner participant)
        {
            participant.League.Rank++;
        }

        private LeagueOwner GetLeagueOwner(Owner owner) => _participants.First(p => p.Owner.Id == owner.Id);
    }
}