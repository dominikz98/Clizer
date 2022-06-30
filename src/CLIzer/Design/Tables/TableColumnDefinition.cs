using CLIzer.Contracts.Design;

namespace CLIzer.Design.Tables;

public class TableColumnDefinition<T> : ITableColumnDefinition<T> where T : class
{
    public string? Name { get; set; }
    public ConsoleColor Color { get; set; } = Console.ForegroundColor;
    public Alignment Alignment { get; set; } = Alignment.Start;
    public bool CanShrink { get; set; } = true;
    public bool PadIfPossible { get; set; } = true;
    public Func<T, string> ValueAccessor { get; set; } = (x) => string.Empty;
    public Func<T, ITableCellStyle, ITableCellStyle> StyleAccessor { get; set; }

    public TableColumnDefinition() : this(string.Empty) { }

    public TableColumnDefinition(string name)
    {
        Name = name;
        StyleAccessor = (_, style) => this;
    }

    public TableColumnDefinition(string name, Func<T, string> valueAccessor)
    {
        Name = name;
        ValueAccessor = valueAccessor;
        StyleAccessor = (_, style) => this;
    }
}
