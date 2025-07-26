using Columbus.Models.Owner;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Columbus.Welkom.Application.Database.ValueConverters;

internal class OwnerIdConverter : ValueConverter<OwnerId, int>
{
    public OwnerIdConverter() : base(v => v.Value, v => OwnerId.Create(v))
    {
        
    }
}
