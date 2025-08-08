using Columbus.Models.Owner;
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

namespace Columbus.Welkom.Application.Services;

public class TeamsService(
    ITeamsRepository teamsRepository,
    IRaceRepository raceRepository,
    SettingsProvider settingsProvider,
    IOptions<AppSettings> appSettings,
    IFilePicker filePicker) : ITeamsService
{
    private readonly ITeamsRepository _teamsRepository = teamsRepository;
    private readonly IRaceRepository _raceRepository = raceRepository;
    private readonly SettingsProvider _settingsProvider = settingsProvider;
    private readonly IOptions<AppSettings> _appSettings = appSettings;
    private readonly IFilePicker _filePicker = filePicker;

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        ICollection<TeamEntity> teams = await _teamsRepository.GetAllWithTeamOwnersAync();

        RaceSettings raceSettings = await _settingsProvider.GetSettingsAsync();
        Dictionary<RaceType, RacePointsSettings> racePointsSettingsByRaceType = raceSettings.RacePointsSettings.ToDictionary(rps => rps.RaceType);
        Dictionary<RaceType, INeutralizationTime> neutralizationTimesByRaceType = raceSettings.GetNeutralizationTimesByRaceType(_appSettings.Value.Year);

        IEnumerable<RaceEntity> raceEntities = await _raceRepository.GetAllByTypesAsync(raceSettings.AppliedRaceTypes.TeamRaceTypes.ToArray());
        IEnumerable<Race> races = raceEntities.Select(re => re.ToRace(
            racePointsSettingsByRaceType[re.Type].PointsQuotient,
            racePointsSettingsByRaceType[re.Type].MaxPoints,
            racePointsSettingsByRaceType[re.Type].MinPoints,
            racePointsSettingsByRaceType[re.Type].DecimalCount,
            neutralizationTimesByRaceType[re.Type]));

        Dictionary<OwnerId, double> pointsByOwnerId = races.SelectMany(r => r.PigeonRaces
                .GroupBy(pr => pr.OwnerId)
                .Select(opr => new { OwnerId = opr.Key, RacePoints = opr.Max(pr => pr.Points) ?? 0 })
            )
            .GroupBy(op => op.OwnerId)
            .ToDictionary(orps => orps.Key, orps => orps.Sum(orp => orp.RacePoints));

        return teams.Select(t => new Team
        {
            Number = t.Number,
            TeamOwners = t.TeamOwners.Select(to => new TeamOwner 
            { 
                Owner = to.Owner!.ToOwner(),
                Position = to.Position,
                Points = pointsByOwnerId.GetValueOrDefault(to.OwnerId, 0)
            }).ToHashSet()
        })
            .OrderByDescending(t => t.TotalPoints)
            .ToList();
    }

    public async Task DeleteTeamAsync(Team team)
    {
        TeamEntity? existingTeam = await _teamsRepository.GetByNumberAsync(team.Number);
        if (existingTeam is null)
            throw new ArgumentException("No team with that number exists.");

        await _teamsRepository.DeleteAsync(existingTeam);
    }

    public async Task UpdateTeamAsync(Team team)
    {
        TeamEntity? existingTeam = await _teamsRepository.GetByNumberAsync(team.Number);

        if (existingTeam is null)
        {
            TeamEntity teamToAdd = new()
            {
                Number = team.Number,
                TeamOwners = team.TeamOwners.Select(to => new OwnerTeamEntity 
                {
                   OwnerId = to.Owner!.Id,
                   Position = to.Position,
                }).ToList(),
            };

            await _teamsRepository.AddAsync(teamToAdd);
        }
        else
        {
            if (team.TeamOwners.Any(to => to.Owner is null))
                throw new ArgumentException("Owner is not set for an entry.");

            IEnumerable<OwnerTeamEntity> teamOwnersToAdd = team.TeamOwners.ExceptBy(existingTeam.TeamOwners.Select(to => to.OwnerId), to => to.Owner!.Id)
                .Select(to => new OwnerTeamEntity { OwnerId = to.Owner!.Id, Position = to.Position, TeamNumber = team.Number });
            IEnumerable<OwnerTeamEntity> teamOwnersToKeep = existingTeam.TeamOwners.IntersectBy(team.TeamOwners.Select(to => to.Owner!.Id), to => to.OwnerId);

            existingTeam.TeamOwners = teamOwnersToKeep.Concat(teamOwnersToAdd).ToList();
            await _teamsRepository.UpdateAsync(existingTeam);
        }
    }

    public async Task ExportAsync(IEnumerable<Team> teams)
    {
        RaceEntity mostRecentRace = await _raceRepository.GetMostRecentRaceAsync();

        Teams documentTeams = new()
        {
            ClubId = _appSettings.Value.Club,
            Year = _appSettings.Value.Year,
            AllTeams = teams.ToList(),
            LastRaceName = mostRecentRace.Name,
            LastRaceDate = mostRecentRace.StartTime,
        };

        TeamsDocument document = new(documentTeams);
        byte[] pdf = document.GeneratePdf();

        await _filePicker.SaveFileAsync("Ploegenspel.pdf", new MemoryStream(pdf));
    }
}
