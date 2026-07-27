using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>Aankomst.DB</c>: one basketed pigeon in one flight, whether or not it
/// came home.
/// </summary>
public sealed record VeniraArrivalRow(
    int FlightId,
    string? MemberNumber,
    string? ClubNumber,
    short? Mark,
    short? ArrivalOrder,
    short? ClockIndex,
    string? RingNumber,
    string? CountryCode,
    string? Sex,
    string? Chip,
    DateTime? ArrivalDate,
    TimeSpan? ArrivalTime)
{
    /// <summary>
    /// Venira's sentinel for "this pigeon has no place": <c>Volgnr</c> is set to
    /// <see cref="short.MaxValue"/> rather than left NULL.
    /// </summary>
    private const short NoArrivalOrder = short.MaxValue;

    /// <summary>
    /// Whether the pigeon was clocked. Venira marks unclocked rows with clock 0, and only
    /// those; the date is a weaker signal, because arrivals typed in by hand rather than read
    /// from an ECS clock get a time but no <c>Constateringsdatum</c>.
    /// </summary>
    public bool HasArrived => ClockIndex > 0 && ArrivalTime is not null;

    /// <summary>
    /// The pigeon's arrival position within its owner's entry, or 0 when it did not arrive.
    /// </summary>
    public int OwnerArrivalOrder => ArrivalOrder is null or NoArrivalOrder ? 0 : ArrivalOrder.Value;

    public static VeniraArrivalRow FromRecord(ParadoxRecord record) => new(
        FlightId: record.GetInt("VluchtID") ?? 0,
        MemberNumber: record.GetText("NPOlidnr"),
        ClubNumber: record.GetText("NPOvernr"),
        // Getekend is the designated-pigeon number the championships score on.
        Mark: record.GetShort("Getekend"),
        ArrivalOrder: record.GetShort("Volgnr"),
        // Which of the owner's clocks recorded this pigeon; indexes into Klok.DB's column
        // families. 0 means the pigeon was never clocked.
        ClockIndex: record.GetShort("Klok"),
        RingNumber: record.GetText("Ringnr"),
        CountryCode: record.GetText("Landcode"),
        Sex: record.GetText("Geslacht"),
        Chip: record.GetText("Chipring"),
        // The raw reading on the owner's own clock; Zuiveretijd holds Venira's corrected time,
        // which we deliberately ignore and recompute from Klok.DB.
        ArrivalDate: record.GetDate("Constateringsdatum"),
        ArrivalTime: record.GetTime("Constateringstijd"));
}
