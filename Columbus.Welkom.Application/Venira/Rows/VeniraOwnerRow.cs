using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>Liefhbr.DB</c> (liefhebbers, i.e. members).
/// </summary>
public sealed record VeniraOwnerRow(
    string? MemberNumber,
    string? ClubNumber,
    string? Name,
    string? Latitude,
    string? Longitude)
{
    public static VeniraOwnerRow FromRecord(ParadoxRecord record) => new(
        MemberNumber: record.GetText("NPOlidnr"),
        ClubNumber: record.GetText("NPOvernr"),
        Name: record.GetText("Naam"),
        // Xcoordinaat/Ycoordinaat are unused (always 0); the real position lives in these
        // degree-minute-second strings.
        Latitude: record.GetText("Latitude"),
        Longitude: record.GetText("Longitude"));
}
