using CLIzer.Contracts;

namespace CLIzer.Models;

public class EmptyCommand : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);
}
