using CLIzer.Contracts;

namespace CLIzer.Models
{
    internal class EmptyCommand : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
