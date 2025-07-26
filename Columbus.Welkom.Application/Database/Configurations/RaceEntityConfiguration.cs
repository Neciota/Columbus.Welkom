using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class RaceEntityConfiguration : IEntityTypeConfiguration<RaceEntity>
{
    public void Configure(EntityTypeBuilder<RaceEntity> builder)
    {
        builder.ToTable("race");

        builder.HasKey(e => e.Code);

        builder.HasIndex(e => e.Type);

        builder.HasIndex(e => e.Number);

        builder.Property(e => e.Code)
            .HasColumnName("code");

        builder.Property(e => e.Number)
            .HasColumnName("number");

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.Type)
            .HasColumnName("type");

        builder.Property(e => e.StartTime)
            .HasColumnName("start_time");

        builder.Property(e => e.Latitude)
            .HasColumnName("latitude");

        builder.Property(e => e.Longitude)
            .HasColumnName("longitude");

        builder.HasMany(e => e.PigeonRaces)
            .WithOne(e => e.Race)
            .HasForeignKey(e => e.RaceCode);
    }
}
