using Columbus.UDP;
using Columbus.UDP.Interfaces;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Repositories;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services;
using Columbus.Welkom.Application.Services.Interfaces;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
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
                .UseMauiCommunityToolkitCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Columbus",
                "Welkom");

            QuestPDF.Settings.License = LicenseType.Community;

            Directory.CreateDirectory(appFolder);

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddRadzenComponents();

            builder.Services.AddOptions<IOptions<AppSettings>>();
            builder.Services.AddSingleton<ISettingService, SettingService>();

            builder.Services.AddTransient<ILeaguesService, LeaguesService>();
            builder.Services.AddTransient<IOwnerService, OwnerService>();
            builder.Services.AddTransient<IPigeonSwapService, PigeonSwapService>();
            builder.Services.AddTransient<IPigeonSaleService, PigeonSaleService>();
            builder.Services.AddTransient<IRaceService, RaceService>();
            builder.Services.AddTransient<ISelectedYearPigeonService, SelectedYearPigeonService>();
            builder.Services.AddTransient<ISelectedYoungPigeonService, SelectedYoungPigeonService>();
            builder.Services.AddTransient<ITeamsService, TeamsService>();

            builder.Services.AddTransient<ILeagueRepository, LeagueRepository>();
            builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
            builder.Services.AddTransient<IPigeonRaceRepository, PigeonRaceRepository>();
            builder.Services.AddTransient<IPigeonRepository, PigeonRepository>();
            builder.Services.AddTransient<IPigeonSwapRepository, PigeonSwapRepository>();
            builder.Services.AddTransient<IPigeonSaleRepository, PigeonSaleRepository>();
            builder.Services.AddTransient<IRaceRepository, RaceRepository>();
            builder.Services.AddTransient<ISelectedYearPigeonRepository, SelectedYearPigeonRepository>();
            builder.Services.AddTransient<ISelectedYoungPigeonRepository, SelectedYoungPigeonRepository>();
            builder.Services.AddTransient<ITeamsRepository, TeamsRepository>();

            builder.Services.AddTransient<Application.Providers.IFilePicker, FilePicker>();
            builder.Services.AddTransient(b => new Application.Providers.SettingsProvider(appFolder));

            builder.Services.AddTransient<IOwnerSerializer, OwnerSerializer>();
            builder.Services.AddTransient<IPigeonSerializer, PigeonSerializer>();
            builder.Services.AddTransient<IRaceSerializer, RaceSerializer>();
            
            builder.Services.AddDbContext<DataContext>(ServiceLifetime.Transient);

            builder.Services.AddSingleton<ILogger>(builder => new DebugLoggerProvider().CreateLogger("debug logger"));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var appSettings = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
                appSettings.Value.AppDirectory = appFolder;
            }

            return app;
        }
    }
}
