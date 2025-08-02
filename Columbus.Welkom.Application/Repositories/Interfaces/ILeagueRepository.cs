using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces;

public interface ILeagueRepository : IBaseRepository<LeagueEntity>
{
    Task<ICollection<LeagueEntity>> GetAllWithOwnersAsync();
    Task<LeagueEntity?> GetByRankAsync(int rank);
}
