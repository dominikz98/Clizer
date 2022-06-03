using CLIzer.Contracts.Design;
using CLIzer.Contracts.Tables;

namespace CLIzer.Design.Tables
{
    public class TableColumnDefinition<T> : ITableColumnDefinition<T> where T : class
    {
        public string? Name { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.White;
        public Alignment Alignment { get; set; } = Alignment.Start;
        public Func<T, string> ValueAccessor { get; set; } = (x) => string.Empty;
        public bool CanShrink { get; set; } = true;
        public bool PadIfPossible { get; set; } = true;

        public TableColumnDefinition() { }

        public TableColumnDefinition(string name)
        {
            Name = name;
        }

        public TableColumnDefinition(string name, Func<T, string> valueAccessor)
        {
            Name = name;
            ValueAccessor = valueAccessor;
        }
    }
}
