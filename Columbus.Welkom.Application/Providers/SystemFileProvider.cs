using System.Text;

namespace Columbus.Welkom.Application.Providers
{
    public class SystemFileProvider : IFileProvider
    {
        public Task<StreamReader> GetFileAsync(string filePath)
        {
            FileStream file = File.OpenRead(filePath);

            return Task.FromResult(new StreamReader(file, Encoding.Latin1, false, 10_000_000));
        }

        public Task<IEnumerable<string>> GetFilePathsAsync(string directory, params string[] fileExtensions)
        {
            IEnumerable<string> paths = Directory.EnumerateFiles(directory)
                .Where(path => fileExtensions.Contains(Path.GetExtension(path)));

            return Task.FromResult(paths);
        }
    }
}
