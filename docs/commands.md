# Commands
Commands can be registered with in the Configure() method. All commands will be automatically injected. You can arrange your commands in a hierarchically or flat way.

(Following examples references the setup on the bottom of this document)

## Main
To define commands of this type, let your class implement **ICliCmd**.<br>
If no main command is passed, a empty command will be registered instead.

These commands will be called with the first argument in the call chain (marked red in the following example):

>Call:
>C:\Users\Spongebob> {{assembly}} **<a style="color:red">karbburger</a>**

## Sub
To define commands of this type, let your class implement **ICliCmd\<TParent\>**.<br>
The passed parent type needs to be a implementation of **ICliCmd** or **ICliCmd\<TParent\>**.

These commands can be nested and will be called after the first argument and until arguments or options are passed in the call chain (marked orange in the following example):

>Call:
>C:\Users\Spongebob> {{assembly}} <a style="color:red">karbburger</a> **<a style="color:orange">eat</a>** --all

## Naming
Commands can be named by attaching the **CliName** attribute on the top of your command class.

Alternative you can define a own name resolver by passing an implementation of **ICommandNameResolver** in the command registration method within the configuration section. (Keep in mind, that the CliName attribute will be ignored in this way)

## Example
```csharp
var clizer = new Clizer()
    .Configure((config) => config
    .RegisterCommands(GetType().Assembly));

await clizer.Execute(args.ToArray());


[CliName("krabburger")]
public class KrabBurgerCmd : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);
}

[CliName("eat")]
public class EatCmd : ICliCmd<KrabBurgerCmd>
{
    [CliIArg("all", "a")]
    public bool All { get; set; }

    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.SUCCESS);
}
```