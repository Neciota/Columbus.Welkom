using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class SelectedYoungPigeonEntityConfiguration : IEntityTypeConfiguration<SelectedYoungPigeonEntity>
{
    public void Configure(EntityTypeBuilder<SelectedYoungPigeonEntity> builder)
    {
        builder.ToTable("selected_young_pigeon");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");

        builder.ComplexProperty(e => e.PigeonId, id =>
        {
            id.Property(p => p.CountryCode)
                .HasColumnName("country_code");
            id.Property(p => p.Year)
                .HasColumnName("year");
            id.Property(p => p.RingNumber)
                .HasColumnName("ring_number");
        });

        builder.Property("_countryCode")
            .HasColumnName("country_code");

        builder.Property("_year")
            .HasColumnName("year");

        builder.Property("_ringNumber")
            .HasColumnName("ring_number");
    }
}
