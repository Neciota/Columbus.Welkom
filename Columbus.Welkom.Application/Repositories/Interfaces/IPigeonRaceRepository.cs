using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IPigeonRaceRepository : IBaseRepository<PigeonRaceEntity>
    {
        Task DeleteAllByRaceCodeAsync(string raceCode);
    }
}
