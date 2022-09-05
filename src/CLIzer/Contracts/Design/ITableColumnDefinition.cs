namespace CLIzer.Contracts.Design;

public interface ITableColumnDefinition<T> : ITableCellStyle
{
    public string? Name { get; }
    public Func<T, string> ValueAccessor { get; }
    public Func<T, ITableCellStyle, ITableCellStyle> StyleAccessor { get; }
    public bool CanShrink { get; }
    public bool PadIfPossible { get; }
}

public interface ITableCellStyle
{
    ConsoleColor Color { get; }
    Alignment Alignment { get; }
}