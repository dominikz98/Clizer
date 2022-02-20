# Clizer

Use Clizer to easily access your written services/commands via CLI.

The project provides useful functions to make it really easy all the way:
- [Main/sub command handling](docs/commands.md)
- [Easy Configuration](docs/configuration.md)
- [Argument parsing and validation](docs/arguments.md)
- [Custom Configs](docs/custom_configs.md)
- [Help text generation](docs/help.md)
- [ID Mapper](docs/mapper.md)
- [Middlewares](docs/middlewares.md)
- [Dependency Injection](docs/dependency_injection.md)
- [Other Utils](docs/utils.md)
- [Custom Extensions](docs/custom_extensions.md)

## Minimal setup:
```csharp
var clizer = new Clizer()
    .Configure((config) => config
        .RegisterCommands(container => container
            .Root<MainCommand>()));

await clizer.Execute(args.ToArray(), default);

class MainCommand : ICliCmd
{
    public Task<int> Execute(CancellationToken cancellationToken)
        => Task.FromResult((int)ClizerExitCodes.SUCCESS);
}
```