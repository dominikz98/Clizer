namespace CLIzer.Contracts
{
    public interface IClizerMiddleware
    {
        Task<ClizerPostAction> Intercept(string[] args, CancellationToken cancellationToken);
    }
}
