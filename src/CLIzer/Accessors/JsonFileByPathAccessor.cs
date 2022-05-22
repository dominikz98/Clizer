using CLIzer.Contracts;
using System.Text.Json;

namespace CLIzer.Accessors
{
    public class JsonFileByPathAccessor<T> : IClizerDataAccessor<T> where T : class, new()
    {
        public string Source { get; set; }

        public JsonFileByPathAccessor(string path)
        {
            Source = path;
        }

        public async Task<T?> Load(CancellationToken cancellationToken)
        {
            if (Source is null)
                return null;

            using var fs = new FileStream(Source, FileMode.OpenOrCreate);

            if (fs.Length == 0)
                return null;

            var parsed = await JsonSerializer.DeserializeAsync<T>(fs, cancellationToken: cancellationToken);
            if (parsed is null)
                return null;

            return parsed;
        }

        public async Task Save(T data, CancellationToken cancellationToken)
        {
            if (Source is null)
                return;

            using var fs = new FileStream(Source, FileMode.Create);
            await JsonSerializer.SerializeAsync(fs, data, cancellationToken: cancellationToken);
        }
    }
}
