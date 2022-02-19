using CLIzer.Contracts;
using System.Text.Json;

namespace CLIzer.Utils
{
    public class JsonFileByRelativePathAccessor<T> : IClizerFileAccessor<T> where T : class, new()
    {
        public string RelativePath { get; }

        public string? Path => System.IO.Path.Combine(Environment.CurrentDirectory, RelativePath);

        public JsonFileByRelativePathAccessor(string relativePath)
        {
            RelativePath = relativePath;
        }

        public async Task<T?> Load(CancellationToken cancellationToken)
        {
            if (Path is null)
                return null;

            using var fs = new FileStream(Path, FileMode.OpenOrCreate);

            if (fs.Length == 0)
                return null;

            var parsed = await JsonSerializer.DeserializeAsync<T>(fs, cancellationToken: cancellationToken);
            if (parsed is null)
                return null;

            return parsed;
        }

        public async Task Save(T data, CancellationToken cancellationToken)
        {
            if (Path is null)
                return;

            using var fs = new FileStream(Path, FileMode.Create);
            await JsonSerializer.SerializeAsync(fs, data, cancellationToken: cancellationToken);
        }
    }
}
