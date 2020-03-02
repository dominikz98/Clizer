# Clizer

## Example setup:
``` c#
public class Program : ICliCmd
{
    public static async Task Main(string[] args)
    {
        var clizer = new Clizer.Clizer();
        var dependencies = new Container();
        dependencies.Register<ITestService, TestService>();

        clizer.Configure()
            .AddCommandContainer(new CommandContainer(typeof(Program), "Simple helptext!")
                .Register<Program, SubCommand>("sub"))
            .AddDependencyContainer(dependencies);

        var result = await clizer.Execute(args.ToArray(), default);
    }
    public Task<int> Execute(CancellationToken cancellationToken)
        => Task.FromResult((int)ClizerExitCodes.SUCCESS);
}

public class SubCommand : ICliCmd
{
    [Required]
    [MinLength(2)]
    [CliIArg("path", "p", "Pass io path.")]
    public string Path { get; set; }

    private readonly ITestService _testService;

    public SubCommand(ITestService testService)
        => _testService = testService;

    public Task<int> Execute(CancellationToken cancellationToken)
    {
        _testService.Print(Path);
        return Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
```

How to call subcommand:
```bat
Call: > assembly sub --path:"C:\\users"
```