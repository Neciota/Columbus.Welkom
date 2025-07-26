using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISelectedYearPigeonService
    {
        Task DeleteOwnerPigeonPairByIdAsync(int id);
        Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsAsync();
        Task UpdateAsync(OwnerPigeonPair ownerPigeonPair);
    }
}
