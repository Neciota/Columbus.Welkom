using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace Columbus.Welkom.Application.Services;

public class SettingService : ISettingService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly SettingsProvider _settingsProvider;
    private readonly IFilePicker _filePicker;

    public SettingService(IOptions<AppSettings> appSettings, SettingsProvider settingsProvider, IFilePicker filePicker)
    {
        _appSettings = appSettings;
        _settingsProvider = settingsProvider;
        _filePicker = filePicker;
    }

    public string GetDatabasePath() => _appSettings.Value.GetDatabasePath();

    public Task<RaceSettings> GetSettingsAsync()
    {
        return _settingsProvider.GetSettingsAsync();
    }

    public Task SaveSettingsAsync(RaceSettings settings)
    {
        return _settingsProvider.SaveSettingsAsync(settings);
    }

    public Task SetClubAsync(int club)
    {
        _appSettings.Value.Club = club;

        return Task.CompletedTask;
    }

    public Task SetYearAsync(int year)
    {
        _appSettings.Value.Year = year;

        return Task.CompletedTask;
    }

    public async Task ExportAsync()
    {
        string databasePath = GetDatabasePath();

        using MemoryStream fileStream = new();
        using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Create);
        zipArchive.CreateEntryFromFile(databasePath, Path.GetFileName(databasePath));

        await _filePicker.SaveFileAsync($"welkom_database_{_appSettings.Value.Club}_{_appSettings.Value.Year}.zip", fileStream);
    }

    public async Task ImportAsync()
    {
        string? zipFilePath = await _filePicker.PickFileAsync([".zip"]);
        if (zipFilePath is null)
            return;

        using FileStream fileStream = new(zipFilePath, FileMode.Open);
        using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read);
        zipArchive.ExtractToDirectory(_appSettings.Value.AppDirectory, true);
    }
}
