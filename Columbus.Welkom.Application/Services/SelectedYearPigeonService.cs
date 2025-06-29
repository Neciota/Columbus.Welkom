using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Models;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Columbus.Welkom.Application.Settings;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Services
{
    public class SelectedYearPigeonService : ISelectedYearPigeonService
    {
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly ISelectedYearPigeonRepository _selectedYearPigeonRepository;
        private readonly RacePointsSettings _racePointsSettings;

        public SelectedYearPigeonService(
            IPigeonRepository pigeonRepository, 
            IRaceRepository raceRepository, 
            ISelectedYearPigeonRepository selectedYearPigeonRepository,
            IOptions<RacePointsSettings> racePointSettings)
        {
            _pigeonRepository = pigeonRepository;
            _raceRepository = raceRepository;
            _selectedYearPigeonRepository = selectedYearPigeonRepository;
            _racePointsSettings = racePointSettings.Value;
        }

        public async Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsAsync()
        {
            IEnumerable<SelectedYearPigeonEntity> selectedYearPigeonEntities = await _selectedYearPigeonRepository.GetAllAsync();
            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync([ 
                RaceType.V,
                RaceType.M,
                RaceType.E,
                RaceType.O,
                RaceType.X,
                RaceType.N
            ]);
            IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(_racePointsSettings.PointsQuotient, _racePointsSettings.MaxPoints, _racePointsSettings.MinPoints));

            List<OwnerPigeonPair> ownerPigeonPairs = selectedYearPigeonEntities.Select(syp => new OwnerPigeonPair(syp.Owner!.ToOwner(), syp.Pigeon!.ToPigeon()))
                .ToList();

            foreach (Race race in races)
            {
                Dictionary<Pigeon, PigeonRace> pigeonRaces = race.PigeonRaces.ToDictionary(pr => pr.Pigeon, pr => pr);
                foreach (var pair in ownerPigeonPairs.Where(pair => pigeonRaces.ContainsKey(pair.Pigeon!)))
                {
                    pair.Points += pigeonRaces[pair.Pigeon!].Points ?? 0;
                }
            }

            return ownerPigeonPairs.OrderByDescending(pair => pair.Points);
        }

        public async Task UpdatePigeonForOwnerAsync(int year, OwnerPigeonPair ownerPigeonPair)
        {
            if (ownerPigeonPair.Owner is null)
                return;

            SelectedYearPigeonEntity? selectedYearPigeonEntity = await _selectedYearPigeonRepository.GetByOwnerAsync(ownerPigeonPair.OwnerId);

            PigeonEntity pigeon = await _pigeonRepository.GetByCountryAndYearAndRingNumberAsync(ownerPigeonPair.Pigeon!.Country, ownerPigeonPair.Pigeon.Year, ownerPigeonPair.Pigeon.RingNumber);

            if (selectedYearPigeonEntity is null)
            {
                await _selectedYearPigeonRepository.AddAsync(new SelectedYearPigeonEntity()
                {
                    OwnerId = ownerPigeonPair.OwnerId,
                    PigeonId = pigeon.Id
                });
            } else
            {
                selectedYearPigeonEntity.PigeonId = pigeon.Id;
                await _selectedYearPigeonRepository.UpdateAsync(selectedYearPigeonEntity);
            }
        }

        public async Task DeleteOwnerPigeonPairForYearAsync(int year, Pigeon pigeon)
        {
            SelectedYearPigeonEntity? oldPair = await _selectedYearPigeonRepository.GetByPigeonAsync(pigeon.Year, pigeon.Country, pigeon.RingNumber);

            if (oldPair is null)
                throw new InvalidOperationException("No pair with this pigeon present.");

            await _selectedYearPigeonRepository.DeleteByOwnerAsync(oldPair.Owner!.Id);
        }

        public async Task DeleteOwnerPigeonPairForYearAsync(int year, OwnerPigeonPair ownerPigeonPair)
        {
            if (ownerPigeonPair.Owner is null)
                throw new InvalidOperationException("Owner cannot be null");

            await _selectedYearPigeonRepository.DeleteByOwnerAsync(ownerPigeonPair.Owner.Id.Value);
        }
    }
}
