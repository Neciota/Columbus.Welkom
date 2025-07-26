using CommunityToolkit.Maui.Storage;
using System.Text;
using System.Text.RegularExpressions;
using IFilePicker = Columbus.Welkom.Application.Providers.IFilePicker;

namespace Columbus.Welkom.Client;

public class FilePicker : IFilePicker
{
    private const int FileBufferSize = 10_000_000;

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

    public async Task<(StreamReader?, string)> OpenFileAsync(string[] fileTypes)
    {
        FileResult? fileResult = await GetFileAsync(fileTypes);
        if (fileResult is null)
            return (null, string.Empty);

        Stream stream = await fileResult.OpenReadAsync();
        return (new StreamReader(stream, Encoding.Latin1, false, FileBufferSize), fileResult.FileName);
    }

    public async Task<IEnumerable<(StreamReader, string)>> OpenFilesAsync(string[] fileTypes, Regex? nameMustMatch = null)
    {
        IEnumerable<FileResult> fileResult = await GetFilesAsync(fileTypes);

        if (nameMustMatch is not null)
            fileResult = fileResult.Where(f => nameMustMatch.IsMatch(f.FileName));

        return (await Task.WhenAll(fileResult.Select(async f => (await f.OpenReadAsync(), f.FileName))))
            .Select(f => (new StreamReader(f.Item1, Encoding.Latin1, false, FileBufferSize), f.FileName));
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
