using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Venira.Rows;
using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira;

/// <inheritdoc cref="IVeniraReader"/>
public class VeniraReader(SettingsProvider settingsProvider) : IVeniraReader
{
    private const string OwnersTable = "Liefhbr.DB";
    private const string PigeonsTable = "DUIF.DB";
    private const string FlightsTable = "Vlucht.DB";
    private const string ArrivalsTable = "Aankomst.DB";
    private const string ClocksTable = "Klok.DB";
    private const string ReleaseSitesTable = "Losplaat.DB";
    private const string SolarPeriodsTable = "NEUTTIJD.DB";

    public Task<IReadOnlyList<VeniraOwnerRow>> GetOwnersAsync() =>
        ReadAsync(OwnersTable, VeniraOwnerRow.FromRecord);

    public Task<IReadOnlyList<VeniraPigeonRow>> GetPigeonsAsync() =>
        ReadAsync(PigeonsTable, VeniraPigeonRow.FromRecord);

    public Task<IReadOnlyList<VeniraFlightRow>> GetFlightsAsync() =>
        ReadAsync(FlightsTable, VeniraFlightRow.FromRecord);

    public Task<IReadOnlyList<VeniraArrivalRow>> GetArrivalsAsync() =>
        ReadAsync(ArrivalsTable, VeniraArrivalRow.FromRecord);

    public Task<IReadOnlyList<VeniraClockRow>> GetClocksAsync() =>
        ReadAsync(ClocksTable, VeniraClockRow.FromRecord);

    public Task<IReadOnlyList<VeniraReleaseSiteRow>> GetReleaseSitesAsync() =>
        ReadAsync(ReleaseSitesTable, VeniraReleaseSiteRow.FromRecord);

    public Task<IReadOnlyList<VeniraSolarPeriodRow>> GetSolarPeriodsAsync() =>
        ReadAsync(SolarPeriodsTable, VeniraSolarPeriodRow.FromRecord);

    private async Task<IReadOnlyList<T>> ReadAsync<T>(string table, Func<ParadoxRecord, T> map)
    {
        RaceSettings settings = await settingsProvider.GetSettingsAsync();
        string dataPath = settings.VeniraDataPath;

        if (string.IsNullOrWhiteSpace(dataPath))
            throw new VeniraException("Er is geen Venira-map ingesteld. Stel deze in bij Instellingen.");

        // Paradox reads are synchronous file I/O, so keep them off the UI thread.
        return await Task.Run(() => Read(dataPath, table, map));
    }

    private static IReadOnlyList<T> Read<T>(string dataPath, string table, Func<ParadoxRecord, T> map)
    {
        string path = Path.Combine(dataPath, table);

        try
        {
            // ParadoxTable holds a native read cursor and is not thread-safe, so it is opened
            // per read and never shared. Each ParadoxRecord copies its values into managed
            // memory, so the rows stay valid once the table is disposed.
            using ParadoxTable paradoxTable = ParadoxTable.Open(path);

            return [.. paradoxTable.Records.Select(map)];
        }
        catch (DllNotFoundException e)
        {
            throw new VeniraException(
                "De pxlib-bibliotheek kon niet worden geladen. Controleer of pxlib.dll naast de applicatie staat.", e);
        }
        catch (Exception e) when (e is ParadoxException or IOException or UnauthorizedAccessException or ArgumentException)
        {
            throw new VeniraException($"Venira-tabel '{table}' kon niet worden gelezen uit '{dataPath}'.", e);
        }
    }
}
