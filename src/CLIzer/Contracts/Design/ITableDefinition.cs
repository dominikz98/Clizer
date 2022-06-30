namespace CLIzer.Contracts.Design;

public interface ITableDefinition<T>
{
    ITableTitleDefinition? Title { get; }
    ITableColumnDefinition<T>[] ColumnDefinitions { get; }
}