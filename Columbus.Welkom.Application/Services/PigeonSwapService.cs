using Columbus.Models;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Export;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Columbus.Welkom.Application.Settings;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Services
{
    public class PigeonSwapService : IPigeonSwapService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IPigeonSwapRepository _pigeonSwapRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly RacePointsSettings _racePointsSettings;

        public PigeonSwapService(
            IFileProvider fileProvider, 
            IPigeonRepository pigeonRepository, 
            IPigeonSwapRepository pigeonSwapRepository, 
            IRaceRepository raceRepository,
            IOptions<RacePointsSettings> racePointsSettings)
        {
            _fileProvider = fileProvider;
            _pigeonRepository = pigeonRepository;
            _pigeonSwapRepository = pigeonSwapRepository;
            _raceRepository = raceRepository;
            _racePointsSettings = racePointsSettings.Value;
        }

        public async Task<IEnumerable<PigeonSwapPair>> GetPigeonSwapPairsAsync()
        {
            IEnumerable<PigeonSwapEntity> pigeonSwapEntities = await _pigeonSwapRepository.GetAllWithOwnersAndPigeonAsync();

            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync([RaceType.L]);
            IEnumerable<Race> races = raceEntities.Select(r => r.ToRace(_racePointsSettings.PointsQuotient, _racePointsSettings.MaxPoints, _racePointsSettings.MinPoints));

            List<PigeonSwapPair> pigeonSwapPairs = pigeonSwapEntities.Select(ps => ps.ToPigeonSwapPair()).ToList();

            Dictionary<Pigeon, PigeonSwapPair> pigeonPigeonSwapPairs = pigeonSwapPairs.ToDictionary(ps => ps.Pigeon!, ps => ps);
            HashSet<Pigeon> pigeonsInPairs = pigeonSwapPairs.Select(ps => ps.Pigeon!).ToHashSet();

            foreach (Race race in races)
            {
                IEnumerable<PigeonRace> pigeonRaces = race.PigeonRaces.Where(pr => pigeonsInPairs.Contains(pr.Pigeon));
                SimpleRace simpleRace = new SimpleRace(race.Number, race.Type, race.Name, race.Code, race.StartTime, race.Location, race.OwnerRaces.Count, race.PigeonRaces.Count);

                int prizeCount = pigeonRaces.Where(pr => pr.ArrivalTime != DateTime.MinValue).Count();
                double pointStep = 170 / Math.Max(prizeCount - 1, 1);
                int i = 0;
                foreach (PigeonRace pigeonRace in pigeonRaces)
                {
                    int points = Convert.ToInt32(Math.Round(200.0 - pointStep * i++));
                    pigeonPigeonSwapPairs[pigeonRace.Pigeon].RacePoints!.Add(simpleRace, points);
                }
            }

            return pigeonSwapPairs.OrderByDescending(ps => ps.Points);
        }

        public async Task UpdatePigeonSwapPairAsync(int year, PigeonSwapPair pigeonSwapPair)
        {
            if (pigeonSwapPair.Pigeon is null || pigeonSwapPair.CoupledPlayer is null)
                return;

            if (pigeonSwapPair.Player is null || pigeonSwapPair.Owner is null)
                throw new ArgumentNullException("PigeonSwapPair Player, Owner, Pigeon, and CoupledOwner cannot be null.");

            PigeonEntity pigeon = await _pigeonRepository.GetByPigeonIdAsync(pigeonSwapPair.Pigeon.Id);
            PigeonSwapEntity? pigeonSwapEntity = await _pigeonSwapRepository.GetByIdAsync(pigeonSwapPair.Id);

            if (pigeonSwapPair.Id == 0)
            {
                await _pigeonSwapRepository.AddAsync(new PigeonSwapEntity()
                {
                    PlayerId = pigeonSwapPair.Player.Id,
                    OwnerId = pigeonSwapPair.Owner.Id,
                    PigeonId = pigeon.Id,
                    CoupledPlayerId = pigeonSwapPair.CoupledPlayer.Id
                });
            }
            else
            {
                if (pigeonSwapEntity is null)
                    throw new ArgumentException("No PigeonSwap entry by this id.");

                pigeonSwapEntity.PigeonId = pigeon.Id;
                pigeonSwapEntity.CoupledPlayerId = pigeonSwapPair.CoupledPlayer.Id;
                await _pigeonSwapRepository.UpdateAsync(pigeonSwapEntity);
            }
        }

        public async Task DeletePigeonSwapPairAsync(PigeonSwapPair pigeonSwapPair)
        {
            if (pigeonSwapPair.Player is null || pigeonSwapPair.Pigeon is null)
                throw new InvalidOperationException("PigeonSwapPair player and pigeon cannot be null");

            await _pigeonSwapRepository.DeleteByPlayerAndPigeonAsync(pigeonSwapPair.Player.Id, pigeonSwapPair.Pigeon.Id);
        }

        public async Task ExportToPdf(IEnumerable<PigeonSwapPair> pigeonSwapPairs)
        {
            PigeonSwapDocument document = new PigeonSwapDocument(pigeonSwapPairs);
            byte[] data = document.GetDocument();
        }
    }
}
