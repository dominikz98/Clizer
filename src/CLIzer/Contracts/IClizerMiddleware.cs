using CLIzer.Models;

namespace CLIzer.Contracts;

public interface IClizerMiddleware
{
    Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken);
}
