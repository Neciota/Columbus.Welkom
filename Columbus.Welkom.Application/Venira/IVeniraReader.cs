using Columbus.Welkom.Application.Venira.Rows;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Reads Venira's Paradox tables as rows. One method per table; joining and mapping to domain
/// models is the callers' job.
/// </summary>
public interface IVeniraReader
{
    /// <summary>Members, from <c>Liefhbr.DB</c>.</summary>
    Task<IReadOnlyList<VeniraOwnerRow>> GetOwnersAsync();

    /// <summary>Loft lists, from <c>DUIF.DB</c>.</summary>
    Task<IReadOnlyList<VeniraPigeonRow>> GetPigeonsAsync();

    /// <summary>The season's flights, from <c>Vlucht.DB</c>.</summary>
    Task<IReadOnlyList<VeniraFlightRow>> GetFlightsAsync();

    /// <summary>Basketed pigeons and their arrivals, from <c>Aankomst.DB</c>.</summary>
    Task<IReadOnlyList<VeniraArrivalRow>> GetArrivalsAsync();

    /// <summary>Clock readings, from <c>Klok.DB</c>.</summary>
    Task<IReadOnlyList<VeniraClockRow>> GetClocksAsync();

    /// <summary>Release sites, from <c>Losplaat.DB</c>.</summary>
    Task<IReadOnlyList<VeniraReleaseSiteRow>> GetReleaseSitesAsync();

    /// <summary>The NPO sunrise/sunset table, from <c>NEUTTIJD.DB</c>.</summary>
    Task<IReadOnlyList<VeniraSolarPeriodRow>> GetSolarPeriodsAsync();
}
