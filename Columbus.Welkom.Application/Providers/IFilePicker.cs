using System.Text.RegularExpressions;

namespace Columbus.Welkom.Application.Providers;

public interface IFilePicker
{
    Task<StreamReader?> OpenFileAsync(string[] fileTypes);
    Task<IEnumerable<StreamReader>> OpenFilesAsync(string[] fileTypes, Regex? nameMustMatch = null);
    Task<string?> PickFileAsync(string[] fileTypes);
    Task<IEnumerable<string>> PickFilesAsync(string[] fileTypes);
}
