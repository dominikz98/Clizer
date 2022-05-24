namespace CLIzer.Contracts
{
    public interface ICommandNameResolver
    {
        string Resolve(Type cmdType);
        string Resolve<T>() where T : ICliCmd;
    }
}
