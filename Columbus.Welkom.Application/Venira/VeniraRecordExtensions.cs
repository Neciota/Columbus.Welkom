using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Typed accessors over <see cref="ParadoxRecord"/>.
/// </summary>
/// <remarks>
/// <see cref="ParadoxRecord.GetValueOrDefault{T}(string)"/> cannot distinguish a NULL from a
/// zero for value types, and <see cref="ParadoxRecord.GetValue{T}(string)"/> is a strict
/// unbox that throws on NULL. Paradox columns are nullable throughout Venira's schema, so
/// every read goes through a nullable cast instead. Indexing by name throws when the column
/// does not exist, which is what we want if Venira ever changes its schema.
/// </remarks>
internal static class VeniraRecordExtensions
{
    /// <summary>Reads an Alpha column.</summary>
    public static string? GetText(this ParadoxRecord record, string name) => record[name] as string;

    /// <summary>Reads a Short column.</summary>
    public static short? GetShort(this ParadoxRecord record, string name) => record[name] as short?;

    /// <summary>Reads a Long or AutoInc column.</summary>
    public static int? GetInt(this ParadoxRecord record, string name) => record[name] as int?;

    /// <summary>Reads a Logical column.</summary>
    public static bool? GetBool(this ParadoxRecord record, string name) => record[name] as bool?;

    /// <summary>Reads a Date column. The time component is always midnight.</summary>
    public static DateTime? GetDate(this ParadoxRecord record, string name) => record[name] as DateTime?;

    /// <summary>Reads a Time column.</summary>
    public static TimeSpan? GetTime(this ParadoxRecord record, string name) => record[name] as TimeSpan?;
}
