using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetOwnersWithAllPigeonsAsync();
        Task<IEnumerable<Owner>> GetOwnersWithYearPigeonsAsync(int year, bool includeOwnersWithoutPigeons = false);
        Task OverwriteOwnersAsync(IEnumerable<Owner> owners);
        Task<IEnumerable<Owner>> ReadOwnersFromFile(string filePath);
    }
}