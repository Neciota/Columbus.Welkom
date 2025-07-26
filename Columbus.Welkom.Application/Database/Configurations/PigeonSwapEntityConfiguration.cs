using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class PigeonSwapEntityConfiguration : IEntityTypeConfiguration<PigeonSwapEntity>
{
    public void Configure(EntityTypeBuilder<PigeonSwapEntity> builder)
    {
        builder.ToTable("pigeon_swap");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");

        builder.Property(e => e.PlayerId)
            .HasColumnName("player_id");

        builder.Property(e => e.CoupledPlayerId)
            .HasColumnName("coupled_player_id");

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
