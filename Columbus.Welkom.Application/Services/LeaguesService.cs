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
        private readonly ILeagueOwnerRepository _leagueOwnerRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly SettingsProvider _settingsProvider;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IFilePicker _filePicker;

        public LeaguesService(ILeagueRepository leagueRepository,
            ILeagueOwnerRepository leagueOwnerRepository,
            IRaceRepository raceRepository, 
            SettingsProvider settingsProvider, 
            IOptions<AppSettings> appSettings, 
            IFilePicker filePicker)
        {
            _leagueRepository = leagueRepository;
            _leagueOwnerRepository = leagueOwnerRepository;
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
                return r.PigeonRaces.Where(pr => pr.Mark is 1 or 2 || pr.ArrivalOrder is 1 or 2)
                    .GroupBy(pr => pr.OwnerId)
                    .Select(prg =>
                    {
                        PigeonRace? topMarkedPigeonRace = prg.Where(pr => pr.Mark is 1 or 2).MaxBy(pr => pr.Points);
                        PigeonRace? topOtherPigeonRace = (topMarkedPigeonRace is null ? prg : prg.Except([topMarkedPigeonRace])).MaxBy(pr => pr.Points);

                        double points = (topMarkedPigeonRace?.Points ?? 0) + (topOtherPigeonRace?.Points ?? 0);
                        return (prg.Key, new RacePoints { RaceCode = r.Code, Points = points } );
                    });
            })
                .GroupBy(op => op.Key)
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
            IEnumerable<LeagueOwnerEntity> leagueOwnersToDelete = existingLeague.LeagueOwners.ExceptBy(league.LeagueOwners.Select(lo => lo.Owner!.Id), lo => lo.OwnerId);

            await _leagueOwnerRepository.AddRangeAsync(leagueOwnersToAdd);
            await _leagueOwnerRepository.DeleteRangeAsync(leagueOwnersToDelete);
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
