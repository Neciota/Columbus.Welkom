using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces;

public interface ITeamsService
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task DeleteTeamAsync(Team team);
    Task UpdateTeamAsync(Team team);
    Task ExportAsync(IEnumerable<Team> teams);
}
