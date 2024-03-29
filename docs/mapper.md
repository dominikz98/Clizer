# ID Mapper
With the EnableMapping() method in the configuration section, an instance of IClizerMapper can be injected in your commands.

With these mapper, entities with complex and long IDs can be (reverse-) mapped into a short four-digit ID.
Guids/Urls/... can be displayed and assigned with a short ID.

The mappings are scope safe and will be stored in passed relative path or data accessor.
Each object type has a separate ID pool, so two objects with equal IDs, but other types have different mappings.

## Example

> C:\Users\Spongebob> {{assembly}} cook

> C:\Users\Spongebob> {{assembly}} eat {{shortId}}

Command:
```csharp
var clizer = new Clizer()
    .Configure((config) => config
    .EnableMapping("mappings.json")
    .RegisterCommands(GetType().Assembly));

[CliName("cook")]
public class CookCmd : ICliCmd
{
    private readonly IClizerMapper _mapper;

    public CookCmd(IClizerMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        KrabBurger burger = await CookBurger(cancellationToken);
        var mapping = await _mapper.MapId(entity, x => x.Id, 10, cancellationToken);
        Console.WriteLine($"Burger cooked: {mapping.ShortId}");
        return ClizerExitCode.SUCCESS;
    }
}

[CliName("eat")]
public class EatCmd : ICliCmd
{
    [Range(1000, 9999)]
    [CliIArg("burger")]
    public int BurgerShortId { get; set; }

    private readonly IClizerMapper _mapper;

    public EatCmd(IClizerMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        var burgerId = _mapper.GetByShortId<KrabBurger>(mapping.ShortId);
        KrabBurger burger = await ReceiveBurger(burgerId, cancellationToken);
        Console.WriteLine($"Burger eaten: {burger.Yummi}");
    }
}
```