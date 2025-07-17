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
        public int Year => throw new NotImplementedException();

        public int Club => throw new NotImplementedException();

        public string AppDirectory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task SetClubAsync(int club)
        {
            throw new NotImplementedException();
        }

        public Task SetYearAsync(int year)
        {
            throw new NotImplementedException();
        }
    }
}
