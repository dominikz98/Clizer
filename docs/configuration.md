# Configuration
Configure commands/middlewars/... with the Configure() method.
A delegate or an instance of the configuration could be passed.

## Available methods
- [EnableMapping()](mapper.md)
- [RegisterConfig()](custom_configs.md)
- [RegisterMiddleware()](middlewares.md)
- [RegisterServices()](dependency_injection.md)
- [RegisterCommands()](commands.md)
- [EnableHelp()](help.md)
- [EnableAliases()](aliases.md)
- HandleException()

## Exception Handling
With the HandleException() method, a custom exception handling could be passed.
By default, the exception will be rethrowed.

**In Prod:** Catch all of the type ClizerException, these exceptions contains details about invalid configuration and other information about the internal structure!

## Example
```csharp
var clizer = new Clizer()
    .Configure((config) => config

        .RegisterCommands((container) => container
            .Root<KrustyKrabCmd>())

        .RegisterServices((services) => services
            .AddScoped<DriveThroughService > ())

        .RegisterMiddleware<KelpShakeMiddleware>()

        .RegisterConfig<SecretFormularConfiguration>("config", "secret_formula.json")

        .EnableHelp()

        .HandleException((ex) => ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, ex.Message))
    );
);
```