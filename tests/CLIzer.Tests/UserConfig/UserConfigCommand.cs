using CLIzer.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.Tests.UserConfig;

public class UserConfigCommand : ICliCmd
{
    private readonly TestUserConfiguration? _configuration;

    public UserConfigCommand(IConfig<TestUserConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        if (_configuration is null)
            return Task.FromResult(ClizerExitCode.ERROR);

        Console.WriteLine($"Value: {_configuration.Value}");
        return Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
