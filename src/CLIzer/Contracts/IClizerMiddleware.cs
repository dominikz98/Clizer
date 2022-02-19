namespace CLIzer.Contracts
{
    public interface IClizerMiddleware
    {
        Task Intercept(string[] args, CancellationToken cancellationToken);
    }
}
