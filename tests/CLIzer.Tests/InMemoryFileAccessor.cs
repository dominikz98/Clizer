using CLIzer.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.Tests
{
    public class InMemoryFileAccessor<T> : IClizerFileAccessor<T> where T : class, new()
    {
        public T? Data { get; set; }

        public string? Path => null;

        public InMemoryFileAccessor() { }

        public Task<T?> Load(CancellationToken cancellationToken)
        {
            Data ??= new();
            return Task.FromResult<T?>(Data);
        }

        public Task Save(T data, CancellationToken cancellationToken)
        {
            Data = data;
            return Task.CompletedTask;
        }
    }
}
