using Columbus.Models;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Export;
using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Columbus.Welkom.Application.Services
{
    public class SelectedYearPigeonService : ISelectedYearPigeonService
    {
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IRaceRepository _raceRepository;
        private readonly ISelectedYearPigeonRepository _selectedYearPigeonRepository;
        private readonly SettingsProvider _settingsProvider;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IFilePicker _filePicker;

        public SelectedYearPigeonService(
            IPigeonRepository pigeonRepository, 
            IRaceRepository raceRepository, 
            ISelectedYearPigeonRepository selectedYearPigeonRepository,
            SettingsProvider settingsProvider,
            IOptions<AppSettings> appSettings, 
            IFilePicker filePicker)
        {
            _pigeonRepository = pigeonRepository;
            _raceRepository = raceRepository;
            _selectedYearPigeonRepository = selectedYearPigeonRepository;
            _settingsProvider = settingsProvider;
            _appSettings = appSettings;
            _filePicker = filePicker;
        }

        public async Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsAsync()
        {
            IEnumerable<SelectedYearPigeonEntity> selectedYearPigeonEntities = await _selectedYearPigeonRepository.GetAllAsync();
            RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
            Dictionary<RaceType, RacePointsSettings> racePointsSettingsByRaceType = raceSettings.RacePointsSettings.ToDictionary(rps => rps.RaceType);
            Dictionary<RaceType, INeutralizationTime> neutralizationTimesByRaceType = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

            IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync(raceSettings.AppliedRaceTypes.SelectedYearPigeonRaceTypes.ToArray());
            IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(
                racePointsSettingsByRaceType[re.Type].PointsQuotient, 
                racePointsSettingsByRaceType[re.Type].MaxPoints, 
                racePointsSettingsByRaceType[re.Type].MinPoints, 
                racePointsSettingsByRaceType[re.Type].DecimalCount,
                neutralizationTimesByRaceType[re.Type]));

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

        public async Task UpdateAsync(OwnerPigeonPair ownerPigeonPair)
        {
            if (ownerPigeonPair.Owner is null || ownerPigeonPair.Pigeon is null)
                return;

            PigeonEntity? pigeon = await _pigeonRepository.GetByPigeonIdAsync(ownerPigeonPair.Pigeon.Id);
            if (pigeon is null)
                return;

            SelectedYearPigeonEntity? selectedYearPigeonEntity = await _selectedYearPigeonRepository.GetByIdAsync(ownerPigeonPair.Id);

            if (selectedYearPigeonEntity is null)
            {
                await _selectedYearPigeonRepository.AddAsync(new SelectedYearPigeonEntity()
                {
                    OwnerId = ownerPigeonPair.Owner.Id,
                    PigeonId = ownerPigeonPair.Pigeon.Id,
                });
            } else
            {
                selectedYearPigeonEntity.OwnerId = ownerPigeonPair.Owner.Id;
                selectedYearPigeonEntity.PigeonId = pigeon.Id;
                await _selectedYearPigeonRepository.UpdateAsync(selectedYearPigeonEntity);
            }
        }

        public async Task DeleteOwnerPigeonPairByIdAsync(int ownerPigeonPairId)
        {
            SelectedYearPigeonEntity? oldPair = await _selectedYearPigeonRepository.GetByIdAsync(ownerPigeonPairId);

            if (oldPair is null)
                throw new InvalidOperationException("No pair with this pigeon present.");

            await _selectedYearPigeonRepository.DeleteAsync(oldPair);
        }

        public async Task ExportAsync(IEnumerable<OwnerPigeonPair> ownerPigeonPairs)
        {
            RaceEntity mostRecentRace = await _raceRepository.GetMostRecentRaceAsync();

            SelectedYearPigeon selectedYearPigeon = new()
            {
                ClubId = _appSettings.Value.Club,
                Year = _appSettings.Value.Year,
                OwnerPigeonPairs = ownerPigeonPairs,
                LastRaceName = mostRecentRace.Name,
                LastRaceDate = mostRecentRace.StartTime,
            };

            SelectedYearPigeonDocument document = new(selectedYearPigeon);
            byte[] pdf = document.GeneratePdf();

            await _filePicker.SaveFileAsync("Tientjesduif.pdf", new MemoryStream(pdf));
        }
    }
}
