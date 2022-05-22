namespace CLIzer.Contracts
{
    public interface IClizerDataAccessor<T>
    {
        string Source { get; }
        Task<T?> Load(CancellationToken cancellationToken);
        Task Save(T data, CancellationToken cancellationToken);
    }
}
