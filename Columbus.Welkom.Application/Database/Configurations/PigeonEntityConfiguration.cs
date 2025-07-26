using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class PigeonEntityConfiguration : IEntityTypeConfiguration<PigeonEntity>
{
    public static readonly string[] KeyPropertyNames = ["_countryCode", "_year", "_ringNumber"];

    public void Configure(EntityTypeBuilder<PigeonEntity> builder)
    {
        builder.ToTable("pigeon");

        builder.HasKey(KeyPropertyNames);

        builder.ComplexProperty(e => e.Id, id =>
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

        builder.Property(e => e.Chip)
            .HasColumnName("chip");

        builder.Property(e => e.Sex)
            .HasColumnName("sex");

        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");

        builder.HasOne(e => e.Owner)
            .WithMany(e => e.Pigeons)
            .HasForeignKey(e => e.OwnerId);

        builder.HasMany(e => e.PigeonRaces)
            .WithOne(e => e.Pigeon)
            .HasForeignKey(KeyPropertyNames);

        builder.HasOne(e => e.SelectedYearPigeonEntity)
            .WithOne(e => e.Pigeon)
            .HasForeignKey<SelectedYearPigeonEntity>(KeyPropertyNames);

        builder.HasOne(e => e.SelectedYoungPigeonEntity)
            .WithOne(e => e.Pigeon)
            .HasForeignKey<SelectedYoungPigeonEntity>(KeyPropertyNames);

        builder.HasOne(e => e.PigeonSale)
            .WithOne(e => e.Pigeon)
            .HasForeignKey<PigeonSaleEntity>(KeyPropertyNames);

        builder.HasMany(e => e.PigeonSwaps)
            .WithOne(e => e.Pigeon)
            .HasForeignKey(KeyPropertyNames);
    }
}
