using Columbus.Welkom.Application.Models.ViewModels;
using System.Text.Json;

namespace Columbus.Welkom.Application.Providers;

public class SettingsProvider
{
    private readonly string _appFolder;
    private const string SettingsFileName = "settings.json";

    private JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    private RaceSettings? _settings;

    public SettingsProvider(string appFolder)
    {
        _appFolder = appFolder;
    }

    public async Task<RaceSettings> GetSettingsAsync()
    {
        if (_settings is not null)
            return _settings;

        using FileStream fileStream = new(GetSettingsPath(), FileMode.OpenOrCreate, FileAccess.Read);

        try
        {
            RaceSettings? settings = await JsonSerializer.DeserializeAsync<RaceSettings>(fileStream, _serializerOptions);
            return settings ?? new RaceSettings();
        }
        catch (JsonException)
        {
            return new RaceSettings();
        }
    }

    public async Task SaveSettingsAsync(RaceSettings settings)
    {
        _settings = settings;

        using FileStream fileStream = new(GetSettingsPath(), FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(fileStream, settings, _serializerOptions);
    }

    private string GetSettingsPath() => Path.Combine(_appFolder, SettingsFileName);
}
