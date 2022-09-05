using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests.Aliases;

public class General
{
    [Fact]
    public async Task Runtime()
    {
        var args = new List<string>()
        {
            "ut"
        };

        var aliases = "ut = unit test al \r\n al = aliases";
        var fileAccessor = new InMemoryFileAccessor<string>(aliases);
        var clizer = new Clizer()
            .Configure(config => config
            .EnableAliases(fileAccessor)
            .RegisterCommands(GetType().Assembly));

        var result = await clizer.Execute(args.ToArray());
        Assert.Equal(ClizerExitCode.SUCCESS, result);
    }

    [CliName("unit")]
    class UnitCommand : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.HINT);
    }

    [CliName("test")]
    class TestCommand : ICliCmd<UnitCommand>
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.HINT);
    }

    [CliName("aliases")]
    class AliasesCommand : ICliCmd<TestCommand>
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
