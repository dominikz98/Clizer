using CLIzer.Attributes;
using CLIzer.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests;

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
                .RegisterCommands(GetType().Assembly));

        var result = await clizer.Execute(args);
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

[CliName("grandparent")]
public class GrandparentCmd : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);
}

[CliName("parent")]
public class ParentCmd : ICliCmd<GrandparentCmd>
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);
}

[CliName("child")]
public class ChildCmd : ICliCmd<ParentCmd>
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);

}

[CliName("company")]
public class CompanyCmd : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);
}

[CliName("department")]
public class DepartmentCmd : ICliCmd<CompanyCmd>
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);

}

[CliName("employee")]
public class EmployeeCmd : ICliCmd<DepartmentCmd>
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);
}
