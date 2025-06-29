using Columbus.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Columbus.Welkom.Application.Database.ValueConverters;

internal class ClubIdConverter : ValueConverter<ClubId, int>
{
    public ClubIdConverter() : base(v => v.Value, v => ClubId.Create(v))
    {
        
    }
}
