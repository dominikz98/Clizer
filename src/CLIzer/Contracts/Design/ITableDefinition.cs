namespace CLIzer.Contracts.Design;

public interface ITableDefinition<T> : IDesignComponent
{
    ITableTitleDefinition? Title { get; }
    ITableColumnDefinition<T>[] ColumnDefinitions { get; }
}