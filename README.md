# Clizer

Use Clizer to easily access your written services/commands via CLI.

The project provides useful functions to make it really easy all the way:
- :rocket: [Main/sub command handling](docs/commands.md)
- :hammer: [Easy Configuration](docs/configuration.md)
- :heavy_check_mark: [Argument parsing and validation](docs/arguments.md)
- :bookmark: [Custom Configs](docs/custom_configs.md)
- :book: [Help text generation](docs/help.md)
- :anchor: [User configurable aliases](docs/aliases.md)
- :link: [ID Mapper](docs/mapper.md)
- :shield: [Middlewares](docs/middlewares.md)
- :syringe: [Dependency Injection](docs/dependency_injection.md)
- :toolbox: [Other Utils](docs/utils.md)
- [Custom Extensions](docs/custom_extensions.md)

# Minimal setup:
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