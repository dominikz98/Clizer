using CLIzer.Models;

namespace CLIzer.Contracts;

public interface ICliCmd
{
    Task<ClizerExitCode> Execute(CancellationToken cancellationToken);
}

public interface ICliCmd<TParent> : ICliCmd { }

public interface IEmptyCmd : ICliCmd<EmptyCommand> { }
