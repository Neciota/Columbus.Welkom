using Columbus.Models.Pigeon;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Columbus.Welkom.Application.Database.ValueConverters;

internal class CountryCodeConverter : ValueConverter<CountryCode, string>
{
    public CountryCodeConverter() : base(v => v.Value, v => CountryCode.Create(v))
    {
        
    }
}
