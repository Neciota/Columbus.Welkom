using Columbus.UDP;
using Columbus.UDP.Interfaces;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services;
using Columbus.Welkom.Application.Services.Interfaces;
using Columbus.Welkom.Client.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Radzen;

namespace Columbus.Welkom.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            if (!OperatingSystem.IsWindows())
                throw new InvalidOperationException("This app can only run on Windows.");

            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Columbus",
                "Welkom");

            Directory.CreateDirectory(appFolder);

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddRadzenComponents();

            builder.Services.AddSingleton<ISettingService, SettingService>();

            builder.Services.AddTransient<ILeaguesService, LeaguesService>();
            builder.Services.AddTransient<IOwnerService, OwnerService>();
            builder.Services.AddTransient<IPigeonSwapService, PigeonSwapService>();
            builder.Services.AddTransient<IRaceService, RaceService>();
            builder.Services.AddTransient<ISelectedYearPigeonService, SelectedYearPigeonService>();
            builder.Services.AddTransient<ISelectedYoungPigeonService, SelectedYoungPigeonService>();

            builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
            builder.Services.AddTransient<IPigeonRaceRepository, PigeonRaceRepository>();
            builder.Services.AddTransient<IPigeonRepository, PigeonRepository>();
            builder.Services.AddTransient<IPigeonSwapRepository, PigeonSwapRepository>();
            builder.Services.AddTransient<IRaceRepository, RaceRepository>();
            builder.Services.AddTransient<ISelectedYearPigeonRepository, SelectedYearPigeonRepository>();
            builder.Services.AddTransient<ISelectedYoungPigeonRepository, SelectedYoungPigeonRepository>();

            builder.Services.AddTransient<Application.Providers.IFileProvider, SystemFileProvider>();

            builder.Services.AddTransient<IOwnerSerializer, OwnerSerializer>();
            builder.Services.AddTransient<IPigeonSerializer, PigeonSerializer>();
            builder.Services.AddTransient<IRaceSerializer, RaceSerializer>();

            //string connectionString = builder.Configuration.GetConnectionString() ?? throw new InvalidOperationException("No connection string has been set.");
            string connectionString = $"Data Source={Path.Combine(appFolder, "database.db")}";
            builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Apply pending EF Core migrations
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureCreated();
                //db.Database.Migrate();
            }

            return app;
        }
    }
}
