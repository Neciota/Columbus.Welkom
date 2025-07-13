using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class PigeonSaleConfiguration : IEntityTypeConfiguration<PigeonSaleEntity>
{
    public void Configure(EntityTypeBuilder<PigeonSaleEntity> builder)
    {
        builder.ToTable("pigeon_sale");

        builder.HasKey(e => e.Id);

        builder.ComplexProperty(e => e.PigeonId, pid =>
        {
            pid.Property(p => p.CountryCode)
                .HasColumnName("country_code");
            pid.Property(p => p.Year)
                .HasColumnName("year");
            pid.Property(p => p.RingNumber)
                .HasColumnName("ring_number");
        });

        builder.Property("_countryCode")
            .HasColumnName("country_code");

        builder.Property("_year")
            .HasColumnName("year");

        builder.Property("_ringNumber")
            .HasColumnName("ring_number");

        builder.Property(e => e.SellerId)
            .HasColumnName("seller_id");

        builder.Property(e => e.BuyerId)
            .HasColumnName("buyer_id");
    }
}
