using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels;

/// <summary>
/// The leading letter of a Venira flight code, e.g. <c>L</c> of <c>L32</c>, as found in the
/// stored races, together with the race types of the flights that carry it.
/// </summary>
/// <remarks>
/// The letter groups flights more finely than <see cref="RaceType"/> does: the young-pigeon
/// flights of one season are race type <c>J</c> throughout, but coded <c>J</c> up to the natour
/// and <c>L</c> from the natour onwards. It is a Venira convention rather than a schema, so the
/// available letters are read from the races that were imported rather than enumerated up front,
/// and <see cref="RaceTypes"/> is a collection because nothing guarantees a letter is used for a
/// single race type.
/// </remarks>
public sealed record RaceCodeLetter(string Letter, IReadOnlyCollection<RaceType> RaceTypes)
{
    /// <summary>
    /// The letter with the race types it covers, e.g. <c>L (J)</c>. The race type is worth
    /// showing because the two rarely-matching letters are easy to mix up: code <c>A</c> is race
    /// type <c>O</c>, while race type <c>A</c> is coded <c>Z</c>.
    /// </summary>
    public string Label => RaceTypes.Count == 0
        ? Letter
        : $"{Letter} ({string.Join(", ", RaceTypes)})";
}
