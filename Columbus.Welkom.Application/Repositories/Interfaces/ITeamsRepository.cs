using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces;

public interface ITeamsRepository : IBaseRepository<TeamEntity>
{
    Task<ICollection<TeamEntity>> GetAllWithTeamOwnersAync();
    Task<TeamEntity?> GetByNumberAsync(int number);
}
