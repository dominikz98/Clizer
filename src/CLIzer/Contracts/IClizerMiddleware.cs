using CLIzer.Utils;

namespace CLIzer.Contracts
{
    public interface IClizerMiddleware
    {
        Task<ClizerPostAction> Intercept(CommandResolver commandResolver, string[] args, CancellationToken cancellationToken);
    }
}
