using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class LeagueOwnerConfiguration : IEntityTypeConfiguration<LeagueOwnerEntity>
{
    public void Configure(EntityTypeBuilder<LeagueOwnerEntity> builder)
    {
        builder.ToTable("league_owner");

        builder.HasKey(e => new { e.OwnerId, e.LeagueRank });

        builder.Property(e => e.LeagueRank)
            .HasColumnName("league_rank");
        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");
    }
}
