using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface ISelectedYoungPigeonService
    {
        Task DeleteOwnerPigeonPairForYearAsync(int year, OwnerPigeonPair ownerPigeonPair);
        Task DeleteOwnerPigeonPairForYearAsync(int year, Pigeon pigeon);
        Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsByYearAsync(int year);
        Task UpdatePigeonForOwnerAsync(int year, OwnerPigeonPair ownerPigeonPair);
    }
}
