using CLIzer.Contracts;

namespace CLIzer.Models
{
    internal class EmptyCommand : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
