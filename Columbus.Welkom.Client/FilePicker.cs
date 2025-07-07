using IFilePicker = Columbus.Welkom.Application.Providers.IFilePicker;

namespace Columbus.Welkom.Client;

public class FilePicker : IFilePicker
{
    public async Task<string?> PickFileAsync(string[] fileTypes)
    {
        FileResult? fileResult = await GetFileAsync(fileTypes);

        return fileResult?.FullPath;
    }

    public async Task<Stream?> OpenFileAsync(string[] fileTypes)
    {
        FileResult? fileResult = await GetFileAsync(fileTypes);
        if (fileResult is null)
            return null;

        return await fileResult.OpenReadAsync();
    }

    private static async Task<FileResult?> GetFileAsync(string[] fileTypes)
    {
        Dictionary<DevicePlatform, IEnumerable<string>> fileTypesByDevice = new()
        {
            { DevicePlatform.WinUI, fileTypes }
        };

        PickOptions options = new()
        {
            FileTypes = new FilePickerFileType(fileTypesByDevice)
        };

        return await Microsoft.Maui.Storage.FilePicker.PickAsync(options);
    }
}
