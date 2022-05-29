# Middlewares
With the RegisterMiddleware() method in the configuration section, you can register your custom middlewares.

The middleware will be automatically  injected and executed before the command gets called.
The project itself uses middlewares to provide functionality like [help text](help.md) generation and opening [custom configs](custom_configs.md).

Use this option to call routines and prevent boiler code which would be executed at the start of your commands.

## CommandContext
This context provides all resolved information about the call that will be executed and the callchain.

## Example
```csharp
public class EpicFileLoader : IClizerMiddleware
{
    private readonly IClizerDataAccessor<SecretFormula> _accessor;
    private readonly IBurgerFactory _burgerFactory;

    public EpicFileLoader(IClizerDataAccessor<SecretFormula> accessor, IBurgerFactory burgerFactory)
    {
        _accessor = accessor;
        _burgerFactory = burgerFactory;
    }

    public async Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken)
    {
        var data = await _accessor.Load(cancellationToken);
        if (data is null)
            return ClizerPostAction.EXIT;

        _burgerFactory.Love(data);
        return ClizerPostAction.CONTINUE;
    }
}
```