using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Columbus.Welkom.Application.Database.Configurations;

public class OwnerEntityConfiguration : IEntityTypeConfiguration<OwnerEntity>
{
    public void Configure(EntityTypeBuilder<OwnerEntity> builder)
    {
        builder.ToTable("owner");

        builder.HasKey(e => e.OwnerId);

        builder.HasIndex(e => e.Club);

        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.Latitude)
            .HasColumnName("latitude");

        builder.Property(e => e.Longitude)
            .HasColumnName("longitude");

        builder.Property(e => e.Club)
            .HasColumnName("club_id");

        builder.HasMany(e => e.Pigeons)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId);

        builder.HasOne(e => e.LeagueOwner)
            .WithOne(e => e.Owner)
            .HasForeignKey<LeagueOwnerEntity>(e => e.OwnerId);

        builder.HasOne(e => e.OwnerTeam)
            .WithOne(e => e.Owner)
            .HasForeignKey<OwnerTeamEntity>(e => e.OwnerId);

        builder.HasOne(e => e.SelectedYearPigeon)
            .WithOne(e => e.Owner)
            .HasForeignKey<SelectedYearPigeonEntity>(e => e.OwnerId);

        builder.HasOne(e => e.SelectedYoungPigeon)
            .WithOne(e => e.Owner)
            .HasForeignKey<SelectedYoungPigeonEntity>(e => e.OwnerId);

        builder.HasOne(e => e.PigeonSale)
            .WithOne(e => e.Seller)
            .HasForeignKey<PigeonSaleEntity>(e => e.SellerId);

        builder.HasOne(e => e.PigeonBuy)
            .WithOne(e => e.Buyer)
            .HasForeignKey<PigeonSaleEntity>(e => e.BuyerId);

        builder.HasMany(e => e.PlayedSwappedPigeons)
            .WithOne(e => e.Player)
            .HasForeignKey(e => e.PlayerId);

        builder.HasMany(e => e.OwnedSwappedPigeons)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId);

        builder.HasMany(e => e.CoupledSwappedPigeons)
            .WithOne(e => e.CoupledPlayer)
            .HasForeignKey(e => e.CoupledPlayerId);
    }
}
