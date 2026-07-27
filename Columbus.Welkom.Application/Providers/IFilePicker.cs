namespace Columbus.Welkom.Application.Providers;

public interface IFilePicker
{
    Task<string?> PickFileAsync(string[] fileTypes);
    Task<IEnumerable<string>> PickFilesAsync(string[] fileTypes);
    Task SaveFileAsync(string name, Stream stream, CancellationToken cancellationToken = default);
}
