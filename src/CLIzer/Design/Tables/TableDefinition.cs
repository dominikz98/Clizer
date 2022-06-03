using CLIzer.Contracts.Design.Tables;

namespace CLIzer.Design.Tables;

public class TableDefinition<T> : ITableDefinition<T> where T : class
{
    public ITableTitleDefinition? Title { get; set; }
    public ITableColumnDefinition<T>[] ColumnDefinitions { get; set; } = Array.Empty<ITableColumnDefinition<T>>();
}
