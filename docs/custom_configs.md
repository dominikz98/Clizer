# Custom Configs
With the RegisterConfig() method in the configuration section a custom config file can be injected in your commands.
Also a command will be created which opens the file when passing its name through args.

There are 2 ways to define how the configuration will be load and saved:

## Relative Path
If you pass in a realtive path, the file will be automatically loaded and parsed from json.
Other formats currently not automatically supported.

## IClizerFileAccessor
Here you go if you want to load your config from databases/storages/... or in another format than json.
[Create a implementation of IClizerFileAccessor](utils.md)

# Example

```batch
C:\Users\Spongebob> {{assembly}} config
```

```csharp
var clizer = new Clizer()
    .Configure((config) => config
        .RegisterConfig<SecretFormularConfig>("config", "secret_formula.json")
        .RegisterCommands((container) => container
            .Root<KrustyKrabCmd>())
    )
);

class SecretFormularConfig
{
    public int ValueOfLove { get; set;}
}

class KrustyKrabCmd 
{
    private readonly SecretFormularConfig? _config;

    public KrustyKrabCmd(IConfig<SecretFormularConfig> config)
    {
        _config = config.Value;
    }

    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        if (_config is null)
            return Task.FromResult(ClizerExitCode.ERROR);

        Console.WriteLine($"Value: {_configuration.Value}");
        return Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
```