namespace CLIzer.Contracts.Tables
{
    public interface ITableDefinition<T>
    {
        public string? Name { get; }
        ITableColumnDefinition<T>[] ColumnDefinitions { get; }
    }
}
