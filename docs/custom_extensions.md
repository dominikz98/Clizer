# Custom Extensions
Try to make your own configuration classes or extend the configuration class itself to prevent duplicate code/work.

# Example

```csharp
var config = new SingleCommandConfiguration<TheFlyingDutchmanCmd>();
var clizer = new Clizer(config)
    .Configure((customConfig) => customConfig
        .AddLogging());

class SingleCommandConfiguration<TCmd> : ClizerConfiguration where TCmd : ICliCmd
{
    public SingleCommandConfiguration()
    {
        RegisterCommands((container) => container.Root<TCmd>());
    }
}

public static class ClizerConfigurationExtension
{
    public static ClizerConfiguration AddLogging(this ClizerConfiguration config)
        => config.RegisterServices((services) => services.AddLogging(logger => logger.AddConsole().AddDebug()));
}
```