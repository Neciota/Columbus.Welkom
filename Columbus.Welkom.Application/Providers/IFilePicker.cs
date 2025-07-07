

namespace Columbus.Welkom.Application.Providers;

public interface IFilePicker
{
    Task<Stream?> OpenFileAsync(string[] fileTypes);
    Task<string?> PickFileAsync(string[] fileTypes);
}
