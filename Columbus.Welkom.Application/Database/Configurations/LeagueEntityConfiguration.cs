using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class LeagueEntityConfiguration : IEntityTypeConfiguration<LeagueEntity>
{
    public void Configure(EntityTypeBuilder<LeagueEntity> builder)
    {
        builder.ToTable("league");

        builder.HasKey(e => e.Rank);

        builder.Property(e => e.Rank)
            .HasColumnName("rank");

        builder.HasMany(e => e.LeagueOwners)
            .WithOne(e => e.League)
            .HasForeignKey(e => e.LeagueRank);
    }
}
