using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class PigeonRaceEntityConfiguration : IEntityTypeConfiguration<PigeonRaceEntity>
{
    public void Configure(EntityTypeBuilder<PigeonRaceEntity> builder)
    {
        builder.ToTable("pigeon_race");

        builder.HasKey([nameof(PigeonRaceEntity.RaceCode), .. PigeonEntityConfiguration.KeyPropertyNames]);

        builder.Property(e => e.RaceCode)
            .HasColumnName("race_code");

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

        builder.Property(e => e.Mark)
            .HasColumnName("mark");

        builder.Property(e => e.ArrivalOrder)
            .HasColumnName("arrival_order");

        builder.Property(e => e.ArrivalTime)
            .HasColumnName("arrival_time");
    }
}
