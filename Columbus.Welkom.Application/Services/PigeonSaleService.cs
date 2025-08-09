using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Export;
using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Columbus.Welkom.Application.Services;

public class PigeonSaleService(
    IPigeonSaleRepository pigeonSaleRepository,
    IRaceRepository raceRepository,
    SettingsProvider settingsProvider,
    IOptions<AppSettings> appSettings,
    IFilePicker filePicker) : IPigeonSaleService
{
    private readonly IPigeonSaleRepository _pigeonSaleRepository = pigeonSaleRepository;
    private readonly IRaceRepository _raceRepository = raceRepository;
    private readonly SettingsProvider _settingsProvider = settingsProvider;
    private readonly IOptions<AppSettings> _appSettings = appSettings;
    private readonly IFilePicker _filePicker = filePicker;

    public async Task DeleteAsync(PigeonSale pigeonSale)
    {
        PigeonSaleEntity? pigeonSaleToDelete = await _pigeonSaleRepository.GetByIdAsync(pigeonSale.Id);
        if (pigeonSaleToDelete is null)
            throw new ArgumentException("No pigeon sale with the given id exists.");

        await _pigeonSaleRepository.DeleteAsync(pigeonSaleToDelete);
    }

    public async Task<IEnumerable<PigeonSale>> GetAllAsync()
    {
        ICollection<PigeonSaleEntity> pigeonSales = await _pigeonSaleRepository.GetAllWithOwnersAndPigeonsAsync();
        HashSet<PigeonId> pigeonsIdsInCompetition = pigeonSales.Select(ps => ps.PigeonId).ToHashSet();

        RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
        Dictionary<RaceType, RacePointsSettings> racePointsSettingsByRaceType = raceSettings.RacePointsSettings.ToDictionary(rps => rps.RaceType);
        Dictionary<RaceType, INeutralizationTime> neutralizationTimesByRaceType = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

        IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync(raceSettings.AppliedRaceTypes.PigeonSaleRaceTypes.ToArray());
        IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(
            racePointsSettingsByRaceType[re.Type].PointsQuotient,
            racePointsSettingsByRaceType[re.Type].MaxPoints,
            racePointsSettingsByRaceType[re.Type].MinPoints,
            racePointsSettingsByRaceType[re.Type].DecimalCount,
            neutralizationTimesByRaceType[re.Type]));

        Dictionary<PigeonId, IEnumerable<RacePoints>> racePointsByPigeon = races.SelectMany(r => GetRacePointsFromRace(pigeonsIdsInCompetition, r))
            .GroupBy(prp => prp.PigeonId)
            .ToDictionary(prpg => prpg.Key, prpg => prpg.Select(prp => prp.RacePoints));

        return pigeonSales.Select(ps => new PigeonSale
        {
            Id = ps.Id,
            Seller = ps.Seller?.ToOwner(),
            Buyer = ps.Buyer?.ToOwner(),
            Pigeon = ps.Pigeon?.ToPigeon(),
            RacePoints = racePointsByPigeon.GetValueOrDefault(ps.PigeonId)?.ToList() ?? []
        })
            .OrderByDescending(ps => ps.TotalPoints)
            .ToList();
    }

    private IEnumerable<(PigeonId PigeonId, RacePoints RacePoints)> GetRacePointsFromRace(HashSet<PigeonId> pigeonsIdsInCompetition, Race race)
    {
        const int maxPoints = 200;
        const int minPoints = 30;

        int finishingPigeonsCount = race.PigeonRaces.Where(pr => pigeonsIdsInCompetition.Contains(pr.Pigeon.Id))
            .Where(pr => pr.ArrivalTime.HasValue && pr.ArrivalTime.Value.Hour < 18)
            .Count();
        double pointStep = Convert.ToDouble(maxPoints - minPoints) / Math.Max(finishingPigeonsCount - 1, 1);

        return race.PigeonRaces.Where(pr => pigeonsIdsInCompetition.Contains(pr.Pigeon.Id))
            .Select((pr, i) => (pr.Pigeon.Id, new RacePoints
            {
                RaceCode = race.Code,
                Points = !pr.ArrivalTime.HasValue || pr.ArrivalTime.Value.Hour > 18 ? 0d : maxPoints - i * pointStep,
            }));
    }

    public async Task UpdateAsync(PigeonSale pigeonSale)
    {
        PigeonSaleEntity? pigeonSaleToUpdate = await _pigeonSaleRepository.GetByIdAsync(pigeonSale.Id);

        if (pigeonSaleToUpdate is null)
        {
            PigeonSaleEntity pigeonSaleToAdd = new()
            {
                SellerId = pigeonSale.Seller!.Id,
                BuyerId = pigeonSale.Buyer!.Id,
                PigeonId = pigeonSale.Pigeon!.Id,
            };

            await _pigeonSaleRepository.AddAsync(pigeonSaleToAdd);
        }
        else
        {
            pigeonSaleToUpdate.SellerId = pigeonSale.Seller!.Id;
            pigeonSaleToUpdate.BuyerId = pigeonSale.Buyer!.Id;
            pigeonSaleToUpdate.PigeonId = pigeonSale.Pigeon!.Id;

            await _pigeonSaleRepository.UpdateAsync(pigeonSaleToUpdate);
        }
    }

    public async Task ExportAsync(IEnumerable<PigeonSale> pigeonSales)
    {
        RaceEntity mostRecentRace = await _raceRepository.GetMostRecentRaceAsync();

        RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
        ICollection<SimpleRaceEntity> races = await _raceRepository.GetAllSimpleByTypesAsync(raceSettings.AppliedRaceTypes.PigeonSaleRaceTypes.ToArray());

        PigeonSales documentPigeonSales = new()
        {
            ClubId = _appSettings.Value.Club,
            Year = _appSettings.Value.Year,
            AllPigeonSales = pigeonSales.ToList(),
            Races = races.Select(r => r.ToSimpleRace()).ToList(),
            LastRaceName = mostRecentRace.Name,
            LastRaceDate = mostRecentRace.StartTime,
        };

        PigeonSaleDocument document = new(documentPigeonSales);
        byte[] pdf = document.GeneratePdf();

        await _filePicker.SaveFileAsync("Duivenverkoop.pdf", new MemoryStream(pdf));
    }
}
