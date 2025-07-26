using Columbus.Models.Pigeon;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Columbus.Welkom.Application.Database.ValueConverters;

internal class RingNumberConverter : ValueConverter<RingNumber, int>
{
    public RingNumberConverter() : base(v => v.Value, v => RingNumber.Create(v))
    {
        
    }
}
