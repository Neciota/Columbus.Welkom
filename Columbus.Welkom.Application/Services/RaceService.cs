using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.UDP.Interfaces;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Columbus.Welkom.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IFilePicker _filePicker;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IPigeonRaceRepository _pigeonRaceRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly IRaceSerializer _raceSerializer;
        private readonly SettingsProvider _settingsProvider;
        private readonly IOptions<AppSettings> _appSettings;

        public RaceService(
            IFilePicker filePicker, 
            IOwnerRepository ownerRepository, 
            IPigeonRepository pigeonRepository, 
            IPigeonRaceRepository pigeonRaceRepository, 
            IRaceRepository raceRepository, 
            IRaceSerializer raceSerializer,
            SettingsProvider settingsProvider,
            IOptions<AppSettings> appSettings)
        {
            _filePicker = filePicker;
            _ownerRepository = ownerRepository;
            _pigeonRepository = pigeonRepository;
            _pigeonRaceRepository = pigeonRaceRepository;
            _raceRepository = raceRepository;
            _raceSerializer = raceSerializer;
            _settingsProvider = settingsProvider;
            _appSettings = appSettings;
        }

        public async Task<Race?> ReadRaceAsync()
        {
            (StreamReader? streamReader, string fileName) = await _filePicker.OpenFileAsync([".udp"]);
            if (streamReader is null)
                return null;

            RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
            Dictionary<RaceType, INeutralizationTime> neutralizationTimes = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

            return await _raceSerializer.DeserializeAsync(streamReader, neutralizationTimes[RaceType.Create(fileName[1])]);
        }

        public async Task<IEnumerable<Race>> ReadRacesAsync()
        {
            IEnumerable<(StreamReader StreamReader, string FileName)> files = await _filePicker.OpenFilesAsync([".udp"], new Regex(@$"W...{_appSettings.Value.Club}.udp"));

            RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
            Dictionary<RaceType, INeutralizationTime> neutralizationTimes = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

            return await Task.WhenAll(files.AsParallel().Select(s => _raceSerializer.DeserializeAsync(s.StreamReader, neutralizationTimes[RaceType.Create(s.FileName[1])])));
        }

        public async Task<IEnumerable<SimpleRace>> GetAllRacesAsync()
        {
            IEnumerable<SimpleRaceEntity> races = await _raceRepository.GetAllSimpleAsync();

            return races.Select(r => r.ToSimpleRace())
                .OrderBy(r => r.Number);
        }

        public async Task<IEnumerable<SimpleRace>> GetAllRacesByTypeAsync(RaceType type)
        {
            IEnumerable<SimpleRaceEntity> races = await _raceRepository.GetAllSimpleByTypesAsync([type]);

            return races.Select(r => r.ToSimpleRace())
                .OrderBy(r => r.Number);
        }

        public async Task OverwriteRacesAsync(IEnumerable<Race> races)
        {
            await _raceRepository.DeleteRangeAsync();

            IEnumerable<PigeonEntity> existingPigeons = await _pigeonRepository.GetByPigeonIdsAsync(races.SelectMany(r => r.PigeonRaces.Select(pr => pr.Pigeon.Id)));
            HashSet<PigeonId> existingPigeonIds = existingPigeons.Select(p => p.Id).ToHashSet();

            PigeonEntity[] pigeonsToAdd = races.SelectMany(r => r.PigeonRaces)
                .ExceptBy(existingPigeonIds, pr => pr.Pigeon.Id)
                .Select(pr => new PigeonEntity(pr.Pigeon, pr.OwnerId))
                .ToArray();
            await _pigeonRepository.AddRangeAsync(pigeonsToAdd);

            RaceEntity[] racesToAdd = races.Select(r => new RaceEntity(r))
                .ToArray();
            await _raceRepository.AddRangeAsync(racesToAdd);

            PigeonRaceEntity[] pigeonRacesToAdd = races.SelectMany(Race => Race.PigeonRaces.Select(PigeonRace => (PigeonRace, Race.Code)))
                .Select(prr => new PigeonRaceEntity(prr.PigeonRace, prr.Code))
                .ToArray();

            await _pigeonRaceRepository.AddRangeAsync(pigeonRacesToAdd);
        }

        public async Task StoreRaceAsync(Race race)
        {
            if (await _raceRepository.IsRaceCodePresentAsync(race.Code))
                return;

            IEnumerable<Owner> raceOwners = race.OwnerRaces.Select(or => or.Owner);
            await AddMissingOwners(raceOwners);

            IEnumerable<PigeonEntity> allPigeonsInRace = await AddMissingPigeons(race.PigeonRaces);

            RaceEntity addedRace = await _raceRepository.AddAsync(new RaceEntity(race));

            IEnumerable<PigeonRaceEntity> pigeonRacesToAdd = race.PigeonRaces.Select(pr => new PigeonRaceEntity(pr, addedRace.Code));
            await _pigeonRaceRepository.AddRangeAsync(pigeonRacesToAdd);
        }

        private async Task<IEnumerable<OwnerEntity>> AddMissingOwners(IEnumerable<Owner> owners)
        {
            IEnumerable<OwnerEntity> existingOwners = await _ownerRepository.GetAllByOwnerIdsAsync(owners.Select(o => o.Id));
            HashSet<OwnerId> ownerIds = existingOwners.Select(o => o.OwnerId).ToHashSet();

            IEnumerable<Owner> ownersToAdd = owners.Where(o => !ownerIds.Contains(o.Id));

            IEnumerable<OwnerEntity> addedOwners = await _ownerRepository.AddRangeAsync(ownersToAdd.Select(o => new OwnerEntity(o)));

            return await _ownerRepository.GetAllByOwnerIdsAsync(owners.Select(o => o.Id));
        }

        private async Task<IEnumerable<PigeonEntity>> AddMissingPigeons(IEnumerable<PigeonRace> allPigeonRaces)
        {
            IEnumerable<Pigeon> pigeons = allPigeonRaces.Select(pr => pr.Pigeon);
            IEnumerable<PigeonEntity> existingPigeons = await _pigeonRepository.GetByPigeonIdsAsync(pigeons.Select(p => p.Id));

            HashSet<PigeonId> existingPigeonIds = existingPigeons.Select(p => p.Id).ToHashSet();

            PigeonEntity[] pigeonsToAdd = allPigeonRaces.ExceptBy(existingPigeonIds, pr => pr.Pigeon.Id)
                .Select(pr => new PigeonEntity(pr.Pigeon, pr.OwnerId))
                .ToArray();

            await _pigeonRepository.AddRangeAsync(pigeonsToAdd);

            return await _pigeonRepository.GetByPigeonIdsAsync(pigeons.Select(p => p.Id));
        }

        public async Task<Race> GetRaceByCodeAsync(string code)
        {
            RaceEntity race = await _raceRepository.GetByCodeAsync(code);

            RaceSettings settings = await _settingsProvider.GetSettingsAsync();
            RacePointsSettings racePointsSettings = settings.RacePointsSettings.FirstOrDefault(rps => rps.RaceType == race.Type, new());
            INeutralizationTime neutralizationTime = settings.GetNeutralizationTimeForRaceType(race.Type, _appSettings.Value.Year);

            return race.ToRace(racePointsSettings.PointsQuotient, racePointsSettings.MaxPoints, racePointsSettings.MinPoints, racePointsSettings.DecimalCount, neutralizationTime);
        }

        public async Task DeleteRaceByCodeAsync(string code)
        {
            await _pigeonRaceRepository.DeleteAllByRaceCodeAsync(code);
            await _raceRepository.DeleteRaceByCodeAsync(code);
        }
    }
}
