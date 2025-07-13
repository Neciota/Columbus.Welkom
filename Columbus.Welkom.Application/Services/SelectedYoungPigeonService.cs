using Columbus.Models;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Columbus.Welkom.Application.Settings;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Services
{
    public class SelectedYoungPigeonService : ISelectedYoungPigeonService
    {
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly ISelectedYoungPigeonRepository _selectedYoungPigeonRepository;
        private readonly RacePointsSettings _racePointsSettings;

        public SelectedYoungPigeonService(
            IPigeonRepository pigeonRepository, 
            IRaceRepository raceRepository, 
            ISelectedYoungPigeonRepository selectedYoungPigeonRepository,
            IOptions<RacePointsSettings> racePointSettings)
        {
            _pigeonRepository = pigeonRepository;
            _raceRepository = raceRepository;
            _selectedYoungPigeonRepository = selectedYoungPigeonRepository;
            _racePointsSettings = racePointSettings.Value;
        }

        public async Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsByYearAsync(int year)
        {
            IEnumerable<SelectedYoungPigeonEntity> selectedYoungPigeonEntities = await _selectedYoungPigeonRepository.GetAllAsync();
            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync([
                RaceType.J,
                RaceType.L,
                RaceType.F
            ]);
            IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(_racePointsSettings.PointsQuotient, _racePointsSettings.MaxPoints, _racePointsSettings.MinPoints));

            List<OwnerPigeonPair> ownerPigeonPairs = selectedYoungPigeonEntities.Select(syp => new OwnerPigeonPair(syp.Owner!.ToOwner(), syp.Pigeon!.ToPigeon()))
                .ToList();

            foreach (Race race in races)
            {
                Dictionary<Pigeon, PigeonRace> pigeonRaces = race.PigeonRaces.ToDictionary(pr => pr.Pigeon, pr => pr);

                foreach (OwnerPigeonPair pair in ownerPigeonPairs.Where(pair => pigeonRaces.ContainsKey(pair.Pigeon!)))
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

            SelectedYoungPigeonEntity? selectedYearPigeonEntity = await _selectedYoungPigeonRepository.GetByOwnerAsync(ownerPigeonPair.Owner.Id);

            PigeonEntity pigeon = await _pigeonRepository.GetByPigeonIdAsync(ownerPigeonPair.Pigeon.Id);

            if (selectedYearPigeonEntity is null)
            {
                await _selectedYoungPigeonRepository.AddAsync(new SelectedYoungPigeonEntity()
                {
                    OwnerId = ownerPigeonPair.Owner.Id,
                    PigeonId = ownerPigeonPair.Pigeon.Id,
                });
            }
            else
            {
                selectedYearPigeonEntity.PigeonId = pigeon.Id;
                await _selectedYoungPigeonRepository.UpdateAsync(selectedYearPigeonEntity);
            }
        }

        public async Task DeleteOwnerPigeonPairForYearAsync(int year, Pigeon pigeon)
        {
            SelectedYoungPigeonEntity? oldPair = await _selectedYoungPigeonRepository.GetByPigeonIdAsync(pigeon.Id);

            if (oldPair is null)
                throw new InvalidOperationException("No pair with this pigeon present.");

            await _selectedYoungPigeonRepository.DeleteByOwnerAsync(oldPair.Owner.OwnerId);
        }

        public async Task DeleteOwnerPigeonPairForYearAsync(int year, OwnerPigeonPair ownerPigeonPair)
        {
            if (ownerPigeonPair.Owner is null)
                throw new InvalidOperationException("Owner cannot be null");

            await _selectedYoungPigeonRepository.DeleteByOwnerAsync(ownerPigeonPair.Owner.Id);
        }
    }
}
