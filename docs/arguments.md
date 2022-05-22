# Argument parsing and validation
You can easily add public properties to your command which, will be filled up with data from passed args.
Just add an CliArg() attribute on the top of your property.

The Properties can also be validated by using common validation attributes.

Three informations can be passed in the CliArg:

## Name
This one defines the name which is necessary to assign the property via args.
Call the arg name with -- prefix to assign the property.

## Shortcut
Don't want to write out the whole name? Just add a shortcut.
Call the arg shortcut with - prefix to assign the property.

## [Helptext](help.md)
Here you can add some custom help text, which will override default generation.

## Example

Calls:
```batch
C:\Users\Spongebob> {{assembly}} --number:1 --force --path:E:\secret_formula.json
```
```batch
C:\Users\Spongebob> {{assembly}} --number:1 -f
```

Command:
```csharp
public class KrabBurgerCmd : ICliCmd
{
    [Range(1, 100)]
    [CliIArg("number")]
    public int Number { get; set; }
    [CliIArg("force", "f")]
    public bool Force { get; set; }

    [CliIArg("path", "p", "Path to my secret formula")]
    public string? Path { get; set; }

    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);
}
```