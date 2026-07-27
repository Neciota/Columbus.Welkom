using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>Losplaat.DB</c> (release sites). Joined to a flight by name, since
/// <c>Vlucht.Lossingsplaats</c> stores the place name rather than a key.
/// </summary>
public sealed record VeniraReleaseSiteRow(
    string? Name,
    string? Latitude,
    string? Longitude)
{
    public static VeniraReleaseSiteRow FromRecord(ParadoxRecord record) => new(
        Name: record.GetText("Plaatsnaam"),
        Latitude: record.GetText("Latitude"),
        Longitude: record.GetText("Longitude"));
}
