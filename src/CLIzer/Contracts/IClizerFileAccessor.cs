namespace CLIzer.Contracts
{
    public interface IClizerFileAccessor<T> where T : class, new()
    {
        string? Path { get; }
        Task<T?> Load(CancellationToken cancellationToken);
        Task Save(T data, CancellationToken cancellationToken);
    }
}
