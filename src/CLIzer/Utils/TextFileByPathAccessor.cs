using CLIzer.Contracts;

namespace CLIzer.Utils
{
    internal class TextFileByPathAccessor : IClizerFileAccessor<string>
    {
        public string Source { get; }

        public TextFileByPathAccessor(string path)
        {
            Source = path;
        }

        public async Task<string?> Load(CancellationToken cancellationToken)
            => await File.ReadAllTextAsync(Source, cancellationToken);

        public async Task Save(string data, CancellationToken cancellationToken)
            => await File.WriteAllTextAsync(Source, data, cancellationToken);
    }
}
