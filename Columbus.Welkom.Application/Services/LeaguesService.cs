using Columbus.Models.Owner;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Export;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Columbus.Welkom.Application.Services
{
    public class LeaguesService : ILeaguesService
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly SettingsProvider _settingsProvider;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IFilePicker _filePicker;

        public LeaguesService(ILeagueRepository leagueRepository, IRaceRepository raceRepository, SettingsProvider settingsProvider, IOptions<AppSettings> appSettings, IFilePicker filePicker)
        {
            _leagueRepository = leagueRepository;
            _raceRepository = raceRepository;
            _settingsProvider = settingsProvider;
            _appSettings = appSettings;
            _filePicker = filePicker;
        }

        public async Task AddLeagueAsync(League league)
        {
            LeagueEntity leagueToAdd = new()
            {
                Name = league.Name,
                Rank = league.Rank,
                LeagueOwners = []
            };

            await _leagueRepository.AddAsync(leagueToAdd);
        }

        public async Task<Leagues> GetLeaguesAsync()
        {
            ICollection<LeagueEntity> leagues = await _leagueRepository.GetAllWithOwnersAsync();

            RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
            Dictionary<RaceType, RacePointsSettings> racePointsSettingsByRaceType = raceSettings.RacePointsSettings.ToDictionary(rps => rps.RaceType);
            Dictionary<RaceType, INeutralizationTime> neutralizationTimesByRaceType = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync(raceSettings.AppliedRaceTypes.LeagueRaceTypes.ToArray());
            IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(
                racePointsSettingsByRaceType[re.Type].PointsQuotient,
                racePointsSettingsByRaceType[re.Type].MaxPoints,
                racePointsSettingsByRaceType[re.Type].MinPoints,
                racePointsSettingsByRaceType[re.Type].DecimalCount,
                neutralizationTimesByRaceType[re.Type]));

            Dictionary<OwnerId, IEnumerable<RacePoints>> pointsByOwnerId = races.SelectMany(r =>
            {
                IEnumerable<PigeonRace> firstMarkedPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.First(pr => pr.Mark == 1));
                IEnumerable<PigeonRace?> secondMarkedPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.FirstOrDefault(pr => pr.Mark == 2));
                IEnumerable<PigeonRace?> topPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.MinBy(pr => pr.Position));

                return firstMarkedPigeons.Join(secondMarkedPigeons, fmp => fmp.OwnerId, smp => smp?.OwnerId, (fmp, smp) => (fmp, smp))
                    .Join(topPigeons, mp => mp.fmp.OwnerId, tp => tp?.OwnerId, (mp, tp) => (mp.fmp, mp.smp, tp))
                    .Select(p =>
                    {
                        double points = p.tp?.Points ?? 0;
                        if (p.tp == p.fmp)
                        {
                            points += p.smp?.Points ?? 0;
                        }
                        else if (p.tp == p.smp)
                        {
                            points += p.fmp.Points ?? 0;
                        }
                        else
                        {
                            points += Math.Max(p.fmp.Points ?? 0, p.smp?.Points ?? 0);
                        }

                        return (p.fmp.OwnerId, new RacePoints { RaceCode = r.Code, Points = points });
                    });
            })
                .GroupBy(op => op.OwnerId)
                .ToDictionary(ops => ops.Key, ops => ops.Select(p => p.Item2));

            return new Leagues(leagues.Select(l => new League
            {
                Rank = l.Rank,
                Name = l.Name,
                LeagueOwners = l.LeagueOwners.Select(lo => new LeagueOwner { 
                    Owner = lo.Owner!.ToOwner(),
                    RacePoints = pointsByOwnerId.GetValueOrDefault(lo.OwnerId)?.ToList() ?? [],
                }).ToHashSet()
            }).ToList());
        }

        public async Task UpdateLeagueAsync(League league)
        {
            LeagueEntity? existingLeague = await _leagueRepository.GetByRankAsync(league.Rank);
            if (existingLeague is null)
                throw new ArgumentException("League does not exist.");
            if (league.LeagueOwners.Any(lo => lo.Owner is null))
                throw new ArgumentException("Owner is not set for an entry.");

            existingLeague.Name = league.Name;

            IEnumerable<LeagueOwnerEntity> leagueOwnersToAdd = league.LeagueOwners.ExceptBy(existingLeague.LeagueOwners.Select(lo => lo.OwnerId), lo => lo.Owner!.Id)
                .Select(lo => new LeagueOwnerEntity { LeagueRank = league.Rank, OwnerId = lo.Owner!.Id });
            IEnumerable<LeagueOwnerEntity> leagueOwnersToKeep = existingLeague.LeagueOwners.IntersectBy(league.LeagueOwners.Select(lo => lo.Owner!.Id), lo => lo.OwnerId);

            existingLeague.LeagueOwners = leagueOwnersToKeep.Concat(leagueOwnersToAdd).ToList();
            await _leagueRepository.UpdateAsync(existingLeague);
        }

        public async Task DeleteLeagueAsync(League league)
        {
            LeagueEntity? existingLeague = await _leagueRepository.GetByRankAsync(league.Rank);
            if (existingLeague is null)
                throw new ArgumentException("League does not exist.");

            await _leagueRepository.DeleteAsync(existingLeague);
        }

        public async Task ExportAsync(Leagues leagues)
        {
            RaceEntity mostRecentRace = await _raceRepository.GetMostRecentRaceAsync();

            Models.DocumentModels.Leagues documentLeagues = new()
            {
                ClubId = _appSettings.Value.Club,
                Year = _appSettings.Value.Year,
                AllLeagues = leagues.AllLeagues,
                LastRaceName = mostRecentRace.Name,
                LastRaceDate = mostRecentRace.StartTime
            };

            LeaguesDocument document = new(documentLeagues);
            byte[] pdf = document.GeneratePdf();

            await _filePicker.SaveFileAsync("Divisiespel.pdf", new MemoryStream(pdf));
        }
    }
}
