using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>Vlucht.DB</c> (flights). The table holds the whole season's schedule,
/// including flights that have not been flown yet; see <see cref="HasResults"/>.
/// </summary>
public sealed record VeniraFlightRow(
    int FlightId,
    short? Number,
    string? Code,
    string? RaceTypeCode,
    string? ReleaseSite,
    DateTime? ReleaseDate,
    TimeSpan? ReleaseTime,
    int? EnteredCount,
    bool Cancelled)
{
    /// <summary>
    /// Whether the flight has been basketed and calculated. Scheduled-but-not-yet-flown rows
    /// have no entries and no release time.
    /// </summary>
    public bool HasResults => !Cancelled && EnteredCount > 0 && ReleaseDate is not null;

    public static VeniraFlightRow FromRecord(ParadoxRecord record) => new(
        FlightId: record.GetInt("VluchtID") ?? 0,
        // Vluchtnummer is sequential across the whole season regardless of race type.
        Number: record.GetShort("Vluchtnummer"),
        // Venira's own code for the flight, e.g. L32. It is the code shown throughout Venira and
        // on its result lists, so it is what owners recognise a flight by. It is neither derived
        // from Vluchtsoort nor from Vluchtnummer: the letter tracks the race type only loosely
        // (J flights are coded L, O flights A, A flights Z, I flights F) and the digits are the
        // week of the release, not the flight number.
        Code: record.GetText("Vluchtcode"),
        RaceTypeCode: record.GetText("Vluchtsoort"),
        ReleaseSite: record.GetText("Lossingsplaats"),
        ReleaseDate: record.GetDate("Lossingsdatum"),
        ReleaseTime: record.GetTime("Lossingstijd"),
        EnteredCount: record.GetInt("Mee"),
        Cancelled: record.GetBool("Geannuleerd") ?? false);
}
