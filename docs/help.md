# Help
With the EnableHelp() method in the configuration section, automatically generated help information can be displayed about your commands.

If help is passed in args, the following sections will be printed about the called command:
- Commands
- Arguments
- Options

To overwrite the displayed help text, fill up the help text parameter in CliIArgAttribute constructor.

# Example

Calls:
```batch
C:\Users\Spongebob> {{assembly}} burger help
```
```batch
C:\Users\Spongebob> {{assembly}} burger --help
```
```batch
C:\Users\Spongebob> {{assembly}} burger -h
```

Command:
```csharp
var clizer = new Clizer()
    .Configure((config) => config
        .EnableHelp()
        .RegisterCommands((container) => container
            .Command<KrabBurgerCmd>("burger"))
    )
);

public class KrabBurgerCmd : ICliCmd
{
    [Range(1, 100)]
    [CliIArg("number")]
    public int Number { get; set; }

    [CliIArg("path", "p", "Path to my secret formula")]
    public string? Path { get; set; }

    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);
}
```