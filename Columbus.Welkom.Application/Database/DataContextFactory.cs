using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Columbus.Welkom.Application.Database;
internal class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlite("Data Source=temp.db");

        return new DataContext(optionsBuilder.Options, new DesignSettings());
    }

    internal class DesignSettings : ISettingService
    {
        public int Year => 0;

        public int Club => 0;

        public string AppDirectory { get => string.Empty; set => throw new NotImplementedException(); }

        public Task<Settings> GetSettingsAsync() => Task.FromResult(new Settings());

        public Task SaveSettingsAsync(Settings settings) => Task.CompletedTask;

        public Task SetClubAsync(int club) => Task.CompletedTask;

        public Task SetYearAsync(int year) => Task.CompletedTask;
    }
}
