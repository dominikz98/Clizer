using CLIzer.Contracts.Tables;

namespace CLIzer.Design.Tables
{
    public class TableDefinition<T> : ITableDefinition<T> where T : class
    {
        public string? Name { get; set; }
        public ITableColumnDefinition<T>[] ColumnDefinitions { get; set; } = Array.Empty<ITableColumnDefinition<T>>();
    }
}
