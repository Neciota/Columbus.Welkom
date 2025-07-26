using Columbus.Welkom.Application.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Columbus.Welkom.Application.Database;
internal class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlite("Data Source=temp.db");

        IOptions<AppSettings> appSettings = Options.Create(new AppSettings());
        return new DataContext(optionsBuilder.Options, appSettings);
    }
}
