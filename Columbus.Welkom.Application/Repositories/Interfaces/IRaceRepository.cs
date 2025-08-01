﻿using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IRaceRepository : IBaseRepository<RaceEntity>
    {
        Task<int> DeleteRaceByCodeAsync(string code);
        Task<int> DeleteRangeAsync();
        Task<IEnumerable<RaceEntity>> GetAllByTypesAsync(RaceType[] types);
        Task<IEnumerable<SimpleRaceEntity>> GetAllSimpleAsync();
        Task<IEnumerable<SimpleRaceEntity>> GetAllSimpleByTypesAsync(RaceType[] types);
        Task<RaceEntity> GetByCodeAsync(string code);
        Task<bool> IsRaceCodePresentAsync(string code);
    }
}