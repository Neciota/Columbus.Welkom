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

            Dictionary<OwnerId, double> pointsByOwnerId = races.SelectMany(r =>
            {
                IEnumerable<PigeonRace> firstMarkedPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.First(pr => pr.Mark == 1));
                IEnumerable<PigeonRace?> secondMarkedPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.FirstOrDefault(pr => pr.Mark == 2));
                IEnumerable<PigeonRace?> topPigeons = r.PigeonRaces.GroupBy(pr => pr.OwnerId).Select(prg => prg.MinBy(pr => pr.Position));

                return firstMarkedPigeons.Join(secondMarkedPigeons, fmp => fmp.OwnerId, smp => smp?.OwnerId, (fmp, smp) => (fmp, smp))
                    .Join(topPigeons, mp => mp.fmp.OwnerId, tp => tp?.OwnerId, (mp, tp) => (mp.fmp, mp.smp, tp))
                    .Select(p => (p.fmp.OwnerId, p.tp?.Pigeon == p.fmp.Pigeon ? p.tp.Points + p.smp?.Points : p.tp?.Points + p.fmp.Points));
            })
                .GroupBy(op => op.OwnerId)
                .Select(ops => (ops.Key, ops.Select(p => p.Item2 ?? 0d).Sum()))
                .ToDictionary();

            return new Leagues(leagues.Select(l => new League
            {
                Rank = l.Rank,
                Name = l.Name,
                LeagueOwners = l.LeagueOwners.Select(lo => new LeagueOwner { 
                    Owner = lo.Owner!.ToOwner(),
                    Points = pointsByOwnerId.GetValueOrDefault(lo.OwnerId, 0),
                }).ToHashSet()
            }).ToList());
        }

        public async Task UpdateLeagueAsync(League league)
        {
            LeagueEntity? existingLeague = await _leagueRepository.GetByRankAsync(league.Rank);
            if (existingLeague is null)
                throw new ArgumentException("League does not exist.");
            if (existingLeague.LeagueOwners.Any(lo => lo.Owner is null))
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
            RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync(raceSettings.AppliedRaceTypes.SelectedYearPigeonRaceTypes.ToArray());

            Models.DocumentModels.Leagues documentLeagues = new()
            {
                ClubId = _appSettings.Value.Club,
                Year = _appSettings.Value.Year,
                AllLeagues = leagues.AllLeagues,
                RaceCodes = raceEntities.Select(re => re.Code)
            };

            LeaguesDocument document = new(documentLeagues);
            byte[] pdf = document.GeneratePdf();

            await _filePicker.SaveFileAsync("Divisiespel.pdf", new MemoryStream(pdf));
        }
    }
}
