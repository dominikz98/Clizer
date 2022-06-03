using CLIzer.Contracts;
using CLIzer.Extensions;

namespace CLIzer.Resolver;

public class NameByAttributeOrDefaultResolver : ICommandNameResolver
{
    public string Resolve<T>() where T : ICliCmd
        => Resolve(typeof(T));

    public string Resolve(Type cmdType)
    {
        var name = cmdType.GetName()?.Value;
        name ??= cmdType.Name;
        return name;
    }
}
