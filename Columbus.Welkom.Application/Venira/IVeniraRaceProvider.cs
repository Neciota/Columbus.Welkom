using Columbus.Models;
using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Builds <see cref="Race"/> models from Venira's flight, arrival and clock tables.
/// </summary>
public interface IVeniraRaceProvider
{
    /// <summary>
    /// Every flight of <paramref name="year"/> that has been flown and calculated, with the
    /// entries of <paramref name="club"/> attached.
    /// </summary>
    Task<IEnumerable<Race>> GetRacesAsync(int year, ClubId club);
}
