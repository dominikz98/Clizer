namespace CLIzer.Contracts.Design.Tables;

public interface ITableRef<T>
{
    public ITableDefinition<T> Definition { get; }
}
