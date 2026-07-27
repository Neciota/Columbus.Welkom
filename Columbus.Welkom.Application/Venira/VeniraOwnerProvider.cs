using Columbus.Models;
using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Venira.Rows;
using System.Globalization;

namespace Columbus.Welkom.Application.Venira;

/// <inheritdoc cref="IVeniraOwnerProvider"/>
public class VeniraOwnerProvider(IVeniraReader reader) : IVeniraOwnerProvider
{
    public async Task<IEnumerable<Owner>> GetOwnersAsync(ClubId club)
    {
        IReadOnlyList<VeniraOwnerRow> ownerRows = await reader.GetOwnersAsync();
        IReadOnlyList<VeniraPigeonRow> pigeonRows = await reader.GetPigeonsAsync();

        ILookup<OwnerId, Pigeon> pigeonsByOwner = pigeonRows.Select(ToOwnedPigeon)
            .OfType<(OwnerId Owner, Pigeon Pigeon)>()
            .Where(op => op.Owner.HasClubId(club))
            .ToLookup(op => op.Owner, op => op.Pigeon);

        return ownerRows.Select(row => ToOwner(row, pigeonsByOwner))
            .OfType<Owner>()
            .Where(o => o.Id.HasClubId(club))
            .ToList();
    }

    private static Owner? ToOwner(VeniraOwnerRow row, ILookup<OwnerId, Pigeon> pigeonsByOwner)
    {
        if (!OwnerId.TryParse(row.MemberNumber, CultureInfo.InvariantCulture, out OwnerId id))
            return null;

        if (!ClubId.TryParse(row.ClubNumber, CultureInfo.InvariantCulture, out ClubId club))
            return null;

        return new Owner(
            id,
            row.Name ?? string.Empty,
            VeniraValues.ParseCoordinate(row.Latitude, row.Longitude),
            club,
            [.. pigeonsByOwner[id]]);
    }

    private static (OwnerId Owner, Pigeon Pigeon)? ToOwnedPigeon(VeniraPigeonRow row)
    {
        if (!OwnerId.TryParse(row.MemberNumber, CultureInfo.InvariantCulture, out OwnerId owner))
            return null;

        if (VeniraValues.ParsePigeonId(row.CountryCode, row.RingNumber) is not PigeonId pigeonId)
            return null;

        return (owner, new Pigeon(pigeonId, VeniraValues.ParseChip(row.Chip), VeniraValues.ParseSex(row.Sex)));
    }
}
