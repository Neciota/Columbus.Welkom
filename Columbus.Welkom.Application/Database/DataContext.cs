using Columbus.Models;
using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Database.ValueConverters;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Database;
public class DataContext : DbContext
{
    private readonly IOptions<AppSettings> _settings;

    public DataContext(DbContextOptions<DataContext> options, IOptions<AppSettings> settings) : base(options)
    {
        _settings = settings;
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
    public DbSet<PigeonSaleClassEntity> PigeonSaleClasses { get; set; }

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
        configurationBuilder.Properties<RaceType>()
            .HaveConversion<RaceTypeConverter>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string connectionString = $"Data Source={_settings.Value.GetDatabasePath()}";
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
