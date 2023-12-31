using Blazored.LocalStorage;
using Columbus.Welkom.Client;
using Columbus.Welkom.Client.Repositories.Interfaces;
using Columbus.Welkom.Client.Repositories;
using Columbus.Welkom.Client.Services;
using Columbus.Welkom.Client.Services.Interfaces;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SqliteWasmHelper;
using Radzen;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Packages
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddFileSystemAccessService();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// Logging
#if DEBUG
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
#else
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
#endif

// DataContext
builder.Services.AddSqliteWasmDbContextFactory<DataContext>(opts => opts.UseSqlite("Data Source=welkom.sqlite3"));

// Repositories
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IPigeonRepository, PigeonRepository>();
builder.Services.AddScoped<IPigeonRaceRepository, PigeonRaceRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<ISelectedYearPigeonRepository, SelectedYearPigeonRepository>();
builder.Services.AddScoped<ISelectedYoungPigeonRepository, SelectedYoungPigeonRepository>();
builder.Services.AddScoped<IPigeonSwapRepository, PigeonSwapRepository>();

//Services
builder.Services.AddScoped<ILeaguesService, LeaguesService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IRaceService, RaceService>();
builder.Services.AddScoped<ISelectedYearPigeonService, SelectedYearPigeonService>();
builder.Services.AddScoped<ISelectedYoungPigeonService, SelectedYoungPigeonService>();
builder.Services.AddScoped<IPigeonSwapService, PigeonSwapService>();

await builder.Build().RunAsync();
