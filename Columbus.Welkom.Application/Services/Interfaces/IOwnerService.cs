using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetOwnersWithAllPigeonsAsync();
        Task<IEnumerable<Owner>> GetOwnersWithYearPigeonsAsync(int year, bool includeOwnersWithoutPigeons = false);
        Task UpdateOwnersAsync(IEnumerable<Owner> owners);

        /// <summary>
        /// Reads the configured club's members, with their lofts, from the Venira database.
        /// </summary>
        Task<IEnumerable<Owner>> ReadOwnersFromVeniraAsync();
    }
}