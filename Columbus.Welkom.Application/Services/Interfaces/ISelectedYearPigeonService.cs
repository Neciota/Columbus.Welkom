using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISelectedYearPigeonService
    {
        Task DeleteOwnerPigeonPairForYearAsync(int year, Pigeon pigeon);
        Task DeleteOwnerPigeonPairForYearAsync(int year, OwnerPigeonPair ownerPigeonPair);
        Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsAsync();
        Task UpdatePigeonForOwnerAsync(int year, OwnerPigeonPair ownerPigeonPair);
    }
}
