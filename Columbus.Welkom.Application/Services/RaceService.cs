using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.UDP.Interfaces;
using Columbus.Welkom.Application.Models;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Columbus.Welkom.Application.Settings;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IPigeonRaceRepository _pigeonRaceRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly IRaceSerializer _raceSerializer;
        private readonly RacePointsSettings _racePointsSettings;

        private const string UdpFileExtension = ".udp";

        public RaceService(
            IFileProvider fileProvider, 
            IOwnerRepository ownerRepository, 
            IPigeonRepository pigeonRepository, 
            IPigeonRaceRepository pigeonRaceRepository, 
            IRaceRepository raceRepository, 
            IRaceSerializer raceSerializer,
            IOptions<RacePointsSettings> racePointsSettings)
        {
            _fileProvider = fileProvider;
            _ownerRepository = ownerRepository;
            _pigeonRepository = pigeonRepository;
            _pigeonRaceRepository = pigeonRaceRepository;
            _raceRepository = raceRepository;
            _raceSerializer = raceSerializer;
            _racePointsSettings = racePointsSettings.Value;
        }

        public async Task<Race> ReadRaceFromFileAsync(string filePath)
        {
            StreamReader streamReader = await _fileProvider.GetFileAsync(filePath);

            return await ReadRaceFromFile(streamReader);
        }

        public async Task<IEnumerable<Race>> ReadRacesFromDirectoryAsync(string directoryPath)
        {
            IEnumerable<string> filePaths = await _fileProvider.GetFilePathsAsync(directoryPath, UdpFileExtension);

            return await Task.WhenAll(filePaths.AsParallel().Select(ReadRaceFromFileAsync));
        }

        private async Task<Race> ReadRaceFromFile(StreamReader streamReader)
        {
            return await _raceSerializer.DeserializeAsync(streamReader);
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

            IEnumerable<Pigeon> pigeonData = races.SelectMany(r => r.PigeonRaces)
                .Select(pr => pr.Pigeon);
            IEnumerable<PigeonEntity> pigeonsInRaces = await _pigeonRepository.GetPigeonsByCountriesAndYearsAndRingNumbersAsync(pigeonData);

            await _raceRepository.AddRangeAsync(races.Select(r => new RaceEntity(r)));
        }

        public async Task StoreRaceAsync(Race race)
        {
            if (await _raceRepository.IsRaceCodePresentAsync(race.Code))
                return;

            IEnumerable<Owner> raceOwners = race.OwnerRaces.Select(or => or.Owner);
            await AddMissingOwners(raceOwners);

            IEnumerable<PigeonEntity> allPigeonsInRace = await AddMissingPigeons(raceOwners);

            RaceEntity addedRace = await _raceRepository.AddAsync(new RaceEntity(race));

            IEnumerable<PigeonRaceEntity> pigeonRacesToAdd = GetPigeonRaceEntities(race.PigeonRaces, allPigeonsInRace, addedRace.Id);

            await _pigeonRaceRepository.AddRangeAsync(pigeonRacesToAdd);
        }

        private async Task<IEnumerable<OwnerEntity>> AddMissingOwners(IEnumerable<Owner> owners)
        {
            IEnumerable<OwnerEntity> existingOwners = await _ownerRepository.GetAllByIdsAsync(owners.Select(o => o.Id.Value));
            HashSet<int> ownerIds = existingOwners.Select(o => o.Id).ToHashSet();

            IEnumerable<Owner> ownersToAdd = owners.Where(o => !ownerIds.Contains(o.Id.Value));

            IEnumerable<OwnerEntity> addedOwners = await _ownerRepository.AddRangeAsync(ownersToAdd.Select(o => new OwnerEntity(o)));

            return await _ownerRepository.GetAllByIdsAsync(owners.Select(o => o.Id.Value));
        }

        private async Task<IEnumerable<PigeonEntity>> AddMissingPigeons(IEnumerable<Owner> owners)
        {
            IEnumerable<OwnerEntity> existingOwners = await _ownerRepository.GetByOwnerIdsAsync(owners.Select(o => o.Id));
            IEnumerable<Pigeon> pigeons = owners.SelectMany(o => o.Pigeons);
            IEnumerable<PigeonEntity> existingPigeons = await _pigeonRepository.GetPigeonsByCountriesAndYearsAndRingNumbersAsync(pigeons);

            HashSet<int> existingPigeonsHashSet = existingPigeons.Select(p => p.GetHashCode()).ToHashSet();
            Dictionary<OwnerId, OwnerEntity> ownersByOwnerId = existingOwners.ToDictionary(eo => eo.OwnerId);

            List<PigeonEntity> pigeonsToAdd = new List<PigeonEntity>();
            foreach (Owner owner in owners)
            {
                foreach (Pigeon pigeon in owner.Pigeons)
                {
                    if (existingPigeonsHashSet.Contains(pigeon.GetHashCode()))
                        continue;

                    pigeonsToAdd.Add(new PigeonEntity(pigeon, ownersByOwnerId[owner.Id].Id));
                }
            }

            await _pigeonRepository.AddRangeAsync(pigeonsToAdd);

            return await _pigeonRepository.GetPigeonsByCountriesAndYearsAndRingNumbersAsync(pigeons);
        }

        private static IEnumerable<PigeonRaceEntity> GetPigeonRaceEntities(IList<PigeonRace> pigeonRaces, IEnumerable<PigeonEntity> pigeonEntities, int raceId)
        {
            Dictionary<int, PigeonEntity> pigeonEntitiesSet = pigeonEntities.ToDictionary(p => p.GetHashCode(), p => p);

            List<PigeonRaceEntity> pigeonRaceEntities = new List<PigeonRaceEntity>();
            foreach (PigeonRace pr in pigeonRaces)
            {
                bool found = pigeonEntitiesSet.TryGetValue(pr.Pigeon.GetHashCode(), out PigeonEntity? pigeonEntity);
                if (!found)
                    throw new ArgumentException($"Given pigeon list did not contain pigeon {pr.Pigeon}");
                pigeonRaceEntities.Add(new PigeonRaceEntity(pr, pigeonEntity!.Id, raceId));
            }

            return pigeonRaceEntities;
        }

        public async Task<Race> GetRaceByCodeAsync(string code)
        {
            RaceEntity race = await _raceRepository.GetByCodeAsync(code);

            return race.ToRace(_racePointsSettings.PointsQuotient, _racePointsSettings.MaxPoints, _racePointsSettings.MinPoints);
        }

        public async Task DeleteRaceByCodeAsync(string code)
        {
            RaceEntity race = await _raceRepository.GetByCodeAsync(code);

            await _pigeonRaceRepository.DeleteAllByRaceId(race.Id);

            await _raceRepository.DeleteRaceByCodeAsync(code);
        }
    }
}
