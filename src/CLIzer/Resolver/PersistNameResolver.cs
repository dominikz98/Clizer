using CLIzer.Contracts;

namespace CLIzer.Resolver;

internal class PersistNameResolver : ICommandNameResolver
{
    public Type CmdType { get; }
    public string Name { get; }
    public ICommandNameResolver Fallback { get; }

    public PersistNameResolver(Type type, string name, ICommandNameResolver fallback)
    {
        CmdType = type;
        Name = name;
        Fallback = fallback;
    }

    public string Resolve<T>() where T : ICliCmd
        => Resolve(typeof(T));

    public string Resolve(Type cmdType)
    {
        if (cmdType != CmdType)
            return Fallback.Resolve(cmdType);

        return Name;
    }
}
