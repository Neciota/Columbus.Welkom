using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class PigeonSaleClassConfiguration : IEntityTypeConfiguration<PigeonSaleClassEntity>
{
    public void Configure(EntityTypeBuilder<PigeonSaleClassEntity> builder)
    {
        builder.ToTable("pigeon_sale_class");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.HasMany(e => e.PigeonSales)
            .WithOne(e => e.Class)
            .HasForeignKey(e => e.ClassId);
    }
}
