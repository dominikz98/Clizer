using CLIzer.Contracts;
using CLIzer.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests.Aliases
{
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
                .RegisterCommands(cmds => cmds
                    .Command<UnitCommand>("unit")
                    .SubCommand<TestCommand>("test")
                    .SubCommand<AliasesCommand>("aliases")));

            var result = await clizer.Execute(args.ToArray());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        class UnitCommand : ICliCmd
        {
            public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
                => Task.FromResult(ClizerExitCode.HINT);
        }

        class TestCommand : ICliCmd
        {
            public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
                => Task.FromResult(ClizerExitCode.HINT);
        }

        class AliasesCommand : ICliCmd
        {
            public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
                => Task.FromResult(ClizerExitCode.SUCCESS);
        }
    }
}
