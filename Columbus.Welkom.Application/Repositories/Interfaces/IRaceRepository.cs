using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IRaceRepository : IBaseRepository<RaceEntity>
    {
        Task<int> DeleteRaceByCodeAsync(string code);
        Task<int> DeleteRangeAsync();
        Task<ICollection<RaceEntity>> GetAllByTypesAsync(RaceType[] types);
        Task<ICollection<SimpleRaceEntity>> GetAllSimpleAsync();
        Task<ICollection<SimpleRaceEntity>> GetAllSimpleByTypesAsync(RaceType[] types);

        /// <summary>
        /// The races whose flight code starts with one of the given letters, e.g. <c>L</c> for
        /// the natour flights. See <see cref="RaceCodeLetter"/> for why this is not the same
        /// grouping as the race type.
        /// </summary>
        Task<ICollection<RaceEntity>> GetAllByCodeLettersAsync(string[] codeLetters);

        /// <inheritdoc cref="GetAllByCodeLettersAsync"/>
        Task<ICollection<SimpleRaceEntity>> GetAllSimpleByCodeLettersAsync(string[] codeLetters);

        /// <summary>
        /// The distinct flight-code letters over the stored races, in alphabetical order.
        /// </summary>
        Task<ICollection<RaceCodeLetter>> GetCodeLettersAsync();
        Task<RaceEntity> GetByCodeAsync(string code);
        Task<bool> IsRaceCodePresentAsync(string code);
        Task<RaceEntity> GetMostRecentRaceAsync();
    }
}