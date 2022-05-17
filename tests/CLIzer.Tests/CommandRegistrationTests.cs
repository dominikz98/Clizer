using CLIzer.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests
{
    public class CommandRegistrationTests
    {

        [Theory]
        [InlineData("")]
        [InlineData("grandparent;parent;child")]
        [InlineData("company;department;employee")]
        public async Task With_Root_And_Sub_Commands(string rawArgs)
        {
            var args = rawArgs.Split(';');

            var clizer = new Clizer()
                .Configure((config) => config

                .RegisterCommands((container) => container
                    .Root<RootCmd>()
                    .Command<GrandparentCmd>("grandparent")
                        .SubCommand<ParentCmd>("parent")
                            .SubCommand<ChildCmd>("child")
                    .Return()
                    .Command<CompanyCmd>("company")
                        .SubCommand<DepartmentCmd>("department")
                                .SubCommand<EmployeeCmd>("employee")
                )
            );
            var result = await clizer.Execute(args);
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        [Fact]
        public async Task Only_Root_Command()
        {
            var clizer = new Clizer();
            clizer.Configure((config) => config

                .RegisterCommands((container) => container
                    .Root<RootCmd>()
                )
            );
            var result = await clizer.Execute(Array.Empty<string>());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        [Fact]
        public async Task Empty()
        {
            var clizer = new Clizer();
            var result = await clizer.Execute(Array.Empty<string>());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }
    }

    public class RootCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.SUCCESS);
    }

    public class GrandparentCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.ERROR);
    }

    public class ParentCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.ERROR);
    }

    public class ChildCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.SUCCESS);

    }

    public class CompanyCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.ERROR);
    }

    public class DepartmentCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.ERROR);

    }
    public class EmployeeCmd : ICliCmd
    {
        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
            => Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
