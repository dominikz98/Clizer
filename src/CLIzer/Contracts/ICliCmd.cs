namespace CLIzer.Contracts
{
    public interface ICliCmd
    {
        Task<int> Execute(CancellationToken cancellationToken);
    }
}
