# Middlewares
With the RegisterMiddleware() method in the configuration section, you can register your custom middlewares.

The middleware will be automatically  injected and executed before the command gets called.
The project itself uses middlewares to provide functionality like [help text](help.md) generation and opening [custom configs](custom_configs.md).

Use this option to call routines and prevent boiler code which would be executed at the start of your commands.

The following parameters are passed in the middleware:

## CommandResolver
With this resolver, you can access the command which will be called after the middleware.

## Args
The remaining args which will be applied to the command.

# Example
```csharp
class EpicFileLoader : IClizerMiddleware
{
    private readonly IClizerFileAccessor<SecretFormula> _accessor;
    private readonly IBurgerFactory _burgerFactory;

    public EpicFileLoader(IClizerFileAccessor<SecretFormula> accessor, IBurgerFactory burgerFactory)
    {
        _accessor = accessor;
        _burgerFactory = burgerFactory;
    }

    public async Task<ClizerPostAction> Intercept(CommandResolver commandResolver, string[] args, CancellationToken cancellationToken)
    {
        var data = await _accessor.Load(cancellationToken);
        if (data is null)
            return ClizerPostAction.EXIT;

        _burgerFactory.Love(data);
        return ClizerPostAction.CONTINUE;
    }
}
```