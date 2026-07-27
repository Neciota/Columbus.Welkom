using Columbus.Models;
using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Builds <see cref="Owner"/> models, with their lofts attached, from Venira's member and
/// pigeon tables.
/// </summary>
public interface IVeniraOwnerProvider
{
    /// <param name="club">Only members of this club are returned; Venira also holds guests.</param>
    Task<IEnumerable<Owner>> GetOwnersAsync(ClubId club);
}
