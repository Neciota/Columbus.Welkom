using Columbus.Models;
using Columbus.Models.Pigeon;
using System.Globalization;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Converts Venira's stringly-typed columns into <c>Columbus.Models</c> value objects.
/// </summary>
/// <remarks>
/// Every conversion is lenient: Venira's tables contain rows with missing coordinates, blank
/// sexes and non-ISO country codes, and a single bad row must not abort a whole import. The
/// old .UDP path was equally lenient about the same fields.
/// </remarks>
internal static class VeniraValues
{
    /// <summary>Length of one of Venira's DMS coordinate components, e.g. <c>" 514113.70"</c>.</summary>
    private const int DmsComponentLength = 10;

    /// <summary>Length of Venira's <c>Ringnr</c>: a two-digit year plus a seven-digit ring number.</summary>
    private const int RingNumberLength = 9;

    private const int YearLength = 2;

    /// <summary>
    /// Combines Venira's separate latitude and longitude columns into a <see cref="Coordinate"/>.
    /// Both are degree-minute-second strings without separators, e.g. <c>" 514113.70"</c> for
    /// 51°41'13.70", which is the same encoding the .UDP files used.
    /// </summary>
    /// <returns>
    /// The coordinate, or <see langword="default"/> when either component is absent or
    /// malformed. Venira leaves the coordinates blank for members who never race, which then
    /// yields a distance of zero — the same result the .UDP import produced.
    /// </returns>
    public static Coordinate ParseCoordinate(string? latitude, string? longitude)
    {
        if (latitude?.Length != DmsComponentLength || longitude?.Length != DmsComponentLength)
            return default;

        // Coordinate.TryParseFromDms expects the two components concatenated into one span.
        return Coordinate.TryParseFromDms(string.Concat(latitude, longitude), CultureInfo.InvariantCulture, out Coordinate coordinate)
            ? coordinate
            : default;
    }

    /// <summary>
    /// Builds a <see cref="PigeonId"/> from Venira's country code and <c>Ringnr</c>, which
    /// prefixes the seven-digit ring number with the pigeon's two-digit birth year.
    /// </summary>
    /// <returns>The id, or <see langword="null"/> when the row cannot be interpreted.</returns>
    public static PigeonId? ParsePigeonId(string? countryCode, string? ringNumber)
    {
        if (ringNumber?.Length != RingNumberLength)
            return null;

        if (!CountryCode.TryParse(countryCode, CultureInfo.InvariantCulture, out CountryCode country))
            return null;

        if (!int.TryParse(ringNumber.AsSpan(0, YearLength), NumberStyles.None, CultureInfo.InvariantCulture, out int year))
            return null;

        if (!RingNumber.TryParse(ringNumber.AsSpan(YearLength), CultureInfo.InvariantCulture, out RingNumber ring))
            return null;

        return PigeonId.Create(country, year, ring);
    }

    /// <summary>
    /// Reads Venira's <c>Geslacht</c>. Unlike <see cref="SexExtensions.Parse"/> this never
    /// throws: the column is blank for pigeons whose sex was never recorded.
    /// </summary>
    public static Sex ParseSex(string? sex) => sex switch
    {
        "M" => Sex.Male,
        "V" => Sex.Female,
        _ => Sex.Unknown,
    };

    /// <summary>
    /// Reads Venira's <c>Chipring</c>, eight hexadecimal digits. Values above
    /// <see cref="int.MaxValue"/> wrap to negative, matching how the .UDP parser stored them
    /// and therefore matching the chips already in the database.
    /// </summary>
    public static int ParseChip(string? chip) =>
        int.TryParse(chip, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value) ? value : 0;

    /// <summary>
    /// Combines a Paradox Date and Time column into a single local <see cref="DateTime"/>.
    /// </summary>
    public static DateTime? Combine(DateTime? date, TimeSpan? time)
    {
        if (date is null)
            return null;

        return DateTime.SpecifyKind(date.Value.Add(time ?? TimeSpan.Zero), DateTimeKind.Local);
    }
}
