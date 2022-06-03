using CLIzer.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.Tests;

public class InMemoryFileAccessor<T> : IClizerDataAccessor<T>
{
    public T? Data { get; set; }
    public string Source => "Unit Test (In memory value)";

    public InMemoryFileAccessor(T? data)
    {
        Data = data;
    }

    public Task<T?> Load(CancellationToken cancellationToken)
       => Task.FromResult(Data);

    public Task Save(T data, CancellationToken cancellationToken)
    {
        Data = data;
        return Task.CompletedTask;
    }
}
