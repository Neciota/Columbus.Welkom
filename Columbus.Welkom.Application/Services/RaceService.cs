using Columbus.Models;
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
using System.Linq;
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
            IEnumerable<(StreamReader StreamReader, string FileName)> files = await _filePicker.OpenFilesAsync([".udp"], new Regex(@"W...[0-9]{4}.udp"));

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

            ClubId club = ClubId.Create(_appSettings.Value.Club);
            races = races.Where(r => r.OwnerRaces.Any(or => or.Owner.Id.HasClubId(club)));

            IEnumerable<OwnerEntity> existingOwners = await _ownerRepository.GetByOwnerIdsAsync(races.SelectMany(r => r.OwnerRaces.Select(o => o.Owner.Id)));
            HashSet<OwnerId> existingOwnerIds = existingOwners.Select(o => o.OwnerId).ToHashSet();

            OwnerEntity[] ownersToAdd = races.SelectMany(r => r.OwnerRaces)
                .ExceptBy(existingOwnerIds, or => or.Owner.Id)
                .Select(or => new OwnerEntity(or.Owner))
                .Where(or => or.OwnerId.HasClubId(club))
                .ToArray();
            await _ownerRepository.AddRangeAsync(ownersToAdd);
            existingOwnerIds = existingOwnerIds.Concat(ownersToAdd.Select(o => o.OwnerId)).ToHashSet();

            IEnumerable<PigeonEntity> existingPigeons = await _pigeonRepository.GetByPigeonIdsAsync(races.SelectMany(r => r.PigeonRaces.Select(pr => pr.Pigeon.Id)));
            Dictionary<PigeonId, PigeonEntity> existingPigeonsById = existingPigeons.ToDictionary(p => p.Id);

            PigeonEntity[] pigeonsToAdd = races.SelectMany(r => r.PigeonRaces)
                .ExceptBy(existingPigeonsById.Keys, pr => pr.Pigeon.Id)
                .Select(pr => new PigeonEntity(pr.Pigeon, pr.OwnerId))
                .Where(pr => existingOwnerIds.Contains(pr.OwnerId))
                .ToArray();
            await _pigeonRepository.AddRangeAsync(pigeonsToAdd);

            foreach (PigeonRace pigeonRace in races.SelectMany(r => r.PigeonRaces)
                 .Where(pr => existingPigeonsById.ContainsKey(pr.Pigeon.Id))
                 .Where(pr => existingPigeonsById[pr.Pigeon.Id].OwnerId != pr.OwnerId))
            {
                existingPigeonsById[pigeonRace.Pigeon.Id].OwnerId = pigeonRace.OwnerId;
            }
            await _pigeonRepository.UpdateRangeAsync(existingPigeonsById.Values);

            RaceEntity[] racesToAdd = races.Select(r => new RaceEntity(r))
                .ToArray();
            await _raceRepository.AddRangeAsync(racesToAdd);

            Dictionary<(OwnerId Owner, string RaceCode), OwnerRace> ownerRacesByOwnerAndRace = races.SelectMany(r => r.OwnerRaces.Select(or => (or, r)))
                .ToDictionary(orr => (orr.or.Owner.Id, orr.r.Code), orr => orr.or);

            PigeonRaceEntity[] pigeonRacesToAdd = races.SelectMany(Race => Race.PigeonRaces.Select(PigeonRace => (PigeonRace, Race.Code)))
                .Where(pr => existingOwnerIds.Contains(pr.PigeonRace.OwnerId))
                .Select(prr => new PigeonRaceEntity(prr.PigeonRace, ownerRacesByOwnerAndRace[(prr.PigeonRace.OwnerId, prr.Code)].SubmissionAt, ownerRacesByOwnerAndRace[(prr.PigeonRace.OwnerId, prr.Code)].StoppedAt, ownerRacesByOwnerAndRace[(prr.PigeonRace.OwnerId, prr.Code)].ClockDeviation, prr.Code))
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

            Dictionary<OwnerId, OwnerRace> ownerRacesByOwner = race.OwnerRaces.ToDictionary(or => or.Owner.Id);

            IEnumerable<PigeonRaceEntity> pigeonRacesToAdd = race.PigeonRaces.Select(pr => new PigeonRaceEntity(pr, ownerRacesByOwner[pr.OwnerId].SubmissionAt, ownerRacesByOwner[pr.OwnerId].StoppedAt, ownerRacesByOwner[pr.OwnerId].ClockDeviation, addedRace.Code));
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
