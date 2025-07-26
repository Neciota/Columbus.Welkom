using Columbus.Welkom.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Columbus.Welkom.Application.Database.Configurations;

public class OwnerTeamEntityConfiguration : IEntityTypeConfiguration<OwnerTeamEntity>
{
    public void Configure(EntityTypeBuilder<OwnerTeamEntity> builder)
    {
        builder.ToTable("owner_team");

        builder.HasKey(e => new { e.OwnerId, e.TeamNumber });

        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");

        builder.Property(e => e.TeamNumber)
            .HasColumnName("team_number");

        builder.Property(e => e.Position)
            .HasColumnName("position");
    }
}
