using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>DUIF.DB</c> (the loft list).
/// </summary>
public sealed record VeniraPigeonRow(
    string? MemberNumber,
    string? ClubNumber,
    string? RingNumber,
    string? CountryCode,
    string? Sex,
    string? Chip)
{
    public static VeniraPigeonRow FromRecord(ParadoxRecord record) => new(
        MemberNumber: record.GetText("NPOlidnr"),
        ClubNumber: record.GetText("NPOvernr"),
        // Ringnr already carries the two-digit year, so the separate Jaar column is redundant.
        RingNumber: record.GetText("Ringnr"),
        CountryCode: record.GetText("Landcode"),
        Sex: record.GetText("Geslacht"),
        Chip: record.GetText("Chipring"));
}
