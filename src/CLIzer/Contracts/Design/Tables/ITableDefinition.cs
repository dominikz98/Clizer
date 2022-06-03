namespace CLIzer.Contracts.Design.Tables;

public interface ITableDefinition<T>
{
    ITableTitleDefinition? Title { get; }
    ITableColumnDefinition<T>[] ColumnDefinitions { get; }
}