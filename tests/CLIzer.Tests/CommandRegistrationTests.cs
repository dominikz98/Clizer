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

            var clizer = new Clizer();
            clizer.Configure((config) => config

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
            var result = await clizer.Execute(args, default);
            Assert.Equal(100, result);
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
            var result = await clizer.Execute(Array.Empty<string>(), default);
            Assert.Equal(100, result);
        }

        [Fact]
        public async Task Empty()
        {
            var clizer = new Clizer();
            var result = await clizer.Execute(Array.Empty<string>(), default);
            Assert.Equal((int)ClizerExitCodes.SUCCESS, result);
        }
    }

    public class RootCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(100);
    }

    public class GrandparentCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(999);
    }

    public class ParentCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(999);
    }

    public class ChildCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(100);

    }

    public class CompanyCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(999);
    }

    public class DepartmentCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(999);

    }
    public class EmployeeCmd : ICliCmd
    {
        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(100);
    }
}
