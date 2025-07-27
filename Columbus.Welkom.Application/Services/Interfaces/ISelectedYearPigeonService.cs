using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISelectedYearPigeonService
    {
        Task DeleteOwnerPigeonPairByIdAsync(int id);
        Task ExportAsync(IEnumerable<OwnerPigeonPair> ownerPigeonPairs);
        Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsAsync();
        Task UpdateAsync(OwnerPigeonPair ownerPigeonPair);
    }
}
