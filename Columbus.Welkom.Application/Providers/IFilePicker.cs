using System.Text.RegularExpressions;

namespace Columbus.Welkom.Application.Providers;

public interface IFilePicker
{
    Task<(StreamReader? FileStream, string FileName)> OpenFileAsync(string[] fileTypes);
    Task<IEnumerable<(StreamReader FileStream, string FileName)>> OpenFilesAsync(string[] fileTypes, Regex? nameMustMatch = null);
    Task<string?> PickFileAsync(string[] fileTypes);
    Task<IEnumerable<string>> PickFilesAsync(string[] fileTypes);
    Task SaveFileAsync(string name, Stream stream, CancellationToken cancellationToken = default);
}
