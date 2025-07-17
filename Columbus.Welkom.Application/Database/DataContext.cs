using Columbus.Models;
using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Database.ValueConverters;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Database;
public class DataContext : DbContext
{
    private readonly ISettingService _settingService;

    public DataContext(DbContextOptions<DataContext> options, ISettingService settingService) : base(options)
    {
        _settingService = settingService;
    }

    public DbSet<PigeonEntity> Pigeons { get; set; }
    public DbSet<OwnerEntity> Owners { get; set; }
    public DbSet<RaceEntity> Races { get; set; }
    public DbSet<PigeonRaceEntity> PigeonRaces { get; set; }
    public DbSet<SelectedYoungPigeonEntity> SelectedYoungPigeons { get; set; }
    public DbSet<SelectedYearPigeonEntity> SelectedYearPigeons { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }
    public DbSet<LeagueOwnerEntity> LeagueOwners { get; set; }
    public DbSet<OwnerTeamEntity> OwnerTeams { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<PigeonSwapEntity> PigeonSwaps { get; set; }
    public DbSet<PigeonSaleEntity> PigeonSales { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<ClubId>()
            .HaveConversion<ClubIdConverter>();
        configurationBuilder.Properties<OwnerId>()
            .HaveConversion<OwnerIdConverter>();
        configurationBuilder.Properties<RingNumber>()
            .HaveConversion<RingNumberConverter>();
        configurationBuilder.Properties<CountryCode>()
            .HaveConversion<CountryCodeConverter>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string connectionString = $"Data Source={Path.Combine(_settingService.AppDirectory, $"database_{_settingService.Club}_{_settingService.Year}.db")}";
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
