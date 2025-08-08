namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class Leagues
    {
        private readonly ICollection<League> _leagues;

        public Leagues(ICollection<League> leagues)
        {
            _leagues = leagues;
        }

        public IEnumerable<LeagueOwner> AllParticipants => _leagues.SelectMany(l => l.LeagueOwners);

        public IEnumerable<LeagueOwner> GetLeagueOwnersByLeagueRank(int leagueRank) => (_leagues.FirstOrDefault(p => p.Rank == leagueRank)?.LeagueOwners ?? []).OrderByDescending(p => p.TotalPoints);

        public IEnumerable<League> AllLeagues => _leagues.OrderBy(l => l.Rank);

        public void RemoveFromCurrentLeague(LeagueOwner participant)
        {
            League league = _leagues.First(l => l.LeagueOwners.Contains(participant));

            if (league.LeagueOwners.IsReadOnly)
            {
                league.LeagueOwners = league.LeagueOwners.Except([participant]).ToArray();
            }
            else
            {
                league.LeagueOwners.Remove(participant);
            }
        }

        public void AddToLeague(LeagueOwner participant, int rank)
        {
            League? league = _leagues.FirstOrDefault(l => l.Rank == rank);
            if (league is null)
                return;

            if (league.LeagueOwners.IsReadOnly)
            {
                league.LeagueOwners = league.LeagueOwners.Append(participant).ToArray();
            }
            else
            {
                league.LeagueOwners.Add(participant);
            }
        }

        public void AddLeague(League league)
        {
            _leagues.Add(league);
        }

        public void RemoveLeague(League league)
        {
            _leagues.Remove(league);
        }
    }
}