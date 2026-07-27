using CommunityToolkit.Maui.Storage;
using IFilePicker = Columbus.Welkom.Application.Providers.IFilePicker;

namespace Columbus.Welkom.Client;

public class FilePicker : IFilePicker
{
    public async Task<string?> PickFileAsync(string[] fileTypes)
    {
        FileResult? fileResult = await GetFileAsync(fileTypes);

        return fileResult?.FullPath;
    }

    public async Task<IEnumerable<string>> PickFilesAsync(string[] fileTypes)
    {
        IEnumerable<FileResult> fileResults = await GetFilesAsync(fileTypes);

        return fileResults.Select(f => f.FullPath);
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

    private async Task<IEnumerable<FileResult>> GetFilesAsync(string[] fileTypes)
    {
        Dictionary<DevicePlatform, IEnumerable<string>> fileTypesByDevice = new()
        {
            { DevicePlatform.WinUI, fileTypes }
        };

        PickOptions options = new()
        {
            FileTypes = new FilePickerFileType(fileTypesByDevice)
        };

        return await Microsoft.Maui.Storage.FilePicker.PickMultipleAsync(options);
    }

    public async Task SaveFileAsync(string name, Stream stream, CancellationToken cancellationToken = default)
    {
        IFileSaver fileSaver = FileSaver.Default;
        await fileSaver.SaveAsync(name, stream, cancellationToken);
    }
}
