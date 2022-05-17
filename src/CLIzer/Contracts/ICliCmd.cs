namespace CLIzer.Contracts
{
    public interface ICliCmd
    {
        Task<ClizerExitCode> Execute(CancellationToken cancellationToken);
    }
}
