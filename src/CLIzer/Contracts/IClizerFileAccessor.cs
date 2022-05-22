namespace CLIzer.Contracts
{
    public interface IClizerFileAccessor<T>
    {
        string Source { get; }
        Task<T?> Load(CancellationToken cancellationToken);
        Task Save(T data, CancellationToken cancellationToken);
    }
}
