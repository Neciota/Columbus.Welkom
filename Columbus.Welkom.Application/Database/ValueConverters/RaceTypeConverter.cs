using Columbus.Models.Race;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Columbus.Welkom.Application.Database.ValueConverters;

internal class RaceTypeConverter : ValueConverter<RaceType, int>
{
    public RaceTypeConverter() : base(v => v.Value, v => RaceType.Create((char)v))
    {
        
    }
}
