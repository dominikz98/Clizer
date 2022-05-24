# Clizer

Use Clizer to easily access your written services/commands via CLI.
<a href="https://img.shields.io/nuget/v/clizer?label=Clizer"></a>

[![NuGet](https://img.shields.io/nuget/v/clizer?label=Clizer)](https://www.nuget.org/packages/CLIzer/)

The project provides useful functions to make it really easy all the way:
- :rocket: [Main/sub command handling](docs/commands.md)
- :hammer: [Easy Configuration](docs/configuration.md)
- :heavy_check_mark: [Argument parsing and validation](docs/arguments.md)
- :bookmark: [Custom Configs](docs/custom_configs.md)
- :book: [Help text generation](docs/help.md)
- :keyboard: [User configurable aliases](docs/aliases.md)
- :link: [ID Mapper](docs/mapper.md)
- :shield: [Middlewares](docs/middlewares.md)
- :syringe: [Dependency Injection](docs/dependency_injection.md)
- :toolbox: [Other Utils](docs/utils.md)
- [Custom Extensions](docs/custom_extensions.md)

# Minimal setup:
```csharp
var clizer = new Clizer()
    .Configure((config) => config
        .RegisterCommands(GetType().Assembly));

await clizer.Execute(args);

class Command : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        => Task.FromResult(ClizerExitCode.ERROR);
}
```
