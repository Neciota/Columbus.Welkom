namespace Columbus.Welkom.Application.Providers
{
    public interface IFileProvider
    {
        Task<StreamReader> GetFileAsync(string filePath);
        Task<IEnumerable<string>> GetFilePathsAsync(string directory, params string[] fileExtensions);
    }
}
